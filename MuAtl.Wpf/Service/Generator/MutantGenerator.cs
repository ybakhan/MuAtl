using Antlr4.Runtime.Tree;
using MuAtl.Model;
using MuAtl.Service.Reader.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuAtl.Service.Generator
{
  public interface IMutantGenerator
  {
    void Generate(string mutantDir,
      MuAtlProject project,
      IEnumerable<MutationCandidate> candidates);
  }

  public class MutantGenerator : IMutantGenerator
  {
    public void Generate(string mutantDir, MuAtlProject project,
      IEnumerable<MutationCandidate> candidates)
    {
      var tokens = (new AtlTokenizer()).GetTokens(project.Module);
      var mutantStartIndices = new Dictionary<MutantType, int>();
      foreach (var mutantType in Enum.GetValues(typeof(MutantType)).Cast<MutantType>())
      {
        var startIndex = project.Mutants.Where(m => m.Type == mutantType).Count() + 1;
        mutantStartIndices.Add(mutantType, startIndex);
      }

      var listener = new MutantGenerationListener(tokens, mutantDir, project, mutantStartIndices);
      listener.M2lCandidates = candidates.OfType<M2lCandidate>();
      listener.L2mCandidates = candidates.OfType<L2mCandidate>();
      listener.DrsCandidates = candidates.OfType<DrsCandidate>();
      listener.DusCandidates = candidates.OfType<DusCandidate>();
      listener.DamCandidates = candidates.OfType<DamCandidate>();
      listener.DfeCandidates = candidates.OfType<DfeCandidate>();
      listener.CstCandidates = candidates.OfType<CstCandidate>();
      listener.CttCandidates = candidates.OfType<CttCandidate>();
      listener.IsCemCandidate = candidates.OfType<CemCandidate>().Any();
      listener.AamCandidates = candidates.OfType<AamCandidate>();
      listener.AfeCandidates = candidates.OfType<AfeCandidate>();

      var tree = (new AtlParser()).Parse(tokens);

      var walker = new ParseTreeWalker();
      walker.Walk(listener, tree);
    }
  }
}
