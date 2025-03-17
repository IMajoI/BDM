using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDM_Form
{
    class ExcelFileData
    {
        Workbook xlWorkbook;
        public ExcelFileData()
        {
        }

        public List<string> SelectFile(string FilePath)
        {
            List<string> L = new List<string>();
            if (!string.IsNullOrEmpty(FilePath))
            {
                Application xlApp = new Application();
                xlWorkbook = xlApp.Workbooks.Open(FilePath, null, true);
                foreach (_Worksheet item in xlWorkbook.Sheets)
                {
                    L.Add(item.Name);
                }
            }
            //Create COM Objects. Create a COM object for everything that is referenced
            return L;
        }

        public System.Data.DataTable SelectList(int listnr)
        {
            _Worksheet xlWorksheet = xlWorkbook.Sheets[listnr];
            Range xlRange = xlWorksheet.UsedRange;
            Array myvalues = (Array)xlRange.Cells.Value;
            System.Data.DataTable DT = new System.Data.DataTable();
            xlWorkbook.Close();
            try
            {
                for (int i = 1; i < myvalues.GetLength(1) + 1; i++)
                {
                    DT.Columns.Add("Column" + i.ToString());
                }
                for (int i = 1; i < myvalues.GetLength(0) + 1; i++)
                {
                    DataRow DR = DT.NewRow();
                    for (int j = 1; j < myvalues.GetLength(1) + 1; j++)
                    {
                        DR[j - 1] = myvalues.GetValue(i, j);
                    }
                    DT.Rows.Add(DR);
                }
            }
            catch
            {
            }
            return DT;
        }
    }
}
