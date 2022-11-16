using Microsoft.UI.Xaml.Controls;

namespace LlamaRpg.App.Toolkit.Controls;

internal class TemplateSelectorAwareContentControl : ContentControl
{
    /// <summary>
    /// Invoked when the value of the Content property changes.
    /// </summary>
    /// <param name="oldContent">The old value of the Content property.</param>
    /// <param name="newContent">The new value of the Content property.</param>
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        // There is a know issue in the standard content control (WinRT) that trashes the value passed into the SelectTemplateCore method.
        // This is a workaround that allows the same basic structure and can hopefully be replaced when the fixed is published.
        // Basically take the new content and figure out what template should be used with it based on the structure of the template selector.
        if (ContentTemplateSelector != null)
        {
            ContentTemplate = ContentTemplateSelector.SelectTemplate(newContent, null);
        }

        base.OnContentChanged(oldContent, newContent);
    }
}
