
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace fractions.ui.configuration;
public class SettingsManager
{
    private Settings settings = new();
    public Settings Settings { get => settings; set => settings = value; }
    public string FileName;
    private static object locker = new();
    public SettingsManager(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        FileName = fileName;
        Load();
        settings.PropertyChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            Save();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.ToString());
        }
    }

    private void CreateDirectory()
    {
        var dir = Path.GetDirectoryName(FileName);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private void Load()
    {
        CreateDirectory();

        if (!File.Exists(FileName))
        {
            Save();
            return;
        }

        var jsonString = File.ReadAllText(FileName, Encoding.UTF8);
        var settings = JsonSerializer.Deserialize<Settings>(jsonString);
        this.Settings = settings;
    }

    private void Save()
    {
        lock (locker)
        {
            CreateDirectory();
            using var createStream = File.Create(FileName);
            JsonSerializer.Serialize(createStream, Settings);
        }
    }
}
