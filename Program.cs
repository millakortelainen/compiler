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
                    if (s == ':')
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 1;
                    } 
                    else if (s != ' ')
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
                        state = 4;
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
                    if (buffer.ToString().Equals("endfor;"))
                    {
                        tokens.Add("end");
                        tokens.Add("for");
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    if (buffer.ToString().Equals("assert"))
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 4;

                    }
                    
                    break;
                case 1:
                    // consume extraspace
                    //if equal then assertion
                    if (s == ' ')
                    {
                        state = 7;
                    }
                    if (s == '=')
                    {
                        tokens.Add(":=");
                        state = 4;
                    }
                    break;
                case 2:
                    //read variable type <type>
                    if (s != ' ')
                        buffer.Append(s);

                    if (buffer.ToString().Equals("int")
                        || buffer.ToString().Equals("string")
                        || buffer.ToString().Equals("bool"))
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
                        // read expression
                        state = 4;
                    }
                    break;
                case 4:
                    //<expr> := <opnd> <op> <opnd> | [ <unary_opnd> ] <opnd>
                    //read expression

                    if (s == '+')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("+");
                        buffer.Clear();
                    }
                    else if (s == '-')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("-");
                        buffer.Clear();
                    }
                    else if (s == '*')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("*");
                        buffer.Clear();
                    }
                    else if (s == '/')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("/");
                        buffer.Clear();
                    }
                    else if (s == '>')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add(">");
                        buffer.Clear();
                    }
                    else if (s == '=')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add("=");
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
                    else if (s == '!')
                    {
                        tokens.Add("!");
                        buffer.Clear();
                    }
                    else if(s=='\"'){
                        tokens.Add("\"");
                        state = 5;
                    }
                    else if (s == ';')
                    {
                        if (buffer.Length > 0)
                            tokens.Add(buffer.ToString());
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    else if (s != ' ')
                        buffer.Append(s);
                    if (buffer.ToString().Length >= 2)
                    {  
                        //Console.WriteLine(buffer.ToString().Substring(buffer.ToString().Length-2,buffer.ToString().Length-1));
                        if (buffer.ToString().Substring(buffer.ToString().Length-2, 2).Equals(".."))
                        {
                            if (buffer.ToString().Substring(0, buffer.ToString().Length - 2).Length > 0)
                            {
                                tokens.Add(buffer.ToString().Substring(0, buffer.ToString().Length - 2));
                            }
                            tokens.Add(buffer.ToString().Substring(buffer.ToString().Length-2, 2));
                            buffer.Clear();
                        } else if (buffer.ToString().Substring(buffer.ToString().Length-2, 2).Equals("do"))
                        {
                            if (buffer.ToString().Substring(0, buffer.ToString().Length - 2).Length > 0)
                            {
                                tokens.Add(buffer.ToString().Substring(0, buffer.ToString().Length - 2));
                            }
                            tokens.Add(buffer.ToString().Substring(buffer.ToString().Length-2, 2));
                            buffer.Clear();
                            state=0;
                        }
                    }
                    

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
                case 7:
                    //read variable  <var_ident>
                    if (s == ';')
                    {
                        tokens.Add(buffer.ToString());
                        tokens.Add(";");
                        buffer.Clear();
                        state = 0;
                    }
                    /*
                    else if (s == '"')
                    {
                        tokens.Add("\"");
                        state = 5;
                    }*/
                    //space end variable name, can you also end it with ':'?
                    if (s == ' ')
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        //state 8 check what comes after the variable
                        state = 8;
                    }
                    /*
                    else if (s == '.')
                    {
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        buffer.Append(".");
                        state = 9;
                    }
                    */
                    else
                        buffer.Append(s);
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
                        //state 2 checks type
                        state = 2;
                    }
                    else if (buffer.ToString().Equals("in"))
                    {
                        tokens.Add("in");
                        buffer.Clear();
                        state = 4;
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
                case 9:
                    if (s == '.')
                    {
                        buffer.Append(".");
                        tokens.Add(buffer.ToString());
                        buffer.Clear();
                        state = 7;
                    }
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
        //Scanner.ScanString("var nTimes : int := !\"asdasd\";");
        //Scanner.ScanString("var X : int := 4 + (6 * 2);");
        //Scanner.ScanString("muuttuja := \"leet\";");
        //Scanner.ScanString("read nTimes;");
        //Scanner.ScanString("read nTimes;var x : int;");
        //Scanner.ScanString("print x;");
        //Scanner.ScanString("for x in 1+1..nTimes-1 do print x; print \" : Hello, World!\\n\"; end for;");
        Scanner.ScanString("assert ((x = nTimes));");
        return -1;
    }
}
