using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class MarkdownToXamlConverter
{
    public static FlowDocument Convert1(string markdownText)
    {
        // 把Markdown转换为简单的XAML
        string xamlText = markdownText;
        //string xamlText = Regex.Replace(markdownText, @"**(.*?)**", @"<Bold>$1</Bold>"); // 粗体
        //xamlText = Regex.Replace(xamlText, @"*(.*?)*", @"<Italic>$1</Italic>"); // 斜体
        //xamlText = Regex.Replace(xamlText, @"# (.*?)n", @"<Paragraph FontSize=""24"">$1</Paragraph>"); // 标题

        // 包裹在 FlowDocument 中
        xamlText = $"<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">{xamlText}</FlowDocument>";

        FlowDocument document = XamlReader.Parse(xamlText) as FlowDocument;
        return document;
    }
    public static FlowDocument Convert2(string markdownText)
    {
        string xamlText = markdownText;
        // 包裹在 FlowDocument 中
        xamlText = $"<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">{xamlText}</FlowDocument>";

        FlowDocument document = XamlReader.Parse(xamlText) as FlowDocument;
        return document;
    }
    public static FlowDocument Convert3(string markdownText)
    {
        string xamlText = markdownText;
        FlowDocument document = new FlowDocument();
        document.Blocks.Clear();

        try
        {
            // 处理粗体 **text**
            var boldPattern = "(\\*\\*|__)(.*?)\\1";

            // 解析删除线 ~~text~~
            var strikethroughPattern = "\\~\\~(.*?)\\~\\~";

            // 解析单行代码 code
            var inlineCodePattern = "`{1,2}[^`](.*?)`{1,2}";
            // 解析多行代码块 
            var blockCodePattern = "```([\\s\\S]*?)```[\\s]?";

            // 可以添加更多的Markdown语法处理......
            // 处理顺序：先处理删除线，再处理粗体，以保证正确的顺序嵌套
            var patterns = new[] { boldPattern, strikethroughPattern };
            var regex = new Regex(string.Join(" | ", patterns), RegexOptions.Compiled);


            // 使用正则表达式查找Markdown模式，并用相应的WPF文本格式替换
            var matches = regex.Matches(markdownText);

            // 初始化一个新的Paragraph
            var paragraph = new Paragraph();
            document.Blocks.Add(paragraph);

            var start = 0;
            foreach (Match match in matches)
            {
                // 添加普通文本到同一个Paragraph
                if (match.Index > start)
                {
                    paragraph.Inlines.Add(new Run(markdownText.Substring(start, match.Index - start)));
                }

                //// 添加粗体文本到同一个Paragraph
                //paragraph.Inlines.Add(new Bold(new Run(match.Groups[2].Value)));
                if (match.Value.StartsWith("~~"))
                {
                    // 添加删除线文本到同一个Paragraph
                    var strikeText = match.Groups[3].Value;
                    var run = new Run(strikeText);
                    run.TextDecorations = TextDecorations.Strikethrough;
                    paragraph.Inlines.Add(run);
                }
                else if (match.Value.StartsWith("**") || match.Value.StartsWith("__"))
                {
                    // 添加粗体文本到同一个Paragraph
                    var boldText = match.Groups[2].Value;
                    paragraph.Inlines.Add(new Bold(new Run(boldText)));
                }

                start = match.Index + match.Length;
            }

            // 添加剩余的普通文本到同一个Paragraph
            if (start < markdownText.Length)
            {
                paragraph.Inlines.Add(new Run(markdownText.Substring(start)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return document;
    }

    public static FlowDocument Convert4(string markdownText)
    {
        string xamlText = markdownText;
        FlowDocument document = new FlowDocument();
        document.Blocks.Clear();

        try
        {
            // 处理粗体 **text**
            var boldPattern = "(\\*\\*|__)(.*?)\\1";

            // 解析删除线 ~~text~~
            var strikethroughPattern = "\\~\\~(.*?)\\~\\~";

            // 解析单行代码 code
            var inlineCodePattern = "`{1,2}[^`](.*?)`{1,2}";
            // 解析多行代码块 
            var blockCodePattern = "```([\\s\\S]*?)```[\\s]?";

            // 初始化一个新的Paragraph
            var paragraph = new Paragraph();
            document.Blocks.Add(paragraph);

            // 处理顺序：先处理删除线，再处理粗体，以保证正确的顺序嵌套
            var patterns = new Dictionary<string, Action<Match>>
            {
                [boldPattern] = (match) =>
                {
                    paragraph.Inlines.Add(new Bold(new Run(match.Groups[2].Value)));
                },
                [strikethroughPattern] = (match) =>
                {
                    var run = new Run(match.Groups[1].Value);
                    run.TextDecorations = TextDecorations.Strikethrough;
                    paragraph.Inlines.Add(run);
                },
                //[inlineCodePattern] = (match) =>
                //{
                //    var run = new Run(match.Groups[1].Value);
                //    run.Background = Brushes.LightGray; // For example, set the background to light gray
                //    run.FontFamily = new FontFamily("Consolas"); // A common code font
                //    paragraph.Inlines.Add(run);
                //},
                [blockCodePattern] = (match) =>
                {
                    // 创建拷贝按钮
                    Button copyButton = new Button { Content = "Copy", IsEnabled = true, Margin = new Thickness(0, 0, 0, 5) };
                    copyButton.Click += (sender, e) =>
                    {
                        // 拷贝代码到剪贴板
                        Clipboard.SetText(match.Groups[1].Value);
                    };

                    // 创建拷贝按钮的容器
                    InlineUIContainer container = new InlineUIContainer(copyButton, paragraph.ContentStart);
                    // 向段落添加拷贝按钮
                    paragraph.Inlines.Add(container);

                    var run = new Run(match.Groups[1].Value);

                    run.Background = Brushes.LightGray; // For example, set the background to light gray
                    run.FontFamily = new FontFamily("Consolas"); // A common code font
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(run);
                    paragraph.Inlines.Add(new LineBreak());
                }
            };
            //var regex = new Regex(string.Join(" | ", patterns), RegexOptions.Compiled);


            // 使用正则表达式查找Markdown模式，并用相应的WPF文本格式替换
            //var matches = regex.Matches(markdownText);

            var start = 0;
            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern.Key, RegexOptions.Singleline);
                var matches = regex.Matches(markdownText);

                foreach (Match match in matches)
                {
                    if (match.Index > start)
                    {
                        var text = markdownText.Substring(start, match.Index - start);
                        paragraph.Inlines.Add(new Run(text));
                    }

                    // Apply the matched formatting
                    pattern.Value.Invoke(match);

                    start = match.Index + match.Length;
                }
            }

            // 添加剩余的普通文本到同一个Paragraph
            if (start < markdownText.Length)
            {
                paragraph.Inlines.Add(new Run(markdownText.Substring(start)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return document;
    }

    public static FlowDocument Convert5(string markdownText)
    {
        string xamlText = markdownText;
        FlowDocument document = new FlowDocument();
        document.Blocks.Clear();

        try
        {
            // 处理粗体 **text**
            var boldPattern = "(\\*\\*|__)(.*?)\\1";

            // 解析删除线 ~~text~~
            var strikethroughPattern = "\\~\\~(.*?)\\~\\~";

            // 解析单行代码 code
            var inlineCodePattern = "`{1,2}[^`](.*?)`{1,2}";
            // 解析多行代码块 
            var blockCodePattern = "```([\\s\\S]*?)```[\\s]?";

            // 初始化一个新的Paragraph
            var paragraph = new Paragraph();
            document.Blocks.Add(paragraph);

            // 处理顺序：先处理删除线，再处理粗体，以保证正确的顺序嵌套
            var patterns = new Dictionary<string, Action<Match>>
            {
                [boldPattern] = (match) =>
                {
                    paragraph.Inlines.Add(new Bold(new Run(match.Groups[2].Value)));
                },
                [strikethroughPattern] = (match) =>
                {
                    var run = new Run(match.Groups[1].Value);
                    run.TextDecorations = TextDecorations.Strikethrough;
                    paragraph.Inlines.Add(run);
                },
                [blockCodePattern] = (match) =>
                {
                    // 创建拷贝按钮
                    Button copyButton = new Button { Content = "Copy", IsEnabled = true, Margin = new Thickness(0, 0, 0, 5) };
                    copyButton.Click += (sender, e) =>
                    {
                        // 拷贝代码到剪贴板
                        Clipboard.SetText(match.Groups[1].Value);
                    };

                    // 创建包含代码文本的TextBlock
                    TextBox codeText = new TextBox
                    {
                        Text = match.Groups[1].Value,
                        Background = Brushes.LightGray,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(4)
                    };

                    // 使用StackPanel来组合按钮和代码边框
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Children.Add(copyButton);
                    stackPanel.Children.Add(codeText);

                    // 创建Border并设置属性
                    Border codeBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0, 4, 0, 4),
                        Padding = new Thickness(4),
                        Child = stackPanel
                    };

                    // 创建UI容器来包裹StackPanel
                    InlineUIContainer container = new InlineUIContainer(codeBorder);


                    // 向段落添加拷贝按钮
                    paragraph.Inlines.Add(container);
                }
            };

            var start = 0;
            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern.Key, RegexOptions.Singleline);
                var matches = regex.Matches(markdownText);

                foreach (Match match in matches)
                {
                    if (match.Index > start)
                    {
                        var text = markdownText.Substring(start, match.Index - start);
                        paragraph.Inlines.Add(new Run(text));
                    }

                    // Apply the matched formatting
                    pattern.Value.Invoke(match);

                    start = match.Index + match.Length;
                }
            }

            // 添加剩余的普通文本到同一个Paragraph
            if (start < markdownText.Length)
            {
                paragraph.Inlines.Add(new Run(markdownText.Substring(start)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return document;
    }
    public static async Task DownloadImageFromUrl(string url, string savePath)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    // 使用同步WriteAllBytes函数，并在Task内部执行
                    await Task.Run(() => File.WriteAllBytes(savePath, imageBytes));
                    Console.WriteLine("Image downloaded successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to download image, reason: " + response.ReasonPhrase);
                }
            }
        }
    }
    private static string getImageFileName(string url)
    {
        Uri uri = new Uri(url);
        string filename = Path.GetFileName(uri.AbsolutePath);

        //Removing additional info after '.png'
        int index = filename.IndexOf(".png");
        if (index > 0) filename = filename.Substring(0, index + 4);

        Console.WriteLine("Filename: " + filename);  //Output: Filename: 30dec492c49145259bba2d025da56d4a.png
        return filename;
    }

    private static string getImageUrl(string inputString)
    {
        string pattern = @"\((.*?)\)";
        Match match = Regex.Match(inputString, pattern);

        if (match.Success)
        {
            string url = match.Groups[1].Value; //获取括号内的内容，即URL
            Console.WriteLine("URL: " + url);
            return url;
        }

        throw new ArgumentException($"error image url:{inputString}");
    }
    public static FlowDocument Convert6(string markdownText)
    {
        string xamlText = markdownText;
        FlowDocument document = new FlowDocument();
        document.Blocks.Clear();

        try
        {
            // 处理粗体 **text**
            var boldPattern = "(\\*\\*|__)(.*?)\\1";

            // 解析删除线 ~~text~~
            var strikethroughPattern = "\\~\\~(.*?)\\~\\~";

            // 解析单行代码 code
            var inlineCodePattern = "`{1,2}[^`](.*?)`{1,2}";
            // 解析多行代码块 
            var blockCodePattern = "```([\\s\\S]*?)```[\\s]?";

            // 解析图片
            var imagePattern = "!\\[[^\\]]+\\]\\([^\\)]+\\)";


            // 初始化一个新的Paragraph
            var paragraph = new Paragraph();
            document.Blocks.Add(paragraph);

            // 处理顺序：先处理删除线，再处理粗体，以保证正确的顺序嵌套
            var patterns = new Dictionary<string, Action<Match>>
            {
                [boldPattern] = (match) =>
                {
                    paragraph.Inlines.Add(new Bold(new Run(match.Groups[2].Value)));
                },
                [strikethroughPattern] = (match) =>
                {
                    var run = new Run(match.Groups[1].Value);
                    run.TextDecorations = TextDecorations.Strikethrough;
                    paragraph.Inlines.Add(run);
                },
                [blockCodePattern] = (match) =>
                {
                    // 创建拷贝按钮
                    Button copyButton = new Button { Content = "Copy", IsEnabled = true, Margin = new Thickness(0, 0, 0, 5) };
                    copyButton.Click += (sender, e) =>
                    {
                        // 拷贝代码到剪贴板
                        Clipboard.SetText(match.Groups[1].Value);
                    };

                    // 创建包含代码文本的TextBlock
                    TextBox codeText = new TextBox
                    {
                        Text = match.Groups[1].Value,
                        Background = Brushes.LightGray,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(4)
                    };

                    // 使用StackPanel来组合按钮和代码边框
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Children.Add(copyButton);
                    stackPanel.Children.Add(codeText);

                    // 创建Border并设置属性
                    Border codeBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0, 4, 0, 4),
                        Padding = new Thickness(4),
                        Child = stackPanel
                    };

                    // 创建UI容器来包裹StackPanel
                    InlineUIContainer container = new InlineUIContainer(codeBorder);


                    // 向段落添加拷贝按钮
                    paragraph.Inlines.Add(container);
                },
                [imagePattern] = (match) =>
                {
                    var all = match.Groups[0].Value;
                    var imageUrl = getImageUrl(all);
                    // 下载图片
                    var savePath = getImageFileName(imageUrl);
                    if (!File.Exists(savePath))
                    {
                        Task.Run(() => DownloadImageFromUrl(imageUrl, savePath)).Wait();
                    }

                    byte[] imageData = File.ReadAllBytes(savePath);
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (var ms = new MemoryStream(imageData))
                        {
                            var image = new Image();
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                            image.Source = bitmap;
                            // 设置图片尺寸，根据需要调整
                            image.MaxWidth = 300; // 示例宽度，根据实际情况调整
                            image.Height = Double.NaN; //Auto; // 高度自适应

                            InlineUIContainer container = new InlineUIContainer(image, paragraph.ContentStart);
                            paragraph.Inlines.Add(container);
                        }
                    }
                }
            };


            var start = 0;
            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern.Key, RegexOptions.Singleline);
                var matches = regex.Matches(markdownText);

                foreach (Match match in matches)
                {
                    if (match.Index > start)
                    {
                        var text = markdownText.Substring(start, match.Index - start);
                        paragraph.Inlines.Add(new Run(text));
                    }

                    // Apply the matched formatting
                    pattern.Value.Invoke(match);

                    start = match.Index + match.Length;
                }
            }

            // 添加剩余的普通文本到同一个Paragraph
            if (start < markdownText.Length)
            {
                paragraph.Inlines.Add(new Run(markdownText.Substring(start)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return document;
    }

    public static FlowDocument Convert(string markdownText)
    {
        string xamlText = markdownText;
        FlowDocument document = new FlowDocument();
        document.Blocks.Clear();

        try
        {
            // 处理粗体 **text**
            var boldPattern = "(\\*\\*|__)(.*?)\\1";

            // 解析删除线 ~~text~~
            var strikethroughPattern = "\\~\\~(.*?)\\~\\~";



            // 解析单行代码 code
            var inlineCodePattern = "`{1,2}[^`](.*?)`{1,2}";
            // 解析多行代码块 
            var blockCodePattern = "```([\\s\\S]*?)```[\\s]?";
            // 处理斜体,表示以一个 * 或者 _ 开头并结尾（\1表示规则和第一个集合相同），中间包含0个或多个字符的字符串
            var italicPattern = "(\\*|_)(.*?)\\1";
            // 引用快
            var blockquotePattern = "(^> ?.+?)((\r?\n\r?\n\\w)|\\Z)";
            // 分割线
            var horizontalRulePattern = "\n-+";
            // 解析无序列表项
            var unorderedListPattern = "[\\s]*[-\\*\\+] +(.*)";
            List currentList = null;

            // 解析图片
            var imagePattern = "!\\[[^\\]]+\\]\\([^\\)]+\\)";


            // 初始化一个新的Paragraph
            var paragraph = new Paragraph();
            document.Blocks.Add(paragraph);

            // 处理顺序：先处理删除线，再处理粗体，以保证正确的顺序嵌套
            var patterns = new Dictionary<string, Action<Match>>
            {
                [boldPattern] = (match) =>
                {
                    paragraph.Inlines.Add(new Bold(new Run(match.Groups[2].Value)));
                },
                //[italicPattern] = (match) =>
                //{
                //    // 捕获斜体文本（无论是用*还是_包裹的）
                //    var italicText = !string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[1].Value : match.Groups[2].Value;

                //    var span = new Span();
                //    span.Inlines.Add(new Italic(new Run(italicText)));

                //    paragraph.Inlines.Add(span);
                //},

                [strikethroughPattern] = (match) =>
                {
                    var run = new Run(match.Groups[1].Value);
                    run.TextDecorations = TextDecorations.Strikethrough;
                    paragraph.Inlines.Add(run);
                },
                [blockCodePattern] = (match) =>
                {
                    // 创建拷贝按钮
                    Button copyButton = new Button { Content = "Copy", IsEnabled = true, Margin = new Thickness(0, 0, 0, 5) };
                    copyButton.Click += (sender, e) =>
                    {
                        // 拷贝代码到剪贴板
                        Clipboard.SetText(match.Groups[1].Value);
                    };

                    // 创建包含代码文本的TextBlock
                    TextBox codeText = new TextBox
                    {
                        Text = match.Groups[1].Value,
                        Background = Brushes.LightGray,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(4)
                    };

                    // 使用StackPanel来组合按钮和代码边框
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Children.Add(copyButton);
                    stackPanel.Children.Add(codeText);

                    // 创建Border并设置属性
                    Border codeBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0, 4, 0, 4),
                        Padding = new Thickness(4),
                        Child = stackPanel
                    };

                    // 创建UI容器来包裹StackPanel
                    InlineUIContainer container = new InlineUIContainer(codeBorder);


                    // 向段落添加拷贝按钮
                    paragraph.Inlines.Add(container);
                },
                //[blockquotePattern] = (match) =>
                //{
                //    // 捕获引用块文本
                //    var blockquoteText = match.Groups[2].Value;

                //    // 创建一个新的 Span 来表示引用块
                //    var span = new Span();
                //    span.Inlines.Add(new Run(blockquoteText));
                //    // 为引用块设置样式，如斜体和前景色
                //    span.FontStyle = FontStyles.Italic;
                //    span.Foreground = new SolidColorBrush(Colors.Gray);

                //    // 根据需要设置段落的其他属性，比如左边距，来更好地区分引用块
                //    paragraph.Margin = new Thickness(20, 0, 0, 0);
                //    paragraph.Inlines.Add(span);
                //},

                //[horizontalRulePattern] = (match) =>
                //{
                //    //// 创建一个新的边界线对象（这个对象可以是UI框架特有的，比如WPF中的Rectangle）
                //    //var line = new System.Windows.Shapes.Rectangle
                //    //{
                //    //    Width = double.NaN, // 自动填满容器的宽度
                //    //    Height = 1, // 分割线的高度，默认为1像素
                //    //    Fill = new SolidColorBrush(Colors.Black), // 分割线的填充颜色，这里为黑色
                //    //    Margin = new Thickness(0, 8, 0, 8) // 上下的间距
                //    //};

                //    // 创建一个Border作为分割线
                //    var line = new Border
                //    {
                //        Background = new SolidColorBrush(Colors.Red), // 分割线颜色
                //        Height = 1, // 设置高度为1
                //        Margin = new Thickness(0, 8, 0, 8), // 设置上下边距
                //        HorizontalAlignment = HorizontalAlignment.Stretch // 使Border填满水平方向
                //    };


                //    // 将BlockUIContainer添加到流文档中
                //    paragraph.Inlines.Add(line);
                //},
                // 添加到patterns字典
                //[unorderedListPattern] = (match) =>
                //{
                //    // 检查是否是列表的开始，或者是继续上一个列表
                //    if (currentList == null)
                //    {
                //        // 创建一个新的无序列表
                //        currentList = new List();
                //        document.Blocks.Add(currentList);
                //    }

                //    // 创建一个新的列表项，并添加到当前列表中
                //    var listItem = new ListItem(new Paragraph(new Run(match.Groups[1].Value.Trim())));
                //    currentList.ListItems.Add(listItem);
                //},
                [imagePattern] = (match) =>
                    {
                        var all = match.Groups[0].Value;
                        var imageUrl = getImageUrl(all);
                        // 下载图片
                        var savePath = getImageFileName(imageUrl);
                        if (!File.Exists(savePath))
                        {
                            Task.Run(() => DownloadImageFromUrl(imageUrl, savePath)).Wait();
                        }

                        byte[] imageData = File.ReadAllBytes(savePath);
                        if (imageData != null && imageData.Length > 0)
                        {
                            using (var ms = new MemoryStream(imageData))
                            {
                                var image = new Image();
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = ms;
                                bitmap.EndInit();
                                image.Source = bitmap;
                                // 设置图片尺寸，根据需要调整
                                image.MaxWidth = 300; // 示例宽度，根据实际情况调整
                                image.Height = Double.NaN; //Auto; // 高度自适应

                                InlineUIContainer container = new InlineUIContainer(image, paragraph.ContentStart);
                                paragraph.Inlines.Add(container);
                            }
                        }
                    }
            };


            var start = 0;
            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern.Key, RegexOptions.Singleline);
                var matches = regex.Matches(markdownText);

                foreach (Match match in matches)
                {
                    if (match.Index > start)
                    {
                        var text = markdownText.Substring(start, match.Index - start);
                        paragraph.Inlines.Add(new Run(text));
                    }

                    // Apply the matched formatting
                    pattern.Value.Invoke(match);

                    start = match.Index + match.Length;
                }
            }

            // 最后，确保处理完整个文本并关闭任何未关闭的列表
            //if (currentList != null && start < markdownText.Length)
            //{
            //    var text = markdownText.Substring(start);
            //    // 创建并添加一个普通段落，或者如果它是列表中的项目，则添加一个列表项
            //    if (!string.IsNullOrWhiteSpace(text))
            //    {
            //        currentList.ListItems.Add(new ListItem(new Paragraph(new Run(text))));
            //    }
            //}

            // 添加剩余的普通文本到同一个Paragraph
            if (start < markdownText.Length)
            {
                paragraph.Inlines.Add(new Run(markdownText.Substring(start)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return document;
    }
}
