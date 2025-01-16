// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;

namespace fractions.examples
{
    /// <summary>Simple arpeggiator</summary>
    /// <remarks>
    /// This example demonstrates input, output and Clock-based scheduling. As Note On and Note Off
    /// events are received from the input device, the Arpeggiator class schedules arpeggiated
    /// chords or scales based on the note played.
    /// </remarks>
    public class ArpeggiatorExample : ExampleBase
    {
        #region Constructors

        /// <summary>
        /// Constructs the example
        /// </summary>
        public ArpeggiatorExample() : base(nameof(Arpeggiator)) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Runs the example
        /// </summary>
        public override void Run()
        {
            // Create a clock running at the specified beats per minute.
            int beatsPerMinute = 180;
            var clock = new Clock(beatsPerMinute);

            // Prompt user to choose an output device (or if there is only one, use that one.
            var outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            outputDevice.Open();

            // Prompt user to choose an input device (or if there is only one, use that one).
            var inputDevice = ExampleUtil.ChooseInputDeviceFromConsole();
            inputDevice?.Open();

            var arpeggiator = new Arpeggiator(inputDevice, outputDevice, clock);
            var drummer = new Drummer(clock, outputDevice, 4);

            clock.Start();
            inputDevice?.StartReceiving(clock);

            bool done = false;
            while (!done)
            {
                Console.Clear();
                Console.WriteLine("BPM = {0}, Playing = {1}, Arpeggiator Mode = {2}", clock.BeatsPerMinute, clock.IsRunning, arpeggiator.Status);
                Console.WriteLine("Escape : Quit");
                Console.WriteLine("Down : Slower");
                Console.WriteLine("Up: Faster");
                Console.WriteLine("Left: Previous Chord or Scale");
                Console.WriteLine("Right: Next Chord or Scale");
                Console.WriteLine("Space = Toggle Play");
                Console.WriteLine("Enter = Toggle Scales/Chords");
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    done = true;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    clock.BeatsPerMinute -= 2;
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    clock.BeatsPerMinute += 2;
                }
                else if (key == ConsoleKey.RightArrow)
                {
                    arpeggiator.Change(1);
                }
                else if (key == ConsoleKey.LeftArrow)
                {
                    arpeggiator.Change(-1);
                }
                else if (key == ConsoleKey.Spacebar)
                {
                    if (clock.IsRunning)
                    {
                        clock.Stop();
                        inputDevice?.StopReceiving();
                        outputDevice.SilenceAllNotes();
                    }
                    else
                    {
                        clock.Start();
                        inputDevice?.StartReceiving(clock);
                    }
                }
                else if (key == ConsoleKey.Enter)
                {
                    arpeggiator.ToggleMode();
                }
                else if (ExampleUtil.IsMockPitch(key, out Pitch pitch))
                {
                    // We've hit a QUERTY key which is meant to simulate a MIDI note, so send the
                    // Note On to the output device and tell the arpeggiator.
                    var noteOn = new NoteOnMessage(outputDevice, 0, pitch, 100, clock.Time);
                    clock.Schedule(noteOn);
                    arpeggiator.NoteOn(noteOn);
                    // We don't get key release events for the console, so schedule a simulated Note
                    // Off one beat from now.
                    var noteOff = new NoteOffMessage(outputDevice, 0, pitch, 100, clock.Time + 1);
                    clock.Schedule(new CallbackMessage(beatTime => arpeggiator.NoteOff(noteOff), noteOff.Time));
                }
            }

            if (clock.IsRunning)
            {
                clock.Stop();
                inputDevice?.StopReceiving();
                outputDevice.SilenceAllNotes();
            }

            outputDevice.Close();
            if (inputDevice != null)
            {
                inputDevice.Close();
                inputDevice.RemoveAllEventHandlers();
            }

            // All done.
        }

        #endregion Methods
    }
}