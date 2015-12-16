using MuAtl.Model;
using MuAtl.ViewModel;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View
{
  public partial class ConfigView : MuAtlViewBase
  {
    public ConfigView()
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
        ((ConfigViewModel)DataContext).Project = project;
      }
    }
  }
}