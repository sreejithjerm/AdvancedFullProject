using System.Data;
using AdvancedFullProject.Application.Abstractions.Persistence;
using AdvancedFullProject.Domain.Entities;
using AdvancedFullProject.Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;

namespace AdvancedFullProject.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SqlConnectionFactory _factory;
    public RefreshTokenRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_RefreshTokens_Get", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Token", token);
        await connection.OpenAsync(cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return new RefreshToken
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            Token = reader.GetString(reader.GetOrdinal("Token")),
            ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
            IsRevoked = reader.GetBoolean(reader.GetOrdinal("IsRevoked"))
        };
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_RefreshTokens_Revoke", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Token", token);
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SaveAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_RefreshTokens_Save", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", token.Id);
        command.Parameters.AddWithValue("@UserId", token.UserId);
        command.Parameters.AddWithValue("@Token", token.Token);
        command.Parameters.AddWithValue("@ExpiresAt", token.ExpiresAt);
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
