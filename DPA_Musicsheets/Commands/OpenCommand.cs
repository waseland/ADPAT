using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class OpenCommand : Command
    {
        private CommandTarget target;

        public OpenCommand(CommandTarget _target)
        {
            this.target = _target;
        }

        public void execute()
        {
            target.OpenFile();
        }
    }
}
