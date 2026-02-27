using System.Data;
using AdvancedFullProject.Application.Abstractions.Persistence;
using AdvancedFullProject.Application.Common.Models;
using AdvancedFullProject.Domain.Entities;
using AdvancedFullProject.Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;

namespace AdvancedFullProject.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly SqlConnectionFactory _factory;
    public EmployeeRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<Guid> CreateAsync(Employee entity, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Employees_Create", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@FirstName", entity.FirstName);
        command.Parameters.AddWithValue("@LastName", entity.LastName);
        command.Parameters.AddWithValue("@Email", entity.Email);
        command.Parameters.AddWithValue("@Department", entity.Department);
        command.Parameters.AddWithValue("@Salary", entity.Salary);
        command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy);
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Employees_GetById", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync(cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return Map(reader);
    }

    public async Task<PagedResult<Employee>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Employees_GetPaged", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@PageNumber", request.PageNumber);
        command.Parameters.AddWithValue("@PageSize", request.PageSize);
        command.Parameters.AddWithValue("@Filter", request.Filter ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@SortBy", request.SortBy ?? "CreatedAt");
        command.Parameters.AddWithValue("@Descending", request.Descending);
        await connection.OpenAsync(cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var list = new List<Employee>();
        while (await reader.ReadAsync(cancellationToken)) list.Add(Map(reader));
        var totalCount = 0;
        if (await reader.NextResultAsync(cancellationToken) && await reader.ReadAsync(cancellationToken)) totalCount = reader.GetInt32(0);
        return new PagedResult<Employee> { Items = list, TotalCount = totalCount, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }

    public async Task SoftDeleteAsync(Guid id, string user, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Employees_SoftDelete", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@DeletedBy", user);
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(Employee entity, CancellationToken cancellationToken)
    {
        using var connection = (SqlConnection)_factory.CreateConnection();
        using var command = new SqlCommand("sp_Employees_Update", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@FirstName", entity.FirstName);
        command.Parameters.AddWithValue("@LastName", entity.LastName);
        command.Parameters.AddWithValue("@Email", entity.Email);
        command.Parameters.AddWithValue("@Department", entity.Department);
        command.Parameters.AddWithValue("@Salary", entity.Salary);
        command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy ?? "system");
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static Employee Map(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        LastName = reader.GetString(reader.GetOrdinal("LastName")),
        Email = reader.GetString(reader.GetOrdinal("Email")),
        Department = reader.GetString(reader.GetOrdinal("Department")),
        Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy"))
    };
}
