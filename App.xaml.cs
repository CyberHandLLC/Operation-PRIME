using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Application.Interfaces;
using OperationPrime.Application.Services;
using OperationPrime.Infrastructure;
using OperationPrime.Presentation.ViewModels;
using OperationPrime.Presentation.Views;
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
            
            // Add global exception handling for .NET 9 / WinUI 3
            this.UnhandledException += OnUnhandledException;
            
            ConfigureServices();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Use proper DI instead of direct instantiation
            _window = GetService<MainWindow>();
            
            // Seed applications database on startup (runs only if empty)
            _ = Task.Run(async () =>
            {
                try
                {
                    var applicationService = GetService<IApplicationService>();
                    await applicationService.SeedApplicationsAsync();
                }
                catch (Exception ex)
                {
                    // Log error but don't prevent app startup
                    System.Diagnostics.Debug.WriteLine($"Failed to seed applications: {ex.Message}");
                }
            });
            
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
            
            // Configure structured logging per Microsoft.Extensions.Logging documentation
            builder.ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                
                // Set minimum log level based on environment
                logging.SetMinimumLevel(LogLevel.Information);
                
#if DEBUG
                logging.SetMinimumLevel(LogLevel.Debug);
#endif
            });
            
            builder.ConfigureServices((context, services) =>
            {
                // Register infrastructure services
                services.AddInfrastructure();
                
                // Register MainWindow with DI (proper constructor injection)
                services.AddTransient<MainWindow>();
                
                // Register ViewModels as Transient (new instance each time)
                services.AddTransient<ShellViewModel>();
                services.AddTransient<IncidentListViewModel>();
                services.AddTransient<IncidentCreateViewModel>();
                
                // Note: Views are not registered in DI as they use service locator pattern
                // for ViewModel resolution due to WinUI 3 Frame navigation requirements
                
                // Note: Application services are registered in Infrastructure layer
                // Following Microsoft's 2024 DI guidelines - services registered once
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

        /// <summary>
        /// Handles unhandled exceptions globally for the application.
        /// Provides comprehensive error logging and graceful failure handling.
        /// </summary>
        /// <param name="sender">The application instance.</param>
        /// <param name="e">Exception event arguments.</param>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            try
            {
                // Log the exception (in a real app, you'd use a logging framework like Serilog)
                System.Diagnostics.Debug.WriteLine(
                    $"Unhandled exception: {e.Exception.GetType().Name} - {e.Exception.Message}");
                
                // Log full stack trace for debugging
                System.Diagnostics.Debug.WriteLine($"Stack trace: {e.Exception.StackTrace}");

                // Mark as handled to prevent app crash (can be removed if you want crashes)
                e.Handled = true;

                // In production, you might want to:
                // 1. Send telemetry/crash reports
                // 2. Show user-friendly error dialog
                // 3. Attempt graceful recovery
                // 4. Save user data before potential shutdown
            }
            catch
            {
                // If exception handling itself fails, let the original exception bubble up
                // This prevents infinite loops in error handling
            }
        }

        /// <summary>
        /// Handles application shutdown and ensures proper disposal of resources.
        /// WinUI 3 doesn't have OnExit like WPF, so we implement IDisposable pattern.
        /// The host will be disposed when the application terminates.
        /// </summary>
        ~App()
        {
            try
            {
                // Dispose of the host to properly shut down all services
                _host?.Dispose();
            }
            catch (Exception ex)
            {
                // Log disposal errors but don't prevent application shutdown
                System.Diagnostics.Debug.WriteLine($"Error during application shutdown: {ex.Message}");
            }
        }
    }
}
