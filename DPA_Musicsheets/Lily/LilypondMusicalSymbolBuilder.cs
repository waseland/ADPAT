using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Lily
{
    public class LilypondMusicalSymbolBuilder
    {
        public ADPMusicalSymbol BuildMusicalSymbol(string musicalSymbolInfo, ADPNote latestNote)
        {
            ADPMusicalSymbol resultMusicalSymbol;

            string key = musicalSymbolInfo.Substring(0, 1);
            string durationValue = Regex.Match(musicalSymbolInfo, @"\d+").Value;
            if(durationValue == "")
            {
                return null;
            }
            int duration = Int32.Parse(durationValue);

            if (key == "r")
            {
                resultMusicalSymbol = new ADPRest();
                resultMusicalSymbol.Duration = duration;
            }
            else
            {
                resultMusicalSymbol = new ADPNote();
                int amountOfDots = 0;
                ((ADPNote)resultMusicalSymbol).Key = key.ToUpper();
                ((ADPNote)resultMusicalSymbol).Octave = calculateOctave(musicalSymbolInfo, latestNote);
                ((ADPNote)resultMusicalSymbol).Duration = duration;
                while (musicalSymbolInfo.Contains("."))
                {
                    int x = musicalSymbolInfo.IndexOf(".");
                    musicalSymbolInfo = musicalSymbolInfo.Remove(x, 1);
                    amountOfDots++;
                }
                ((ADPNote)resultMusicalSymbol).AmountOfDots = amountOfDots;

                if (musicalSymbolInfo.Contains("is"))
                {
                    ((ADPNote)resultMusicalSymbol).Alter = 1;
                }
                else if (musicalSymbolInfo.Contains("es"))
                {
                    ((ADPNote)resultMusicalSymbol).Alter = 2;
                }
                else
                {
                    ((ADPNote)resultMusicalSymbol).Alter = 0;
                }
            }
            return resultMusicalSymbol;
        }

        private int calculateOctave(string noteInfo, ADPNote latestNote)
        {
            string newKey = noteInfo.Substring(0, 1).ToUpper();
            int resultOctave;

            string[] keys = { "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F", "G", "A", "B" };

            int[] differences = new int[3];

            int latestNoteIndex = Array.IndexOf(keys, latestNote.Key.Substring(0, 1), 7);

            int newKeyIndex1 = Array.IndexOf(keys, newKey, 0);
            int newKeyIndex2 = Array.IndexOf(keys, newKey, 7);
            int newKeyIndex3 = Array.IndexOf(keys, newKey, 14);

            differences[0] = Math.Abs(latestNoteIndex - newKeyIndex1);
            differences[1] = Math.Abs(latestNoteIndex - newKeyIndex2);
            differences[2] = Math.Abs(latestNoteIndex - newKeyIndex3);

            differences.Min();
            if (differences[0] == differences.Min())
            {
                // lower octave
                resultOctave = latestNote.Octave - 1;
            }
            else if (differences[1] == differences.Min())
            {
                //middle octave
                resultOctave = latestNote.Octave;
            }
            else
            {
                // higher octave
                resultOctave = latestNote.Octave + 1;
            }

            while (noteInfo.Contains("'"))
            {
                resultOctave++;
                noteInfo = noteInfo.Remove(noteInfo.IndexOf("'"), 1);
            }

            while (noteInfo.Contains(","))
            {
                resultOctave--;
                noteInfo = noteInfo.Remove(noteInfo.IndexOf(","), 1);
            }

            return resultOctave;
        }
    }
}
