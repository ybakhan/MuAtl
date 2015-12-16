using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Threading;

namespace MuAtl
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    static App()
    {
      DispatcherHelper.Initialize();
    }

    public App()
    {
    }

    public static string ProjectDir
    {
      get
      {
        return @"I:\yak\Dropbox\Eclipse\workspaces\masterproj\UCM_2_AD";
      }
    }
  }
}
