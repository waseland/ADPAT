using DPA_Musicsheets.Adapter;
using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controller;
using DPA_Musicsheets.Controllers;
using DPA_Musicsheets.Editor;
using DPA_Musicsheets.Lily;
using DPA_Musicsheets.Midi;
using DPA_Musicsheets.MusicComponentModels;
using Microsoft.Win32;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DPA_Musicsheets
{
    public partial class MainWindow : Window, CommandTarget
    {
        private MidiPlayer player;
        private int savedMementos;
        private int currentMemento;
        private Originator originator;
        private CareTaker careTaker;
        private ADPKeyHandler keyHandler;
        private PSAMAdapter psamAdapter;
        private ADPFileConverter firstFileConverter;
        private bool needsSaving;
        private LilyADPConverter lilyADPConverter;
        private FileExporter fileExporter;


        public ObservableCollection<MidiTrack> MidiTracks { get; private set; }

        private OutputDevice outputDevice = new OutputDevice(0);

        public MainWindow()
        {
            lilyADPConverter = new LilyADPConverter();
            fileExporter = new FileExporter();
            this.MidiTracks = new ObservableCollection<MidiTrack>();
            keyHandler = new ADPKeyHandler(this);
            psamAdapter = new PSAMAdapter();
            InitializeComponent();
            DataContext = MidiTracks;
            showSampleVisualisation();
            initializeEditor();
            initializeFileConverters();
            needsSaving = false;
        }

        private void initializeFileConverters() //initializes the FileConverters (gets called in the constructor of MainWindow)
        {
            LilyADPConverter lyConverter = new LilyADPConverter();
            MidiADPConverter midConverter = new MidiADPConverter();

            lyConverter.SetNextADPFileConverter(midConverter);

            firstFileConverter = lyConverter;
        }

        private void initializeEditor() //initializes the Editor (gets called in the constructor of MainWindow)
        {
            savedMementos = 0;
            currentMemento = 0;
            originator = new Originator();
            careTaker = new CareTaker();

            SetNewState();

            reEvaluateButtons();
        }

        private void showSheetVisualisation(ADPTrack _adpTrack) //ChannelCommand to show the Sheet in the MainWindow via the PSAMAdapter, takes a ADPTrack as parameter
        {
            barlinesScrollViewer.Content = psamAdapter.GetSheetVisualisation(_adpTrack);
        }

        private void showSampleVisualisation() //Shows the basic sample of how a Barline looks like in the MainWindow via the PSAMAdapter, takes a ADPTrack as paramter
        {
            barlinesScrollViewer.Content = psamAdapter.GetSampleVisualisation();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e) //This gets called whenever the play button is clicked in the MainWindow (Plays the Midi File)
        {
            if(player != null)
            {
                player.Dispose();
            }

            player = new MidiPlayer(outputDevice);
            player.Play(txt_MidiFilePath.Text);
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e) //Undo's a chance in the editor, gets called by clicking the undo button in the MainWinwdow
        {
            CareTaker c = careTaker;
            if (currentMemento >= 1)
            {
                currentMemento--;
                lilypondText.Text = originator.RestoreFromMemento(careTaker.Get(currentMemento));
                keyHandler.RestartThread();
            }


            reEvaluateButtons();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e) //Redo's a chance in the editor, gets called by clicking the undo button in the MainWinwdow
        {
            if (currentMemento < savedMementos)
            {
                currentMemento++;
                lilypondText.Text = originator.RestoreFromMemento(careTaker.Get(currentMemento - 1));
                keyHandler.RestartThread();
            }

            reEvaluateButtons();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            Console.WriteLine("----------------------------- Update --------------------------");
        }

        private void reEvaluateButtons() //Checks if the undo and redo button should be enabled or not
        {
            if (savedMementos > 1 && currentMemento > 0)
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

        private void btnOpen_Click(object sender, RoutedEventArgs e) //Calls the openfile function (See function: OpenFile)
        {
            // TODO remove
            OpenFile();
        }
        
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
                player.Dispose();
        }

        private void btn_ShowContent_Click(object sender, RoutedEventArgs e) //Shows the content of a midi file in the tracks section of the MainWindow, also shows the sheetvisualisation
        {
            string extension = txt_MidiFilePath.Text.Split('.').Last();

            if (extension == "mid")
            {
                showMidiTracks(MidiReader.ReadMidi(txt_MidiFilePath.Text));
                //MidiConverter midiConverter = new MidiConverter();
                //MyMusicSheet mss = midiConverter.convertMidi(txt_MidiFilePath.Text);
                //ShowTrack(mss.Tracks[1], mss.TimeSignature[0], mss.TimeSignature[1]);
                MidiADPConverter midiConverter = new MidiADPConverter();
                ADPSheet sheet = midiConverter.ReadFile(txt_MidiFilePath.Text);
                showSheetVisualisation(sheet.Tracks[1]);
            }
        }

        private void showMidiTracks(IEnumerable<MidiTrack> _midiTracks)  //Shows the content of a midi file in the tracks section of the MainWindow
        {
            MidiTracks.Clear();
            foreach (var midiTrack in _midiTracks)
            {
                MidiTracks.Add(midiTrack);
            }

            tabCtrl_MidiContent.SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) //Function gets called when you want to close the MainWindow, checks if the editor needs saving
        {
            if(needsSaving)
            {
                string sMessageBoxText = "You haven't saved your progress. Do you want to save this before closing?";
                string sCaption = "Save";

                MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        SaveFileToLilypond();
                        outputDevice.Close();
                        if (player != null)
                        {
                            player.Dispose();
                        }
                        break;

                    case MessageBoxResult.No:
                        Console.WriteLine("CLOSING");
                        outputDevice.Close();
                        if (player != null)
                        {
                            player.Dispose();
                        }
                        break;
                }
            }
        }

        public void OpenFile() //Opens a filedialog where you can select a file to open, accepts: midi and lilypond files
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi Files(.mid)|*.mid|Lily files (*.ly*)|*.ly*" };
            if (openFileDialog.ShowDialog() == true)
            {
                txt_MidiFilePath.Text = openFileDialog.FileName;
                ADPSheet sheet = firstFileConverter.Handle(openFileDialog.FileName);
                if(sheet != null)
                {
                    showSheetVisualisation(sheet.getTrack());
                    NoteToLilypondConverter ntlc = new NoteToLilypondConverter();
                    lilypondText.Text = ntlc.GetLilypond(sheet);
                    SetNewState();
                }
            }
        }

        public void SaveFileToLilypond() //Channelcommand to save the editor as a a lilypond file (see class FileExporter)
        {
            fileExporter.SaveAsLilypond(lilypondText.Text);
            needsSaving = false;
        }

        public void SaveFileToPdf() //Channelcommand to save the editor as a a pdf file (see class FileExporter)
        {
            fileExporter.LilypondToPDF(lilypondText.Text);
        }

        public string UpdateBarlinesFromLilypond() //Channelcommand to update the barlines from the text in the editor (see class LilyADPConverter
        {
            return this.lilypondText.Dispatcher.Invoke(
                () =>
                {
                    ADPSheet sheet = lilyADPConverter.ConvertText(lilypondText.Text);
                    if (sheet != null)
                    {
                        showSheetVisualisation(sheet.getTrack());
                    }
                    return this.lilypondText.Text;
                }
            );
        }

        public void AddTekstAtSelection(string _text) //replaces text to a specific selection of the editor, takes a string
        {
            int selectionStart = lilypondText.SelectionStart;
            lilypondText.SelectedText = _text;
            SetNewState();
        }

        private void OnKeyDown(object sender, RoutedEventArgs e) //gets called whenever a key is pressed, sends the event to they KeyHandler
        {
            keyHandler.OnKeyPressed();
        }

        private void OnKeyUp(object sender, KeyEventArgs e) //gets called whenever a key is released, sends the event to they KeyHandler, mostly used for backspace
        {
            if (e.Key == System.Windows.Input.Key.Back)
            {
                keyHandler.OnKeyPressed();
            }
        }

        public void SetNewState() //Sets a new state in the memento pattern
        {
            needsSaving = true;
            originator.SetState(lilypondText.Text);
            careTaker.Add(originator.StoreInMemento());
            savedMementos++;
            currentMemento++;

            reEvaluateButtons();
        }

        public void AddBarlinesToEditor() //Adds barlines where missing to the selected text in the editor
        {
            int[] timeSignature = new int[2];

            string[] lilypondContents = lilypondText.Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray(); // get time signature
            for (int i = 0; i < lilypondContents.Length; i++)
            {
                lilypondContents[i] = lilypondContents[i].Replace("\r\n", string.Empty);
                lilypondContents[i] = lilypondContents[i].Replace("\n", string.Empty);
                if (lilypondContents[i].Contains("time"))
                {
                    string str = lilypondContents[i + 1];
                    timeSignature[0] = (int)Char.GetNumericValue(str[0]);
                    timeSignature[1] = (int)Char.GetNumericValue(str[2]);
                }
            }

            if (timeSignature != null)
            {
                string selectedpart = lilypondText.SelectedText;
                double barLengthCounter = 0;
                string[] notes = selectedpart.Split(' ');
                string result = "";

                double singleBeat = (double)(1.0 / timeSignature[1]);
                double lengthPerBar = timeSignature[0] * singleBeat;

                if (selectedpart == "" || selectedpart.Contains("{") || selectedpart.Contains("}") || selectedpart.Contains("\\"))
                {
                    return;
                }

                for (int x = 0; x < notes.Length; x++)
                {
                    string durationValue = Regex.Match(notes[x], @"\d+").Value;
                    if (durationValue != "")
                    {
                        int duration = Int32.Parse(durationValue);
                        double tempNoteLength = 0;
                        result += notes[x] + " ";

                        switch (duration)
                        {
                            case 1:
                                tempNoteLength = 1.0;
                                break;
                            case 2:
                                tempNoteLength += 0.5;
                                break;
                            case 4:
                                tempNoteLength += 0.25;
                                break;
                            case 8:
                                tempNoteLength = 0.125;
                                break;
                            case 16:
                                tempNoteLength = 0.0625;
                                break;
                        }
                        if (notes[x].Contains("."))
                        {
                            tempNoteLength = (double)((double)tempNoteLength + (double)(tempNoteLength / 2));
                        };

                        barLengthCounter = (double)((double)barLengthCounter + (double)tempNoteLength);
                        if (barLengthCounter >= lengthPerBar)
                        {
                            if (notes[x + 1].Contains("|\n"))
                            {
                                barLengthCounter = 0;
                            }
                            else
                            {
                                result += " | ";
                                barLengthCounter = 0;
                            }
                        }
                    }
                }

                string s = lilypondText.Text.Replace(selectedpart, result);
                lilypondText.Text = s;
            }
        }
    }
}
