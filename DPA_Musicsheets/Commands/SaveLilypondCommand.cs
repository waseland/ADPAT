
namespace DPA_Musicsheets.Commands
{
    public class SaveLilypondCommand : Command
    {
        private CommandTarget target;

        public SaveLilypondCommand(CommandTarget _target)
        {
            this.target = _target;
        }

        public void Execute()
        {
            target.SaveFileToLilypond();
        }
    }
}
