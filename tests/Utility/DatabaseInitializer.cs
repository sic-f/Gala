using Microsoft.Data.SqlClient;
using Respawn;

namespace Utility;

public class DatabaseInitializer
{
    public static async Task ResetDatabase(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });

        await respawner.ResetAsync(connection);
    }
}