using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    public class ExpressionFactory
    {
        static Dictionary<TokenType, Expresion> expressions;
        static ExpressionFactory()
        {
            expressions = new Dictionary<TokenType, Expresion>();
            expressions.Add(TokenType.Note, new NootExpresion());
            expressions.Add(TokenType.relative, new RelativeExpresion());
            expressions.Add(TokenType.timeSignaturedata, new TimeSignatureExpresion());
            expressions.Add(TokenType.Maatstreep, new MaatExpresion());
            expressions.Add(TokenType.Rust, new RustExpression());
            expressions.Add(TokenType.TempoValue, new TempoExpresioncs());
            expressions.Add(TokenType.EndBlok, new EndblokExpresion());
            expressions.Add(TokenType.Repeat, new RepeatExpression());
        }

        public static Expresion getExpresionHandler(TokenType type)
        {
            if (expressions.ContainsKey(type))
            {
                return expressions[type].clone();
            }
            else
            {
                return null;
            }
        }
    }
}
