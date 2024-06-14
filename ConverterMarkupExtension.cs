using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter where T : class, new()
{
    private static T _converter = null;

    public ConverterMarkupExtension()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _converter ?? (_converter = new T());
    }

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}

public abstract class ConverterMarkupExtensions<T> : MarkupExtension, IMultiValueConverter where T : class, new()
{
    private static T _converter = null;

    public ConverterMarkupExtensions()
    {
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _converter ?? (_converter = new T());
    }
    public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

    public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);


}
