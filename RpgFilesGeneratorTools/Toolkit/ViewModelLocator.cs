using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace RpgFilesGeneratorTools.Toolkit;

public static class ViewModelLocator
{
    public static bool GetAutoHookedUpViewModel(DependencyObject obj)
    {
        return (bool)obj.GetValue(AutoHookedUpViewModelProperty);
    }

    public static void SetAutoHookedUpViewModel(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoHookedUpViewModelProperty, value);
    }

    public static readonly DependencyProperty AutoHookedUpViewModelProperty = DependencyProperty.RegisterAttached(
        "AutoHookedUpViewModel",
        typeof(bool),
        typeof(ViewModelLocator),
        new PropertyMetadata(false, AutoHookedUpViewModelChanged));

    private static void AutoHookedUpViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement frameworkElement)
        {
            return;
        }

        var viewType = d.GetType();

        var fullName = viewType.FullName;

        if (fullName is null)
        {
            return;
        }

        const string viewModelSuffix = "ViewModel";
        const string pagesNamespace = ".Pages.";
        const string viewModelsNamespace = ".ViewModels.";

        var viewModelTypeName = fullName.Replace(pagesNamespace, viewModelsNamespace) + viewModelSuffix;
        var viewModelType = Type.GetType(viewModelTypeName);

        if (viewModelType is null)
        {
            return;
        }

        var serviceProvider = App.Services;
        var viewModel = ActivatorUtilities.CreateInstance(serviceProvider, viewModelType);
        frameworkElement.DataContext = viewModel;
    }
}