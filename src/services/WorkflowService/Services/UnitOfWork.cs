using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly WorkflowDbContext _context;
    public UnitOfWork(WorkflowDbContext context) => _context = context;

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
