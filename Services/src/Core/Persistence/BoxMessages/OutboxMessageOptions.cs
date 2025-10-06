using KubeFood.Core.Options;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class OutboxMessageOptions : BaseBoxMessageOptions, IOptionsSection
{
    public static string Name => "OutboxMessageConfiguration";
}