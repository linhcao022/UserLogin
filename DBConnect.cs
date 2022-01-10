using App1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public static class DBConnect
    {
        public async static void InitializeDatabase()
        {
            await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("users.db", Windows.Storage.CreationCollisionOption.OpenIfExists);

            string dbpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "users.db");

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbpath};Version=3"))
            {
                await connection.OpenAsync();
                String tableCommand = "CREATE TABLE IF NOT EXISTS User (id INTEGER PRIMARY KEY, username nvarchar(10), password nvarchar(10))";
                SQLiteCommand createTable = new SQLiteCommand(tableCommand, connection);

                createTable.ExecuteReader();
                connection.Close();
            }
        }

        public async static void AddData(string username, string password)
        {
            string dbpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "users.db");
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbpath};Version=3"))
            {
                await connection.OpenAsync();

                SQLiteCommand insertCommand = new SQLiteCommand();
                insertCommand.Connection = connection;

                insertCommand.CommandText = "INSERT INTO Money User (NULL, @username, @password);";
                insertCommand.Parameters.AddWithValue("@username", username);
                insertCommand.Parameters.AddWithValue("@password", password);

                insertCommand.ExecuteReaderAsync();

                connection.Close();
            }
        }

        public async static Task LoadRecordAsync(ObservableCollection<User> items)
        {
            string dbpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "users.db");
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbpath};Version=3"))
            {
                await connection.OpenAsync();
                SQLiteCommand command = new SQLiteCommand("Select * FROM User", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var usernameOrdinal = reader.GetOrdinal("username");
                    var passwordOrdinal = reader.GetOrdinal("password");

                    while (await reader.ReadAsync())
                    {
                        string username = reader.GetString(usernameOrdinal);
                        string password = reader.GetString(passwordOrdinal);

                        items.Add( new User(username, password));
                    };
                }

            }
        }
    }
}
