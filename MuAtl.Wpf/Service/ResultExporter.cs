using MuAtl.Model;
using System;
using Application = Microsoft.Office.Interop.Excel.Application;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Collections;
using log4net;

namespace MuAtl.Service
{
  public interface IResultExporter : IDisposable
  {
    void Init();
    void Export(MuAtlResult result);
    void Save(string path);
  }

  public class ResultExporter : IResultExporter
  {
    private static readonly ILog logger = LogManager.GetLogger(typeof(ResultExporter));

    private Application xlApp;
    private Workbook xlWorkbook;
    private Worksheet xlWorkSheet;
    private int row;

    public void Init()
    {
      xlApp = new Application();
      xlWorkbook = xlApp.Workbooks.Add();
      xlWorkSheet = xlWorkbook.Worksheets[1];
      xlWorkSheet.Name = "Results";
      row = 1;

      WriteRow(xlWorkSheet, row++, 1, new[] { "Mutant", "Test Case", "Verdict", "Comment" }, true);
    }

    public void Export(MuAtlResult result)
    {
      WriteRow(xlWorkSheet, row++, 1,
        new object[] 
        { 
          result.Mutant.Name, 
          result.TestCase.Name, 
          result.Verdict.HasValue ? result.Verdict.Value.ToString("F") : string.Empty, 
          result.Comment 
        });
    }

    public void Save(string path)
    {
      try
      {
        xlWorkbook.SaveAs(path);
      }
      catch (Exception ex)
      {
        logger.ErrorFormat("Exception {0} occurred while exporting results to destination {1}", ex.Message, path);
      }
      finally
      {
        try
        {
          xlWorkbook.Close(false);
        }
        catch
        {
        }
        try
        {
          xlApp.Quit();
        }
        catch
        {
        }
        ReleaseObject(xlWorkSheet);
        ReleaseObject(xlWorkbook);
        ReleaseObject(xlApp);
      }
    }

    private void WriteCell(Worksheet sheet, int row, int col, object value, bool bold = false)
    {
      sheet.Cells[row, col] = value;
      sheet.Cells[row, col].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;
    }

    private int WriteRow(Worksheet sheet, int row, int startCol, IEnumerable cellValues, bool bold = false)
    {
      var col = startCol;
      foreach (var cellValue in cellValues)
      {
        WriteCell(sheet, row, col, cellValue);
        if (bold)
        {
          MakeCellBold(sheet, row, col);
        }
        ++col;
      }
      return col;
    }

    private void MakeCellBold(Worksheet sheet, int row, int col)
    {
      sheet.Cells[row, col].Font.Bold = true;
    }

    private static void ReleaseObject(object obj)
    {
      try
      {
        Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch
      {
        obj = null;
      }
      finally
      {
        GC.Collect();
      }
    }

    public void Dispose()
    {
      if (xlWorkSheet != null)
        ReleaseObject(xlWorkSheet);

      if (xlWorkbook != null)
      {
        try
        {
          xlWorkbook.Close(false);
        }
        catch
        {
        }
        ReleaseObject(xlWorkbook);
      }

      if (xlApp != null)
      {
        try
        {
          xlApp.Quit();
        }
        catch
        {
        }
        ReleaseObject(xlApp);
      }
    }
  }
}
