using MuAtl.Service.Reader.Model;
using MuAtl.ViewModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class L2mRuleSelectionListToIsChecked : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var selectedRules = (values[0] as IEnumerable<L2mCandidate>);
      var allRules = values[1] as SelectionList<L2mCandidate>;

      if (!selectedRules.Any())
      {
        return false;
      }

      if (selectedRules.Count() == allRules.Count())
      {
        return true;
      }
      else
      {
        return null;
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
