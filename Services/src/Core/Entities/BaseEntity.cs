using KubeFood.Core.DomainEvents;

namespace KubeFood.Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        IsActive = true;
    }

    public virtual void Activate()
        => IsActive = true;

    public virtual void Deactivate()
        => IsActive = false;

    public virtual void SetUpdatedAtDate(DateTime updatedAtDate)
        => UpdatedAt = updatedAtDate;

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
        => _domainEvents.ToList();

    public void ClearDomainEvents()
        => _domainEvents.Clear();

    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
}