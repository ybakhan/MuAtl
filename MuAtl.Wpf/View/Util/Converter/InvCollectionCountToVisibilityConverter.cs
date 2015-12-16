using System;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class InvCollectionCountToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
       System.Globalization.CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("The target must be a count");

      if ((int)value == 0)
      {
        return Visibility.Visible;
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
       System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

}
