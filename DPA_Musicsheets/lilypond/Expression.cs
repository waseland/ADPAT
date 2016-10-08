using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    public interface Expresion
    {
        void evaluate(LinkedListNode<Token> token, Context context);

        Expresion clone();
    }
}
