using System.Security.Cryptography;
using MathGuide.Models;
using SQLite;

namespace MathGuide.Services;

public static class UserSession
{
    private const string CurrentUserIdKey = "current_user_id";
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static SQLiteAsyncConnection? _database;
    private static int? _currentUserId;
    private static UserAccount? _currentUser;

    public static bool IsAuthenticated => _currentUserId.HasValue && _currentUser is not null;
    public static string Name => _currentUser?.Name ?? "Пользователь";
    public static string Email => _currentUser?.Email ?? string.Empty;
    public static string AvatarPath => string.IsNullOrWhiteSpace(_currentUser?.AvatarPath) ? "avatar.png" : _currentUser.AvatarPath;

    public static async Task InitializeAsync()
    {
        await Gate.WaitAsync();
        try
        {
            if (_database is null)
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "mathguide.db3");
                _database = new SQLiteAsyncConnection(path);
                await _database.CreateTableAsync<UserAccount>();
            }

            if (_currentUser is null)
            {
                var id = Preferences.Default.Get(CurrentUserIdKey, 0);
                if (id > 0)
                {
                    _currentUser = await _database.Table<UserAccount>().FirstOrDefaultAsync(user => user.Id == id);
                    _currentUserId = _currentUser?.Id;
                }
            }
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task RegisterAsync(string name, string email, string password)
    {
        await InitializeAsync();
        ValidateCredentials(name, email, password);

        var normalizedEmail = NormalizeEmail(email);
        var existing = await _database!.Table<UserAccount>()
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail);
        if (existing is not null)
            throw new InvalidOperationException("Пользователь с таким email уже зарегистрирован.");

        var salt = RandomNumberGenerator.GetBytes(16);
        var user = new UserAccount
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordSalt = Convert.ToBase64String(salt),
            PasswordHash = HashPassword(password, salt)
        };

        await _database.InsertAsync(user);
        await SetCurrentUserAsync(user);
    }

    public static async Task SignInAsync(string email, string password)
    {
        await InitializeAsync();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Заполни email и пароль.");

        var user = await _database!.Table<UserAccount>()
            .FirstOrDefaultAsync(item => item.Email == NormalizeEmail(email));
        if (user is null || !VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidOperationException("Неверный email или пароль.");

        await SetCurrentUserAsync(user);
    }

    public static async Task UpdateProfileAsync(string name, string email)
    {
        await InitializeAsync();
        if (_currentUser is null)
            throw new InvalidOperationException("Пользователь не авторизован.");
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Заполни имя и email.");

        var normalizedEmail = NormalizeEmail(email);
        var duplicate = await _database!.Table<UserAccount>()
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail && user.Id != _currentUser.Id);
        if (duplicate is not null)
            throw new InvalidOperationException("Этот email уже используется.");

        _currentUser.Name = name.Trim();
        _currentUser.Email = normalizedEmail;
        await _database.UpdateAsync(_currentUser);
    }

    public static async Task SetAvatarPathAsync(string path)
    {
        await InitializeAsync();
        if (_currentUser is null)
            throw new InvalidOperationException("Пользователь не авторизован.");

        _currentUser.AvatarPath = path;
        await _database!.UpdateAsync(_currentUser);
    }

    public static async Task SetPasswordAsync(string password)
    {
        await InitializeAsync();
        if (_currentUser is null)
            throw new InvalidOperationException("Пользователь не авторизован.");
        if (password.Length < 6)
            throw new ArgumentException("Пароль должен содержать минимум 6 символов.");

        var salt = RandomNumberGenerator.GetBytes(16);
        _currentUser.PasswordSalt = Convert.ToBase64String(salt);
        _currentUser.PasswordHash = HashPassword(password, salt);
        await _database!.UpdateAsync(_currentUser);
    }

    public static void SignOut()
    {
        _currentUser = null;
        _currentUserId = null;
        Preferences.Default.Remove(CurrentUserIdKey);
    }

    private static Task SetCurrentUserAsync(UserAccount user)
    {
        _currentUser = user;
        _currentUserId = user.Id;
        Preferences.Default.Set(CurrentUserIdKey, user.Id);
        return Task.CompletedTask;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static void ValidateCredentials(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Заполни имя и email.");
        if (password.Length < 6)
            throw new ArgumentException("Пароль должен содержать минимум 6 символов.");
    }

    private static string HashPassword(string password, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 120_000, HashAlgorithmName.SHA256, 32));

    private static bool VerifyPassword(string password, string hash, string salt)
    {
        var expected = Convert.FromBase64String(hash);
        var actual = Convert.FromBase64String(HashPassword(password, Convert.FromBase64String(salt)));
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
