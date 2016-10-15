using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class SaveLilypondCommand : Command
    {
        private CommandTarget target;

        public SaveLilypondCommand(CommandTarget _target)
        {
            this.target = _target;
        }

        public void execute()
        {
            target.SaveFileToLilypond();
        }
    }
}
