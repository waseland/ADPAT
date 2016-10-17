using DPA_Musicsheets.Commands;
using System.Windows.Input;
using DPA_Musicsheets.Controller;

namespace DPA_Musicsheets.Controllers.KeyChains
{
    public class ALT_T_KeyChain : KeyChain
    {
        public ALT_T_KeyChain(CommandTarget _target) : base(_target)
        {
        }

        protected override void addCommands(CommandTarget _target)
        {
            commands.Add(new Add44TimeCommand(_target));
        }

        protected override void addKeys()
        {
            keys.Add(Key.LeftAlt);
            keys.Add(Key.T);
        }
    }
}
