using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controllers.KeyChains;
using DPA_Musicsheets.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Controller
{
    public class ADPKeyHandler
    {
        private KeyChain firstKeyChain;
        private CommandTarget target;
        private Updater updater;

        public ADPKeyHandler(CommandTarget _target)
        {
            target = _target;
            updater = new Updater(target);
            initializeKeyChains();
        }

        public void OnKeyPressed()
        {

            //updater.update();

            updater.StopThread();
            updater.StartThread();

            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Is Alt key pressed
            {
                foreach (Key k in getPressedKeys())
                {
                    Console.Write(k + " - ");
                }
                Console.WriteLine("____________________________________");
                firstKeyChain.Handle(getPressedKeys());
            } else
            {
                target.SetNewState();
            }
        }

        private List<Key> getPressedKeys()
        {
            List<Key> pressedKeys = new List<Key>();
            //alt and control
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) // Is ALT key pressed
            {
                pressedKeys.Add(Key.LeftAlt);
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Is CTRL key pressed
            {
                pressedKeys.Add(Key.LeftCtrl);
            }

            // normal characters
            if (Keyboard.IsKeyDown(Key.P))
            {
                pressedKeys.Add(Key.P);
            }
            if (Keyboard.IsKeyDown(Key.B))
            {
                pressedKeys.Add(Key.B);
            }
            if (Keyboard.IsKeyDown(Key.C))
            {
                pressedKeys.Add(Key.C);
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                pressedKeys.Add(Key.S);
            }
            if (Keyboard.IsKeyDown(Key.T))
            {
                pressedKeys.Add(Key.T);
            }
            if (Keyboard.IsKeyDown(Key.O))
            {
                pressedKeys.Add(Key.O);
            }

            // numbers
            if (Keyboard.IsKeyDown(Key.D3))
            {
                pressedKeys.Add(Key.D3);
            }
            if (Keyboard.IsKeyDown(Key.D4))
            {
                pressedKeys.Add(Key.D4);
            }
            if (Keyboard.IsKeyDown(Key.D6))
            {
                pressedKeys.Add(Key.D6);
            }

            return pressedKeys;
        }

        private void initializeKeyChains()
        {
            KeyChain alt_b = new ALT_B_KeyChain(target);
            KeyChain alt_c = new ALT_C_KeyChain(target);
            KeyChain alt_s = new ALT_S_KeyChain(target);
            KeyChain alt_t_3 = new ALT_T_3_KeyChain(target);
            KeyChain alt_t_4 = new ALT_T_4_KeyChain(target);
            KeyChain alt_t_6 = new ALT_T_6_KeyChain(target);
            KeyChain alt_t = new ALT_T_KeyChain(target);
            KeyChain ctrl_o = new CTRL_O_KeyChain(target);
            KeyChain ctrl_s = new CTRL_S_KeyChain(target);
            KeyChain ctrl_s_p = new CTRL_S_P_KeyChain(target);

            ctrl_s.setNextKeyChain(ctrl_s_p);
            ctrl_o.setNextKeyChain(ctrl_s);
            alt_t.setNextKeyChain(ctrl_o);
            alt_t_6.setNextKeyChain(alt_t);
            alt_t_4.setNextKeyChain(alt_t_6);
            alt_t_3.setNextKeyChain(alt_t_4);
            alt_s.setNextKeyChain(alt_t_3);
            alt_c.setNextKeyChain(alt_s);
            alt_b.setNextKeyChain(alt_c);

            firstKeyChain = alt_b;
        }
    }
}
