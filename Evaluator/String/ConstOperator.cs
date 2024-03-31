using System.Globalization;

namespace Evaluator.String;

public class ConstOperator(double value) : IOperator
{
    public double GetValue()
    {
        return value;
    }

    public override string ToString()
    {
        return value.ToString(CultureInfo.CurrentCulture);
    }
}