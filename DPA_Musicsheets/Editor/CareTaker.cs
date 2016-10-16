using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Editor
{
    public class CareTaker
    {
        private List<Memento> mementoList;

        public CareTaker()
        {
            mementoList = new List<Memento>();
        }

        public void add(Memento newMemento)
        {
            mementoList.Add(newMemento);
        }
        public Memento get(int index)
        {
            return mementoList[index];
        }
    }
}
