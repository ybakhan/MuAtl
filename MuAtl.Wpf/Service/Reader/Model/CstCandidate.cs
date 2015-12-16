using System.Collections.ObjectModel;
namespace MuAtl.Service.Reader.Model
{
  public class CstCandidate : RuleCandidate
  {
    public string SourceType { get; set; }
    public ObservableCollection<string> SrcTypes { get; set; }
    public ObservableCollection<string> MuSrcTypes { get; set; }
  }
}
