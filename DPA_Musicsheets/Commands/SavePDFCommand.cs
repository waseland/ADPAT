using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public class SavePdfCommand : Command
    {
        private CommandTarget target;

        public SavePdfCommand(CommandTarget _target)
        {
            this.target = _target;
        }

        public void execute()
        {
            target.SaveFileToPdf();
        }
    }
}
