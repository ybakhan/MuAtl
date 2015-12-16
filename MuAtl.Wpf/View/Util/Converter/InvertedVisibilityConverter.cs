using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class InvertedVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("The target must be a count");

      if ((Visibility)value == Visibility.Visible)
      {
        return Visibility.Collapsed;
      }

      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
