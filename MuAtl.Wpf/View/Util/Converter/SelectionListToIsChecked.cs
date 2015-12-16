using System;
using System.Windows.Data;
using System.Linq;
using MuAtl.Model;
using System.Collections.Generic;
using MuAtl.ViewModel.Util;

namespace MuAtl.View.Util.Converter
{  
  public class SelectionListToIsChecked : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var selectedTestCases = (values[0] as IEnumerable<MuAtlTestCase>);
      var allTestCases = values[1] as SelectionList<MuAtlTestCase>;

      if (!selectedTestCases.Any())
      {
        return false;
      }

      if (selectedTestCases.Count() == allTestCases.Count())
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
