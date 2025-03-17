using System.Diagnostics;
using Microsoft.Office.Tools.Ribbon;
using System.IO;
using System;
using BDMdata;
using Microsoft.Win32;
using System.Windows.Forms;
using BDM_Form;
using TIAIFCNS;
using System.ComponentModel;
using System.Linq;
using AIFCNS;
using Microsoft.Office.Interop.Excel;
using CodeGenerators.Frms;
using System.Runtime.InteropServices;

namespace BDM
{
    public partial class Ribbon
    {
        public BindingList<HeaderCls> header;
        public int headerCount = 0;
        public GData _GlobalData;
        FileSystemWatcher watcher = null;
        string path = "";
        private BindingList<DBMapDef> DBnames;

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            //register event for filewatcher - checks if temp file bdm.xml was changed and notify about that with icon change
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetTempPath() + @"\";                           //where u want to watch files
            watcher.NotifyFilter = NotifyFilters.LastWrite;                     //what type of notify you want to apply
            watcher.Filter = "bdm.xml";                                         //what file should be checked
            watcher.Changed += new FileSystemEventHandler(watcher_OnChanged);   //register event on changed
            _GlobalData = new GData();
            header = new BindingList<HeaderCls>();

            RegistryKey RegK = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Excel\\Addins\\BDM"); // get instalation path from registry
            if (RegK != null)
            {
                path = RegK.GetValue("Manifest").ToString();
                path = path.Replace("BDM.vsto|vstolocal", "XMLSources/AObjectTypes.xml"); //|vstolocal
                path = path.Replace("file:///", "");
            }

            _GlobalData.DeserializeDataTypes(path);

            Worksheet activeSheet = Globals.ThisAddIn.Application.ActiveSheet;

            //register events for data are set and load
            Excel.DataSetDoneEvent += Excel_DataSetDoneEvent;
            Excel.DataGetDoneEvent += Excel_DataGetDoneEvent;
        }

        private void Excel_DataGetDoneEvent(object sender, EventArgs e)
        {
            //if BDM is turned on, turn on filewatcher to check bdm.xml changes
            if (GlobalData.bdmOn)
                watcher.EnableRaisingEvents = true;

            LoadDataBtn.Image = Resource.cloud_computing;       //change pic of load button so user realize data were changed
        }

        private void Excel_DataSetDoneEvent(object sender, EventArgs e)
        {
            //if BDM is turned on, turn on filewatcher to check bdm.xml changes
            if (GlobalData.bdmOn)
                watcher.EnableRaisingEvents = true;
        }

        private void watcher_OnChanged(object sender, FileSystemEventArgs e)
        {
            //change picture to syncbutton
            LoadDataBtn.Image = Resource.cloud_computing_5;
        }

        private void SetHeadBtn_Click(object sender, RibbonControlEventArgs e)
        {
            //set columns names
            Excel.SetExcelHead();
        }

        private void OpnIfcBtn_Click(object sender, RibbonControlEventArgs e)
        {
            RegistryKey RegK = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Excel\\Addins\\BDM"); // get instalation path from registry
            string path;
            if (RegK != null)
            {
                path = RegK.GetValue("Manifest").ToString();
                path = path.Replace("BDM.vsto", "BDM_Form.exe");
                Process.Start(path);
            }
        }

        private void SetDataBtn_Click(object sender, RibbonControlEventArgs e)
        {
            //turn off filewatcher event raising during saving data to bdm.xml
            watcher.EnableRaisingEvents = false;
            Excel.SetExcelData();
        }

        private void LoadDataBtn_Click(object sender, RibbonControlEventArgs e)
        {
            //turn off filewatcher event raising during saving data to bdm.xml
            watcher.EnableRaisingEvents = false;
            Excel.GetExcelData();
        }

        private void OnOffBtn_Click(object sender, RibbonControlEventArgs e)
        {
            //enable or disable buttons and event raising up to on/off button
            if ((sender as RibbonToggleButton).Checked)
            {
                GlobalData.bdmOn = true;
                OnOffBtn.Image = Resource.switch_7;

                Excel.AddDropdownMenu();

                LoadDataBtn.Enabled = true;
                OpnIfcBtn.Enabled = true;
                SetDataBtn.Enabled = true;
                SetHeadBtn.Enabled = true;

                DTBtn.Enabled = true;
                DBsetupBtn.Enabled = true;
                TIAImportBtn.Enabled = true;
                ExportMenu.Enabled = true;
                CalcDBBtn.Enabled = true;
                ExportStpBtn.Enabled = true;
                CalculateDBBtn.Enabled = true;
                GenerateSrcBtn.Enabled = true;
                AImportBtn.Enabled = true;
                AExportBtn.Enabled = true;
                ASetupBtn.Enabled = true;
                BuildXMLBtn.Enabled = true;
                InstallBtn.Enabled = true;
                AlarmSpsBtn.Enabled = true;

                watcher.EnableRaisingEvents = true;
            }
            else
            {
                GlobalData.bdmOn = false;
                OnOffBtn.Image = Resource.switch_6;

                Excel.RemoveDropdownMenu();

                LoadDataBtn.Enabled = false;
                OpnIfcBtn.Enabled = false;
                SetDataBtn.Enabled = false;
                SetHeadBtn.Enabled = false;

                DTBtn.Enabled = false;
                DBsetupBtn.Enabled = false;
                TIAImportBtn.Enabled = false;
                ExportMenu.Enabled = false;
                CalcDBBtn.Enabled = false;
                ExportStpBtn.Enabled = false;
                CalculateDBBtn.Enabled = false;
                GenerateSrcBtn.Enabled = false;
                AImportBtn.Enabled = false;
                AExportBtn.Enabled = false;
                ASetupBtn.Enabled = false;
                BuildXMLBtn.Enabled = false;
                InstallBtn.Enabled = false;
                AlarmSpsBtn.Enabled = false;

                watcher.EnableRaisingEvents = false;
            }
        }

        private void DTBtn_Click(object sender, RibbonControlEventArgs e)
        {
            _GlobalData.DeserializeDataTypes(path);
            SetupDataType Setup = new SetupDataType(_GlobalData.AObjectTypes);
            Setup.Text = "DataType Setup";
            Setup.Show();
            Setup.FormClosed += setup_FormClosed;
        }
        private void setup_FormClosed(object sender, FormClosedEventArgs e)
        {
            _GlobalData.SerializeDataTypes(path);
            Excel.AddDropdownMenu();
        }

        private int SendMessage(object handle, uint wM_NCHITTEST, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        private void DBsetupBtn_Click(object sender, RibbonControlEventArgs e)
        {
            _GlobalData.DataFiltered.Objects.Clear();
            Excel.eData(_GlobalData.DataFiltered);
            DBSetUp DBSetUpWin = new DBSetUp(_GlobalData);
            DBSetUpWin.Text = "DB Mapping Setup";
            DBSetUpWin.Show();
            DBSetUpWin.FormClosed += DBsetup_FormClosed;
        }
        private void DBsetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            CalcDBBtn_Click(sender, e as RibbonControlEventArgs);
        }

        private void ExportStpBtn_Click(object sender, RibbonControlEventArgs e)
        {
            _GlobalData.DeserializeDataTypes(path);
            ExportSetup Setup = new ExportSetup(_GlobalData.AObjectTypes);
            Setup.Text = "Export Setup";
            Setup.Show();
            Setup.FormClosed += setup_FormClosed;

        }


        private void CalcDBBtn_Click(object sender, RibbonControlEventArgs e)
        {
            if (_GlobalData.DBMappingRef.Count == 0)
            {
                MessageBox.Show("Check DB Mapping");
            }
            else
            {
                _GlobalData.DataFiltered.Objects.Clear();
                _GlobalData.DeserializeDataTypes(path);
                Excel.eData(_GlobalData.DataFiltered);
                DBnames = _GlobalData.DBMappingRef;
                Z45TIAIFC.CalculateDBMapping(_GlobalData);
                _GlobalData.CopyBDM(false);          //copy DataFiltered -> Data

                Globals.ThisAddIn.Application.ScreenUpdating = false;       //turn off screen updating - faster progress

                Worksheet activeSheet2 = Globals.ThisAddIn.Application.ActiveSheet;
                activeSheet2.Cells.Clear();      //clear list
                Excel.SetExcelHead();                 //set columns names

                int i = 1;                      //iterator for foreach loop

                //loop thru all objects loaded from xml file
                foreach (var obj in _GlobalData.DataFiltered.Objects)
                {

                    i++;    //iterate i to have row index
                    Range rng = (Range)activeSheet2.Range[activeSheet2.Cells[i, 1], activeSheet2.Cells[i, 57]];    //set range whole row
                                                                                                                   //fill range with array built from BDMDataClass.BasicObject
                    rng.Value = new string[] {  obj.TagName,
                                                obj.Descr,
                                                obj.DataType,
                                                obj.DBNr,
                                                obj.PositionInDB,
                                                obj.DBName,
                                                obj.NormalState,
                                                obj.AEConfigDiff,
                                                obj.Alarm_InitDelay,
                                                obj.AESeverity,
                                                obj.PulseTime,
                                                obj.Bipolar,
                                                obj.MIN,
                                                obj.MAX,
                                                obj.Unit,
                                                obj.Init_HH_Dly,
                                                obj.Init_H_Dly,
                                                obj.Init_L_Dly,
                                                obj.Init_LL_Dly,
                                                obj.AL_DB,
                                                obj.AE_HH_Cfg,
                                                obj.AE_H_Cfg,
                                                obj.AE_L_Cfg,
                                                obj.AE_LL_Cfg,
                                                obj.Init_HH_Lvl,
                                                obj.Init_H_Lvl,
                                                obj.Init_L_Lvl,
                                                obj.Init_LL_Lvl,
                                                obj.AEHH_severity,
                                                obj.AEH_severity,
                                                obj.AEL_severity,
                                                obj.AELL_severity,
                                                obj.SigFltSeverity,
                                                obj.FB0_suffix,
                                                obj.FB1_CW_suffix,
                                                obj.FB1_CCW_suffix,
                                                obj.CMD0_suffix,
                                                obj.CMD1_CW_suffix,
                                                obj.CMD1_CCW_suffix,
                                                obj.IBF,
                                                obj.EnAuto,
                                                obj.FBConfig,
                                                obj.StartStep,
                                                obj.StopStep,
                                                obj.StartDelay,
                                                obj.StopDelay,
                                                obj.HornName,
                                                obj.Presel_RBID,
                                                obj.ProfiNet_ProfiBus_ID,
                                                obj.DeviceNumber,
                                                obj.S7Comm,
                                                obj.IOAddress,
                                                obj.HMIArea,
                                                obj.HMIparrent,
                                                obj.ValueDecimalPlaces,
                                                obj.HMI_HWSigFault_severity,
                                                obj.SecurityDefinition,
                                        };
                    //report progress
                    int progress = (int)(100.0 / (double)(_GlobalData.DataFiltered.Objects.Count) * (i - 1));

                }

                Globals.ThisAddIn.Application.ScreenUpdating = true;        //turn on screen updating
                MessageBox.Show("Data Blocks Calculation Done");
            }
        }

        private void TIAImportBtn_Click(object sender, RibbonControlEventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();                  //import a calculate db nefunfuje
            OpenFile.Filter = "TIA file (*.db,*.scl)|*.db;*.scl|xlsx File (*.xlsx)|*.xlsx";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                _GlobalData.DataFiltered.Objects.Clear();
                if (OpenFile.FileName.Contains(".xlsx"))
                {
                    Z45TIAIFC.ImportTIAIOListSRC(_GlobalData.DataFiltered.Objects, OpenFile.FileName);
                }
                else
                {
                    Z45TIAIFC.ImportTIADBSRC(_GlobalData.DataFiltered.Objects, OpenFile.FileName);
                    Z45TIAIFC.ImportTIAFCSRC(_GlobalData.DataFiltered.Objects, OpenFile.FileName);
                }
                //save
                _GlobalData.CopyBDM(false);          //copy DataFiltered -> Data
                _GlobalData.Data.SaveSerialized();   //save to temp bdm.xml
                watcher.EnableRaisingEvents = false;
                Excel.GetExcelData();
                MessageBox.Show("Import Done");
            }
        }

        private void IOListBtn_Click(object sender, RibbonControlEventArgs e)
        {
            SaveFileDialog TIAExtSrcFile = new SaveFileDialog();
            TIAExtSrcFile.Filter = "xlsx File (*.xlsx)|*.xlsx";
            TIAExtSrcFile.ShowDialog();
            if (!string.IsNullOrEmpty(TIAExtSrcFile.FileName))
            {
                _GlobalData.DataFiltered.Objects.Clear();
                Excel.eData(_GlobalData.DataFiltered);
                Z45TIAIFC.GeneratePLCTags(_GlobalData.DataFiltered.Objects, TIAExtSrcFile.FileName.Replace(".xlsx", ""));
                MessageBox.Show(TIAExtSrcFile.FileName.Split('\\').Last() + " created");
            }
        }

        private void DBsFCsBtn_Click(object sender, RibbonControlEventArgs e)
        {
            if (_GlobalData.DBMappingRef.Count == 0)
            {
                MessageBox.Show("Check DB Mapping");
            }
            else
            {
                string DBResourceTextDBs = "";
                string DBResourceTextFCs = "";
                DBnames = _GlobalData.DBMappingRef;
                SaveFileDialog TIAExtSrcFile = new SaveFileDialog();
                TIAExtSrcFile.ShowDialog();
                _GlobalData.DataFiltered.Objects.Clear();
                Excel.eData(_GlobalData.DataFiltered);
                if (!string.IsNullOrEmpty(TIAExtSrcFile.FileName))
                {
                    foreach (var DBMapping in DBnames)
                    {
                        if (!string.IsNullOrEmpty(DBMapping.DBName))
                        {
                            DBResourceTextDBs = DBResourceTextDBs + Z45TIAIFC.GenerateExtSrcDB(_GlobalData.DataFiltered.Objects, DBMapping.DBName);
                            DBResourceTextFCs = DBResourceTextFCs + Z45TIAIFC.GenerateExtSrcFC(_GlobalData.DataFiltered.Objects, DBMapping.DBName, _GlobalData.AObjectTypes);
                        }
                    }
                    File.WriteAllText(TIAExtSrcFile.FileName + ".db", DBResourceTextDBs);
                    File.WriteAllText(TIAExtSrcFile.FileName + ".scl", DBResourceTextFCs);
                    MessageBox.Show("External source files created");
                }
            }
        }

        private void ASetupBtn_Click(object sender, RibbonControlEventArgs e)
        {
            ArchestraSetup Setup = new ArchestraSetup(_GlobalData.AObjectTypes);
            Setup.Text = "Archestra Setup";
            Setup.Show();
            Setup.FormClosed += setup_FormClosed;
        }

        private void AExportBtn_Click(object sender, RibbonControlEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV (*.csv)|*.csv";


            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                _GlobalData.DataFiltered.Objects.Clear();
                Excel.eData(_GlobalData.DataFiltered);
                Z45AIFC.ExportData(saveDialog.FileName, _GlobalData, header);
                MessageBox.Show("Export Done");
            }
        }

        private void AImportBtn_Click(object sender, RibbonControlEventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "CSV (*.csv)|*.csv";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                Z45AIFC.ImportData(File.ReadAllText(OpenFile.FileName), _GlobalData);
                //save
                _GlobalData.CopyBDM(false);          //copy DataFiltered -> Data
                _GlobalData.Data.SaveSerialized();   //save to temp bdm.xml
                watcher.EnableRaisingEvents = false;
                Excel.GetExcelData();
                MessageBox.Show("Import Done");
            }
        }

        private void SaveToFileBtn_Click(object sender, RibbonControlEventArgs e)
        {
            //Save data to file
            if (_GlobalData.Data.SaveSerializedToFile())
            {
                MBox mbox = new MBox("Done", "Saved");
            }
        }

        private void LoadFromBtn_Click(object sender, RibbonControlEventArgs e)
        {
            //Load data from file            
            _GlobalData.Data.LoadDeSerializedFromFile();
            _GlobalData.CopyBDM(true);
            Globals.ThisAddIn.Application.ScreenUpdating = false;       //turn off screen updating - faster progress

            Worksheet activeSheet = Globals.ThisAddIn.Application.ActiveSheet;
            activeSheet.Cells.Clear();      //clear list
            Excel.SetExcelHead();                 //set columns names

            int i = 1;                      //iterator for foreach loop

            //loop thru all objects loaded from xml file
            foreach (var obj in _GlobalData.DataFiltered.Objects)
            {

                i++;    //iterate i to have row index
                Range rng = (Range)activeSheet.Range[activeSheet.Cells[i, 1], activeSheet.Cells[i, 57]];    //set range whole row
                                                                                                            //fill range with array built from BDMDataClass.BasicObject
                rng.Value = new string[] {  obj.TagName,
                                                obj.Descr,
                                                obj.DataType,
                                                obj.DBNr,
                                                obj.PositionInDB,
                                                obj.DBName,
                                                obj.NormalState,
                                                obj.AEConfigDiff,
                                                obj.Alarm_InitDelay,
                                                obj.AESeverity,
                                                obj.PulseTime,
                                                obj.Bipolar,
                                                obj.MIN,
                                                obj.MAX,
                                                obj.Unit,
                                                obj.Init_HH_Dly,
                                                obj.Init_H_Dly,
                                                obj.Init_L_Dly,
                                                obj.Init_LL_Dly,
                                                obj.AL_DB,
                                                obj.AE_HH_Cfg,
                                                obj.AE_H_Cfg,
                                                obj.AE_L_Cfg,
                                                obj.AE_LL_Cfg,
                                                obj.Init_HH_Lvl,
                                                obj.Init_H_Lvl,
                                                obj.Init_L_Lvl,
                                                obj.Init_LL_Lvl,
                                                obj.AEHH_severity,
                                                obj.AEH_severity,
                                                obj.AEL_severity,
                                                obj.AELL_severity,
                                                obj.SigFltSeverity,
                                                obj.FB0_suffix,
                                                obj.FB1_CW_suffix,
                                                obj.FB1_CCW_suffix,
                                                obj.CMD0_suffix,
                                                obj.CMD1_CW_suffix,
                                                obj.CMD1_CCW_suffix,
                                                obj.IBF,
                                                obj.EnAuto,
                                                obj.FBConfig,
                                                obj.StartStep,
                                                obj.StopStep,
                                                obj.StartDelay,
                                                obj.StopDelay,
                                                obj.HornName,
                                                obj.Presel_RBID,
                                                obj.ProfiNet_ProfiBus_ID,
                                                obj.DeviceNumber,
                                                obj.S7Comm,
                                                obj.IOAddress,
                                                obj.HMIArea,
                                                obj.HMIparrent,
                                                obj.ValueDecimalPlaces,
                                                obj.HMI_HWSigFault_severity,
                                                obj.SecurityDefinition,
                                        };
                //report progress
                int progress = (int)(100.0 / (double)(_GlobalData.DataFiltered.Objects.Count) * (i - 1));

            }

            Globals.ThisAddIn.Application.ScreenUpdating = true;        //turn on screen updating
        }

        private void AddColBtn_Click(object sender, RibbonControlEventArgs e)
        {
            HeaderSetup Setup = new HeaderSetup(header, ref headerCount);
            Setup.Text = "Header Setup";
            Setup.Show();
            Setup.FormClosed += headerSetup_FormClosed;

        }
        private void headerSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            Excel.ClearHeaders(headerCount);
            Excel.SetExcelHead();
        }
    }

    public static class GlobalData
    {
        //global var to check if bdm tool is turned on
        public static bool bdmOn;
    }
}
