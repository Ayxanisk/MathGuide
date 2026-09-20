using MathGuide.ViewModels;

namespace MathGuide.Pages;

public partial class RulesListPage : ContentPage
{
    public RulesListPage(string subject)
    {
        InitializeComponent();
        BindingContext = new RulesListViewModel(subject);
    }
}
