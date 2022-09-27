namespace AsyncEnumerable.Client;

public partial class MainPage : ContentPage
{
    private int _count;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        if (_count == 1)
            CounterBtn.Text = $"Clicked {_count} time";
        else
            CounterBtn.Text = $"Clicked {_count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private async void OnEnumerableBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EnumerablePage));
    }

    private async void OnAsyncEnumerableBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AsyncEnumerablePage));
    }

    private async void OnSlowPaginationBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SlowPaginationPage));
    }

    private async void OnQuickPaginationBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(QuickPaginationPage));
    }
}
