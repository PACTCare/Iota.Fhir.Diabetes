namespace Pact.Fhir.Api.Services
{
  using System.Data.SQLite;
  using System.IO;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.SqlLite;

  using Task = System.Threading.Tasks.Task;

  public class SqlLiteGlucoseCache
  {
    public SqlLiteGlucoseCache()
    {
      var databaseFilename = "glucosecache.sqlite";
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      this.Init(databaseFilename);
    }

    private string ConnectionString { get; set; }

    private void Init(string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand("CREATE TABLE GlucoseCache (Id TEXT NOT NULL PRIMARY KEY, Payload TEXT NULL)", connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }

    public async Task WriteDataAsync(string id, Bundle bundle)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = new SQLiteCommand("INSERT OR IGNORE INTO GlucoseCache (Id, Payload) VALUES (@Id, @Payload)", connection))
        {
          command.AddWithValue("Id", id);
          command.AddWithValue("Payload", bundle.ToJson());

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    public async Task DeleteDataAsync(string id)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = new SQLiteCommand("DELETE FROM GlucoseCache WHERE Id = @Id", connection))
        {
          command.AddWithValue("Id", id);
          await command.ExecuteNonQueryAsync();
        }
      }
    }

    public async Task<Bundle> ReadDataAsync(string id)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = new SQLiteCommand("SELECT Payload FROM GlucoseCache WHERE Id = @Id", connection))
        {
          command.AddWithValue("Id", id);
          var result = await command.ExecuteScalarAsync();
          return result == null ? null : new FhirJsonParser().Parse<Bundle>(result as string);
        }
      }
    }
  }
}