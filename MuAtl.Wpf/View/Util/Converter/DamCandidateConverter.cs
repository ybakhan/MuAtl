using MuAtl.Service.Reader.Model;
using System;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class DamCandidateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var candidate = value as DamCandidate;
      return string.Format("{0} ({1} {2})", candidate.Mapping, candidate.Rule, candidate.Line);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
