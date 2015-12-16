using GalaSoft.MvvmLight.CommandWpf;
using MuAtl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using MuAtl.ViewModel.Base;
using MuAtl.Service;
using MuAtl.ViewModel.Util;
using MuAtl.Service.Generator;
using MuAtl.Service.Reader;
using System.Collections.ObjectModel;
using MuAtl.Service.Reader.Model;
using Antlr4.Runtime.Tree;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using System.IO;

namespace MuAtl.ViewModel
{
  public class MutantsViewModel : MutantViewModelBase
  {
    #region constants

    private const string MutantNameKey = "MutantName";
    private const string MutantPathKey = "MutantPath";
    private const string MuDirKey = "MuDir";
    private const string MatchedRulesKey = "MatchedRules";
    private const string LazyRulesKey = "LazyRules";
    private const string DrsRulesKey = "DrsRules";
    private const string LibrariesKey = "Libraries";

    #endregion

    #region instance vars

    private IMutantGenerator mMutantGenerator;
    private IAtlParser mParser;
    private IDiffer mDiffer;

    #endregion

    #region cmds

    public RelayCommand AddMutantCmd { get; private set; }
    public RelayCommand DelMutantCmd { get; private set; }
    public RelayCommand UpdateMutantCmd { get; private set; }
    public RelayCommand<object> MutantSlctCmd { get; private set; }

    public RelayCommand SelectMutantFileCmd { get; private set; }

    public RelayCommand SelectMuDirCmd { get; private set; }
    public RelayCommand GenerateCmd { get; private set; }

    public RelayCommand ChkStatusCmd { get; private set; }
    public RelayCommand DiffCmd { get; private set; }

    public RelayCommand SelectAllMatchedRulesCmd { get; private set; }
    public RelayCommand UnSelectAllMatchedRulesCmd { get; private set; }
    public RelayCommand SelectAllLazyRulesCmd { get; private set; }
    public RelayCommand UnSelectAllLazyRulesCmd { get; private set; }
    public RelayCommand SelectAllCalledRulesCmd { get; private set; }
    public RelayCommand UnSelectAllCalledRulesCmd { get; private set; }
    public RelayCommand SelectAllLibrariesRulesCmd { get; private set; }
    public RelayCommand UnSelectAllLibrariesRulesCmd { get; private set; }
    public RelayCommand SelectAllDamCandidatesCmd { get; private set; }
    public RelayCommand UnSelectAllDamCandidatesCmd { get; private set; }
    public RelayCommand SelectAllDfeCandidatesCmd { get; private set; }
    public RelayCommand UnSelectAllDfeCandidatesCmd { get; private set; }

    public RelayCommand<string> AddSrcTypeCmd { get; private set; }
    public RelayCommand DelSrcTypeCmd { get; private set; }
    public RelayCommand MoveAllSrcsToMuSrcsCmd { get; private set; }
    public RelayCommand MoveSrcToMuSrcsCmd { get; private set; }

    public RelayCommand<string> AddMuSrcCmd { get; private set; }
    public RelayCommand DelMuSrcTypeCmd { get; private set; }
    public RelayCommand MoveAllMuSrcsToSrcsCmd { get; private set; }
    public RelayCommand MoveMuSrcToSrcsCmd { get; private set; }

    public RelayCommand<string> AddTrgtTypeCmd { get; private set; }
    public RelayCommand DelTrgtTypeCmd { get; private set; }
    public RelayCommand MoveAllTrgtsToMuTrgtsCmd { get; private set; }
    public RelayCommand MoveTrgtToMuTrgtsCmd { get; private set; }

    public RelayCommand<string> AddMuTrgtCmd { get; private set; }
    public RelayCommand DelMuTrgtTypeCmd { get; private set; }
    public RelayCommand MoveMuTrgtToTrgtsCmd { get; private set; }
    public RelayCommand MoveAllMuTrgtsToTrgtsCmd { get; private set; }

    public RelayCommand<string> AddMuAttrCmd { get; private set; }
    public RelayCommand DelMuAttrCmd { get; private set; }

    public RelayCommand<string> AddMuFiltrExprCmd { get; private set; }
    public RelayCommand DelMuFiltrExprCmd { get; private set; }

    #endregion

    #region observable collections

    private SelectionList<M2lCandidate> mMatchedRules = new SelectionList<M2lCandidate>();
    public SelectionList<M2lCandidate> MatchedRules
    {
      get
      {
        return mMatchedRules;
      }
      set
      {
        mMatchedRules = value;
        RaisePropertyChanged(MatchedRulesKey);
      }
    }

    private SelectionList<L2mCandidate> mLazyRules = new SelectionList<L2mCandidate>();
    public SelectionList<L2mCandidate> LazyRules
    {
      get
      {
        return mLazyRules;
      }
      set
      {
        mLazyRules = value;
        RaisePropertyChanged(LazyRulesKey);
      }
    }

    private SelectionList<DrsCandidate> mDrsRules = new SelectionList<DrsCandidate>();
    public SelectionList<DrsCandidate> DrsRules
    {
      get
      {
        return mDrsRules;
      }
      set
      {
        mDrsRules = value;
        RaisePropertyChanged(DrsRulesKey);
      }
    }

    private SelectionList<DusCandidate> mLibraries = new SelectionList<DusCandidate>();
    public SelectionList<DusCandidate> Libraries
    {
      get
      {
        return mLibraries;
      }
      set
      {
        mLibraries = value;
        RaisePropertyChanged(LibrariesKey);
      }
    }

    private SelectionList<DamCandidate> mDamCandidate = new SelectionList<DamCandidate>();
    public SelectionList<DamCandidate> DamCandidates
    {
      get
      {
        return mDamCandidate;
      }
      set
      {
        mDamCandidate = value;
        RaisePropertyChanged("DamCandidates");
      }
    }

    private SelectionList<DfeCandidate> mDfeCandidate = new SelectionList<DfeCandidate>();
    public SelectionList<DfeCandidate> DfeCandidates
    {
      get
      {
        return mDfeCandidate;
      }
      set
      {
        mDfeCandidate = value;
        RaisePropertyChanged("DfeCandidates");
      }
    }

    private ObservableCollection<CstCandidate> mCstCandidates = new ObservableCollection<CstCandidate>();
    public ObservableCollection<CstCandidate> CstCandidates
    {
      get
      {
        return mCstCandidates;
      }
      set
      {
        mCstCandidates = value;
        RaisePropertyChanged("CstCandidates");
      }
    }

    private ObservableCollection<CttCandidate> mCttCandidates = new ObservableCollection<CttCandidate>();
    public ObservableCollection<CttCandidate> CttCandidates
    {
      get
      {
        return mCttCandidates;
      }
      set
      {
        mCttCandidates = value;
        RaisePropertyChanged("CttCandidates");
      }
    }

    private ObservableCollection<AamCandidate> mAamCandidates = new ObservableCollection<AamCandidate>();
    public ObservableCollection<AamCandidate> AamCandidates
    {
      get
      {
        return mAamCandidates;
      }
      set
      {
        mAamCandidates = value;
        RaisePropertyChanged("AamCandidates");
      }
    }

    private ObservableCollection<AfeCandidate> mAfeCandidates = new ObservableCollection<AfeCandidate>();
    public ObservableCollection<AfeCandidate> AfeCandidates
    {
      get
      {
        return mAfeCandidates;
      }
      set
      {
        mAfeCandidates = value;
        RaisePropertyChanged("AfeCandidates");
      }
    }

    #endregion

    #region props

    private bool mProjectPersistent;
    public bool ProjectPersistent
    {
      get
      {
        return mProjectPersistent;
      }
      set
      {
        mProjectPersistent = value;
        RaisePropertyChanged("ProjectPersistent");
      }
    }

    private bool mCemChecked;
    public bool CemChecked
    {
      get
      {
        return mCemChecked;
      }
      set
      {
        mCemChecked = value;
        RaisePropertyChanged("CemChecked");
      }
    }

    private bool mMuGenerating;
    public bool MuGenerating
    {
      get
      {
        return mMuGenerating;
      }
      set
      {
        mMuGenerating = value;
        RaisePropertyChanged("MuGenerating");
      }
    }

    private double mMuGenProgress;
    public double MuGenProgress
    {
      get
      {
        return mMuGenProgress;
      }
      set
      {
        mMuGenProgress = value;
        RaisePropertyChanged("MuGenProgress");
      }
    }

    private string mNewTrgtType;
    public string NewTrgtType
    {
      get
      {
        return mNewTrgtType;
      }
      set
      {
        mNewTrgtType = value;
        RaisePropertyChanged("NewTrgtType");
      }
    }

    private string mNewMuTrgtType;
    public string NewMuTrgtType
    {
      get
      {
        return mNewMuTrgtType;
      }
      set
      {
        mNewMuTrgtType = value;
        RaisePropertyChanged("NewMuTrgtType");
      }
    }

    private string mNewMuSrcType;
    public string NewMuSrcType
    {
      get
      {
        return mNewMuSrcType;
      }
      set
      {
        mNewMuSrcType = value;
        RaisePropertyChanged("NewMuSrcType");
      }
    }

    private string mMutantName;
    public string MutantName
    {
      get
      {
        return mMutantName;
      }
      set
      {
        mMutantName = value;
        RaisePropertyChanged(MutantNameKey);
        ValidateProperty(MutantName, MutantNameKey, "Mutant name is required");
      }
    }

    private string mMutantPath;
    public string MutantPath
    {
      get
      {
        return mMutantPath;
      }
      set
      {
        mMutantPath = value;
        RaisePropertyChanged(MutantPathKey);
        ValidateProperty(MutantPath, MutantPathKey, "Mutant batchFilePath is required");
      }
    }

    private string mMuDir;
    public string MuDir
    {
      get
      {
        return mMuDir;
      }
      set
      {
        mMuDir = value;
        RaisePropertyChanged(MuDirKey);
        //ValidateProperty(MuDir, MuDirKey, "Mutant batchFilePath is required");
      }
    }

    private MutantType? mType;
    public MutantType? MutantType
    {
      get
      {
        return mType;
      }
      set
      {
        mType = value;
        RaisePropertyChanged("MutantType");
      }
    }

    public List<MutantType> MutantTypes { get; set; }

    private MuAtlMutant mFoundResult;
    public MuAtlMutant FoundResult
    {
      get
      {
        return mFoundResult;
      }
      set
      {
        mFoundResult = value;
        RaisePropertyChanged("FoundResult");
      }
    }

    private string mSelectedMuSrcType;
    public string SelectedMuSrcType
    {
      get
      {
        return mSelectedMuSrcType;
      }
      set
      {
        mSelectedMuSrcType = value;
        RaisePropertyChanged("SelectedMuSrcType");
      }
    }

    private string mSelectedSrcType;
    public string SelectedSrcType
    {
      get
      {
        return mSelectedSrcType;
      }
      set
      {
        mSelectedSrcType = value;
        RaisePropertyChanged("SelectedSrcType");
      }
    }

    private CstCandidate mSelectedCstCandidate;
    public CstCandidate SelectedCstCandidate
    {
      get { return mSelectedCstCandidate; }
      set
      {
        mSelectedCstCandidate = value;
        RaisePropertyChanged("SelectedCstCandidate");
      }
    }

    private AamCandidate mAamCandidate;
    public AamCandidate SelectedAamCandidate
    {
      get
      {
        return mAamCandidate;
      }
      set
      {
        mAamCandidate = value;
        RaisePropertyChanged("SelectedAamCandidate");
      }
    }

    private CttCandidate mSelectedCttCandidate;
    public CttCandidate SelectedCttCandidate
    {
      get
      {
        return mSelectedCttCandidate;
      }
      set
      {
        mSelectedCttCandidate = value;
        RaisePropertyChanged("SelectedCttCandidate");
      }
    }

    private AfeCandidate mSelectedAfeCandidate;
    public AfeCandidate SelectedAfeCandidate
    {
      get
      {
        return mSelectedAfeCandidate;
      }
      set
      {
        mSelectedAfeCandidate = value;
        RaisePropertyChanged("SelectedAfeCandidate");
      }
    }

    private string mNewSrcType;
    public string NewSrcType
    {
      get
      {
        return mNewSrcType;
      }
      set
      {
        mNewSrcType = value;
        RaisePropertyChanged("NewSrcType");
      }
    }

    private string mSelectedTrgtType;
    public string SelectedTrgtType
    {
      get
      {
        return mSelectedTrgtType;
      }
      set
      {
        mSelectedTrgtType = value;
        RaisePropertyChanged("SelectedTrgtType");
      }
    }

    private string mSelectedMuTrgtType;
    public string SelectedMuTrgtType
    {
      get
      {
        return mSelectedMuTrgtType;
      }
      set
      {
        mSelectedMuTrgtType = value;
        RaisePropertyChanged("SelectedMuTrgtType");
      }
    }

    private string mSelectedMuAttr;
    public string SelectedMuAttr
    {
      get
      {
        return mSelectedMuAttr;
      }
      set
      {
        mSelectedMuAttr = value;
        RaisePropertyChanged("SelectedMuAttr");
      }
    }

    private string mNewMuAttr;
    public string NewMuAttr
    {
      get
      {
        return mNewMuAttr;
      }
      set
      {
        mNewMuAttr = value;
        RaisePropertyChanged("NewMuAttr");
      }
    }

    private string mNewMuFiltrExpr;
    public string NewMuFiltrExpr
    {
      get
      {
        return mNewMuFiltrExpr;
      }
      set
      {
        mNewMuFiltrExpr = value;
        RaisePropertyChanged("NewMuFiltrExpr");
      }
    }

    private string mSelectedMuFltrExpr;
    public string SelectedMuFltrExpr
    {
      get
      {
        return mSelectedMuFltrExpr;
      }
      set
      {
        mSelectedMuFltrExpr = value;
        RaisePropertyChanged("SelectedMuFltrExpr");
      }
    }

    private MuAtlMutant mSelectedMutant;
    public MuAtlMutant SelectedMutant
    {
      get
      {
        return mSelectedMutant;
      }
      set
      {
        mSelectedMutant = value;
        RaisePropertyChanged("SelectedMutant");
      }
    }

    public override MuAtlProject Project
    {
      get
      {
        return base.Project;
      }
      set
      {
        base.Project = value;
      }
    }

    #endregion

    public MutantsViewModel(IDialogService dlgService, IMutantGenerator mutantGenerator, IAtlParser parser, IDiffer differ)
      : base(dlgService)
    {
      mMutantGenerator = mutantGenerator;
      mParser = parser;
      mDiffer = differ;

      #region cmds

      AddMutantCmd = new RelayCommand(AddMutant);
      DelMutantCmd = new RelayCommand(DelMutant);
      UpdateMutantCmd = new RelayCommand(UpdateMutant);
      MutantSlctCmd = new RelayCommand<object>(SelectMutant);
      SelectMutantFileCmd = new RelayCommand(SelectMutantFile);
      SelectMuDirCmd = new RelayCommand(SelectMuDir);
      DiffCmd = new RelayCommand(Diff);

      SelectAllLazyRulesCmd = new RelayCommand(() => LazyRules.SelectAll());
      UnSelectAllLazyRulesCmd = new RelayCommand(() => LazyRules.UnselectAll());
      SelectAllMatchedRulesCmd = new RelayCommand(() => MatchedRules.SelectAll());
      UnSelectAllMatchedRulesCmd = new RelayCommand(() => MatchedRules.UnselectAll());
      SelectAllCalledRulesCmd = new RelayCommand(() => DrsRules.SelectAll());
      UnSelectAllCalledRulesCmd = new RelayCommand(() => DrsRules.UnselectAll());
      SelectAllLibrariesRulesCmd = new RelayCommand(() => Libraries.SelectAll());
      UnSelectAllLibrariesRulesCmd = new RelayCommand(() => Libraries.UnselectAll());
      SelectAllDamCandidatesCmd = new RelayCommand(() => DamCandidates.SelectAll());
      UnSelectAllDamCandidatesCmd = new RelayCommand(() => DamCandidates.UnselectAll());
      SelectAllDfeCandidatesCmd = new RelayCommand(() => DfeCandidates.SelectAll());
      UnSelectAllDfeCandidatesCmd = new RelayCommand(() => DfeCandidates.UnselectAll());

      AddSrcTypeCmd = new RelayCommand<string>(AddSourceType);
      DelSrcTypeCmd = new RelayCommand(DeleteSourceType);
      AddMuSrcCmd = new RelayCommand<string>(AddMuSource);
      DelMuSrcTypeCmd = new RelayCommand(DeleteMuSource);
      MoveSrcToMuSrcsCmd = new RelayCommand(MoveSrcToMuSrcs);
      MoveMuSrcToSrcsCmd = new RelayCommand(MoveMuSrcToSrcs);
      MoveAllSrcsToMuSrcsCmd = new RelayCommand(MoveAllSrcsToMuSrcs);
      MoveAllMuSrcsToSrcsCmd = new RelayCommand(MoveAllMuSrcsToSrcs);

      AddTrgtTypeCmd = new RelayCommand<string>(AddTargetType);
      DelTrgtTypeCmd = new RelayCommand(DeleteTargetType);
      AddMuTrgtCmd = new RelayCommand<string>(AddMuTarget);
      DelMuTrgtTypeCmd = new RelayCommand(DeletMuTarget);
      MoveTrgtToMuTrgtsCmd = new RelayCommand(MoveTrgtToMuTrgts);
      MoveMuTrgtToTrgtsCmd = new RelayCommand(MoveMuTrgtToTrgts);
      MoveAllTrgtsToMuTrgtsCmd = new RelayCommand(MoveAllTrgtsToMuTrgts);
      MoveAllMuTrgtsToTrgtsCmd = new RelayCommand(MoveAllMuTrgtsToTrgts);

      AddMuAttrCmd = new RelayCommand<string>(AddMuAttr);
      DelMuAttrCmd = new RelayCommand(DelMuAttr);
      AddMuFiltrExprCmd = new RelayCommand<string>(AddMuFiltrExpr);
      DelMuFiltrExprCmd = new RelayCommand(DelMuFiltrExpr);

      ChkStatusCmd = new RelayCommand(ChkStatus);
      GenerateCmd = new RelayCommand(Generate);

      #endregion

      MutantTypes = Enum.GetValues(typeof(MutantType)).Cast<MutantType>().OrderBy(m => m.ToString()).ToList();

      Messenger.Default.Register<int>(this, "ClearCandidates", ClearCandidates);
      Messenger.Default.Register<bool>(this, "ProjectPersistent", OnProjectPersistent);

      if (IsInDesignModeStatic)
      {
        ProjectPersistent = true;
      }
    }

    private void OnProjectPersistent(bool persistent)
    {
      if (!persistent)
      {
        ProjectPersistent = persistent;
        return;
      }

      if (string.IsNullOrEmpty(Project.Module))
      {
        return;
      }

      ProjectPersistent = persistent;
      var tokens = (new AtlTokenizer()).GetTokens(Project.Module); ;
      var listener = new MuCandidateListener(tokens);
      var parseTree = mParser.Parse(tokens);

      var walker = new ParseTreeWalker();
      walker.Walk(listener, parseTree);

      foreach (var matchedRule in listener.MutationCandidates.OfType<M2lCandidate>().Except(listener.MutationCandidates.OfType<DrsCandidate>()))
        MatchedRules.Add(matchedRule);

      foreach (var lazyRule in listener.MutationCandidates.OfType<L2mCandidate>())
        LazyRules.Add(lazyRule);

      foreach (var calledRule in listener.MutationCandidates.OfType<DrsCandidate>())
        DrsRules.Add(calledRule);

      foreach (var library in listener.MutationCandidates.OfType<DusCandidate>())
        Libraries.Add(library);

      foreach (var damCandidate in listener.MutationCandidates.OfType<DamCandidate>())
        DamCandidates.Add(damCandidate);

      foreach (var dfeCandidate in listener.MutationCandidates.OfType<DfeCandidate>())
        DfeCandidates.Add(dfeCandidate);

      foreach (var aamCandidate in listener.MutationCandidates.OfType<AamCandidate>())
      {
        aamCandidate.MuMappings = new ObservableCollection<string>();
        AamCandidates.Add(aamCandidate);
      }

      foreach (var cstCandidate in listener.MutationCandidates.OfType<CstCandidate>())
      {
        cstCandidate.SrcTypes = new ObservableCollection<string>(
          listener.SrcTypes.Except(new[] { cstCandidate.SourceType }));
        cstCandidate.MuSrcTypes = new ObservableCollection<string>();
        CstCandidates.Add(cstCandidate);
      }

      foreach (var cttCandidate in listener.MutationCandidates.OfType<CttCandidate>())
      {
        cttCandidate.TargetTypes = new ObservableCollection<string>(
          listener.TargetTypes.Except(new[] { cttCandidate.TargetType }));
        cttCandidate.MuTargetTypes = new ObservableCollection<string>();
        CttCandidates.Add(cttCandidate);
      }

      foreach (var afeCandidate in listener.MutationCandidates.OfType<AfeCandidate>())
      {
        afeCandidate.MuFilteringExpressions = new ObservableCollection<string>();
        AfeCandidates.Add(afeCandidate);
      }
    }

    public const string MutantNotFoundMsg = "Mutant cannot be found on disk. Mutant moved or deleted.";
    public const string MutantNotFoundCaption = "Diff Mutant";

    private void Diff()
    {
      if (SelectedMutant == null) return;

      if (!File.Exists(SelectedMutant.Path))
      {
        mDlgService.Error(MutantNotFoundMsg, MutantNotFoundCaption);
        return;
      }

      if (!File.Exists(Project.Module))
      {
        mDlgService.Error("ATL module cannot be found on disk. Moved or deleted.", "Diff Mutant");
        return;
      }

      mDiffer.Diff(Project.Module, SelectedMutant.Path);
    }

    private void AddMuFiltrExpr(string filtrExpr)
    {
      if (string.IsNullOrEmpty(filtrExpr) ||
          SelectedAfeCandidate == null ||
          SelectedAfeCandidate.MuFilteringExpressions.Contains(filtrExpr))
        return;

      SelectedAfeCandidate.MuFilteringExpressions.Add(filtrExpr);
      NewMuFiltrExpr = string.Empty;
    }

    private void DelMuFiltrExpr()
    {
      if (SelectedAfeCandidate != null && !string.IsNullOrEmpty(SelectedMuFltrExpr))
        SelectedAfeCandidate.MuFilteringExpressions.Remove(SelectedMuFltrExpr);
    }

    private void AddMuAttr(string attr)
    {
      if (string.IsNullOrEmpty(attr) ||
          SelectedAamCandidate == null ||
          SelectedAamCandidate.MuMappings.Contains(attr))
        return;

      SelectedAamCandidate.MuMappings.Add(attr);
      NewMuAttr = string.Empty;
    }

    private void DelMuAttr()
    {
      if (SelectedAamCandidate != null &&
          !string.IsNullOrEmpty(SelectedMuAttr))
        SelectedAamCandidate.MuMappings.Remove(SelectedMuAttr);
    }

    private void ClearCandidates(int param)
    {
      MatchedRules.Clear();
      LazyRules.Clear();
      DrsRules.Clear();
      Libraries.Clear();
      DamCandidates.Clear();
      DfeCandidates.Clear();
      CstCandidates.Clear();
      CttCandidates.Clear();
      AamCandidates.Clear();
      AfeCandidates.Clear();
      MuDir = string.Empty;
      CemChecked = false;
    }

    private void MoveSrcToMuSrcs()
    {
      if (SelectedCstCandidate == null ||
         string.IsNullOrEmpty(SelectedSrcType) ||
         SelectedCstCandidate.MuSrcTypes.Contains(SelectedSrcType))
        return;

      SelectedCstCandidate.MuSrcTypes.Add(SelectedSrcType);
      SelectedCstCandidate.SrcTypes.Remove(SelectedSrcType);
    }

    private void MoveTrgtToMuTrgts()
    {
      if (SelectedCttCandidate == null ||
        string.IsNullOrEmpty(SelectedTrgtType) ||
        SelectedCttCandidate.MuTargetTypes.Contains(SelectedTrgtType))
        return;

      SelectedCttCandidate.MuTargetTypes.Add(SelectedTrgtType);
      SelectedCttCandidate.TargetTypes.Remove(SelectedTrgtType);
    }

    private void MoveAllSrcsToMuSrcs()
    {
      if (SelectedCstCandidate == null || !SelectedCstCandidate.SrcTypes.Any())
        return;

      var remDic = new Dictionary<string, bool>();
      foreach (var srcType in SelectedCstCandidate.SrcTypes)
      {
        if (!SelectedCstCandidate.MuSrcTypes.Contains(srcType))
        {
          SelectedCstCandidate.MuSrcTypes.Add(srcType);
          remDic.Add(srcType, true);
        }
        else
        {
          remDic.Add(srcType, false);
        }
      }

      foreach (var muSrcType in SelectedCstCandidate.MuSrcTypes)
      {
        if (SelectedCstCandidate.SrcTypes.Contains(muSrcType) && remDic[muSrcType])
        {
          SelectedCstCandidate.SrcTypes.Remove(muSrcType);
        }
      }
    }

    private void MoveAllTrgtsToMuTrgts()
    {
      if (SelectedCttCandidate == null || !SelectedCttCandidate.TargetTypes.Any())
        return;

      var remDic = new Dictionary<string, bool>();
      foreach (var trgtType in SelectedCttCandidate.TargetTypes)
      {
        if (!SelectedCttCandidate.MuTargetTypes.Contains(trgtType))
        {
          SelectedCttCandidate.MuTargetTypes.Add(trgtType);
          remDic.Add(trgtType, true);
        }
        else
        {
          remDic.Add(trgtType, false);
        }
      }

      foreach (var muTrgtType in SelectedCttCandidate.MuTargetTypes)
      {
        if (SelectedCttCandidate.TargetTypes.Contains(muTrgtType) && remDic[muTrgtType])
        {
          SelectedCttCandidate.TargetTypes.Remove(muTrgtType);
        }
      }
    }

    private void MoveAllMuSrcsToSrcs()
    {
      if (SelectedCstCandidate == null ||
          !SelectedCstCandidate.MuSrcTypes.Any())
        return;

      var remDic = new Dictionary<string, bool>();
      foreach (var muSrcType in SelectedCstCandidate.MuSrcTypes)
      {
        if (!SelectedCstCandidate.SrcTypes.Contains(muSrcType))
        {
          SelectedCstCandidate.SrcTypes.Add(muSrcType);
          remDic.Add(muSrcType, true);
        }
        else
        {
          remDic.Add(muSrcType, false);
        }
      }

      foreach (var srcType in SelectedCstCandidate.SrcTypes)
      {
        if (SelectedCstCandidate.MuSrcTypes.Contains(srcType) && remDic[srcType])
        {
          SelectedCstCandidate.MuSrcTypes.Remove(srcType);
        }
      }
    }

    private void MoveAllMuTrgtsToTrgts()
    {
      if (SelectedCttCandidate == null || !SelectedCttCandidate.MuTargetTypes.Any())
        return;

      var remDic = new Dictionary<string, bool>();
      foreach (var muTargetType in SelectedCttCandidate.MuTargetTypes)
      {
        if (!SelectedCttCandidate.TargetTypes.Contains(muTargetType))
        {
          SelectedCttCandidate.TargetTypes.Add(muTargetType);
          remDic.Add(muTargetType, true);
        }
        else
        {
          remDic.Add(muTargetType, false);
        }
      }

      foreach (var targetType in SelectedCttCandidate.TargetTypes)
      {
        if (SelectedCttCandidate.MuTargetTypes.Contains(targetType) && remDic[targetType])
        {
          SelectedCttCandidate.MuTargetTypes.Remove(targetType);
        }
      }
    }

    private void MoveMuSrcToSrcs()
    {
      if (SelectedCstCandidate == null ||
          string.IsNullOrEmpty(SelectedMuSrcType) ||
          SelectedCstCandidate.SrcTypes.Contains(SelectedMuSrcType))
        return;

      SelectedCstCandidate.SrcTypes.Add(SelectedMuSrcType);
      SelectedCstCandidate.MuSrcTypes.Remove(SelectedMuSrcType);
    }

    private void MoveMuTrgtToTrgts()
    {
      if (SelectedCttCandidate == null ||
          string.IsNullOrEmpty(SelectedMuTrgtType) ||
          SelectedCttCandidate.TargetTypes.Contains(SelectedMuTrgtType))
        return;

      SelectedCttCandidate.TargetTypes.Add(SelectedMuTrgtType);
      SelectedCttCandidate.MuTargetTypes.Remove(SelectedMuTrgtType);
    }

    private void DeleteMuSource()
    {
      if (SelectedCstCandidate == null ||
          string.IsNullOrEmpty(SelectedMuSrcType) ||
          !SelectedCstCandidate.MuSrcTypes.Contains(SelectedMuSrcType))
        return;

      SelectedCstCandidate.MuSrcTypes.Remove(SelectedMuSrcType);
    }

    private void DeletMuTarget()
    {
      if (SelectedCttCandidate == null ||
          string.IsNullOrEmpty(SelectedMuTrgtType) ||
          !SelectedCttCandidate.MuTargetTypes.Contains(SelectedMuTrgtType))
        return;

      SelectedCttCandidate.MuTargetTypes.Remove(SelectedMuTrgtType);
    }

    public const string AddMuSrcTypeCaption = "Add Mutant source type";

    private void AddMuSource(string type)
    {
      if (string.IsNullOrEmpty(type) || SelectedCstCandidate == null)
        return;

      if (SelectedCstCandidate.MuSrcTypes.Contains(type))
      {
        mDlgService.Error(string.Format(
          "Mutant source type {0} already added for rule {1}", type, SelectedCstCandidate.Rule),
          AddMuSrcTypeCaption);
        return;
      }

      SelectedCstCandidate.MuSrcTypes.Add(type);
      NewMuSrcType = string.Empty;
    }

    public const string AddMuTrgtTypeCaption = "Add Mutant target type";

    private void AddMuTarget(string type)
    {
      if (string.IsNullOrEmpty(type) || SelectedCttCandidate == null)
        return;

      if (SelectedCttCandidate.MuTargetTypes.Contains(type))
      {
        mDlgService.Error(
          string.Format("Mutant target type {0} already added for rule {1}", type, SelectedCttCandidate.Rule), AddMuTrgtTypeCaption);
        return;
      }

      SelectedCttCandidate.MuTargetTypes.Add(type);
      NewMuTrgtType = string.Empty;
    }

    private void DeleteTargetType()
    {
      if (SelectedCttCandidate == null || string.IsNullOrEmpty(SelectedTrgtType)
          || !SelectedCttCandidate.TargetTypes.Contains(SelectedTrgtType))
        return;

      SelectedCttCandidate.TargetTypes.Remove(SelectedTrgtType);
    }

    private void DeleteSourceType()
    {
      if (SelectedCstCandidate == null ||
          string.IsNullOrEmpty(SelectedSrcType) ||
          !SelectedCstCandidate.SrcTypes.Contains(SelectedSrcType))
        return;

      SelectedCstCandidate.SrcTypes.Remove(SelectedSrcType);
    }

    public const string AddSrcTypeCaption = "Add source type";

    private void AddSourceType(string type)
    {
      if (string.IsNullOrEmpty(type) || SelectedCstCandidate == null)
        return;

      if (SelectedCstCandidate.SrcTypes.Contains(type))
      {
        mDlgService.Error(string.Format(
          "Source type {0} already exists for CST mutant generation", type),
          AddSrcTypeCaption);
        return;
      }

      foreach (var candidate in CstCandidates)
      {
        candidate.SrcTypes.Add(type);
      }
      NewSrcType = string.Empty;
    }

    public const string AddTrgtTypeCaption = "Add target type";

    private void AddTargetType(string type)
    {
      if (string.IsNullOrEmpty(type) || SelectedCttCandidate == null)
        return;

      if (SelectedCttCandidate.TargetTypes.Contains(type))
      {
        mDlgService.Error(string.Format("Source type {0} already exists for CTT mutant generation", type), AddTrgtTypeCaption);
        return;
      }

      foreach (var candidate in CttCandidates)
      {
        candidate.TargetTypes.Add(type);
      }
      NewTrgtType = string.Empty;
    }

    public const string MutantMsg = "Mutant";

    private void SelectMutantFile()
    {
      var path = mDlgService.BrowseAtl(MutantMsg);
      if (!string.IsNullOrEmpty(path))
        MutantPath = path;
    }

    public const string SelectMuDirMsg = "Select mutant directory";

    private void SelectMuDir()
    {
      var muDir = mDlgService.BrowseFolder(SelectMuDirMsg);
      if (!string.IsNullOrEmpty(muDir))
      {
        MuDir = muDir;
      }
    }

    public const string MutantDirNotSelectedErrorMsg = "Select mutant destination directory";
    public const string MutantDirNotSelectedCaption = "Generate Mutants";
    public const string NoCandidatesSelectedError = "Select mutation candidates";

    private void Generate()
    {
      if(string.IsNullOrEmpty(MuDir))
      {
        mDlgService.Error(MutantDirNotSelectedErrorMsg, MutantDirNotSelectedCaption);
        return;
      }

      var candidates = new List<MutationCandidate>();
      candidates.AddRange(MatchedRules.SelectedItems);
      candidates.AddRange(LazyRules.SelectedItems);
      candidates.AddRange(DrsRules.SelectedItems);
      candidates.AddRange(Libraries.SelectedItems);
      candidates.AddRange(DamCandidates.SelectedItems);
      candidates.AddRange(DfeCandidates.SelectedItems);
      candidates.AddRange(CstCandidates.Where(c => c.MuSrcTypes.Count > 0));
      candidates.AddRange(CttCandidates.Where(c => c.MuTargetTypes.Count > 0));
      candidates.AddRange(AamCandidates.Where(c => c.MuMappings.Count > 0));
      candidates.AddRange(AfeCandidates.Where(c => c.MuFilteringExpressions.Count > 0));
      if (CemChecked)
        candidates.Add(new CemCandidate());

      if (!candidates.Any())
      {
        mDlgService.Error(NoCandidatesSelectedError, MutantDirNotSelectedCaption);
        return;
      }

      MuGenerating = true;
      Task.Factory.StartNew(() =>
      {
        mMutantGenerator.Generate(MuDir, Project, candidates);
      })
      .ContinueWith(t =>
      {
        MatchedRules.UnselectAll();
        LazyRules.UnselectAll();
        DrsRules.UnselectAll();
        Libraries.UnselectAll();
        DamCandidates.UnselectAll();
        DfeCandidates.UnselectAll();

        MuGenerating = false;
        CemChecked = false;

        foreach (var cstCandidate in CstCandidates.Where(c => c.MuSrcTypes.Count > 0))
        {
          foreach (var source in cstCandidate.MuSrcTypes)
          {
            cstCandidate.SrcTypes.Add(source);
          }
          cstCandidate.MuSrcTypes.Clear();
        }

        foreach (var cttCandidate in CttCandidates)
        {
          foreach (var target in cttCandidate.MuTargetTypes)
          {
            cttCandidate.TargetTypes.Add(target);
          }
          cttCandidate.MuTargetTypes.Clear();
        }

        foreach (var aamCandidate in AamCandidates)
        {
          aamCandidate.MuMappings.Clear();
        }

        foreach (var afeCandidate in AfeCandidates)
        {
          afeCandidate.MuFilteringExpressions.Clear();
        }
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void SelectMutant(object param)
    {
      var mutant = param as MuAtlMutant;
      if (mutant == null)
      {
        MutantName = string.Empty;
        MutantPath = string.Empty;
        MutantType = null;
        ClearValErrors();
        return;
      }

      ShowMutant(mutant);
    }

    public const string AddMutantErrorMsg = "Cannot add mutant. Select mutant type";
    public const string AddMutantCaption = "Add Mutant";

    private void AddMutant()
    {
      ValidateProperty(MutantName, MutantNameKey, "Mutant name is required");
      ValidateProperty(MutantPath, MutantPathKey, "Mutant path is required");

      if (HasErrors)
        return;

      if (!MutantType.HasValue)
      {
        mDlgService.Error(AddMutantErrorMsg, AddMutantCaption);
        return;
      }

      if (Project.Mutants.Any(m => m.Name == MutantName))
      {
        mDlgService.Error(string.Format("Cannot add mutant {0} because it already exists", MutantName), AddMutantCaption);
        return;
      }

      if (!File.Exists(MutantPath))
      {
        mDlgService.Error(string.Format("Mutant {0} does not exist", MutantPath), AddMutantCaption);
        return;
      }

      var mutant = new MuAtlMutant
      {
        Name = MutantName,
        Path = MutantPath,
        Type = MutantType.Value
      };

      Project.AddMutant(mutant);

      MutantName = string.Empty;
      MutantPath = string.Empty;
      MutantType = null;
      ClearValErrors();
    }

    protected override void FindItem(string searchParam)
    {
      if (string.IsNullOrEmpty(searchParam))
        return;

      FoundResult = Project.Mutants.FirstOrDefault(m =>
        string.Compare(m.Name, searchParam, true) == 0);

      if (FoundResult != null)
        ShowMutant(FoundResult);
    }

    private void ShowMutant(MuAtlMutant mutant)
    {
      MutantName = mutant.Name;
      MutantPath = mutant.Path;
      MutantType = mutant.Type;
    }

    private void ClearValErrors()
    {
      mValidationErrors.Clear();
      RaiseErrorsChanged(MutantPathKey);
      RaiseErrorsChanged(MutantNameKey);
    }

    private void ChkStatus()
    {
      foreach (var mutant in Project.Mutants)
      {
        var mutantResults = Project.Results.Where(r => r.Mutant.Equals(mutant));
        if (!mutantResults.Any())
        {
          mutant.Status = MutantStatus.Undetermined;
          continue;
        }

        if (mutantResults.All(r => r.Verdict.HasValue && r.Verdict == MuAtlVerdict.Fail) 
            && mutantResults.Count() == Project.TestSuite.Count)
        {
          mutant.Status = MutantStatus.Dead;
        }
        else if (mutantResults.Any(r => r.Verdict.HasValue && r.Verdict == MuAtlVerdict.Pass))
        {
          mutant.Status = MutantStatus.Live;
        }
        else
        {
          mutant.Status = MutantStatus.Undetermined;
        }
      }
    }

    public const string SelectMutantMsg = "Select mutant";
    public const string DelMutantCaption = "Delete Mutant";

    private void DelMutant()
    {
      if (SelectedMutant == null)
      {
        mDlgService.Error(SelectMutantMsg, DelMutantCaption);
        return;
      }
      Project.Mutants.Remove(SelectedMutant);
    }

    public const string UpdateMutantCaption = "Update mutant";
    public const string UpdateMutantErrorMsg = "Cannot update mutant. Select mutant type";

    private void UpdateMutant()
    {
      if (SelectedMutant == null)
      {
        mDlgService.Error(SelectMutantMsg, UpdateMutantCaption);
        return;
      }

      ValidateProperty(MutantName, MutantNameKey, "Mutant name is required");
      ValidateProperty(MutantPath, MutantPathKey, "Mutant path is required");

      if (HasErrors)
        return;

      if (!MutantType.HasValue)
      {
        mDlgService.Error(UpdateMutantErrorMsg, UpdateMutantCaption);
        return;
      }

      if (Project.Mutants.Except(new[] { SelectedMutant }).Any(m => m.Name == MutantName))
      {
        mDlgService.Error(string.Format("Mutant {0} already exists", MutantName), UpdateMutantCaption);
        return;
      }

      if (!File.Exists(MutantPath))
      {
        mDlgService.Error(string.Format("Mutant {0} does not exist", MutantPath), UpdateMutantCaption);
        return;
      }

      SelectedMutant.Name = MutantName;
      SelectedMutant.Path = MutantPath;
      SelectedMutant.Type = MutantType.Value;
    }
  }
}