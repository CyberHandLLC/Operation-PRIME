# UI Setup Guide for Beginners
> **🎯 Complete beginner's guide to building WinUI 3 applications with proper MVVM architecture**

## Table of Contents
1. [Understanding the UI Architecture](#understanding-the-ui-architecture)
2. [NavigationView Shell Setup](#navigationview-shell-setup)
3. [Page Structure and MVVM](#page-structure-and-mvvm)
4. [Service Setup and Dependency Injection](#service-setup-and-dependency-injection)
5. [Step-by-Step Implementation](#step-by-step-implementation)
6. [Common Patterns and Best Practices](#common-patterns-and-best-practices)

---

## Understanding the UI Architecture

### What is MVVM?
**Model-View-ViewModel** is a design pattern that separates your UI from your business logic:

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│    View     │───▶│  ViewModel  │───▶│    Model    │
│   (.xaml)   │    │   (.cs)     │    │ (Entities)  │
│             │◀───│             │◀───│             │
└─────────────┘    └─────────────┘    └─────────────┘
```

- **View**: The XAML file (what users see)
- **ViewModel**: The C# class that handles UI logic and data
- **Model**: Your business entities (like Incident class)

### WinUI 3 Application Structure
```
OperationPrime/
├── MainWindow.xaml          ← Main application window
├── App.xaml                 ← Application startup and resources
├── Views/                   ← All your XAML pages
│   ├── ShellView.xaml      ← Navigation shell
│   ├── IncidentListView.xaml
│   └── IncidentCreateView.xaml
├── ViewModels/              ← All your ViewModel classes
│   ├── ShellViewModel.cs
│   ├── IncidentListViewModel.cs
│   └── IncidentCreateViewModel.cs
└── Services/                ← Navigation and other UI services
    └── NavigationService.cs
```

---

## NavigationView Shell Setup

### What is NavigationView?
NavigationView is WinUI 3's built-in control for creating modern Windows apps with:
- Left navigation pane with menu items
- Content area that changes based on selection
- Built-in hamburger menu and search
- Automatic responsive behavior

### Step 1: Create the Shell Structure

**MainWindow.xaml** - The main application window:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Window x:Class="OperationPrime.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- NavigationView is the main shell -->
    <NavigationView x:Name="MainNavigationView"
                    IsBackButtonVisible="Collapsed"
                    Header="Operation Prime">
        
        <!-- Navigation Menu Items -->
        <NavigationView.MenuItems>
            <NavigationViewItem Content="Dashboard" Icon="Home" Tag="Dashboard"/>
            <NavigationViewItem Content="Incidents" Icon="Important" Tag="Incidents"/>
            <NavigationViewItem Content="Reports" Icon="Document" Tag="Reports"/>
        </NavigationView.MenuItems>
        
        <!-- Content area where pages will be displayed -->
        <Frame x:Name="ContentFrame"/>
        
    </NavigationView>
</Window>
```

**MainWindow.xaml.cs** - The code-behind:
```csharp
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace OperationPrime;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.Title = "Operation Prime - Incident Management";
        
        // Handle navigation when user clicks menu items
        MainNavigationView.SelectionChanged += OnNavigationSelectionChanged;
        
        // Set default page
        ContentFrame.Navigate(typeof(Views.IncidentListView));
    }
    
    private void OnNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            string tag = item.Tag?.ToString() ?? "";
            
            switch (tag)
            {
                case "Dashboard":
                    ContentFrame.Navigate(typeof(Views.DashboardView));
                    break;
                case "Incidents":
                    ContentFrame.Navigate(typeof(Views.IncidentListView));
                    break;
                case "Reports":
                    ContentFrame.Navigate(typeof(Views.ReportsView));
                    break;
            }
        }
    }
}
```

---

## Page Structure and MVVM

### How to Create a New Page

Every page in your app follows the same pattern:

#### Step 1: Create the View (XAML file)
**Views/IncidentListView.xaml**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Page x:Class="OperationPrime.Views.IncidentListView"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>    <!-- Header/Search -->
            <RowDefinition Height="*"/>       <!-- Main content -->
            <RowDefinition Height="Auto"/>    <!-- Footer/Actions -->
        </Grid.RowDefinitions>
        
        <!-- Header with search and filters -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="20">
            <TextBox x:Name="SearchBox" 
                     PlaceholderText="Search incidents..."
                     Text="{x:Bind ViewModel.SearchText, Mode=TwoWay}"
                     Width="300"/>
            
            <ComboBox x:Name="TypeFilter"
                      PlaceholderText="Filter by type"
                      SelectedItem="{x:Bind ViewModel.SelectedIncidentType, Mode=TwoWay}"
                      Margin="10,0">
                <x:String>All</x:String>
                <x:String>Pre-Incident</x:String>
                <x:String>Major Incident</x:String>
            </ComboBox>
        </StackPanel>
        
        <!-- Main data grid -->
        <ListView Grid.Row="1" 
                  ItemsSource="{x:Bind ViewModel.FilteredIncidents, Mode=OneWay}"
                  Margin="20">
            <ListView.HeaderTemplate>
                <DataTemplate>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="150"/>
                            <ColumnDefinition Width="*"/>
                            <ColumnDefinition Width="120"/>
                            <ColumnDefinition Width="100"/>
                            <ColumnDefinition Width="80"/>
                        </Grid.ColumnDefinitions>
                        
                        <TextBlock Grid.Column="0" Text="Number" FontWeight="Bold"/>
                        <TextBlock Grid.Column="1" Text="Title" FontWeight="Bold"/>
                        <TextBlock Grid.Column="2" Text="Type" FontWeight="Bold"/>
                        <TextBlock Grid.Column="3" Text="Status" FontWeight="Bold"/>
                        <TextBlock Grid.Column="4" Text="Priority" FontWeight="Bold"/>
                    </Grid>
                </DataTemplate>
            </ListView.HeaderTemplate>
            
            <ListView.ItemTemplate>
                <DataTemplate>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="150"/>
                            <ColumnDefinition Width="*"/>
                            <ColumnDefinition Width="120"/>
                            <ColumnDefinition Width="100"/>
                            <ColumnDefinition Width="80"/>
                        </Grid.ColumnDefinitions>
                        
                        <TextBlock Grid.Column="0" Text="{Binding IncidentNumber}"/>
                        <TextBlock Grid.Column="1" Text="{Binding Title}"/>
                        <TextBlock Grid.Column="2" Text="{Binding IncidentType}"/>
                        <TextBlock Grid.Column="3" Text="{Binding IncidentStatus}"/>
                        <TextBlock Grid.Column="4" Text="{Binding IncidentPriority}"/>
                    </Grid>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
        
        <!-- Action buttons -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Right" Margin="20">
            <Button Content="New Pre-Incident" 
                    Command="{x:Bind ViewModel.CreatePreIncidentCommand}"
                    Style="{StaticResource AccentButtonStyle}"/>
            <Button Content="New Major Incident" 
                    Command="{x:Bind ViewModel.CreateMajorIncidentCommand}"
                    Margin="10,0,0,0"/>
        </StackPanel>
    </Grid>
</Page>
```

#### Step 2: Create the Code-Behind
**Views/IncidentListView.xaml.cs**:
```csharp
using Microsoft.UI.Xaml.Controls;
using OperationPrime.ViewModels;

namespace OperationPrime.Views;

public sealed partial class IncidentListView : Page
{
    public IncidentListViewModel ViewModel { get; }
    
    public IncidentListView()
    {
        this.InitializeComponent();
        
        // Get ViewModel from dependency injection
        ViewModel = App.GetService<IncidentListViewModel>();
        
        // Initialize data when page loads
        this.Loaded += async (s, e) => await ViewModel.LoadIncidentsAsync();
    }
}
```

#### Step 3: Create the ViewModel
**ViewModels/IncidentListViewModel.cs**:
```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Domain.Entities;
using OperationPrime.Application.Interfaces;

namespace OperationPrime.ViewModels;

public partial class IncidentListViewModel : ObservableObject
{
    private readonly IIncidentService _incidentService;
    
    public IncidentListViewModel(IIncidentService incidentService)
    {
        _incidentService = incidentService;
        Incidents = new ObservableCollection<Incident>();
    }
    
    // Properties that the UI binds to
    [ObservableProperty]
    private string? searchText;
    
    [ObservableProperty]
    private string? selectedIncidentType = "All";
    
    [ObservableProperty]
    private ObservableCollection<Incident> incidents;
    
    // Computed property for filtered results
    public IEnumerable<Incident> FilteredIncidents
    {
        get
        {
            var filtered = Incidents.AsEnumerable();
            
            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(i => 
                    i.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.IncidentNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            
            // Filter by incident type
            if (SelectedIncidentType != "All")
            {
                filtered = filtered.Where(i => i.IncidentType == SelectedIncidentType);
            }
            
            return filtered;
        }
    }
    
    // Commands that buttons bind to
    [RelayCommand]
    private async Task CreatePreIncidentAsync()
    {
        // Navigate to create page with Pre-Incident type
        // Implementation depends on your navigation service
    }
    
    [RelayCommand]
    private async Task CreateMajorIncidentAsync()
    {
        // Navigate to create page with Major Incident type
        // Implementation depends on your navigation service
    }
    
    // Method to load data
    public async Task LoadIncidentsAsync()
    {
        var incidents = await _incidentService.GetAllAsync();
        
        Incidents.Clear();
        foreach (var incident in incidents)
        {
            Incidents.Add(incident);
        }
        
        // Notify UI that filtered results may have changed
        OnPropertyChanged(nameof(FilteredIncidents));
    }
    
    // This runs when SearchText or SelectedIncidentType changes
    partial void OnSearchTextChanged(string? value)
    {
        OnPropertyChanged(nameof(FilteredIncidents));
    }
    
    partial void OnSelectedIncidentTypeChanged(string? value)
    {
        OnPropertyChanged(nameof(FilteredIncidents));
    }
}
```

---

## Service Setup and Dependency Injection

### Understanding Dependency Injection
Dependency Injection (DI) is a way to provide your classes with the services they need, rather than creating them directly.

#### Step 1: Define Service Interfaces
**Application/Interfaces/INavigationService.cs**:
```csharp
namespace OperationPrime.Application.Interfaces;

public interface INavigationService
{
    void NavigateTo(string pageKey);
    void NavigateTo(string pageKey, object parameter);
    bool CanGoBack { get; }
    void GoBack();
}
```

#### Step 2: Implement Services
**Infrastructure/Services/NavigationService.cs**:
```csharp
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Application.Interfaces;

namespace OperationPrime.Infrastructure.Services;

public class NavigationService : INavigationService
{
    private Frame? _frame;
    
    public void Initialize(Frame frame)
    {
        _frame = frame;
    }
    
    public bool CanGoBack => _frame?.CanGoBack ?? false;
    
    public void GoBack()
    {
        if (CanGoBack)
            _frame?.GoBack();
    }
    
    public void NavigateTo(string pageKey)
    {
        NavigateTo(pageKey, null);
    }
    
    public void NavigateTo(string pageKey, object? parameter)
    {
        if (_frame == null) return;
        
        var pageType = pageKey switch
        {
            "IncidentList" => typeof(Views.IncidentListView),
            "IncidentCreate" => typeof(Views.IncidentCreateView),
            "Dashboard" => typeof(Views.DashboardView),
            _ => null
        };
        
        if (pageType != null)
        {
            _frame.Navigate(pageType, parameter);
        }
    }
}
```

#### Step 3: Register Services
**Infrastructure/ServiceCollectionExtensions.cs**:
```csharp
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Application.Interfaces;
using OperationPrime.Infrastructure.Services;
using OperationPrime.ViewModels;

namespace OperationPrime.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IIncidentService, IncidentService>();
        
        // Register ViewModels (Transient = new instance each time)
        services.AddTransient<IncidentListViewModel>();
        services.AddTransient<IncidentCreateViewModel>();
        services.AddTransient<DashboardViewModel>();
        
        return services;
    }
}
```

#### Step 4: Configure in App.xaml.cs
**App.xaml.cs**:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using OperationPrime.Infrastructure;

namespace OperationPrime;

public partial class App : Application
{
    private static IHost? _host;
    
    public App()
    {
        this.InitializeComponent();
        
        // Build dependency injection container
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure();
            })
            .Build();
    }
    
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }
    
    private Window? m_window;
    
    // Helper method to get services
    public static T GetService<T>() where T : class
    {
        return _host?.Services.GetService<T>() ?? throw new ArgumentException($"Service {typeof(T)} not found");
    }
}
```

---

## Step-by-Step Implementation

### Phase 1: Basic Shell (Start Here)
1. **Update MainWindow.xaml** to use NavigationView
2. **Create basic navigation** in MainWindow.xaml.cs
3. **Test navigation** between placeholder pages
4. **Verify Hot Reload** works properly

### Phase 2: First Real Page
1. **Create IncidentListView.xaml** with basic layout
2. **Create IncidentListViewModel.cs** with mock data
3. **Test data binding** with simple properties
4. **Add basic filtering** functionality

### Phase 3: Navigation Service
1. **Create INavigationService** interface
2. **Implement NavigationService** class
3. **Register in DI container**
4. **Update MainWindow** to use service

### Phase 4: Real Data Integration
1. **Connect to IncidentService**
2. **Replace mock data** with real service calls
3. **Add error handling** and loading states
4. **Test with real data**

---

## Common Patterns and Best Practices

### 1. Property Binding Patterns
```csharp
// ✅ GOOD: Observable property with automatic notifications
[ObservableProperty]
private string? title;

// ✅ GOOD: Computed property
public string DisplayName => $"{FirstName} {LastName}";

// ❌ BAD: Manual property without notifications
private string _title;
public string Title 
{ 
    get => _title; 
    set => _title = value; // No notification!
}
```

### 2. Command Patterns
```csharp
// ✅ GOOD: Async command with error handling
[RelayCommand]
private async Task SaveAsync()
{
    try
    {
        IsLoading = true;
        await _service.SaveAsync(CurrentItem);
        // Show success message
    }
    catch (Exception ex)
    {
        // Show error message
        _logger.LogError(ex, "Failed to save");
    }
    finally
    {
        IsLoading = false;
    }
}

// ✅ GOOD: Command with can-execute logic
[RelayCommand(CanExecute = nameof(CanSave))]
private async Task SaveAsync() { /* ... */ }

private bool CanSave() => !string.IsNullOrEmpty(Title) && !IsLoading;
```

### 3. Data Loading Patterns
```csharp
// ✅ GOOD: Async data loading with loading states
public async Task LoadDataAsync()
{
    IsLoading = true;
    ErrorMessage = null;
    
    try
    {
        var data = await _service.GetDataAsync();
        Items.Clear();
        foreach (var item in data)
        {
            Items.Add(item);
        }
    }
    catch (Exception ex)
    {
        ErrorMessage = "Failed to load data. Please try again.";
        _logger.LogError(ex, "Data loading failed");
    }
    finally
    {
        IsLoading = false;
    }
}
```

### 4. XAML Binding Best Practices
```xml
<!-- ✅ GOOD: x:Bind is faster and compile-time checked -->
<TextBlock Text="{x:Bind ViewModel.Title, Mode=OneWay}"/>

<!-- ✅ GOOD: Two-way binding for input controls -->
<TextBox Text="{x:Bind ViewModel.SearchText, Mode=TwoWay}"/>

<!-- ✅ GOOD: Command binding -->
<Button Content="Save" Command="{x:Bind ViewModel.SaveCommand}"/>

<!-- ❌ AVOID: Binding without Mode specified -->
<TextBlock Text="{x:Bind ViewModel.Title}"/>
```

### 5. Error Handling Patterns
```csharp
// ✅ GOOD: Centralized error handling
public class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string? errorMessage;
    
    protected async Task ExecuteAsync(Func<Task> operation)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred. Please try again.";
            _logger.LogError(ex, "Operation failed");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

---

## Quick Reference

### Essential XAML Controls
- **NavigationView**: Main app shell with menu
- **ListView/DataGrid**: Display lists of data
- **TextBox**: User text input
- **ComboBox**: Dropdown selection
- **Button**: User actions
- **StackPanel**: Arrange controls horizontally/vertically
- **Grid**: Flexible layout with rows/columns

### Essential ViewModel Attributes
- **[ObservableProperty]**: Auto-generates property with notifications
- **[RelayCommand]**: Auto-generates command from method
- **[NotifyPropertyChangedFor]**: Notify when related property changes

### Service Lifetimes
- **Singleton**: One instance for entire app (NavigationService)
- **Scoped**: One instance per operation (IncidentService)
- **Transient**: New instance every time (ViewModels)

### Common Binding Modes
- **OneWay**: Data flows from ViewModel to UI only
- **TwoWay**: Data flows both directions (for input controls)
- **OneTime**: Set once when page loads

---

*This guide provides the foundation for building modern WinUI 3 applications with proper MVVM architecture. Start with Phase 1 and build incrementally!*
