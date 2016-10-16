using DPA_Musicsheets.MusicComponentModels;
using PSAMControlLibrary;
using PSAMWPFControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DPA_Musicsheets.Adapter
{
    public class PSAMAdapter
    {
        public StackPanel GetSampleVisualisation()
        {
            StackPanel resultStackPanel = new StackPanel();
            IncipitViewerWPF barline = new IncipitViewerWPF();
            barline.Width = 500;

            barline.AddMusicalSymbol(new Clef(ClefType.GClef, 2));
            barline.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, 4, 4));

            barline.AddMusicalSymbol(new Note("A", 1, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start }));
            barline.AddMusicalSymbol(new Note("A", 0, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.Stop, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start }));
            barline.AddMusicalSymbol(new Note("C", 1, 5, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Continue, NoteBeamType.End }));
            barline.AddMusicalSymbol(new Note("D", 0, 5, MusicalSymbolDuration.Eighth, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.End }));
            barline.AddMusicalSymbol(new Barline());

            barline.AddMusicalSymbol(new Note("D", 0, 5, MusicalSymbolDuration.Whole, NoteStemDirection.Down, NoteTieType.Stop, new List<NoteBeamType>() { NoteBeamType.Single }));
            barline.AddMusicalSymbol(new Note("E", 0, 4, MusicalSymbolDuration.Quarter, NoteStemDirection.Up, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Single }) { NumberOfDots = 1 });
            barline.AddMusicalSymbol(new Barline());

            //staff.AddMusicalSymbol(new Rest
            barline.AddMusicalSymbol(new Note("C", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
            barline.AddMusicalSymbol(
                new Note("E", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                { IsChordElement = true });
            barline.AddMusicalSymbol(
                new Note("G", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                { IsChordElement = true });
            barline.AddMusicalSymbol(new Barline());

            resultStackPanel.Children.Add(barline);
            return resultStackPanel;
        }

        public StackPanel GetSheetVisualisation(ADPTrack _adpTrack)
        {
            StackPanel resultStackPanel = new StackPanel();
            int barCount = 0;
            bool barlineHasTimeSignature = false;
            int[] currentTimeSignature = new int[2];

            IncipitViewerWPF barLine = createNewBarline();

            foreach (ADPBar tempBar in _adpTrack.Bars)
            {
                if (!barlineHasTimeSignature)
                {
                    currentTimeSignature[0] = tempBar.TimeSignature[0];
                    currentTimeSignature[1] = tempBar.TimeSignature[1];
                    barLine.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)tempBar.TimeSignature[0], (uint)tempBar.TimeSignature[1]));
                    barLine.AddMusicalSymbol(new Barline());
                    barlineHasTimeSignature = true;
                } else if(currentTimeSignature[0] != tempBar.TimeSignature[0] && currentTimeSignature[1] != tempBar.TimeSignature[1])
                {
                    currentTimeSignature[0] = tempBar.TimeSignature[0];
                    currentTimeSignature[1] = tempBar.TimeSignature[1];
                    barLine.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)tempBar.TimeSignature[0], (uint)tempBar.TimeSignature[1]));
                }

                // add symbols
                foreach (ADPMusicalSymbol tempSymbol in tempBar.MusicalSymbols)
                {
                    if (tempSymbol.GetType() == typeof(ADPRest))
                    {
                        //rest
                        barLine.AddMusicalSymbol(new Rest(ConvertDuration(tempSymbol.Duration)));
                    }
                    else
                    {
                        ADPNote tempNote = (ADPNote)tempSymbol;
                        //note
                        if (tempNote.AmountOfDots > 0)
                        {
                            barLine.AddMusicalSymbol(new Note(tempNote.Key, tempNote.Alter, tempNote.Octave, ConvertDuration(tempNote.Duration), NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }) { NumberOfDots = tempNote.AmountOfDots });
                        }
                        else
                        {
                            barLine.AddMusicalSymbol(new Note(tempNote.Key, tempNote.Alter, tempNote.Octave, ConvertDuration(tempNote.Duration), NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
                        }
                    }
                }

                // add endOfBarLine

                barCount++;
                barLine.AddMusicalSymbol(new Barline());
                if (barCount == 3)
                {
                    resultStackPanel.Children.Add(barLine);
                    barLine = createNewBarline();
                    barCount = 0;
                    barlineHasTimeSignature = false;
                }
            }
            if(barCount > 0)
            {
                resultStackPanel.Children.Add(barLine);
            }

            return resultStackPanel;
        }

        private MusicalSymbolDuration ConvertDuration(int _duration)
        {
            switch (_duration)
            {
                case 16:
                    return MusicalSymbolDuration.Sixteenth;
                case 8:
                    return MusicalSymbolDuration.Eighth;
                case 4:
                    return MusicalSymbolDuration.Quarter;
                case 2:
                    return MusicalSymbolDuration.Half;
                case 1:
                    return MusicalSymbolDuration.Whole;
                default:
                    return MusicalSymbolDuration.Unknown;
            }
        }

        private IncipitViewerWPF createNewBarline()
        {
            IncipitViewerWPF barLine = new IncipitViewerWPF();
            barLine.Width = 525;

            barLine.AddMusicalSymbol(new Clef(ClefType.GClef, 2));

            return barLine;
        }
    }
}
