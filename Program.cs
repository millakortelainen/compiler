//input is a text file
//
//run the commands on C#
using System;
using System.Text;

public class Scanner
{
    //Make tokens out of the input string
    public static void ScanString(string x)
    {
        var reserved_keywords = new List<string> {"var",
        "for", "end", "in", "do", "read", "print", "int",
        "string", "bool", "assert"};
        var tokens = new List<string> { };
        //string buffer = "";
        StringBuilder buffer = new StringBuilder("", 50);
        int state = 0;
        foreach (var s in x)
        {
            switch (state)
            {
                case 0:
                    //start state
                    if (s != ' ')
                        buffer.Append(s);
                    if (buffer.ToString().Equals("var"))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 1;
                    }
                    if (buffer.ToString().Equals("print"))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 1;
                    }
                    if (buffer.ToString().Equals("read"))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 1;
                    }
                    if (buffer.ToString().Equals("for"))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 1;
                    }
                    break;
                case 1:
                    // consume extraspace
                    if (s == ' ')
                    {
                        state = 7;
                    }
                    break;
                case 7:
                    //read variable
                    if (s == ';')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    else if (s == '"')
                    {
                        tokens.Add("\"");
                        state = 5;
                    }
                    else if (s == ' ')
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 8;
                    }
                    else if (s == '.')
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        buffer.Append(".");
                        state = 9;
                    }
                    else
                        buffer.Append(s);
                    break;
                case 9:
                    if (s == '.')
                    {
                        buffer.Append(".");
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state=7;
                    }
                    break;
                case 8:
                // check next after variable
                    
                    if (s == ';')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    else if (s == ':')
                    {
                        tokens.Add(":");
                        buffer.Clear();
                        state = 2;
                    }
                    else if (buffer.ToString().Equals("in"))
                    {
                        tokens.Add("in");
                        buffer.Clear();
                        state = 7;
                    }
                    else if (buffer.ToString().Equals("do"))
                    {
                        tokens.Add("do");
                        buffer.Clear();
                        state = 0;
                    }
                    else if (s != ' ')
                        buffer.Append(s);
                    break;
                case 2:
                    //read variable type
                    if (s != ' ')
                        buffer.Append(s);

                    if (buffer.ToString().Equals("int"))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 3;
                    }
                    break;
                case 3:
                    //check if assertion or declaration
                    if (s == ';')
                    {
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    else if (s != ' ')
                    {
                        buffer.Append(s);
                    }
                    if (buffer.ToString().Equals(":="))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 4;
                    }
                    break;
                case 4:
                    //read integer or math expression
                    if (s == '+')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("+");
                        buffer.Clear();
                    }
                    else if (s == '*')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("*");
                        buffer.Clear();
                    }
                    else if (s == ')')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add(")");
                        buffer.Clear();
                    }
                    else if (s == '(')
                    {
                        tokens.Add("(");
                        buffer.Clear();
                    }
                    else if (s == ';')
                    {
                        if (buffer.Length >0)
                            tokens.Add(buffer.ToString());
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    else if (s != ' ')
                        buffer.Append(s);
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
                    if ( s == ';')
                    {
                        tokens.Add(";");
                        state = 0;
                    } else if (s == '+')
                    {
                        tokens.Add("+");
                        state = 1;
                    }
                    break;
                default:
                    break;

            }

        }

        //Console.WriteLine(s);
        Console.WriteLine("content of buffer:");
        Console.WriteLine(buffer);
        Console.WriteLine("Content of tokens");
        foreach (var t in tokens)
        {
            Console.WriteLine(t);
        }
    }
}

class MainClass
{
    static int Main(string[] args)
    {
        //Scanner.ScanString("var nTimes : int := 0;");
        //Scanner.ScanString("var X : int := 4 + (6 * 2);");
        //Scanner.ScanString("read nTimes;var x : int;");
        Scanner.ScanString("for x in 0..nTimes-1 do print x; print \" : Hello, World!\\n\";");
        return -1;
    }
}
