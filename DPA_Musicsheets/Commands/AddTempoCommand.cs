
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
