using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.Services;
using OperationPrime.Presentation.ViewModels;
using OperationPrime.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace OperationPrime
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Microsoft.UI.Xaml.Application
    {
        private Window? _window;
        private IHost? _host;

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public static new App Current => (App)Microsoft.UI.Xaml.Application.Current;

        /// <summary>
        /// Gets the service provider for dependency injection.
        /// </summary>
        public IServiceProvider Services => _host?.Services ?? throw new InvalidOperationException("Services not initialized");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            ConfigureServices();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            _window = new MainWindow(Services);
            _window.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Configures the dependency injection services.
        /// </summary>
        private void ConfigureServices()
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices((context, services) =>
            {
                // Register ViewModels as Transient (new instance each time)
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<PlaceholderViewModel>();
                services.AddTransient<MainPageViewModel>();
                services.AddTransient<BaseViewModel>();
                services.AddTransient<IncidentViewModel>();
                services.AddTransient<PreIncidentViewModel>();
                services.AddTransient<MajorIncidentViewModel>();
                services.AddTransient<IncidentWizardViewModel>();

                // Create configuration for the infrastructure layer
                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "OperationPrime",
                    "incidents.db");

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Database:ConnectionString"] = $"Data Source={dbPath}",
                        ["Database:EncryptionKey"] = "local-dev-key"
                    })
                    .Build();

                // Register infrastructure services and repositories
                services.AddInfrastructure(config);
            });

            _host = builder.Build();
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get.</typeparam>
        /// <returns>The service instance.</returns>
        public T GetService<T>() where T : class
        {
            return Services.GetRequiredService<T>();
        }
    }
}
