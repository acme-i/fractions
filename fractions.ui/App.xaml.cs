#region Usings
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System.Windows;
using Prism;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
#endregion

namespace fractions.ui;

public partial class App : PrismApplication
{
    private static SettingsManager Manager = new("./settings.json");
    public static Settings Settings { get { return Manager.Settings; } }
    public static Settings GetSettings() { return Manager.Settings; }
    public static IDictionary<string, int> RecentQeuee = new Dictionary<string, int>(1000);

    public App()
    {
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void InitializeShell(Window shell)
    {
        base.InitializeShell(shell);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(Manager.Settings);
        containerRegistry.RegisterInstance<ISettings>(Manager.Settings);
        containerRegistry.RegisterInstance<IMessenger>(WeakReferenceMessenger.Default);
    }

    protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
    {
        base.ConfigureDefaultRegionBehaviors(regionBehaviors);
    }

    protected override IModuleCatalog CreateModuleCatalog()
    {
        return new DirectoryModuleCatalog() { ModulePath = @"C:\dev\_acme_i\fractions\fractions.ui\modules" };
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<MainModule>();
    }
}
