using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace RandomCodeReviewPlanner
{
    class Program
    {
        public static void Main()
        {
            string[] developers = GetDevelopers();

            Randomize(developers);

            string output = GenerateOutput(developers);

            SaveToDatabase(output);
        }

        private static void SaveToDatabase(string output)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TracDatabase"].ConnectionString;
            string wikiPage = ConfigurationManager.AppSettings["WikiPage"];
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "select max(version) from wiki where name=@name";
                command.Parameters.Add(
                    new SQLiteParameter("name", DbType.String) {Value = wikiPage});
                object value = command.ExecuteScalar();
                int version = (value == DBNull.Value) ? 1 : Convert.ToInt32(value) + 1;
                int seconds = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                command.Parameters.Add(
                    new SQLiteParameter("version", DbType.Int32) {Value = version});
                command.Parameters.Add(
                    new SQLiteParameter("time", DbType.Int32) {Value = seconds});
                command.Parameters.Add(
                    new SQLiteParameter("text", DbType.String) {Value = output});
                command.CommandText =
                    "insert into wiki(name, version, time, author, ipnr, text, comment, readonly) " +
                    "values (@name, @version, @time, 'RandomCodeReviewPlanner', '127.0.0.1', @text, '', 0)";
                command.ExecuteNonQuery();
                conn.Close();
            }
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
