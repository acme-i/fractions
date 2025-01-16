using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ascii
{
    public class AsciiReader : StreamReader
    {
        public AsciiReader(string path) : base(path)
        {

        }

        public override string ReadToEnd()
        {

            return base.ReadToEnd();
        }

    }
}
