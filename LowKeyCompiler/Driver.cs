public class Driver
{
    static void Main()
    {
        if (!File.Exists("test.lk"))
        {
            Console.WriteLine("Compiler Error: No .lk file found.");
            return;
        }

        Lexer lexer = new Lexer(File.ReadAllText("test.lk"));
        List<Token> Tokens = lexer.Tokenizer();

        Parser parser = new Parser(Tokens);
        List<ASTNode> Nodes = parser.ParseProgram();

        CodeGenerator codeGenerator = new CodeGenerator(Nodes);
        string code = codeGenerator.Generate();

        File.WriteAllText("output.c", code);
    }
}