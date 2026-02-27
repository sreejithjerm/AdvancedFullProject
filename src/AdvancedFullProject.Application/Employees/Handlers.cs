using AdvancedFullProject.Application.Abstractions.Caching;
using AdvancedFullProject.Application.Abstractions.Messaging;
using AdvancedFullProject.Application.Abstractions.Persistence;
using AdvancedFullProject.Application.Abstractions.Webhook;
using AdvancedFullProject.Application.Common.Models;
using AdvancedFullProject.Application.Employees.Commands;
using AdvancedFullProject.Application.Employees.DTOs;
using AdvancedFullProject.Application.Employees.Queries;
using AdvancedFullProject.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AdvancedFullProject.Application.Employees;

public sealed class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IRabbitMqPublisher _publisher;
    private readonly IWebhookSender _webhook;

    public CreateEmployeeHandler(IEmployeeRepository repo, IUnitOfWork uow, IMapper mapper, IRabbitMqPublisher publisher, IWebhookSender webhook)
    {
        _repo = repo; _uow = uow; _mapper = mapper; _publisher = publisher; _webhook = webhook;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = new Employee { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, Department = request.Department, Salary = request.Salary };
        await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            employee.Id = await _repo.CreateAsync(employee, cancellationToken);
            await _uow.CommitAsync(cancellationToken);
            await _publisher.PublishAsync("employee.created", new { employee.Id, employee.Email });
            await _webhook.SendAsync("employee.created", employee, cancellationToken);
            return _mapper.Map<EmployeeDto>(employee);
        }
        catch
        {
            await _uow.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

public sealed class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _repo;
    private readonly IMapper _mapper;
    public UpdateEmployeeHandler(IEmployeeRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _repo.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException("Employee not found");
        employee.FirstName = request.FirstName; employee.LastName = request.LastName; employee.Email = request.Email; employee.Department = request.Department; employee.Salary = request.Salary;
        employee.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(employee, cancellationToken);
        return _mapper.Map<EmployeeDto>(employee);
    }
}

public sealed class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _repo;
    public DeleteEmployeeHandler(IEmployeeRepository repo) => _repo = repo;
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken) => await _repo.SoftDeleteAsync(request.Id, request.DeletedBy, cancellationToken);
}

public sealed class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _repo; private readonly ICacheService _cache; private readonly IMapper _mapper;
    public GetEmployeeByIdHandler(IEmployeeRepository repo, ICacheService cache, IMapper mapper) { _repo = repo; _cache = cache; _mapper = mapper; }
    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var key = $"employee:{request.Id}";
        var cached = await _cache.GetAsync<EmployeeDto>(key, cancellationToken);
        if (cached is not null) return cached;
        var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return null;
        var dto = _mapper.Map<EmployeeDto>(entity);
        await _cache.SetAsync(key, dto, TimeSpan.FromMinutes(5), cancellationToken);
        return dto;
    }
}

public sealed class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IEmployeeRepository _repo; private readonly IMapper _mapper;
    public GetEmployeesHandler(IEmployeeRepository repo, IMapper mapper){_repo=repo;_mapper=mapper;}
    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var paged = await _repo.GetPagedAsync(request.Request, cancellationToken);
        return new PagedResult<EmployeeDto>
        {
            Items = paged.Items.Select(_mapper.Map<EmployeeDto>).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
