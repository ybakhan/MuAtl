using GalaSoft.MvvmLight;
using MuAtl.Model;
using MuAtl.Service;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Collections;

namespace MuAtl.ViewModel.Base
{
  public abstract class MuAtlViewModelBase : ViewModelBase, INotifyDataErrorInfo
  {
    protected readonly IDialogService mDlgService;
    protected readonly Dictionary<string, ICollection<string>> mValidationErrors = new Dictionary<string, ICollection<string>>();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public MuAtlViewModelBase(IDialogService dlgService)
    {
      mDlgService = dlgService;
    }

    protected const string ProjectKey = "Project";
    private MuAtlProject mProject;

    public virtual MuAtlProject Project
    {
      get
      {
        return mProject;
      }
      set
      {
        mProject = value;
        RaisePropertyChanged(ProjectKey);
      }
    }

    public IEnumerable GetErrors(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName)
            || !mValidationErrors.ContainsKey(propertyName))
        return null;

      return mValidationErrors[propertyName];
    }

    public bool HasErrors
    {
      get { return mValidationErrors.Count > 0; }
    }

    protected void RaiseErrorsChanged(string propertyName)
    {
      if (ErrorsChanged != null)
        ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateProperty(string property, string propertyKey, string errorMessage)
    {
      if (string.IsNullOrEmpty(property))
      {
        var validationErrors = new List<string>();
        validationErrors.Add(errorMessage);

        mValidationErrors[propertyKey] = validationErrors;
        RaiseErrorsChanged(propertyKey);
      }
      else if (mValidationErrors.ContainsKey(propertyKey))
      {
        mValidationErrors.Remove(propertyKey);
        RaiseErrorsChanged(propertyKey);
      }
    }

    protected void ClearProp(string propertyKey)
    {
      mValidationErrors.Remove(propertyKey);
      RaiseErrorsChanged(propertyKey);
    }
  }
}
