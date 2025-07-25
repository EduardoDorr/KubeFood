﻿using Core.Errors;
using Core.Interfaces;
using Core.Options;
using Core.Results;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface ISaveImageService
    : IRequestHandler<IFormFile, Result<string>>
{ }

public class SaveImageService : ISaveImageService
{
    private const int MAX_IMAGE_SIZE_IN_MB = 2 * 1024 * 1024;
    private readonly string[] ALLOWED_EXTENSIONS = [".jpg", ".jpeg", ".png"];

    private readonly string _imageFolderPath;

    public SaveImageService(IWebHostEnvironment environment, IOptions<ImageValidationOptions> options)
    {
        _imageFolderPath = Path.Combine(environment.WebRootPath, options.Value.FolderPath);

        Directory.CreateDirectory(_imageFolderPath);
    }

    public async Task<Result<string>> HandleAsync(IFormFile request, CancellationToken cancellationToken = default)
    {
        if (request is null || request.Length == 0)
            return Result.Fail<string>(ImageValidationError.NoImageSent);

        if (request.Length > MAX_IMAGE_SIZE_IN_MB)
            return Result.Fail<string>(ImageValidationError.WrongSize);

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (!ALLOWED_EXTENSIONS.Contains(extension))
            return Result.Fail<string>(ImageValidationError.InvalidFormat);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_imageFolderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);

        await request.CopyToAsync(stream, cancellationToken);

        return Result.Ok(fileName);
    }
}