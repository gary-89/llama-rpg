using LlamaRpg.Models.Items.PrimaryTypes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LlamaRpg.App.Pages.Items;

internal sealed class ItemDetailsTemplateSelector : DataTemplateSelector
{
    public DataTemplate? WeaponTemplate { get; set; }
    public DataTemplate? ArmorTemplate { get; set; }
    public DataTemplate? OffhandTemplate { get; set; }
    public DataTemplate? JewelryTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is not ItemBase itemBase)
        {
            return default;
        }

        return itemBase switch
        {
            Weapon => WeaponTemplate,
            Armor => ArmorTemplate,
            Offhand => OffhandTemplate,
            Jewelry => JewelryTemplate,
            _ => default
        };
    }
}
