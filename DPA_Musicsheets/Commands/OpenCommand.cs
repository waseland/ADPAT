
namespace DPA_Musicsheets.Commands
{
    public class OpenCommand : Command
    {
        private CommandTarget target;

        public OpenCommand(CommandTarget _target)
        {
            this.target = _target;
        }

        public void Execute()
        {
            target.OpenFile();
        }
    }
}
