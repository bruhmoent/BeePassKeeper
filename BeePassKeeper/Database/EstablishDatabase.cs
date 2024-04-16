using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using BeePassKeeper.Models;
using BeePassKeeper.Database.Models;
using System.Linq;

namespace BeePassKeeper.Database
{
    class EstablishDatabase
    {
        private const string DatabaseFileName = "BeePassKeeper.db";

        private static SQLiteAsyncConnection _database;

        public static SQLiteAsyncConnection Database
        {
            get
            {
                if (_database == null)
                {
                    _database = CreateDatabaseConnection();
                }
                return _database;
            }
        }

        private static SQLiteAsyncConnection CreateDatabaseConnection()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFileName);
           
            return new SQLiteAsyncConnection(databasePath);
        }

        public static async Task InitializeDatabaseAsync()
        {
            var connection = Database;

            await connection.CreateTableAsync<PasswordModel>().ConfigureAwait(false);
            await connection.CreateTableAsync<EncryptionKeyModel>().ConfigureAwait(false);

            var existingKeysQuery = await connection.Table<EncryptionKeyModel>().ToListAsync().ConfigureAwait(false);
            var existingKeys = existingKeysQuery.Select(k => k.Key).ToList();

            // For debugging.
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFileName);
            File.Copy(databasePath, Path.Combine("/storage/emulated/0/Documents", DatabaseFileName), true);
        }

    }
}
