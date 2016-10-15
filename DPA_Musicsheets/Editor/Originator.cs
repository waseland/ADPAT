﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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