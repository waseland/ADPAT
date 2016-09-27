using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class NoteBar
    {
        private List<Note> notes;
        private int maxLength;
        private int length;

        public NoteBar(int maxLength)
        {
            this.length = 0;
            this.maxLength = maxLength;
            notes = new List<Note>();
        }

        //public Boolean addNote(Note newNote)
        //{
        //    if(this.length + newNote.Duration) > maxLength)
        //    {
        //        return false;
        //    } else
        //    {

        //    }
        //}
    }
}
