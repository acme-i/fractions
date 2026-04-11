using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fractions.ui.views;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace fractions.ui.viewmodels;

public partial class NoteOnOffListViewModel : EnumerateViewModel<NoteOnOffViewModel>
{
    private readonly InternalCollectionView _noteList;

    public NoteOnOffListViewModel() : base()
    {
        _noteList = new InternalCollectionView(this, Source.ToList());
        _noteList.CurrentChanged += (_, _) =>
        {
            SelectedItem = _noteList.CurrentItem as NoteOnOffViewModel;
        };
    }

    private readonly int minVol = 47;
    private readonly int maxVol = 107;
    private readonly int maxLeft = 4;
    private readonly int maxRight = 127;
    private readonly Dictionary<Channel, Enumerate<float>> VolMap = [];
    private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap = [];
    private readonly Dictionary<Channel, Enumerate<float>> EchoVolMap2 = [];
    private readonly Dictionary<Channel, Enumerate<float>> PanMap = [];
    private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap = [];
    private readonly Dictionary<Channel, Enumerate<float>> EchoPanMap2 = [];

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

    private static readonly Instrument[] lInstr = [
            Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.Vibraphone, Instrument.ElectricPiano1,
            Instrument.SlapBass1, Instrument.ElectricPiano1, Instrument.ElectricPiano1, Instrument.Vibraphone,
            Instrument.SlapBass1, Instrument.Vibraphone, Instrument.ElectricPiano1, Instrument.ElectricPiano1,
    ];

    private static readonly Instrument[] rInstr = [
                Instrument.ElectricBassPick, Instrument.ElectricPiano1, Instrument.ElectricBassPick,
                Instrument.ElectricBassPick, Instrument.ElectricBassPick, Instrument.ElectricPiano1,
                Instrument.ElectricPiano1, Instrument.ElectricBassPick, Instrument.ElectricBassPick,
            ];

    private readonly Enumerate<Instrument> mainInstr = lInstr.AsCycle();
    private readonly Enumerate<Instrument> secondInstr = rInstr.AsCycle().AsReversed();
    private readonly Enumerate<Instrument> echoMainInstr = rInstr.AsCycle();
    private readonly Enumerate<Instrument> echoSecondInstr = lInstr.AsCycle().AsReversed();

    private static readonly string path = @".\midifiles\bach_js_bwv0999_prelude_in_cm_for_lute.mid";
    private static readonly MidiFile file = new(path);
    private static readonly float div = file.TicksPerQuarterNote + 0f;

    private static (IEnumerable<MidiEvent> OnEvents, IEnumerable<MidiEvent> OffEvents, IEnumerable<float> Durations) result = file.GetEventsAndDurations();
    private static readonly Enumerate<MidiEvent> nots = result.OnEvents.AsEnumeration();
    private static readonly Enumerate<float> durs = result.Durations.AsEnumeration();

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

        NotifyPropertyChangingOnUiThread(nameof(Source));
        Source = Source
                .Where(n => double.IsNaN(n.Duration) == false)
                .OrderBy(n => n.Time)
                .AsEnumeration();
        NotifyPropertyChangedOnUiThread(nameof(Source));
        NotifyPropertyChangedOnUiThread(nameof(SelectedIndex));
        NotifyPropertyChangedOnUiThread(nameof(SelectedItem));
        NotifyPropertyChangedOnUiThread(nameof(SelectedItems));
        NotifyPropertyChangedOnUiThread(nameof(SelectedItemsCount));
        NotifyPropertyChangedOnUiThread(nameof(IsRangeSelected));
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

        Source.Add(new NoteOnOffViewModel(nt));
    }

    private readonly Enumerate<int> ps1 = new[] {
        12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0,
        24, 0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12,
        0, 12, 24, 0, 12, 24, 0, 12, 24, 0, 12, 24
    }.AsEnumeration();

    private readonly Enumerate<int> ps2 = new[] {
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

            Source.Add(new NoteOnOffViewModel(nt));
        }

        ps.GetNext();
    }

    [RelayCommand]
    public void SelectAll()
    {
        ListView.SelectAll();
        NotifyPropertyChangedOnUiThread(nameof(SelectedIndex));
        NotifyPropertyChangedOnUiThread(nameof(SelectedItem));
        NotifyPropertyChangedOnUiThread(nameof(SelectedItems));
        NotifyPropertyChangedOnUiThread(nameof(SelectedItemsCount));
        NotifyPropertyChangedOnUiThread(nameof(IsRangeSelected));
    }

    public IList<NoteOnOffViewModel> GetSelectedItems() => ListView.GetSelectedItems() ;

    public IListView<NoteOnOffViewModel> ListView { get; set; }

    public int SelectedIndex { get => ListView?.SelectedIndex ?? -1; set => ListView.SelectedIndex = value; }

    public int SelectedItemsCount => ListView?.SelectedItemsCount ?? 0;
    public bool IsSingleItemSelected => SelectedItemsCount == 1;
    public bool IsRangeSelected => SelectedItemsCount > 0;

    [ObservableProperty]
    private bool isAscendingOrder;

    [ObservableProperty]
    private NoteOnOffViewModel? selectedItem;

    public IList<NoteOnOffViewModel> SelectedItems => ListView?.GetSelectedItems();

    #region Methods

    [RelayCommand]
    public void InterpolateVelocity()
    {
        try
        {
            var notes = GetSelectedItems();
            var start = notes.First().Velocity;
            var end = notes.Last().Velocity;
            var count = notes.Count;

            if (count < 3 || start == end) return;

            NotifyPropertyChangedOnUiThread(nameof(Source));

            var values = Interpolator.Interpolate(start, end, count, 0).AsEnumeration();
            foreach (var n in notes)
            {
                n.Velocity = values.GetNext();
            }

            NotifyPropertyChangedOnUiThread(nameof(Source));
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand]
    public void InterpolateDuration()
    {
        try
        {
            var notes = GetSelectedItems();
            var start = notes.First().Duration;
            var end = notes.Last().Duration;
            var count = notes.Count;

            if (count < 3 || start == end) return;

            NotifyPropertyChangingOnUiThread(nameof(Source));

            var values = Interpolator.Interpolate(start, end, count, 0).AsEnumeration();
            foreach (var n in notes)
            {
                n.Duration = values.GetNext();
            }

            NotifyPropertyChangedOnUiThread(nameof(Source));
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand]
    public void InterpolateReverb()
    {
        try
        {
            var notes = GetSelectedItems();
            var start = (float)notes.First().Reverb;
            var end = (float)notes.Last().Reverb;
            var count = notes.Count;

            if (count < 3 || start == end) return;

            NotifyPropertyChangingOnUiThread(nameof(Source));

            var values = Interpolator.Interpolate(start, end, count, 0).AsEnumeration();
            foreach (var n in notes)
            {
                n.Reverb = values.GetNext();
            }

            NotifyPropertyChangedOnUiThread(nameof(Source));
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand]
    public void InterpolatePan()
    {
        try
        {
            var notes = GetSelectedItems();
            var start = (float)notes.First().Pan;
            var end = (float)notes.Last().Pan;
            var count = notes.Count;

            if (count < 3 || start == end) return;

            NotifyPropertyChangingOnUiThread(nameof(Source));

            var values = Interpolator.Interpolate(start, end, count, 0).AsEnumeration();
            foreach (var n in notes)
            {
                n.Pan = values.GetNext();
            }

            NotifyPropertyChangedOnUiThread(nameof(Source));
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    #endregion

    #region InternalCollectionView
    public class InternalCollectionView(NoteOnOffListViewModel owner, IList list) : ListCollectionView(list), ICollectionView
    {
        public NoteOnOffListViewModel Owner { get; } = owner;

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