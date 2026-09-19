using MathGuide.Services;

namespace MathGuide.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        NameEntry.Text = UserSession.Name;
        EmailEntry.Text = UserSession.Email;
        AvatarImage.Source = UserSession.AvatarPath;
    }

    private async void OnCloseTapped(object sender, TappedEventArgs e) =>
        await Navigation.PopModalAsync();

    private async void OnChangeAvatarTapped(object sender, TappedEventArgs e)
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo is null)
            return;

        var extension = Path.GetExtension(photo.FileName);
        var targetPath = Path.Combine(FileSystem.Current.AppDataDirectory, $"profile-avatar{extension}");
        await using var source = await photo.OpenReadAsync();
        await using var target = File.Create(targetPath);
        await source.CopyToAsync(target);

        UserSession.SetAvatarPath(targetPath);
        AvatarImage.Source = targetPath;
    }

    private async void OnSaveProfileTapped(object sender, TappedEventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Проверь поля", "Заполни имя и email.", "Ок");
            return;
        }

        UserSession.UpdateProfile(name, email);
        await DisplayAlert("Сохранено", "Данные профиля обновлены.", "Ок");
    }

    private async void OnChangePasswordTapped(object sender, TappedEventArgs e)
    {
        var password = PasswordEntry.Text;
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            await DisplayAlert("Слишком короткий пароль", "Пароль должен содержать минимум 6 символов.", "Ок");
            return;
        }

        if (password != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Пароли не совпадают", "Проверь оба поля с паролем.", "Ок");
            return;
        }

        await UserSession.SetPasswordAsync(password);
        PasswordEntry.Text = string.Empty;
        ConfirmPasswordEntry.Text = string.Empty;
        await DisplayAlert("Сохранено", "Пароль успешно изменён.", "Ок");
    }

    private async void OnLogoutTapped(object sender, TappedEventArgs e)
    {
        UserSession.SignOut();
        await Navigation.PopModalAsync();
    }
}
