using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.MusicSheetModels
{
    public class ADPRest : ADPMusicalSymbol
    {
        public ADPRest(int _duration)
        {
            this.Duration = _duration;
        }
    }
}
