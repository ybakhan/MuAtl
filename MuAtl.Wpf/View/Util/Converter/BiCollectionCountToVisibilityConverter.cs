using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class BiCollectionCountToVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("The target must be a count");

      if(values[0].GetType() != typeof(int) || values[1].GetType() != typeof(int))
      {
        return Visibility.Collapsed;
      }

      if ((int)values[0] > 0 && (int)values[1] > 0)
      {
        return Visibility.Visible;
      }

      return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
