using System;

namespace DPA_Musicsheets.Editor
{
    public class Originator
    {
        private string state;

        public void setState(string newState)
        {
            this.state = newState;
        }

        public string getState()
        {
            return state;
        }

        public Memento storeInMemento()
        {
            return new Memento(state);
        }

        public String restoreFromMemento(Memento memento)
        {
            state = memento.getState();
            return state;
        }


    }
}
