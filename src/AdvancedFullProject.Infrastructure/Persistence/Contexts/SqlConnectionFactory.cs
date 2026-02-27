using System.Data;
using AdvancedFullProject.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace AdvancedFullProject.Infrastructure.Persistence.Contexts;

public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(IOptions<DatabaseOptions> options) => _connectionString = options.Value.ConnectionString;
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
