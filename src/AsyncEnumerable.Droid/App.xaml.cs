namespace AsyncEnumerable.Droid
{
#pragma warning disable CA1724
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
