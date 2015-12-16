using GalaSoft.MvvmLight.CommandWpf;
using MuAtl.Service;

namespace MuAtl.ViewModel.Base
{
  public abstract class MutantViewModelBase : MuAtlViewModelBase
  {
    public RelayCommand<string> FindCmd { get; private set; }

    public MutantViewModelBase(IDialogService dlgService)
      : base(dlgService)
    {
      FindCmd = new RelayCommand<string>(FindItem);
    }

    protected abstract void FindItem(string searchParam);
  }
}
