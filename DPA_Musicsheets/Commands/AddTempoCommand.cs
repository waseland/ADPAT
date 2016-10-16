using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class AddTempoCommand : Command
    {
        private CommandTarget target;

        public AddTempoCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void Execute()
        {
            string newText = "\\tempo 4=120";
            target.AddTekstAtSelection(newText);
        }
    }
}
