using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace MyAndroidApp;

[Activity(Label = "My Android App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
public class MainActivity : Activity
{
    private int clickCount = 0;
    private TextView? counterTextView;
    private TextView? messageTextView;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Create main layout
        var layout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertrical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };
        
        // Set background color
        layout.SetBackgroundColor(Color.ParseColor("#F5F5F5"));
        layout.SetPadding(50, 100, 50, 100);
        
        // Title TextView
        var titleText = new TextView(this)
        {
            Text = "My Android App",
            TextSize = 32,
            Gravity = GravityFlags.CenterHorizontal
        };
        titleText.SetTextColor(Color.ParseColor("#512BD4"));
        titleText.SetTypeface(null, TypefaceStyle.Bold);
        titleText.SetPadding(0, 0, 0, 50);
        
        // Message TextView
        messageTextView = new TextView(this)
        {
            Text = "Built with GitHub Actions!",
            TextSize = 18,
            Gravity = GravityFlags.CenterHorizontal
        };
        messageTextView.SetTextColor(Color.ParseColor("#666666"));
        messageTextView.SetPadding(0, 0, 0, 80);
        
        // Counter TextView
        counterTextView = new TextView(this)
        {
            Text = "0 clicks",
            TextSize = 24,
            Gravity = GravityFlags.CenterHorizontal
        };
        counterTextView.SetTextColor(Color.ParseColor("#333333"));
        counterTextView.SetPadding(0, 0, 0, 50);
        
        // Button
        var button = new Button(this)
        {
            Text = "Click Me!",
            TextSize = 20
        };
        
        // Style the button
        button.SetBackgroundColor(Color.ParseColor("#512BD4"));
        button.SetTextColor(Color.White);
        button.SetPadding(40, 20, 40, 20);
        
        // Button click handler
        button.Click += (sender, e) =>
        {
            clickCount++;
            if (counterTextView != null)
            {
                counterTextView.Text = clickCount == 1 ? 
                    "Clicked 1 time" : 
                    $"Clicked {clickCount} times";
            }
            
            if (messageTextView != null)
            {
                if (clickCount == 1)
                    messageTextView.Text = "Great start! 🎉";
                else if (clickCount == 5)
                    messageTextView.Text = "You're on fire! 🔥";
                else if (clickCount == 10)
                    messageTextView.Text = "Awesome! 10 clicks! 🏆";
                else
                    messageTextView.Text = "Keep clicking! 🚀";
            }
            
            // Vibrate on click (requires permission)
            if (clickCount % 5 == 0)
            {
                var vibrator = (Vibrator?)GetSystemService(VibratorService);
                vibrator?.Vibrate(100);
            }
        };
        
        // Add all views to layout
        layout.AddView(titleText);
        layout.AddView(messageTextView);
        layout.AddView(counterTextView);
        layout.AddView(button);
        
        // Add info text at bottom
        var infoText = new TextView(this)
        {
            Text = "Version 1.0 | Built with GitHub Actions",
            TextSize = 12,
            Gravity = GravityFlags.CenterHorizontal
        };
        infoText.SetTextColor(Color.ParseColor("#999999"));
        infoText.SetPadding(0, 100, 0, 0);
        layout.AddView(infoText);
        
        SetContentView(layout);
    }
}