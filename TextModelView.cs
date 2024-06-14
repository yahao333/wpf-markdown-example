using System;
using System.Windows.Documents;

public class TextModelView : ViewModelBase
{
    public string content_;

    public string MarkdownContent
    {
        get { return content_; }
        set
        {

            if (content_ != value)
            {
                SetProperty(ref content_, value);
                RenderMarkdownContent();
            }
        }
    }

    public FlowDocument Document { get; private set; } = new FlowDocument();

    public void RenderMarkdownContent()
    {
        // 将_markdownContent 中的文本转化成FlowDocument 存储在Document 中
        // 这里需要你实现Markdown文本到FlowDocument的转换
        Console.WriteLine("render");
    }
}
