using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public class KeyCodeConvertor
    {
        public Note getNote(string input)
        {
            if(input.Contains("Command: NoteOn") || input.Contains("Command: NoteOff"))
            {
                String[] attributes = input.Split(',');
                int testValue = int.Parse(attributes[0].Substring(9));

                string noteValue = "";
                int noteAlter = 0;
                MusicalSymbolDuration duration = MusicalSymbolDuration.Whole;

                switch (testValue % 12)
                {
                    // http://computermusicresource.com/midikeys.html
                    case 0:
                        //  C
                        noteValue = "C";
                        break;
                    case 1:
                        //  C#/Db
                        noteValue = "C";
                        noteAlter = 1;
                        break;
                    case 2:
                        //  D
                        noteValue = "D";
                        break;
                    case 3:
                        //  D#/Eb
                        noteValue = "D";
                        noteAlter = 1;
                        break;
                    case 4:
                        //  E
                        noteValue = "E";
                        break;
                    case 5:
                        //  F
                        noteValue = "F";
                        break;
                    case 6:
                        //  F#/Gb
                        noteValue = "F";
                        noteAlter = 1;
                        break;
                    case 7:
                        //  G
                        noteValue = "G";
                        break;
                    case 8:
                        //  G#/Ab
                        noteValue = "G";
                        noteAlter = 1;
                        break;
                    case 9:
                        //  A
                        noteValue = "A";
                        break;
                    case 10:
                        //  A#/Bb
                        noteValue = "A";
                        noteAlter = 1;
                        break;
                    case 11:
                        //  B
                        noteValue = "B";
                        break;
                }

                if(input.Contains("delta time: 0"))
                {
                    return null;
                } else if (input.Contains("delta time: 4"))
                {
                    duration = MusicalSymbolDuration.Quarter;
                }
                else if (input.Contains("delta time: 384"))
                {
                    duration = MusicalSymbolDuration.Eighth;
                }
                else if (input.Contains("delta time: 192"))
                {
                    duration = MusicalSymbolDuration.Sixteenth;
                }
                else if (input.Contains("delta time: 96"))
                {
                    duration = MusicalSymbolDuration.d32nd;
                }
                else if (input.Contains("delta time: 48"))
                {
                    duration = MusicalSymbolDuration.d64th;
                }
                else if (input.Contains("delta time: 24"))
                {
                    duration = MusicalSymbolDuration.d128th;
                }

                int octave = testValue / 12 - 1;
                //Console.WriteLine("-----------------------------------------------------------------------------------");
                //Console.WriteLine(input);
                //Console.WriteLine("went to");
                //Console.WriteLine("note: " + noteValue + ", alter: " + noteAlter + ", octave: " + octave + ", duration: " + duration);
                return new Note(noteValue, noteAlter, octave, duration, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start });
            } else
            {
                Console.WriteLine(input);
                return null;
            }
        }
    }
}
