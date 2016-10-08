using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Midi
{
    public class MidiMusicalSymbolBuilder
    {
        private List<string> keyValues;

        public MidiMusicalSymbolBuilder()
        {
            InitKeyValues();
        }

        public ADPMusicalSymbol BuildMusicalSymbol(ChannelMessage _message, MidiEvent _midiEvent, double _wholeNoteLength)
        {
            if (_midiEvent.DeltaTicks == 0)
            {
                return null;
            }

            int[] tempDurationAndDots = calculateDurationAndDots(_midiEvent.DeltaTicks, _wholeNoteLength);

            if (_midiEvent.DeltaTicks != 0 && _message.Data2 != 0)
            {
                //rest 
                ADPRest rest = new ADPRest();
                rest.Duration = tempDurationAndDots[0];
                return rest;
            } else
            {
                //note
                ADPNote note = new ADPNote();
                note.Duration = tempDurationAndDots[0];
                note.AmountOfDots = tempDurationAndDots[1];
                note = SetKey(_message.Data1 % 12, note);
                note.Octave = _message.Data1 / 12 - 1;
                return note;
            }
        }

        private int[] calculateDurationAndDots(int _deltaTime, double _wholeNoteLength)
        {
            int[] result = { 0, 0 };

            if (_deltaTime - _wholeNoteLength == 0)
            {
                result[0] = 1;
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 2) >= 0)
            {
                result[0] = 2;
                if (_deltaTime - (_wholeNoteLength / 2) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 4) >= 0)
            {
                result[0] = 4;
                if (_deltaTime - (_wholeNoteLength / 4) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 8) >= 0)
            {
                result[0] = 8;
                if (_deltaTime - (_wholeNoteLength / 8) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 16) >= 0)
            {
                result[0] = 16;
                if (_deltaTime - (_wholeNoteLength / 16) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else
            {
                return result;
            }
        }

        private void InitKeyValues()
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

        private ADPNote SetKey(int keyCode, ADPNote _note)
        {
            _note.Key = keyValues[keyCode];
            if(_note.Key.Length > 1)
            {
                _note.Alter = 1;
            }
            return _note;
        }
    }
}
