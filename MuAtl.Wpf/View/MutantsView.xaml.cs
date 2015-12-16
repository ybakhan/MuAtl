using MuAtl.Model;
using MuAtl.ViewModel;
using System.Windows;
using System.Windows.Data;

namespace MuAtl.View
{
  public partial class MutantsView : MuAtlViewBase
  {
    public MutantsView()
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
        ((MutantsViewModel)DataContext).Project = project;
      }
    }
  }
}