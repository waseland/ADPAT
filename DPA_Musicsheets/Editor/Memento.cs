using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Editor
{
    public class Memento
    {
        private string state;

        public Memento(string newState)
        {
            this.state = newState;
        }
        public string getState()
        {
            return state;
        }
    }
}
