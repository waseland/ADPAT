using System.Collections.Generic;

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
