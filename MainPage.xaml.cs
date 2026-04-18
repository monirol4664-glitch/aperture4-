using MathNet.Symbolics;
using MathNet.Numerics;
using Microsoft.Maui.Controls;
using System;

namespace aperture4;

public partial class MainPage : ContentPage
{
    private string _currentInput = "";
    
    public MainPage()
    {
        InitializeComponent();
        AddWelcomeMessage();
    }
    
    private void AddWelcomeMessage()
    {
        AddOutput("=", new string('=', 50), "#666666");
        AddOutput("🔷", "Aperture4 Ready", "#4CAF50", true);
        AddOutput("=", new string('=', 50), "#666666");
        AddOutput("", "");
        AddOutput("📖", "Examples:", "#ff9800", true);
        AddOutput("", "  2 + 2 = 4");
        AddOutput("", "  sin(pi/2) = 1");
        AddOutput("", "  sqrt(16) = 4");
        AddOutput("", "  2^3 = 8");
        AddOutput("", "");
        AddOutput("✅", "Ready!", "#4CAF50", true);
    }
    
    private void OnButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var symbol = button.Text;
        
        symbol = symbol switch
        {
            "÷" => "/",
            "×" => "*",
            "−" => "-",
            "√" => "sqrt(",
            "π" => "pi",
            _ => symbol
        };
        
        _currentInput += symbol;
        InputEntry.Text = _currentInput;
    }
    
    private async void OnEvaluateClicked(object sender, EventArgs e)
    {
        var expression = InputEntry.Text?.Trim();
        if (string.IsNullOrEmpty(expression))
            return;
        
        AddOutput("»", expression, "#4CAF50", true);
        
        try
        {
            var expr = Infix.ParseOrUndefined(expression);
            
            if (expr.IsUndefined)
                throw new Exception("Invalid expression");
            
            var result = Evaluate.Evaluate(null, expr);
            string resultStr = FormatResult(result);
            
            AddOutput("=", resultStr, "#eeeeee");
        }
        catch (Exception ex)
        {
            AddOutput("✗", $"Error: {ex.Message}", "#f44336", true);
        }
        
        _currentInput = "";
        InputEntry.Text = "";
    }
    
    private string FormatResult(FloatingPoint value)
    {
        var realValue = value.RealValue;
        
        if (Math.Abs(realValue - Math.Round(realValue)) < 1e-10)
            return Math.Round(realValue).ToString();
        
        return realValue.ToString("G8");
    }
    
    private void OnClearClicked(object sender, EventArgs e)
    {
        _currentInput = "";
        InputEntry.Text = "";
    }
    
    private void AddOutput(string prefix, string message, string color = "#eeeeee", bool isPrefix = false)
    {
        var layout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 10
        };
        
        if (!string.IsNullOrEmpty(prefix))
        {
            layout.Children.Add(new Label
            {
                Text = prefix,
                TextColor = Color.FromArgb("#4CAF50"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                WidthRequest = 30
            });
        }
        
        layout.Children.Add(new Label
        {
            Text = message,
            TextColor = Color.FromArgb(color),
            FontSize = isPrefix ? 16 : 14,
            FontAttributes = isPrefix ? FontAttributes.Bold : FontAttributes.None
        });
        
        OutputStack.Children.Add(layout);
    }
    
    private void AddOutput(string message)
    {
        OutputStack.Children.Add(new Label
        {
            Text = message,
            TextColor = Color.FromArgb("#eeeeee"),
            FontSize = 12,
            Margin = new Thickness(0, 2)
        });
    }
}