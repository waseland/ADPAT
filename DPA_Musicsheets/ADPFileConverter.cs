using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public abstract class ADPFileConverter
    {
        private ADPFileConverter nextADPFileConverter;
        private string ext;

        public ADPFileConverter()
        {
            ext = ".mid";
        }

        public void SetNextADPFileConverter(ADPFileConverter _nextADPFileConverter)
        {
            nextADPFileConverter = _nextADPFileConverter;
        }

        public abstract ADPSheet ReadFile(string _path);
    }
}
