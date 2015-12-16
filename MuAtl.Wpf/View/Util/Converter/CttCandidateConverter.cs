using MuAtl.Service.Reader.Model;
using System;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class CttCandidateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var candidate = value as CttCandidate;
      if (candidate == null)
        return "";

      return string.Format("{0} ({1})", candidate.Rule, candidate.TargetType);    
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
