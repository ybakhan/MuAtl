/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MuAtl.Wpf.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MuAtl.Service;
using MuAtl.Service.Runner;
using MuAtl.Service.Generator;

namespace MuAtl.ViewModel
{
  public class ViewModelLocator
  {
    static ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      SimpleIoc.Default.Register<IMutantRunner, MutantRunner>();
      SimpleIoc.Default.Register<IDialogService, DialogService>();
      SimpleIoc.Default.Register<IRepository, Repository>();
      SimpleIoc.Default.Register<IResultExporter, ResultExporter>();
      SimpleIoc.Default.Register<IMutantGenerator, MutantGenerator>();
      SimpleIoc.Default.Register<IAtlParser, AtlParser>();
      SimpleIoc.Default.Register<IDiffer, Differ>();
      SimpleIoc.Default.Register<ILogService, LogService>();
      SimpleIoc.Default.Register<IOracle, Oracle>();

      SimpleIoc.Default.Register<TestSuiteViewModel>();
      SimpleIoc.Default.Register<ConfigViewModel>();
      SimpleIoc.Default.Register<MutantsViewModel>();
      SimpleIoc.Default.Register<RunTestsViewModel>();
      SimpleIoc.Default.Register<ProjectViewModel>();
      SimpleIoc.Default.Register<ResultsViewModel>();
    }

    public TestSuiteViewModel TestSuiteViewModel
    {
      get
      {
        return ServiceLocator.Current.GetInstance<TestSuiteViewModel>();
      }
    }

    public ConfigViewModel Dependencies
    {
      get
      {
        return ServiceLocator.Current.GetInstance<ConfigViewModel>();
      }
    }

    public MutantsViewModel MutantViewModel
    {
      get
      {
        return ServiceLocator.Current.GetInstance<MutantsViewModel>();
      }
    }

    public RunTestsViewModel MutantExecutionViewModel
    {
      get
      {
        return ServiceLocator.Current.GetInstance<RunTestsViewModel>();
      }
    }

    public ResultsViewModel ResultViewModel
    {
      get
      {
        return ServiceLocator.Current.GetInstance<ResultsViewModel>();
      }
    }

    public ProjectViewModel MuAtlProjectViewModel
    {
      get
      {
        return ServiceLocator.Current.GetInstance<ProjectViewModel>();
      }
    }

    /// <summary>
    /// Cleans up all the resources.
    /// </summary>
    public static void Cleanup()
    {
    }
  }
}