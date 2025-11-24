using KubeFood.Core.Persistence.BoxMessages;

namespace KubeFood.Core.Persistence.Configurations;

public class OutboxMessageConfiguration<TId>
    : BaseBoxMessageConfiguration<OutboxMessage<TId>, TId>
{
}