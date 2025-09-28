namespace fractions.ui.configuration;
public class ReadonlySettings : ISettings
{
    public ReadonlySettings(ISettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Version = settings.Version;
    }

    public string Version { get; }
}