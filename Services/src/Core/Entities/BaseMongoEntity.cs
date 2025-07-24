using Core.DomainEvents;

using MongoDB.Bson;

namespace Core.Entities;

public abstract class BaseMongoEntity
{
    public ObjectId Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];

    protected BaseMongoEntity()
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

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
}