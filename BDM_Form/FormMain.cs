using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BDM_Form;
using BDMdata;
using TIAIFCNS;
using AIFCNS;
using System.Data;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace BDM_Form
{
    public partial class FormMain : Form
    {
        private BackgroundWorker backgroundWorker1 = null;
        private BackgroundWorker bw_BuildXML = null;            //bw for build complete xml importing file
        private BackgroundWorker bw_BuildXMLpart = null;        //bw for build partial xml importing file
        private BackgroundWorker bw_DownloadPmiLibs = null;     //bw for copy pmi resources to have  custom indicators
        private BackgroundWorker bw_DownloadResources = null;   //bw for copy resource pics for faceplates
        private ExcelListSelector ELS;
        private ExcelFileData EF;
        private int ExcelListIndex;       
        private SaveFileDialog saveDialog = new SaveFileDialog();                
        private BindingList<DBMapDef> DBnames;
        private List<int> InvPtrs = new List<int>();
        private int InvCtr = 0;       
        private delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        private static event StatusUpdateHandler OnUpdateStatus;
        public GData GlobalData;
        private BindingList<string> listDataTypes;
        public static BindingList<HeaderCls> _header;
        private string path = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            GlobalData = new GData();
            RegistryKey RegK = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Excel\\Addins\\BDM"); // get instalation path from registry
            if (RegK != null)
            {
                path = RegK.GetValue("Manifest").ToString();
                path = path.Replace("BDM.vsto", "XMLSources/AObjectTypes.xml"); //|vstolocal
                path = path.Replace("file:///", "");
            }

            GlobalData.DeserializeDataTypes(path);
            listDataTypes = new BindingList<string>();
            _header = new BindingList<HeaderCls>();

            listChange();
            #region table init

            //Bind BDMdata to grid
            //Manualy add col1, col2 and col3 to have col3 - DataType combobox type column and then bind whole bindinglist to table
            //textbox columns tagname, descr
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.DataPropertyName = Resource.Col1;
            col1.HeaderText = Resource.Col1;
            col1.Name = Resource.Col1;
            col1.Frozen = true;
            dgvBDM.Columns.Add(col1);

            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.DataPropertyName = Resource.Col2;
            col2.HeaderText = Resource.Col2;
            col2.Name = Resource.Col2;
            col2.Width = 300;
            col2.Frozen = true;
            dgvBDM.Columns.Add(col2);

            //combobox column for DataType - comboboxcolumn type
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            comboBoxColumn.DataPropertyName = Resource.Col3;
            comboBoxColumn.HeaderText = Resource.Col3;
            comboBoxColumn.Name = Resource.Col3;

            //list of datatypes
            comboBoxColumn.DataSource = listDataTypes;
            dgvBDM.Columns.Add(comboBoxColumn);
            //bind rest of columns
            BindingSource bs = new BindingSource(GlobalData.DataFiltered.Objects, null);
            dgvBDM.DataSource = bs;

            //register events for sorting and filtering
            dgvBDM.SortStringChanged += new EventHandler(dgv_SortStringChanged);
            dgvBDM.FilterStringChanged += new EventHandler(dgv_FilterStringChanged);
            #endregion

            #region archestra init
            OnUpdateStatus += new StatusUpdateHandler(StatusUpdated);
            backgroundWorker1 = new BackgroundWorker();            
            backgroundWorker1.WorkerReportsProgress = true;            
            #endregion

            #region promotic init
            Promotic.OnUpdateStatus += new Promotic.StatusUpdateHandler(StatusUpdated_promotic);     //register event for progress status update

            //background workers settings and event register
            bw_BuildXML = new BackgroundWorker();
            bw_BuildXML.WorkerReportsProgress = true;
            bw_BuildXML.DoWork += bw_BuildXML_DoWork;
            bw_BuildXML.ProgressChanged += bw_BuildXML_ProgressChanged;
            bw_BuildXML.RunWorkerCompleted += bw_BuildXML_RunWorkerCompleted;

            bw_BuildXMLpart = new BackgroundWorker();
            bw_BuildXMLpart.WorkerReportsProgress = true;
            bw_BuildXMLpart.DoWork += bw_BuildXMLpart_DoWork;
            bw_BuildXMLpart.ProgressChanged += bw_BuildXMLpart_ProgressChanged;
            bw_BuildXMLpart.RunWorkerCompleted += bw_BuildXMLpart_RunWorkerCompleted;

            bw_DownloadPmiLibs = new BackgroundWorker();
            bw_DownloadPmiLibs.WorkerReportsProgress = true;
            bw_DownloadPmiLibs.DoWork += bw_DownloadPmiLibs_DoWork;
            bw_DownloadPmiLibs.ProgressChanged += bw_DownloadPmiLibs_ProgressChanged;
            bw_DownloadPmiLibs.RunWorkerCompleted += bw_DownloadPmiLibs_RunWorkerCompleted;

            bw_DownloadResources = new BackgroundWorker();
            bw_DownloadResources.WorkerReportsProgress = true;
            bw_DownloadResources.DoWork += bw_DownloadResources_DoWork;
            bw_DownloadResources.ProgressChanged += bw_DownloadResources_ProgressChanged;
            bw_DownloadResources.RunWorkerCompleted += bw_DownloadResources_RunWorkerCompleted;
            #endregion

            #region TIA init                        
            #endregion
        }
        

        public void listChange()
        {
            listDataTypes.Clear();
            foreach (var item in GlobalData.aotToString())
            {
                listDataTypes.Add(item);
            }
        }

        #region custom progress event
        /// <summary>
        /// Custom eventargs to pass status progress
        /// </summary>
        public class ProgressEventArgs : EventArgs
        {
            public int Status { get; private set; }
            public string Msg { get; private set; }

            public ProgressEventArgs(double status, string msg)
            {
                Status = (int)status;
                Msg = msg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        private static void UpdateStatus(double status, string msg)
        {
            // Make sure someone is listening to event
            if (OnUpdateStatus == null) return;

            ProgressEventArgs args = new ProgressEventArgs(status, msg);
            OnUpdateStatus(null, args);
        }

        private void StatusUpdated(object sender, ProgressEventArgs e)
        {
            //Report only that process which is being proceed now

            if(backgroundWorker1.IsBusy)
                backgroundWorker1.ReportProgress(e.Status, e.Msg);
        }

        private void StatusUpdated_promotic(object sender, Promotic.ProgressEventArgs e)
        {
            //Report only that process which is being proceed now

            if (bw_BuildXML.IsBusy)
                bw_BuildXML.ReportProgress(e.Status, e.Msg);

            if (bw_BuildXMLpart.IsBusy)
                bw_BuildXMLpart.ReportProgress(e.Status, e.Msg);

            if (bw_DownloadPmiLibs.IsBusy)
                bw_DownloadPmiLibs.ReportProgress(e.Status, e.Msg);

            if (bw_DownloadResources.IsBusy)
                bw_DownloadResources.ReportProgress(e.Status, e.Msg);
        }
        #endregion

        #region table control

        #region Filtering

        /// <summary>
        /// User has used sort on ADGV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgv_SortStringChanged(object sender, EventArgs e)
        {
            GlobalData.CopyBDM(true);       //copy Data->DataFiletered
            Sorting(dgvBDM.SortString);     //sort it

            //if validation checkbox active, display invalids
            if (checkBoxValid.Checked)
            {
                ClearInvalids();
                DisplayInvalids();
            }
        }

        /// <summary>
        /// Sort filtered data according sortstring from ADGV
        /// </summary>
        /// <param name="sortString"></param>
        void Sorting(string sortString)
        {
            List<BasicObject> sortedList = null;

            if (sortString != "")
            {
                //get all sorting options
                string[] sorts = sortString.Split(',');

                //get match only on last sorting option
                string[] matches = Regex.Matches(sorts.Last(), @"\w+").Cast<Match>().Select(x => x.Value).ToArray();

                //ascending
                if (matches[1] == "ASC")
                {
                    sortedList = new List<BasicObject>(GlobalData.DataFiltered.Objects.OrderBy(x => x.GetPropValue(matches[0])).ToList());
                }
                //descending
                else
                {
                    sortedList = new List<BasicObject>(GlobalData.DataFiltered.Objects.OrderByDescending(x => x.GetPropValue(matches[0])).ToList());
                }

                //clear FilteredData and pupulate with new
                GlobalData.DataFiltered.Objects.Clear();
                foreach (var obj in sortedList)
                {
                    GlobalData.DataFiltered.Objects.Add(obj);
                }
            }
        }

        /// <summary>
        /// Usere has used filter on ADGV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgv_FilterStringChanged(object sender, EventArgs e)
        {
            GlobalData.CopyBDM(true);       //copy Data->DataFiletered            
            Filtering(dgvBDM.FilterString); //filter it

            //if validation checkbox active, display invalids
            if (checkBoxValid.Checked)
            {
                ClearInvalids();
                DisplayInvalids();
            }
        }

        /// <summary>
        /// Filter DataFiltered according to filterstring from ADGV
        /// </summary>
        /// <param name="filterString"></param>
        void Filtering(string filterString)
        {
            string[] filters;   //array of strings

            //get all filters on all columns
            filters = filterString.Split(new string[] { " AND " }, StringSplitOptions.None);    //array of filters

            //filter data according to each filter
            foreach (var item in filters)
            {
                if (item.Contains("LIKE"))
                    FilterLIKE(item);
                if (item.Contains("IN"))
                    FilterIN(item);
            }
        }

        /// <summary>
        /// Filter LIKE - contains, endwith, startwith, equal + NOTs
        /// </summary>
        /// <param name="filterString"></param>
        void FilterLIKE(string filterString)
        {
            Regex regexWhere = new Regex(@"\[\w+\]");
            Regex regexNoSpec = new Regex(@"\w+");

            //get column name
            string col = regexNoSpec.Match(regexWhere.Match(filterString).Value).Value;

            //check if equal, contains, startwith or endwith

            IEnumerable<BasicObject> filtered = null;

            if (filterString.Contains("NOT LIKE"))
            {
                //not contains
                if (Regex.Match(filterString, @"\'\%\w+\%\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => !x.GetPropValue(col).Contains(regexNoSpec.Match(Regex.Match(filterString, @"\'\%\w+\%\'").Value).Value)).ToList();
                //not endswith
                if (Regex.Match(filterString, @"\'\%\w+\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => !x.GetPropValue(col).EndsWith(regexNoSpec.Match(Regex.Match(filterString, @"\'\%\w+\'").Value).Value)).ToList();
                //not startswith
                if (Regex.Match(filterString, @"\'\w+\%\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => !x.GetPropValue(col).StartsWith(regexNoSpec.Match(Regex.Match(filterString, @"\'\w+\%\'").Value).Value)).ToList();
                //not equal
                if (Regex.Match(filterString, @"\'\w+\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => x.GetPropValue(col) != (regexNoSpec.Match(Regex.Match(filterString, @"\'\w+\'").Value).Value)).ToList();
            }
            else
            {
                //contains
                if (Regex.Match(filterString, @"\'\%\w+\%\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => x.GetPropValue(col).Contains(regexNoSpec.Match(Regex.Match(filterString, @"\'\%\w+\%\'").Value).Value)).ToList();
                //endswith
                if (Regex.Match(filterString, @"\'\%\w+\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => x.GetPropValue(col).EndsWith(regexNoSpec.Match(Regex.Match(filterString, @"\'\%\w+\'").Value).Value)).ToList();
                //startswith
                if (Regex.Match(filterString, @"\'\w+\%\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => x.GetPropValue(col).StartsWith(regexNoSpec.Match(Regex.Match(filterString, @"\'\w+\%\'").Value).Value)).ToList();
                //equal
                if (Regex.Match(filterString, @"\'\w+\'").Success)
                    filtered = GlobalData.DataFiltered.Objects.Where(x => x.GetPropValue(col) == (regexNoSpec.Match(Regex.Match(filterString, @"\'\w+\'").Value).Value)).ToList();
            }

            //if filtered not null, add all objs to DataFiltered global var
            if (filtered != null)
            {
                GlobalData.DataFiltered.Objects.Clear();
                foreach (var obj in filtered)
                {
                    GlobalData.DataFiltered.Objects.Add(obj);
                }
            }
        }

        /// <summary>
        /// Filter IN
        /// </summary>
        /// <param name="filterString"></param>
        void FilterIN(string filterString)
        {
            Regex regexWhere = new Regex(@"\[\w+\]");
            Regex regexNoSpec = new Regex(@"\w+");

            //get column name
            string col = regexNoSpec.Match(regexWhere.Match(filterString).Value).Value;

            List<BasicObject> filtered = new List<BasicObject>();

            //get all matches to have filters values any word characters
            List<string> insList = Regex.Matches(filterString.Substring(filterString.IndexOf("IN") + 3), @"\w+").Cast<Match>().Select(x => x.Value).ToList();

            //check if ' ' selected to filter and add it to list
            if (Regex.Match(filterString.Substring(filterString.IndexOf("IN") + 3), @"'\s+'").ToString().Equals("' '"))
            {
                insList.Add(" ");
            }

            //compare filters with data and populate List filtered
            foreach (var str in insList)
            {
                foreach (var obj in GlobalData.DataFiltered.Objects)
                {
                    if (obj.GetPropValue(col).Equals(str))
                        filtered.Add(obj);
                }
            }

            //if List filtered not 0, move all objs to DataFiltered global var
            if (filtered.Count > 0)
            {
                GlobalData.DataFiltered.Objects.Clear();
                foreach (var obj in filtered)
                {
                    GlobalData.DataFiltered.Objects.Add(obj);
                }
            }
        }
        #endregion

        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //Save data to file
            if (GlobalData.Data.SaveSerializedToFile())
            {
                MBox mbox = new MBox("Done", "Saved");
            }
        }

        private void loadToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //Load data from file            
            GlobalData.Data.LoadDeSerializedFromFile();
            GlobalData.CopyBDM(true);
        }

        private void dgvBDM_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            return;
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //remove filters
            dgvBDM.ClearFilter(true);
            dgvBDM.ClearSort(true);

            //save
            GlobalData.CopyBDM(false);          //copy DataFiltered -> Data
            GlobalData.SerializeDataTypes(path);
            GlobalData.Data.SaveSerialized();   //save to temp bdm.xml
        }

        private void clearFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvBDM.ClearFilter(true);
            dgvBDM.ClearSort(true);
        }

        private void dgvBDM_KeyDown(object sender, KeyEventArgs e)
        {
            #region delete

            //if pressed DELETE key, delete selected cells / cells value
            if (e.KeyCode == Keys.Delete)
            {
                //if selected 1 or more whole rows, must be removed also BasicObjects
                if (dgvBDM.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvBDM.SelectedRows)
                    {
                        try
                        {
                            //find object to remove
                            BasicObject objToRemove = GlobalData.Data.Objects.SingleOrDefault(x => x.TagName.Equals(row.Cells[0].Value));
                            //remove it
                            GlobalData.Data.Objects.Remove(objToRemove);
                        }
                        catch { }
                    }
                }
                else //if not whole row selected, delete only value of selected cells
                {
                    foreach (DataGridViewCell SC in dgvBDM.SelectedCells)
                    {
                        SC.Value = "";      //set empty value for all cells
                    }
                }
            }
            #endregion

            #region paste

            //if user pressed CTRL + V
            if (e.KeyCode == Keys.V && e.Control)
            {
                char[] rowSplitter = { '\r', '\n' };
                char[] columnSplitter = { '\t' };

                //get data from clipboard - anytime you CTRL+C something, works from excel too
                IDataObject dataInClipboard = Clipboard.GetDataObject();
                string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);

                //if any data in clipboard
                if (!string.IsNullOrEmpty(stringInClipboard))
                {
                    string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);     //split rows
                    string[] columnsInClipboard = stringInClipboard.Split(columnSplitter, StringSplitOptions.RemoveEmptyEntries);  //split cols

                    //more then 1 cell in clipboard -> paste selection must be 1
                    if (rowsInClipboard.Count() > 1 || columnsInClipboard.Count() > 1)
                    {
                        int r = dgvBDM.SelectedCells[0].RowIndex;
                        int c = dgvBDM.SelectedCells[0].ColumnIndex;
                        int nr = r + rowsInClipboard.Length - dgvBDM.Rows.Count;
                        int newRows = (r + rowsInClipboard.Length + 1) - dgvBDM.Rows.Count;

                        //if you pasting cells over table range
                        if (newRows > 0)
                        {                            
                            for (int i = 0; i < newRows; i++)
                            {
                                GlobalData.DataFiltered.Objects.Add(new BasicObject());
                            }
                        }

                        //if you select more then 1 cell as pasting area

                        if (dgvBDM.SelectedCells.Count > 1 && dgvBDM.SelectedRows.Count == 0)
                        {
                            MessageBox.Show("Select only one cell as an insert point");
                        }
                        else
                        {
                            foreach (string CBRS in rowsInClipboard)
                            {
                                string[] CellValue = CBRS.Split(columnSplitter, StringSplitOptions.RemoveEmptyEntries);    //split columns
                                foreach (string CBCS in CellValue)
                                {
                                    try
                                    {
                                        dgvBDM.Rows[r].Cells[c].Value = CBCS;
                                    }
                                    catch
                                    {
                                    }
                                    c = c + 1;
                                }
                                c = dgvBDM.SelectedCells[0].ColumnIndex;
                                r = r + 1;
                            }
                        }
                    }

                    //only 1 cell is coppied -> more then 1 cell can be selected for paste
                    else
                    {
                        foreach (DataGridViewCell cell in dgvBDM.SelectedCells)
                        {
                            cell.Value = rowsInClipboard[0].ToString();
                        }
                    }
                }
            }
            #endregion

            #region replace or find

            //if user press CTRL + F or CTRL + H
            if ((e.KeyCode == Keys.H || e.KeyCode == Keys.F) && e.Control)
            {
                ReplaceForm replaceForm = new ReplaceForm(dgvBDM);
                replaceForm.Show();
            }

            #endregion
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MBox msgbox = new MBox("Warning", "Delete all data?");

            if (msgbox.Result)
            {
                GlobalData.Data.Objects.Clear();    //clear all objects from Data
                GlobalData.DBMappingRef.Clear();//clear data mapping
                GlobalData.CopyBDM(true);           //copy cleared Data -> DataFiltered
            }
        }

        private void ClearInvalids()
        {
            //clear background in whole table
            foreach (DataGridViewRow row in dgvBDM.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = System.Drawing.Color.White;
                }
            }
        }

        /// <summary>
        /// Display invalids coloured in table
        /// </summary>
        /// <returns></returns>
        private bool DisplayInvalids()
        {
            //find invalids
            var invalids = GlobalData.DataFiltered.Invalid();

            if (invalids != null)
            {
                //loop thru all founds invalids
                foreach (var invalid in invalids)
                {
                    //loop thru all rows in table
                    foreach (DataGridViewRow row in dgvBDM.Rows)
                    {
                        //try-catch block because of null reference of nonfilled DataType column
                        try
                        {
                            if (row.Cells[invalid.Column].Value.ToString().Equals(invalid.Value))
                            {
                                InvPtrs.Add(row.Index);
                                //set yellow whole row where some invalid found
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    cell.Style.BackColor = System.Drawing.Color.Yellow;
                                }

                                //set red specific cell which is invalid 
                                row.Cells[invalid.Column].Style.BackColor = System.Drawing.Color.Red;
                            }
                        }
                        catch
                        {
                            //if null reference, only whole row is yellowed
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                cell.Style.BackColor = System.Drawing.Color.Yellow;
                            }
                        }
                    }
                }

                //set last row to white background - shouldn't be checked for validity
                foreach (DataGridViewCell cell in dgvBDM.Rows[dgvBDM.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells)
                {
                    cell.Style.BackColor = System.Drawing.Color.White;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void checkBoxValid_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxValid.Checked)
            {
                ClearInvalids();
                DisplayInvalids();
                btnNextInvalid.Enabled = true;
            }
            else
            {
                ClearInvalids();
                btnNextInvalid.Enabled = false;
            }
        }

        private void dgvBDM_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (checkBoxValid.Checked)
            {
                ClearInvalids();
                DisplayInvalids();
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //remove filters
            dgvBDM.ClearFilter(true);
            dgvBDM.ClearSort(true);

            //load            
            GlobalData.Data.LoadDeSerialized();
            GlobalData.CopyBDM(true);
        }

        /// <summary>
        /// Scroll to next invalid row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextInvalid_Click(object sender, EventArgs e)
        {
            if (InvPtrs.Count > 0)
            {
                //scroll and select found invalid cell
                dgvBDM.FirstDisplayedCell = dgvBDM[1, InvPtrs[InvCtr]];   
                dgvBDM.ClearSelection();
                dgvBDM[0, InvPtrs[InvCtr]].Selected = true;

                if (InvCtr < InvPtrs.Count - 1)
                    InvCtr++;
                else
                    InvCtr = 0;
            }
        }

        /// <summary>
        /// Add row clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalData.DataFiltered.Objects.Add(new BasicObject());
        }

        /// <summary>
        /// Add more rows clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputMsgBox imb = new InputMsgBox();
            for (int i = 0; i < imb.newRowsNmb; i++)
            {
                GlobalData.DataFiltered.Objects.Add(new BasicObject());
            }
        }

        /// <summary>
        /// Filter columns checkbox clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxFilterCols_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox localCB = (CheckBox)sender;

            if (!localCB.Checked)
            {
                foreach (DataGridViewColumn col in dgvBDM.Columns)
                {
                    col.Visible = true;
                }
            }
        }

        /// <summary>
        /// Row double clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvBDM_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {            

            if (checkBoxFilterCols.Checked)
            {
                string clickedDatatype = dgvBDM.Rows[e.RowIndex].Cells[Resource.Col3].Value.ToString();
                List<string> FilterCols = new List<string>();

                //Default columns visible for all
                FilterCols.Add(Resource.Col1);
                FilterCols.Add(Resource.Col2);
                FilterCols.Add(Resource.Col3);
                FilterCols.Add(Resource.Col4);
                FilterCols.Add(Resource.Col5);
                FilterCols.Add(Resource.Col6);
                FilterCols.Add(Resource.Col51);
                FilterCols.Add(Resource.Col53);
                FilterCols.Add(Resource.Col54);

                //Select what to show up to datatype
                #region Datatype selection
                if (clickedDatatype.Equals(Resource.DinType))
                {
                    FilterCols.Add(Resource.Col7);
                    FilterCols.Add(Resource.Col8);
                    FilterCols.Add(Resource.Col9);
                    FilterCols.Add(Resource.Col10);
                    FilterCols.Add(Resource.Col52);
                    
                }
                else if (clickedDatatype.Equals(Resource.DoutType))
                {
                    FilterCols.Add(Resource.Col11);
                    FilterCols.Add(Resource.Col52);
                }
                else if (clickedDatatype.Equals(Resource.AinType))
                {
                    FilterCols.Add(Resource.Col12);
                    FilterCols.Add(Resource.Col13);
                    FilterCols.Add(Resource.Col14);
                    FilterCols.Add(Resource.Col15);
                    FilterCols.Add(Resource.Col16);
                    FilterCols.Add(Resource.Col17);
                    FilterCols.Add(Resource.Col18);
                    FilterCols.Add(Resource.Col19);
                    FilterCols.Add(Resource.Col20);
                    FilterCols.Add(Resource.Col21);
                    FilterCols.Add(Resource.Col22);
                    FilterCols.Add(Resource.Col23);
                    FilterCols.Add(Resource.Col24);
                    FilterCols.Add(Resource.Col25);
                    FilterCols.Add(Resource.Col26);
                    FilterCols.Add(Resource.Col27);
                    FilterCols.Add(Resource.Col28);
                    FilterCols.Add(Resource.Col29);
                    FilterCols.Add(Resource.Col30);
                    FilterCols.Add(Resource.Col31);
                    FilterCols.Add(Resource.Col32);
                    FilterCols.Add(Resource.Col33);
                    FilterCols.Add(Resource.Col52);
                }
                else if (clickedDatatype.Equals(Resource.AoutType))
                {
                    FilterCols.Add(Resource.Col12);
                    FilterCols.Add(Resource.Col13);
                    FilterCols.Add(Resource.Col14);
                    FilterCols.Add(Resource.Col15);
                    FilterCols.Add(Resource.Col52);
                }
                else if (clickedDatatype.Equals(Resource.OnOffType) || clickedDatatype.Equals(Resource.OnOff2DType) || clickedDatatype.Equals(Resource.OnOffVSDType))
                {
                    FilterCols.Add(Resource.Col34);
                    FilterCols.Add(Resource.Col35);
                    FilterCols.Add(Resource.Col36);
                    FilterCols.Add(Resource.Col37);
                    FilterCols.Add(Resource.Col38);
                    FilterCols.Add(Resource.Col39);
                    FilterCols.Add(Resource.Col40);
                    FilterCols.Add(Resource.Col41);
                    FilterCols.Add(Resource.Col42);
                    FilterCols.Add(Resource.Col43);
                    FilterCols.Add(Resource.Col44);
                    FilterCols.Add(Resource.Col45);
                    FilterCols.Add(Resource.Col46);
                    FilterCols.Add(Resource.Col47);
                }
                else if (clickedDatatype.Equals(Resource.PreselType))
                {
                    FilterCols.Add(Resource.Col48);
                }

                #endregion

                //Hide all columns
                foreach (DataGridViewColumn col in dgvBDM.Columns)
                {
                    col.Visible = false;
                }

                //Display only columns selected up to datatype
                foreach (string s in FilterCols)
                {
                    dgvBDM.Columns[s].Visible = true;
                }
            }
        }

        #endregion
        public static void DeserializeHeaders()
        {
            string path = "";
            RegistryKey RegK = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Excel\\Addins\\BDM"); // get instalation path from registry
            if (RegK != null)
            {
                path = RegK.GetValue("Manifest").ToString();
                path = path.Replace("BDM.vsto", "XMLSources/Headers.xml");
                path = path.Replace("file:///", "");
            }
            try
            {
                if (File.Exists(path))
                {
                    XmlSerializer serializer = new XmlSerializer(_header.GetType());
                    using (StreamReader sr = new StreamReader(path))
                    {
                        _header = (BindingList<HeaderCls>)serializer.Deserialize(sr);
                    }
                }
                else
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        if (Resource.ResourceManager.GetString("Col" + i) != null)
                        {
                            HeaderCls he = new HeaderCls();
                            he.HeaderName = Resource.ResourceManager.GetString("Col" + i);
                            _header.Add(he);
                        }
                    }
                    foreach (var item in _header)
                    {
                        item.Visible = true;
                    }
                    //throw new FileNotFoundException("File not found!" + Environment.NewLine + "Headers generated automatically");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #region archestra control
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicObject BO = GlobalData.Data.Objects.First(x => x.TagName == "AIS1");
            BO.Descr = 
            saveDialog.Filter = "CSV (*.csv)|*.csv";
            DeserializeHeaders();

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {             
                Z45AIFC.ExportData(saveDialog.FileName, GlobalData, _header);
            }
            MessageBox.Show("Export Done");
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "CSV (*.csv)|*.csv";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                Z45AIFC.ImportData(File.ReadAllText(OpenFile.FileName), GlobalData);
            }
                     
        }

        #endregion

        #region promotic control

        #region Background workers

        //BuildXML
        void bw_BuildXML_DoWork(object sender, DoWorkEventArgs e)
        {
            Promotic.BuildXML(GlobalData.DataFiltered, (string)e.Argument);
        }

        //progress update while building xml
        void bw_BuildXML_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressLabel.Text = (string)e.UserState;
        }

        //run worker completed
        void bw_BuildXML_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MBox msg = new MBox("Info", "Promotic XML file generation done.");
        }

        //BuildXMLpart
        void bw_BuildXMLpart_DoWork(object sender, DoWorkEventArgs e)
        {
            Promotic.BuildXMLPart(GlobalData.DataFiltered, (string)e.Argument);
        }

        //progress update while building xml part
        void bw_BuildXMLpart_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressLabel.Text = (string)e.UserState;
        }

        //run worker completed
        void bw_BuildXMLpart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MBox msg = new MBox("Info", "Promotic XML file generation done.");
        }

        //DownloadPmiLibs
        void bw_DownloadPmiLibs_DoWork(object sender, DoWorkEventArgs e)
        {
            Promotic.DownloadPmiLibs((string)e.Argument);
        }

        //progress update while downloading pmi libs
        void bw_DownloadPmiLibs_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressLabel.Text = (string)e.UserState;
        }

        //run worker completed
        void bw_DownloadPmiLibs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MBox msg = new MBox("Info", "Promotic libraries updated.");
        }

        //DownloadResources
        void bw_DownloadResources_DoWork(object sender, DoWorkEventArgs e)
        {
            Promotic.DownloadResource((string)e.Argument);
        }

        //progress update while downloading resources
        void bw_DownloadResources_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressLabel.Text = (string)e.UserState;
        }
        #endregion

        //run worker completed
        void bw_DownloadResources_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MBox msg = new MBox("Info", "Promotic resources updated.");
        }

        private void completeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if data are invalid, xml isn't build
            if (GlobalData.DataFiltered.Invalid() != null)
            {
                MessageBox.Show("Invalid data, use validity check.");
            }
            else
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                    bw_BuildXML.RunWorkerAsync(saveDialog.FileName);    //pass filename path to backgroundworker
            }
        }

        private void graphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog form = new FolderBrowserDialog();
            form.Description = "Locate Promotic install folder.";

            if (form.ShowDialog() == DialogResult.OK)
                bw_DownloadPmiLibs.RunWorkerAsync(form.SelectedPath);   //pass filename path to backgroundworker
        }

        private void faceplateResourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog form = new FolderBrowserDialog();
            form.Description = "Locate Promotic project folder.";

            if (form.ShowDialog() == DialogResult.OK)
                bw_DownloadResources.RunWorkerAsync(form.SelectedPath); //pass filename path to backgroundworker
        }

        private void coreFolderOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if data are invalid, xml part isn't build
            if (GlobalData.Data.Invalid() != null)
            {
                MessageBox.Show("Invalid data. Check Main tab.");
            }
            else
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                    bw_BuildXMLpart.RunWorkerAsync(saveDialog.FileName);    //pass filename path to backgroundworker
            }
        }
        #endregion

        #region TIA control        
        
        private void IOListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            progressBar.Step = 1;            
            SaveFileDialog TIAExtSrcFile = new SaveFileDialog();
            TIAExtSrcFile.Filter = "xlsx File (*.xlsx)|*.xlsx";
            TIAExtSrcFile.ShowDialog();
            if (!string.IsNullOrEmpty(TIAExtSrcFile.FileName))
            {
                progressBar.Maximum = 100;
                progressBar.Value = 50;
                Z45TIAIFC.GeneratePLCTags(GlobalData.DataFiltered.Objects, TIAExtSrcFile.FileName.Replace(".xlsx", ""));
                progressBar.Value = 100;
                MessageBox.Show(TIAExtSrcFile.FileName.Split('\\').Last() + " created");
                progressBar.Value = 0;
            }
            SetProgressDefault();
        }

        private void importToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (GlobalData.DBMappingRef.Count == 0)
            {
                MessageBox.Show("Check DB Mapping");
            }
            else
            {
                SetProgressDefault();
                progressBar.Step = 1;
                string DBResourceTextDBs = "";
                string DBResourceTextFCs = "";
                DBnames = GlobalData.DBMappingRef;
                SaveFileDialog TIAExtSrcFile = new SaveFileDialog();
                TIAExtSrcFile.ShowDialog();
                if (!string.IsNullOrEmpty(TIAExtSrcFile.FileName))
                {
                    progressBar.Maximum = DBnames.Count;
                    progressBar.Value = 0;
                    foreach (var DBMapping in DBnames)
                    {
                        progressBar.PerformStep();
                        if (!string.IsNullOrEmpty(DBMapping.DBName))
                        {
                            DBResourceTextDBs = DBResourceTextDBs + Z45TIAIFC.GenerateExtSrcDB(GlobalData.DataFiltered.Objects, DBMapping.DBName);
                            DBResourceTextFCs = DBResourceTextFCs + Z45TIAIFC.GenerateExtSrcFC(GlobalData.DataFiltered.Objects, DBMapping.DBName, GlobalData.AObjectTypes);
                        }
                    }
                    File.WriteAllText(TIAExtSrcFile.FileName + ".db", DBResourceTextDBs);
                    File.WriteAllText(TIAExtSrcFile.FileName + ".scl", DBResourceTextFCs);
                    MessageBox.Show("External source files created");
                    progressBar.Value = 0;
                }
                SetProgressDefault();
            }           
        }

        private void SetProgressDefault()
        {
            progressBar.Step = 10;
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;

            progressLabel.Text = "Progress";
        }
    
        #endregion

        #region Step7 control
        /// <summary>
        /// Enumerator to get datatypes offset in db
        /// </summary>
        private enum DataTypeEnum
        {
            DinData = 6,
            DoutData = 6,
            AinData = 44,
            AoutData = 24,
            F_DinData = 282,
            F_DoutData = 156,
            F_AinData = 510,
            F_AoutData = 510,
            OnOffCtrlData = 20,
            OnOffCtrlData_VSD = 40,
            OnOffCtrlData_2D = 26,
            AnalogPosCtrlData = 62,
            GrpData = 30,
            PreselData = 4,
            PIDCtrlData = 140,
            PIDStepCtrlData = 140
        }

        /// <summary>
        /// Calculate db offsets up to dbnames - offline function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculateDBS7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get list of uniques
            List<BasicObject> uniqueItemsList = GlobalData.DataFiltered.Objects.GroupBy(x => x.DBName).Select(group => group.First()).ToList();

            int offset;

            foreach (var uniqueItem in uniqueItemsList)
            {
                //initialize offset
                offset = 0;

                foreach (var obj in GlobalData.DataFiltered.Objects)
                {
                    if (obj.DBName == uniqueItem.DBName)
                    {
                        obj.PositionInDB = offset.ToString();
                        try
                        {
                            if (obj.DataType != null)
                            {
                                offset += (int)Enum.Parse(typeof(DataTypeEnum), obj.DataType);
                            }
                            else throw new Exception("Wrong DataType!");
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }

            dgvBDM.Refresh();
        }

        /// <summary>
        /// Generate source codes for step7
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateSourceS7ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            progressBar.Step = 1;
            string DBResourceTextDBs = "";
            string DBResourceTextFCs = "";
            DBnames = GlobalData.DBMappingRef;
            SaveFileDialog TIAExtSrcFile = new SaveFileDialog();
            TIAExtSrcFile.ShowDialog();
            if (!string.IsNullOrEmpty(TIAExtSrcFile.FileName))
            {
                progressBar.Maximum = DBnames.Count;
                progressBar.Value = 0;
                foreach (var DBMapping in DBnames)
                {
                    progressBar.PerformStep();
                    if (!string.IsNullOrEmpty(DBMapping.DBName))
                    {
                        DBResourceTextDBs = DBResourceTextDBs + GenerateExtSrcDB(GlobalData.DataFiltered.Objects, DBMapping.DBName);
                        DBResourceTextFCs = DBResourceTextFCs + GenerateExtSrcFC(GlobalData.DataFiltered.Objects, DBMapping.DBName);
                    }
                }
                File.WriteAllText(TIAExtSrcFile.FileName + "DBs.scl", DBResourceTextDBs);
                File.WriteAllText(TIAExtSrcFile.FileName + "FCs.scl", DBResourceTextFCs);
                MessageBox.Show("External source files created");
                progressBar.Value = 0;
            }
            SetProgressDefault();
        }

        private string GenerateExtSrcDB(BindingList<BasicObject> ImportData, string DBName)
        {
            string SrcCode = "";
            SrcCode = "DATA_BLOCK " + DBName + "" + Environment.NewLine;
            //SrcCode = SrcCode + "{ S7_Optimized_Access := 'FALSE' }" + Environment.NewLine;
            //SrcCode = SrcCode + "VERSION : 0.1" + Environment.NewLine;
            //SrcCode = SrcCode + "NON_RETAIN" + Environment.NewLine;
            SrcCode = SrcCode + "   STRUCT" + Environment.NewLine;
            foreach (var item in ImportData)
            {
                if (item.DBName == DBName)
                {
                    SrcCode = SrcCode + "       " + item.TagName + ":" + item.DataType + "; //" + item.Descr + Environment.NewLine;
                }
            }
            SrcCode = SrcCode + "   END_STRUCT;" + Environment.NewLine;
            SrcCode = SrcCode + "BEGIN" + Environment.NewLine;
            SrcCode = SrcCode + "END_DATA_BLOCK" + Environment.NewLine;
            return SrcCode;
        }

        private string GenerateExtSrcFC(BindingList<BasicObject> ImportData, string DBName)
        {
            string SrcCode = "";
            SrcCode = "FUNCTION " + DBName + "_FC : Void" + Environment.NewLine;
            //SrcCode = SrcCode + "{ S7_Optimized_Access := 'false' }" + Environment.NewLine;
            //SrcCode = SrcCode + "VERSION : 0.1" + Environment.NewLine;
            SrcCode = SrcCode + "BEGIN" + Environment.NewLine;
            foreach (var item in ImportData)
            {
                if (item.DBName == DBName)
                {
                    if (item.DataType == Resource.AinType)
                    {
                        SrcCode = SrcCode + ADD_AIN_FC(item);
                    }
                    else if (item.DataType == Resource.AnalogPosType)
                    {
                        SrcCode = SrcCode + ADD_AnalogPos_FC(item);
                    }
                    else if (item.DataType == Resource.AoutType)
                    {
                        SrcCode = SrcCode + ADD_Aout_FC(item);
                    }
                    else if ((item.DataType == Resource.DinType))
                    {
                        SrcCode = SrcCode + ADD_Din_FC(item);
                    }
                    else if ((item.DataType == Resource.DoutType))
                    {
                        SrcCode = SrcCode + ADD_Dout_FC(item);
                    }
                    else if ((item.DataType == Resource.GrpType))
                    {
                        SrcCode = SrcCode + ADD_GrpCtrl_FC(item);
                    }
                    else if ((item.DataType == Resource.OnOffType))
                    {
                        SrcCode = SrcCode + ADD_OnOffCtrl_FC(item);
                        if (!string.IsNullOrEmpty(item.StartStep))
                        {
                            SrcCode = SrcCode + ADD_SGC_FC(item);
                        }
                    }
                    else if ((item.DataType == Resource.OnOff2DType))
                    {
                        SrcCode = SrcCode + ADD_OnOffCtrl_2D_FC(item);
                        if (!string.IsNullOrEmpty(item.StartStep))
                        {
                            SrcCode = SrcCode + ADD_SGC_FC(item);
                        }
                    }
                    else if ((item.DataType == Resource.OnOffVSDType))
                    {
                        SrcCode = SrcCode + ADD_OnOffCtrl_VSD_FC(item);
                        if (!string.IsNullOrEmpty(item.StartStep))
                        {
                            SrcCode = SrcCode + ADD_SGC_FC(item);
                        }
                    }
                    else if ((item.DataType == Resource.PIDType))
                    {
                        SrcCode = SrcCode + ADD_PID_FC(item);
                    }
                    else if ((item.DataType == Resource.PreselType))
                    {
                        SrcCode = SrcCode + ADD_Presel_FC(item);
                    }
                    SrcCode = SrcCode + Environment.NewLine;
                }
            }
            SrcCode = SrcCode + "END_FUNCTION" + Environment.NewLine;

            return SrcCode;
        }

        # region Public method - Add Analog input function in to the source file
        /// <summary>
        /// Add Analog input function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_AIN_FC(BasicObject BO)
        {
            string FCstring;
            string Bipolar;
            string FBConfig;
            string MAX;
            string MIN;
            string Init_HH_Dly;
            string Init_H_Dly;
            string Init_L_Dly;
            string Init_LL_Dly;
            string AL_DB;
            string AE_HH_Cfg;
            string AE_H_Cfg;
            string AE_L_Cfg;
            string AE_LL_Cfg;
            string Init_HH_Lvl;
            string Init_H_Lvl;
            string Init_L_Lvl;
            string Init_LL_Lvl;
            if (string.IsNullOrEmpty(BO.Bipolar)) { Bipolar = "false"; } else { Bipolar = BO.Bipolar; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            if (string.IsNullOrEmpty(BO.MAX)) { MAX = "100.0"; } else { MAX = BO.MAX; }
            if (string.IsNullOrEmpty(BO.MIN)) { MIN = "0.0"; } else { MIN = BO.MIN; }
            if (string.IsNullOrEmpty(BO.Init_HH_Dly)) { Init_HH_Dly = "0"; } else { Init_HH_Dly = BO.Init_HH_Dly; }
            if (string.IsNullOrEmpty(BO.Init_H_Dly)) { Init_H_Dly = "0"; } else { Init_H_Dly = BO.Init_H_Dly; }
            if (string.IsNullOrEmpty(BO.Init_L_Dly)) { Init_L_Dly = "0"; } else { Init_L_Dly = BO.Init_L_Dly; }
            if (string.IsNullOrEmpty(BO.Init_LL_Dly)) { Init_LL_Dly = "0"; } else { Init_LL_Dly = BO.Init_LL_Dly; }
            if (string.IsNullOrEmpty(BO.AL_DB)) { AL_DB = "0.5"; } else { AL_DB = BO.AL_DB; }
            if (string.IsNullOrEmpty(BO.AE_HH_Cfg)) { AE_HH_Cfg = "0"; } else { AE_HH_Cfg = BO.AE_HH_Cfg; }
            if (string.IsNullOrEmpty(BO.AE_H_Cfg)) { AE_H_Cfg = "0"; } else { AE_H_Cfg = BO.AE_H_Cfg; }
            if (string.IsNullOrEmpty(BO.AE_L_Cfg)) { AE_L_Cfg = "0"; } else { AE_L_Cfg = BO.AE_L_Cfg; }
            if (string.IsNullOrEmpty(BO.AE_LL_Cfg)) { AE_LL_Cfg = "0"; } else { AE_LL_Cfg = BO.AE_LL_Cfg; }
            if (string.IsNullOrEmpty(BO.Init_HH_Lvl)) { Init_HH_Lvl = "95.0"; } else { Init_HH_Lvl = BO.Init_HH_Lvl; }
            if (string.IsNullOrEmpty(BO.Init_H_Lvl)) { Init_H_Lvl = "90.0"; } else { Init_H_Lvl = BO.Init_H_Lvl; }
            if (string.IsNullOrEmpty(BO.Init_L_Lvl)) { Init_L_Lvl = "10.0"; } else { Init_L_Lvl = BO.Init_L_Lvl; }
            if (string.IsNullOrEmpty(BO.Init_LL_Lvl)) { Init_LL_Lvl = "5.0"; } else { Init_LL_Lvl = BO.Init_LL_Lvl; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            if (!string.IsNullOrEmpty(BO.IOAddress))
            { FCstring = FCstring + "" + BO.DBName + "." + BO.TagName + ".HWSig:=%" + BO.IOAddress + ";" + Environment.NewLine; }
            FCstring = FCstring + "Ain(Bipolar:=" + Bipolar + "," + Environment.NewLine;
            FCstring = FCstring + "MaxVal:=" + MAX + "," + Environment.NewLine;
            FCstring = FCstring + "MinVal:=" + MIN + "," + Environment.NewLine;
            FCstring = FCstring + "AlarmInitDelayHH:=" + Init_HH_Dly + "," + Environment.NewLine;
            FCstring = FCstring + "AlarmInitDelayH:=" + Init_H_Dly + "," + Environment.NewLine;
            FCstring = FCstring + "AlarmInitDelayL:=" + Init_L_Dly + "," + Environment.NewLine;
            FCstring = FCstring + "AlarmInitDelayLL:=" + Init_LL_Dly + "," + Environment.NewLine;
            FCstring = FCstring + "AEDeadBand:=" + AL_DB + "," + Environment.NewLine;
            FCstring = FCstring + "AEConfigHH:=" + AE_HH_Cfg + "," + Environment.NewLine;
            FCstring = FCstring + "AELevelHH:=" + Init_HH_Lvl + "," + Environment.NewLine;
            FCstring = FCstring + "AEConfigH:=" + AE_H_Cfg + "," + Environment.NewLine;
            FCstring = FCstring + "AELevelH:=" + Init_H_Lvl + "," + Environment.NewLine;
            FCstring = FCstring + "AEConfigL:=" + AE_L_Cfg + "," + Environment.NewLine;
            FCstring = FCstring + "AELevelL:=" + Init_L_Lvl + "," + Environment.NewLine;
            FCstring = FCstring + "AEConfigLL:=" + AE_LL_Cfg + "," + Environment.NewLine;
            FCstring = FCstring + "AELevelLL:=" + Init_LL_Lvl + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add Analog positioning function in to the source file
        /// <summary>
        /// Add Analog positioning function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_AnalogPos_FC(BasicObject BO)
        {
            string FCstring;
            string Bipolar;
            string FBConfig;
            if (string.IsNullOrEmpty(BO.Bipolar)) { Bipolar = "false"; } else { Bipolar = BO.Bipolar; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "AnalogPosCtrl(Bipolar:=" + Bipolar + "," + Environment.NewLine;
            FCstring = FCstring + "FBConf:=" + FBConfig + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add Analog output function in to the source file
        /// <summary>
        /// Add Analog output function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_Aout_FC(BasicObject BO)
        {
            string FCstring;
            string Bipolar;
            string MAX;
            string MIN;
            if (string.IsNullOrEmpty(BO.Bipolar)) { Bipolar = "false"; } else { Bipolar = BO.Bipolar; }
            if (string.IsNullOrEmpty(BO.MAX)) { MAX = "100.0"; } else { MAX = BO.MAX; }
            if (string.IsNullOrEmpty(BO.MIN)) { MIN = "0.0"; } else { MIN = BO.MIN; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "Aout(Bipolar:=" + Bipolar + "," + Environment.NewLine;
            FCstring = FCstring + "MaxVal:=" + MAX + "," + Environment.NewLine;
            FCstring = FCstring + "MinVal:=" + MIN + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add Digital input function in to the source file
        /// <summary>
        /// Add Digital input function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_Din_FC(BasicObject BO)
        {
            string FCstring;
            string NormalState;
            string AEConfigDiff;
            string Alarm_InitDelay;
            if (string.IsNullOrEmpty(BO.NormalState)) { NormalState = "false"; } else { NormalState = BO.NormalState; }
            if (string.IsNullOrEmpty(BO.AEConfigDiff)) { AEConfigDiff = "0"; } else { AEConfigDiff = BO.AEConfigDiff; }
            if (string.IsNullOrEmpty(BO.Alarm_InitDelay)) { Alarm_InitDelay = "0"; } else { Alarm_InitDelay = BO.Alarm_InitDelay; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            if (!string.IsNullOrEmpty(BO.IOAddress))
            { FCstring = FCstring + "" + BO.DBName + "." + BO.TagName + ".HWSig:=%" + BO.IOAddress + ";" + Environment.NewLine; }
            FCstring = FCstring + "Din(NormalState:=" + NormalState + "," + Environment.NewLine;
            FCstring = FCstring + "AEConfigDiff:=" + AEConfigDiff + "," + Environment.NewLine;
            FCstring = FCstring + "AlarmInitDelay:=" + Alarm_InitDelay + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add Digital output function in to the source file
        /// <summary>
        /// Add Digital output function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_Dout_FC(BasicObject BO)
        {
            string FCstring;
            string PulseTime;
            if (string.IsNullOrEmpty(BO.PulseTime)) { PulseTime = "0"; } else { PulseTime = BO.PulseTime; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "Dout(PulseTime:=" + PulseTime + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add Group control function in to the source file
        /// <summary>
        /// Add group control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_GrpCtrl_FC(BasicObject BO)
        {
            string FCstring;
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "GrpCtrl(LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add OnOff control function in to the source file
        /// <summary>
        /// Add OnOff control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_OnOffCtrl_FC(BasicObject BO)
        {
            string FCstring;
            string IBF;
            string EnAuto;
            string FBConfig;
            if (string.IsNullOrEmpty(BO.IBF)) { IBF = "false"; } else { IBF = BO.IBF; }
            if (string.IsNullOrEmpty(BO.EnAuto)) { EnAuto = "false"; } else { EnAuto = BO.EnAuto; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "OnOffCtrl(IBF:=" + IBF + "," + Environment.NewLine;
            FCstring = FCstring + "EnAuto:=" + EnAuto + "," + Environment.NewLine;
            FCstring = FCstring + "FBConf:=" + FBConfig + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add OnOff two direction control function in to the source file
        /// <summary>
        /// Add OnOff two direction control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_OnOffCtrl_2D_FC(BasicObject BO)
        {
            string FCstring;
            string IBF;
            string EnAuto;
            string FBConfig;
            if (string.IsNullOrEmpty(BO.IBF)) { IBF = "false"; } else { IBF = BO.IBF; }
            if (string.IsNullOrEmpty(BO.EnAuto)) { EnAuto = "false"; } else { EnAuto = BO.EnAuto; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "OnOffCtrl_2D(IBF:=" + IBF + "," + Environment.NewLine;
            FCstring = FCstring + "EnAuto:=" + EnAuto + "," + Environment.NewLine;
            FCstring = FCstring + "FBConf:=" + FBConfig + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add OnOff varibale speed control function in to the source file
        /// <summary>
        /// Add OnOff variable speed control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_OnOffCtrl_VSD_FC(BasicObject BO)
        {
            string FCstring;
            string IBF;
            string EnAuto;
            if (string.IsNullOrEmpty(BO.IBF)) { IBF = "false"; } else { IBF = BO.IBF; }
            if (string.IsNullOrEmpty(BO.EnAuto)) { EnAuto = "false"; } else { EnAuto = BO.EnAuto; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "OnOffCtrl_VSD(IBF:=" + IBF + "," + Environment.NewLine;
            FCstring = FCstring + "EnAuto:=" + EnAuto + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add PID control function in to the source file
        /// <summary>
        /// Add PID function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_PID_FC(BasicObject BO)
        {
            string FCstring;
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "PID(LIO:=" + BO.DBName + "." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add Preselection control function in to the source file
        /// <summary>
        /// Add Preselection control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_Presel_FC(BasicObject BO)
        {
            string FCstring;
            string Presel_RBID;
            if (string.IsNullOrEmpty(BO.Presel_RBID)) { Presel_RBID = "1"; } else { Presel_RBID = BO.Presel_RBID; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "Presel(RBID:=" + Presel_RBID + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=" + BO.DBName + "." + BO.TagName + "," + Environment.NewLine;
            FCstring = FCstring + "GrpLink:= " + BO.DBName + "." + BO.DBName + ".GrpLink);" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region public method - Add SGC control function in to the source file
        /// <summary>
        /// Add SGC control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        public string ADD_SGC_FC(BasicObject BO)
        {
            string FCstring;
            string GrpStartStep;
            string GrpStopStep;
            string GrpStartDly;
            string GrpStopDly;
            if (string.IsNullOrEmpty(BO.StartStep)) { GrpStartStep = "1"; } else { GrpStartStep = BO.StartStep; }
            if (string.IsNullOrEmpty(BO.StopStep)) { GrpStopStep = "1001"; } else { GrpStopStep = BO.StopStep; }
            if (string.IsNullOrEmpty(BO.StartDelay)) { GrpStartDly = "0"; } else { GrpStartDly = BO.StartDelay; }
            if (string.IsNullOrEmpty(BO.StopDelay)) { GrpStopDly = "0"; } else { GrpStopDly = BO.StopDelay; }
            FCstring = "// Group connection for " + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "SGC(GrpStartStep:=" + GrpStartStep + "," + Environment.NewLine;
            FCstring = FCstring + "GrpStopStep:=" + GrpStopStep + "," + Environment.NewLine;
            FCstring = FCstring + "GrpStartDelay:=" + GrpStartDly + "," + Environment.NewLine;
            FCstring = FCstring + "GrpStopDelay:=" + GrpStopDly + "," + Environment.NewLine;
            FCstring = FCstring + "GMData:=" + BO.DBName + "." + BO.TagName + ".SGCLink," + Environment.NewLine;
            FCstring = FCstring + "GrpLink:=" + BO.DBName + "." + BO.DBName + ".GrpLink);" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #endregion


        #region Import IO Table
        private void importIOTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                EF = new ExcelFileData();
                OpenFileDialog SelectFile = new OpenFileDialog();
                SelectFile.Filter = "excel files (*.xls/*.xlsx)|*.xls*|All files (*.*)|*.*";
                SelectFile.ShowReadOnly = true;
                if (SelectFile.ShowDialog() == DialogResult.OK)
                {
                    ELS = new ExcelListSelector(EF.SelectFile(SelectFile.FileName));
                    ELS.SelectionChanged += new EventHandler(AssigneIndex);
                    ELS.SelectionAccepted += new EventHandler(UpdateDataGrid);
                    ELS.TopMost = true;
                    ELS.Show();
                }
            }
            catch
            {
                MessageBox.Show("Excel is not installed");
            }
                      
        }
        private void AssigneIndex(object sender, EventArgs e)
        {
            ExcelListIndex = ELS.SelIndex;            
        }
        private void UpdateDataGrid(object sender, EventArgs e)
        {
            DataTable DT = new DataTable();
            DT = EF.SelectList(ExcelListIndex);
            ImportTableView IT = new ImportTableView(DT,GlobalData);            
            IT.Show();            
        }
        #endregion

        private void objectTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            DBSetUp DBSetUpWin = new DBSetUp(GlobalData);
            DBSetUpWin.Text = "DB Mapping Setup";
            DBSetUpWin.Show();
        }
        

        private void allZ45PanelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Promotic.CalcAlarmStripes();
        }

        private void singlePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Promotic.CalcAlarmStripeSingle();
        }        

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "TIA file (*.db,*.scl)|*.db;*.scl|xlsx File (*.xlsx)|*.xlsx";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                if (OpenFile.FileName.Contains(".xlsx"))
                {
                    progressBar.Value = 50;
                    Z45TIAIFC.ImportTIAIOListSRC(GlobalData.DataFiltered.Objects, OpenFile.FileName);
                }
                else
                {
                    progressBar.Value = 50;
                    Z45TIAIFC.ImportTIADBSRC(GlobalData.DataFiltered.Objects, OpenFile.FileName);
                    Z45TIAIFC.ImportTIAFCSRC(GlobalData.DataFiltered.Objects, OpenFile.FileName);                    
                }                
                progressBar.Value = 100;
                dgvBDM.Refresh();
                //save
                GlobalData.CopyBDM(false);          //copy DataFiltered -> Data
                GlobalData.Data.SaveSerialized();   //save to temp bdm.xml
                MessageBox.Show("Import Done");
                progressBar.Value = 0;
            }
        }

        private void calculateDBToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            SetProgressDefault();
            DBnames = GlobalData.DBMappingRef;
            progressBar.Step = 1;
            progressBar.Maximum = DBnames.Count + 1;
            progressBar.Value = 0;     
            Z45TIAIFC.CalculateDBMapping(GlobalData);
            dgvBDM.Refresh();
            progressBar.PerformStep();
            MessageBox.Show("Data Blocks Calculation Done");
            progressBar.Value = 0;
            SetProgressDefault();
            GlobalData.CopyBDM(false);          //copy DataFiltered -> Data
            GlobalData.Data.SaveSerialized();   //save to temp bdm.xml
        }

        private void setupDataTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupDataType Setup = new SetupDataType(GlobalData.AObjectTypes);            
            Setup.Text = "DataType Setup";
            Setup.Show();
            Setup.FormClosed -= setupDataType_FormClosed;
        }

        private void setupDataType_FormClosed(object sender, FormClosedEventArgs e)
        {
            listChange();
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArchestraSetup Setup = new ArchestraSetup(GlobalData.AObjectTypes);
            Setup.Text = "Archestra Setup";
            Setup.Show();
        }

        private void exportSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportSetup Setup = new ExportSetup(GlobalData.AObjectTypes);
            Setup.Text = "Export Setup";
            Setup.Show();
        }
    }
}
