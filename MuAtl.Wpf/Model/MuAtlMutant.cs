using MuAtl.Service.Reader.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuAtl.Model
{
  public class Zallel
  {
    public MutantType Operator { get; set; }
    public IEnumerable<MutationCandidate> Candidates { get; set;}
  }
  
  public enum MutantType
  {
    AAM, AFE, CEM, CST, CTT, DAM, DFE, DRS, DUS, L2M, M2L
  }

  public enum MutantStatus { Undetermined, Live, Dead, Equivalent }

  public class MuAtlMutant : MuAtlModelBase, IComparable<MuAtlMutant>
  {
    private MutantType mType;
    public MutantType Type
    {
      get
      {
        return mType;
      }
      set
      {
        mType = value;
        OnPropertyChanged("Type");
      }
    }

    private MutantStatus mStatus;
    public MutantStatus Status
    {
      get
      {
        return mStatus;
      }
      set
      {
        mStatus = value;
        OnPropertyChanged("Status");
      }
    }

    public override bool Equals(object obj)
    {
      var other = obj as MuAtlMutant;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }

    public int CompareTo(MuAtlMutant other)
    {
      return Name.CompareTo(other.Name);
    }
  }

  public class MutantSuite : List<MuAtlMutant>
  {
    public MutantSuite(IEnumerable<MuAtlMutant> mutants) 
      : base(mutants)
    {
    }

    public MutantSuite()
      : base()
    {
    }
  }
}
