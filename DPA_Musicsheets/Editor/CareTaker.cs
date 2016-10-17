using System.Collections.Generic;

namespace DPA_Musicsheets.Editor
{
    public class CareTaker
    {
        private List<Memento> mementoList;

        public CareTaker()
        {
            mementoList = new List<Memento>();
        }

        public void Add(Memento _newMemento)
        {
            mementoList.Add(_newMemento);
        }
        public Memento Get(int _index)
        {
            return mementoList[_index];
        }
    }
}
