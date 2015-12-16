using System.Collections.ObjectModel;

namespace MuAtl.Service.Reader.Model
{
  public class AfeCandidate : RuleCandidate
  {
    public ObservableCollection<string> MuFilteringExpressions { get; set; }
  }
}
