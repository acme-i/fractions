﻿// Copyright (c) 2009, Tom Lokovic
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
    /// <summary>
    /// Example
    /// </summary>
    public class PrintNotesAndChords : ExampleBase
    {
        #region Constructors

        /// <summary>
        /// Construct the example
        /// </summary>
        public PrintNotesAndChords() : base("Prints notes and chords as they are played.") { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Runs the example
        /// </summary>
        public override void Run()
        {
            // Prompt user to choose an input device (or if there is only one, use that one).
            var inputDevice = ExampleUtil.ChooseInputDeviceFromConsole();
            if (inputDevice == null)
            {
                Console.WriteLine("No input devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            inputDevice.Open();
            inputDevice.StartReceiving(null);

            new Summarizer(inputDevice);
            ExampleUtil.PressAnyKeyToContinue();
            inputDevice.StopReceiving();
            inputDevice.Close();
            inputDevice.RemoveAllEventHandlers();
        }

        #endregion Methods
    }
}