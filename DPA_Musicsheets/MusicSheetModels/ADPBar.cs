﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.MusicSheetModels
{
    public class ADPBar
    {
        public List<ADPMusicalSymbol> MusicalSymbols { get; set; }

        public ADPBar()
        {
            MusicalSymbols = new List<ADPMusicalSymbol>();
        }
    }
}