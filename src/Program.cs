//input is a text file
//
//run the commands on C#
using System;
using System.Text;

public class Scanner
{
    //check that everything is written correctly
    List<string> tokens = new List<string> { };
    //Make tokens out of the input string
    public List<string> ScanString(string inputString)
    {

        StringBuilder buffer = new StringBuilder("", 50);
        int state = 0;
        foreach (var s in inputString)
        {
            switch (state)
            {
                case 0:
                    if (s != ' ')
                        //buffer ignores spaces
                        buffer.Append(s);

                    string bufferString = buffer.ToString();

                    /*
                    Check statement type
                    <stmt> ::=
                    Exeption: for end is also recognized
                    */
                    if (bufferString.Equals("var"))
                        //“var” <var_ident> “:” <type> [ “:=” <expr> ]
                        state = StartToVarIdent(buffer);

                    if (EndOfBufferContains(buffer, ":="))
                        //<var_ident> “:=” <expr>
                        state = Expr(buffer, ":=");

                    if (bufferString.Equals("for"))
                        //“for” <var_ident> “in” <expr> “..” <expr> “do” <stmts> “end” “for”
                        state = StartToVarIdent(buffer);

                    if (bufferString.Equals("endfor;"))
                        //regocnize end of the loop 
                        state = Start(buffer, "endfor;");

                    if (buffer.Equals("print"))
                        //“print” <expr>
                        state = Expr(buffer, "print");

                    if (bufferString.Equals("read"))
                        //“read” <var_ident>
                        state = StartToVarIdent(buffer);

                    if (bufferString.Equals("assert"))
                        //“assert” “(” <expr> “)”
                        state = Expr(buffer, "assert");

                    break;
                case 1:
                    //read variable  <var_ident>
                    if (s == ';')
                        //variable name ends when the line ends
                        state = Start(buffer, ";");
                    //space end variable name, can you also end it with ':'?
                    else if (s == ' ' && buffer.ToString().Length > 0)
                        state = AfterVarIdent(buffer);
                    else if (s != ' ')
                        buffer.Append(s);
                    break;

                case 2:
                    //read variable type <type>
                    if (s == ';')
                        state = Start(buffer, ";");

                    else if (s != ' ')
                        buffer.Append(s);

                    if (EndOfBufferContains(buffer, ":="))
                        state = Expr(buffer, ":=");

                    break;
                case 4:
                    //<expr> := <opnd> <op> <opnd> | [ <unary_opnd> ] <opnd>
                    //read expression

                    if (s == '+' || s == '-' || s == '*'
                        || s == '/' || s == '>' || s == '='
                        || s == ')')
                    {
                        if (buffer.ToString().Length > 0)
                        {
                            tokens.Add(buffer.ToString());
                        }
                        tokens.Add(s.ToString());
                        buffer.Clear();
                    }
                    else if (s == '(' || s == '!')
                    {
                        tokens.Add(s.ToString());
                        buffer.Clear();
                    }
                    else if (s == '\"')
                        state = String(buffer);

                    else if (s == ';')
                        state = Start(buffer, ";");

                    else if (s != ' ')
                        buffer.Append(s);

                    if (EndOfBufferContains(buffer, ".."))
                        state = Expr(buffer, "..");

                    else if (EndOfBufferContains(buffer, "do"))
                        state = Start(buffer, "do");

                    break;
                case 5:
                    // reading a string
                    if (s == '\"')
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        tokens.Add("\"");
                        state = 6;
                    }
                    else
                        buffer.Append(s);

                    break;
                case 6:
                    // string ends or concatenaiton
                    if (s == ';')
                    {
                        tokens.Add(";");
                        state = 0;
                    }
                    else if (s == '+')
                    {
                        tokens.Add("+");
                        state = 5;
                    }
                    break;
                case 8:
                    // check next after variable
                    if (s == ';')
                        state = Start(buffer, ";");

                    else if (s == ':')
                        state = Type(buffer);

                    else if (buffer.ToString().Equals("in"))
                        state = Expr(buffer, "in");

                    else if (buffer.ToString().Equals("do"))
                        state = Start(buffer, "do");

                    else if (s != ' ')
                        buffer.Append(s);

                    break;
            }

        }
        tokens.Add("$$");
        return tokens;
    }

    public override string ToString()
    {
        string str = "";
        foreach (var t in tokens)
        {
            str += t + "\n";
        }
        return str;
    }
    public int StartToVarIdent(StringBuilder buffer)
    {
        tokens.Add(buffer.ToString());
        buffer.Clear();
        return 1;
    }

    public int AfterVarIdent(StringBuilder buffer)
    {
        tokens.Add(buffer.ToString());
        buffer.Clear();
        return 8;
    }

    public int Type(StringBuilder buffer)
    {
        tokens.Add(":");
        buffer.Clear();
        return 2;
    }

    public int String(StringBuilder buffer)
    {
        tokens.Add("\"");
        buffer.Clear();
        return 5;
    }

    public int Expr(StringBuilder buffer, string keyword)
    {
        switch (keyword)
        {
            case ":=":
                AddTokenBeforeKeyword(buffer, ":=");
                tokens.Add(":=");
                break;
            case "..":
                AddTokenBeforeKeyword(buffer, "..");
                tokens.Add("..");
                break;
            default:
                tokens.Add(keyword);
                break;
        }
        buffer.Clear();
        return 4;
    }

    public void AddTokenBeforeKeyword(StringBuilder buffer, string keyword)
    {
        int offset = keyword.Length;
        if (buffer.ToString().Substring(0, buffer.ToString().Length - offset).Length > 0)
            tokens.Add(buffer.ToString().Substring(0, buffer.ToString().Length - offset));
    }

    public int Start(StringBuilder buffer, string str)
    {
        switch (str)
        {
            case ";":
                if (buffer.Length > 0)
                    tokens.Add(buffer.ToString());
                tokens.Add(";");
                break;
            case "endfor;":
                tokens.Add("end");
                tokens.Add("for");
                tokens.Add(";");
                break;
            case "do":
                AddTokenBeforeKeyword(buffer, "do");
                tokens.Add("do");
                break;
            default:
                tokens.Add(str);
                break;
        }
        buffer.Clear();
        return 0;
    }

    public static bool EndOfBufferContains(StringBuilder buffer, string str)
    {
        string bufferString = buffer.ToString();
        int offset = str.Length;
        if (bufferString.Length >= offset)
        {
            if (bufferString.Substring(bufferString.Length - offset, offset).Equals(str))
                return true;
        }
        return false;
    }

}

public class Node
{
    List<Node> Neighbours = new List<Node> { };
    public Node(string content, int id)
    {
        Content = content;
        Id = id;
    }

    public string Content { get; }
    public int Id { get; }
    public List<Node> GetNeighbours() { return Neighbours; }

    public void AddNeighbour(Node n)
    {
        Neighbours.Add(n);
    }

    public override string ToString()
    {
        return Id + ":" + Content;
    }
}

public class Edge
{
    public Edge(Node parent, Node child, int level)
    {
        Parent = parent;
        Child = child;
        Level = level;
        Parent.AddNeighbour(Child);
    }

    public Node Parent { get; }

    public Node Child { get; }

    public int Level { get; }

    public override string ToString()
    {
        return "(" + Parent + "," + Child + ")" + Level;
    }
}

public class Tree
{
    public Tree()
    {
        Nodes = new List<Node> { };
        Edges = new List<Edge> { };
    }
    public List<Node> Nodes { get; }

    public List<Edge> Edges { get; }

    public void AddNode(Node newNode)
    {
        Nodes.Add(newNode);
    }

    public void AddEdge(Edge newEdge)
    {
        Edges.Add(newEdge);
    }

    public Node GetRoot()
    {
        foreach (var n in Nodes)
        {
            if (n.Id == 0) return n;
        }
        return null;
    }

    public override string ToString()
    {
        string str = "";
        foreach (var edge in Edges)
        {
            str += edge.ToString() + "\n";
        }
        return str;
    }
}

public class Parser
{
    // output a parse tree
    //check that everything is in correct order
    Tree ParseTree = new Tree();
    int Pointer = 0;
    int NodeCount = -1;
    List<string> Tokens = new List<string> { };
    string Token = "";

    public Parser(List<string> tokens)
    {
        Tokens = tokens;
        Token = Tokens[0];
    }
    public int Node_id()
    {
        NodeCount++;
        return NodeCount;
    }
    public Tree ParseTokens()
    {
        Node init = new Node("program", Node_id());
        ParseTree.AddNode(init);
        StmtList(init, 0);
        if (Token.Equals("$$"))
        {
            ParseTree.AddEdge(new Edge(init, new Node("$$", Node_id()), 0));
        }
        return ParseTree;
    }

    public void StmtList(Node from, int level)
    {
        //stmt stmt_list
        if (Token.Equals("$$")) return;
        if (Token.Equals("end")) return;

        Node node = new Node("stmt_list", Node_id());
        ParseTree.AddEdge(new Edge(from, node, level));

        Stmt(node, level + 1);
        StmtList(node, level + 1);
    }

    public void Stmt(Node from, int level)
    {
        if (Token.Equals("$$")) return;

        Node node = new Node("stmt", Node_id());
        ParseTree.AddEdge(new Edge(from, node, level));

        //read” <var_ident>
        if (Token.Equals("read"))
        {
            //consume the 'read'
            Match(node, level);
            //consume 'id'
            Match(node, level);
            if (Token.Equals(";"))
            {
                //EOL
                Match(node, level);
                return;
            }
        }
        //“var” <var_ident> “:” <type> [ “:=” <expr> ]
        if (Token.Equals("var"))
        {
            //consume 'var'
            Match(node, level);
            //consume id
            Match(node, level);

            if (Token.Equals(":"))
            {
                //consume ":"
                Match(node, level);

                //type
                if (Token.Equals("int") |
                Token.Equals("string") |
                Token.Equals("bool"))
                {
                    //consume type
                    Match(node, level);

                    if (Token.Equals(";"))
                    {
                        //End of line
                        Match(node, level);
                        return;
                    }
                    else if (Token.Equals(":="))
                    {
                        Match(node, level);
                        Expr(node, level);
                        if (Token.Equals(";"))
                        {
                            //End of line
                            Match(node, level);
                            return;
                        }
                    }
                }

            }
        }

        //“print” <expr>
        if (Token.Equals("print"))
        {
            Match(node, level);
            Expr(node, level);
            if (Token.Equals(";"))
            {
                //End of line
                Match(node, level);
                return;
            }
        }

        //“assert” “(” <expr> “)”
        if (Token.Equals("assert"))
        {
            //assert
            Match(node, level);
            //(
            Match(node, level);
            Expr(node, level);
            //)
            Match(node, level);
            if (Token.Equals(";"))
            {
                //End of line
                Match(node, level);
                return;
            }
        }

        //“for” <var_ident> “in” <expr> “..” <expr> “do” <stmts> “end” “for''
        if (Token.Equals("for"))
        {
            //for
            Match(node, level);
            //ID
            Match(node, level);
            //in
            Match(node, level);
            Expr(node, level);
            //..
            Match(node, level);
            Expr(node, level);
            //do
            Match(node, level);
            StmtList(node, level);
            //end
            Match(node, level);
            //for
            Match(node, level);
            if (Token.Equals(";"))
            {
                //End of line
                Match(node, level);
                return;
            }
        }

        //<var_ident> “:=” <expr>
        //var_iden
        Match(node, level);
        //:=
        Match(node, level);
        Expr(node, level);
        if (Token.Equals(";"))
        {
            //End of line
            Match(node, level);
            return;
        }
    }

    public void Opnd(Node from, int level)
    {
        if (Token.Equals(";")) return;
        if (Token.Equals("("))
        {
            //another expression
            Match(from, level);
            Expr(from, level);
            //match closing bracket
            if (Token.Equals(")"))
            {
                Match(from, level);
            }
        }
        else
        {
            if (Token.Equals("!"))
            {
                //consume unary operand
                Match(from, level);
            }
            //consume the operand
            if (Token.Equals("\""))
            {
                //string //"
                Match(from, level);
                //string content
                Match(from, level);
                //string ending //''
                Match(from, level);
            }
            else
            {
                //int or var_ident
                Match(from, level);
            }
        }
    }

    public void Op(Node from, int level)
    {
        if (Token.Equals("+") ||
            Token.Equals("-") ||
            Token.Equals("*") ||
            Token.Equals("/") ||
            Token.Equals("<") ||
            Token.Equals("=") ||
            Token.Equals("&"))
        {
            Match(from, level);
        }
    }

    public void Expr(Node from, int level)
    {
        if (Token.Equals("$$")) return;
        if (Token.Equals(";")) return;

        Node node = new Node("expr", Node_id());
        ParseTree.AddEdge(new Edge(from, node, level));

        Opnd(node, level);
        Op(node, level);
        Opnd(node, level);
    }

    public void Match(Node from, int level)
    {
        ParseTree.AddEdge(new Edge(from, new Node(Token, Node_id()), level));
        Pointer = Pointer + 1;
        Token = Tokens[Pointer];
    }
}

class SematicAnalyzer
{
    //output a symbol table and a abstract symbol tree
    //check that all the variables are declared before usage
    //check that variables are defined correctly
    //No identifier is used in an inappropriate context
    Tree ParseTree;
    Tree AST = new Tree();
    Dictionary<string, string> SymbolTable = new Dictionary<string, string>();
    Node LastNode = new Node("program", 0);
    string VarType = "";
    public SematicAnalyzer(Tree parseTree)
    {
        ParseTree = parseTree;
        AST.AddNode(LastNode);
    }

    public void SematicAnalyze()
    {
        /*
        Walk tree left to right
        check node type stmt
            if var declaration update symbol table
            add stmt to AST
        
        */
        Walk(ParseTree.GetRoot());
    }

    public void Walk(Node node)
    {
        //Console.WriteLine("Im currently at:");
        //Console.WriteLine(node);
        //Console.WriteLine("last node is");
        //Console.WriteLine(LastNode);
        if (!node.GetNeighbours().Any())
        {
            return;
        } else
        {
            LastNode = node;
        }
        if (node.Content.Equals("stmt"))
        {
            ValidityOfStmt(node);
            return;
        }
        foreach (var n in node.GetNeighbours())
        {
            Walk(n);
        }
    }

    public Boolean ValidityOfStmt(Node node)
    {
        Console.WriteLine("length of stmt");
        Console.WriteLine(node.GetNeighbours().Count());
        if (node.GetNeighbours()[0].Content.Equals("var"))
        {
            return ValidityOfVariableDeclaration(node) && node.GetNeighbours().Last().Content.Equals(";");
        }
        return true;
    }

    public Boolean ValidityOfVariableDeclaration(Node node)
    {
        SymbolTable.Add(node.GetNeighbours()[1].Content, node.GetNeighbours()[3].Content);
        return true;
    }

    public Tree GetAST() { return ParseTree; }
    public Dictionary<string, string> GetSymbolTable() { return SymbolTable; }

}

class Compile
{
    //input a symbol table and AST
    //run code & make intermediate representation
    Tree AST;
    Dictionary<string, string> SymbolTable;
    Dictionary<string, object> Values = new Dictionary<string, object> { };
    List<string> Stmt = new List<string> { };

    public Compile(Tree ast, Dictionary<string, string> symbolTable)
    {
        AST = ast;
        SymbolTable = symbolTable;
    }

    public void Execute()
    {
        Walk(AST.GetRoot());

    }

    public int ResolveMathOpnd(Node opnd)
    {
        int number;
        if (!int.TryParse(opnd.Content, out number))
        {
            number = (int)Values[opnd.Content];

        }
        return number;
    }

    public int ResolveMathExpr(Node expr)
    {
        int number;
        int number2;
        //first is bracket
        if (expr.GetNeighbours()[0].Content.Equals("("))
        {
            number = ResolveMathExpr(expr.GetNeighbours()[1]);
            if (expr.GetNeighbours().Count > 3)
            {
                //second is bracket
                if (expr.GetNeighbours()[4].Content.Equals("("))
                {
                    number2 = ResolveMathExpr(expr.GetNeighbours()[5]);
                }
                else
                {
                    //second is not bracket
                    number2 = ResolveMathOpnd(expr.GetNeighbours()[4]);
                }
                if (expr.GetNeighbours()[3].Content.Equals("+")) return number + number2;
            }
        }
        else
        {
            //first is not bracket
            number = ResolveMathOpnd(expr.GetNeighbours()[0]);
            if (expr.GetNeighbours().Count > 3)
            {
                //second is bracket
                if (expr.GetNeighbours()[2].Content.Equals("("))
                {
                    number2 = ResolveMathExpr(expr.GetNeighbours()[3]);
                    if (expr.GetNeighbours()[1].Content.Equals("+")) return number + number2;
                }
            }
            else if (expr.GetNeighbours().Count > 1)
            {
                //second is not bracket
                number2 = ResolveMathOpnd(expr.GetNeighbours()[2]);

                if (expr.GetNeighbours()[1].Content.Equals("+")) return number + number2;
            }
        }
        return number;
    }

    public void Walk(Node node)
    {
        foreach (var n in node.GetNeighbours())
        {
            Walk(n);
        }
        if (node.Content.Equals("stmt"))
        {
            if (node.GetNeighbours().Count > 4)
            {
                if (SymbolTable[node.GetNeighbours()[1].Content].Equals("int"))
                    Values.Add(node.GetNeighbours()[1].Content, ResolveMathExpr(node.GetNeighbours()[5]));
            }
        }
        if (node.GetNeighbours().Count == 0)
        {
            Stmt.Add(node.Content);
        }

    }
}

class MainClass
{
    static int Main(string[] args)
    {
        //string str = "assert (x = nTimes);";
        //string str = "for x in 1+1..nTimes-1 do print x; print \" : Hello, World!\\n\"; end for;";
        //string str = "var nTimes : int"
        //string str = "var nTimes : int := !\"asdasd\";";
        //string str = "var X : int := 4 + (6 * 2);";
        //string str = "read nTimes;read nTimes;read nTimes;read nTimes;";
        //string str = "read nTimes;var x : int;";
        //string str = "print x;";
        //string str = "assert (x = nTimes);";
        string str = "var nTimes : int := (3+6)+id;";
        //string str = "var n : int := 2;var m : int := n+(1+1);";
        string prog1 = "var X : int := 4; print X;";
        //example programs
        //string prog1 = "var X : int := 4 + (6 * 2); print X;";
        string prog2 = "var nTimes : int = 0; print \"How many times?\"; read nTimes; var x : int; for x in 0..nTimes-1 do    print x;    print \" : Hello, World!\n\" ; end for; assert (x = nTimes);";
        string prog3 = "print \"Give a number \" var n : int; read n; var v : int := 1; var i : int; for i in i..n do    v := v * i; end for; print \"The result is: \" ; print v;";
        
        Scanner scanner = new Scanner();
        Parser parser = new Parser(scanner.ScanString(prog1));
        SematicAnalyzer SA = new SematicAnalyzer(parser.ParseTokens());
        SA.SematicAnalyze();
        //SematicAnalyzer SA = new SematicAnalyzer(parser.ParseTokens());
        //SA.SematicAnalyze();
        //Compile compiler = new Compile(SA.GetAST(), SA.GetSymbolTable());
        //compiler.Execute();
        return -1;
    }
}
