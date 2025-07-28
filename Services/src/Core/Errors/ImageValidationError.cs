using KubeFood.Core.Results.Errors;

namespace KubeFood.Core.Errors;

public static class ImageValidationError
{
    public static Error NoImageSent =>
        new("ProductError.NoImageSent", "No image was sent", ErrorType.Validation);

    public static Error WrongSize =>
        new("ProductError.WrongSize", "Image is too large (max 2 MB)", ErrorType.Validation);

    public static Error InvalidFormat =>
        new("ProductError.InvalidFormat", "Image format is invalid", ErrorType.Validation);
}