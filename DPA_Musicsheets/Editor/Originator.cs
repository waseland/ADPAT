using System;

namespace DPA_Musicsheets.Editor
{
    public class Originator
    {
        private string state;

        public void SetState(string _newState)
        {
            this.state = _newState;
        }

        public string GetState()
        {
            return state;
        }

        public Memento StoreInMemento()
        {
            return new Memento(state);
        }

        public String RestoreFromMemento(Memento _memento)
        {
            state = _memento.GetState();
            return state;
        }


    }
}
