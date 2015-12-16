using MuAtl.Service.Reader.Model;
using System;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class L2mCandidateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var candidate = value as L2mCandidate;
      return string.Format("{0} {1}", candidate.Rule, candidate.Line);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  } 
}
