using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Specialized;
using MuAtl.View.Util.Paging;

namespace MuAtl.View.Util
{
  public class MuAtlDataGrid : DataGrid
  {
    private int mPageSize = 10;
    private PagedCollectionView mPagedItems;
    private INotifyCollectionChanged sourceCollection;

    private Button firstBtn, previousBtn, nextBtn, lastBtn;
    private TextBlock mPageLbl, mNumPagesLbl;

    private const string PagedItemSourceKey = "PagedItemSource";
    public ListCollectionView PagedItemSource
    {
      get
      {
        return (ListCollectionView)GetValue(PagedItemSourceProperty);
      }
      set
      {
        SetValue(PagedItemSourceProperty, value);
      }
    }

    public static readonly DependencyProperty PagedItemSourceProperty =
      DependencyProperty.Register(
        PagedItemSourceKey,
        typeof(ListCollectionView),
        typeof(MuAtlDataGrid),
        new FrameworkPropertyMetadata(null, PagedItemSourceChanged));

    private const string FoundResultKey = "FoundResult";
    public object FoundResult
    {
      get
      {
        return GetValue(FoundResultProperty);
      }
      set
      {
        SetValue(FoundResultProperty, value);
      }
    }

    public static readonly DependencyProperty FoundResultProperty =
      DependencyProperty.Register(
        FoundResultKey,
        typeof(object),
        typeof(MuAtlDataGrid),
        new FrameworkPropertyMetadata(null, FoundResultChanged));

    public Int32 PageSize
    {
      get
      {
        return mPageSize;
      }
      set
      {
        mPageSize = value;
      }
    }

    static MuAtlDataGrid()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MuAtlDataGrid), new FrameworkPropertyMetadata(typeof(MuAtlDataGrid)));
    }

    public MuAtlDataGrid()
      : base()
    {
      PreviewKeyDown += OnPreviewKeyDown;
      SelectionMode = DataGridSelectionMode.Single;
      SelectionUnit = DataGridSelectionUnit.FullRow;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      firstBtn = (Button)GetTemplateChild("btnFirstPage");
      previousBtn = (Button)GetTemplateChild("btnPreviousPage");
      nextBtn = (Button)GetTemplateChild("btnNextPage");
      lastBtn = (Button)GetTemplateChild("btnLastPage");
      mNumPagesLbl = (TextBlock)GetTemplateChild("lblNumberOfPages");
      mPageLbl = (TextBlock)GetTemplateChild("lblPageIndex");
      mPageLbl.Text = "1";

      firstBtn.Click += firstBtn_Click;
      previousBtn.Click += previousBtn_Click;
      nextBtn.Click += nextBtn_Click;
      lastBtn.Click += lastBtn_Click;
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Remove)
        return;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Delete)
        return;

      var index = mPagedItems.CurrentPosition;
      var item = mPagedItems.CurrentItem;
      PagedItemSource.Remove(item);
      PagedItemSource.Refresh();

      Focus();

      if (index >= mPagedItems.Count)
      {
        --index;
      }
      SelectedIndex = index;
    }

    private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      var maxPages = (int)Math.Ceiling((sender as ListCollectionView).Count / (double)PageSize);
      mNumPagesLbl.Text = maxPages.ToString();
    }

    private static void PagedItemSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      (obj as MuAtlDataGrid).OnPagedItemSourceChanged(e);
    }

    private static void FoundResultChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      (obj as MuAtlDataGrid).OnFoundResultChanged(e);
    }

    private void OnFoundResultChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        return;

      var index = PagedItemSource.IndexOf(e.NewValue);
      var pageNumber = (int)Math.Floor(index / (double)PageSize);
      mPagedItems.MoveToPage(pageNumber);
      SelectedItem = e.NewValue;
      Focus();
    }

    private void OnPagedItemSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      var listItemSource = e.NewValue as ListCollectionView;
      if (listItemSource == null)
        return;

      if (mNumPagesLbl == null || mPageLbl == null)
      {
        ApplyTemplate();
      }

      sourceCollection = (listItemSource as INotifyCollectionChanged);
      sourceCollection.CollectionChanged += OnSourceCollectionChanged;

      mPagedItems = new PagedCollectionView(listItemSource);
      mPagedItems.PageSize = PageSize;
      mPagedItems.CollectionChanged += mPagedItems_CollectionChanged;

      foreach (var sd in listItemSource.SortDescriptions)
      {
        mPagedItems.SortDescriptions.Add(sd);
      }
      foreach (var gd in listItemSource.GroupDescriptions)
      {
        mPagedItems.GroupDescriptions.Add(gd);
      }

      ItemsSource = mPagedItems;
      SelectedIndex = -1;

      mNumPagesLbl.Text = GetMaxPages().ToString();
      mPageLbl.Text = 1.ToString();
    }


    private void mPagedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (mNumPagesLbl == null || mPageLbl == null)
      {
        ApplyTemplate();
      }
    }

    private int GetMaxPages()
    {
      if (mPagedItems == null || mPagedItems.TotalItemCount == 0)
        return 1;

      var maxPages = (int)Math.Ceiling(mPagedItems.TotalItemCount / (double)PageSize);
      return maxPages;
    }

    private void lastBtn_Click(object sender, RoutedEventArgs e)
    {
      if (mPagedItems.MoveToLastPage())
      {
        Unselect();
        mPageLbl.Text = (mPagedItems.PageIndex + 1).ToString();
      }
    }

    private void nextBtn_Click(object sender, RoutedEventArgs e)
    {
      if (mPagedItems.MoveToNextPage())
      {
        Unselect();
        mPageLbl.Text = (mPagedItems.PageIndex + 1).ToString();
      }
    }

    private void previousBtn_Click(object sender, RoutedEventArgs e)
    {
      if (mPagedItems.MoveToPreviousPage())
      {
        Unselect();
        mPageLbl.Text = (mPagedItems.PageIndex + 1).ToString();
      }
    }

    private void firstBtn_Click(object sender, RoutedEventArgs e)
    {
      if (mPagedItems.MoveToFirstPage())
      {
        Unselect();
        mPageLbl.Text = 1.ToString();
      }
    }

    private void Unselect()
    {
      //SelectedIndex = -1;
    }
  }

  public static class IEnumerableExtension
  {
    public static IEnumerable<object> GetPage(this IEnumerable<object> collection, int page, int pageSize)
    {
      return collection.Skip(pageSize * (page)).Take(pageSize);
    }
  }
}
