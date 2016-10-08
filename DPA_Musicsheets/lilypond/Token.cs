using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    public class Token
    {
        public TokenType type { get; set; }
        public string value { get; set; }

        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
