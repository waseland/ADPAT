
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
            target.AddBarlinesToEditor();
        }

    }
}
