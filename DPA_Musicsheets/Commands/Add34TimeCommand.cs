using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class Add34TimeCommand : Command
    {
        private CommandTarget target;

        public Add34TimeCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void Execute()
        {
            string newText = "\\time 3/4";
            target.AddTekstAtSelection(newText);
        }
    }
}
