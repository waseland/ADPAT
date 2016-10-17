
namespace DPA_Musicsheets.Commands
{
    public class SavePdfCommand : Command
    {
        private CommandTarget target;

        public SavePdfCommand(CommandTarget _target)
        {
            this.target = _target;
        }

        public void Execute()
        {
            target.SaveFileToPdf();
        }
    }
}
