using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPBar
    {
        public List<ADPMusicalSymbol> MusicalSymbols { get; set; }
        public int[] TimeSignature { get; set; }

        public ADPBar()
        {
            MusicalSymbols = new List<ADPMusicalSymbol>();
        }
    }
}
