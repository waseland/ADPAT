using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    public class Context
    {
        private Dictionary<string, Boolean> _variables;
        public MusicSheet musicSheet { get; set; }

        public Context()
        {
            _variables = new Dictionary<string, Boolean>();
            musicSheet = new MusicSheet();
        }

        public Boolean this[string variableName]
        {
            get
            {
                if (_variables.ContainsKey(variableName))
                {
                    return _variables[variableName];
                }
                else
                {
                    return false;
                }
            }
            set { _variables[variableName] = value; }
        }
    }
}
