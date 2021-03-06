﻿using MuAtl.Model;
using MuAtl.ViewModel;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View
{
  public partial class ResultsView : MuAtlViewBase
  {
    public ResultsView()
    {
      InitializeComponent();

      SetBinding(ProjectProperty,
       new Binding(ProjectKey)
       {
         Mode = BindingMode.TwoWay
       });    
    }

    protected override void OnProjectChanged(DependencyPropertyChangedEventArgs e)
    {
      var project = e.NewValue as MuAtlProject;
      if (project != null)
      {
        ((ResultsViewModel)DataContext).Project = project;
      }
    }
  }
}