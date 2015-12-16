using NUnit.Framework;
using MuAtl.Service;
using System.IO;
using MuAtl.Service.Reader;
using Antlr4.Runtime.Tree;
using System.Linq;
using MuAtl.Service.Reader.Model;

namespace MuAtl.Test.Service
{
  [TestFixture]
  public class MutantCandidateListenerTest
  {
    [Test]
    public void TestWalk_CandidatesRead()
    {
      var tokens = new AtlTokenizer().GetTokens(Path.GetFullPath(@"TestData\ucm2ad.atl"));
      var tree = new AtlParser().Parse(tokens);
      var srvc = new MuCandidateListener(tokens);
      var walker = new ParseTreeWalker();

      walker.Walk(srvc, tree);

      #region Candidates

      #region M2L

      var m2lCandidates = srvc.MutationCandidates.OfType<M2lCandidate>().Except(
         srvc.MutationCandidates.OfType<DrsCandidate>());
      Assert.AreEqual(1, m2lCandidates.Count());
      Assert.AreEqual("URNDefinition_To_UMLPackage", m2lCandidates.ElementAt(0).Rule);
      Assert.AreEqual(614, m2lCandidates.ElementAt(0).Line);

      #endregion

      #region L2M

      var l2mCandidates = srvc.MutationCandidates.OfType<L2mCandidate>();
      Assert.AreEqual(13, l2mCandidates.Count());

      Assert.AreEqual("StartPoint_To_InitialNodeX", l2mCandidates.ElementAt(0).Rule);
      Assert.AreEqual(49, l2mCandidates.ElementAt(0).Line);

      Assert.AreEqual("StartPoint_To_InitialNode", l2mCandidates.ElementAt(1).Rule);
      Assert.AreEqual(56, l2mCandidates.ElementAt(1).Line);

      Assert.AreEqual("StartPoint_To_InitialNode2", l2mCandidates.ElementAt(2).Rule);
      Assert.AreEqual(65, l2mCandidates.ElementAt(2).Line);

      Assert.AreEqual("EndPoint_To_FinalNode", l2mCandidates.ElementAt(3).Rule);
      Assert.AreEqual(76, l2mCandidates.ElementAt(3).Line);

      Assert.AreEqual("EndPoint_To_FinalNode2", l2mCandidates.ElementAt(4).Rule);
      Assert.AreEqual(85, l2mCandidates.ElementAt(4).Line);

      Assert.AreEqual("Responsibility_To_OpaqueAction", l2mCandidates.ElementAt(5).Rule);
      Assert.AreEqual(96, l2mCandidates.ElementAt(5).Line);

      Assert.AreEqual("Timer_To_OpaqueAction", l2mCandidates.ElementAt(6).Rule);
      Assert.AreEqual(105, l2mCandidates.ElementAt(6).Line);

      Assert.AreEqual("FailurePoint_To_OpaqueAction", l2mCandidates.ElementAt(7).Rule);
      Assert.AreEqual(114, l2mCandidates.ElementAt(7).Line);

      Assert.AreEqual("AndFork_To_ForkNode", l2mCandidates.ElementAt(8).Rule);
      Assert.AreEqual(123, l2mCandidates.ElementAt(8).Line);

      Assert.AreEqual("AndJoin_To_ForkNode", l2mCandidates.ElementAt(9).Rule);
      Assert.AreEqual(132, l2mCandidates.ElementAt(9).Line);

      Assert.AreEqual("ORFork_To_MergeNode", l2mCandidates.ElementAt(10).Rule);
      Assert.AreEqual(141, l2mCandidates.ElementAt(10).Line);

      Assert.AreEqual("ORJoin_To_MergeNode", l2mCandidates.ElementAt(11).Rule);
      Assert.AreEqual(150, l2mCandidates.ElementAt(11).Line);

      Assert.AreEqual("WaitingPlace_To_MergeNode", l2mCandidates.ElementAt(12).Rule);
      Assert.AreEqual(159, l2mCandidates.ElementAt(12).Line);

      #endregion

      #region DUS

      var dusCandidates = srvc.MutationCandidates.OfType<DusCandidate>();
      Assert.AreEqual(6, dusCandidates.Count());

      Assert.AreEqual("PathNode", dusCandidates.ElementAt(0).Library);
      Assert.AreEqual(4, dusCandidates.ElementAt(0).Line);

      Assert.AreEqual("Component", dusCandidates.ElementAt(1).Library);
      Assert.AreEqual(5, dusCandidates.ElementAt(1).Line);

      Assert.AreEqual("Stub", dusCandidates.ElementAt(2).Library);
      Assert.AreEqual(6, dusCandidates.ElementAt(2).Line);

      Assert.AreEqual("UCMmap", dusCandidates.ElementAt(3).Library);
      Assert.AreEqual(7, dusCandidates.ElementAt(3).Line);

      Assert.AreEqual("ComponentRef", dusCandidates.ElementAt(4).Library);
      Assert.AreEqual(8, dusCandidates.ElementAt(4).Line);

      Assert.AreEqual("NodeConnection", dusCandidates.ElementAt(5).Library);
      Assert.AreEqual(9, dusCandidates.ElementAt(5).Line);

      #endregion

      #region DRS

      var drsCandidates = srvc.MutationCandidates.OfType<DrsCandidate>();

      Assert.AreEqual(226, drsCandidates.ElementAt(0).Line);
      Assert.AreEqual("InitUmlEdge", drsCandidates.ElementAt(0).Rule);
      Assert.AreEqual("e;", drsCandidates.ElementAt(0).Statement);

      Assert.AreEqual(277, drsCandidates.ElementAt(1).Line);
      Assert.AreEqual("InitStaticStrAct", drsCandidates.ElementAt(1).Rule);
      Assert.AreEqual("a;", drsCandidates.ElementAt(1).Statement);

      Assert.AreEqual(281, drsCandidates.ElementAt(2).Line);
      Assert.AreEqual("InitStaticStrAct", drsCandidates.ElementAt(2).Rule);
      Assert.AreEqual("a;", drsCandidates.ElementAt(2).Statement);

      Assert.AreEqual(344, drsCandidates.ElementAt(3).Line);
      Assert.AreEqual("CreateDynStrAct", drsCandidates.ElementAt(3).Rule);
      Assert.AreEqual("a;", drsCandidates.ElementAt(3).Statement);

      Assert.AreEqual(348, drsCandidates.ElementAt(4).Line);
      Assert.AreEqual("CreateDynStrAct", drsCandidates.ElementAt(4).Rule);
      Assert.AreEqual("a;", drsCandidates.ElementAt(4).Statement);

      Assert.AreEqual(375, drsCandidates.ElementAt(5).Line);
      Assert.AreEqual("InitDecDynEdge", drsCandidates.ElementAt(5).Rule);
      Assert.AreEqual("e;", drsCandidates.ElementAt(5).Statement);

      Assert.AreEqual(387, drsCandidates.ElementAt(6).Line);
      Assert.AreEqual("InitDynEdge", drsCandidates.ElementAt(6).Rule);
      Assert.AreEqual("e;", drsCandidates.ElementAt(6).Statement);

      Assert.AreEqual(484, drsCandidates.ElementAt(7).Line);
      Assert.AreEqual("InitMergeNode", drsCandidates.ElementAt(7).Rule);
      Assert.AreEqual("n;", drsCandidates.ElementAt(7).Statement);

      Assert.AreEqual(501, drsCandidates.ElementAt(8).Line);
      Assert.AreEqual("InitDecisionNode", drsCandidates.ElementAt(8).Rule);
      Assert.AreEqual("n;", drsCandidates.ElementAt(8).Statement);

      Assert.AreEqual(539, drsCandidates.ElementAt(9).Line);
      Assert.AreEqual("InitUmlGroup", drsCandidates.ElementAt(9).Rule);
      Assert.AreEqual("a;", drsCandidates.ElementAt(9).Statement);

      Assert.AreEqual(543, drsCandidates.ElementAt(10).Line);
      Assert.AreEqual("InitUmlGroup", drsCandidates.ElementAt(10).Rule);
      Assert.AreEqual("a;", drsCandidates.ElementAt(10).Statement);

      Assert.AreEqual(11, drsCandidates.Count());

      #endregion

      #region CST

      var cstCandidates = srvc.MutationCandidates.OfType<CstCandidate>();

      Assert.AreEqual("StartPoint_To_InitialNodeX", cstCandidates.ElementAt(0).Rule);
      Assert.AreEqual("UCM!\"ucm::map::StartPoint\"", cstCandidates.ElementAt(0).SourceType);
      Assert.AreEqual(51, cstCandidates.ElementAt(0).Line);

      Assert.AreEqual("StartPoint_To_InitialNode", cstCandidates.ElementAt(1).Rule);
      Assert.AreEqual("UCM!\"ucm::map::StartPoint\"", cstCandidates.ElementAt(1).SourceType);
      Assert.AreEqual(58, cstCandidates.ElementAt(1).Line);

      Assert.AreEqual("StartPoint_To_InitialNode2", cstCandidates.ElementAt(2).Rule);
      Assert.AreEqual("UCM!\"ucm::map::StartPoint\"", cstCandidates.ElementAt(2).SourceType);
      Assert.AreEqual(67, cstCandidates.ElementAt(2).Line);

      Assert.AreEqual("EndPoint_To_FinalNode", cstCandidates.ElementAt(3).Rule);
      Assert.AreEqual("UCM!\"ucm::map::EndPoint\"", cstCandidates.ElementAt(3).SourceType);
      Assert.AreEqual(78, cstCandidates.ElementAt(3).Line);

      Assert.AreEqual("EndPoint_To_FinalNode2", cstCandidates.ElementAt(4).Rule);
      Assert.AreEqual("UCM!\"ucm::map::EndPoint\"", cstCandidates.ElementAt(4).SourceType);
      Assert.AreEqual(87, cstCandidates.ElementAt(4).Line);

      Assert.AreEqual("Responsibility_To_OpaqueAction", cstCandidates.ElementAt(5).Rule);
      Assert.AreEqual("UCM!\"ucm::map::RespRef\"", cstCandidates.ElementAt(5).SourceType);
      Assert.AreEqual(98, cstCandidates.ElementAt(5).Line);

      Assert.AreEqual("Timer_To_OpaqueAction", cstCandidates.ElementAt(6).Rule);
      Assert.AreEqual("UCM!\"ucm::map::Timer\"", cstCandidates.ElementAt(6).SourceType);
      Assert.AreEqual(107, cstCandidates.ElementAt(6).Line);

      Assert.AreEqual("FailurePoint_To_OpaqueAction", cstCandidates.ElementAt(7).Rule);
      Assert.AreEqual("UCM!\"ucm::map::FailurePoint\"", cstCandidates.ElementAt(7).SourceType);
      Assert.AreEqual(116, cstCandidates.ElementAt(7).Line);

      Assert.AreEqual("AndFork_To_ForkNode", cstCandidates.ElementAt(8).Rule);
      Assert.AreEqual("UCM!\"ucm::map::AndFork\"", cstCandidates.ElementAt(8).SourceType);
      Assert.AreEqual(125, cstCandidates.ElementAt(8).Line);

      Assert.AreEqual("AndJoin_To_ForkNode", cstCandidates.ElementAt(9).Rule);
      Assert.AreEqual("UCM!\"ucm::map::AndJoin\"", cstCandidates.ElementAt(9).SourceType);
      Assert.AreEqual(134, cstCandidates.ElementAt(9).Line);

      Assert.AreEqual("ORFork_To_MergeNode", cstCandidates.ElementAt(10).Rule);
      Assert.AreEqual("UCM!\"ucm::map::OrFork\"", cstCandidates.ElementAt(10).SourceType);
      Assert.AreEqual(143, cstCandidates.ElementAt(10).Line);

      Assert.AreEqual("ORJoin_To_MergeNode", cstCandidates.ElementAt(11).Rule);
      Assert.AreEqual("UCM!\"ucm::map::OrJoin\"", cstCandidates.ElementAt(11).SourceType);
      Assert.AreEqual(152, cstCandidates.ElementAt(11).Line);

      Assert.AreEqual("WaitingPlace_To_MergeNode", cstCandidates.ElementAt(12).Rule);
      Assert.AreEqual("UCM!\"ucm::map::WaitingPlace\"", cstCandidates.ElementAt(12).SourceType);
      Assert.AreEqual(161, cstCandidates.ElementAt(12).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", cstCandidates.ElementAt(13).Rule);
      Assert.AreEqual("UCM!\"urn::URNspec\"", cstCandidates.ElementAt(13).SourceType);
      Assert.AreEqual(616, cstCandidates.ElementAt(13).Line);

      Assert.AreEqual(14, cstCandidates.Count());

      #endregion

      #region CTT

      var cttCandidates = srvc.MutationCandidates.OfType<CttCandidate>();

      Assert.AreEqual("StartPoint_To_InitialNodeX", cttCandidates.ElementAt(0).Rule);
      Assert.AreEqual("UML!InitialNode", cttCandidates.ElementAt(0).TargetType);
      Assert.AreEqual(53, cttCandidates.ElementAt(0).Line);

      Assert.AreEqual("StartPoint_To_InitialNode", cttCandidates.ElementAt(1).Rule);
      Assert.AreEqual("UML!InitialNode", cttCandidates.ElementAt(1).TargetType);
      Assert.AreEqual(60, cttCandidates.ElementAt(1).Line);

      Assert.AreEqual("StartPoint_To_InitialNode2", cttCandidates.ElementAt(2).Rule);
      Assert.AreEqual("UML!InitialNode", cttCandidates.ElementAt(2).TargetType);
      Assert.AreEqual(71, cttCandidates.ElementAt(2).Line);

      Assert.AreEqual("EndPoint_To_FinalNode", cttCandidates.ElementAt(3).Rule);
      Assert.AreEqual("UML!ActivityFinalNode", cttCandidates.ElementAt(3).TargetType);
      Assert.AreEqual(80, cttCandidates.ElementAt(3).Line);

      Assert.AreEqual("EndPoint_To_FinalNode2", cttCandidates.ElementAt(4).Rule);
      Assert.AreEqual("UML!ActivityFinalNode", cttCandidates.ElementAt(4).TargetType);
      Assert.AreEqual(91, cttCandidates.ElementAt(4).Line);

      Assert.AreEqual("Responsibility_To_OpaqueAction", cttCandidates.ElementAt(5).Rule);
      Assert.AreEqual("UML!OpaqueAction", cttCandidates.ElementAt(5).TargetType);
      Assert.AreEqual(100, cttCandidates.ElementAt(5).Line);

      Assert.AreEqual("Timer_To_OpaqueAction", cttCandidates.ElementAt(6).Rule);
      Assert.AreEqual("UML!OpaqueAction", cttCandidates.ElementAt(6).TargetType);
      Assert.AreEqual(109, cttCandidates.ElementAt(6).Line);

      Assert.AreEqual("FailurePoint_To_OpaqueAction", cttCandidates.ElementAt(7).Rule);
      Assert.AreEqual("UML!OpaqueAction", cttCandidates.ElementAt(7).TargetType);
      Assert.AreEqual(118, cttCandidates.ElementAt(7).Line);

      Assert.AreEqual("AndFork_To_ForkNode", cttCandidates.ElementAt(8).Rule);
      Assert.AreEqual("UML!ForkNode", cttCandidates.ElementAt(8).TargetType);
      Assert.AreEqual(127, cttCandidates.ElementAt(8).Line);

      Assert.AreEqual("AndJoin_To_ForkNode", cttCandidates.ElementAt(9).Rule);
      Assert.AreEqual("UML!ForkNode", cttCandidates.ElementAt(9).TargetType);
      Assert.AreEqual(136, cttCandidates.ElementAt(9).Line);

      Assert.AreEqual("ORFork_To_MergeNode", cttCandidates.ElementAt(10).Rule);
      Assert.AreEqual("UML!MergeNode", cttCandidates.ElementAt(10).TargetType);
      Assert.AreEqual(145, cttCandidates.ElementAt(10).Line);

      Assert.AreEqual("ORJoin_To_MergeNode", cttCandidates.ElementAt(11).Rule);
      Assert.AreEqual("UML!MergeNode", cttCandidates.ElementAt(11).TargetType);
      Assert.AreEqual(154, cttCandidates.ElementAt(11).Line);

      Assert.AreEqual("WaitingPlace_To_MergeNode", cttCandidates.ElementAt(12).Rule);
      Assert.AreEqual("UML!MergeNode", cttCandidates.ElementAt(12).TargetType);
      Assert.AreEqual(163, cttCandidates.ElementAt(12).Line);

      Assert.AreEqual("InitUmlEdge", cttCandidates.ElementAt(13).Rule);
      Assert.AreEqual("UML!ControlFlow", cttCandidates.ElementAt(13).TargetType);
      Assert.AreEqual(219, cttCandidates.ElementAt(13).Line);

      Assert.AreEqual("InitStaticStrAct", cttCandidates.ElementAt(14).Rule);
      Assert.AreEqual("UML!StructuredActivityNode", cttCandidates.ElementAt(14).TargetType);
      Assert.AreEqual(267, cttCandidates.ElementAt(14).Line);

      Assert.AreEqual("CreateDynStrAct", cttCandidates.ElementAt(15).Rule);
      Assert.AreEqual("UML!StructuredActivityNode", cttCandidates.ElementAt(15).TargetType);
      Assert.AreEqual(329, cttCandidates.ElementAt(15).Line);

      Assert.AreEqual("InitDecDynEdge", cttCandidates.ElementAt(16).Rule);
      Assert.AreEqual("UML!ControlFlow", cttCandidates.ElementAt(16).TargetType);
      Assert.AreEqual(368, cttCandidates.ElementAt(16).Line);

      Assert.AreEqual("InitDynEdge", cttCandidates.ElementAt(17).Rule);
      Assert.AreEqual("UML!ControlFlow", cttCandidates.ElementAt(17).TargetType);
      Assert.AreEqual(381, cttCandidates.ElementAt(17).Line);

      Assert.AreEqual("InitMergeNode", cttCandidates.ElementAt(18).Rule);
      Assert.AreEqual("UML!MergeNode", cttCandidates.ElementAt(18).TargetType);
      Assert.AreEqual(478, cttCandidates.ElementAt(18).Line);

      Assert.AreEqual("InitDecisionNode", cttCandidates.ElementAt(19).Rule);
      Assert.AreEqual("UML!MergeNode", cttCandidates.ElementAt(19).TargetType);
      Assert.AreEqual(495, cttCandidates.ElementAt(19).Line);

      Assert.AreEqual("InitUmlGroup", cttCandidates.ElementAt(20).Rule);
      Assert.AreEqual("UML!ActivityPartition", cttCandidates.ElementAt(20).TargetType);
      Assert.AreEqual(528, cttCandidates.ElementAt(20).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", cttCandidates.ElementAt(21).Rule);
      Assert.AreEqual("UML!Package", cttCandidates.ElementAt(21).TargetType);
      Assert.AreEqual(618, cttCandidates.ElementAt(21).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", cttCandidates.ElementAt(22).Rule);
      Assert.AreEqual("UML!Activity", cttCandidates.ElementAt(22).TargetType);
      Assert.AreEqual(621, cttCandidates.ElementAt(22).Line);

      Assert.AreEqual(23, cttCandidates.Count());

      #endregion

      #region DFE

      var dfeCandidates = srvc.MutationCandidates.OfType<DfeCandidate>();

      Assert.AreEqual("StartPoint_To_InitialNode2", dfeCandidates.ElementAt(0).Rule);
      Assert.AreEqual("p.abc>1", dfeCandidates.ElementAt(0).Filtering);
      Assert.AreEqual(68, dfeCandidates.ElementAt(0).Line);

      Assert.AreEqual("EndPoint_To_FinalNode2", dfeCandidates.ElementAt(1).Rule);
      Assert.AreEqual("p.xyz>2", dfeCandidates.ElementAt(1).Filtering);
      Assert.AreEqual(88, dfeCandidates.ElementAt(1).Line);

      Assert.AreEqual(2, dfeCandidates.Count());

      #endregion

      #region AFE

      var afeCandidates = srvc.MutationCandidates.OfType<AfeCandidate>();

      Assert.AreEqual("StartPoint_To_InitialNodeX", afeCandidates.ElementAt(0).Rule);
      Assert.AreEqual(51, afeCandidates.ElementAt(0).Line); ;

      Assert.AreEqual("StartPoint_To_InitialNode", afeCandidates.ElementAt(1).Rule);
      Assert.AreEqual(58, afeCandidates.ElementAt(1).Line);

      Assert.AreEqual("EndPoint_To_FinalNode", afeCandidates.ElementAt(2).Rule);
      Assert.AreEqual(78, afeCandidates.ElementAt(2).Line);

      Assert.AreEqual("Responsibility_To_OpaqueAction", afeCandidates.ElementAt(3).Rule);
      Assert.AreEqual(98, afeCandidates.ElementAt(3).Line);

      Assert.AreEqual("Timer_To_OpaqueAction", afeCandidates.ElementAt(4).Rule);
      Assert.AreEqual(107, afeCandidates.ElementAt(4).Line);

      Assert.AreEqual("FailurePoint_To_OpaqueAction", afeCandidates.ElementAt(5).Rule);
      Assert.AreEqual(116, afeCandidates.ElementAt(5).Line);

      Assert.AreEqual("AndFork_To_ForkNode", afeCandidates.ElementAt(6).Rule);
      Assert.AreEqual(125, afeCandidates.ElementAt(6).Line);

      Assert.AreEqual("AndJoin_To_ForkNode", afeCandidates.ElementAt(7).Rule);
      Assert.AreEqual(134, afeCandidates.ElementAt(7).Line);

      Assert.AreEqual("ORFork_To_MergeNode", afeCandidates.ElementAt(8).Rule);
      Assert.AreEqual(143, afeCandidates.ElementAt(8).Line);

      Assert.AreEqual("ORJoin_To_MergeNode", afeCandidates.ElementAt(9).Rule);
      Assert.AreEqual(152, afeCandidates.ElementAt(9).Line);

      Assert.AreEqual("WaitingPlace_To_MergeNode", afeCandidates.ElementAt(10).Rule);
      Assert.AreEqual(161, afeCandidates.ElementAt(10).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", afeCandidates.ElementAt(11).Rule);
      Assert.AreEqual(616, afeCandidates.ElementAt(11).Line);

      Assert.AreEqual(12, afeCandidates.Count());

      #endregion

      #region AAM

      var aamCandidates = srvc.MutationCandidates.OfType<AamCandidate>();
      Assert.AreEqual("StartPoint_To_InitialNodeX", aamCandidates.ElementAt(0).Rule);
      Assert.AreEqual("UML!InitialNode", aamCandidates.ElementAt(0).OutPattern);
      Assert.AreEqual(53, aamCandidates.ElementAt(0).Line);

      Assert.AreEqual("StartPoint_To_InitialNode", aamCandidates.ElementAt(1).Rule);
      Assert.AreEqual("UML!InitialNode", aamCandidates.ElementAt(1).OutPattern);
      Assert.AreEqual(60, aamCandidates.ElementAt(1).Line);

      Assert.AreEqual("StartPoint_To_InitialNode2", aamCandidates.ElementAt(2).Rule);
      Assert.AreEqual("UML!InitialNode", aamCandidates.ElementAt(2).OutPattern);
      Assert.AreEqual(71, aamCandidates.ElementAt(2).Line);

      Assert.AreEqual("EndPoint_To_FinalNode", aamCandidates.ElementAt(3).Rule);
      Assert.AreEqual("UML!ActivityFinalNode", aamCandidates.ElementAt(3).OutPattern);
      Assert.AreEqual(80, aamCandidates.ElementAt(3).Line);

      Assert.AreEqual("EndPoint_To_FinalNode2", aamCandidates.ElementAt(4).Rule);
      Assert.AreEqual("UML!ActivityFinalNode", aamCandidates.ElementAt(4).OutPattern);
      Assert.AreEqual(91, aamCandidates.ElementAt(4).Line);

      Assert.AreEqual("Responsibility_To_OpaqueAction", aamCandidates.ElementAt(5).Rule);
      Assert.AreEqual("UML!OpaqueAction", aamCandidates.ElementAt(5).OutPattern);
      Assert.AreEqual(100, aamCandidates.ElementAt(5).Line);

      Assert.AreEqual("Timer_To_OpaqueAction", aamCandidates.ElementAt(6).Rule);
      Assert.AreEqual("UML!OpaqueAction", aamCandidates.ElementAt(6).OutPattern);
      Assert.AreEqual(109, aamCandidates.ElementAt(6).Line);

      Assert.AreEqual("FailurePoint_To_OpaqueAction", aamCandidates.ElementAt(7).Rule);
      Assert.AreEqual("UML!OpaqueAction", aamCandidates.ElementAt(7).OutPattern);
      Assert.AreEqual(118, aamCandidates.ElementAt(7).Line);

      Assert.AreEqual("AndFork_To_ForkNode", aamCandidates.ElementAt(8).Rule);
      Assert.AreEqual("UML!ForkNode", aamCandidates.ElementAt(8).OutPattern);
      Assert.AreEqual(127, aamCandidates.ElementAt(8).Line);

      Assert.AreEqual("AndJoin_To_ForkNode", aamCandidates.ElementAt(9).Rule);
      Assert.AreEqual("UML!ForkNode", aamCandidates.ElementAt(9).OutPattern);
      Assert.AreEqual(136, aamCandidates.ElementAt(9).Line);

      Assert.AreEqual("ORFork_To_MergeNode", aamCandidates.ElementAt(10).Rule);
      Assert.AreEqual("UML!MergeNode", aamCandidates.ElementAt(10).OutPattern);
      Assert.AreEqual(145, aamCandidates.ElementAt(10).Line);

      Assert.AreEqual("ORJoin_To_MergeNode", aamCandidates.ElementAt(11).Rule);
      Assert.AreEqual("UML!MergeNode", aamCandidates.ElementAt(11).OutPattern);
      Assert.AreEqual(154, aamCandidates.ElementAt(11).Line);

      Assert.AreEqual("WaitingPlace_To_MergeNode", aamCandidates.ElementAt(12).Rule);
      Assert.AreEqual("UML!MergeNode", aamCandidates.ElementAt(12).OutPattern);
      Assert.AreEqual(163, aamCandidates.ElementAt(12).Line);

      Assert.AreEqual("InitUmlEdge", aamCandidates.ElementAt(13).Rule);
      Assert.AreEqual("UML!ControlFlow", aamCandidates.ElementAt(13).OutPattern);
      Assert.AreEqual(219, aamCandidates.ElementAt(13).Line);

      Assert.AreEqual("InitStaticStrAct", aamCandidates.ElementAt(14).Rule);
      Assert.AreEqual("UML!StructuredActivityNode", aamCandidates.ElementAt(14).OutPattern);
      Assert.AreEqual(267, aamCandidates.ElementAt(14).Line);

      Assert.AreEqual("CreateDynStrAct", aamCandidates.ElementAt(15).Rule);
      Assert.AreEqual("UML!StructuredActivityNode", aamCandidates.ElementAt(15).OutPattern);
      Assert.AreEqual(329, aamCandidates.ElementAt(15).Line);

      Assert.AreEqual("InitDecDynEdge", aamCandidates.ElementAt(16).Rule);
      Assert.AreEqual("UML!ControlFlow", aamCandidates.ElementAt(16).OutPattern);
      Assert.AreEqual(368, aamCandidates.ElementAt(16).Line);

      Assert.AreEqual("InitDynEdge", aamCandidates.ElementAt(17).Rule);
      Assert.AreEqual("UML!ControlFlow", aamCandidates.ElementAt(17).OutPattern);
      Assert.AreEqual(381, aamCandidates.ElementAt(17).Line);

      Assert.AreEqual("InitMergeNode", aamCandidates.ElementAt(18).Rule);
      Assert.AreEqual("UML!MergeNode", aamCandidates.ElementAt(18).OutPattern);
      Assert.AreEqual(478, aamCandidates.ElementAt(18).Line);

      Assert.AreEqual("InitDecisionNode", aamCandidates.ElementAt(19).Rule);
      Assert.AreEqual("UML!MergeNode", aamCandidates.ElementAt(19).OutPattern);
      Assert.AreEqual(495, aamCandidates.ElementAt(19).Line);

      Assert.AreEqual("InitUmlGroup", aamCandidates.ElementAt(20).Rule);
      Assert.AreEqual("UML!ActivityPartition", aamCandidates.ElementAt(20).OutPattern);
      Assert.AreEqual(528, aamCandidates.ElementAt(20).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", aamCandidates.ElementAt(21).Rule);
      Assert.AreEqual("UML!Package", aamCandidates.ElementAt(21).OutPattern);
      Assert.AreEqual(618, aamCandidates.ElementAt(21).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", aamCandidates.ElementAt(22).Rule);
      Assert.AreEqual("UML!Activity", aamCandidates.ElementAt(22).OutPattern);
      Assert.AreEqual(621, aamCandidates.ElementAt(22).Line);

      Assert.AreEqual(23, aamCandidates.Count());

      #endregion

      #region DAM

      var damCandidates = srvc.MutationCandidates.OfType<DamCandidate>();
      Assert.AreEqual("StartPoint_To_InitialNode", damCandidates.ElementAt(0).Rule);
      Assert.AreEqual(61, damCandidates.ElementAt(0).Line);

      Assert.AreEqual("StartPoint_To_InitialNode2", damCandidates.ElementAt(1).Rule);
      Assert.AreEqual("name<-p.name", damCandidates.ElementAt(1).Mapping);
      Assert.AreEqual(72, damCandidates.ElementAt(1).Line);

      Assert.AreEqual("EndPoint_To_FinalNode", damCandidates.ElementAt(2).Rule);
      Assert.AreEqual("name<-p.name", damCandidates.ElementAt(2).Mapping);
      Assert.AreEqual(81, damCandidates.ElementAt(2).Line);

      Assert.AreEqual("EndPoint_To_FinalNode2", damCandidates.ElementAt(3).Rule);
      Assert.AreEqual("name<-p.name", damCandidates.ElementAt(3).Mapping);
      Assert.AreEqual(92, damCandidates.ElementAt(3).Line);

      Assert.AreEqual("Responsibility_To_OpaqueAction", damCandidates.ElementAt(4).Rule);
      Assert.AreEqual("name<-r.respDef.name", damCandidates.ElementAt(4).Mapping);
      Assert.AreEqual(101, damCandidates.ElementAt(4).Line);

      Assert.AreEqual("Timer_To_OpaqueAction", damCandidates.ElementAt(5).Rule);
      Assert.AreEqual("name<-t.name+' (No Action)'", damCandidates.ElementAt(5).Mapping);
      Assert.AreEqual(110, damCandidates.ElementAt(5).Line);

      Assert.AreEqual("FailurePoint_To_OpaqueAction", damCandidates.ElementAt(6).Rule);
      Assert.AreEqual("name<-f.name+' (Handle Failure)'", damCandidates.ElementAt(6).Mapping);
      Assert.AreEqual(119, damCandidates.ElementAt(6).Line);

      Assert.AreEqual("AndFork_To_ForkNode", damCandidates.ElementAt(7).Rule);
      Assert.AreEqual("name<-f.name+' (Fork)'", damCandidates.ElementAt(7).Mapping);
      Assert.AreEqual(128, damCandidates.ElementAt(7).Line);

      Assert.AreEqual("AndJoin_To_ForkNode", damCandidates.ElementAt(8).Rule);
      Assert.AreEqual("name<-f.name+' (Join)'", damCandidates.ElementAt(8).Mapping);
      Assert.AreEqual(137, damCandidates.ElementAt(8).Line);

      Assert.AreEqual("ORFork_To_MergeNode", damCandidates.ElementAt(9).Rule);
      Assert.AreEqual("name<-o.name+' (Decision)'", damCandidates.ElementAt(9).Mapping);
      Assert.AreEqual(146, damCandidates.ElementAt(9).Line);

      Assert.AreEqual("ORJoin_To_MergeNode", damCandidates.ElementAt(10).Rule);
      Assert.AreEqual("name<-o.name+' (Merge)'", damCandidates.ElementAt(10).Mapping);
      Assert.AreEqual(155, damCandidates.ElementAt(10).Line);

      Assert.AreEqual("WaitingPlace_To_MergeNode", damCandidates.ElementAt(11).Rule);
      Assert.AreEqual("name<-w.name+' (Wait)'", damCandidates.ElementAt(11).Mapping);
      Assert.AreEqual(164, damCandidates.ElementAt(11).Line);

      Assert.AreEqual("InitUmlEdge", damCandidates.ElementAt(12).Rule);
      Assert.AreEqual("source<-umlSource", damCandidates.ElementAt(12).Mapping);
      Assert.AreEqual(220, damCandidates.ElementAt(12).Line);

      Assert.AreEqual("InitUmlEdge", damCandidates.ElementAt(13).Rule);
      Assert.AreEqual("target<-umlTarget", damCandidates.ElementAt(13).Mapping);
      Assert.AreEqual(221, damCandidates.ElementAt(13).Line);

      Assert.AreEqual("InitUmlEdge", damCandidates.ElementAt(14).Rule);
      Assert.AreEqual("name<-label", damCandidates.ElementAt(14).Mapping);
      Assert.AreEqual(222, damCandidates.ElementAt(14).Line);

      Assert.AreEqual("InitStaticStrAct", damCandidates.ElementAt(15).Rule);
      Assert.AreEqual("incoming<-source.getUmlEdge()", damCandidates.ElementAt(15).Mapping);
      Assert.AreEqual(268, damCandidates.ElementAt(15).Line);

      Assert.AreEqual("InitStaticStrAct", damCandidates.ElementAt(16).Rule);
      Assert.AreEqual("outgoing<-target.getUmlEdge()", damCandidates.ElementAt(16).Mapping);
      Assert.AreEqual(269, damCandidates.ElementAt(16).Line);

      Assert.AreEqual("InitStaticStrAct", damCandidates.ElementAt(17).Rule);
      Assert.AreEqual("node<-map.getNodes()", damCandidates.ElementAt(17).Mapping);
      Assert.AreEqual(270, damCandidates.ElementAt(17).Line);

      Assert.AreEqual("InitStaticStrAct", damCandidates.ElementAt(18).Rule);
      Assert.AreEqual("edge<-map.getEdges()", damCandidates.ElementAt(18).Mapping);
      Assert.AreEqual(271, damCandidates.ElementAt(18).Line);

      Assert.AreEqual("InitStaticStrAct", damCandidates.ElementAt(19).Rule);
      Assert.AreEqual("name<-stub.name", damCandidates.ElementAt(19).Mapping);
      Assert.AreEqual(272, damCandidates.ElementAt(19).Line);

      Assert.AreEqual("CreateDynStrAct", damCandidates.ElementAt(20).Rule);
      Assert.AreEqual("node<-map.getNodes()", damCandidates.ElementAt(20).Mapping);
      Assert.AreEqual(330, damCandidates.ElementAt(20).Line);

      Assert.AreEqual("CreateDynStrAct", damCandidates.ElementAt(21).Rule);
      Assert.AreEqual("edge<-map.getEdges()", damCandidates.ElementAt(21).Mapping);
      Assert.AreEqual(331, damCandidates.ElementAt(21).Line);

      Assert.AreEqual("CreateDynStrAct", damCandidates.ElementAt(22).Rule);
      Assert.AreEqual("name<-map.name", damCandidates.ElementAt(22).Mapping);
      Assert.AreEqual(332, damCandidates.ElementAt(22).Line);

      Assert.AreEqual("InitDecDynEdge", damCandidates.ElementAt(23).Rule);
      Assert.AreEqual("source<-s", damCandidates.ElementAt(23).Mapping);
      Assert.AreEqual(369, damCandidates.ElementAt(23).Line);

      Assert.AreEqual("InitDecDynEdge", damCandidates.ElementAt(24).Rule);
      Assert.AreEqual("target<-t", damCandidates.ElementAt(24).Mapping);
      Assert.AreEqual(370, damCandidates.ElementAt(24).Line);

      Assert.AreEqual("InitDecDynEdge", damCandidates.ElementAt(25).Rule);
      Assert.AreEqual("name<-condition", damCandidates.ElementAt(25).Mapping);
      Assert.AreEqual(371, damCandidates.ElementAt(25).Line);

      Assert.AreEqual("InitDynEdge", damCandidates.ElementAt(26).Rule);
      Assert.AreEqual("source<-s", damCandidates.ElementAt(26).Mapping);
      Assert.AreEqual(382, damCandidates.ElementAt(26).Line);

      Assert.AreEqual("InitDynEdge", damCandidates.ElementAt(27).Rule);
      Assert.AreEqual("target<-t", damCandidates.ElementAt(27).Mapping);
      Assert.AreEqual(383, damCandidates.ElementAt(27).Line);

      Assert.AreEqual("InitMergeNode", damCandidates.ElementAt(28).Rule);
      Assert.AreEqual("name<-label", damCandidates.ElementAt(28).Mapping);
      Assert.AreEqual(479, damCandidates.ElementAt(28).Line);

      Assert.AreEqual("InitMergeNode", damCandidates.ElementAt(29).Rule);
      Assert.AreEqual("outgoing<-outgoingEdge", damCandidates.ElementAt(29).Mapping);
      Assert.AreEqual(480, damCandidates.ElementAt(29).Line);

      Assert.AreEqual("InitDecisionNode", damCandidates.ElementAt(30).Rule);
      Assert.AreEqual("incoming<-incomingEdge", damCandidates.ElementAt(30).Mapping);
      Assert.AreEqual(496, damCandidates.ElementAt(30).Line);

      Assert.AreEqual("InitDecisionNode", damCandidates.ElementAt(31).Rule);
      Assert.AreEqual("name<-label", damCandidates.ElementAt(31).Mapping);
      Assert.AreEqual(497, damCandidates.ElementAt(31).Line);

      Assert.AreEqual("InitUmlGroup", damCandidates.ElementAt(32).Rule);
      Assert.AreEqual("name<-groupName", damCandidates.ElementAt(32).Mapping);
      Assert.AreEqual(529, damCandidates.ElementAt(32).Line);

      Assert.AreEqual("InitUmlGroup", damCandidates.ElementAt(33).Rule);
      Assert.AreEqual("node<-groupNodes", damCandidates.ElementAt(33).Mapping);
      Assert.AreEqual(530, damCandidates.ElementAt(33).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", damCandidates.ElementAt(34).Rule);
      Assert.AreEqual("packagedElement<-a", damCandidates.ElementAt(34).Mapping);
      Assert.AreEqual(619, damCandidates.ElementAt(34).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", damCandidates.ElementAt(35).Rule);
      Assert.AreEqual("name<-thisModule.rootUCM.name", damCandidates.ElementAt(35).Mapping);
      Assert.AreEqual(622, damCandidates.ElementAt(35).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", damCandidates.ElementAt(36).Rule);
      Assert.AreEqual("node<-thisModule.umlNodes", damCandidates.ElementAt(36).Mapping);
      Assert.AreEqual(623, damCandidates.ElementAt(36).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", damCandidates.ElementAt(37).Rule);
      Assert.AreEqual("edge<-thisModule.umlEdges", damCandidates.ElementAt(37).Mapping);
      Assert.AreEqual(624, damCandidates.ElementAt(37).Line);

      Assert.AreEqual("URNDefinition_To_UMLPackage", damCandidates.ElementAt(38).Rule);
      Assert.AreEqual("group<-thisModule.umlGroups", damCandidates.ElementAt(38).Mapping);
      Assert.AreEqual(625, damCandidates.ElementAt(38).Line);

      Assert.AreEqual(39, damCandidates.Count());

      #endregion

      Assert.AreEqual(144, srvc.MutationCandidates.Count()); 

      #endregion

      #region Source Types

      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::StartPoint\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::EndPoint\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::RespRef\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::Timer\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::FailurePoint\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::AndFork\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::AndJoin\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::OrFork\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::OrJoin\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"ucm::map::WaitingPlace\""));
      Assert.IsTrue(srvc.SrcTypes.Contains("UCM!\"urn::URNspec\""));
      Assert.AreEqual(11, srvc.SrcTypes.Count());

      #endregion

      #region Target Types

      Assert.IsTrue(srvc.TargetTypes.Contains("UML!InitialNode"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!ActivityFinalNode"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!OpaqueAction"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!ForkNode"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!MergeNode"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!ControlFlow"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!StructuredActivityNode"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!ActivityPartition"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!Package"));
      Assert.IsTrue(srvc.TargetTypes.Contains("UML!Activity"));
      Assert.AreEqual(10, srvc.TargetTypes.Count()); 

      #endregion
    }
  }
}
