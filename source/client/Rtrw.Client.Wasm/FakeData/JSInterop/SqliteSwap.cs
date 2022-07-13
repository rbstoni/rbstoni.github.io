using Microsoft.Data.Sqlite;

namespace Rtrw.Client.Wasm.FakeData.JSInterop
{
    public interface ISqliteSwap
    {
        void DoSwap(string sourceFilename, string destinationFilename);
    }

    public class SqliteSwap : ISqliteSwap
    {
        public void DoSwap(string sourceFilename, string destinationFilename)
        {
            using var source = new SqliteConnection($"Data Source={sourceFilename}");
            using var target = new SqliteConnection($"Data Source={destinationFilename}");

            source.Open();
            target.Open();

            source.BackupDatabase(target);

            target.Close();
            source.Close();
        }
    }
}
