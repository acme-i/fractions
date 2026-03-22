#region Usings
using fractions.ui.configuration;
using fractions.ui.viewmodels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
#endregion

namespace fractions.ui;

public partial class App : System.Windows.Application
{
    private static SettingsManager Manager = new("./settings.json");
    public static Settings Settings { get { return Manager.Settings; } }
    public static Settings GetSettings() { return Manager.Settings; }

    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // Infrastructure
                services.AddSingleton<ISettings>(settings => Settings);

                // ViewModels
                services.AddTransient<MainViewModel>();

                // Views
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();


        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
