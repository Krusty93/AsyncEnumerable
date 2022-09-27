using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AsyncEnumerable.Client;

public partial class EnumerablePage : ContentPage
{
    public EnumerablePage()
    {
        InitializeComponent();
        BindingContext = new EnumerablePageViewModel();
    }
}

public class EnumerablePageViewModel : INotifyPropertyChanged
{
    public EnumerablePageViewModel()
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

            var result = await EnumerateAsync();
            Results = new ObservableCollection<int>(result);

            OnPropertyChanged(nameof(Results));

            cancellationToken.Cancel();
        });
    }

    public string Text { get; private set; }

    public ObservableCollection<int> Results { get; private set; }

    public ICommand LoadCommand { get; }

    private static async Task<List<int>> EnumerateAsync()
    {
        const int LIMIT = 20;

        List<int> points = new(LIMIT);

        for (int i = 1; i <= LIMIT; i++)
        {
            await Task.Delay(250);
            points.Add(i);
        }

        return points;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
