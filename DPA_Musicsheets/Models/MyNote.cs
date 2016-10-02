﻿using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class MyNote
    {
        private List<string> keyValues;
        public string Key { get; set; }
        public enum alterValue { none, sharp, flat };
        public alterValue Alter { get; set; }
        public int Octave { get; set; }
        public double Type { get; set; }
        public bool IsPause { get; set; }
        public int AbsoluteTicks { get; set; }
        public bool IsEndOfBar { get; set; }

        public MyNote(int _keyValue, int _absoluteTicks)
        {
            InitKeyValues();
            SetKey(_keyValue % 12);
            Octave = _keyValue / 12;
            AbsoluteTicks = _absoluteTicks;
        }

        public void InitKeyValues()
        {
            keyValues = new List<string>();
            keyValues.Add("C");
            keyValues.Add("C#");
            keyValues.Add("D");
            keyValues.Add("D#");
            keyValues.Add("E");
            keyValues.Add("F");
            keyValues.Add("F#");
            keyValues.Add("G");
            keyValues.Add("G#");
            keyValues.Add("A");
            keyValues.Add("A#");
            keyValues.Add("B");
        }

        public void SetKey(int keyCode)
        {
            this.Key = keyValues[keyCode];
        }

    }
}
