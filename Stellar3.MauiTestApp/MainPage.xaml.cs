using R3;
using Stellar3.PropertyChanged;

namespace Stellar3.MauiTestApp;

public partial class MainPage : ContentPage
{
    private MyViewModel ViewModel { get; }

    public MainPage()
    {
        BindingContext = ViewModel = new MyViewModel();

        InitializeComponent();
        
        ViewModel.BindOneWay(this, vm => vm.Counter.Value, ui => ui.CounterLabel.Text, x => $"Current Count: {x}");
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        ViewModel.Counter.Value++;
    }
}

public class MyViewModel : RxObject
{
    public BindableReactiveProperty<int> Counter { get; }
    
    public BindableReactiveProperty<string> DisplayCounter { get; }

    public MyViewModel()
    {
        Counter = new();
        
        DisplayCounter =
            this.WhenChanged(x => x.Counter.Value)
                .Select(x => $"Clicked {x} times")
                .ToBindableReactiveProperty(string.Empty);
    }
}