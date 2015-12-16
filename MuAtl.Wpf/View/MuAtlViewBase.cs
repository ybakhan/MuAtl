using MuAtl.Model;
using System.Windows;
using System.Windows.Controls;

namespace MuAtl.View
{
  public abstract class MuAtlViewBase : UserControl
  {
    public static readonly DependencyProperty ProjectProperty =
      DependencyProperty.Register(
        ProjectKey,
        typeof(MuAtlProject),
        typeof(MuAtlViewBase),
        new FrameworkPropertyMetadata(null, ProjectChanged));
    
    protected const string ProjectKey = "Project";

    public MuAtlProject Project
    {
      get
      {
        return (MuAtlProject)GetValue(ProjectProperty);
      }
      set
      {
        SetValue(ProjectProperty, value);
      }
    }

    private static void ProjectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      (obj as MuAtlViewBase).OnProjectChanged(e);
    }

    protected abstract void OnProjectChanged(DependencyPropertyChangedEventArgs e);
  }
}
