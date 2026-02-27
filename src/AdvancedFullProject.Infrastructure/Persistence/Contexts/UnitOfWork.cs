using System.Data;
using AdvancedFullProject.Application.Abstractions.Persistence;
using Microsoft.Data.SqlClient;

namespace AdvancedFullProject.Infrastructure.Persistence.Contexts;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SqlConnectionFactory _factory;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public UnitOfWork(SqlConnectionFactory factory) => _factory = factory;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _connection = (SqlConnection)_factory.CreateConnection();
        await _connection.OpenAsync(cancellationToken);
        _transaction = (SqlTransaction)await _connection.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction is not null) await _transaction.CommitAsync(cancellationToken);
        await DisposeAsync();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transaction is not null) await _transaction.RollbackAsync(cancellationToken);
        await DisposeAsync();
    }

    private async Task DisposeAsync()
    {
        if (_transaction is not null) await _transaction.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
        _transaction = null; _connection = null;
    }
}
