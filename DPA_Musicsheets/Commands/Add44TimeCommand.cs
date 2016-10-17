
namespace DPA_Musicsheets.Commands
{
    public class Add44TimeCommand : Command
    {
        private CommandTarget target;

        public Add44TimeCommand(CommandTarget _target)
        {
            target = _target;
        }

        public void Execute()
        {
            string newText = "\\time 4/4";
            target.AddTekstAtSelection(newText);
        }
    }
}
