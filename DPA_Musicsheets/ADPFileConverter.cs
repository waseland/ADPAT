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
        protected ADPFileConverter nextADPFileConverter;
        protected ADPMusicalSymbolFactory musicalSymbolFactory;
        protected string ext;

        public ADPFileConverter()
        {
            musicalSymbolFactory = new ADPMusicalSymbolFactory();
        }

        public void SetNextADPFileConverter(ADPFileConverter _nextADPFileConverter)
        {
            nextADPFileConverter = _nextADPFileConverter;
        }

        public ADPSheet Handle(string _path)
        {
            string tempExt = System.IO.Path.GetExtension(_path);
            if (tempExt.Equals(ext))
            {
                return ReadFile(_path);
            }
            else
            {
                if (nextADPFileConverter == null)
                {
                    return null;
                }
                else
                {
                    return nextADPFileConverter.Handle(_path);
                }
            }
        }

        public abstract ADPSheet ReadFile(string _path);
    }
}
