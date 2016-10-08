using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    class RestExpression : Expresion
    {
        public Expresion clone()
        {
            return new RustExpression();
        }

        public void evaluate(LinkedListNode<Token> token, Context context)
        {
            MyMusicalSymbol rest = new MyMusicalSymbol(0, 0);
            rest.IsPause = true;
            String input = token.Value.value;
            note.octaaf = context.musicSheet.startOctaaf;
            note.duur = Convert.ToInt16(input.Substring(1));
            context.musicSheet.addmusicSymbol(note);
        }
    }
}
