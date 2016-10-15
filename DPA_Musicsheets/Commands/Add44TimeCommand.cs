using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class Add44TimeCommand : Command
    {
        private CommandTarget target;

        public Add44TimeCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void execute()
        {
            string newText = "\\time 4/4";
            target.AddTekstAtSelection(newText);
        }
    }
}
