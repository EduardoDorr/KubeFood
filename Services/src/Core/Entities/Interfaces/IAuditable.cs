namespace KubeFood.Core.Entities.Interfaces;

public interface IAuditable<TId>
{
    TId? UpdatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }

    public virtual void SetUpdatedByAtDate(TId updatedBy, DateTime updatedAtDate)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAtDate;
    }

    public virtual void SetUpdatedBy(TId updatedBy)
        => UpdatedBy = updatedBy;

    public virtual void SetUpdatedAtDate(DateTime updatedAtDate)
        => UpdatedAt = updatedAtDate;
}