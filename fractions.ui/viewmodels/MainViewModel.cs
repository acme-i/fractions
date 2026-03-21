using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fractions.ui.viewmodels;

public partial class MainViewModel : BaseViewModel
{
    public IOutputDevice outputDevice;
    public Clock clock;
    private readonly int BPM = 64;
    string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";

    public MainViewModel(IMessenger messenger, Settings settings) : base(messenger, settings)
    {
        Settings = settings;

        // Prompt user to choose an output device (or if there is only one, use that one).
        outputDevice = OutputDevice.InstalledDevices.FirstOrDefault();
        outputDevice.Open();

        foreach (var x in Channels.InstrumentChannels)
        {
            outputDevice.SendControlChange(x, Control.ReverbLevel, 0);
            outputDevice.SendControlChange(x, Control.Volume, (int)Control.MaxControl);
        }

        clock = new Clock(BPM);
        var file = new MidiFile(path);
        var notes = file.GetNotes(outputDevice, clock);
        var noteE = notes.AsEnumeration();
    }


}