using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages;

internal sealed partial class AffixDetails
{
    public AffixDetails()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
