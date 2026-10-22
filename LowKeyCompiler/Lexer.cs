// The Lexer main job is to read the entire file, character by character, and convert what it reads to tokens
// See Token.cs to see how a token is structured

public class Lexer
{
    private readonly string _source;
    private int _position = 0;

    private char[] operators =
    {
      '=',
      '+',
      '-',
      '*',
      '/',
      '<',
      '>',
    };

    // Constructor, gets the string to analyze (entire file)
    public Lexer(string source)
    {
        _source = source;
    }

    // Peeks the current character, returns a null character if _position has reached the end of _source
    private char Peek()
    {
        if (_position >= _source.Length)
        {
            return '\0';
        }

        return _source[_position];
    }

    // Gets the current character and advances
    private char Advance()
    {   
        char c = _source[_position];

        _position++;

        return c;
    }

    // Gets all the adjacent numbers, commas and periods (for non-integer numbers)
    private string ReadNumber()
    {
        int _start = _position;

        while (_position < _source.Length && (char.IsDigit(Peek()) || Peek() == ',' || Peek() == '.'))
        {
            Advance();
        }

        return _source.Substring(_start, _position - _start);
    }

    // Reads the adjacent characters to get a complete word
    private string ReadWord()
    {
        int _start = _position;

        while (_position < _source.Length && char.IsLetterOrDigit(Peek()))
        {
            Advance();
        }

        return _source.Substring(_start, _position - _start);
    }

    // Reads everything that is inside ""
    private string ReadString()
    {
        int _start = _position;

        while (_position < _source.Length && Peek() != '"')
        {
            Advance();
        }

        return _source.Substring(_start, _position - _start);
    }

    private string ReadOperators()
    {
        int _start = _position;

        while (_position < _source.Length && operators.Contains(Peek()))
        {
            Advance();
        }

        return _source.Substring(_start, _position - _start);
    }

    // Defines if the given string is a keyword or an identifier
    private TokenType KeywordOrIdentifier(string text)
    {
        return text switch
        {
            "int" or "string" or "decimal" or "if" or "do" or "else" or "end" or "print" or "while" => TokenType.Keyword,
            _ => TokenType.Identifier
        };
    }

    // The main method that converts everything to tokens
    public List<Token> Tokenizer()
    {
        List<Token> Tokens = new List<Token>();

        while (_position < _source.Length)
        {
            char currentChar = Peek();

            // If the char is an empty space we skip
            if (char.IsWhiteSpace(currentChar))
            {
                Advance();

                continue;
            }

            // If its a digit we call ReadNumber()
            if (char.IsDigit(currentChar))
            {
                string number = ReadNumber();

                Tokens.Add(new Token(TokenType.Number, number));

                continue;
            }

            // If its a word we call ReadWord()
            if (char.IsLetter(currentChar))
            {
                string word = ReadWord();

                TokenType type = KeywordOrIdentifier(word);

                Tokens.Add(new Token(type, word));

                continue;
            }

            // If its " we call ReadString() and read everything until the next "
            if (currentChar == '"')
            {
                Advance();

                string str = ReadString();

                Tokens.Add(new Token(TokenType.String, str));

                Advance();

                continue;
            }

            // Checks if the current character is an operator and get the adjacent operators
            if (operators.Contains(currentChar))
            {
                string ops = ReadOperators();

                Tokens.Add(new Token(TokenType.Operator, ops));

                continue;
            }

            // Checks if the current character is a round bracket
            if (currentChar == '(' || currentChar == ')')
            {
                Tokens.Add(new Token(TokenType.RoundBracket, $"{currentChar}"));

                Advance();

                continue;
            }

            // Checks if the current character is a semicolon
            if (currentChar == ';')
            {
                Tokens.Add(new Token(TokenType.Semicolon, ";"));

                Advance();

                continue;
            }
        }

        Tokens.Add(new Token(TokenType.EOF, ""));

        return Tokens;
    }
}