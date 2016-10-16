using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public class NoteToLilypondConverter
    {
        public string getLilypond(ADPSheet musicSheet)
        {
            string result = "";
            result += "\\relative c' { \n";
            result += "\\clef treble \n";
            result += "\\time ";
            result += musicSheet.getTrack().Bars[1].TimeSignature[0];
            result += "/";
            result += musicSheet.getTrack().Bars[1].TimeSignature[1];
            result += " \n";
            result += "\\tempo 4=120 \n";

            // add notes
            result += convertMusicalSymbols(musicSheet.getTrack().Bars);

            result += "}";

            return result;
        }

        private string convertMusicalSymbols(List<ADPBar> _bars)
        {
            string result = "";
            ADPNote lastUsedNote = new ADPNote();
            lastUsedNote.Key = "C";
            lastUsedNote.Octave = 4;

            foreach (ADPBar bar in _bars)
            {
                foreach (ADPMusicalSymbol musicalSymbol in bar.MusicalSymbols)
                {
                    if (musicalSymbol is ADPNote)
                    {
                        result += ((ADPNote)musicalSymbol).Key.Substring(0, 1).ToLower();
                        if (((ADPNote)musicalSymbol).Alter == 1)
                        {
                            // C#
                            result += "is";
                        }
                        else if (((ADPNote)musicalSymbol).Alter == 2)
                        {
                            // Bb
                            result += "es";
                        }

                        result += calculateOctave((ADPNote)musicalSymbol, lastUsedNote);
                        result += musicalSymbol.Duration;

                        for (int i = 0; i < ((ADPNote)musicalSymbol).AmountOfDots; i++){
                            result += ".";
                        }

                        lastUsedNote = (ADPNote)musicalSymbol;
                    }
                    else
                    {
                        result += "r";
                        result += musicalSymbol.Duration;
                    }

                    
                    result += " ";
                }
                result += "| \n";
            }
            return result;
        }

        private string calculateOctave(ADPNote _newNote, ADPNote _oldNote)
        {
            string result = "";
            int nearestKeyOctave;

            string[] keys = { "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F", "G", "A", "B" };
            int[] differences = new int[3];

            int oldKeyIndex = Array.IndexOf(keys, _oldNote.Key.Substring(0,1), 7);

            int newKeyIndex1 = Array.IndexOf(keys, _newNote.Key.Substring(0,1), 0);
            int newKeyIndex2 = Array.IndexOf(keys, _newNote.Key.Substring(0, 1), 7);
            int newKeyIndex3 = Array.IndexOf(keys, _newNote.Key.Substring(0, 1), 14);

            differences[0] = Math.Abs(oldKeyIndex - newKeyIndex1);
            differences[1] = Math.Abs(oldKeyIndex - newKeyIndex2);
            differences[2] = Math.Abs(oldKeyIndex - newKeyIndex3);

            differences.Min();
            if(differences[0] == differences.Min())
            {
                // lower octave
                nearestKeyOctave = _oldNote.Octave - 1;
            } else if (differences[1] == differences.Min())
            {
                //middle octave
                nearestKeyOctave = _oldNote.Octave;
            } else
            {
                // higher octave
                nearestKeyOctave = _oldNote.Octave + 1;
            }

            if(_newNote.Octave > nearestKeyOctave)
            {
                for(int i = 0; i < (_newNote.Octave - nearestKeyOctave); i++)
                {
                    result += "'";
                }
                return result;
            } else if(_newNote.Octave < nearestKeyOctave)
            {
                for (int i = 0; i < (nearestKeyOctave - _newNote.Octave); i++)
                {
                    result += ",";
                }
                return result;
            } else
            {
                return "";
            }
        }
    }
}
