using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public interface CommandTarget
    {
        void OpenFile();
        void SaveFileToLilypond();
        void SaveFileToPdf();
        void AddTekstAtSelection(string _text);
        void SetNewState();
        void AddBarlinesToEditor();
        string UpdateBarlinesFromLilypond();
        
    }
}
