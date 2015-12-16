using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class MutantTestSuiteConverter : IMultiValueConverter
  {
    public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return values.Clone();
    }

    public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new System.NotImplementedException();
    }
  }
}
