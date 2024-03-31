using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Avalonia.Data;
using Calculator.Models;
using Evaluator;
using Evaluator.String;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Calculator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        HandleButtonInputCommand = new RelayCommand(HandleButtonInput);
        SetCurrentFormulaFromExpressionCommand = new RelayCommand(SetCurrentFormulaFromExpression);
        SetCurrentFormulaFromResultCommand = new RelayCommand(SetCurrentFormulaFromResult);
    }

    public ICommand HandleButtonInputCommand { get; }
    public ICommand SetCurrentFormulaFromExpressionCommand { get; }
    public ICommand SetCurrentFormulaFromResultCommand { get; }

    [Reactive] public ObservableCollection<EvaluateResult> History { get; private set; } = [];
    [Reactive] public string CurrentFormula { get; set; } = "";
    [Reactive] public bool IncorrectInput { get; private set; }

    private void SetCurrentFormulaFromExpression(object? param)
    {
        if (param is not EvaluateResult res)
            return;
        CurrentFormula = res.Expression;
    }  
    
    private void SetCurrentFormulaFromResult(object? param)
    {
        if (param is not EvaluateResult res)
            return;
        CurrentFormula += res.Result.ToString(CultureInfo.InvariantCulture);
    } 
    
    private void HandleButtonInput(object? param)
    {
        if (param is not ButtonSymbol symbol)
            return;
        string? add = TryGetString(symbol);
        if (add != null)
        {
            IncorrectInput = false;
            CurrentFormula += add;
            return;
        }

        switch (symbol)
        {
            case ButtonSymbol.Clear:
                CurrentFormula = "";
                IncorrectInput = false;
                break;
            case ButtonSymbol.Result:
                var eval = new StringEvaluator(CurrentFormula);
                try
                {
                    var res = new EvaluateResult(CurrentFormula, eval.Evaluate());
                    History.Add(res);
                    CurrentFormula = res.Result.ToString(CultureInfo.CurrentCulture);
                    IncorrectInput = false;
                }
                catch (Exception)
                {
                    IncorrectInput = true;
                }
                break;
            default:
                break;
        }
    }

    public string? TryGetString(ButtonSymbol sym)
    {
        return sym switch
        {
            ButtonSymbol.Digit0 => "0",
            ButtonSymbol.Digit1 => "1",
            ButtonSymbol.Digit2 => "2",
            ButtonSymbol.Digit3 => "3",
            ButtonSymbol.Digit4 => "4",
            ButtonSymbol.Digit5 => "5",
            ButtonSymbol.Digit6 => "6",
            ButtonSymbol.Digit7 => "7",
            ButtonSymbol.Digit8 => "8",
            ButtonSymbol.Digit9 => "9",
            ButtonSymbol.Plus => "+",
            ButtonSymbol.Minus => "-",
            ButtonSymbol.Multiply => "*",
            ButtonSymbol.Divide => "/",
            ButtonSymbol.LeftParenthesis => "(",
            ButtonSymbol.RightParenthesis => ")",
            ButtonSymbol.Dot => ".",
            ButtonSymbol.Percentage => "%",
            ButtonSymbol.Modulus => " mod ",
            ButtonSymbol.Pi => "π",
            ButtonSymbol.Root => "sqrt(",
            ButtonSymbol.Square => "^2",
            _ => null
        };
    }
}