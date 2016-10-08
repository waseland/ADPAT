using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    class RelativeExpresion : Expresion
    {
        private Dictionary<char, int> startWaarde;

        public RelativeExpresion()
        {
            startWaarde = new Dictionary<char, int>();
            startWaarde.Add('c', 4);
        }

        public void evaluat(LinkedListNode<Tokenizer.Token> token, Context context)
        {
            if (token.Next.Value.type == Tokenizer.TokenType.Note)
            {
                string note = token.Next.Value.value;
                context.musicSheet.startOctaaf = startWaarde[note[0]];
                context["relative"] = true;
            }
        }

        public Expresion clone()
        {
            return new RelativeExpresion();
        }
    }
}
