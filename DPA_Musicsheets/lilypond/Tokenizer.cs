using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    public class Tokenizer
    {
        private LinkedList<Token> tokens;
        private String[] inputList;
        private Dictionary<String, TokenType> keyWords;


        public Tokenizer()
        {
            tokens = new LinkedList<Token>();
            keyWords = new Dictionary<string, TokenType>();
            keyWords.Add("\\relative", TokenType.relative);
            keyWords.Add("{", TokenType.Startblok);
            keyWords.Add("}", TokenType.EndBlok);
            keyWords.Add("\\time", TokenType.timeSignature);
            keyWords.Add("\\clef", TokenType.Clef);
            keyWords.Add("treble", TokenType.ClefType);
            keyWords.Add("|", TokenType.Maatstreep);
            keyWords.Add("\\tempo", TokenType.Tempo);
            keyWords.Add("\\version", TokenType.Version);
            keyWords.Add("\\header", TokenType.Header);
            keyWords.Add("\\repeat", TokenType.Repeat);
            keyWords.Add("volta", TokenType.Nothing); // TODO: nog geen idee wat die doet
        }

        public void proces(String music)
        {
            music = music.Replace("\r\n", string.Empty);
            inputList = music.Split(' ');
            for (int i = 0; i < inputList.Length; i++)
            {
                String input = inputList[i];
                if (keyWords.ContainsKey(input))
                {
                    setKeyWord(input);
                }
                else
                {
                    //todo chain of responsebilitie toevoegen inplaats van if else
                    if (input.Contains("/"))
                    {
                        Token token = new Token(TokenType.timeSignaturedata, input);
                        tokens.AddLast(token);
                    }
                    else if (isNumber(input))
                    {
                        Token token = new Token(TokenType.Number, input);
                        tokens.AddLast(token);
                    }
                    else if (input.Contains("r"))
                    {
                        Token token = new Token(TokenType.Rust, input);
                        tokens.AddLast(token);
                    }
                    else if (input.Contains("="))
                    {
                        Token token = new Token(TokenType.TempoValue, input);
                        tokens.AddLast(token);
                    }
                    else if (input.Length > 0 && !input.Contains("\""))
                    {
                        Token token = new Token(TokenType.Note, input);
                        tokens.AddLast(token);
                    }
                }
            }
        }

        public LinkedList<Token> getTokens()
        {
            return tokens;
        }

        private Boolean isNumber(String input)
        {
            int n;
            return int.TryParse(input, out n);
        }

        private void setKeyWord(String keyword)
        {
            Token token = new Token(keyWords[keyword], keyword);
            tokens.AddLast(token);
        }
    }
}
