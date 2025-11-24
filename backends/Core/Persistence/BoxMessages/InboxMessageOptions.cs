using KubeFood.Core.Options;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class InboxMessageOptions : BaseBoxMessageOptions, IOptionsSection
{
    public static string Name => "InboxMessageConfiguration";
}