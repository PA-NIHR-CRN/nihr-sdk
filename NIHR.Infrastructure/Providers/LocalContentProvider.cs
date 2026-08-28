using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Models;

public class LocalContentProvider : IContentProvider
{
    private readonly string _rootPath;

    public LocalContentProvider(string rootPath)
    {
        _rootPath = rootPath;
    }

    public async Task<TContent> GetContentAsync<TContent>(
        ContentRequestModel request,
        CancellationToken cancellationToken = default)
        where TContent : new()
    {
        if (string.IsNullOrWhiteSpace(request.ContentValue))
            throw new ArgumentException("Content Value cannot be null or empty.");

        var filePath = Path.Combine(_rootPath, request.ContentValue + ".json");

        if (!File.Exists(filePath))
            return default;

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return JsonConvert.DeserializeObject<TContent>(json);
    }
}
