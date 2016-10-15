using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class AddBarlinesCommand : Command
    {
        private CommandTarget target;

        public AddBarlinesCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void Execute()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
