using System.ComponentModel;

namespace MuAtl.Model
{
  public class MuAtlInput : MuAtlModelBase
  {
    public override bool Equals(object obj)
    {
      var other = obj as MuAtlInput;
      if (other == null)
        return false;

      return Name.Equals(other.Name);
    }
  }
}
