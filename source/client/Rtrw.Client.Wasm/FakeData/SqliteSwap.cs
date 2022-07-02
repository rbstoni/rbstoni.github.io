using Microsoft.Data.Sqlite;

namespace Rtrw.Client.Wasm.FakeData
{
    public interface ISqliteSwap
    {
        void DoSwap(string srcFilename, string destFilename);
    }

    public class SqliteSwap : ISqliteSwap
    {
        public void DoSwap(string srcFilename, string destFilename)
        {
            using var src = new SqliteConnection($"Data Source={srcFilename}");
            using var tgt = new SqliteConnection($"Data Source={destFilename}");

            src.Open();
            tgt.Open();

            src.BackupDatabase(tgt);

            tgt.Close();
            src.Close();
        }
    }
}
