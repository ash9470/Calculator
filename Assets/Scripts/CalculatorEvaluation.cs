using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

public static class CalculatorEvaluation
{
    public static (bool success, string result) Calculate(string Value)
    {
        try
        {
            var token = Tokenize(Value);
            var postfix = ToPostfix(token);
            var value = SolvePostfix(postfix);
            return (true, value.ToString(CultureInfo.InvariantCulture));
        }
        catch (DivideByZeroException)
        {
            return (false, "Error: Division by zero");
        }

        catch (Exception e)
        {
            return (false, "Error:" + e.Message);
        }
    }

    private static List<string> Tokenize(string expr)
    {
        var result = new List<string>();
        int i = 0;

        while (i < expr.Length)
        {
            char c = expr[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // Numbers 
            if (char.IsDigit(c) || c == '.')
            {
                string number = "";
                while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                {
                    number += expr[i];
                    i++;
                }
                result.Add(number);
                continue;
            }

            // Operators and parentheses
            if ("+-*/()".Contains(c))
            {
                result.Add(c.ToString());
                i++;
                continue;
            }

            throw new Exception($"Unexpected character '{c}'");
        }

        return result;
    }

    // Operator precedence 
    private static int Precedence(string op)
    {
        if (op == "*" || op == "/") return 2;
        if (op == "+" || op == "-") return 1;
        return 0;
    }

    // Convert infix tokens into postfix
    private static List<string> ToPostfix(List<string> tokens)
    {
        var output = new List<string>();
        var stack = new Stack<string>();

        foreach (var t in tokens)
        {
            if (decimal.TryParse(t, out _))
            {
                // number straight to output
                output.Add(t);
            }
            else if ("+-*/".Contains(t))
            {
                while (stack.Count > 0 && "+-*/".Contains(stack.Peek()) && Precedence(stack.Peek()) >= Precedence(t))
                {
                    output.Add(stack.Pop());
                }
                stack.Push(t);
            }
        }

        // Remove remaining operators
        while (stack.Count > 0)
        {
            var op = stack.Pop();
            if (op == "(") throw new Exception("Mismatched parentheses");
            output.Add(op);
        }

        return output;
    }

    //  postfix expression
    private static decimal SolvePostfix(List<string> postfix)
    {
        var stack = new Stack<decimal>();

        foreach (var token in postfix)
        {
            if (decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal num))
            {
                stack.Push(num);
            }
            else
            {
                if (stack.Count < 2) throw new Exception("Invalid expression");

                var b = stack.Pop();
                var a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/":
                        if (b == 0) throw new DivideByZeroException();
                        stack.Push(a / b);
                        break;
                    default: throw new Exception("Unknown operator " + token);
                }
            }
        }

        if (stack.Count != 1) throw new Exception("Invalid expression");

        return stack.Pop();
    }
}
