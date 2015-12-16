using System.Windows;
using System.Collections.ObjectModel;
using MuAtl.Model;
using System.Windows.Data;
using MuAtl.ViewModel;

namespace MuAtl.View
{
  public partial class TestSuiteView : MuAtlViewBase
  {
    public TestSuiteView()
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
        ((TestSuiteViewModel)DataContext).Project = project;
      }
    }
  }
}
