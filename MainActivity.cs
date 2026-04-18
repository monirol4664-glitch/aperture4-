using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MathematicaConsole;

[Activity(Label = "Mathematica Console", MainLauncher = true)]
public class MainActivity : Activity
{
    private EditText inputField;
    private TextView outputArea;
    private LinearLayout layout;
    private int counter = 1;
    private List<string> history = new List<string>();
    private int historyIndex = 0;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        layout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            SetBackgroundColor = Color.ParseColor("#1e1e1e")
        };
        
        var title = new TextView(this)
        {
            Text = "Mathematica Console",
            TextSize = 24,
            Gravity = GravityFlags.Center,
            SetBackgroundColor = Color.ParseColor("#2d2d2d")
        };
        title.SetTextColor(Color.ParseColor("#4ec9b0"));
        title.SetPadding(0, 30, 0, 30);
        
        outputArea = new TextView(this)
        {
            TextSize = 16,
            Typeface = Typeface.Create("monospace", TypefaceStyle.Normal)
        };
        outputArea.SetTextColor(Color.White);
        outputArea.SetPadding(30, 30, 30, 30);
        
        outputArea.Text = "========================================\n";
        outputArea.Text += "    Mathematica Console v1.0\n";
        outputArea.Text += "========================================\n\n";
        outputArea.Text += "Examples:\n";
        outputArea.Text += "  2 + 2\n";
        outputArea.Text += "  10 / 3\n";
        outputArea.Text += "  (5+3)*2\n";
        outputArea.Text += "  2^3\n";
        outputArea.Text += "  Sin[30]\n";
        outputArea.Text += "  Sqrt[16]\n";
        outputArea.Text += "  Log[100]\n\n";
        outputArea.Text += "========================================\n";
        
        var inputLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            SetBackgroundColor = Color.ParseColor("#252526")
        };
        inputLayout.SetPadding(20, 20, 20, 20);
        
        var prompt = new TextView(this)
        {
            Text = $"[{counter}]> ",
            TextSize = 16,
            Typeface = Typeface.Create("monospace", TypefaceStyle.Bold)
        };
        prompt.SetTextColor(Color.ParseColor("#4ec9b0"));
        
        inputField = new EditText(this)
        {
            TextSize = 16,
            Hint = "Enter expression...",
            Typeface = Typeface.Create("monospace", TypefaceStyle.Normal),
            LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1)
        };
        inputField.SetTextColor(Color.ParseColor("#d4d4d4"));
        inputField.SetHintTextColor(Color.ParseColor("#6a6a6a"));
        inputField.SetBackgroundColor(Color.Transparent);
        
        inputField.EditorAction += (s, e) => {
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Send)
            {
                Evaluate();
                e.Handled = true;
            }
        };
        
        inputField.KeyPress += (s, e) => {
            if (e.KeyCode == Keycode.DpadUp && history.Count > 0)
            {
                if (historyIndex > 0) historyIndex--;
                inputField.Text = history[historyIndex];
                inputField.SetSelection(inputField.Text.Length);
                e.Handled = true;
            }
            else if (e.KeyCode == Keycode.DpadDown && historyIndex < history.Count - 1)
            {
                historyIndex++;
                inputField.Text = history[historyIndex];
                inputField.SetSelection(inputField.Text.Length);
                e.Handled = true;
            }
        };
        
        var button = new Button(this)
        {
            Text = "=",
            SetBackgroundColor = Color.ParseColor("#0e639c")
        };
        button.SetTextColor(Color.White);
        button.Click += (s, e) => Evaluate();
        
        inputLayout.AddView(prompt);
        inputLayout.AddView(inputField);
        inputLayout.AddView(button);
        
        layout.AddView(title);
        layout.AddView(outputArea, new LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.MatchParent, 0, 1));
        layout.AddView(inputLayout);
        
        SetContentView(layout);
        inputField.RequestFocus();
    }
    
    private void Evaluate()
    {
        string input = inputField.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(input)) return;
        
        history.Add(input);
        historyIndex = history.Count;
        
        // Show input
        outputArea.Text = $"\n[{counter}]> {input}\n" + outputArea.Text;
        
        // Evaluate
        string result = MathematicaEvaluate(input);
        outputArea.Text = $"[{counter}] = {result}\n" + outputArea.Text;
        
        inputField.Text = "";
        counter++;
        
        // Update prompt
        var prompt = ((LinearLayout)inputField.Parent).GetChildAt(0) as TextView;
        if (prompt != null) prompt.Text = $"[{counter}]> ";
    }
    
    private string MathematicaEvaluate(string expr)
    {
        try
        {
            // Basic arithmetic
            if (Regex.IsMatch(expr, @"^[\d\s\+\-\*/\(\)\^\.]+$"))
            {
                return EvaluateArithmetic(expr);
            }
            
            // Constants
            if (expr == "Pi") return Math.PI.ToString();
            if (expr == "E") return Math.E.ToString();
            
            // Sin, Cos, Tan
            var trigMatch = Regex.Match(expr, @"(Sin|Cos|Tan)\[(\d+(?:\.\d+)?)\]", RegexOptions.IgnoreCase);
            if (trigMatch.Success)
            {
                double val = double.Parse(trigMatch.Groups[2].Value);
                double rad = val * Math.PI / 180;
                double result = trigMatch.Groups[1].Value.ToLower() switch
                {
                    "sin" => Math.Sin(rad),
                    "cos" => Math.Cos(rad),
                    "tan" => Math.Tan(rad),
                    _ => 0
                };
                return $"{trigMatch.Groups[1].Value}({val}°) = {result:F6}";
            }
            
            // Sqrt
            var sqrtMatch = Regex.Match(expr, @"Sqrt\[(\d+(?:\.\d+)?)\]", RegexOptions.IgnoreCase);
            if (sqrtMatch.Success)
            {
                double val = double.Parse(sqrtMatch.Groups[1].Value);
                return $"√{val} = {Math.Sqrt(val):F6}";
            }
            
            // Log
            var logMatch = Regex.Match(expr, @"Log\[(\d+(?:\.\d+)?)\]", RegexOptions.IgnoreCase);
            if (logMatch.Success)
            {
                double val = double.Parse(logMatch.Groups[1].Value);
                return $"log({val}) = {Math.Log10(val):F6}";
            }
            
            // Integrate
            var intMatch = Regex.Match(expr, @"Integrate\[(.+),\s*(\w+)\]", RegexOptions.IgnoreCase);
            if (intMatch.Success)
            {
                string func = intMatch.Groups[1].Value;
                return func switch
                {
                    "x^2" => "∫ x² dx = x³/3 + C",
                    "x" => "∫ x dx = x²/2 + C",
                    "Sin[x]" => "∫ sin(x) dx = -cos(x) + C",
                    "Cos[x]" => "∫ cos(x) dx = sin(x) + C",
                    "1/x" => "∫ 1/x dx = ln|x| + C",
                    "E^x" => "∫ eˣ dx = eˣ + C",
                    _ => $"∫ {func} dx = [Symbolic result not available]"
                };
            }
            
            // Derivative
            var diffMatch = Regex.Match(expr, @"D\[(.+),\s*(\w+)\]", RegexOptions.IgnoreCase);
            if (diffMatch.Success)
            {
                string func = diffMatch.Groups[1].Value;
                return func switch
                {
                    "x^3" => "d/dx (x³) = 3x²",
                    "x^2" => "d/dx (x²) = 2x",
                    "Sin[x]" => "d/dx sin(x) = cos(x)",
                    "Cos[x]" => "d/dx cos(x) = -sin(x)",
                    "E^x" => "d/dx (eˣ) = eˣ",
                    "Log[x]" => "d/dx ln(x) = 1/x",
                    _ => $"d/dx ({func}) = [Derivative not available]"
                };
            }
            
            // Expand
            var expMatch = Regex.Match(expr, @"Expand\[(.+)\]", RegexOptions.IgnoreCase);
            if (expMatch.Success)
            {
                string func = expMatch.Groups[1].Value;
                return func switch
                {
                    "(x+1)^2" => "(x+1)² expands to x² + 2x + 1",
                    "(x-1)^3" => "(x-1)³ expands to x³ - 3x² + 3x - 1",
                    "(x+2)(x+3)" => "(x+2)(x+3) expands to x² + 5x + 6",
                    _ => $"{func} = [Expansion not implemented]"
                };
            }
            
            // Factor
            var facMatch = Regex.Match(expr, @"Factor\[(.+)\]", RegexOptions.IgnoreCase);
            if (facMatch.Success)
            {
                string func = facMatch.Groups[1].Value;
                return func switch
                {
                    "x^2+2x+1" => "x² + 2x + 1 factors to (x+1)²",
                    "x^2-1" => "x² - 1 factors to (x-1)(x+1)",
                    "x^2+5x+6" => "x² + 5x + 6 factors to (x+2)(x+3)",
                    _ => $"{func} = [Prime or cannot factor]"
                };
            }
            
            // Simplify
            var simMatch = Regex.Match(expr, @"Simplify\[(.+)\]", RegexOptions.IgnoreCase);
            if (simMatch.Success)
            {
                string func = simMatch.Groups[1].Value;
                return func switch
                {
                    "Sin[x]^2+Cos[x]^2" => "sin²(x) + cos²(x) simplifies to 1",
                    "x+x" => "x + x simplifies to 2x",
                    "x*x" => "x * x simplifies to x²",
                    _ => $"{func} = [Cannot simplify further]"
                };
            }
            
            // MatrixForm
            var matMatch = Regex.Match(expr, @"MatrixForm\[{{(.+),(.+)},{(.+),(.+)}}\]", RegexOptions.IgnoreCase);
            if (matMatch.Success)
            {
                return $"┌ {matMatch.Groups[1].Value}  {matMatch.Groups[2].Value} ┐\n└ {matMatch.Groups[3].Value}  {matMatch.Groups[4].Value} ┘";
            }
            
            // Determinant
            var detMatch = Regex.Match(expr, @"Det\[{{(.+),(.+)},{(.+),(.+)}}\]", RegexOptions.IgnoreCase);
            if (detMatch.Success)
            {
                double a = double.Parse(detMatch.Groups[1].Value);
                double b = double.Parse(detMatch.Groups[2].Value);
                double c = double.Parse(detMatch.Groups[3].Value);
                double d = double.Parse(detMatch.Groups[4].Value);
                double det = a * d - b * c;
                return $"det = {det}";
            }
            
            // Default
            return EvaluateArithmetic(expr);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    
    private string EvaluateArithmetic(string expr)
    {
        try
        {
            // Replace ^ with Math.Pow
            var powMatches = Regex.Matches(expr, @"(\d+(?:\.\d+)?)\^(\d+(?:\.\d+)?)");
            foreach (Match match in powMatches)
            {
                double baseNum = double.Parse(match.Groups[1].Value);
                double exponent = double.Parse(match.Groups[2].Value);
                double result = Math.Pow(baseNum, exponent);
                expr = expr.Replace(match.Value, result.ToString());
            }
            
            var table = new DataTable();
            var result = table.Compute(expr, "");
            return $"{expr} = {result}";
        }
        catch
        {
            return $"Could not evaluate: {expr}";
        }
    }
}
