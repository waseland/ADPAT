using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controller;
using System.Windows.Input;

namespace DPA_Musicsheets.Controllers.KeyChains
{
    public class CTRL_S_P_KeyChain : KeyChain
    {
        public CTRL_S_P_KeyChain(CommandTarget _target) : base(_target)
        {
        }

        protected override void addCommands(CommandTarget _target)
        {
            commands.Add(new SavePdfCommand(_target));
        }

        protected override void addKeys()
        {
            keys.Add(Key.LeftCtrl);
            keys.Add(Key.S);
            keys.Add(Key.P);
        }
    }
}
