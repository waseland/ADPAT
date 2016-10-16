using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Lily
{
    class SaveAsLilypond
    {
        public SaveAsLilypond(string text)
        {
            if (text == "")
            {

            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Ly Files(*.ly)|*.ly" };
                saveFileDialog.ShowDialog();
                if (saveFileDialog.SafeFileName != "")
                {
                    string targetFolderSave = Path.GetFullPath(saveFileDialog.FileName);
                    System.IO.File.WriteAllText(@targetFolderSave, text);
                }
            }
        }
    }
}
