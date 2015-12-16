using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class CollectionCountToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
       System.Globalization.CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("The target must be a count");

      if ((int)value > 0)
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

  public class SelectedItemToVisConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("The target must be a count");

      if(value != null)
      {
        return Visibility.Visible;
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class InvBoolToVisConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return Visibility.Visible;
      }

      var boolVal = (bool)value;
      if(boolVal)
      {
        return Visibility.Collapsed;
      }

      return Visibility.Visible; ;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
