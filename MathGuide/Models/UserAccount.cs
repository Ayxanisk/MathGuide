using SQLite;

namespace MathGuide.Models;

public sealed class UserAccount
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, NotNull]
    public string Email { get; set; } = string.Empty;

    [NotNull]
    public string Name { get; set; } = string.Empty;

    [NotNull]
    public string PasswordHash { get; set; } = string.Empty;

    [NotNull]
    public string PasswordSalt { get; set; } = string.Empty;

    public string AvatarPath { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
