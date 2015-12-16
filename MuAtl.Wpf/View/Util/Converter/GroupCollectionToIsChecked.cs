using MuAtl.Model;
using MuAtl.ViewModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class GroupCollectionToIsChecked : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var mutants = (values[0] as SelectionList<MuAtlMutant>);
      var selectedMutants = (values[1] as IEnumerable<MuAtlMutant>);
      var type = (MutantType)values[2];

      var mutantsOfCurrype = mutants.Where(m => m.Item.Type == type);
      var selectedMutantsOfCurrType = selectedMutants.Where(m => m.Type == type);

      if (!selectedMutantsOfCurrType.Any())
      {
        return false;
      }

      if (mutantsOfCurrype.Count() == selectedMutantsOfCurrType.Count())
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
