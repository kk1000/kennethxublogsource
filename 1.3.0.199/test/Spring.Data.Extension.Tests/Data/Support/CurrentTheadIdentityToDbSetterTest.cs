using System.Data.SQLite;
using System.Security.Principal;
using NUnit.Framework;
using System.Data;
using System.Threading;

namespace Spring.Data.Support
{
    [TestFixture] class CurrentTheadIdentityToDbSetterTest
    {
        private CurrentTheadIdentityToDbSetter _testee;
        private const string _testUserName = "TestName";

        [SetUp] public void SetUp()
        {
            _testee = new CurrentTheadIdentityToDbSetter();
        }

        [Test] public void AfterConnectionOpen_ExecuteCommand_WithoutParameterName()
        {
            using (SQLiteConnection connection = NewConnection())
            {
                connection.Open();
                SetupDataBase(connection);

                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(_testUserName), null);
                _testee.CommandText = "insert into user values(?)";
                _testee.CommandType = CommandType.Text;
                _testee.AfterConnectionOpen(connection);

                ValidateResult(connection);
                connection.Close();
            }
        }

        [Test] public void AfterConnectionOpen_ExecuteCommand_WithParameterName()
        {
            using (SQLiteConnection connection = NewConnection())
            {
                connection.Open();
                SetupDataBase(connection);

                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(_testUserName), null);
                _testee.CommandText = "insert into user values(@username)";
                _testee.CommandType = CommandType.Text;
                _testee.ParameterName = "username";
                _testee.AfterConnectionOpen(connection);

                ValidateResult(connection);
                connection.Close();
            }
        }

        private SQLiteConnection NewConnection()
        {
            return new SQLiteConnection("Data source=:memory:");
        }

        private void ValidateResult(SQLiteConnection connection)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "select * from user";
                var reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read(), "Must have at lease one row");
                Assert.AreEqual(_testUserName, reader.GetString(0));
            }
        }

        private void SetupDataBase(SQLiteConnection connection)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "create table user (name varchar(10))";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }
    }
}
