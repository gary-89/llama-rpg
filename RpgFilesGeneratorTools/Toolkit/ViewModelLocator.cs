using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

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
        try
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

            const string ViewModelSuffix = "ViewModel";
            const string PagesNamespace = ".Pages.";
            const string ViewModelsNamespace = ".ViewModels.";

            var viewModelTypeName = fullName.Replace(PagesNamespace, ViewModelsNamespace) + ViewModelSuffix;
            var viewModelType = Type.GetType(viewModelTypeName);

            if (viewModelType is null)
            {
                return;
            }

            if (App.Services is null)
            {
                throw new InvalidCastException();
            }

            var viewModel = ActivatorUtilities.CreateInstance(App.Services, viewModelType);
            frameworkElement.DataContext = viewModel;
        }
        catch (Exception exception)
        {
            var error = exception.Message;
        }
    }
}
