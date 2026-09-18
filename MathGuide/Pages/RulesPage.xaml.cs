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
            await DisplayAlert(section.Title, $"{section.RuleCount} правил · {section.Subtitle}", "Закрыть");
        }
    }
}