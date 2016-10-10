using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class DoubleValueVariable
    {
        public MusicalSymbolDuration Duration { get; set; }
        public bool HasDot { get; set; }

        public DoubleValueVariable()
        {
            HasDot = false;
        }
    }
}
