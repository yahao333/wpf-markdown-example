using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Markdown
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.RegisterAttached(
                "Document",
                typeof(FlowDocument),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata(null, OnDocumentChanged));

        public static void SetDocument(RichTextBox target, FlowDocument value)
        {
            target.SetValue(DocumentProperty, value);
        }

        public static FlowDocument GetDocument(RichTextBox target)
        {
            return (FlowDocument)target.GetValue(DocumentProperty);
        }

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                richTextBox.Document = (FlowDocument)e.NewValue;
            }
        }
    }

}
