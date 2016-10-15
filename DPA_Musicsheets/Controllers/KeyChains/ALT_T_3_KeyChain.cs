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
    public class ALT_T_3_KeyChain : KeyChain
    {
        public ALT_T_3_KeyChain(CommandTarget _target) : base(_target)
        {
        }

        protected override void addCommands(CommandTarget _target)
        {
            commands.Add(new Add34TimeCommand(_target));
        }

        protected override void addKeys()
        {
            keys.Add(Key.LeftAlt);
            keys.Add(Key.T);
            keys.Add(Key.D3);
        }
    }
}
