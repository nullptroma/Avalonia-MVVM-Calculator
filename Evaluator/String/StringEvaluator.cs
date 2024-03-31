namespace Evaluator.String;

public class StringEvaluator(string expressionString) : IEvaluator
{
    public double Evaluate()
    {
        var op = Operator.GetGeneralOperator(expressionString);
        if (op != null)
        {
            try
            {
                return op.GetValue();
            }
            catch (Exception)
            {
                throw new ArgumentException($"Input '{expressionString}' is not valid.");
            }
        }
        throw new ArgumentException($"Input '{expressionString}' is not valid.");
    }
}