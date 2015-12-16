using System;

namespace MuAtl.Service.Reader.Model
{
  public abstract class MutationCandidate : IComparable<MutationCandidate>
  {
    public int Line { get; set; }

    public int CompareTo(MutationCandidate other)
    {
      return Line.CompareTo(other.Line);
    }
  }

  public class CemCandidate : MutationCandidate
  {
  }
}
