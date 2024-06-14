using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Markdown
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //DataContext = new TextModelView();
            DataContext = new MainViewModel();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textModelView = DataContext as TextModelView;
            textModelView.MarkdownContent = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
        }

    }
}
