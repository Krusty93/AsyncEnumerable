using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AsyncEnumerable.Client;

public partial class AsyncEnumerablePage : ContentPage
{
    public AsyncEnumerablePage()
    {
        InitializeComponent();
        BindingContext = new AsyncEnumerablePageViewModel();
    }
}

public class AsyncEnumerablePageViewModel : INotifyPropertyChanged
{
    public AsyncEnumerablePageViewModel()
    {
        Results = new();
        OnPropertyChanged(nameof(Results));

        LoadCommand = new Command(async () =>
        {
            var cancellationToken = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                var i = 0;

                while (true)
                {
                    Text = i++.ToString(CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(Text));

                    await Task.Delay(1000, cancellationToken.Token);
                }
            }, cancellationToken.Token);

            await foreach (var point in EnumerateThroughWrapperAsync()) // alternative: await foreach(var customer in await EnumerateThroughWrapperAsync()) 
            {
                Results.Add(point);
                OnPropertyChanged(nameof(Results));
            };

            cancellationToken.Cancel();
        });
    }

    public string Text { get; private set; }

    public ObservableCollection<int> Results { get; private set; }

    public ICommand LoadCommand { get; }

    private static async IAsyncEnumerable<int> EnumerateAsync()
    {
        const int LIMIT = 20;

        for (int i = 1; i <= LIMIT; i++)
        {
            await Task.Delay(250);
            yield return i;
        }
    }

    private static async IAsyncEnumerable<int> EnumerateThroughWrapperAsync()
    {
        await foreach (var i in EnumerateAsync())
        {
            yield return i;
        }
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
