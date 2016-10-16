using DPA_Musicsheets.Adapter;
using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controller;
using DPA_Musicsheets.Editor;
using DPA_Musicsheets.Lily;
using DPA_Musicsheets.Midi;
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
        private PSAMAdapter psamAdapter;


        public ObservableCollection<MidiTrack> MidiTracks { get; private set; }

        private OutputDevice _outputDevice = new OutputDevice(0);

        public MainWindow()
        {
            this.MidiTracks = new ObservableCollection<MidiTrack>();
            keyHandler = new ADPKeyHandler(this);
            psamAdapter = new PSAMAdapter();
            InitializeComponent();
            DataContext = MidiTracks;
            ShowSampleVisualisation();
            initializeEditor();
        }

        private void initializeEditor()
        {
            savedMementos = 0;
            currentMemento = 0;
            originator = new Originator();
            careTaker = new CareTaker();

            SetNewState();

            ReEvaluateButtons();
        }

        private void ShowSheetVisualisation(ADPTrack _adpTrack)
        {
            barlinesScrollViewer.Content = psamAdapter.GetSheetVisualisation(_adpTrack);
        }

        private void ShowSampleVisualisation()
        {
            barlinesScrollViewer.Content = psamAdapter.GetSampleVisualisation();
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
            CareTaker c = careTaker;
            if (currentMemento >= 1)
            {
                currentMemento--;
                lilypondText.Text = originator.restoreFromMemento(careTaker.get(currentMemento));
            }


            ReEvaluateButtons();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
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

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // TODO remove
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
                ShowSheetVisualisation(sheet.Tracks[1]);
            }
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
                string ext = System.IO.Path.GetExtension(openFileDialog.FileName);
                if (ext == ".mid")
                {
                    MidiADPConverter midiConverter = new MidiADPConverter();
                    ADPSheet sheet = midiConverter.convertMidi(txt_MidiFilePath.Text);
                    ShowSheetVisualisation(sheet.Tracks[1]);
                    NoteToLilypondConverter ntlc = new NoteToLilypondConverter();
                    lilypondText.Text = ntlc.getLilypond(sheet);
                    SetNewState();
                }
                else if (ext == ".ly")
                {
                    LilyADPConverter lilyConverter = new LilyADPConverter();
                    ADPSheet sheet = lilyConverter.ReadFile(txt_MidiFilePath.Text);
                    ShowSheetVisualisation(sheet.Tracks[0]);
                    lilypondText.Text = System.IO.File.ReadAllText(txt_MidiFilePath.Text);
                    SetNewState();
                    LilypondToPDF l2pdf = new LilypondToPDF(txt_MidiFilePath.Text); //De Lilypond to PDF converter wordt zo aangeroepen
                }
            }
        }

        public void SaveFileToLilypond()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void SaveFileToPdf()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void AddTekstAtSelection(string _text)
        {
            int selectionStart = lilypondText.SelectionStart;
            lilypondText.SelectedText = _text;
            SetNewState();
        }

        private void OnKeyDown(object sender, RoutedEventArgs e)
        {
            keyHandler.OnKeyPressed();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Back)
            {
                SetNewState();
            }
        }

        public void SetNewState()
        {
            originator.setState(lilypondText.Text);
            careTaker.add(originator.storeInMemento());
            savedMementos++;
            currentMemento++;

            ReEvaluateButtons();
        }
    }
}
