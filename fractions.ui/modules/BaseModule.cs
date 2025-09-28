using Prism.Ioc;
using Prism.Modularity;

namespace fractions.ui;
public abstract class BaseModule : IModule
{
    public BaseModule()
    {
    }

    public void Initialize()
    {
    }

    public abstract void OnInitialized(IContainerProvider containerProvider);

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }
}
