using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Net.Mail;
using System.Reflection;

namespace RandomCodeReviewPlanner
{
    class Program
    {
        public static void Main()
        {
            string[] developers = GetDevelopers();

            Randomize(developers);

            string output = GenerateOutput(developers);

            string action = ConfigurationManager.AppSettings["Action"];

            switch (action)
            {
                case "Trac" :
                    SaveToDatabase(output);
                    break;
                case "Stdout":
                    Console.Out.WriteLine(output);
                    break;
                case "Email":
                    SendMail(output);
                    break;
            }

        }

        private static void SendMail(string output)
        {
            var smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
            var smtpClient = new SmtpClient(smtpServer);
            var from = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
            var to = new MailAddress(ConfigurationManager.AppSettings["MailTo"]);
            var message = new MailMessage(from, to)
                              {
                                  Body = output,
                                  Subject = "Code Review Assignment"
                              };
            smtpClient.Send(message);
        }

        private static void SaveToDatabase(string output)
        {
            var connectionSettings = ConfigurationManager.ConnectionStrings["TracDatabase"];
            string connectionString = connectionSettings.ConnectionString;
            string connectionProvider = connectionSettings.ProviderName;
            var assemblyName = connectionProvider.Split(new[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries)[1];
            Assembly.Load(new AssemblyName(assemblyName));
            var connectionClass = Type.GetType(connectionProvider);
            string wikiPage = ConfigurationManager.AppSettings["WikiPage"];
            using (DbConnection conn = (DbConnection)Activator.CreateInstance(connectionClass, connectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "select max(version) from wiki where name=@name";

                AddParameter(command, "name", DbType.String, wikiPage);
                object value = command.ExecuteScalar();
                int version = (value == DBNull.Value) ? 1 : Convert.ToInt32(value) + 1;
                int seconds = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                AddParameter(command, "version", DbType.Int32, version);
                AddParameter(command, "time", DbType.Int32, seconds);
                AddParameter(command, "text", DbType.String, output);
                command.CommandText =
                    "insert into wiki(name, version, time, author, ipnr, text, comment, readonly) " +
                    "values (@name, @version, @time, 'RandomCodeReviewPlanner', '127.0.0.1', @text, '', 0)";
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        private static void AddParameter(DbCommand command, string name, DbType type, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Value = value;
            command.Parameters.Add(param);
        }

        private static string GenerateOutput(string[] developers)
        {
            string format = ConfigurationManager.AppSettings["OutputTemplate"];
            object[] param = new object[developers.Length + 1];

            int index = 0;
            param[index++] = DateTime.Now;
            foreach (string developer in developers)
            {
                param[index++] = developer;
            }

            return string.Format(format, param);
        }

        private static void Randomize(string[] developers)
        {
            List<string> copy = new List<string>(developers);

            Random random = new Random();
            int index = developers.Length;
            while (index > 0)
            {
                int pick = random.Next(index);
                developers[--index] = copy[pick];
                copy.RemoveAt(pick);
            }
        }

        private static string[] GetDevelopers()
        {
            string developerSetting = ConfigurationManager.AppSettings["Developers"];
            string[] developers = developerSetting.Split(
                new char[]{',', ';'}, StringSplitOptions.RemoveEmptyEntries );
            for(int i = 0; i<developers.Length; i++)
            {
                developers[i] = developers[i].Trim();
            }
            return developers;
        }
    }
}
