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
            await Navigation.PushModalAsync(new RulesListPage(section.Title));
        }
    }

    private async void OnProfileTapped(object sender, TappedEventArgs e)
        => await Navigation.PushModalAsync(new LoginPage());
}
