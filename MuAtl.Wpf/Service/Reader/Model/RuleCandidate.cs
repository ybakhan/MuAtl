using System;

namespace MuAtl.Service.Reader.Model
{
  public abstract class RuleCandidate : MutationCandidate
  {
    public string Rule { get; set; }
  }
}
