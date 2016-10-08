using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    public class LilypondInterpreter
    {
        public MusicSheet proces(LinkedList<Token> tokens)
        {
            Context context = new Context();
            LinkedListNode<Token> currentToken = tokens.First;
            while (currentToken != null)
            {
                Expresion handler = ExpressionFactory.getExpresionHandler(currentToken.Value.type);
                if (handler != null)
                {
                    handler.evaluate(currentToken, context);

                }
                currentToken = currentToken.Next;
            }
            return context.musicSheet;
        }
    }
}
