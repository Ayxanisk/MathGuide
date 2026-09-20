using MathGuide.Services;

namespace MathGuide
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            _ = UserSession.InitializeAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}   