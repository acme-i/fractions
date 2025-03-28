To listen to some crazy experiments with reading and generating a new midi,
open the solution in Visual Studio. Set the '''fractions.examples''' as startup project.
Start the project listen to for example 22

# Midi #

## Type CallbackMessage

Pseudo-MIDI message used to arrange for a callback at a certain time



> This message can be scheduled with [[ Clock.Schedule |M:Midi.Clock.Schedule(Midi.Message)]] just like any other message. When its time comes and it gets "sent", it invokes the callback provided in the constructor. 

 The idea is that you can embed callback points into the music you've scheduled, so that (if the clock gets to that point in the music) your code has an opportunity for some additional processing. 

 The callback is invoked on the MidiOutputDevice's worker thread. 





---
## Type CallbackMessage.CallbackType

Delegate called when a CallbackMessage is sent

|Name | Description |
|-----|------|
|time: | The time at which this event was scheduled|
**Returns**:  Additional messages which should be scheduled as a result of this callback, or null. 



---
#### Method CallbackMessage.#ctor(Midi.CallbackMessage.CallbackType,System.Single)

Constructs a Callback message

|Name | Description |
|-----|------|
|callback: | The callback to invoke when this message is "sent"|
|time: | The timestamp for this message|


---
#### Property CallbackMessage.Callback

The callback to invoke when this message is "sent"



---
#### Method CallbackMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method CallbackMessage.SendNow

Sends this message immediately, ignoring the beatTime



---
## Type Channel

A MIDI Channel



> Each MIDI device has 16 independent channels. Channels are named starting at 1, but are encoded programmatically starting at 0. 

 All of the channels are general-purpose except for Channel10, which is the dedicated percussion channel. Any notes sent to that channel will play [[ percussion notes |T:Midi.Percussion]], regardless of any [[ Program Change |M:fractions.OutputDevice.SendProgramChange(Midi.Channel,Midi.Instrument)]] messages sent on that channel. 

 This enum has extension methods, such as [[|M:Midi.ChannelExtensions.Name(Midi.Channel)]] and [[|M:Midi.ChannelExtensions.IsValid(Midi.Channel)]], defined in [[|T:Midi.ChannelExtensions]]. 





---
#### Field Channel.Channel1

MIDI Channel 1



---
#### Field Channel.Channel2

MIDI Channel 2



---
#### Field Channel.Channel3

MIDI Channel 3



---
#### Field Channel.Channel4

MIDI Channel 4



---
#### Field Channel.Channel5

MIDI Channel 5



---
#### Field Channel.Channel6

MIDI Channel 6



---
#### Field Channel.Channel7

MIDI Channel 7



---
#### Field Channel.Channel8

MIDI Channel 8



---
#### Field Channel.Channel9

MIDI Channel 9



---
#### Field Channel.Channel10

MIDI Channel 10, the dedicated percussion channel



---
#### Field Channel.Channel11

MIDI Channel 11



---
#### Field Channel.Channel12

MIDI Channel 12



---
#### Field Channel.Channel13

MIDI Channel 13



---
#### Field Channel.Channel14

MIDI Channel 14



---
#### Field Channel.Channel15

MIDI Channel 15



---
#### Field Channel.Channel16

MIDI Channel 16



---
## Type ChannelExtensions

Extension methods for the Channel enum



---
#### Field ChannelExtensions.ChannelNames

Table of channel names



---
#### Method ChannelExtensions.IsValid(Midi.Channel)

Returns true if the specified channel is valid

|Name | Description |
|-----|------|
|channel: |The channel to test|


---
#### Method ChannelExtensions.Name(Midi.Channel)

Returns the human-readable name of a MIDI channel

|Name | Description |
|-----|------|
|channel: |The channel|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The channel is out-of-range.



---
#### Method ChannelExtensions.Validate(Midi.Channel)

Throws an exception if channel is not valid

|Name | Description |
|-----|------|
|channel: |The channel to validate|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The channel is out-of-range.



---
## Type ChannelMessage

Base class for messages relevant to a specific device channel



---
#### Method ChannelMessage.#ctor(Midi.DeviceBase,Midi.Channel,System.Single)

Protected constructor



---
#### Property ChannelMessage.Channel

Channel



---
## Type Chord

A chord



> A chord is defined by its root note, the sequence of semitones, the sequence of letters, and the inversion. The root note is described with a [[|T:Midi.Note]] because we want to be able to talk about the chord independent of any one octave. The pattern of semitones and letters is given by the [[|P:Midi.Chord.Pattern]] nested class. The inversion is an integer indicating how many rotations the pattern has undergone. 

 This class comes with a collection of predefined chord patterns, such as [[|F:Midi.Chord.Major]] and [[|F:Midi.Chord.Minor]]. 





---
#### Property Chord.Bass

The bass note of this chord



---
#### Property Chord.Inversion

The inversion of this chord



---
#### Property Chord.Name

The name of this chord



---
#### Property Chord.NoteSequence

The sequence of notes in this chord



---
#### Property Chord.Pattern

The pattern of this chord



---
#### Property Chord.Root

The root note of this chord



---
#### Method Chord.#ctor(Midi.Note,Midi.ChordPattern,System.Int32)

Constructs a chord from its root note, pattern, and inversion

|Name | Description |
|-----|------|
|root: | The root note of the chord|
|pattern: | The chord pattern|
|inversion: | The inversion, in [0..N-1] where N is the number of notes in pattern. |
[[T:System.ArgumentNullException|T:System.ArgumentNullException]]: pattern is null.

[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: inversion is out of range.



---
#### Method Chord.#ctor(System.String)

Constructs a chord from a string

|Name | Description |
|-----|------|
|name: | The name to parse. This is the same format as the Name property: a letter in ['A'..'G'], an optional series of accidentals (#'s or b's), then an optional inversion specified as a '/' followed by another note name. If the inversion is present it must be one of the notes in the chord. |
[[T:System.ArgumentNullException|T:System.ArgumentNullException]]: name is null.

[[T:System.ArgumentException|T:System.ArgumentException]]: cannot parse a chord from name.



---
#### Method Chord.FindMatchingChords(System.Collections.Generic.List{Midi.Pitch})

Returns a list of chords which match the set of input pitches

|Name | Description |
|-----|------|
|pitches: | Notes being analyzed|
**Returns**:  A (possibly empty) list of chords. 



---
#### Method Chord.Contains(Midi.Pitch)

Returns true if this chord contains the specified pitch

|Name | Description |
|-----|------|
|pitch: | The pitch to test|
**Returns**:  True if this chord contains the pitch. 



---
#### Field Chord.Augmented

Pattern for Augmented chords



---
#### Field Chord.Diminished

Pattern for Diminished chords



---
#### Field Chord.Major

Pattern for Major chords



---
#### Field Chord.Minor

Pattern for Minor chords



---
#### Field Chord.Seventh

Pattern for Seventh chords



---
#### Field Chord.Patterns

Array of all the built-in chord patterns



---
#### Method Chord.op_Inequality(Midi.Chord,Midi.Chord)

Inequality operator does value inequality because Chord is immutable



---
#### Method Chord.op_Equality(Midi.Chord,Midi.Chord)

Equality operator does value equality because Chord is immutable



---
#### Method Chord.Equals(System.Object)

Value equality



---
#### Method Chord.GetHashCode

Hash code



---
#### Method Chord.ToString

ToString returns the chord name

**Returns**:  The chord's name. 



---
#### Method Chord.RotateArrayLeft(System.Array,System.Array,System.Int32)

Fills dest with a rotated version of source

|Name | Description |
|-----|------|
|source: | The source array|
|dest: | The dest array, which must have the same length and underlying type as source. |
|rotation: | The number of elements to rotate to the left by|


---
## Type ChordPattern

Description of a chord's pattern starting at the root note



> This class describes the ascending sequence of notes included in a chord, starting with the root note. It is described in terms of semitones relative to root and letters relative to the root. To apply it to particular tonic, pass one of these to the constructor of [[|T:Midi.Chord]]. 



---
#### Property ChordPattern.Abbreviation

Abbreviation for this chord pattern



> This is the string used in the abbreviated name for a chord, placed immediately after the tonic and before the slashed inversion (if there is one). For example, for minor chords the abbreviation is "m", as in "Am". 



---
#### Property ChordPattern.Ascent

The ascending note sequence of the chord, in semitones-above-the-root



> This sequence starts at zero (for the root) and is monotonically increasing, each element representing a pitch in semitones above the root. 





---
#### Property ChordPattern.LetterOffsets

The sequence of letters in the chord



> This array describes what sequence of letters appears in this chord. Each element is a "letter offset", a positive integer that tell you how many letters to move up from the root for that note. It must start at zero, representing the letter for the root note. 





---
#### Property ChordPattern.Name

The name of the chord pattern



---
#### Method ChordPattern.#ctor(System.String,System.String,System.Int32[],System.Int32[])

Constructs a chord pattern

|Name | Description |
|-----|------|
|name: | The name of the chord pattern|
|abbreviation: | The abbreviation for the chord. See the [[|P:Midi.ChordPattern.Abbreviation]] property for details. |
|ascent: | Array encoding the notes in the chord. See the [[|P:Midi.ChordPattern.Ascent]] property for details. |
|letterOffsets: | Array encoding the sequence of letters in the chord. Must be the same length as ascent. See the [[|P:Midi.ChordPattern.LetterOffsets]] property for details. |
[[T:System.ArgumentException|T:System.ArgumentException]]:  ascent or letterOffsets is invalid, or they have different lengths. 

[[T:System.ArgumentNullException|T:System.ArgumentNullException]]: an argument is null.



---
#### Method ChordPattern.op_Inequality(Midi.ChordPattern,Midi.ChordPattern)

Inequality operator does value inequality



---
#### Method ChordPattern.op_Equality(Midi.ChordPattern,Midi.ChordPattern)

Equality operator does value equality



---
#### Method ChordPattern.Equals(System.Object)

Value equality



---
#### Method ChordPattern.GetHashCode

Hash code. TODO



---
#### Method ChordPattern.ToString

ToString returns the pattern name

**Returns**:  The pattern's name, such as "Major" or "Minor". 



---
#### Method ChordPattern.IsSequenceValid(System.Int32[])

 Returns true if sequence has at least two elements, starts at zero, and is monotonically increasing. 



---
## Type Clock

A clock for scheduling MIDI messages in a rate-adjustable, pausable timeline



> Clock is used for scheduling MIDI messages. Though you can always send messages synchronously with the various [[|T:fractions.OutputDevice]].Send* methods, doing so requires your code to be "ready" at the precise moment each message needs to be sent. In most cases, and especially in interactive programs, it's more convenient to describe messages that  be sent at specified points in the future, and then rely on a scheduler to make it happen. Clock is such a scheduler. 

 In the simplest case, Clock can be used to schedule a sequence of messages which is known in its entirety ahead of time. For example, this code snippet schedules two notes to play one after the other: 



######  code

```
    Clock clock(120);  // beatsPerMinute=120
    OutputDevice outputDevice = ...;
    clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Note.E4, 80, 0));
    clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Note.E4, 80, 1));
    clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Note.D4, 80, 1));
    clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Note.D4, 80, 2));
```

 At this point, four messages have been scheduled, but they haven't been sent because the clock has not started. We can start the clock with [[|M:Midi.Clock.Start]], pause it with [[|M:Midi.Clock.Stop]], and reset it with [[|M:Midi.Clock.Reset]]. We can change the beats-per-minute at any time, even as the sequence is playing. And the playing happens in a background thread, so your client code can focus on arranging the notes and controlling the clock. 

 You can even schedule new notes as the clock is playing. Generally you should schedule messages for times in the future; scheduling a message with a time in the past simply causes it to play immediately, which is probably not what you wanted. 

 In the above example, we wanted to play two notes but had to schedule four messages. This case is so common that we provide a convenience class, [[|T:Midi.NoteOnOffMessage]], which encapsulates a Note On message and its corresponding Note Off message in a single unit. We could rewrite the above example as follows: 



######  code

```
    Clock clock(120);  // beatsPerMinute=120
    OutputDevice outputDevice = ...;
    clock.Schedule(new NoteOnOffMessage(outputDevice, Channel.Channel1, Note.E4, 80, 0, 1));
    clock.Schedule(new NoteOnOffMessage(outputDevice, Channel.Channel1, Note.D4, 80, 1, 1));
```

 This works because each NoteOnOffMessage, when it is actually sent, does two things: it sends the Note On message to the output device, and  schedules the correponding Note Off message for the appropriate time in the future. This is an example of a : a message which, when triggered, schedules additional events for the future. 

 You can design your own self-propagating messages by subclassing from [[|T:Midi.Message]]. For example, you could make a self-propagating MetronomeMessage which keeps a steady beat by always scheduling the  MetronomeMessage when it plays the current beat. However, subclassing can be tedious, and it is usually preferable to use [[|T:Midi.CallbackMessage]] to call-out to your own code instead. 





---
#### Field Clock.isSchedulerThread

 Thread-local, set to true in the scheduler thread, false in all other threads. 



---
#### Method Clock.#ctor(System.Single)

Constructs a midi clock with a given beats-per-minute

|Name | Description |
|-----|------|
|beatsPerMinute: | The initial beats-per-minute, which can be changed later. |


> When constructed, the clock is not running, and so [[|P:Midi.Clock.Time]] will return zero. Call [[|M:Midi.Clock.Start]] when you are ready for the clock to start progressing (and scheduled messages to actually trigger). 



[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: beatsPerMinute is non-positive



---
#### Property Clock.BeatsPerMinute

Beats per minute property



> Setting this property changes the rate at which the clock progresses. If the clock is currently running, the new rate is effectively immediately. 





---
#### Property Clock.IsRunning

Returns true if this clock is currently running



---
#### Property Clock.Time

This clock's current time in beats



> Normally, this method polls the clock's current time, and thus changes from moment to moment as long as the clock is running. However, when called from the scheduler thread (that is, from a [[ Message.SendNow |M:Midi.Message.SendNow]] method or a [[|T:Midi.CallbackMessage]]), it returns the precise time at which the message was scheduled. 

 For example, suppose a callback was scheduled for time T, and the scheduler managed to call that callback at time T+delta. In the callback, Time will return T for the duration of the callback. In any other thread, Time would return approximately T+delta. 





---
#### Method Clock.Reset

Resets the clock to zero and discards pending messages



> This method resets the clock to zero and discards any scheduled but as-yet-unsent messages. It may only be called when the clock is not running. 



[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: Clock is running.



---
#### Method Clock.Schedule(Midi.Message)

Schedules a single message based on its beatTime

|Name | Description |
|-----|------|
|message: | The message to schedule|


> This method schedules a message to be sent at the time indicated in the message's [[|P:Midi.Message.Time]] property. It may be called at any time, whether the clock is running or not. The message will not be sent until the clock progresses to the specified time. (If the clock is never started, or is paused before that time and not re-started, then the message will never be sent.) 

 If a message is scheduled for a time that has already passed, then the scheduler will send the message at the first opportunity. 





---
#### Method Clock.Schedule(System.Collections.Generic.List{Midi.Message},System.Single)

 Schedules a collection of messages, applying an optional time delta to the scheduled beatTime. 

|Name | Description |
|-----|------|
|messages: | The message to send |
|beatTimeDelta: | The delta to apply (or zero)|


---
#### Method Clock.Start

Starts or resumes the clock



> This method causes the clock to start progressing at the rate given in the [[|P:Midi.Clock.BeatsPerMinute]] property. It may only be called when the clock is not yet rnuning. 

 If this is the first time Start is called, the clock starts at time zero and progresses from there. If the clock was previously started, stopped, and not reset, then Start effectively "unpauses" the clock, picking up at the left-off time, and resuming scheduling of any as-yet-unsent messages. 

 This method creates a new thread which runs in the background and sends messages at the appropriate times. All [[ Message.SendNow |M:Midi.Message.SendNow]] methods and [[|T:Midi.CallbackMessage]] s will be called in that thread. 

 The scheduler thread is joined (shut down) in [[|M:Midi.Clock.Stop]]. 



[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: Clock is already running.



---
#### Method Clock.Stop

Stops the clock (but does not reset its time or discard pending events)



> This method stops the progression of the clock. It may only be called when the clock is running. 

 Any scheduled but as-yet-unsent messages remain in the queue. A consecutive call to [[|M:Midi.Clock.Start]] can re-start the progress of the clock, or [[|M:Midi.Clock.Reset]] can discard pending messages and reset the clock to zero. 

 This method waits for any in-progress messages to be processed and joins (shuts down) the scheduler thread before returning, so when it returns you can be sure that no more messages will be sent or callbacks invoked. 

 It is illegal to call Stop from the scheduler thread (ie, from any [[ Message.SendNow |M:Midi.Message.SendNow]] method or [[|T:Midi.CallbackMessage]]. If a callback really needs to stop the clock, consider using BeginInvoke to arrange for it to happen in another thread. 



[[T:System.InvalidOperationException|T:System.InvalidOperationException]]:  Clock is not running or Stop was invoked from the scheduler thread. 



---
#### Method Clock.MillisecondsUntil(System.Single)

 Returns the number of milliseconds from now until the specified beat time. 

|Name | Description |
|-----|------|
|beatTime: | The beat time|
**Returns**:  The positive number of milliseconds, or 0 if beatTime is in the past. 



---
#### Method Clock.ThreadRun

Worker thread function



---
## Type Control

MIDI Control, used in Control Change messages



> In MIDI, Control Change messages are used to influence various auxiliary "controls" on a device, such as knobs, levers, and pedals. Controls are specified with integers in [0..127]. This enum provides an incomplete list of controls, because most controls are too obscure to document effetively here. Even for the ones listed here, the details of how the value is interpreted are arcane. Please see the MIDI spec for details. 

 The most commonly used control is SustainPedal, which is considered off when < 64, on when > 64. 

 This enum has extension methods, such as [[|M:Midi.ControlExtensionMethods.Name(Midi.Control)]] and [[|M:Midi.ControlExtensionMethods.IsValid(Midi.Control)]], defined in [[|T:Midi.ControlExtensionMethods]]. 





---
#### Field Control.ModulationWheel

General MIDI Control--See MIDI spec for details



---
#### Field Control.DataEntryMSB

General MIDI Control--See MIDI spec for details



---
#### Field Control.Volume

General MIDI Control--See MIDI spec for details



---
#### Field Control.Pan

General MIDI Control--See MIDI spec for details



---
#### Field Control.Expression

General MIDI Control--See MIDI spec for details



---
#### Field Control.DataEntryLSB

General MIDI Control--See MIDI spec for details



---
#### Field Control.SustainPedal

General MIDI Control--See MIDI spec for details



---
#### Field Control.ReverbLevel

General MIDI Control--See MIDI spec for details



---
#### Field Control.TremoloLevel

General MIDI Control--See MIDI spec for details



---
#### Field Control.ChorusLevel

General MIDI Control--See MIDI spec for details



---
#### Field Control.CelesteLevel

General MIDI Control--See MIDI spec for details



---
#### Field Control.PhaserLevel

General MIDI Control--See MIDI spec for details



---
#### Field Control.NonRegisteredParameterLSB

General MIDI Control--See MIDI spec for details



---
#### Field Control.NonRegisteredParameterMSB

General MIDI Control--See MIDI spec for details



---
#### Field Control.RegisteredParameterNumberLSB

General MIDI Control--See MIDI spec for details



---
#### Field Control.RegisteredParameterNumberMSB

General MIDI Control--See MIDI spec for details



---
#### Field Control.AllControllersOff

General MIDI Control--See MIDI spec for details



---
#### Field Control.AllNotesOff

General MIDI Control--See MIDI spec for details



---
## Type ControlExtensionMethods

Extension methods for the Control enum

 Be sure to "using midi" if you want to use these as extension methods. 

---
#### Field ControlExtensionMethods.ControlNames

Table of control names



---
#### Method ControlExtensionMethods.IsValid(Midi.Control)

Returns true if the specified control is valid

|Name | Description |
|-----|------|
|control: | The Control to test|


---
#### Method ControlExtensionMethods.Name(Midi.Control)

Returns the human-readable name of a MIDI control

|Name | Description |
|-----|------|
|control: | The control|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The control is out-of-range.



---
#### Method ControlExtensionMethods.Validate(Midi.Control)

Throws an exception if control is not valid

|Name | Description |
|-----|------|
|control: | The control to validate|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The control is out-of-range.



---
## Type ControlChangeMessage

Control change message



---
#### Method ControlChangeMessage.#ctor(Midi.DeviceBase,Midi.Channel,Midi.Control,System.Int32,System.Single)

Construts a Control Change message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|channel: | Channel, 0..15, 10 reserved for percussion|
|control: | Control, 0..119 |
|value: | Value, 0..127|
|time: | The timestamp for this message|


---
#### Property ControlChangeMessage.Control

The control for this message



---
#### Property ControlChangeMessage.Value

Value, 0..127



---
#### Method ControlChangeMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method ControlChangeMessage.SendNow

Sends this message immediately



---
## Type DeviceBase

Common base class for input and output devices

 This base class exists mainly so that input and output devices can both go into the same kinds of MidiMessages. 

---
#### Method DeviceBase.#ctor(System.String)

Protected constructor

|Name | Description |
|-----|------|
|name: | The name of this device|


---
#### Property DeviceBase.Name

The name of this device



---
## Type DeviceException

Exception thrown when an operation on a MIDI device cannot be satisfied



---
#### Method DeviceException.#ctor





---
#### Method DeviceException.#ctor(System.String)



|Name | Description |
|-----|------|
|message: ||


---
#### Method DeviceException.#ctor(System.String,System.Exception)



|Name | Description |
|-----|------|
|message: ||
|innerException: ||


---
## Type DeviceMessage

Base class for messages relevant to a specific device



---
#### Method DeviceMessage.#ctor(Midi.DeviceBase,System.Single)

Protected constructor



---
#### Property DeviceMessage.Device

The device from which this message originated, or for which it is destined



---
## Type Instrument

General MIDI instrument, used in Program Change messages



> The MIDI protocol defines a Program Change message, which can be used to switch a device among "presets". The General MIDI specification further standardizes those presets into the specific instruments in this enum. General-MIDI-compliant devices will have these particular instruments; non-GM devices may have other instruments. 

 MIDI instruments are one-indexed in the spec, but they're zero-indexed in code, so we have them zero-indexed here. 

 This enum has extension methods, such as [[|M:Midi.InstrumentExtensions.Name(Midi.Instrument)]] and [[|M:Midi.InstrumentExtensions.IsValid(Midi.Instrument)]], defined in [[|T:Midi.InstrumentExtensions]]. 





---
#### Field Instrument.AcousticGrandPiano

General MIDI instrument 0 ("Acoustic Grand Piano")



---
#### Field Instrument.BrightAcousticPiano

General MIDI instrument 1 ("Bright Acoustic Piano")



---
#### Field Instrument.ElectricGrandPiano

General MIDI instrument 2 ("Electric Grand Piano")



---
#### Field Instrument.HonkyTonkPiano

General MIDI instrument 3 ("Honky Tonk Piano")



---
#### Field Instrument.ElectricPiano1

General MIDI instrument 4 ("Electric Piano 1")



---
#### Field Instrument.ElectricPiano2

General MIDI instrument 5 ("Electric Piano 2")



---
#### Field Instrument.Harpsichord

General MIDI instrument 6 ("Harpsichord")



---
#### Field Instrument.Clavinet

General MIDI instrument 7 ("Clavinet")



---
#### Field Instrument.Celesta

General MIDI instrument 8 ("Celesta")



---
#### Field Instrument.Glockenspiel

General MIDI instrument 9 ("Glockenspiel")



---
#### Field Instrument.MusicBox

General MIDI instrument 10 ("Music Box")



---
#### Field Instrument.Vibraphone

General MIDI instrument 11 ("Vibraphone")



---
#### Field Instrument.Marimba

General MIDI instrument 12 ("Marimba")



---
#### Field Instrument.Xylophone

General MIDI instrument 13 ("Xylophone")



---
#### Field Instrument.TubularBells

General MIDI instrument 14 ("Tubular Bells")



---
#### Field Instrument.Dulcimer

General MIDI instrument 15 ("Dulcimer")



---
#### Field Instrument.DrawbarOrgan

General MIDI instrument 16 ("Drawbar Organ")



---
#### Field Instrument.PercussiveOrgan

General MIDI instrument 17 ("Percussive Organ")



---
#### Field Instrument.RockOrgan

General MIDI instrument 18 ("Rock Organ")



---
#### Field Instrument.ChurchOrgan

General MIDI instrument 19 ("Church Organ")



---
#### Field Instrument.ReedOrgan

General MIDI instrument 20 ("Reed Organ")



---
#### Field Instrument.Accordion

General MIDI instrument 21 ("Accordion")



---
#### Field Instrument.Harmonica

General MIDI instrument 22 ("Harmonica")



---
#### Field Instrument.TangoAccordion

General MIDI instrument 23 ("Tango Accordion")



---
#### Field Instrument.AcousticGuitarNylon

General MIDI instrument 24 ("Acoustic Guitar (nylon)")



---
#### Field Instrument.AcousticGuitarSteel

General MIDI instrument 25 ("Acoustic Guitar (steel)")



---
#### Field Instrument.ElectricGuitarJazz

General MIDI instrument 26 ("Electric Guitar (jazz)")



---
#### Field Instrument.ElectricGuitarClean

General MIDI instrument 27 ("Electric Guitar (clean)")



---
#### Field Instrument.ElectricGuitarMuted

General MIDI instrument 28 ("Electric Guitar (muted)")



---
#### Field Instrument.OverdrivenGuitar

General MIDI instrument 29 ("Overdriven Guitar")



---
#### Field Instrument.DistortionGuitar

General MIDI instrument 30 ("Distortion Guitar")



---
#### Field Instrument.GuitarHarmonics

General MIDI instrument 31 ("Guitar Harmonics")



---
#### Field Instrument.AcousticBass

General MIDI instrument 32 ("Acoustic Bass")



---
#### Field Instrument.ElectricBassFinger

General MIDI instrument 33 ("Electric Bass (finger)")



---
#### Field Instrument.ElectricBassPick

General MIDI instrument 34 ("Electric Bass (pick)")



---
#### Field Instrument.FretlessBass

General MIDI instrument 35 ("Fretless Bass")



---
#### Field Instrument.SlapBass1

General MIDI instrument 36 ("Slap Bass 1")



---
#### Field Instrument.SlapBass2

General MIDI instrument 37 ("Slap Bass 2")



---
#### Field Instrument.SynthBass1

General MIDI instrument 38 ("Synth Bass 1")



---
#### Field Instrument.SynthBass2

General MIDI instrument 39("Synth Bass 2")



---
#### Field Instrument.Violin

General MIDI instrument 40 ("Violin")



---
#### Field Instrument.Viola

General MIDI instrument 41 ("Viola")



---
#### Field Instrument.Cello

General MIDI instrument 42 ("Cello")



---
#### Field Instrument.Contrabass

General MIDI instrument 43 ("Contrabass")



---
#### Field Instrument.TremoloStrings

General MIDI instrument 44 ("Tremolo Strings")



---
#### Field Instrument.PizzicatoStrings

General MIDI instrument 45 ("Pizzicato Strings")



---
#### Field Instrument.OrchestralHarp

General MIDI instrument 46 ("Orchestral Harp")



---
#### Field Instrument.Timpani

General MIDI instrument 47 ("Timpani")



---
#### Field Instrument.StringEnsemble1

General MIDI instrument 48 ("String Ensemble 1")



---
#### Field Instrument.StringEnsemble2

General MIDI instrument 49 ("String Ensemble 2")



---
#### Field Instrument.SynthStrings1

General MIDI instrument 50 ("Synth Strings 1")



---
#### Field Instrument.SynthStrings2

General MIDI instrument 51 ("Synth Strings 2")



---
#### Field Instrument.ChoirAahs

General MIDI instrument 52 ("Choir Aahs")



---
#### Field Instrument.VoiceOohs

General MIDI instrument 53 ("Voice oohs")



---
#### Field Instrument.SynthVoice

General MIDI instrument 54 ("Synth Voice")



---
#### Field Instrument.OrchestraHit

General MIDI instrument 55 ("Orchestra Hit")



---
#### Field Instrument.Trumpet

General MIDI instrument 56 ("Trumpet")



---
#### Field Instrument.Trombone

General MIDI instrument 57 ("Trombone")



---
#### Field Instrument.Tuba

General MIDI instrument 58 ("Tuba")



---
#### Field Instrument.MutedTrumpet

General MIDI instrument 59 ("Muted Trumpet")



---
#### Field Instrument.FrenchHorn

General MIDI instrument 60 ("French Horn")



---
#### Field Instrument.BrassSection

General MIDI instrument 61 ("Brass Section")



---
#### Field Instrument.SynthBrass1

General MIDI instrument 62 ("Synth Brass 1")



---
#### Field Instrument.SynthBrass2

General MIDI instrument 63 ("Synth Brass 2")



---
#### Field Instrument.SopranoSax

General MIDI instrument 64 ("Soprano Sax")



---
#### Field Instrument.AltoSax

General MIDI instrument 65 ("Alto Sax")



---
#### Field Instrument.TenorSax

General MIDI instrument 66 ("Tenor Sax")



---
#### Field Instrument.BaritoneSax

General MIDI instrument 67 ("Baritone Sax")



---
#### Field Instrument.Oboe

General MIDI instrument 68 ("Oboe")



---
#### Field Instrument.EnglishHorn

General MIDI instrument 69 ("English Horn")



---
#### Field Instrument.Bassoon

General MIDI instrument 70 ("Bassoon")



---
#### Field Instrument.Clarinet

General MIDI instrument 71 ("Clarinet")



---
#### Field Instrument.Piccolo

General MIDI instrument 72 ("Piccolo")



---
#### Field Instrument.Flute

General MIDI instrument 73 ("Flute")



---
#### Field Instrument.Recorder

General MIDI instrument 74 ("Recorder")



---
#### Field Instrument.PanFlute

General MIDI instrument 75 ("PanFlute")



---
#### Field Instrument.BlownBottle

General MIDI instrument 76 ("Blown Bottle")



---
#### Field Instrument.Shakuhachi

General MIDI instrument 77 ("Shakuhachi")



---
#### Field Instrument.Whistle

General MIDI instrument 78 ("Whistle")



---
#### Field Instrument.Ocarina

General MIDI instrument 79 ("Ocarina")



---
#### Field Instrument.Lead1Square

General MIDI instrument 80 ("Lead 1 (square)")



---
#### Field Instrument.Lead2Sawtooth

General MIDI instrument 81 ("Lead 2 (sawtooth)")



---
#### Field Instrument.Lead3Calliope

General MIDI instrument 82 ("Lead 3 (calliope)")



---
#### Field Instrument.Lead4Chiff

General MIDI instrument 83 ("Lead 4 (chiff)")



---
#### Field Instrument.Lead5Charang

General MIDI instrument 84 ("Lead 5 (charang)")



---
#### Field Instrument.Lead6Voice

General MIDI instrument 85 ("Lead 6 (voice)")



---
#### Field Instrument.Lead7Fifths

General MIDI instrument 86 ("Lead 7 (fifths)")



---
#### Field Instrument.Lead8BassPlusLead

General MIDI instrument 87 ("Lead 8 (bass + lead)")



---
#### Field Instrument.Pad1NewAge

General MIDI instrument 88 ("Pad 1 (new age)")



---
#### Field Instrument.Pad2Warm

General MIDI instrument 89 ("Pad 2 (warm)")



---
#### Field Instrument.Pad3Polysynth

General MIDI instrument 90 ("Pad 3 (polysynth)")



---
#### Field Instrument.Pad4Choir

General MIDI instrument 91 ("Pad 4 (choir)")



---
#### Field Instrument.Pad5Bowed

General MIDI instrument 92 ("Pad 5 (bowed)")



---
#### Field Instrument.Pad6Metallic

General MIDI instrument 93 ("Pad 6 (metallic)")



---
#### Field Instrument.Pad7Halo

General MIDI instrument 94 ("Pad 7 (halo)")



---
#### Field Instrument.Pad8Sweep

General MIDI instrument 95 ("Pad 8 (sweep)")



---
#### Field Instrument.FX1Rain

General MIDI instrument 96 ("FX 1 (rain)")



---
#### Field Instrument.FX2Soundtrack

General MIDI instrument 97 ("FX 2 (soundtrack)")



---
#### Field Instrument.FX3Crystal

General MIDI instrument 98 ("FX 3 (crystal)")



---
#### Field Instrument.FX4Atmosphere

General MIDI instrument 99 ("FX 4 (atmosphere)")



---
#### Field Instrument.FX5Brightness

General MIDI instrument 100 ("FX 5 (brightness)")



---
#### Field Instrument.FX6Goblins

General MIDI instrument 101 ("FX 6 (goblins)")



---
#### Field Instrument.FX7Echoes

General MIDI instrument 102 ("FX 7 (echoes)")



---
#### Field Instrument.FX8SciFi

General MIDI instrument 103 ("FX 8 (sci-fi)")



---
#### Field Instrument.Sitar

General MIDI instrument 104 ("Sitar")



---
#### Field Instrument.Banjo

General MIDI instrument 105 ("Banjo")



---
#### Field Instrument.Shamisen

General MIDI instrument 106 ("Shamisen")



---
#### Field Instrument.Koto

General MIDI instrument 107 ("Koto")



---
#### Field Instrument.Kalimba

General MIDI instrument 108 ("Kalimba")



---
#### Field Instrument.Bagpipe

General MIDI instrument 109 ("Bagpipe")



---
#### Field Instrument.Fiddle

General MIDI instrument 110 ("Fiddle")



---
#### Field Instrument.Shanai

General MIDI instrument 111 ("Shanai")



---
#### Field Instrument.TinkleBell

General MIDI instrument 112 ("Tinkle Bell")



---
#### Field Instrument.Agogo

General MIDI instrument 113 (Agogo"")



---
#### Field Instrument.SteelDrums

General MIDI instrument 114 ("Steel Drums")



---
#### Field Instrument.Woodblock

General MIDI instrument 115 ("Woodblock")



---
#### Field Instrument.TaikoDrum

General MIDI instrument 116 ("Taiko Drum")



---
#### Field Instrument.MelodicTom

General MIDI instrument 117 ("Melodic Tom")



---
#### Field Instrument.SynthDrum

General MIDI instrument 118 ("Synth Drum")



---
#### Field Instrument.ReverseCymbal

General MIDI instrument 119 ("Reverse Cymbal")



---
#### Field Instrument.GuitarFretNoise

General MIDI instrument 120 ("Guitar Fret Noise")



---
#### Field Instrument.BreathNoise

General MIDI instrument 121 ("Breath Noise")



---
#### Field Instrument.Seashore

General MIDI instrument 122 ("Seashore")



---
#### Field Instrument.BirdTweet

General MIDI instrument 123 ("Bird Tweet")



---
#### Field Instrument.TelephoneRing

General MIDI instrument 124 ("Telephone Ring")



---
#### Field Instrument.Helicopter

General MIDI instrument 125 ("Helicopter")



---
#### Field Instrument.Applause

General MIDI instrument 126 ("Applause")



---
#### Field Instrument.Gunshot

General MIDI instrument 127 ("Gunshot")



---
## Type InstrumentExtensions

Extension methods for the Instrument enum



---
#### Field InstrumentExtensions.InstrumentNames

General Midi instrument names, used by GetInstrumentName()



---
#### Method InstrumentExtensions.IsValid(Midi.Instrument)

Returns true if the specified instrument is valid

|Name | Description |
|-----|------|
|instrument: | The instrument to test|


---
#### Method InstrumentExtensions.Name(Midi.Instrument)

Returns the human-readable name of a MIDI instrument

|Name | Description |
|-----|------|
|instrument: | The instrument|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The instrument is out-of-range.



---
#### Method InstrumentExtensions.Validate(Midi.Instrument)

Throws an exception if instrument is not valid

|Name | Description |
|-----|------|
|instrument: | The instrument to validate|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The instrument is out-of-range.



---
## Type Interval

Interval measuring the relationship between pitches



> This enum is simply for making interval operations more explicit. When adding to or subtracting from the [[|T:Midi.Pitch]] enum, one can either use ints... 



######  code

```
    Pitch p = Pitch.C4 + 5;
```

 ...or use the Interval enum, cast to int... 



######  code

```
    Pitch p = Pitch.C4 + (int)Interval.PerfectFourth;
```

 These two examples are equivalent. The benefit of the latter is simply that it makes the intention more explicit. 

 This enum has extension methods, such as [[|M:Midi.IntervalExtensions.Name(Midi.Interval)]], defined in [[|T:Midi.IntervalExtensions]]. 





---
#### Field Interval.Unison

Unison interval, 0 semitones



---
#### Field Interval.Semitone

Semitone interval, 1 semitone



---
#### Field Interval.WholeTone

Whole Tone interval, 2 semitones



---
#### Field Interval.MinorThird

Minor Third interval, 3 semitones



---
#### Field Interval.MajorThird

Major Third interval, 4 semitones



---
#### Field Interval.PerfectFourth

Perfect Fourth interval, 5 semitones



---
#### Field Interval.Tritone

Tritone interval, 6 semitones



---
#### Field Interval.PerfectFifth

Perfect Fifth interval, 7 semitones



---
#### Field Interval.MinorSixth

Minor Sixth interval, 8 semitones



---
#### Field Interval.MajorSixth

Major Sixth interval, 9 semitones



---
#### Field Interval.MinorSeventh

Minor seventh interval, 10 semitones



---
#### Field Interval.MajorSeventh

Major Seventh interval, 11 semitones



---
#### Field Interval.Octave

Octave interval, 12 semitones



---
## Type IntervalExtensions

Extension methods for the Interval enum



---
#### Field IntervalExtensions.IntervalNames

Table of interval names



---
#### Method IntervalExtensions.Name(Midi.Interval)

Returns the human-readable name of an interval

|Name | Description |
|-----|------|
|interval: | The interval|
**Returns**:  The human-readable name. If the interval is less than an octave, it gives the standard term (eg, "Major third"). If the interval is more than an octave, it gives the number of semitones in the interval. 



---
## Type Message

Base class for all MIDI messages



---
#### Method Message.#ctor(System.Single)

Protected constructor

|Name | Description |
|-----|------|
|time: | The timestamp for this message|


---
#### Property Message.Time

Milliseconds since the music started



---
#### Method Message.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method Message.SendNow

Sends this message immediately



---
## Type Note

A letter and accidental, which together form an octave-independent note



> This class lets you define a note by combining a letters A-G with accidentals (sharps and flats). Examples of notes are D, B#, and Gbb. This is the conventional way to refer to notes in an octave independent way. 

 Each note unambiguously identifies a pitch (modulo octave), but each pitch has potentially many notes. For example, the notes F, E#, D###, and Gbb all resolve to the same pitch, though the last two names are unlikely to be used in practice. 





---
#### Field Note.DoubleFlat

Double-flat accidental value



---
#### Field Note.DoubleSharp

Double-sharp accidental value



---
#### Field Note.Flat

Flat accidental value



---
#### Field Note.Natural

Natural accidental value



---
#### Field Note.Sharp

Sharp accidental value



---
#### Field Note.LetterToNote

 Table mapping (letter-'A') to the Note in octave -1, used to compute positionInOctave. 



---
#### Method Note.#ctor(System.Char)

Constructs a note from a letter

|Name | Description |
|-----|------|
|letter: | The letter, which must be in ['A'..'G']|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: letter is out of range.



---
#### Method Note.#ctor(System.String)

Constructs a note from a string

|Name | Description |
|-----|------|
|name: | The name to parse. Must begin with a letter in ['A'..'G'], then optionally be followed by a series of '#' (sharps) or a series of 'b' (flats). |
[[T:System.ArgumentNullException|T:System.ArgumentNullException]]: name is null.

[[T:System.ArgumentException|T:System.ArgumentException]]: name cannot be parsed.



---
#### Method Note.#ctor(System.Char,System.Int32)

Constructs a note name from a letter and accidental

|Name | Description |
|-----|------|
|letter: | The letter, which must be in ['A'..'G']|
|accidental: | The accidental. Zero means natural, positive values are sharp by that many semitones, and negative values are flat by that many semitones. Likely values are [[|F:Midi.Note.Natural]] (0), [[|F:Midi.Note.Sharp]] (1), [[|F:Midi.Note.DoubleSharp]] (2), [[|F:Midi.Note.Flat]] (-1), and [[|F:Midi.Note.DoubleFlat]] (-2). |
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: letter is out of range.



---
#### Property Note.Accidental

The accidental for this note name



> Zero means natural, positive values are sharp by that many semitones, and negative values are flat by that many semitones. Likely values are [[|F:Midi.Note.Natural]] (0), [[|F:Midi.Note.Sharp]] (1), [[|F:Midi.Note.DoubleSharp]] (2), [[|F:Midi.Note.Flat]] (-1), and [[|F:Midi.Note.DoubleFlat]] (-2). 





---
#### Property Note.Letter

The letter for this note name, in ['A'..'G']



---
#### Property Note.PositionInOctave

This note's position in the octave, where octaves start at each C



---
#### Method Note.op_Inequality(Midi.Note,Midi.Note)

Inequality operator does value comparison



---
#### Method Note.op_Equality(Midi.Note,Midi.Note)

Equality operator does value comparison



---
#### Method Note.ParseNote(System.String,System.Int32@)

Parses a Note from s, starting at position pos

|Name | Description |
|-----|------|
|s: | The string to parse from|
|pos: | The position to start at. On success, advances pos to after the end of the note. |
**Returns**:  The note. 

[[T:System.ArgumentException|T:System.ArgumentException]]: A note cannot be parsed.



> This function must find a valid letter at s[pos], and then optionally a sequence of '#' (sharps) or 'b' (flats). It finds as many of the accidental as it can and then stops at the first character that can't be part of the accidental. 





---
#### Method Note.Equals(System.Object)

Value equality for Note



---
#### Method Note.GetHashCode

Hash code



---
#### Method Note.IsEharmonicWith(Midi.Note)

Returns true if this note name is enharmonic with otherNote

|Name | Description |
|-----|------|
|otherNote: | Another note|
**Returns**:  True if the names can refer to the same pitch. 



---
#### Method Note.PitchAtOrAbove(Midi.Pitch)

Returns the pitch for this note that is at or above nearPitch

|Name | Description |
|-----|------|
|nearPitch: | The pitch from which the search is based|
**Returns**:  The pitch for this note at or above nearPitch. 



---
#### Method Note.PitchAtOrBelow(Midi.Pitch)

Returns the pitch for this note that is at or below nearPitch

|Name | Description |
|-----|------|
|nearPitch: | The pitch from which the search is based|
**Returns**:  The pitch for this note at or below nearPitch. 



---
#### Method Note.PitchInOctave(System.Int32)

Returns the pitch for this note in the specified octave

|Name | Description |
|-----|------|
|octave: | The octave, where octaves begin at each C and Middle C is the first note in octave 4. |
**Returns**:  The pitch with this name in the specified octave. 



---
#### Method Note.SemitonesDownTo(Midi.Note)

Returns the number of semitones it takes to move down to the next otherNote

|Name | Description |
|-----|------|
|otherNote: | The other note|
**Returns**:  The number of semitones. 



---
#### Method Note.SemitonesUpTo(Midi.Note)

Returns the number of semitones it takes to move up to the next otherNote

|Name | Description |
|-----|------|
|otherNote: | The other note|
**Returns**:  The number of semitones. 



---
#### Method Note.ToString

ToString returns the note name

**Returns**:  The note name with '#' for sharp and 'b' for flat. For example, "G", "A#", "Cb", "Fbb". 



---
## Type NoteMessage

Base class for messages relevant to a specific note



---
#### Method NoteMessage.#ctor(Midi.DeviceBase,Midi.Channel,Midi.Pitch,System.Int32,System.Single)

Protected constructor



---
#### Property NoteMessage.Pitch

The pitch for this note message



---
#### Property NoteMessage.Velocity

Velocity, 0..127



---
## Type NoteOffMessage

Note Off message



---
#### Method NoteOffMessage.#ctor(Midi.DeviceBase,Midi.Channel,Midi.Pitch,System.Int32,System.Single)

Constructs a Note Off message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|channel: | Channel, 0..15, 10 reserved for percussion|
|pitch: | The pitch for this note message|
|velocity: | Velocity, 0..127|
|time: | The timestamp for this message|


---
#### Method NoteOffMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method NoteOffMessage.SendNow

Sends this message immediately



---
## Type NoteOnMessage

Note On message



---
#### Method NoteOnMessage.#ctor(Midi.DeviceBase,Midi.Channel,Midi.Pitch,System.Int32,System.Single)

Constructs a Note On message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|channel: | Channel, 0..15, 10 reserved for percussion|
|pitch: | The pitch for this note message|
|velocity: | Velocity, 0..127|
|time: | The timestamp for this message|


---
#### Method NoteOnMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method NoteOnMessage.MakeTimeShiftedCopy(System.Single,System.Int32)

 Returns a copy of this message, shifted in time by the specified amount with the new velocity. 



---
#### Method NoteOnMessage.MakeTimeShiftedOffCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method NoteOnMessage.SendNow

Sends this message immediately



---
## Type NoteOnOffMessage

A Note On message which schedules its own Note Off message when played



---
#### Method NoteOnOffMessage.#ctor(Midi.DeviceBase,Midi.Channel,Midi.Pitch,System.Int32,System.Single,Midi.Clock,System.Single)

Constructs a Note On/Off message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|channel: | Channel, 0..15, 10 reserved for percussion|
|pitch: | The pitch for this note message|
|velocity: | Velocity, 0..127|
|time: | The timestamp for this message|
|clock: | The clock that should schedule the off message|
|duration: | Time delay between on message and off messasge|


---
#### Property NoteOnOffMessage.Clock

The clock used to schedule the follow-up message



---
#### Property NoteOnOffMessage.Duration

Time delay between the Note On and the Note Off



---
#### Method NoteOnOffMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method NoteOnOffMessage.SendNow

Sends this message immediately



---
## Type Percussion

General MIDI percussion note



> In General MIDI, notes played on [[|F:Midi.Channel.Channel10]] make the following percussion sounds, regardless of any [[ Program Change |M:fractions.OutputDevice.SendProgramChange(Midi.Channel,Midi.Instrument)]] messages on that channel. 

 This enum is used with [[ OutputDevice.SendPercussion |M:fractions.OutputDevice.SendPercussion(Midi.Percussion,System.Int32)]] and [[|T:Midi.PercussionMessage]]. Equivalently, when cast to [[|T:Midi.Note]] it can be used with [[ OutputDevice.SendNoteOn |M:fractions.OutputDevice.SendNoteOn(Midi.Channel,Midi.Pitch,System.Int32)]] and [[|T:Midi.NoteOnMessage]] on [[|F:Midi.Channel.Channel10]]. 

 This enum has extension methods, such as [[|M:Midi.PercussionExtensions.Name(Midi.Percussion)]] and [[|M:Midi.PercussionExtensions.IsValid(Midi.Percussion)]], defined in [[|T:Midi.PercussionExtensions]]. 





---
#### Field Percussion.BassDrum2

General MIDI percussion 35 ("Bass Drum 2")



---
#### Field Percussion.BassDrum1

General MIDI percussion 36 ("Bass Drum 1")



---
#### Field Percussion.SideStick

General MIDI percussion 37 ("Side Stick")



---
#### Field Percussion.SnareDrum1

General MIDI percussion 38 ("Snare Drum 1")



---
#### Field Percussion.HandClap

General MIDI percussion 39 ("Hand Clap")



---
#### Field Percussion.SnareDrum2

General MIDI percussion 40 ("Snare Drum 2")



---
#### Field Percussion.LowTom2

General MIDI percussion 41 ("Low Tom 2")



---
#### Field Percussion.ClosedHiHat

General MIDI percussion 42 ("Closed Hi-hat")



---
#### Field Percussion.LowTom1

General MIDI percussion 43 ("Low Tom 1")



---
#### Field Percussion.PedalHiHat

General MIDI percussion 44 ("Pedal Hi-hat")



---
#### Field Percussion.MidTom2

General MIDI percussion 45 ("Mid Tom 2")



---
#### Field Percussion.OpenHiHat

General MIDI percussion 46 ("Open Hi-hat")



---
#### Field Percussion.MidTom1

General MIDI percussion 47 ("Mid Tom 1")



---
#### Field Percussion.HighTom2

General MIDI percussion 48 ("High Tom 2")



---
#### Field Percussion.CrashCymbal1

General MIDI percussion 49 ("Crash Cymbal 1")



---
#### Field Percussion.HighTom1

General MIDI percussion 50 ("High Tom 1")



---
#### Field Percussion.RideCymbal1

General MIDI percussion 51 ("Ride Cymbal 1")



---
#### Field Percussion.ChineseCymbal

General MIDI percussion 52 ("Chinese Cymbal")



---
#### Field Percussion.RideBell

General MIDI percussion 53 ("Ride Bell")



---
#### Field Percussion.Tambourine

General MIDI percussion 54 ("Tambourine")



---
#### Field Percussion.SplashCymbal

General MIDI percussion 55 ("Splash Cymbal")



---
#### Field Percussion.Cowbell

General MIDI percussion 56 ("Cowbell")



---
#### Field Percussion.CrashCymbal2

General MIDI percussion 57 ("Crash Cymbal 2")



---
#### Field Percussion.VibraSlap

General MIDI percussion 58 ("Vibra Slap")



---
#### Field Percussion.RideCymbal2

General MIDI percussion 59 ("Ride Cymbal 2")



---
#### Field Percussion.HighBongo

General MIDI percussion 60 ("High Bongo")



---
#### Field Percussion.LowBongo

General MIDI percussion 61 ("Low Bongo")



---
#### Field Percussion.MuteHighConga

General MIDI percussion 62 ("Mute High Conga")



---
#### Field Percussion.OpenHighConga

General MIDI percussion 63 ("Open High Conga")



---
#### Field Percussion.LowConga

General MIDI percussion 64 ("Low Conga")



---
#### Field Percussion.HighTimbale

General MIDI percussion 65 ("High Timbale")



---
#### Field Percussion.LowTimbale

General MIDI percussion 66 ("Low Timbale")



---
#### Field Percussion.HighAgogo

General MIDI percussion 67 ("High Agogo")



---
#### Field Percussion.LowAgogo

General MIDI percussion 68 ("Low Agogo")



---
#### Field Percussion.Cabasa

General MIDI percussion 69 ("Cabasa")



---
#### Field Percussion.Maracas

General MIDI percussion 70 ("Maracas")



---
#### Field Percussion.ShortWhistle

General MIDI percussion 71 ("Short Whistle")



---
#### Field Percussion.LongWhistle

General MIDI percussion 72 ("Long Whistle")



---
#### Field Percussion.ShortGuiro

General MIDI percussion 73 ("Short Guiro")



---
#### Field Percussion.LongGuiro

General MIDI percussion 74 ("Long Guiro")



---
#### Field Percussion.Claves

General MIDI percussion 75 ("Claves")



---
#### Field Percussion.HighWoodBlock

General MIDI percussion 76 ("High Wood Block")



---
#### Field Percussion.LowWoodBlock

General MIDI percussion 77 ("Low Wood Block")



---
#### Field Percussion.MuteCuica

General MIDI percussion 78 ("Mute Cuica")



---
#### Field Percussion.OpenCuica

General MIDI percussion 79 ("Open Cuica")



---
#### Field Percussion.MuteTriangle

General MIDI percussion 80 ("Mute Triangle")



---
#### Field Percussion.OpenTriangle

General MIDI percussion 81 ("Open Triangle")



---
## Type InputDevice

A MIDI input device



> Each instance of this class describes a MIDI input device installed on the system. You cannot create your own instances, but instead must go through the [[|P:Midi.InputDevice.InstalledDevices]] property to find which devices are available. You may wish to examine the [[|P:Midi.DeviceBase.Name]] property of each one and present the user with a choice of which device(s) to use. 

 Open an input device with [[|M:Midi.InputDevice.Open]] and close it with [[|M:Midi.InputDevice.Close]]. While it is open, you may arrange to start receiving messages with [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]] and then stop receiving them with [[|M:Midi.InputDevice.StopReceiving]]. An input device can only receive messages when it is both open and started. 

 Incoming messages are routed to the corresponding events, such as [[|E:Midi.InputDevice.NoteOn]] and [[|E:Midi.InputDevice.ProgramChange]]. The event handlers are invoked on a background thread which is started in [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]] and stopped in [[|M:Midi.InputDevice.StopReceiving]]. 

 As each message is received, it is assigned a timestamp in one of two ways. If [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]] is called with a [[|T:Midi.Clock]], then each message is assigned a time by querying the clock's [[|P:Midi.Clock.Time]] property. If [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]] is called with null, then each message is assigned a time based on the number of seconds since [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]] was called. 





---
## Type InputDevice.ControlChangeHandler

Delegate called when an input device receives a Control Change message



---
## Type InputDevice.NoteOffHandler

Delegate called when an input device receives a Note Off message



---
## Type InputDevice.NoteOnHandler

Delegate called when an input device receives a Note On message



---
## Type InputDevice.PitchBendHandler

Delegate called when an input device receives a Pitch Bend message



---
## Type InputDevice.ProgramChangeHandler

Delegate called when an input device receives a Program Change message



---
#### Event InputDevice.ControlChange

Event called when an input device receives a Control Change message



---
#### Event InputDevice.NoteOff

Event called when an input device receives a Note Off message



---
#### Event InputDevice.NoteOn

Event called when an input device receives a Note On message



---
#### Event InputDevice.PitchBend

Event called when an input device receives a Pitch Bend message



---
#### Event InputDevice.ProgramChange

Event called when an input device receives a Program Change message



---
#### Method InputDevice.RemoveAllEventHandlers

Removes all event handlers from the input events on this device



---
#### Property InputDevice.InstalledDevices

List of input devices installed on this system



---
#### Property InputDevice.IsOpen

True if this device has been successfully opened



---
#### Property InputDevice.IsReceiving

True if this device is receiving messages



---
#### Method InputDevice.Close

Closes this input device

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]:  The device is not open or is still receiving. 

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The device cannot be closed.



---
#### Method InputDevice.Open

Opens this input device

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is already open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The device cannot be opened.



> Note that Open() establishes a connection to the device, but no messages will be received until [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]] is called. 



---
#### Method InputDevice.StartReceiving(Midi.Clock)

Starts this input device receiving messages

|Name | Description |
|-----|------|
|clock: | If non-null, the clock's [[|P:Midi.Clock.Time]] property will be used to assign a timestamp to each incoming message. If null, timestamps will be in seconds since StartReceiving() was called. |
[[T:System.InvalidOperationException|T:System.InvalidOperationException]]:  The device is not open or is already receiving. 

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The device cannot start receiving.



> This method launches a background thread to listen for input events, and as events are received, the event handlers are invoked on that background thread. Event handlers should be written to work from a background thread. (For example, if they want to update the GUI, they may need to BeginInvoke to arrange for GUI updates to happen on the correct thread.) 

 The background thread which is created by this method is joined (shut down) in [[|M:Midi.InputDevice.StopReceiving]]. 





---
#### Method InputDevice.StopReceiving

Stops this input device from receiving messages



> This method waits for all in-progress input event handlers to finish, and then joins (shuts down) the background thread that was created in [[|M:Midi.InputDevice.StartReceiving(Midi.Clock)]]. Thus, when this function returns you can be sure that no more event handlers will be invoked. 

 It is illegal to call this method from an input event handler (ie, from the background thread), and doing so throws an exception. If an event handler really needs to call this method, consider using BeginInvoke to schedule it on another thread. 



[[T:System.InvalidOperationException|T:System.InvalidOperationException]]:  The device is not open; is not receiving; or called from within an event handler (ie, from the background thread). 

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The device cannot start receiving.



---
#### Method InputDevice.#ctor(System.UIntPtr,Midi.Win32API.MIDIINCAPS)

 Private Constructor, only called by the getter for the InstalledDevices property. 

|Name | Description |
|-----|------|
|deviceId: | Position of this device in the list of all devices|
|caps: | Win32 Struct with device metadata |


---
#### Method InputDevice.CheckReturnCode(Midi.Win32API.MMRESULT)

 Makes sure rc is MidiWin32Wrapper.MMSYSERR_NOERROR. If not, throws an exception with an appropriate error message. 

|Name | Description |
|-----|------|
|rc: ||


---
#### Method InputDevice.MakeDeviceList

 Private method for constructing the array of MidiInputDevices by calling the Win32 api. 

**Returns**: 



---
#### Method InputDevice.CheckNotOpen

Throws a MidiDeviceException if this device is open



---
#### Method InputDevice.CheckNotReceiving

Throws a MidiDeviceException if this device is receiving



---
#### Method InputDevice.CheckOpen

Throws a MidiDeviceException if this device is not open



---
#### Method InputDevice.CheckReceiving

Throws a MidiDeviceException if this device is not receiving



---
#### Method InputDevice.InputCallback(Midi.Win32API.HMIDIIN,Midi.Win32API.MidiInMessage,System.UIntPtr,System.UIntPtr,System.UIntPtr)

The input callback for midiOutOpen



---
#### Field InputDevice.isInsideInputHandler

 Thread-local, set to true when called by an input handler, false in all other threads. 



---
## Type MessageQueue

A time-sorted queue of MIDI messages

 Messages can be added in any order, and can be popped off in timestamp order. 

---
#### Property MessageQueue.EarliestTimestamp

The timestamp of the earliest messsage(s) in the queue

 Throws an exception if the queue is empty. 

---
#### Property MessageQueue.IsEmpty

True if the queue is empty



---
#### Method MessageQueue.AddMessage(Midi.Message)

Adds a message to the queue

|Name | Description |
|-----|------|
|message: | The message to add to the queue|
 The message must have a valid timestamp (not MidiMessage.Now), but other than that there is no restriction on the timestamp. For example, it is legal to add a message with a timestamp earlier than some other message which was previously removed from the queue. Such a message would become the new "earliest" message, and so would be be the first message returned by PopEarliest(). 

---
#### Method MessageQueue.Clear

Discards all messages in the queue



---
#### Method MessageQueue.PopEarliest

 Removes and returns the message(s) in the queue that have the earliest timestamp. 



---
## Type OutputDevice

A MIDI output device



> Each instance of this class describes a MIDI output device installed on the system. You cannot create your own instances, but instead must go through the [[|P:fractions.OutputDevice.InstalledDevices]] property to find which devices are available. You may wish to examine the [[|P:Midi.DeviceBase.Name]] property of each one and present the user with a choice of which device to use. 

 Open an output device with [[|M:fractions.OutputDevice.Open]] and close it with [[|M:fractions.OutputDevice.Close]]. While it is open, you may send MIDI messages with functions such as [[|M:fractions.OutputDevice.SendNoteOn(Midi.Channel,Midi.Pitch,System.Int32)]], [[|M:fractions.OutputDevice.SendNoteOff(Midi.Channel,Midi.Pitch,System.Int32)]] and [[|M:fractions.OutputDevice.SendProgramChange(Midi.Channel,Midi.Instrument)]]. All notes may be silenced on the device by calling [[|M:fractions.OutputDevice.SilenceAllNotes]]. 

 Note that the above methods send their messages immediately. If you wish to arrange for a message to be sent at a specific future time, you'll need to instantiate some subclass of [[|T:Midi.Message]] (eg [[|T:Midi.NoteOnMessage]]) and then pass it to [[ Clock.Schedule |M:Midi.Clock.Schedule(Midi.Message)]]. 





---
#### Property OutputDevice.InstalledDevices

List of devices installed on this system



---
#### Property OutputDevice.IsOpen

True if this device is open



---
#### Method OutputDevice.Close

Closes this output device

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The device cannot be closed.



---
#### Method OutputDevice.Open

Opens this output device

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is already open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The device cannot be opened.



---
#### Method OutputDevice.SendControlChange(Midi.Channel,Midi.Control,System.Int32)

Sends a Control Change message to this MIDI output device

|Name | Description |
|-----|------|
|channel: | The channel|
|control: | The control|
|value: | The new value 0..127|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]:  channel, control, or value is out-of-range. 

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



---
#### Method OutputDevice.SendNoteOff(Midi.Channel,Midi.Pitch,System.Int32)

Sends a Note Off message to this MIDI output device

|Name | Description |
|-----|------|
|channel: | The channel|
|pitch: | The pitch|
|velocity: | The velocity 0..127|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]:  channel, note, or velocity is out-of-range. 

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



---
#### Method OutputDevice.SendNoteOn(Midi.Channel,Midi.Pitch,System.Int32)

Sends a Note On message to this MIDI output device

|Name | Description |
|-----|------|
|channel: | The channel|
|pitch: | The pitch|
|velocity: | The velocity 0..127|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]:  channel, pitch, or velocity is out-of-range. 

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



---
#### Method OutputDevice.SendPercussion(Midi.Percussion,System.Int32)

Sends a Note On message to Channel10 of this MIDI output device

|Name | Description |
|-----|------|
|percussion: | The percussion|
|velocity: | The velocity 0..127|


> This is simply shorthand for a Note On message on Channel10 with a percussion-specific note, so there is no corresponding message to receive from an input device. 

[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: percussion or velocity is out-of-range.

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



---
#### Method OutputDevice.SendPitchBend(Midi.Channel,System.Int32)

Sends a Pitch Bend message to this MIDI output device

|Name | Description |
|-----|------|
|channel: | The channel|
|value: | The pitch bend value, 0..16383, 8192 is centered|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: channel or value is out-of-range.

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



---
#### Method OutputDevice.SendProgramChange(Midi.Channel,Midi.Instrument)

Sends a Program Change message to this MIDI output device

|Name | Description |
|-----|------|
|channel: | The channel|
|instrument: | The instrument|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: channel or instrument is out-of-range.

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



> A Program Change message is used to switch among instrument settings, generally instrument voices. An instrument conforming to General Midi 1 will have the instruments described in the [[|T:Midi.Instrument]] enum; other instruments may have different instrument sets. 



---
#### Method OutputDevice.SilenceAllNotes

Silences all notes on this output device

[[T:System.InvalidOperationException|T:System.InvalidOperationException]]: The device is not open.

[[T:Midi.DeviceException|T:Midi.DeviceException]]: The message cannot be sent.



---
#### Method OutputDevice.#ctor(System.UIntPtr,Midi.Win32API.MIDIOUTCAPS)

 Private Constructor, only called by the getter for the InstalledDevices property. 

|Name | Description |
|-----|------|
|deviceId: | Position of this device in the list of all devices|
|caps: |Win32 Struct with device metadata|


---
#### Method OutputDevice.CheckReturnCode(Midi.Win32API.MMRESULT)

 Makes sure rc is MidiWin32Wrapper.MMSYSERR_NOERROR. If not, throws an exception with an appropriate error message. 

|Name | Description |
|-----|------|
|rc: ||


---
#### Method OutputDevice.MakeDeviceList

 Private method for constructing the array of MidiOutputDevices by calling the Win32 api. 

**Returns**: 



---
#### Method OutputDevice.CheckNotOpen

Throws a MidiDeviceException if this device is open



---
#### Method OutputDevice.CheckOpen

Throws a MidiDeviceException if this device is not open



---
## Type PercussionExtensions

Extension methods for the Percussion enum

 Be sure to "using midi" if you want to use these as extension methods. 

---
#### Method PercussionExtensions.IsValid(Midi.Percussion)

Returns true if the specified percussion is valid

|Name | Description |
|-----|------|
|percussion: | The percussion to test|


---
#### Method PercussionExtensions.Name(Midi.Percussion)

Returns the human-readable name of a MIDI percussion

|Name | Description |
|-----|------|
|percussion: | The percussion|


---
#### Method PercussionExtensions.Validate(Midi.Percussion)

Throws an exception if percussion is not valid

|Name | Description |
|-----|------|
|percussion: | The percussion to validate|
[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: The percussion is out-of-range.



---
## Type PercussionMessage

Percussion message



> A percussion message is simply shorthand for sending a Note On message to Channel10 with a percussion-specific note. This message can be sent to an OutputDevice but will be received by an InputDevice as a NoteOn message. 



---
#### Method PercussionMessage.#ctor(Midi.DeviceBase,Midi.Percussion,System.Int32,System.Single)

Constructs a Percussion message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|percussion: | Percussion|
|velocity: | Velocity, 0..127|
|time: | The timestamp for this message|


---
#### Property PercussionMessage.Percussion

Percussion



---
#### Property PercussionMessage.Velocity

Velocity, 0..127



---
#### Method PercussionMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method PercussionMessage.SendNow

Sends this message immediately



---
## Type Pitch

Pitches supported by MIDI



> MIDI defines 127 distinct pitches, in semitone intervals, ranging from C five octaves below middle C, up to G five octaves above middle C. This covers several octaves above and below the range of a normal 88-key piano. 

 These 127 pitches are the only ones directly expressible in MIDI. Precise variations in frequency can be achieved with [[ Pitch Bend |M:fractions.OutputDevice.SendPitchBend(Midi.Channel,System.Int32)]] messages, though Pitch Bend messages apply to the whole channel at once. 

 In this enum, pitches are given C Major note names (eg "F", "GSharp") followed by the octave number. Octaves use standard piano terminology: Middle C is in octave 4. (Note that this is different from "MIDI octaves", which have Middle C in octave 0.) 

 This enum has extension methods, such as [[|M:Midi.PitchExtensions.NotePreferringSharps(Midi.Pitch)]] and [[|M:Midi.PitchExtensions.IsInMidiRange(Midi.Pitch)]], defined in [[|T:Midi.PitchExtensions]]. 





---
#### Field Pitch.CNeg1

C in octave -1



---
#### Field Pitch.CSharpNeg1

C# in octave -1



---
#### Field Pitch.DNeg1

D in octave -1



---
#### Field Pitch.DSharpNeg1

D# in octave -1



---
#### Field Pitch.ENeg1

E in octave -1



---
#### Field Pitch.FNeg1

F in octave -1



---
#### Field Pitch.FSharpNeg1

F# in octave -1



---
#### Field Pitch.GNeg1

G in octave -1



---
#### Field Pitch.GSharpNeg1

G# in octave -1



---
#### Field Pitch.ANeg1

A in octave -1



---
#### Field Pitch.ASharpNeg1

A# in octave -1



---
#### Field Pitch.BNeg1

B in octave -1



---
#### Field Pitch.C0

C in octave 0



---
#### Field Pitch.CSharp0

C# in octave 0



---
#### Field Pitch.D0

D in octave 0



---
#### Field Pitch.DSharp0

D# in octave 0



---
#### Field Pitch.E0

E in octave 0



---
#### Field Pitch.F0

F in octave 0



---
#### Field Pitch.FSharp0

F# in octave 0



---
#### Field Pitch.G0

G in octave 0



---
#### Field Pitch.GSharp0

G# in octave 0



---
#### Field Pitch.A0

A in octave 0



---
#### Field Pitch.ASharp0

A# in octave 0, usually the lowest key on an 88-key keyboard



---
#### Field Pitch.B0

B in octave 0



---
#### Field Pitch.C1

C in octave 1



---
#### Field Pitch.CSharp1

C# in octave 1



---
#### Field Pitch.D1

D in octave 1



---
#### Field Pitch.DSharp1

D# in octave 1



---
#### Field Pitch.E1

E in octave 1



---
#### Field Pitch.F1

F in octave 1



---
#### Field Pitch.FSharp1

F# in octave 1



---
#### Field Pitch.G1

G in octave 1



---
#### Field Pitch.GSharp1

G# in octave 1



---
#### Field Pitch.A1

A in octave 1



---
#### Field Pitch.ASharp1

A# in octave 1



---
#### Field Pitch.B1

B in octave 1



---
#### Field Pitch.C2

C in octave 2



---
#### Field Pitch.CSharp2

C# in octave 2



---
#### Field Pitch.D2

D in octave 2



---
#### Field Pitch.DSharp2

D# in octave 2



---
#### Field Pitch.E2

E in octave 2



---
#### Field Pitch.F2

F in octave 2



---
#### Field Pitch.FSharp2

F# in octave 2



---
#### Field Pitch.G2

G in octave 2



---
#### Field Pitch.GSharp2

G# in octave 2



---
#### Field Pitch.A2

A in octave 2



---
#### Field Pitch.ASharp2

A# in octave 2



---
#### Field Pitch.B2

B in octave 2



---
#### Field Pitch.C3

C in octave 3



---
#### Field Pitch.CSharp3

C# in octave 3



---
#### Field Pitch.D3

D in octave 3



---
#### Field Pitch.DSharp3

D# in octave 3



---
#### Field Pitch.E3

E in octave 3



---
#### Field Pitch.F3

F in octave 3



---
#### Field Pitch.FSharp3

F# in octave 3



---
#### Field Pitch.G3

G in octave 3



---
#### Field Pitch.GSharp3

G# in octave 3



---
#### Field Pitch.A3

A in octave 3



---
#### Field Pitch.ASharp3

A# in octave 3



---
#### Field Pitch.B3

B in octave 3



---
#### Field Pitch.C4

C in octave 4, also known as Middle C



---
#### Field Pitch.CSharp4

C# in octave 4



---
#### Field Pitch.D4

D in octave 4



---
#### Field Pitch.DSharp4

D# in octave 4



---
#### Field Pitch.E4

E in octave 4



---
#### Field Pitch.F4

F in octave 4



---
#### Field Pitch.FSharp4

F# in octave 4



---
#### Field Pitch.G4

G in octave 4



---
#### Field Pitch.GSharp4

G# in octave 4



---
#### Field Pitch.A4

A in octave 4



---
#### Field Pitch.ASharp4

A# in octave 4



---
#### Field Pitch.B4

B in octave 4



---
#### Field Pitch.C5

C in octave 5



---
#### Field Pitch.CSharp5

C# in octave 5



---
#### Field Pitch.D5

D in octave 5



---
#### Field Pitch.DSharp5

D# in octave 5



---
#### Field Pitch.E5

E in octave 5



---
#### Field Pitch.F5

F in octave 5



---
#### Field Pitch.FSharp5

F# in octave 5



---
#### Field Pitch.G5

G in octave 5



---
#### Field Pitch.GSharp5

G# in octave 5



---
#### Field Pitch.A5

A in octave 5



---
#### Field Pitch.ASharp5

A# in octave 5



---
#### Field Pitch.B5

B in octave 5



---
#### Field Pitch.C6

C in octave 6



---
#### Field Pitch.CSharp6

C# in octave 6



---
#### Field Pitch.D6

D in octave 6



---
#### Field Pitch.DSharp6

D# in octave 6



---
#### Field Pitch.E6

E in octave 6



---
#### Field Pitch.F6

F in octave 6



---
#### Field Pitch.FSharp6

F# in octave 6



---
#### Field Pitch.G6

G in octave 6



---
#### Field Pitch.GSharp6

G# in octave 6



---
#### Field Pitch.A6

A in octave 6



---
#### Field Pitch.ASharp6

A# in octave 6



---
#### Field Pitch.B6

B in octave 6



---
#### Field Pitch.C7

C in octave 7



---
#### Field Pitch.CSharp7

C# in octave 7



---
#### Field Pitch.D7

D in octave 7



---
#### Field Pitch.DSharp7

D# in octave 7



---
#### Field Pitch.E7

E in octave 7



---
#### Field Pitch.F7

F in octave 7



---
#### Field Pitch.FSharp7

F# in octave 7



---
#### Field Pitch.G7

G in octave 7



---
#### Field Pitch.GSharp7

G# in octave 7



---
#### Field Pitch.A7

A in octave 7



---
#### Field Pitch.ASharp7

A# in octave 7



---
#### Field Pitch.B7

B in octave 7



---
#### Field Pitch.C8

C in octave 8, usually the highest key on an 88-key keyboard



---
#### Field Pitch.CSharp8

C# in octave 8



---
#### Field Pitch.D8

D in octave 8



---
#### Field Pitch.DSharp8

D# in octave 8



---
#### Field Pitch.E8

E in octave 8



---
#### Field Pitch.F8

F in octave 8



---
#### Field Pitch.FSharp8

F# in octave 8



---
#### Field Pitch.G8

G in octave 8



---
#### Field Pitch.GSharp8

G# in octave 8



---
#### Field Pitch.A8

A in octave 8



---
#### Field Pitch.ASharp8

A# in octave 8



---
#### Field Pitch.B8

B in octave 8



---
#### Field Pitch.C9

C in octave 9



---
#### Field Pitch.CSharp9

C# in octave 9



---
#### Field Pitch.D9

D in octave 9



---
#### Field Pitch.DSharp9

D# in octave 9



---
#### Field Pitch.E9

E in octave 9



---
#### Field Pitch.F9

F in octave 9



---
#### Field Pitch.FSharp9

F# in octave 9



---
#### Field Pitch.G9

G in octave 9



---
## Type PitchBendMessage

Pitch Bend message



---
#### Method PitchBendMessage.#ctor(Midi.DeviceBase,Midi.Channel,System.Int32,System.Single)

Constructs a Pitch Bend message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|channel: | Channel, 0..15, 10 reserved for percussion|
|value: | Pitch bend value, 0..16383, 8192 is centered|
|time: | The timestamp for this message|


---
#### Property PitchBendMessage.Value

Pitch bend value, 0..16383, 8192 is centered



---
#### Method PitchBendMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method PitchBendMessage.SendNow

Sends this message immediately



---
## Type PitchExtensions

Extension methods for the Pitch enum



---
#### Field PitchExtensions.PositionInOctaveToNotesPreferringFlats

Maps PositionInOctave() to a Note preferring flats



---
#### Field PitchExtensions.PositionInOctaveToNotesPreferringSharps

Maps PositionInOctave() to a Note preferring sharps



---
#### Method PitchExtensions.IsInMidiRange(Midi.Pitch)

Returns true if pitch is in the MIDI range [1..127]

|Name | Description |
|-----|------|
|pitch: | The pitch to test|
**Returns**:  True if the pitch is in [0..127]. 



---
#### Method PitchExtensions.NotePreferringFlats(Midi.Pitch)

 Returns the simplest note that resolves to this pitch, preferring flats where needed. 

|Name | Description |
|-----|------|
|pitch: | The pitch|
**Returns**:  The simplest note for that pitch. If that pitch is a "white key", the note is simply a letter with no accidentals (and is the same as [[|M:Midi.PitchExtensions.NotePreferringSharps(Midi.Pitch)]]). Otherwise the note has a flat. 



---
#### Method PitchExtensions.NotePreferringSharps(Midi.Pitch)

 Returns the simplest note that resolves to this pitch, preferring sharps where needed. 

|Name | Description |
|-----|------|
|pitch: | The pitch|
**Returns**:  The simplest note for that pitch. If that pitch is a "white key", the note is simply a letter with no accidentals (and is the same as [[|M:Midi.PitchExtensions.NotePreferringFlats(Midi.Pitch)]]). Otherwise the note has a sharp. 



---
#### Method PitchExtensions.NoteWithLetter(Midi.Pitch,System.Char)

Returns the note that would name this pitch if it used the given letter

|Name | Description |
|-----|------|
|pitch: | The pitch being named|
|letter: | The letter to use in the name, in ['A'..'G']|
**Returns**:  The note for pitch with letter. The result may have a large number of accidentals if pitch is not easily named by letter. 

[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: letter is out of range.



---
#### Method PitchExtensions.Octave(Midi.Pitch)

Returns the octave containing this pitch

|Name | Description |
|-----|------|
|pitch: | The pitch|
**Returns**:  The octave, where octaves begin at each C, and Middle C is the first pitch in octave 4. 



---
#### Method PitchExtensions.PositionInOctave(Midi.Pitch)

Returns the position of this pitch in its octave

|Name | Description |
|-----|------|
|pitch: | The pitch|
**Returns**:  The pitch's position in its octave, where octaves start at each C, so C's position is 0, C#'s position is 1, etc. 



---
## Type ProgramChangeMessage

Program Change message



---
#### Method ProgramChangeMessage.#ctor(Midi.DeviceBase,Midi.Channel,Midi.Instrument,System.Single)

Constructs a Program Change message

|Name | Description |
|-----|------|
|device: | The device associated with this message|
|channel: | Channel|
|instrument: | Instrument|
|time: | The timestamp for this message|


---
#### Property ProgramChangeMessage.Instrument

Instrument



---
#### Method ProgramChangeMessage.MakeTimeShiftedCopy(System.Single)

Returns a copy of this message, shifted in time by the specified amount



---
#### Method ProgramChangeMessage.SendNow

Sends this message immediately



---
## Type ScalePattern

Description of a scale's pattern as it ascends through an octave



> This class describes the general behavior of a scale as it ascends from a tonic up to the next tonic. It is described in terms of semitones relative to the tonic; to apply it to a particular tonic, pass one of these to the constructor of [[|T:Midi.Scale]]. 



---
#### Property ScalePattern.Ascent

The ascent of the scale



> The ascent is expressed as a series of integers, each giving a semitone distance above the tonic. It must have at least two elements, start at zero (the tonic) , be monotonically increasing, and stay below 12 (the next tonic above). 

 The number of elements in the ascent tells us how many notes-per-octave in the scale. For example, a heptatonic scale will always have seven elements in the ascent. 





---
#### Property ScalePattern.Name

The name of the scale being described



---
#### Method ScalePattern.#ctor(System.String,System.Int32[])

Constructs a scale pattern

|Name | Description |
|-----|------|
|name: | The name of the scale pattern|
|ascent: | The ascending pattern of the scale. See the [[|P:Midi.ScalePattern.Ascent]] property for a detailed description and requirements. This parameter is copied. |
[[T:System.ArgumentNullException|T:System.ArgumentNullException]]: name or ascent is null.

[[T:System.ArgumentException|T:System.ArgumentException]]: ascent is invalid.



---
#### Method ScalePattern.op_Inequality(Midi.ScalePattern,Midi.ScalePattern)

Inequality operator does value inequality



---
#### Method ScalePattern.op_Equality(Midi.ScalePattern,Midi.ScalePattern)

Equality operator does value equality



---
#### Method ScalePattern.Equals(System.Object)

Value equality



---
#### Method ScalePattern.GetHashCode

Hash code



---
#### Method ScalePattern.ToString

ToString returns the pattern name

**Returns**:  The pattern's name, such as "Major" or "Melodic Minor (ascending)". 



---
#### Method ScalePattern.AscentIsValid(System.Int32[])

Returns true if ascent is valid



---
## Type ShortMsg

Utility functions for encoding and decoding short messages



---
#### Method ShortMsg.DecodeControlChange(System.UIntPtr,System.UIntPtr,Midi.Channel@,Midi.Control@,System.Int32@,System.UInt32@)

Decodes a Control Change short message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|
|channel: | Filled in with the channel|
|control: | Filled in with the control|
|value: | Filled in with the value, 0-127|
|timestamp: | Filled in with the timestamp in microseconds since midiInStart()|


---
#### Method ShortMsg.DecodeNoteOff(System.UIntPtr,System.UIntPtr,Midi.Channel@,Midi.Pitch@,System.Int32@,System.UInt32@)

Decodes a Note Off short message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|
|channel: | Filled in with the channel|
|pitch: | Filled in with the pitch|
|velocity: | Filled in with the velocity, 0.127 |
|timestamp: | Filled in with the timestamp in microseconds since midiInStart()|


---
#### Method ShortMsg.DecodeNoteOn(System.UIntPtr,System.UIntPtr,Midi.Channel@,Midi.Pitch@,System.Int32@,System.UInt32@)

Decodes a Note On short message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|
|channel: | Filled in with the channel|
|pitch: | Filled in with the pitch|
|velocity: | Filled in with the velocity, 0.127 |
|timestamp: | Filled in with the timestamp in microseconds since midiInStart()|


---
#### Method ShortMsg.DecodePitchBend(System.UIntPtr,System.UIntPtr,Midi.Channel@,System.Int32@,System.UInt32@)

Decodes a Pitch Bend message based on MidiInProc params

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|
|channel: | Filled in with the channel, 0-15|
|value: | Filled in with the pitch bend value, 0..16383, 8192 is centered|
|timestamp: | Filled in with the timestamp in microseconds since midiInStart()|


---
#### Method ShortMsg.DecodeProgramChange(System.UIntPtr,System.UIntPtr,Midi.Channel@,Midi.Instrument@,System.UInt32@)

Decodes a Program Change short message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|
|channel: | Filled in with the channel, 0-15|
|instrument: | Filled in with the instrument, 0-127 |
|timestamp: | Filled in with the timestamp in microseconds since midiInStart()|


---
#### Method ShortMsg.EncodeControlChange(Midi.Channel,Midi.Control,System.Int32)

Encodes a Control Change short message

|Name | Description |
|-----|------|
|channel: | The channel|
|control: | The control|
|value: | The new value 0..127|
**Returns**:  A value that can be passed to midiOutShortMsg. 



---
#### Method ShortMsg.EncodeNoteOff(Midi.Channel,Midi.Pitch,System.Int32)

Encodes a Note Off short message

|Name | Description |
|-----|------|
|channel: | The channel|
|pitch: | The pitch|
|velocity: | The velocity 0..127|
**Returns**:  A value that can be passed to midiOutShortMsg. 



---
#### Method ShortMsg.EncodeNoteOn(Midi.Channel,Midi.Pitch,System.Int32)

Encodes a Note On short message

|Name | Description |
|-----|------|
|channel: | The channel|
|pitch: | The pitch|
|velocity: | The velocity 0..127|
**Returns**:  A value that can be passed to midiOutShortMsg. 

[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: pitch is not in MIDI range.



---
#### Method ShortMsg.EncodePitchBend(Midi.Channel,System.Int32)

Encodes a Pitch Bend short message

|Name | Description |
|-----|------|
|channel: | The channel|
|value: | The pitch bend value, 0..16383, 8192 is centered|
**Returns**:  A value that can be passed to midiOutShortMsg. 



---
#### Method ShortMsg.EncodeProgramChange(Midi.Channel,Midi.Instrument)

Encodes a Program Change short message

|Name | Description |
|-----|------|
|channel: | The channel|
|instrument: | The instrument|
**Returns**:  A value that can be passed to midiOutShortMsg. 



---
#### Method ShortMsg.IsControlChange(System.UIntPtr,System.UIntPtr)

Returns true if the given short message describes a Control Change message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|


---
#### Method ShortMsg.IsNoteOff(System.UIntPtr,System.UIntPtr)

Returns true if the given short message describes a Note Off message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|


---
#### Method ShortMsg.IsNoteOn(System.UIntPtr,System.UIntPtr)

Returns true if the given short message describes a Note On message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|


---
#### Method ShortMsg.IsPitchBend(System.UIntPtr,System.UIntPtr)

Returns true if the given MidiInProc params describe a Pitch Bend message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|


---
#### Method ShortMsg.IsProgramChange(System.UIntPtr,System.UIntPtr)

Returns true if the given short message a Program Change message

|Name | Description |
|-----|------|
|dwParam1: | The dwParam1 arg passed to MidiInProc|
|dwParam2: | The dwParam2 arg passed to MidiInProc|


---
## Type Win32API

C# wrappers for the Win32 MIDI API

 Because .NET does not provide MIDI support itself, in C# we must use P/Invoke to wrap the Win32 API. That API consists of the MMSystem.h C header and the winmm.dll library. The API is described in detail here: http://msdn.microsoft.com/en-us/library/ms712733(VS.85).aspx. The P/Invoke interop mechanism is described here: http: //msdn.microsoft.com/en-us/library/aa288468(VS.71).aspx. This file covers the subset of the MIDI protocol needed to manage input and output devices and send and receive Note On/Off, Control Change, Pitch Bend and Program Change messages. Other portions of the MIDI protocol (such as sysex events) are supported in the Win32 API but are not wrapped here. Some of the C functions are not typesafe when wrapped, so those wrappers are made private and typesafe variants are provided. 

---
#### Field Win32API.MAXPNAMELEN

Max length of a manufacturer name in the Win32 API



---
## Type Win32API.MidiDeviceType

Values for wTechnology field of MIDIOUTCAPS structure



---
## Type Win32API.MidiExtraFeatures

Flags for dwSupport field of MIDIOUTCAPS structure



---
## Type Win32API.MidiInMessage

"Midi In Messages", passed to wMsg param of MidiInProc



---
## Type Win32API.MidiOpenFlags

Flags passed to midiInOpen() and midiOutOpen()



---
## Type Win32API.MidiOutMessage

"Midi Out Messages", passed to wMsg param of MidiOutProc



---
## Type Win32API.MMRESULT

Status type returned from most functions in the Win32 API



---
## Type Win32API.HMIDIIN

Win32 handle for a MIDI input device



---
## Type Win32API.HMIDIOUT

Win32 handle for a MIDI output device



---
## Type Win32API.MIDIINCAPS

Struct representing the capabilities of an input device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711596(VS.85).aspx 

---
## Type Win32API.MIDIOUTCAPS

Struct representing the capabilities of an output device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711619(VS.85).aspx 

---
## Type Win32API.MidiOutProc

 Callback invoked when a MIDI output device is opened, closed, or finished with a buffer. 

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711637(VS.85).aspx 

---
#### Method Win32API.midiOutClose(Midi.Win32API.HMIDIOUT)

Closes a MIDI output device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711620(VS.85).aspx 

---
#### Method Win32API.midiOutGetDevCaps(System.UIntPtr,Midi.Win32API.MIDIOUTCAPS@)

Fills in the capabilities struct for a specific output device

 NOTE: This is adapted from the original Win32 function in order to make it typesafe. Win32 docs: http://msdn.microsoft.com/en-us/library/ms711621(VS.85).aspx 

---
#### Method Win32API.midiOutGetErrorText(Midi.Win32API.MMRESULT,System.Text.StringBuilder)

Gets the error text for a return code related to an output device

 NOTE: This is adapted from the original Win32 function in order to make it typesafe. Win32 docs: http://msdn.microsoft.com/en-us/library/ms711622(VS.85).aspx 

---
#### Method Win32API.midiOutGetNumDevs

Returns the number of MIDI output devices on this system

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711627(VS.85).aspx 

---
#### Method Win32API.midiOutOpen(Midi.Win32API.HMIDIOUT@,System.UIntPtr,Midi.Win32API.MidiOutProc,System.UIntPtr)

Opens a MIDI output device

 NOTE: This is adapted from the original Win32 function in order to make it typesafe. Win32 docs: http://msdn.microsoft.com/en-us/library/ms711632(VS.85).aspx 

---
#### Method Win32API.midiOutReset(Midi.Win32API.HMIDIOUT)

Turns off all notes and sustains on a MIDI output device

 Win32 docs: http://msdn.microsoft.com/en-us/library/dd798479(VS.85).aspx 

---
#### Method Win32API.midiOutShortMsg(Midi.Win32API.HMIDIOUT,System.UInt32)

Sends a short MIDI message (anything but sysex or stream)

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711640(VS.85).aspx 

---
## Type Win32API.MidiInProc

Callback invoked when a MIDI event is received from an input device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711612(VS.85).aspx 

---
#### Method Win32API.midiInClose(Midi.Win32API.HMIDIIN)

Closes a MIDI input device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711602(VS.85).aspx 

---
#### Method Win32API.midiInGetDevCaps(System.UIntPtr,Midi.Win32API.MIDIINCAPS@)

Fills in the capabilities struct for a specific input device

 NOTE: This is adapted from the original Win32 function in order to make it typesafe. Win32 docs: http://msdn.microsoft.com/en-us/library/ms711604(VS.85).aspx 

---
#### Method Win32API.midiInGetErrorText(Midi.Win32API.MMRESULT,System.Text.StringBuilder)

Gets the error text for a return code related to an input device

 NOTE: This is adapted from the original Win32 function in order to make it typesafe. Win32 docs: http://msdn.microsoft.com/en-us/library/ms711605(VS.85).aspx 

---
#### Method Win32API.midiInGetNumDevs

Returns the number of MIDI input devices on this system

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711608(VS.85).aspx 

---
#### Method Win32API.midiInOpen(Midi.Win32API.HMIDIIN@,System.UIntPtr,Midi.Win32API.MidiInProc,System.UIntPtr)

Opens a MIDI input device

 NOTE: This is adapted from the original Win32 function in order to make it typesafe. Win32 docs: http://msdn.microsoft.com/en-us/library/ms711610(VS.85).aspx 

---
#### Method Win32API.midiInReset(Midi.Win32API.HMIDIIN)

Resets input on a MIDI input device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711613(VS.85).aspx 

---
#### Method Win32API.midiInStart(Midi.Win32API.HMIDIIN)

Starts input on a MIDI input device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711614(VS.85).aspx 

---
#### Method Win32API.midiInStop(Midi.Win32API.HMIDIIN)

Stops input on a MIDI input device

 Win32 docs: http://msdn.microsoft.com/en-us/library/ms711615(VS.85).aspx 

---
## Type Scale

A scale based on a pattern and a tonic note



> For our purposes, a scale is defined by a tonic and then the pattern that it uses to ascend up to the next tonic. The tonic is described with a [[|T:Midi.Note]] because it is not specific to any one octave. The ascending pattern is provided by the [[|T:Midi.ScalePattern]] class. 

 This class comes with a collection of predefined patterns, such as [[|F:Midi.Scale.Major]] and [[|F:Midi.Scale.HarmonicMinor]]. 





---
#### Property Scale.Name

 The scale's human-readable name, such as "G# Major" or "Eb Melodic Minor (ascending)". 



---
#### Property Scale.NoteSequence

The sequence of notes in this scale



> This sequence begins at the tonic and ascends, stopping before the next tonic. 



---
#### Property Scale.Pattern

The pattern of this scale



---
#### Property Scale.Tonic

The tonic of this scale



---
#### Method Scale.#ctor(Midi.Note,Midi.ScalePattern)

Constructs a scale from its tonic and its pattern

|Name | Description |
|-----|------|
|tonic: | The tonic note|
|pattern: | The scale pattern|
[[T:System.ArgumentNullException|T:System.ArgumentNullException]]: tonic or pattern is null.



---
#### Method Scale.Contains(Midi.Pitch)

Returns true if pitch is in this scale

|Name | Description |
|-----|------|
|pitch: | The pitch to test|
**Returns**:  True if pitch is in this scale. 



---
#### Method Scale.ScaleDegree(Midi.Pitch)

Returns the scale degree of the given pitch in this scale

|Name | Description |
|-----|------|
|pitch: | The pitch to test|
**Returns**:  The scale degree of pitch in this scale, where 1 is the tonic. Returns -1 if pitch is not in this scale. 



---
#### Field Scale.Chromatic

Pattern for Chromatic scales



---
#### Field Scale.HarmonicMinor

Pattern for Harmonic Minor scales



---
#### Field Scale.Major

Pattern for Major scales



---
#### Field Scale.MelodicMinorAscending

Pattern for Melodic Minor scale as it ascends



---
#### Field Scale.MelodicMinorDescending

Pattern for Melodic Minor scale as it descends



---
#### Field Scale.NaturalMinor

Pattern for Natural Minor scales



---
#### Field Scale.Patterns

Array of all the built-in scale patterns



---
#### Method Scale.op_Inequality(Midi.Scale,Midi.Scale)

Inequality operator does value inequality because Chord is immutable



---
#### Method Scale.op_Equality(Midi.Scale,Midi.Scale)

Equality operator does value equality because Scale is immutable



---
#### Method Scale.Equals(System.Object)

Value equality



---
#### Method Scale.GetHashCode

Hash code



---
#### Method Scale.ToString

ToString returns the scale's human-readable name

**Returns**:  The scale's name, such as "G# Major" or "Eb Melodic Minor (ascending)". 



---
#### Method Scale.Build(Midi.Note,Midi.ScalePattern,System.Int32[],Midi.Note[],System.Int32@)

Builds a scale

|Name | Description |
|-----|------|
|tonic: | The tonic|
|pattern: | The scale pattern|
|positionInOctaveToSequenceIndex: | Must have 12 elements, and is filled with the 0-indexed scale position (or -1) for each position in the octave. |
|noteSequence: | Must have pattern.Ascent.Length elements, and is filled with the notes for each scale degree. |
|numAccidentals: | Filled with the total number of accidentals in the built scale. |


---


