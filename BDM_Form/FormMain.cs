using System;
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


namespace BDM_Form
{
    public partial class FormMain : Form
    {
        private BackgroundWorker backgroundWorker1 = null;
        private BackgroundWorker bw_BuildXML = null;            //bw for build complete xml importing file
        private BackgroundWorker bw_BuildXMLpart = null;        //bw for build partial xml importing file
        private BackgroundWorker bw_DownloadPmiLibs = null;     //bw for copy pmi resources to have  custom indicators
        private BackgroundWorker bw_DownloadResources = null;   //bw for copy resource pics for faceplates

        private SaveFileDialog saveDialog = new SaveFileDialog();

        private ConnectPLCForm connectForm = new ConnectPLCForm();
        private Z45TIAIFC TIAIFC;        
        private List<string> DBnames;
        string selectedplc;

        private static double progStatus;
        private static double progStep;

        private delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        private static event StatusUpdateHandler OnUpdateStatus;

        /// <summary>
        /// Constructor
        /// </summary>
        public FormMain()
        {
            InitializeComponent();

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
            List<string> listDataTypes = new List<string> { "empty",
                                                            Resource.AinType,
                                                            Resource.DinType,
                                                            Resource.AoutType,
                                                            Resource.DoutType,
                                                            Resource.AnalogPosType,
                                                            Resource.OnOffType,
                                                            Resource.OnOff2DType,
                                                            Resource.OnOffVSDType,
                                                            Resource.GrpType,
                                                            Resource.PIDType,
                                                            Resource.PreselType};
            //add options to combobox column
            foreach (var item in listDataTypes)
            {
                comboBoxColumn.Items.Add(item);
            }

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

            bw_BuildXMLpart = new BackgroundWorker();
            bw_BuildXMLpart.WorkerReportsProgress = true;
            bw_BuildXMLpart.DoWork += bw_BuildXMLpart_DoWork;
            bw_BuildXMLpart.ProgressChanged += bw_BuildXMLpart_ProgressChanged;

            bw_DownloadPmiLibs = new BackgroundWorker();
            bw_DownloadPmiLibs.WorkerReportsProgress = true;
            bw_DownloadPmiLibs.DoWork += bw_DownloadPmiLibs_DoWork;
            bw_DownloadPmiLibs.ProgressChanged += bw_DownloadPmiLibs_ProgressChanged;

            bw_DownloadResources = new BackgroundWorker();
            bw_DownloadResources.WorkerReportsProgress = true;
            bw_DownloadResources.DoWork += bw_DownloadResources_DoWork;
            bw_DownloadResources.ProgressChanged += bw_DownloadResources_ProgressChanged;
            #endregion

            #region TIA init            
            TIAIFC = new Z45TIAIFC();            
            connectPLCToolStripMenuItem.Enabled = TIAIFC.TIAOpennessEnable;
            #endregion
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
            Debug.WriteLine(dgvBDM.FilterString);
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

                    //more then 1 cell in clipboard -> paste selection must be equal with copy seleciton
                    if (rowsInClipboard.Count() > 1)
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
                        if (dgvBDM.SelectedCells.Count > 1)
                        {
                            MessageBox.Show("Select only one cell as an insert point");
                        }
                        else
                        {
                            foreach (string CBRS in rowsInClipboard)
                            {
                                string[] CellValue = CBRS.Split(columnSplitter);    //split columns
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
            }
            else
            {
                ClearInvalids();
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
        #endregion

        #region archestra control
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveDialog.Filter = "CSV (*.csv)|*.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                Z45AIFC ArchestraExport = new Z45AIFC();
                ArchestraExport.ExportData(saveDialog.FileName,GlobalData.DataFiltered.Objects);                
            }
            MessageBox.Show("Export Done");
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

        private void completeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if data are invalid, xml isn't build
            if (GlobalData.DataFiltered.Invalid() != null)
            {
                MessageBox.Show("Invalid data. Check Main tab.");
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
        private async void iOListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            progressBar.Step = 1;
            if (!string.IsNullOrEmpty(selectedplc))
            {
                progressBar.Maximum = GlobalData.DataFiltered.Objects.Count + 1;
                var progress = new Progress<int>(percent =>
                {
                    progressBar.Value = percent;
                });
                await Task.Run(() => TIAIFC.ImportIOlist(GlobalData.DataFiltered.Objects, progress));
                Focus();
                MessageBox.Show("IO List Import Done");
                progressBar.Value = 0;
            }
            else
            {
                MessageBox.Show("PLC is not attached");
            }
            SetProgressDefault();
        }

        private void dBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            DBnames = new List<string>();
            foreach (var item in GlobalData.DataFiltered.Objects)
            {
                if (!DBnames.Contains(item.DBName))
                {
                    DBnames.Add(item.DBName);
                }
            }
            if (!string.IsNullOrEmpty(selectedplc))
            {
                progressBar.Step = 1;
                progressBar.Maximum = DBnames.Count + 1;
                progressBar.Value = 0;
                foreach (var DBName in DBnames)
                {
                    progressBar.PerformStep();
                    if (!string.IsNullOrEmpty(DBName))
                    {
                        TIAIFC.ImportDB(GlobalData.DataFiltered.Objects, DBName);
                    }
                }
                TIAIFC.CalculateDBMapping(GlobalData.DataFiltered.Objects);
                dgvBDM.Refresh();
                progressBar.PerformStep();
                MessageBox.Show("Data Blocks Import Done");
                progressBar.Value = 0;
            }
            else
            {
                MessageBox.Show("PLC is not attached");
            }
            SetProgressDefault();
        }

        private void fCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            DBnames = new List<string>();
            foreach (var item in GlobalData.DataFiltered.Objects)
            {
                if (!DBnames.Contains(item.DBName))
                {
                    DBnames.Add(item.DBName);
                }
            }
            if (!string.IsNullOrEmpty(selectedplc))
            {
                progressBar.Step = 1;
                progressBar.Maximum = DBnames.Count;
                progressBar.Value = 0;
                foreach (var DBName in DBnames)
                {
                    progressBar.PerformStep();
                    if (!string.IsNullOrEmpty(DBName))
                    {
                        TIAIFC.ImportFC(GlobalData.DataFiltered.Objects, DBName);
                    }
                }
                MessageBox.Show("Functions Import Done");
                progressBar.Value = 0;
            }
            else
            {
                MessageBox.Show("PLC is not attached");
            }
            SetProgressDefault();
        }

        private async void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            DBnames = new List<string>();
            foreach (var item in GlobalData.DataFiltered.Objects)
            {
                if (!DBnames.Contains(item.DBName))
                {
                    DBnames.Add(item.DBName);
                }
            }
            if (!string.IsNullOrEmpty(selectedplc))
            {
                progressBar.Maximum = GlobalData.DataFiltered.Objects.Count + 20 * DBnames.Count + 2;
                progressBar.Value = 0;
                var progress = new Progress<int>(percent =>
                {
                    progressBar.Value = percent;
                });
                await Task.Run(() => TIAIFC.ImportIOlist(GlobalData.DataFiltered.Objects, progress));
                progressBar.Step = 10;
                foreach (var DBName in DBnames)
                {
                    progressBar.PerformStep();
                    if (!string.IsNullOrEmpty(DBName))
                    {
                        TIAIFC.ImportDB(GlobalData.DataFiltered.Objects, DBName);
                    }
                }
                foreach (var DBName in DBnames)
                {
                    progressBar.PerformStep();
                    if (!string.IsNullOrEmpty(DBName))
                    {
                        TIAIFC.ImportFC(GlobalData.DataFiltered.Objects, DBName);
                    }
                }
                TIAIFC.CalculateDBMapping(GlobalData.DataFiltered.Objects);
                progressBar.PerformStep();
                dgvBDM.Refresh();
                Focus();
                MessageBox.Show("Import Done");
                progressBar.Value = 0;
            }
            else
            {
                MessageBox.Show("PLC is not attached");
            }
            SetProgressDefault();
        }

        private async void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            if (!string.IsNullOrEmpty(selectedplc))
            {
                progressBar.Step = 1;
                progressBar.Maximum = 100;
                progressBar.Value = 0;
                GlobalData.DataFiltered.Objects.Clear();
                var progress = new Progress<int>(percent =>
                {
                    progressBar.Value = percent;
                });
                BindingList<BDMdata.BasicObject> ExportTempData = new BindingList<BDMdata.BasicObject>();
                await Task.Run(() => ExportTempData = TIAIFC.ExportPLCData(progress));
                foreach (var item in ExportTempData)
                {
                    GlobalData.DataFiltered.Objects.Add(item);
                }
                GlobalData.CopyBDM(false);
                GlobalData.DataFiltered.SaveSerialized();
                MessageBox.Show("Export Done");
                progressBar.Value = 0;
            }
            else
            {
                MessageBox.Show("PLC is not attached");
            }
            SetProgressDefault();
        }

        private void generateSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProgressDefault();
            progressBar.Step = 1;
            string DBResourceTextDBs = "";
            string DBResourceTextFCs = "";
            DBnames = new List<string>();
            foreach (var item in GlobalData.DataFiltered.Objects)
            {
                if (!DBnames.Contains(item.DBName))
                {
                    DBnames.Add(item.DBName);
                }
            }
            SaveFileDialog TIAExtSrcFile = new SaveFileDialog();
            TIAExtSrcFile.ShowDialog();
            if (!string.IsNullOrEmpty(TIAExtSrcFile.FileName))
            {
                progressBar.Maximum = DBnames.Count;
                progressBar.Value = 0;
                foreach (var DBName in DBnames)
                {
                    progressBar.PerformStep();
                    if (!string.IsNullOrEmpty(DBName))
                    {
                        DBResourceTextDBs = DBResourceTextDBs + TIAIFC.GenerateExtSrcDB(GlobalData.DataFiltered.Objects, DBName);
                        DBResourceTextFCs = DBResourceTextFCs + TIAIFC.GenerateExtSrcFC(GlobalData.DataFiltered.Objects, DBName);
                    }
                }
                File.WriteAllText(TIAExtSrcFile.FileName + "DBs.db", DBResourceTextDBs);
                File.WriteAllText(TIAExtSrcFile.FileName + "FCs.scl", DBResourceTextFCs);
                MessageBox.Show("External source files created");
                progressBar.Value = 0;
            }
            SetProgressDefault();
        }

        private void SetProgressDefault()
        {
            progressBar.Step = 10;
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;

            progressLabel.Text = "Progress";
        }

        private void connectPLCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connectForm.FormClosing += connectForm_FormClosing;
            connectForm.ShowDialog();
        }

        private void connectForm_FormClosing(object sender, EventArgs e)
        {
            if (connectForm.Valid)
            {
                TIAIFC = connectForm.TIAIFC;
                selectedplc = connectForm.selectedplc;
                importToolStripMenuItem1.Enabled = true;
                exportToolStripMenuItem1.Enabled = true;
                generateSourceToolStripMenuItem.Enabled = true;
                calculateDBMappingToolStripMenuItem.Enabled = true;                
                labelProject.Text = "| " + connectForm.selectedproj + " |";
                labelPLC.Text = connectForm.selectedplc;
                compileAndSaveToolStripMenuItem.Enabled = true;
            }
            else
            {
                importToolStripMenuItem1.Enabled = false;
                exportToolStripMenuItem1.Enabled = false;
                generateSourceToolStripMenuItem.Enabled = false;
                calculateDBMappingToolStripMenuItem.Enabled = false;
                compileAndSaveToolStripMenuItem.Enabled = false;
                //connectPLCToolStripMenuItem.Enabled = false;
            }
        }
        private void calculateDBMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TIAIFC.CalculateDBMapping(GlobalData.Data.Objects);
            dgvBDM.Refresh();            
        }
        #endregion

        private void compileAndSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TIAIFC.CompileSave();
        }
    }

    #region GlobalData
    /// <summary>
    /// Global data - used in whole project
    /// </summary>
    public static class GlobalData
    {
        public static BDMdataClass Data = new BDMdataClass();           //Full data, as a backup for filtering and sorting
        public static BDMdataClass DataFiltered = new BDMdataClass();   //Used data for generating and binded to datagridview on MAIN tab, at start are same as Data

        /// <summary>
        /// Constructor
        /// </summary>
        static GlobalData()
        {
            //At beggining load data and copy to datafiltered
            Data.LoadDeSerialized();
            CopyBDM(true);
        }

        /// <summary>
        /// Copy Data <-> DataFiltered
        /// </summary>
        /// <param name="direction">true: Data -> DataFiltered, false: inverted</param>
        public static void CopyBDM(bool direction)
        {
            //copy data to filtereddata
            if (direction)
            {
                DataFiltered.Objects.Clear();
                foreach (var obj in Data.Objects)
                {
                    DataFiltered.Objects.Add(obj);
                }
            }
            //copy filtereddata to data
            else
            {
                Data.Objects.Clear();
                foreach (var obj in DataFiltered.Objects)
                {
                    Data.Objects.Add(obj);
                }
            }
        } 
    }
    #endregion
}
