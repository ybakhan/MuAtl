using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using MuAtl.Model;
using MuAtl.Service;
using MuAtl.ViewModel.Base;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MuAtl.ViewModel
{
  public class ProjectViewModel : MuAtlViewModelBase
  {
    #region constants

    private static readonly ILog logger = LogManager.GetLogger(typeof(ProjectViewModel));

    public const string SelectModuleErrorMsg = "Select ATL module";
    public const string SaveProjCaption = "Save Project";
    public const string SaveAsProjCaption = "Save As Project";
    public const string SaveProjMsg = "Save MuATL project";
    public const string SaveAsProjMsg = "Save As MuATL project";
    public const string ConfirmOpenProjMsg = "Are you sure? Running tests will be aborted?";
    public const string OpenProjCaption = "Open Project";
    private const string ConfirmExitProjectCaption = "Exit Project";

    #endregion

    #region instance vars

    private readonly IRepository mRepository;
    public string ProjectPath { get; set; }
    private bool mProjectPersistent; 

    #endregion

    #region cmds

    public RelayCommand NewCmd { get; private set; }
    public RelayCommand OpenCmd { get; private set; }
    public RelayCommand SaveCmd { get; private set; }
    public RelayCommand SaveAsCmd { get; private set; }
    public RelayCommand ExitCmd { get; private set; }

    #endregion

    public ProjectViewModel(IDialogService dlgService, IRepository repository)
      : base(dlgService)
    {
      mRepository = repository;

      #region CMDS

      NewCmd = new RelayCommand(NewProject);
      OpenCmd = new RelayCommand(OpenProject);
      SaveCmd = new RelayCommand(Save);
      SaveAsCmd = new RelayCommand(SaveAs);
      ExitCmd = new RelayCommand(Exit);

      #endregion

      Project = new MuAtlProject();
      Messenger.Default.Register<int>(this, "NewProject", NewProject);
    }

    #region Save Project

    private void SaveAs()
    {
      if (string.IsNullOrEmpty(Project.Module))
      {
        mDlgService.Error(SelectModuleErrorMsg, SaveAsProjCaption);
        return;
      }

      if (!File.Exists(Project.Module))
      {
        mDlgService.Error(string.Format("Selectd ATL module '{0}' does not exist", Project.Module), SaveAsProjCaption);
        return;
      }

      if (!CheckConfig())
        return;

      var path = mDlgService.SaveXml(SaveAsProjMsg);
      if (string.IsNullOrEmpty(path))
        return;

      if (!Save(path))
        return;

      ProjectPath = path;
      if (!mProjectPersistent) //if project saved first time
      {
        mProjectPersistent = true;
        Messenger.Default.Send(mProjectPersistent, "ProjectPersistent");
      }
    }

    private void Save()
    {
      if (string.IsNullOrEmpty(Project.Module))
      {
        mDlgService.Error(SelectModuleErrorMsg, SaveProjCaption);
        return;
      }

      if (!File.Exists(Project.Module))
      {
        mDlgService.Error(string.Format("Selectd ATL module '{0}' does not exist", Project.Module), SaveProjCaption);
        return;
      }

      if (!CheckConfig())
        return;

      if (string.IsNullOrEmpty(ProjectPath)) //project saved first time;
      {
        var path = mDlgService.SaveXml(SaveProjMsg);
        if (string.IsNullOrEmpty(path))
          return;

        if (Save(path))
        {
          ProjectPath = path;

          mProjectPersistent = true; //project is persistent now
          Messenger.Default.Send(mProjectPersistent, "ProjectPersistent");
        }

        return;
      }

      Save(ProjectPath); //project saved 2nd time and beyond
    }

    private bool CheckConfig()
    {
      var errMsg = new StringBuilder();

      foreach (var inMm in Project.Dependency.InMetamodels.Where(m => string.IsNullOrEmpty(m.Path)))
      {
        errMsg.AppendFormat("Enter path of input metamodel {0}\n", inMm.Name);
      }

      foreach (var inMm in Project.Dependency.InMetamodels.Where(m => !string.IsNullOrEmpty(m.Path) && !File.Exists(m.Path)))
      {
        errMsg.AppendFormat("Input metamodel {0} does not exist\n", inMm.Path);
      }

      if (string.IsNullOrEmpty(Project.Dependency.OutMetamodel.Path))
      {
        errMsg.AppendFormat("Enter path of output metamodel {0}\n", Project.Dependency.OutMetamodel.Name);
      }
      else if(!File.Exists(Project.Dependency.OutMetamodel.Path))
      {
        errMsg.AppendFormat("Output metamodel {0} does not exist\n", Project.Dependency.OutMetamodel.Path);
      }

      foreach (var lib in Project.Dependency.Libraries.Where(l => string.IsNullOrEmpty(l.Path)))
      {
        errMsg.AppendFormat("Enter path of ATL library module {0}\n", lib.Name);
      }

      foreach (var lib in Project.Dependency.Libraries.Where(l => !string.IsNullOrEmpty(l.Path) && !File.Exists(l.Path)))
      {
        errMsg.AppendFormat("ATL library module {0} does not exist\n", lib.Path);
      }

      if (errMsg.ToString() != string.Empty)
      {
        mDlgService.Error(errMsg.ToString(), SaveProjCaption);
        return false;
      }

      return true;
    }

    private bool Save(string path)
    {
      try
      {
        Project.Name = Path.GetFileNameWithoutExtension(path);
        mRepository.Save(path, Project);
        logger.InfoFormat("MuATL project {0} saved at destination {1}", Project.Name, path);
        return true;
      }
      catch (Exception ex)
      {
        mDlgService.Error(
          string.Format("Exception '{0}' occured while saving MuATL project {1} at destination {2}", ex.Message, Project.Name, path),
          SaveProjCaption);
        return false;
      }
    }

    #endregion

    #region Open Project

    private void OpenProject()
    {
      if (!Project.IsRunning)
      {
        OpenProjectInternal();
        return;
      }

      var dlg = mDlgService.YesNoDialog(ConfirmOpenProjMsg, OpenProjCaption);
      if (dlg)
      {
        OpenProjectInternal();
      }
    }

    private void OpenProjectInternal()
    {
      Project.Clear();
      Messenger.Default.Send(0, "ClearCandidates");
      LoadProject();
    }

    public const string OpenProjectMsg = "Load MuATL project";

    private void LoadProject()
    {
      var path = mDlgService.BrowseXml(OpenProjectMsg);
      if (string.IsNullOrEmpty(path))
        return;

      try
      {
        Project = mRepository.Load<MuAtlProject>(path);
        ProjectPath = path;
        mProjectPersistent = true;
        Messenger.Default.Send(mProjectPersistent, "ProjectPersistent");

        logger.InfoFormat("MuATL project {0} at destination {1} opened", Project.Name, ProjectPath);
      }
      catch (Exception ex)
      {
        mDlgService.Error(ex.Message, "Error loading project");
      }
    }

    #endregion

    #region New Project

    private void NewProject(int param)
    {
      NewProject();
    }

    public const string NewProjCaption = "New Project";
    public const string ConfirmNewProj = "Are you sure? Running tests will be aborted?";

    private void NewProject()
    {
      if (!Project.IsRunning)
      {
        NewProjectInternal();
        return;
      }

      var dlg = mDlgService.YesNoDialog(ConfirmNewProj, NewProjCaption);
      if (dlg)
      {
        NewProjectInternal();
      }
    }

    private void NewProjectInternal()
    {
      Messenger.Default.Send(0, "ClearCandidates");
      Project.Clear();

      ProjectPath = string.Empty;
      mProjectPersistent = false;
      Messenger.Default.Send(mProjectPersistent, "ProjectPersistent");
    }

    #endregion

    private void Exit()
    {
      if (!Project.IsRunning)
      {
        ExitInternal();
      }

      var dlg = mDlgService.YesNoDialog(ConfirmOpenProjMsg, ConfirmExitProjectCaption);
      if (dlg)
      {
        ExitInternal();
      }
    }

    private void ExitInternal()
    {
      try
      {
        Save();
      }
      catch
      {
      }
      finally
      {
        System.Windows.Application.Current.MainWindow.Close();
      }
    }
  }
}