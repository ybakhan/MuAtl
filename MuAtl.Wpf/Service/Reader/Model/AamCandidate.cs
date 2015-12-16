using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MuAtl.Service.Reader.Model
{
  public class AamCandidate : RuleCandidate
  {
    public string OutPattern { get; set; }
    public ObservableCollection<string> MuMappings { get; set; }
  }
}
