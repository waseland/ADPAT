using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Commands;
using System.Windows.Input;
using DPA_Musicsheets.Controller;

namespace DPA_Musicsheets.Controllers.KeyChains
{
    public class ALT_C_KeyChain : KeyChain
    {
        public ALT_C_KeyChain(CommandTarget _target) : base(_target)
        {
        }

        protected override void addCommands(CommandTarget _target)
        {
            commands.Add(new AddTrebleClefCommand(_target));
        }

        protected override void addKeys()
        {
            keys.Add(Key.LeftAlt);
            keys.Add(Key.C);
        }
    }
}
