using MuAtl.Service.Reader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class AamCandidateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var candidate = value as AamCandidate;
      if (candidate == null)
        return string.Empty;

      return string.Format("{0} ({1})", candidate.Rule, candidate.OutPattern);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
