using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace AsyncEnumerable.Client;

public partial class QuickPaginationPage : ContentPage
{
    public QuickPaginationPage()
    {
        InitializeComponent();
        BindingContext = new QuickPaginationPageViewModel();
    }
}

public class QuickPaginationPageViewModel : INotifyPropertyChanged
{
    private static readonly HttpClient s_httpClient = new();
    private CancellationTokenSource _cancellationTokenSource;

    public QuickPaginationPageViewModel()
    {
        Results = new();
        OnPropertyChanged(nameof(Results));

        LoadCommand = new Command(async () =>
        {
            _ = Task.Run(async () =>
            {
                var i = 0;

                while (true)
                {
                    Text = i++.ToString(CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(Text));

                    await Task.Delay(1000);
                }
            });

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    await foreach (var item in EnumerateAsync(i, _cancellationTokenSource.Token))
                    {
                        Results.Add(item);
                        OnPropertyChanged(nameof(Results));
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        });

        StopCommand = new Command(() =>
        {
            _cancellationTokenSource?.Cancel();
        });
    }

    public string Text { get; private set; }

    public ObservableCollection<int> Results { get; private set; }

    public ICommand LoadCommand { get; }

    public ICommand StopCommand { get; }

    private static async IAsyncEnumerable<int> EnumerateAsync(
        int page,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await s_httpClient.GetAsync(
            $"http://localhost:5164/enumerators/enumerate-async?page={page}",
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        IAsyncEnumerable<int> results = JsonSerializer.DeserializeAsyncEnumerable<int>(
            stream,
            new JsonSerializerOptions
            {
                DefaultBufferSize = 128
            },
            cancellationToken);

        await foreach (int item in results)
        {
            await Task.Delay(10, cancellationToken);
            yield return item;
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
