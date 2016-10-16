using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Controllers.KeyChains
{
    class CTRL_S_KeyChain : KeyChain
    {
        public CTRL_S_KeyChain(CommandTarget _target) : base(_target)
        {
        }

        protected override void addCommands(CommandTarget _target)
        {
            commands.Add(new SaveLilypondCommand(_target));
        }

        protected override void addKeys()
        {
            keys.Add(Key.LeftCtrl);
            keys.Add(Key.S);
        }
    }
}
