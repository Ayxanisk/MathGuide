using MathGuide.Models;
using MathGuide.ViewModels;

namespace MathGuide.Pages;

public partial class RulesPage : ContentPage
{
    public RulesPage()
    {
        InitializeComponent();
        BindingContext = new RulesViewModel();
    }

    private async void OnSectionTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border { BindingContext: MathSection section })
        {
            // Здесь позже: Shell.Current.GoToAsync($"topics?section={section.Title}");
            await DisplayAlert(section.Title, $"{section.RuleCount} правил · {section.Subtitle}", "Закрыть");
        }
    }

    private async void OnProfileTapped(object sender, TappedEventArgs e)
        => await Navigation.PushModalAsync(new LoginPage());
}
