using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public class MyMusicalSymbolBuilder
    {
        public MyMusicalSymbol BuildMusicalSymbol(ChannelMessage _message, MidiEvent _midiEvent, double _wholeNoteLength)
        {
            if (_midiEvent.DeltaTicks == 0)
            {
                return null;
            }

            MyMusicalSymbol note = new MyMusicalSymbol(_message.Data1, _midiEvent.AbsoluteTicks);

            if (_midiEvent.DeltaTicks != 0 && _message.Data2 != 0)
            {
                note.IsPause = true;
            }

            DoubleValueVariable length = getNoteLength(_midiEvent.DeltaTicks, _wholeNoteLength);
            if (length != null) {
                note.HasDot = length.HasDot;
                note.Duration = length.Duration;

                if (note.AbsoluteTicks % _wholeNoteLength == 0)
                {
                    //end of bar
                    note.IsEndOfBar = true;
                }

                return note;
            } else
            {
                return null;
            }
        }

        private  DoubleValueVariable getNoteLength(int _deltaTime, double _wholeNoteLength)
        {
            DoubleValueVariable tempVariable = new DoubleValueVariable();
            if(_deltaTime - _wholeNoteLength == 0)
            {
                tempVariable.Duration = MusicalSymbolDuration.Whole;
                return tempVariable;
            } else if(_deltaTime - (_wholeNoteLength / 2) >= 0)
            {
                tempVariable.Duration = MusicalSymbolDuration.Half;
                if(_deltaTime - (_wholeNoteLength / 2) > 0)
                {
                    tempVariable.HasDot = true;
                }
                return tempVariable;
            }
            else if (_deltaTime - (_wholeNoteLength / 4) >= 0)
            {
                tempVariable.Duration = MusicalSymbolDuration.Quarter;
                if (_deltaTime - (_wholeNoteLength / 4) > 0)
                {
                    tempVariable.HasDot = true;
                }
                return tempVariable;
            }
            else if (_deltaTime - (_wholeNoteLength / 8) >= 0)
            {
                tempVariable.Duration = MusicalSymbolDuration.Eighth;
                if (_deltaTime - (_wholeNoteLength / 8) > 0)
                {
                    tempVariable.HasDot = true;
                }
                return tempVariable;
            }
            else if (_deltaTime - (_wholeNoteLength / 16) >= 0)
            {
                tempVariable.Duration = MusicalSymbolDuration.Sixteenth;
                if (_deltaTime - (_wholeNoteLength / 16) > 0)
                {
                    tempVariable.HasDot = true;
                }
                return tempVariable;
            } else
            {
                return null;
            }
        }
    }
}
