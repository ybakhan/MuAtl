using System.Collections.ObjectModel;

namespace MuAtl.Service.Reader.Model
{
  public class CttCandidate : RuleCandidate
  {
    public string TargetType { get; set; }
    public ObservableCollection<string> TargetTypes { get; set; }
    public ObservableCollection<string> MuTargetTypes { get; set; }
  }
}
