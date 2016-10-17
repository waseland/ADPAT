using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace DPA_Musicsheets.Controllers
{
    public class FileExporter
    {
        public void SaveAsLilypond(string _text)
        {
            if (_text == "")
            {

            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Ly Files(*.ly)|*.ly" };
                saveFileDialog.ShowDialog();
                if (saveFileDialog.SafeFileName != "")
                {
                    string targetFolderSave = Path.GetFullPath(saveFileDialog.FileName);
                    System.IO.File.WriteAllText(@targetFolderSave, _text);
                }
            }
        }

        public void LilypondToPDF(string _text)
        {
            string file = @"C:\temp\WriteText.ly";
            System.IO.File.WriteAllText(@file, _text);

            string name = System.IO.Path.GetFileName(file);
            string location = System.IO.Path.GetDirectoryName(file);
            string temp = System.IO.Path.GetFileNameWithoutExtension(file);


            string lilypondLocation = @"C:\Program Files (x86)\LilyPond\usr\bin\lilypond.exe";
            string sourceFolder = @location + "\\";
            string sourceFileName = name;
            string targetFileName = temp + ".pdf";

            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PDF Files(*.pdf)|*.pdf" };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.SafeFileName != "")
            {
                string targetFolderSave = Path.GetFullPath(saveFileDialog.FileName);

                Process process = new Process
                {
                    StartInfo =
                {
                    WorkingDirectory = sourceFolder,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("--pdf \"{0}{1}\"", sourceFolder, sourceFileName),
                    FileName = lilypondLocation,
                    Verb = "runas"
                }
                };

                process.Start();
                while (!process.HasExited)
                {
                }
                try
                {
                    File.Copy(sourceFolder + targetFileName, targetFolderSave, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
