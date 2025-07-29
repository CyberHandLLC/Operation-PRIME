using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.Constants;
using System.Collections.ObjectModel;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the main application shell with NavigationView
/// </summary>
public partial class ShellViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private bool _isBackEnabled;

    [ObservableProperty]
    private NavigationViewItem? _selected;

    /// <summary>
    /// Collection of navigation menu items
    /// </summary>
    public ObservableCollection<NavigationViewItem> MenuItems { get; }

    /// <summary>
    /// Collection of footer menu items
    /// </summary>
    public ObservableCollection<NavigationViewItem> FooterMenuItems { get; }

    public ShellViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        
        // Initialize menu collections
        MenuItems = new ObservableCollection<NavigationViewItem>();
        FooterMenuItems = new ObservableCollection<NavigationViewItem>();
        
        // Initialize navigation items
        InitializeNavigationItems();
        
        // Subscribe to navigation changes
        _navigationService.NavigationChanged += OnNavigationChanged;
    }

    /// <summary>
    /// Initialize navigation menu items
    /// </summary>
    private void InitializeNavigationItems()
    {
        // Main navigation items using NavigationKeys constants
        MenuItems.Add(new NavigationViewItem
        {
            Content = NavigationKeys.GetFriendlyName(NavigationKeys.Dashboard),
            Icon = new SymbolIcon(Symbol.Home),
            Tag = NavigationKeys.Dashboard
        });

        MenuItems.Add(new NavigationViewItem
        {
            Content = NavigationKeys.GetFriendlyName(NavigationKeys.Incidents),
            Icon = new SymbolIcon(Symbol.Important),
            Tag = NavigationKeys.Incidents
        });

        MenuItems.Add(new NavigationViewItem
        {
            Content = NavigationKeys.GetFriendlyName(NavigationKeys.Reports),
            Icon = new SymbolIcon(Symbol.Document),
            Tag = NavigationKeys.Reports
        });

        MenuItems.Add(new NavigationViewItem
        {
            Content = NavigationKeys.GetFriendlyName(NavigationKeys.NOI),
            Icon = new SymbolIcon(Symbol.Mail),
            Tag = NavigationKeys.NOI
        });

        // Footer items
        FooterMenuItems.Add(new NavigationViewItem
        {
            Content = NavigationKeys.GetFriendlyName(NavigationKeys.Settings),
            Icon = new SymbolIcon(Symbol.Setting),
            Tag = NavigationKeys.Settings
        });
    }

    /// <summary>
    /// Handle navigation item selection
    /// </summary>
    [RelayCommand]
    private void ItemInvoked(NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem item && item.Tag is string tag)
        {
            NavigateToPage(tag);
        }
    }

    /// <summary>
    /// Handle back navigation request
    /// </summary>
    [RelayCommand]
    private void BackRequested()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
    }

    /// <summary>
    /// Navigate to a specific page
    /// </summary>
    /// <param name="pageKey">The key of the page to navigate to</param>
    private void NavigateToPage(string pageKey)
    {
        // For placeholder pages, pass the page name as parameter to customize the message
        object? parameter = null;
        if (pageKey == NavigationKeys.Dashboard || pageKey == NavigationKeys.Reports || 
            pageKey == NavigationKeys.NOI || pageKey == NavigationKeys.Settings)
        {
            parameter = NavigationKeys.GetFriendlyName(pageKey);
        }
        
        // Attempt navigation
        bool navigationSucceeded = _navigationService.NavigateTo(pageKey, parameter);
        
        if (!navigationSucceeded)
        {
            // Navigation failure is now logged in NavigationService with structured logging
        }
    }
    


    /// <summary>
    /// Handle navigation changes
    /// </summary>
    private void OnNavigationChanged(object? sender, string pageName)
    {
        IsBackEnabled = _navigationService.CanGoBack;
        
        // Update selected item based on current page
        UpdateSelectedItem(pageName);
    }

    /// <summary>
    /// Update the selected navigation item
    /// </summary>
    private void UpdateSelectedItem(string pageName)
    {
        // Find and select the corresponding navigation item
        var item = MenuItems.FirstOrDefault(x => x.Tag?.ToString() == pageName) ??
                   FooterMenuItems.FirstOrDefault(x => x.Tag?.ToString() == pageName);
        
        if (item != null)
        {
            Selected = item;
        }
    }

    /// <summary>
    /// Set the navigation frame
    /// </summary>
    public void SetNavigationFrame(Frame frame)
    {
        _navigationService.SetFrame(frame);
    }

    #region IDisposable Implementation

    private bool _disposed = false;

    /// <summary>
    /// Dispose of resources and unsubscribe from events to prevent memory leaks.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method following the standard dispose pattern.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Unsubscribe from navigation service events to prevent memory leaks
            _navigationService.NavigationChanged -= OnNavigationChanged;
            _disposed = true;
        }
    }

    #endregion
}
