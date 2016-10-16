using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controller;
using DPA_Musicsheets.Editor;
using DPA_Musicsheets.Lily;
using DPA_Musicsheets.Midi;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.MusicComponentModels;
using Microsoft.Win32;
using PSAMControlLibrary;
using PSAMWPFControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DPA_Musicsheets
{
    public partial class MainWindow : Window, CommandTarget
    {
        private MidiPlayer _player;
        private int savedMementos;
        private int currentMemento;
        private Originator originator;
        private CareTaker careTaker;
        private ADPKeyHandler keyHandler;


        public ObservableCollection<MidiTrack> MidiTracks { get; private set; }

        // De OutputDevice is een midi device of het midikanaal van je PC.
        // Hierop gaan we audio streamen.
        // DeviceID 0 is je audio van je PC zelf.
        private OutputDevice _outputDevice = new OutputDevice(0);

        public MainWindow()
        {
            this.MidiTracks = new ObservableCollection<MidiTrack>();
            keyHandler = new ADPKeyHandler(this);
            InitializeComponent();
            DataContext = MidiTracks;
            //FillPSAMViewer();
            initializeEditor();

            //FillPSAMViewer();
            //notenbalk.LoadFromXmlFile("Resources/example.xml");
        }

        private void initializeEditor()
        {
            savedMementos = 0;
            currentMemento = 0;
            originator = new Originator();
            careTaker = new CareTaker();

            originator.setState(lilypondText.Text);
            careTaker.add(originator.storeInMemento());
            savedMementos++;
            currentMemento++;

            ReEvaluateButtons();
        }

        private IncipitViewerWPF createNewBarline()
        {
            IncipitViewerWPF barLine = new IncipitViewerWPF();
            barLine.Width = 525;

            barLine.AddMusicalSymbol(new Clef(ClefType.GClef, 2));

            return barLine;
        }

        private IncipitViewerWPF createNewBarline(int _timeSignatureUp, int _timeSignatureDown)
        {
            IncipitViewerWPF barLine = new IncipitViewerWPF();
            barLine.Width = 525;

            barLine.AddMusicalSymbol(new Clef(ClefType.GClef, 2));
            barLine.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)_timeSignatureUp, (uint)_timeSignatureDown));
            barLine.AddMusicalSymbol(new Barline());

            return barLine;
        }

        private void ShowADPTrack(ADPTrack _myTrack)
        {
            int barCount = 0;
            int[] timeSignature = new int[2];

            // clear view
            barlinesStackPanel.Children.Clear();

            IncipitViewerWPF barLine = createNewBarline();

            foreach (ADPBar tempBar in _myTrack.Bars)
            {
                if (tempBar.TimeSignature != timeSignature)
                {
                    timeSignature = tempBar.TimeSignature;
                    //barLine.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, 4, 4));
                    barLine.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)timeSignature[0], (uint)timeSignature[1]));
                }

                // add symbols
                foreach(ADPMusicalSymbol tempSymbol in tempBar.MusicalSymbols)
                {
                    if (tempSymbol.GetType() == typeof(ADPRest) )
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
                    barlinesStackPanel.Children.Add(barLine);
                    barLine = createNewBarline();
                    barCount = 0;
                }
            }
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

        private void ShowTrack(MyTrack _myTrack, int _timeSignatureUp, int _timeSignatureDown)
        {
            int barCount = 0;
            barlinesStackPanel.Children.Clear();
            IncipitViewerWPF barLine = createNewBarline(_timeSignatureUp, _timeSignatureDown);

            foreach (MyMusicalSymbol tempNote in _myTrack.Notes)
            {
                if (tempNote.IsPause)
                {
                    //rest
                    barLine.AddMusicalSymbol(new Rest(tempNote.Duration));
                } else
                {
                    //note
                    if (tempNote.HasDot)
                    {
                        barLine.AddMusicalSymbol(new Note(tempNote.Key, tempNote.Alter, tempNote.Octave, tempNote.Duration, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }) { NumberOfDots = 1 });
                    } else
                    {
                        barLine.AddMusicalSymbol(new Note(tempNote.Key, tempNote.Alter, tempNote.Octave, tempNote.Duration, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
                    }
                }

                if (tempNote.IsEndOfBar)
                {
                    barCount++;
                    barLine.AddMusicalSymbol(new Barline());
                    if(barCount == 3)
                    {
                        barlinesStackPanel.Children.Add(barLine);
                        barLine = createNewBarline(_timeSignatureUp, _timeSignatureDown);
                        barCount = 0;
                    }
                }
            }
        }

        private void FillPSAMViewer()
        {
            //String s = txt_MidiFilePath.Text;
            IEnumerable<MidiTrack> testList = MidiReader.ReadMidi(txt_MidiFilePath.Text);
            staff.ClearMusicalIncipit();
            // Clef = sleutel
            staff.AddMusicalSymbol(new Clef(ClefType.GClef, 2));
            staff.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, 4, 4));
            /* 
                The first argument of Note constructor is a string representing one of the following names of steps: A, B, C, D, E, F, G. 
                The second argument is number of sharps (positive number) or flats (negative number) where 0 means no alteration. 
                The third argument is the number of an octave. 
                The next arguments are: duration of the note, stem direction and type of tie (NoteTieType.None if the note is not tied). 
                The last argument is a list of beams. If the note doesn't have any beams, it must still have that list with just one 
                    element NoteBeamType.Single (even if duration of the note is greater than eighth). 
                    To make it clear how beamlists work, let's try to add a group of two beamed sixteenths and eighth:
                        Note s1 = new Note("A", 0, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start});
                        Note s2 = new Note("C", 1, 5, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Continue, NoteBeamType.End });
                        Note e = new Note("D", 0, 5, MusicalSymbolDuration.Eighth, NoteStemDirection.Down, NoteTieType.None,new List<NoteBeamType>() { NoteBeamType.End });
                        viewer.AddMusicalSymbol(s1);
                        viewer.AddMusicalSymbol(s2);
                        viewer.AddMusicalSymbol(e); 
            */

            staff.AddMusicalSymbol(new Note("A", 1, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start }));
            staff.AddMusicalSymbol(new Note("A", 0, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.Stop, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start }));
            staff.AddMusicalSymbol(new Note("C", 1, 5, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Continue, NoteBeamType.End }));
            staff.AddMusicalSymbol(new Note("D", 0, 5, MusicalSymbolDuration.Eighth, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.End }));
            staff.AddMusicalSymbol(new Barline());

            staff.AddMusicalSymbol(new Note("D", 0, 5, MusicalSymbolDuration.Whole, NoteStemDirection.Down, NoteTieType.Stop, new List<NoteBeamType>() { NoteBeamType.Single }));
            staff.AddMusicalSymbol(new Note("E", 0, 4, MusicalSymbolDuration.Quarter, NoteStemDirection.Up, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Single }) { NumberOfDots = 1 });
            staff.AddMusicalSymbol(new Barline());

            //staff.AddMusicalSymbol(new Rest
            staff.AddMusicalSymbol(new Note("C", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
            staff.AddMusicalSymbol(
                new Note("E", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                { IsChordElement = true });
            staff.AddMusicalSymbol(
                new Note("G", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                { IsChordElement = true });
            staff.AddMusicalSymbol(new Barline());
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(_player != null)
            {
                _player.Dispose();
            }

            _player = new MidiPlayer(_outputDevice);
            _player.Play(txt_MidiFilePath.Text);
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //Console.WriteLine("-----------------undo-------- ------ ------undo----------------");
            CareTaker c = careTaker;
            if (currentMemento >= 1)
            {
                //Console.WriteLine("from #"+(currentMemento+1)+" to #"+currentMemento+". going to previous version: "+careTaker.get(currentMemento - 1).getState());
                lilypondText.Text = originator.restoreFromMemento(careTaker.get(currentMemento - 1));
                currentMemento--;
            }


            ReEvaluateButtons();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            if (currentMemento < savedMementos)
            {
                currentMemento++;
                lilypondText.Text = originator.restoreFromMemento(careTaker.get(currentMemento - 1));
            }

            ReEvaluateButtons();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            Console.WriteLine("----------------------------- Update --------------------------");
        }

        private void ReEvaluateButtons()
        {
            if (savedMementos > 1)
            {
                btnUndo.IsEnabled = true;
            }
            else
            {
                btnUndo.IsEnabled = false;
            }

            if (currentMemento < savedMementos)
            {
                btnRedo.IsEnabled = true;
            }
            else
            {
                btnRedo.IsEnabled = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ... Get control that raised this event.
            originator.setState(lilypondText.Text);
            careTaker.add(originator.storeInMemento());
            savedMementos++;
            currentMemento++;

            ReEvaluateButtons();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            //Console.WriteLine("hier...");
            //TextBox textBox = sender as TextBox;
            //originator.setState(textBox.Text);
            //careTaker.add(originator.storeInMemento());
            //savedMementos++;
            //currentMemento++;

            //if (savedMementos > 1)
            //{
            //    btnUndo.IsEnabled = true;
            //}
            //else
            //{
            //    btnUndo.IsEnabled = false;
            //}

            //if (currentMemento < savedMementos)
            //{
            //    btnRedo.IsEnabled = true;
            //}
            //else
            //{
            //    btnRedo.IsEnabled = false;
            //}
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }
        
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
                _player.Dispose();
        }

        private void btn_ShowContent_Click(object sender, RoutedEventArgs e)
        {
            string extension = txt_MidiFilePath.Text.Split('.').Last();

            if (extension == "mid")
            {
                ShowMidiTracks(MidiReader.ReadMidi(txt_MidiFilePath.Text));
                //MidiConverter midiConverter = new MidiConverter();
                //MyMusicSheet mss = midiConverter.convertMidi(txt_MidiFilePath.Text);
                //ShowTrack(mss.Tracks[1], mss.TimeSignature[0], mss.TimeSignature[1]);
                MidiADPConverter midiConverter = new MidiADPConverter();
                ADPSheet sheet = midiConverter.convertMidi(txt_MidiFilePath.Text);
                ShowADPTrack(sheet.Tracks[1]);
            }
            // TODO: add lilypond file extension
        }

        private void ShowMidiTracks(IEnumerable<MidiTrack> midiTracks)
        {
            MidiTracks.Clear();
            foreach (var midiTrack in midiTracks)
            {
                MidiTracks.Add(midiTrack);
            }

            tabCtrl_MidiContent.SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _outputDevice.Close();
            if (_player != null)
            {
                _player.Dispose();
            }
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi Files(.mid)|*.mid|Lily files (*.ly*)|*.ly*" };
            if (openFileDialog.ShowDialog() == true)
            {
                txt_MidiFilePath.Text = openFileDialog.FileName;
                //FillTestPSAMViewer();
                string ext = System.IO.Path.GetExtension(openFileDialog.FileName);
                if (ext == ".mid")
                {
                    MidiADPConverter midiConverter = new MidiADPConverter();
                    ADPSheet sheet = midiConverter.convertMidi(txt_MidiFilePath.Text);
                    ShowADPTrack(sheet.Tracks[1]);
                    NoteToLilypondConverter ntlc = new NoteToLilypondConverter();
                    lilypondText.Text = ntlc.getLilypond(sheet);
                }
                else if (ext == ".ly")
                {
                    LilyADPConverter lilyConverter = new LilyADPConverter(txt_MidiFilePath.Text);
                    ADPSheet sheet = lilyConverter.readContent();
                    ShowADPTrack(sheet.Tracks[0]);
                    lilypondText.Text = System.IO.File.ReadAllText(txt_MidiFilePath.Text);
                    //LilypondToPDF l2pdf = new LilypondToPDF(txt_MidiFilePath.Text); //De Lilypond to PDF converter wordt zo aangeroepen
                    //SaveFileToPdf();
                }
            }
        }

        public void SaveFileToLilypond()
        {
            SaveAsLilypond saly = new SaveAsLilypond(lilypondText.Text);
        }

        public void SaveFileToPdf()
        {
            LilypondToPDF l2pdf = new LilypondToPDF(lilypondText.Text);
        }

        public void AddTekstAtSelection(string _text)
        {
            int selectionStart = lilypondText.SelectionStart;
            lilypondText.SelectedText = _text;
        }

        private void OnKeyPressed(object sender, RoutedEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Is Alt key pressed
            {
                keyHandler.OnKeyPressed();
            }
        }

        public void AddBarlinesToEditor() //Werkt op het moment alleen met 4/4 maatsoort
        {
            //int[] timeSignature = new int[2];

            //string[] lilyPondContents = lilypondText.Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray(); // get time signature (skip this, it only works for 4/4)
            //for (int i = 0; i < lilyPondContents.Length; i++)
            //{
            //    lilyPondContents[i] = lilyPondContents[i].Replace("\r\n", string.Empty);
            //    lilyPondContents[i] = lilyPondContents[i].Replace("\n", string.Empty);
            //    if (lilyPondContents[i].Contains("time"))
            //    {
            //        string str = lilyPondContents[i + 1];
            //        timeSignature[0] = (int)Char.GetNumericValue(str[0]);
            //        timeSignature[1] = (int)Char.GetNumericValue(str[2]);


            //    }
            //}

            int e = lilypondText.Text.Length;
            string selectedpart = lilypondText.SelectedText;
            double counter = 0;
            string[] notes = selectedpart.Split(' ');
            string result = "";
            if (selectedpart == "" || selectedpart.Contains("{") || selectedpart.Contains("}") || selectedpart.Contains("\\"))
            {
                return;
            }

            for (int x = 0; x < notes.Length; x++)
            {
                string temp = Regex.Match(notes[x], @"\d+").Value;
                if (temp == "")
                {
                    //stop
                }
                else
                {
                    int d = Int32.Parse(temp);
                    double y = 0;
                    result += notes[x] + " ";

                    switch (d) //works for 4/4
                    {
                        case 1:
                            y += 4;
                            break;
                        case 2:
                            y += 2;
                            break;
                        case 4:
                            y += 1;
                            break;
                        case 8:
                            y += 0.5;
                            break;
                        case 16:
                            y += 0.25;
                            break;
                    }
                    if (notes[x].Contains("."))
                    {
                        y = y + (y / 2);
                    };

                    counter += y;
                    if (counter >= 4) //only works for 4/4
                    {
                        if (notes[x + 1].Contains("|\n"))
                        {
                            counter = 0;
                        }
                        else
                        {
                            result += "| ";
                            counter = 0;
                        }


                    }
                }

            }

            string s = lilypondText.Text.Replace(selectedpart, result);
            lilypondText.Text = s;
        }
    }
}
