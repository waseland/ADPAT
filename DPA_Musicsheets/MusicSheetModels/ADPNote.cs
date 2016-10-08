using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.MusicSheetModels
{
    public class ADPNote : ADPMusicalSymbol
    {
        public string Key { get; set; }
        public int Alter { get; set; }
        public int Octave { get; set; }
        public int AmountOfDots { get; set; }

        public ADPNote()
        {
            AmountOfDots = 0;
        }
    }
}
