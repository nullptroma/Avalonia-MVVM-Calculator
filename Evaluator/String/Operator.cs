namespace Evaluator.String;

class Operator : IOperator
{
    private string StringOperator { get; set; } = "";
    private int PriorityLevel { get; set; } = -1;
    private IOperator? OperatorLeft { get; set; }
    private IOperator? OperatorRight { get; set; }

    private const int MaxOperatorLength = 4;

    public double GetValue()
    {
        if (OperatorLeft != null && OperatorRight != null) // два операнда
        {
            return StringOperator switch
            {
                "-" => OperatorLeft.GetValue() - OperatorRight.GetValue(),
                "+" => OperatorLeft.GetValue() + OperatorRight.GetValue(),
                "*" => OperatorLeft.GetValue() * OperatorRight.GetValue(),
                "/" => OperatorLeft.GetValue() / OperatorRight.GetValue(),
                "mod" => OperatorLeft.GetValue() % OperatorRight.GetValue(),
                "^" => Math.Pow(OperatorLeft.GetValue(), OperatorRight.GetValue()),
                _ => throw new ArgumentException()
            };
        }
        if (OperatorLeft != null && OperatorRight == null) // только левый
        {
            return StringOperator switch
            {
                "%" => OperatorLeft.GetValue()/100.0,
                _ => throw new ArgumentException()
            };
        }
        if (OperatorLeft == null && OperatorRight != null) // только правый
        {
            return StringOperator switch
            {
                "sqrt" => Math.Sqrt(OperatorRight.GetValue()),
                "-" => -OperatorRight.GetValue(),
                _ => throw new ArgumentException()
            };
        }

        throw new ArgumentException();
    }
    
    public bool ChangeToLessPriorityOperator(string op)
    {
        int newOperatorPriority = GetOperatorPriority(op);
        if (newOperatorPriority != -1 && (newOperatorPriority < PriorityLevel || PriorityLevel == -1))
        {
            StringOperator = op;
            PriorityLevel = newOperatorPriority;
            return true;
        }
        return false;
    }

    public static int GetOperatorPriority(string op)
    {
        switch (op)
        {
            case "-":
            case "+":
                return 1;
            case "*":
            case "%":
            case "/":
            case "mod":
                return 2;
            case "^":
            case "sqrt":
                return 3;

            default:
                return -1;
        }
    }

    public override string ToString()
    {
        return $"'{OperatorLeft}' [{StringOperator}] '{OperatorRight}'";
    }
    
    public static IOperator? GetGeneralOperator(string expression)
    {
        expression = SyncTrimBrackets(expression.Replace(" ",""));
        var countBrackets = 0;//текущий уровень скобки
        Operator resultOperator = new Operator();
        var operatorIndex = -1;
        for (var i = expression.Length - 1; i >= 0; i--)
        {
            if (expression[i] == '(')
                countBrackets--;
            else if (expression[i] == ')')
                countBrackets++;

            if (countBrackets != 0) 
                continue; 
            
            for (int len = Operator.MaxOperatorLength; len >= 1; len--)
            {
                if (i + len - 1 >= expression.Length)
                    continue;
                var op = expression.Substring(i, len);
                if (resultOperator.ChangeToLessPriorityOperator(op))
                {
                    operatorIndex = i;
                    break;
                }
            }
        }
        
        if (resultOperator.PriorityLevel < 0) 
            return double.TryParse(expression, out var result) ? new ConstOperator(result) : 
                    expression == "π" ? new ConstOperator(Math.PI) : 
                    expression.Equals("pi", StringComparison.CurrentCultureIgnoreCase) ? new ConstOperator(Math.PI) : 
                    expression.Equals("e", StringComparison.CurrentCultureIgnoreCase) ? new ConstOperator(Math.E) : null;
        resultOperator.OperatorLeft = GetGeneralOperator(expression[..operatorIndex]);
        resultOperator.OperatorRight = GetGeneralOperator(expression[(operatorIndex + resultOperator.StringOperator.Length)..]);
        return resultOperator;
    }
    
    private static string SyncTrimBrackets(string str)
    {
        if (str.Length < 2 || str[0] != '(' || str[^1] != ')')
            return str;

        var countBrackets = 0;//текущий уровень скобки
        for (var i = 1; i < str.Length - 1; i++)
        {
            if (str[i] == '(')
                countBrackets++;
            else if (str[i] == ')')
                countBrackets--;

            if (countBrackets == -1)
                return str;
        }

        if (countBrackets != 0) 
            return str;
        var answer = str.Remove(str.Length - 1, 1).Remove(0, 1);
        var nextAnswer = SyncTrimBrackets(answer);
        return nextAnswer;
    }
}