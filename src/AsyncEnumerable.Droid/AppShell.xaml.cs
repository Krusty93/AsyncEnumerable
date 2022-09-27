namespace AsyncEnumerable.Client
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EnumerablePage), typeof(EnumerablePage));

            Routing.RegisterRoute(nameof(AsyncEnumerablePage), typeof(AsyncEnumerablePage));

            Routing.RegisterRoute(nameof(SlowPaginationPage), typeof(SlowPaginationPage));

            Routing.RegisterRoute(nameof(QuickPaginationPage), typeof(QuickPaginationPage));
        }
    }
}
