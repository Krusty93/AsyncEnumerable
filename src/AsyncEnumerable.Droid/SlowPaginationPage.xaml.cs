using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using AsyncEnumerable.Client.Models;

namespace AsyncEnumerable.Client;

public partial class SlowPaginationPage : ContentPage
{
    public SlowPaginationPage()
    {
        InitializeComponent();
        BindingContext = new SlowPaginationPageViewModel();
    }
}

public class SlowPaginationPageViewModel : INotifyPropertyChanged
{
    private static readonly HttpClient s_httpClient = new();

    public SlowPaginationPageViewModel()
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

            for (var i = 0; i < 5; i++)
            {
                var result = await EnumerateAsync(i);

                foreach (var r in result)
                {
                    Results.Add(r);
                    OnPropertyChanged(nameof(Results));
                }
            }

            cancellationToken.Cancel();
        });
    }

    public string Text { get; private set; }

    public ObservableCollection<int> Results { get; private set; }

    public ICommand LoadCommand { get; }

    private static async Task<IEnumerable<int>> EnumerateAsync(int page)
    {
        var response = await s_httpClient.GetAsync($"http://localhost:5164/enumerators/enumerate?page={page}");

        var text = await response.Content.ReadAsStringAsync();

        var results = JsonSerializer.Deserialize<List<Response>>(text);

        return results.Select(x => x.Value);
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
