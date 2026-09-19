namespace MathGuide.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage() => InitializeComponent();

    private async void OnCloseTapped(object sender, TappedEventArgs e)
        => await Navigation.PopModalAsync();

    private async void OnLoginTapped(object sender, TappedEventArgs e)
    {
        var login = LoginEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Проверь поля", "Заполни email или телефон и пароль.", "Ок");
            return;
        }

        // Здесь позже: реальный запрос к backend авторизации.
        await DisplayAlert("Вход выполнен", $"Демо-вход: {login}", "Отлично");
        await Navigation.PopModalAsync();
    }

    private async void OnGuestTapped(object sender, TappedEventArgs e)
        => await Navigation.PopModalAsync();

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
        => await DisplayAlert("Восстановление пароля", "Этот экран пока не реализован.", "Ок");

    private async void OnGoToRegisterTapped(object sender, TappedEventArgs e)
        => await Navigation.PushModalAsync(new RegisterPage());
}
