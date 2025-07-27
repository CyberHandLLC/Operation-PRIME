using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Application.Interfaces;
using System.Collections.ObjectModel;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the main application shell with NavigationView
/// </summary>
public partial class ShellViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private bool _isBackEnabled;

    [ObservableProperty]
    private object? _selected;

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
        // Main navigation items
        MenuItems.Add(new NavigationViewItem
        {
            Content = "Dashboard",
            Icon = new SymbolIcon(Symbol.Home),
            Tag = "Dashboard"
        });

        MenuItems.Add(new NavigationViewItem
        {
            Content = "Incidents",
            Icon = new SymbolIcon(Symbol.Important),
            Tag = "IncidentList"
        });

        MenuItems.Add(new NavigationViewItem
        {
            Content = "Reports",
            Icon = new SymbolIcon(Symbol.Document),
            Tag = "Reports"
        });

        MenuItems.Add(new NavigationViewItem
        {
            Content = "NOI Management",
            Icon = new SymbolIcon(Symbol.Mail),
            Tag = "NOI"
        });

        // Footer items
        FooterMenuItems.Add(new NavigationViewItem
        {
            Content = "Settings",
            Icon = new SymbolIcon(Symbol.Setting),
            Tag = "Settings"
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
        // For now, we'll just show placeholder content
        // This will be expanded when we add actual pages
        switch (pageKey)
        {
            case "Dashboard":
                // TODO: Navigate to Dashboard page
                break;
            case "IncidentList":
                // TODO: Navigate to Incident List page
                break;
            case "Reports":
                // TODO: Navigate to Reports page
                break;
            case "NOI":
                // TODO: Navigate to NOI Management page
                break;
            case "Settings":
                // TODO: Navigate to Settings page
                break;
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
        IsBackEnabled = _navigationService.CanGoBack;
    }
}
