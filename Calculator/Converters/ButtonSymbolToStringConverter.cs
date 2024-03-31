using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Calculator.Models;

namespace Calculator.Converters;

public class ButtonSymbolToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        if (value is not ButtonSymbol symbol)
            return new BindingNotification(new InvalidCastException(),
                BindingErrorType.Error);

        // converter used for the wrong type
        return symbol switch
        {
            ButtonSymbol.Digit0 => 0,
            ButtonSymbol.Digit1 => 1,
            ButtonSymbol.Digit2 => 2,
            ButtonSymbol.Digit3 => 3,
            ButtonSymbol.Digit4 => 4,
            ButtonSymbol.Digit5 => 5,
            ButtonSymbol.Digit6 => 6,
            ButtonSymbol.Digit7 => 7,
            ButtonSymbol.Digit8 => 8,
            ButtonSymbol.Digit9 => 9,
            ButtonSymbol.Plus => "+",
            ButtonSymbol.Minus => "-",
            ButtonSymbol.Multiply => "*",
            ButtonSymbol.Divide => "/",
            ButtonSymbol.Result => "=",
            ButtonSymbol.LeftParenthesis => "(",
            ButtonSymbol.RightParenthesis => ")",
            ButtonSymbol.Clear => "C",
            ButtonSymbol.Dot => ".",
            ButtonSymbol.Percentage => "%",
            ButtonSymbol.Modulus => " mod ",
            ButtonSymbol.Pi => "π",
            ButtonSymbol.Root => "\u221a",
            ButtonSymbol.Square => "x\u00b2",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}