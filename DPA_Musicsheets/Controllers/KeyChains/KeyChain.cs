using DPA_Musicsheets.Commands;
using System.Collections.Generic;
using System.Windows.Input;

namespace DPA_Musicsheets.Controller
{
    public abstract class KeyChain
    {
        private KeyChain nextKeyChain;
        protected List<Command> commands;
        protected List<Key> keys;

        public KeyChain(CommandTarget _target)
        {
            commands = new List<Command>();
            addCommands(_target);
            keys = new List<Key>();
            addKeys();
        }

        public void SetNextKeyChain(KeyChain _nextKeyChain)
        {
            nextKeyChain = _nextKeyChain;
        }

        protected abstract void addKeys();

        protected abstract void addCommands(CommandTarget _target);

        private bool matchesRequirements(List<Key> _givenKeys)
        {
            if(_givenKeys.Count == keys.Count)
            {
                foreach(Key k in keys){
                    if (!_givenKeys.Contains(k))
                    {
                        return false;
                    }
                }

                return true;
            } else
            {
                return false;
            }
        }

        public void Handle(List<Key> _givenKeys)
        {
            if (matchesRequirements(_givenKeys))
            {
                foreach (Command c in commands)
                {
                    c.Execute();
                }
            } else
            {
                if (nextKeyChain != null)
                {
                    nextKeyChain.Handle(_givenKeys);
                }
            }
        }
    }
}
