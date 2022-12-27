using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSplashScreenDemo.ViewModels;
using AvaloniaSplashScreenDemo.Views;
using System.Threading.Tasks;

namespace AvaloniaSplashScreenDemo
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                // Create the splash screen
                var splashScreenVM = new SplashScreenViewModel();
                var splashScreen = new SplashScreen {
                    DataContext = splashScreenVM
                };

                // Set as the (temporary) main window.
                // By default, the application lifetime will shtut down the application when the main
                // window is closed (unless ShutdownMode is set to something else). Later on we will
                // swap out MainWindow for the "true" MainWindow before closing the splash screen.
                // I see that this type of MainWindow swapping is used when switching themes in
                // Avalonias ControlCatalog source, so presumably it's OK to do this.
                desktop.MainWindow = splashScreen;
                
                splashScreen.Show();

                try {
                    // Initialize the device. We can interact with splashScreenVM.CancellationToken
                    // to determine if the user wants to abort the connection process.
                    splashScreenVM.StartupMessage = "Searching for devices...";
                    await Task.Delay(1000, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Connecting to device #1...";
                    await Task.Delay(2000, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Configuring device...";
                    await Task.Delay(2000, splashScreenVM.CancellationToken);
                }
                catch (TaskCanceledException) {
                    // User cancelled somewhere along the line. We could clean up here if needed.
                    // Then, close the splash screen and return to shut down the application.
                    // (If we don't close the splash screen, the app will remain running since
                    // it is the MainWindow.)
                    splashScreen.Close();
                    return;
                }

                // Create the main window, and swap it in for the real main window
                var mainWin = new MainWindow {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow = mainWin;
                mainWin.Show();
                
                // Get rid of the splash screen
                splashScreen.Close();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
