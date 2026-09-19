namespace MathGuide.Services;

public static class UserSession
{
    private const string AuthenticatedKey = "user_authenticated";
    private const string NameKey = "user_name";
    private const string EmailKey = "user_email";
    private const string AvatarPathKey = "user_avatar_path";

    public static bool IsAuthenticated =>
        Preferences.Default.Get(AuthenticatedKey, false);

    public static string Name =>
        Preferences.Default.Get(NameKey, "Пользователь");

    public static string Email =>
        Preferences.Default.Get(EmailKey, "");

    public static string AvatarPath =>
        Preferences.Default.Get(AvatarPathKey, "avatar.png");

    public static void SignIn(string name, string email)
    {
        Preferences.Default.Set(AuthenticatedKey, true);
        UpdateProfile(name, email);
    }

    public static void UpdateProfile(string name, string email)
    {
        Preferences.Default.Set(NameKey, name);
        Preferences.Default.Set(EmailKey, email);
    }

    public static void SetAvatarPath(string path) =>
        Preferences.Default.Set(AvatarPathKey, path);

    public static void SignOut()
    {
        Preferences.Default.Remove(AuthenticatedKey);
        Preferences.Default.Remove(NameKey);
        Preferences.Default.Remove(EmailKey);
        Preferences.Default.Remove(AvatarPathKey);
        SecureStorage.Default.Remove("user_password");
    }

    public static Task SetPasswordAsync(string password) =>
        SecureStorage.Default.SetAsync("user_password", password);
}
