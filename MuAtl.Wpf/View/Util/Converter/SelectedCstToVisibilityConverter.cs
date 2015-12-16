﻿using MuAtl.Service.Reader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View.Util.Converter
{
  public class SelectedCstToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("The target must be a count");

      if ((value as CstCandidate) == null)
        return Visibility.Hidden;

      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
