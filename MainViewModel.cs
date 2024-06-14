using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Markdown
{
    public class MainViewModel : ViewModelBase
    {
        private string _markdownText;

        public FlowDocument MarkdownContent { get; set; }

        public string MarkdownText
        {
            get => _markdownText;
            set
            {
                _markdownText = value;
                // 转换Markdown到XAML，并更新显示内容
                MarkdownContent = MarkdownToXamlConverter.Convert(_markdownText);
                // 通知UI更新MarkdownContent属性
                OnPropertyChanged(nameof(MarkdownContent));
            }
        }

        public MainViewModel()
        {
            if (!File.Exists("data.md"))
            {
                MessageBox.Show("未找到data.md文件");
                return;
            }

            using (var sr = new StreamReader("data.md"))
            {
                var content = sr.ReadToEnd();
                Console.WriteLine("\n\n", content, "\n\n");
                MarkdownText = content;
            }
        }
    }
}
