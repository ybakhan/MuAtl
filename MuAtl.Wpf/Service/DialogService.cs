using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace MuAtl.Service
{
  public interface IDialogService
  {
    string BrowseModule();
    string BrowseAtl(string title);
    string Browse(string title, string filter);
    string BrowseMetamodel(string title);
    string BrowseModel(string title);
    string BrowseXml(string title);
    string BrowseFolder(string description);
    string SaveXml(string title);
    string Save(string title, string filter);
    bool YesNoDialog(string message, string caption);
    void Error(string message, string caption);
  }
  
  public class DialogService : IDialogService
  {
    private const string XmlFilter = "XML (.xml)|*.xml";
    private const string DependencyTitle = "ATL dependency file";
    private const string TestSuiteTitle = "MuATL test suite";
    private const string ModelFilter = "XMI (.xmi)|*.xmi|UML (.uml)|*.uml|JUCM (.jucm)|*.jucm|All Files (*.*)|*.*";
    private const string AtlFilter = "ATL Files (.atl)|*.atl";
    private const string MetamodelFilter = "Ecore (.ecore)|*.ecore|XMI (.xmi)|*.xmi";

    public DialogService()
    {
    }

    public string BrowseModule()
    {
      return BrowseAtl("Module");
    }

    public string BrowseAtl(string title)
    {
      return Browse(title, AtlFilter);
    }

    public string Browse(string title, string filter)
    {
      var dialog = new OpenFileDialog
      {
        Title = title,
        Filter = filter,
        InitialDirectory = App.ProjectDir
      };

      if (dialog.ShowDialog() == true)
      {
        return dialog.FileName;
      }

      return string.Empty;
    }

    public string BrowseModel(string title)
    {
      return Browse(DependencyTitle, ModelFilter);
    }

    public string BrowseMetamodel(string title)
    {
      return Browse(title, MetamodelFilter); 
    }

    public string BrowseXml(string title)
    {
      return Browse(title, XmlFilter);
    }

    public string SaveXml(string title)
    {
      return Save(title, XmlFilter);
    }

    public string Save(string title, string filter)
    {
      var dialog = new SaveFileDialog
      {
        Filter = filter,
        Title = title,
        InitialDirectory = App.ProjectDir
      };

      if (dialog.ShowDialog() == true)
      {
        return dialog.FileName;
      }

      return string.Empty;
    }

    public bool YesNoDialog(string message, string caption)
    {
      var dlgResult = MessageBox.Show(message, caption, System.Windows.MessageBoxButton.YesNo);
      return dlgResult == System.Windows.MessageBoxResult.Yes;
    }

    public void Error(string message, string caption)
    {
      MessageBox.Show(
          message,
          caption,
          System.Windows.MessageBoxButton.OK,
          System.Windows.MessageBoxImage.Error);
    }

    public string BrowseFolder(string description)
    {
      var fbd = new System.Windows.Forms.FolderBrowserDialog
      {
        Description = description,
        ShowNewFolderButton = true
      };

      if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        return fbd.SelectedPath;
      }
      return string.Empty;
    }
  }
}
