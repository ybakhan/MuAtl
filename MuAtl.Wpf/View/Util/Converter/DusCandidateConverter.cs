using MuAtl.Service.Reader.Model;
using System;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class DusCandidateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var candidate = value as DusCandidate;
      return string.Format("{0} {1}", candidate.Library, candidate.Line);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
