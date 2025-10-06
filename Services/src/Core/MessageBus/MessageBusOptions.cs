using KubeFood.Core.Options;

using System.ComponentModel.DataAnnotations;

namespace KubeFood.Core.MessageBus;

public class MessageBusOptions : IOptionsSection
{
    public static string Name => "MessageBusProvider";

    [Required]
    public MessageBusProvider Provider { get; init; }
}