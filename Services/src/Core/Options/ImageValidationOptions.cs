namespace Core.Options;

public sealed class ImageValidationOptions
{
    public const string Name = "ImageValidation";

    public required string FolderPath { get; init; }
}