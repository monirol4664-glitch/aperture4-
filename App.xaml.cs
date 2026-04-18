using Microsoft.Maui.Controls;

namespace aperture4;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new MainPage();
    }
}