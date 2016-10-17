
namespace DPA_Musicsheets.Commands
{
    public class Add68TimeCommand : Command
    {
        private CommandTarget target;

        public Add68TimeCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void Execute()
        {
            string newText = "\\time 6/8";
            target.AddTekstAtSelection(newText);
        }
    }
}
