using DPA_Musicsheets.Commands;
using System.Windows.Input;
using DPA_Musicsheets.Controller;

namespace DPA_Musicsheets.Controllers.KeyChains
{
    public class ALT_S_KeyChain : KeyChain
    {
        public ALT_S_KeyChain(CommandTarget _target) : base(_target)
        {
        }

        protected override void addCommands(CommandTarget _target)
        {
            commands.Add(new AddTempoCommand(_target));
        }

        protected override void addKeys()
        {
            keys.Add(Key.LeftAlt);
            keys.Add(Key.S);
        }
    }
}
