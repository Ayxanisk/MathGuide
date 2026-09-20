namespace MathGuide.Services;

public sealed class PdfManagerService
{
    public async Task<string> GetPdfFilePathAsync(string fileName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Имя PDF-файла не задано.", nameof(fileName));

        var safeFileName = Path.GetFileName(fileName);
        if (!string.Equals(safeFileName, fileName, StringComparison.Ordinal))
            throw new ArgumentException("Недопустимое имя PDF-файла.", nameof(fileName));

        var cachePath = Path.Combine(FileSystem.CacheDirectory, safeFileName);
        if (File.Exists(cachePath))
            return cachePath;

        await using var source = await FileSystem.OpenAppPackageFileAsync(safeFileName);
        await using var destination = new FileStream(
            cachePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await source.CopyToAsync(destination, ct);
        return cachePath;
    }
}
