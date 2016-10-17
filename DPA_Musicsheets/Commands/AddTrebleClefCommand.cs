
namespace DPA_Musicsheets.Commands
{
    public class AddTrebleClefCommand : Command
    {
        private CommandTarget target;

        public AddTrebleClefCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void Execute()
        {
            string newText = "\\clef treble";
            target.AddTekstAtSelection(newText);
        }
    }
}
