using System;

public class Driver
{
    static void Main()
    {
        if (!File.Exists("test.lk"))
        {
            Console.WriteLine("Error: File 'test.lk' not found.");

            return;
        }

        Lexer lexer = new Lexer(File.ReadAllText("test.lk"));
        List<Token> tokens = lexer.Tokenize();

        Parser parser = new Parser(tokens);
        List<ASTNode> nodes = parser.ParseProgram();

        CodeGenerator codeGenerator = new CodeGenerator(nodes);

        string code = codeGenerator.Generate();

        File.WriteAllText("output.c", code);
    }
}