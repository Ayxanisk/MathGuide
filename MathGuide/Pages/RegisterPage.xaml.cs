using MathGuide.Services;

namespace MathGuide.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage() => InitializeComponent();

    private async void OnCloseTapped(object sender, TappedEventArgs e)
        => await Navigation.PopModalAsync();

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var contact = ContactEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        var confirm = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(contact) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Проверь поля", "Заполни имя, email или телефон и пароль.", "Ок");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("Пароли не совпадают", "Проверь оба поля с паролем.", "Ок");
            return;
        }

        UserSession.SignIn(name, contact);
        await UserSession.SetPasswordAsync(password);
        await DisplayAlert("Готово", $"Демо-регистрация: {name}", "Отлично");

        // Закрываем и регистрацию, и страницу входа под ней — возвращаемся в приложение.
        await Navigation.PopModalAsync();
        await Navigation.PopModalAsync();
        await Navigation.PushModalAsync(new ProfilePage());
    }

    private async void OnBackToLoginTapped(object sender, TappedEventArgs e)
        => await Navigation.PopModalAsync();
}
