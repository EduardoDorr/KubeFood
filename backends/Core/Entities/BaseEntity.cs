using KubeFood.Core.Events;

namespace KubeFood.Core.Entities;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];

    protected BaseEntity()
    {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;

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

    public void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
}

public abstract class BaseEntity<TId> : BaseEntity
{
    public TId Id { get; protected set; }    
}