using System.Data;
using AdvancedFullProject.Application.Abstractions.Persistence;
using AdvancedFullProject.Domain.Entities;
using AdvancedFullProject.Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;

namespace AdvancedFullProject.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _factory;
    public UserRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Users_Create", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@Role", user.Role);
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return user.Id;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Users_GetById", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync(cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return Map(reader);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Users_GetByUsername", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Username", username);
        await connection.OpenAsync(cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return Map(reader);
    }

    private static User Map(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Username = reader.GetString(reader.GetOrdinal("Username")),
        Email = reader.GetString(reader.GetOrdinal("Email")),
        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
        Role = reader.GetString(reader.GetOrdinal("Role"))
    };
}
