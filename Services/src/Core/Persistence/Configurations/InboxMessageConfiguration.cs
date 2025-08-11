using KubeFood.Core.Persistence.BoxMessages;

namespace KubeFood.Core.Persistence.Configurations;

public class InboxMessageConfiguration<TId>
    : BaseBoxMessageConfiguration<InboxMessage<TId>, TId>
{
}