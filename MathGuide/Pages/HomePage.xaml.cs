using MathGuide.ViewModels;

namespace MathGuide.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        BindingContext = new HomeViewModel();
    }

    private async void OnSolveTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//calc");

    private async void OnProfileTapped(object sender, TappedEventArgs e)
        => await Navigation.PushModalAsync(new LoginPage());
}
