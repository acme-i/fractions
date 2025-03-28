// Copyright (c) 2009, Tom Lokovic All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

namespace fractions
{
    /// <summary>Exception thrown when an operation on a MIDI device cannot be satisfied</summary>
    public class DeviceException : System.ApplicationException
    {
        #region Constructors

        /// <summary><see cref="System.ApplicationException()"/></summary>
        public DeviceException() : base() { }

        /// <summary><see cref="System.ApplicationException(string)"/></summary>
        public DeviceException(string message) : base(message) { }

        /// <summary><see cref="System.ApplicationException(string, System.Exception)"/></summary>
        public DeviceException(string message, System.Exception innerException) : base(message, innerException) { }

        #endregion Constructors
    }
}