using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fractions.ui.configuration;
using fractions.ui.views;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace fractions.ui.viewmodels;

public partial class NoteOnOffListViewModel : EnumerateViewModel<NoteOnOffViewModel>
{
    private ObservableCollection<NoteOnOffViewModel> _allNotes;
    private InternalCollectionView _noteList;

    public NoteOnOffListViewModel(IMessenger messenger) : base(messenger)
    {
        _allNotes = new ObservableCollection<NoteOnOffViewModel>(Source);
        _noteList = new InternalCollectionView(this, _allNotes);
        _noteList.CurrentChanged += (_, _) =>
        {
            SelectedItem = _noteList.CurrentItem as NoteOnOffViewModel;
        };
    }

    public ObservableCollection<NoteOnOffViewModel> AllNotes => _allNotes;
    
    private readonly int minVol = 47;
    private readonly int maxVol = 107;
    private readonly int maxLeft = 4;
    private readonly int maxRight = 127;
    private readonly Dictionary<Channel, Enumerate<float>> VolMap = new Dictionary<Channel, Enumerate<float>>();
    private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap = new Dictionary<Channel, Enumerate<float>>();
    private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap2 = new Dictionary<Channel, Enumerate<float>>();
    private readonly Dictionary<Channel, Enumerate<float>> PanMap = new Dictionary<Channel, Enumerate<float>>();
    private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap = new Dictionary<Channel, Enumerate<float>>();
    private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap2 = new Dictionary<Channel, Enumerate<float>>();

    private readonly Enumerate<Channel> chans = new[] {
            Channel.Channel1,
            Channel.Channel2,
            Channel.Channel3,
            Channel.Channel4,
        }.AsEnumeration();

    private readonly Enumerate<Channel> echoChans = new[] {
            Channel.Channel5,
            Channel.Channel6,
            Channel.Channel7,
            Channel.Channel8,
        }.AsEnumeration();

    private readonly Enumerate<Channel> chans2 = new[] {
            Channel.Channel9,
            Channel.Channel11,
            Channel.Channel12,
            Channel.Channel13,
        }.AsEnumeration();

    private readonly Enumerate<Channel> echoChans2 = new[] {
            Channel.Channel14,
            Channel.Channel15,
            Channel.Channel16
        }.AsEnumeration();

    static Instrument[] lInstr = new[] {
            Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.Vibraphone, Instrument.ElectricPiano1,
            Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.Vibraphone,
            Instrument.SlapBass1, Instrument.Vibraphone, Instrument.ElectricPiano1, Instrument.ElectricPiano1,
    };

    static Instrument[] rInstr = new[] {
                Instrument.ElectricBassPick, Instrument.ElectricPiano1, Instrument.ElectricBassPick,
                Instrument.ElectricBassPick, Instrument.ElectricBassPick, Instrument.ElectricPiano1,
                Instrument.ElectricPiano1, Instrument.ElectricBassPick, Instrument.ElectricBassPick,
            };

    private readonly Enumerate<Instrument> mainInstr = lInstr.AsCycle();
    private readonly Enumerate<Instrument> secondInstr = rInstr.AsCycle().AsReversed();
    private readonly Enumerate<Instrument> echoMainInstr = rInstr.AsCycle();
    private readonly Enumerate<Instrument> echoSecondInstr = lInstr.AsCycle().AsReversed();

    static string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
    static MidiFile file = new MidiFile(path);
    static float div = file.TicksPerQuarterNote + 0f;

    static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
    static readonly Enumerate<MidiEvent> nots = result.OnEvents.AsEnumeration();
    static readonly Enumerate<float> durs = result.Durations.AsEnumeration();

    [RelayCommand]
    public void ReadFile()
    {
        var channels = Channels.InstrumentChannels;
        var pSteps = Interpolator.Interpolate(maxLeft, maxRight, 4 * 12, 0);
        var vSteps = Interpolator.Interpolate(minVol, maxVol, 4 * 4, 0);

        var pSteps2 = Interpolator.Interpolate(maxLeft, maxRight, 4 * 6, 0);
        var vSteps2 = Interpolator.Interpolate(minVol, maxVol, 4 * 2, 0);

        var voffset = 24;
        var poffset = 8;
        Enumerate<float> fen;
        foreach (var x in channels)
        {
            var s = (int)x;

            fen = vSteps.AsCycle();
            for (var i = 0; i < s; i++)
                fen.GetNext();
            VolMap.Add(x, fen);

            fen = vSteps2.AsCycle();
            for (var i = 0; i < s + voffset; i++)
                fen.GetNext();
            EchoVolMap.Add(x, fen);

            fen = vSteps.AsCycle();
            for (var i = 0; i < s + (int)x + voffset; i++)
                fen.GetNext();
            EchoVolMap2.Add(x, fen);

            fen = pSteps.AsCycle();
            for (var i = 0; i < s; i++)
                fen.GetNext();
            PanMap.Add(x, fen);

            fen = pSteps.AsCycle();
            for (var i = 0; i < s + poffset; i++)
                fen.GetNext();
            EchoPanMap.Add(x, fen);

            fen = pSteps2.AsCycle();
            for (var i = 0; i < s + (int)x + poffset; i++)
                fen.GetNext();
            EchoPanMap2.Add(x, fen);
        }

        var times = new[] {
                0, 0, 0, 2,
                0, 0, 4, 0,
                0, 2, 0, 0,
                4, 0, 0, 0,

                0, 2, 0, 0,
                0, 0, 4, 0,
                0, 0, 0, 2,
                0, 0, 4, 0,

                0, 2, 0, 0,
                4, 0, 0, 0,
                0, 2, 0, 0,
                0, 0, 4, 0,

                4, 0, 0, 0,
                0, 8, 0, 0,
                0, 0, 4, 0,
                0, 0, 0, 2,

                0, 0, 4, 0,
                0, 8, 0, 0,
                4, 0, 0, 0,
                0, 2, 0, 0,

                0, 0, 4, 0,
            };

        var resultTimes = new List<int>(times);
        resultTimes.AddRange(times.Select(t => 2));
        resultTimes.AddRange(times.Select(t => 4));
        resultTimes.AddRange(times.Select(t => 8));
        resultTimes.AddRange(times.Select(t => 4));

        var echoes = resultTimes.AsCycle();
        
        Source.Clear();
        for (var i = 0; i < result.Durations.Count(); i++)
        {
            PlayAt(i);
            var nextEcho = echoes.GetNext();
            if (nextEcho > 1)
            {
                PlayEchos(i, nextEcho);
            }
        }

        //Source.ForEach(n => n.Reverb = (n.Velocity + n.Pan)/2.0);

        NotifyPropertyChangingOnUiThread(nameof(AllNotes));
        _allNotes = new ObservableCollection<NoteOnOffViewModel>(
            Source
                .Where(n => Double.IsNaN(n.Duration) == false)
                .OrderBy(n => n.Time)
        );
        NotifyPropertyChangedOnUiThread(nameof(AllNotes));
    }

    private void PlayAt(int i)
    {
        var note = nots.GetNext();
        var dur = durs.GetNext();
        var chan = i % 2 == 0 ? chans : chans2;
        var ch = chan.GetNext();
        var pm = i % 2 == 0 ? PanMap[ch] : EchoPanMap[ch];
        var vol = i % 2 == 0 ? VolMap[ch] : EchoVolMap[ch];
        var instr = i % 4 != 0 ? mainInstr : secondInstr;

        var nt = new NoteOnOffMessage(App.OutputDevice, ch, (Pitch)note.Note + 12, vol.GetNext(), note.Time / div, App.DefaultClock, dur, pm.GetNext());
        nt.BeforeSendingNoteOnOff += m =>
        {
            App.OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
        };
        App.DefaultClock.Schedule(nt);

        Source.Add(new NoteOnOffViewModel(Messenger, nt));
    }

    readonly Enumerate<int> ps1 = new[] {
            12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
            24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
            0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24
        }.AsEnumeration();
    readonly Enumerate<int> ps2 = new[] {
            24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
            0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,24,
            12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
        }.AsEnumeration();
    private void PlayEchos(int i, int max)
    {
        var notsClone = nots.Clone();
        var durClone = durs.Clone();

        var note = notsClone.GetNext();
        var dur = durClone.GetNext();

        var noteDest = notsClone.PeekAt(max);
        var durDest = durClone.PeekAt(max);
        var ps = i % 2 == 0 ? ps1 : ps2;

        var scaler = Interpolator.Interpolate(1f, 1.5f, max, 0).AsCycle();
        var notesTimes = Interpolator.Interpolate(note.Time, noteDest.Time, max, 1).AsEnumeration();
        var noteDurs = Interpolator.Interpolate(dur, durDest, max, 1).AsEnumeration();
        var sm = max;
        var scalers = Enumerable
                .Range(1, sm)
                .Select(x => x / (sm * scaler.GetNext()))
                .AsMaxMinEnumeration();

        max = i + max;

        for (var j = i; j < max; j++)
        {
            var chan = j % 2 == 0 ? echoChans : echoChans2;
            var ch = chan.GetNext();
            var pm = i % 2 == 0 ? EchoPanMap[ch] : EchoPanMap2[ch];
            var vol = i % 2 == 0 ? EchoVolMap[ch] : EchoVolMap2[ch];
            var instr = j % 2 == 0 ? echoMainInstr : echoSecondInstr;

            var nt = new NoteOnOffMessage(App.OutputDevice, ch, (Pitch)note.Note + ps.GetNext(), vol.GetNext(), (notsClone.Current.Time + notesTimes.GetNext()) / div, App.DefaultClock, noteDurs.GetNext(), pm.GetNext());
            nt.BeforeSendingNoteOnOff += m =>
            {
                App.OutputDevice.SendProgramChange(m.Channel, instr.GetNext());
            };
            App.DefaultClock.Schedule(nt);

            note = notsClone.GetNext();
            dur = durClone.GetNext();

            Source.Add(new NoteOnOffViewModel(Messenger, nt));
        }

        ps.GetNext();
    }

    [RelayCommand]
    public void SelectAll()
    {
        View.SelectAll();
    }

    public IList<NoteOnOffViewModel> GetSelectedItems()
    {
        return View.GetSelectedItems();
    }

    public IListView<NoteOnOffViewModel> View { get; set; }

    public int SelectedIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int SelectedItemsCount => throw new NotImplementedException();

    [ObservableProperty]
    private bool isAscendingOrder;

    [ObservableProperty]
    private NoteOnOffViewModel? selectedItem;

    public IList<NoteOnOffViewModel> SelectedItems
    {
        get
        {
            return View.GetSelectedItems();
        }
    }

    #region InternalCollectionView
    public class InternalCollectionView : ListCollectionView, ICollectionView
    {
        public InternalCollectionView(NoteOnOffListViewModel owner, IList list) : base(list)
        {
            Owner = owner;
        }

        public NoteOnOffListViewModel Owner { get; }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            Owner.OnPropertyChanged(new PropertyChangedEventArgs($"Notes.{args.PropertyName}"));
        }

        public void RaiseItemsChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SourceCollection)));
        }

        public void RaiseFilterChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Filter)));
        }
    }
    #endregion

}