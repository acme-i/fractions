#region Usings
using fractions.ui.configuration;
using fractions.ui.viewmodels;
using fractions.ui.views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
#endregion

namespace fractions.ui;

public partial class App : System.Windows.Application
{
    private static readonly SettingsManager Manager = new("./settings.json");
    public static Settings Settings => Manager.Settings;

    public static IMessenger Messenger = WeakReferenceMessenger.Default;

    public static IHost? AppHost { get; private set; }
    public static IOutputDevice OutputDevice { get; internal set; }
    public static Clock DefaultClock { get; } = new(120f);

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                // Clock: expose the shared static instance
                services.AddSingleton<Clock>(_ => DefaultClock);

                // IOutputDevice: resolved once from the OS; also sets App.OutputDevice
                services.AddSingleton<IOutputDevice>(_ =>
                {
                    var device = fractions.OutputDevice.InstalledDevices.FirstOrDefault();
                    OutputDevice = device;          // keep the static ref in sync
                    return device;
                });

                // ── Core singletons ───────────────────────────────────────
                services.AddSingleton<ClockViewModel>();
                services.AddSingleton<OutputDevicesViewModel>();
                services.AddSingleton<MidiFileSourceViewModel>();

                // NoteOnOffListViewModel primary ctor needs Enumerate<NoteOnOffMessage>.
                // The VM loads its own notes via ReadFile(), so an empty source is fine.
                services.AddSingleton<Enumerate<NoteOnOffMessage>>(
                    _ => Enumerable.Empty<NoteOnOffMessage>().AsEnumeration());

                services.AddTransient<IntegerInterpolatorViewModel>();
                services.AddTransient<IntegerEnumeratorViewModel>();
                services.AddTransient<FloatInterpolatorViewModel>();
                services.AddTransient<FloatEnumeratorViewModel>();

                services.AddSingleton<NoteOnOffListViewModel>();
                
                // MainViewModel is the root DataContext — must be Singleton so every
                // view that inherits it sees the same instance.
                services.AddSingleton<MainViewModel>();

                services.AddTransient<MidiFileSourceView>();
                services.AddTransient<EchoGeneratorView>();
                services.AddTransient<NoteListPreviewView>();
                services.AddTransient<IntegerInterpolatorView>();
                services.AddTransient<FloatInterpolatorView>();
                services.AddTransient<DoubleInterpolatorView>();

                // ── Views ─────────────────────────────────────────────────
                // Only register views that are resolved directly from the container.
                // Child views declared in XAML are created by WPF and receive their
                // DataContext through the inherited binding chain.
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        // Do NOT call base.OnStartup — that would process StartupUri and open a second window.
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
