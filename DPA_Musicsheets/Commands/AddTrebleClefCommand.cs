using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class AddTrebleClefCommand : Command
    {
        private CommandTarget target;

        public AddTrebleClefCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void execute()
        {
            string newText = "\\clef treble";
            target.AddTekstAtSelection(newText);
        }
    }
}
