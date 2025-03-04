using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using TiaPortalOpennessClassLibrary;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using BDMdata;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TIAIFCNS
{
    
    public class Z45TIAIFC
    {
        #region Private attribute - Class private attributes
        private TiaPortal myTiaPortal;
        private Project myProject;
        private string projectname;
        private List<ControllerTarget> prjDeviceList;
        private List<TIADBNmNrRef> dbRefList;
        private ControllerTarget SelectedPLC;
        private string SelectedPLCName;
        private List<Z45PLCDataType> DTL;
        private BindingList<TIAPrcInst> tialist;
        private IList<IBlock> blockList;
        private int ProgressBarValue;
        private string DTExceptionText;
        #endregion

        #region Public attribute - Class public attributes        
        /// <summary>
        /// Tia project PLC device list 
        /// </summary>
        public List<string> PrjDeviceList
        {
            get
            {
                List<string> DeviceListNames = new List<string>();
                if (string.IsNullOrEmpty(projectname))
                {
                    return DeviceListNames;
                }
                else
                {                    
                    foreach (var item in prjDeviceList)
                    {
                        DeviceListNames.Add(item.Name);
                    }
                    return DeviceListNames;
                }
            }
        }
        public bool TIAOpennessEnable { get; }

        /// <summary>
        /// Selected plc Data block name/number reference list
        /// </summary>
        public List<TIADBNmNrRef> DBRefList { get { return dbRefList; } }
        public string AttachedTIAPrj
        {
            get
            {
                string PN;
                try
                {
                    String[] SplitPath = myProject.Path.Split(new char[] { '\\' });
                    PN = SplitPath[SplitPath.Length - 1];
                }
                catch
                {

                    PN = "";
                }
                return PN;
            }
        }
        public BindingList<TIAPrcInst> TIAList { get { return tialist; } }
        #endregion

        #region Public class constructor - Z45TIAIFC Class constructor
        /// <summary>
        /// Z45 TIA portal interface constructor
        /// <returns></returns>
        /// </summary>
        public Z45TIAIFC()
        {
            AppDomain CurrentDomain = AppDomain.CurrentDomain;
            CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolver);
            try
            {
                tialist = new BindingList<TIAPrcInst>();
                TIAOpennessEnable = true;
            }
            catch 
            {
                TIAOpennessEnable = false;
            }
                      
        }
        #endregion

        #region Private method - Tia openes path resolver
        /// <summary>
        /// Project path resolver. It will find instalation path of TIAOpenness folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly MyResolver(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            string name = args.Name.Substring(0, index) + ".dll";
            // Check for 64bit installation
            RegistryKey filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Automation\\_InstalledSW\\TIAP13\\TIA_Opns");
            // Check for 32bit installation
            if (filePathReg == null)
                filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Siemens\\Automation\\_InstalledSW\\TIAP13\\TIA_Opns");
            string filePath = filePathReg.GetValue("Path").ToString() + "PublicAPI\\V13 SP1";
            string path = Path.Combine(filePath, name);
            // User must provide the correct path
            string fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                try
                {
                    return Assembly.LoadFrom(fullPath);
                }
                catch
                {
                    return null;
                }                
            }
            return null;
        }
        #endregion

        #region Public method - Update list of running TIA portal processes. Return list of runnin TiaProcesses List(TIAPrcInst)
        /// <summary>
        /// Update list of running TIA portal processes. Return list of runnin TiaProcesses List(TIAPrcInst)
        /// </summary>
        /// <returns></returns>
        public void Update()
        {
            tialist.Clear();
            try
            {
                foreach (TiaPortalProcess TiaProcess in TiaPortal.GetProcesses())
                {
                    if (TiaProcess.Version == "V13 SP1")
                    {
                        try
                        {
                            String[] SplitPath = TiaProcess.ProjectPath.Split(new char[] { '\\' });
                            TIAList.Add(new TIAPrcInst() { ProcessName = TiaProcess.Process.ProcessName, ProjectPath = TiaProcess.ProjectPath, ProjectName = SplitPath[SplitPath.Length - 1], Process = TiaProcess });
                        }
                        catch
                        {
                            TIAList.Add(new TIAPrcInst() { ProcessName = TiaProcess.Process.ProcessName, ProjectPath = TiaProcess.ProjectPath, ProjectName = "Empty", Process = TiaProcess });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TIAList.Add(new TIAPrcInst() { ProcessName = "", ProjectPath = "", ProjectName = e.ToString(), Process = null });
            }
        }
        #endregion

        #region Public method - Connect to the selected running TIA instance.
        /// <summary>
        /// Connect to the running TIA instance. 
        /// </summary>
        /// <param name="Prcs">Selected TIA instance as object</param>
        /// <returns>Text s pozdravem</returns>
        public void ConnectTiaPortal(object Prcs)
        {
            try
            {
                TIAPrcInst SelPrj = ((TIAPrcInst)Prcs);
                switchWindow(SelPrj.Process.Process.ProcessName);
                myTiaPortal = SelPrj.Process.Attach();
                foreach (var Project in myTiaPortal.Projects)
                {
                    if (Project.Path == SelPrj.ProjectPath)
                    {
                        myProject = Project;
                    }
                }
                //Splic path and extract Proj name
                string ProjectPath = myProject.Path;
                string[] SplitPath = ProjectPath.Split(new char[] { '\\' });
                projectname = SplitPath[SplitPath.Length - 1];
                GetProjDevices();                
                switchWindow(Process.GetCurrentProcess().ProcessName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //W_ProjList.ItemsSource = LoadTIAProcesses.Update();
                //ConnectPrj.IsEnabled = false;
            }
        }
        #endregion

        #region Public method - Open Tia project. Open file dialog and select project to be attached.
        /// <summary>
        /// Open Tia project. Open file dialog and select project to be attached.
        /// </summary>
        /// <returns>Text s pozdravem</returns>
        ///         
        public void OpenProject()
        {
            // Open File dialog and select path
            OpenFileDialog fileSearch = new OpenFileDialog();
            fileSearch.Filter = "*.ap13|*.ap13";
            fileSearch.RestoreDirectory = true;
            fileSearch.ValidateNames = false;
            fileSearch.ShowDialog();

            //Splic path and extract Proj name
            string ProjectPath = fileSearch.FileName.ToString();
            string[] SplitPath = ProjectPath.Split(new char[] { '\\' });
            if (!string.IsNullOrEmpty(ProjectPath))
            {
                //Open project                
                myTiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
                myProject = myTiaPortal.Projects.Open(ProjectPath);
                projectname = SplitPath[SplitPath.Length - 1];
                GetProjDevices();
            }

        }
        #endregion

        #region Public method - deattache TIA portal
        /// <summary>
        /// Deatache TIA portal
        /// </summary>
        public void DisconectTia()
        {
            myProject.Save();
            myTiaPortal.Dispose();
            projectname = "";
            SelectedPLCName = "";
        }
        #endregion

        #region Private method - Load All deviceces in the connected project
        /// <summary>
        /// Load All deviceces in the connected project
        /// </summary>
        private void GetProjDevices()
        {
            Collection<ItemClass> PrjItems = ((Collection<ItemClass>)OpennessHelper.GetDevicesInProject(myProject));            
            List<Device> PrjDevices = new List<Device>();
            prjDeviceList = new List<ControllerTarget>();

            foreach (var item in PrjItems)
            {
                PrjDevices.Add(item.ItemData);
            }            
            foreach (var Device in PrjDevices)
            {
                if (OpennessHelper.GetTarget(Device) is ControllerTarget)
                {                    
                    prjDeviceList.Add(OpennessHelper.GetTarget(Device));                    
                }
            }            
        }
        #endregion

        #region Public method - Select PLC controler for import export data
        /// <summary>
        /// Select PLC controler for import export data
        /// </summary>
        /// <param name="PLCName">PLCname to be used for import/export data</param>
        public void SelectControllerTarget(string PLCName)
        {
            if (!string.IsNullOrEmpty(projectname))
            {
                SelectedPLC = prjDeviceList.Find(x => (x.Name == PLCName));
                SelectedPLCName = PLCName;
            }
            else
            {
                MessageBox.Show("Project is not attached");
            }
        }

        #endregion

        #region Private method - Get data blok name/number reference list
        /// <summary>
        /// Get data blok name/number reference list
        /// </summary>
        private void GetDBRefList()
        {
            if (!string.IsNullOrEmpty(projectname))
                if (!string.IsNullOrEmpty(SelectedPLCName))
                {
                    dbRefList = new List<TIADBNmNrRef>();
                    TIADBNmNrRef DBRefItem = new TIADBNmNrRef();
                    foreach (dynamic FolderItem in SelectedPLC.ProgramblockFolder.Blocks)
                    {
                        if (FolderItem is DataBlock)                        {
                            
                            DBRefItem.DBName = FolderItem.Name;
                            DBRefItem.DBNr = FolderItem.GetAttribute("Number").ToString();
                            DBRefItem.DBInstance = FolderItem;
                            dbRefList.Add(DBRefItem);
                        }
                    }
                    foreach (dynamic FolderItem in SelectedPLC.ProgramblockFolder.Folders)
                    {
                        GetDBRef(FolderItem);
                    }
                }
                else
                {
                    MessageBox.Show("PLC is not selected");
                }

            else
            {
                MessageBox.Show("Project is not attached");
            }
        }
        #endregion

        #region Private method - Diging datablocks in subfolder structure 
        /// <summary>
        /// Diging datablocks in subfolder structure 
        /// </summary>
        /// <param name="Folder"></param>
        private void GetDBRef(dynamic Folder)
        {
            TIADBNmNrRef DBRefItem = new TIADBNmNrRef();
            foreach (dynamic FolderItem in Folder.Blocks)
            {
                if (FolderItem is DataBlock)
                {
                    DBRefItem.DBName = FolderItem.Name;
                    DBRefItem.DBNr = FolderItem.GetAttribute("Number").ToString();
                    DBRefItem.DBInstance = FolderItem;
                    dbRefList.Add(DBRefItem);
                }
            }
            foreach (var folder in Folder.Folders)
            {
                GetDBRef(folder);
            }
        }
        #endregion

        public string GenerateExtSrcDB(BindingList<BasicObject> ImportData, string DBName)
        {
            string SrcCode = "";
            SrcCode = "DATA_BLOCK \"" + DBName + "\"" + Environment.NewLine;
            SrcCode = SrcCode + "{ S7_Optimized_Access := 'FALSE' }" + Environment.NewLine;
            SrcCode = SrcCode + "VERSION : 0.1" + Environment.NewLine;
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

        #region Public method Create and Import source file, generate and compile data block
        /// <summary>
        /// Private method Create and Import source file, generate and compile data block
        /// </summary>        
        /// <param name="ImportData"></param>
        /// <param name="DBName"></param>
        /// <returns></returns>
        public void ImportDB(BindingList<BasicObject> ImportData, string DBName)
        {
            if (!string.IsNullOrEmpty(projectname))
            {
                if (!string.IsNullOrEmpty(SelectedPLCName))
                {
                    string TIAImportPath = Path.GetTempPath() + "Z45TIAImport.scl";
                    StreamWriter ImportDBFCTxt = new StreamWriter(TIAImportPath);
                    string ImportString;
                    ImportString = GenerateExtSrcDB(ImportData, DBName);
                   
                    using (ImportDBFCTxt)
                    {
                        ImportDBFCTxt.Write(ImportString);
                    }
                    ImportFCDBSrcFile();
                }
                else
                {
                    MessageBox.Show("PLC is not selected");
                }
            }
            else
            {
                MessageBox.Show("Project is not attached");
            }
            //return ImportFCDBSrcFile();
        }
        #endregion

        public string GenerateExtSrcFC(BindingList<BasicObject> ImportData, string DBName)
        {
            string SrcCode = "";
            SrcCode = "FUNCTION \"" + DBName + "_FC\" : Void" + Environment.NewLine;
            SrcCode = SrcCode + "{ S7_Optimized_Access := 'false' }" + Environment.NewLine;
            SrcCode = SrcCode + "VERSION : 0.1" + Environment.NewLine;
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

        # region Public method Create and Import source file, generate and compile function
        /// <summary>
        /// Private method Create and Import source file, generate and compile function
        /// </summary>        
        /// <param name="ImportData"></param>
        /// <param name="DBName"></param>
        /// <returns></returns>
        public void ImportFC(BindingList<BasicObject> ImportData, string DBName)
        {
            if (!string.IsNullOrEmpty(projectname))
            {
                if (!string.IsNullOrEmpty(SelectedPLCName))
                {
                    string TIAImportPath = Path.GetTempPath() + "Z45TIAImport.scl";
                    StreamWriter ImportDBFCTxt = new StreamWriter(TIAImportPath);
                    string ImportString;
                    ImportString = GenerateExtSrcFC(ImportData, DBName);                   
                    using (ImportDBFCTxt)
                    {
                        ImportDBFCTxt.Write(ImportString);
                    }
                    ImportFCDBSrcFile();
                }
                else
                {
                    MessageBox.Show("PLC is not selected");
                }
            }
            else
            {
                MessageBox.Show("Project is not attached");
            }
        }
        #endregion

        #region Private method - Import DBs,FCs from source file
        /// <summary>
        /// Import DBs and FCs data to the connected TIA portal project
        /// </summary>
        /// <returns>List of data block name and number mapping</returns>
        private void ImportFCDBSrcFile()
        {
            string LastImpStatus;
            string TIAImportPath = Path.GetTempPath() + "Z45TIAImport.scl";
            ExternalSource ES = null;
            try
            {
                ES = SelectedPLC.ExternalSourceFolder.ExternalSources.Find("Import.scl");
                SelectedPLC.ExternalSourceFolder.ExternalSources.Delete(ES);
            }
            catch 
            {  
            }
            SelectedPLC.ExternalSourceFolder.ExternalSources.CreateFromFile("Import.scl", TIAImportPath);
            try
            {
                ES = SelectedPLC.ExternalSourceFolder.ExternalSources.Find("Import.scl");
                ES.GenerateBlocksFromSourceAndCompile();
                SelectedPLC.ExternalSourceFolder.ExternalSources.Delete(ES);
                LastImpStatus = "Import Complete";
            }
            catch (System.Exception ex)
            {
                LastImpStatus = ex.ToString();
                SelectedPLC.ExternalSourceFolder.ExternalSources.Delete(ES);
                string IFTime = DateTime.Now.ToString().Replace(".", "_").Replace(":", "_");
                SelectedPLC.ExternalSourceFolder.ExternalSources.CreateFromFile("Failed_Import_" + IFTime, TIAImportPath);
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion        

        #region Public method - Import IO list
        /// <summary>
        /// Import IO list to the connected TIA portal project
        /// </summary>        
        /// /// <param name="BDMData">BDM data from excel tool</param>
        public void ImportIOlist(BindingList<BasicObject> BOList, IProgress<int> progress)
        {
            if (!string.IsNullOrEmpty(projectname))
            {
                if (!string.IsNullOrEmpty(SelectedPLCName))
                {
                    string TempFolderPath = Path.GetTempPath();
                    #region Create XML file in local temp folder
                    TIAIOList IOTable = new TIAIOList();
                    ProgressBarValue = 0;
                    foreach (var MotorComponent in BOList)
                    {                                                
                        ProgressBarValue = ProgressBarValue + 1;
                        if (progress != null)
                            progress.Report(ProgressBarValue);
                        string datatype = "";
                        if (!string.IsNullOrEmpty(MotorComponent.IOAddress))
                        {
                            if (MotorComponent.DataType.ToLower().StartsWith("a"))
                            {
                                datatype = "int";
                            }
                            else
                            {
                                datatype = "bool";
                            }
                            IOTable.AddTag(MotorComponent.TagName, MotorComponent.IOAddress, datatype, MotorComponent.Descr);
                        }
                    }
                    XmlSerializer Serializer = new XmlSerializer(typeof(TIAIOList));
                    using (TextWriter writer = new StreamWriter(TempFolderPath + "IOListImport.xml"))
                    {
                        Serializer.Serialize(writer, IOTable);
                    }                    
                    #endregion
                    try
                    {
                        SelectedPLC.ControllerTagFolder.TagTables.Import(TempFolderPath + "IOListImport.xml", ImportOptions.Override);
                        ProgressBarValue = ProgressBarValue + 1;
                        if (progress != null)
                            progress.Report(ProgressBarValue);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Wrong IO address or data type definition:" + Environment.NewLine + ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("PLC is not selected");
                }
            }
            else
            {
                MessageBox.Show("Project is not attached");
            }
        }
        #endregion

        # region Private method - Add Analog input function in to the source file
        /// <summary>
        /// Add Analog input function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_AIN_FC(BasicObject BO)
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
            { FCstring = FCstring + "\"" + BO.DBName + "\"." + BO.TagName + ".HWSig:=%" + BO.IOAddress + ";" + Environment.NewLine; }
            FCstring = FCstring + "\"Ain\"(Bipolar:=" + Bipolar + "," + Environment.NewLine;
            FCstring = FCstring + "Max:=" + MAX + "," + Environment.NewLine;
            FCstring = FCstring + "Min:=" + MIN + "," + Environment.NewLine;
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
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add Analog positioning function in to the source file
        /// <summary>
        /// Add Analog positioning function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_AnalogPos_FC(BasicObject BO)
        {
            string FCstring;
            string Bipolar;
            string FBConfig;
            if (string.IsNullOrEmpty(BO.Bipolar)) { Bipolar = "false"; } else { Bipolar = BO.Bipolar; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"AnalogPosCtrl\"(Bipolar:=" + Bipolar + "," + Environment.NewLine;
            FCstring = FCstring + "FBConf:=" + FBConfig + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add Analog output function in to the source file
        /// <summary>
        /// Add Analog output function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_Aout_FC(BasicObject BO)
        {
            string FCstring;
            string Bipolar;
            string MAX;
            string MIN;
            if (string.IsNullOrEmpty(BO.Bipolar)) { Bipolar = "false"; } else { Bipolar = BO.Bipolar; }
            if (string.IsNullOrEmpty(BO.MAX)) { MAX = "100.0"; } else { MAX = BO.MAX; }
            if (string.IsNullOrEmpty(BO.MIN)) { MIN = "0.0"; } else { MIN = BO.MIN; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"Aout\"(Bipolar:=" + Bipolar + "," + Environment.NewLine;
            FCstring = FCstring + "Max:=" + MAX + "," + Environment.NewLine;
            FCstring = FCstring + "Min:=" + MIN + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add Digital input function in to the source file
        /// <summary>
        /// Add Digital input function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_Din_FC(BasicObject BO)
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
            { FCstring = FCstring + "\"" + BO.DBName + "\"." + BO.TagName + ".HWSig:=%" + BO.IOAddress + ";" + Environment.NewLine; }
            FCstring = FCstring + "\"Din\"(NormalState:=" + NormalState + "," + Environment.NewLine;
            FCstring = FCstring + "AEConfigDiff:=" + AEConfigDiff + "," + Environment.NewLine;
            FCstring = FCstring + "AlarmInitDelay:=" + Alarm_InitDelay + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add Digital output function in to the source file
        /// <summary>
        /// Add Digital output function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_Dout_FC(BasicObject BO)
        {
            string FCstring;
            string PulseTime;
            if (string.IsNullOrEmpty(BO.PulseTime)) { PulseTime = "0"; } else { PulseTime = BO.PulseTime; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"Dout\"(PulseTime:=" + PulseTime + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add Group control function in to the source file
        /// <summary>
        /// Add group control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_GrpCtrl_FC(BasicObject BO)
        {
            string FCstring;
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"GrpCtrl\"(\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add OnOff control function in to the source file
        /// <summary>
        /// Add OnOff control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_OnOffCtrl_FC(BasicObject BO)
        {
            string FCstring;
            string IBF;
            string EnAuto;
            string FBConfig;
            if (string.IsNullOrEmpty(BO.IBF)) { IBF = "false"; } else { IBF = BO.IBF; }
            if (string.IsNullOrEmpty(BO.EnAuto)) { EnAuto = "false"; } else { EnAuto = BO.EnAuto; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"OnOffCtrl\"(IBF:=" + IBF + "," + Environment.NewLine;
            FCstring = FCstring + "EnAuto:=" + EnAuto + "," + Environment.NewLine;
            FCstring = FCstring + "FBConf:=" + FBConfig + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add OnOff two direction control function in to the source file
        /// <summary>
        /// Add OnOff two direction control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_OnOffCtrl_2D_FC(BasicObject BO)
        {
            string FCstring;
            string IBF;
            string EnAuto;
            string FBConfig;
            if (string.IsNullOrEmpty(BO.IBF)) { IBF = "false"; } else { IBF = BO.IBF; }
            if (string.IsNullOrEmpty(BO.EnAuto)) { EnAuto = "false"; } else { EnAuto = BO.EnAuto; }
            if (string.IsNullOrEmpty(BO.FBConfig)) { FBConfig = "0"; } else { FBConfig = BO.FBConfig; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"OnOffCtrl_2D\"(IBF:=" + IBF + "," + Environment.NewLine;
            FCstring = FCstring + "EnAuto:=" + EnAuto + "," + Environment.NewLine;
            FCstring = FCstring + "FBConf:=" + FBConfig + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add OnOff varibale speed control function in to the source file
        /// <summary>
        /// Add OnOff variable speed control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_OnOffCtrl_VSD_FC(BasicObject BO)
        {
            string FCstring;
            string IBF;
            string EnAuto;
            if (string.IsNullOrEmpty(BO.IBF)) { IBF = "false"; } else { IBF = BO.IBF; }
            if (string.IsNullOrEmpty(BO.EnAuto)) { EnAuto = "false"; } else { EnAuto = BO.EnAuto; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"OnOffCtrl_VSD\"(IBF:=" + IBF + "," + Environment.NewLine;
            FCstring = FCstring + "EnAuto:=" + EnAuto + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add PID control function in to the source file
        /// <summary>
        /// Add PID function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_PID_FC(BasicObject BO)
        {
            string FCstring;
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"PID\"(\"" + BO.DBName + "\"." + BO.TagName + ");" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add Preselection control function in to the source file
        /// <summary>
        /// Add Preselection control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_Presel_FC(BasicObject BO)
        {
            string FCstring;
            string Presel_RBID;
            if (string.IsNullOrEmpty(BO.Presel_RBID)) { Presel_RBID = "1"; } else { Presel_RBID = BO.Presel_RBID; }
            FCstring = "//" + BO.TagName + "-" + BO.Descr + Environment.NewLine;
            FCstring = FCstring + "\"Presel\"(RBID:=" + Presel_RBID + "," + Environment.NewLine;
            FCstring = FCstring + "LIO:=\"" + BO.DBName + "\"." + BO.TagName + "," + Environment.NewLine;
            FCstring = FCstring + "GrpLink:=\"" + BO.DBName + "\"." + BO.DBName + ".GrpLink);" + Environment.NewLine;
            return FCstring;
        }
        #endregion

        #region Private method - Add SGC control function in to the source file
        /// <summary>
        /// Add SGC control function in to the source file
        /// </summary>
        /// <param name="BO"></param>
        /// <returns></returns>
        private string ADD_SGC_FC(BasicObject BO)
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
            FCstring = FCstring + "\"SGC\"(GrpStartStep:=" + GrpStartStep + "," + Environment.NewLine;
            FCstring = FCstring + "GrpStopStep:=" + GrpStopStep + "," + Environment.NewLine;
            FCstring = FCstring + "GrpStartDelay:=" + GrpStartDly + "," + Environment.NewLine;
            FCstring = FCstring + "GrpStopDelay:=" + GrpStopDly + "," + Environment.NewLine;
            FCstring = FCstring + "GMData:=\"" + BO.DBName + "\"." + BO.TagName + ".SGCLink," + Environment.NewLine;
            FCstring = FCstring + "GrpLink:=\"" + BO.DBName + "\"." + BO.DBName + ".GrpLink);" + Environment.NewLine;
            return FCstring;
        }
        #endregion    

        #region Private method - Get project data types 
        /// <summary>
        /// Get project data types
        /// </summary>
        public void GetDataTypes()
        {
            DTExceptionText = "";
            DTL = new List<Z45PLCDataType>();            
            foreach (dynamic FolderItem in SelectedPLC.ControllerDatatypeFolder.Datatypes)
            {
                Z45PLCDataType DT = new Z45PLCDataType();
                if (FolderItem is ControllerDatatype)
                {
                    DT.Name = FolderItem.Name;
                    DT.PLCDataType = FolderItem;
                    DTL.Add(DT);
                }
            }
            foreach (dynamic FolderItem in SelectedPLC.ControllerDatatypeFolder.Folders)
            {
                GetDT(FolderItem);
            }
            string TempFolderPath = Path.GetTempPath();
            foreach (Z45PLCDataType datatype in DTL)
            {
                string FilePath = TempFolderPath + datatype.Name + ".xml";
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                datatype.PLCDataType.Export(FilePath, ExportOptions.None);
                datatype.DTElements = new List<DTElement>();
                datatype.DTElements = exportDT(FilePath);
            }
            foreach (Z45PLCDataType datatype in DTL)
            {
                datatype.Size = DTCalc(datatype.DTElements);
            }
            if (DTExceptionText.Length > 0)
            {
                MessageBox.Show("Not supported data types:" + DTExceptionText);
            }
            
        }
        #endregion

        #region Privte method - Diging datatypes in subfolder structure
        /// <summary>
        /// Diging datatypes in subfolder structure 
        /// </summary>
        /// <param name="Folder"></param>
        private void GetDT(dynamic Folder)
        {
            foreach (dynamic folderitem in Folder.Datatypes)
            {
                Z45PLCDataType DT = new Z45PLCDataType();
                if (folderitem is ControllerDatatype)
                {
                    DT.Name = folderitem.Name;
                    DT.PLCDataType = folderitem;
                    DTL.Add(DT);
                }
            }
            foreach (var folder in Folder.Folders)
            {
                GetDT(folder);
            }
        }
        #endregion

        #region Private method - Export Data type
        /// <summary>
        /// Export Data type
        /// </summary>
        /// <param name="filepath"></param>
        private List<DTElement> exportDT(string filepath)
        {
            XmlDocument XMLDOC = new XmlDocument();
            XmlNode XMLN;
            string TempFolderPath = Path.GetTempPath();
            XMLDOC.Load(filepath);
            XMLN = XMLDOC.ChildNodes[1];
            XMLN = XMLN.SelectSingleNode("SW.ControllerDatatype/AttributeList/Interface");
            XMLN = XMLN.FirstChild;
            XMLN = XMLN.FirstChild;
            List<DTElement> TempList = new List<DTElement>();
            foreach (XmlNode child in XMLN.ChildNodes)
            {
                DTElement DBE = new DTElement();
                DBE.Name = child.Attributes[0].Value;
                DBE.DataType = child.Attributes[1].Value.Replace("\"", "");
                TempList.Add(DBE);
            }
            return TempList;
        }
        #endregion

        #region Private method - Calculate User data type size
        /// <summary>
        /// Calculate User data type size 
        /// </summary>
        /// <param name="DTelements"></param>
        /// <returns></returns>
        private double DTCalc(List<DTElement> DTelements)
        {
            DbOffset OffsetCalc = new DbOffset();
            double SubTypeSize = 0;            
            foreach (DTElement Element in DTelements)
            {

                if (Element.DataType.ToLower() == "bool")
                {
                    OffsetCalc.AddBit();
                }
                else if (Element.DataType.ToLower() == "byte" || Element.DataType.ToLower() == "sint" || Element.DataType.ToLower() == "usint")
                {
                    OffsetCalc.AddByte();
                }
                else if (Element.DataType.ToLower() == "int" || Element.DataType.ToLower() == "uint" || Element.DataType.ToLower() == "word" || Element.DataType.ToLower() == "s5time")
                {
                    OffsetCalc.AddWord();
                }
                else if (Element.DataType.ToLower() == "real" || Element.DataType.ToLower() == "dword" || Element.DataType.ToLower() == "time" || Element.DataType.ToLower() == "time_of_day" || Element.DataType.ToLower() == "time" || Element.DataType.ToLower() == "udint" || Element.DataType.ToLower() == "dint")
                {
                    OffsetCalc.AddDWord();
                }
                else if (Element.DataType.ToLower() == "lint" || Element.DataType.ToLower() == "lreal" || Element.DataType.ToLower() == "ltime" || Element.DataType.ToLower() == "ltimeltime_of_day" || Element.DataType.ToLower() == "lword" || Element.DataType.ToLower() == "nref" || Element.DataType.ToLower() == "ulint")
                {
                    OffsetCalc.AddDWord();
                    OffsetCalc.AddDWord();
                }
                else
                {
                    try
                    {
                        Z45PLCDataType DT = ((Z45PLCDataType)DTL.Find(x => x.PLCDataType.Name == Element.DataType));
                        if (!double.IsNaN(DT.Size))
                        {
                            SubTypeSize = SubTypeSize + DT.Size;
                        }
                        else
                        {
                            DT.Size = DTCalc(DT.DTElements);
                            SubTypeSize = SubTypeSize + DT.Size;
                        }
                    }
                    catch (Exception)
                    {
                        DTExceptionText = DTExceptionText + Element.DataType + ";";                        
                    }

                }
            }            
            return OffsetCalc.Value + SubTypeSize;
        }
        #endregion

        #region Public method - Export PLC project to the TIA Tool
        /// <summary>
        /// Export PLC project to the TIA Tool
        /// </summary>
        /// <returns></returns>
        public BindingList<BasicObject> ExportPLCData(IProgress<int> progress)
        {
            BindingList<BasicObject> bolist = new BindingList<BasicObject>();
            ProgressBarValue = 0;            
            string TempFolderPath = Path.GetTempPath();            
            foreach (var table in SelectedPLC.ControllerTagFolder.TagTables)
            {
                if (File.Exists(TempFolderPath + "IOListExport.xml"))
                {
                    File.Delete(TempFolderPath + "IOListExport.xml");
                }                
                table.Export(TempFolderPath + "IOListExport.xml", ExportOptions.None);
                ProgressBarValue = 5;
                progress.Report(ProgressBarValue);
                TIAIOList IOList = null;
                try
                {
                    XmlSerializer Serializer = new XmlSerializer(typeof(TIAIOList));
                    using (TextReader reader = new StreamReader(TempFolderPath + "IOListExport.xml"))
                    {
                        IOList = Serializer.Deserialize(reader) as TIAIOList;
                    }
                }
                catch
                {
                }
                ProgressBarValue = 10;
                progress.Report(ProgressBarValue);
                try
                {
                    foreach (var item in IOList.ControllerTagTable.ObjectList.SWControllerTag)
                    {
                        if (item.AttributeList.IOAddres.ToLower().StartsWith("%i") || item.AttributeList.IOAddres.ToLower().StartsWith("%q"))
                        {
                            BasicObject BO = new BasicObject();
                            BO.TagName = item.AttributeList.Name;
                            BO.IOAddress = item.AttributeList.IOAddres.Replace("%", "");
                            BO.DataType = item.LinkList.DataType.DataType;
                            try
                            {
                                BO.Descr = item.ObjectList.MultilingualText.AttributeList.TextItems.Value.Description;
                            }
                            catch (Exception)
                            {
                                BO.Descr = item.AttributeList.Name + " - Description";
                            }
                            bolist.Add(BO);
                        }                       
                    }
                }
                catch 
                {
                }
            }
            ProgressBarValue = 20;
            progress.Report(ProgressBarValue);
            ExportDBs(bolist);
            ProgressBarValue = 50;
            progress.Report(ProgressBarValue);
            CalculateDBMapping(bolist);
            ProgressBarValue = 75;
            progress.Report(ProgressBarValue);
            ProgressBarValue = 85;
            progress.Report(ProgressBarValue);
            GetFCRefList();
            ProgressBarValue = 95;
            progress.Report(ProgressBarValue);
            UpdateBOList(bolist);
            ProgressBarValue = 100;
            progress.Report(ProgressBarValue);
            return bolist;
        }
        #endregion

        private void ExportDBs(BindingList<BasicObject> bolist)
        {
            GetDBRefList();
            string TIATempPath = Path.GetTempPath();
            XmlDocument XMLDOC = new XmlDocument();
            XmlNode XMLN;
            List<BasicObject> TempList = new List<BasicObject>();
            foreach (var DBRef in dbRefList)
            {   
                string TIAExportPath = TIATempPath + DBRef.DBName + ".xml";
                if (File.Exists(TIAExportPath))
                {
                    File.Delete(TIAExportPath);
                }
                try
                {
                    DBRef.DBInstance.Export(TIAExportPath, ExportOptions.None);
                    XMLDOC.Load(TIAExportPath);
                    XMLN = XMLDOC.ChildNodes[1];
                    XMLN = XMLN.SelectSingleNode("SW.DataBlock/AttributeList/Interface");
                    XMLN = XMLN.FirstChild;
                    XMLN = XMLN.FirstChild;
                    foreach (XmlNode child in XMLN.ChildNodes)
                    {
                        BasicObject BO = new BasicObject();
                        BO.TagName = child.Attributes[0].Value;
                        BO.DataType = child.Attributes[1].Value.Replace("\"", "");
                        BO.DBName = DBRef.DBName;
                        BO.DBNr = DBRef.DBNr;
                        try
                        {
                            BO.Descr = child.FirstChild.FirstChild.InnerText;
                        }
                        catch
                        {
                            BO.Descr = child.Attributes[0].Value + "-Description";
                        }
                        try
                        {
                            BasicObject bo = bolist.ToList().Find(x => x.TagName == BO.TagName);
                            bo.DBName = BO.DBName;
                            bo.PositionInDB = BO.PositionInDB;
                            bo.DBNr = BO.DBNr;
                            bo.Descr = BO.Descr;
                            bo.DataType = BO.DataType;
                        }
                        catch
                        {
                            bolist.Add(BO);
                        }

                    }
                }
                catch 
                {                       
                }      
            }
        }

        #region Private method - Get function  
        /// <summary>
        /// Get data blok name/number reference list
        /// </summary>
        public void GetFCRefList()
        {
            string TIATempPath = Path.GetTempPath();
            if (!string.IsNullOrEmpty(projectname))
                if (!string.IsNullOrEmpty(SelectedPLCName))
                {
                    ProgramblockSystemFolder ProgramFolder = ((ProgramblockSystemFolder)SelectedPLC.ProgramblockFolder);
                    blockList = new List<IBlock>();
                    foreach (dynamic FolderItem in SelectedPLC.ProgramblockFolder.Blocks)
                    {
                        if (FolderItem is CodeBlock)
                        {
                            IBlock block = FolderItem as IBlock;
                            if (block.Type.ToString() == "FC" && !block.IsKnowHowProtected && block.ProgrammingLanguage == ProgrammingLanguage.SCL)
                            {
                                blockList.Add(block);
                            }
                        }
                    }
                    foreach (dynamic FolderItem in SelectedPLC.ProgramblockFolder.Folders)
                    {
                        GetFCRef(FolderItem);
                    }
                    if (File.Exists(TIATempPath + "FCExport.scl"))
                    {
                        File.Delete(TIATempPath + "FCExport.scl");
                    }                       
                    ProgramFolder.GenerateSourceFromBlocks(blockList, TIATempPath + "FCExport.scl");
                }
                else
                {
                    MessageBox.Show("PLC is not selected");
                }

            else
            {
                MessageBox.Show("Project is not attached");
            }
        }
        #endregion

        #region Private method - Diging datablocks in subfolder structure 
        /// <summary>
        /// Diging datablocks in subfolder structure 
        /// </summary>
        /// <param name="Folder"></param>
        private void GetFCRef(dynamic Folder)
        {
            string TIATempPath = Path.GetTempPath();
            foreach (dynamic FolderItem in Folder.Blocks)
            {
                if (FolderItem is CodeBlock)
                {
                    IBlock block = FolderItem as IBlock;
                    if (block.Type.ToString() == "FC" && !block.IsKnowHowProtected && block.ProgrammingLanguage == ProgrammingLanguage.SCL)
                    {
                        blockList.Add(block);
                    }
                }
            }
            foreach (var folder in Folder.Folders)
            {
                GetFCRef(folder);
            }
        }
        #endregion

        private void UpdateBOList(BindingList<BasicObject> bolist)
        {
            string text = File.ReadAllText(Path.GetTempPath() + "FCExport.scl");
            text = text.Replace(" ", "").Replace("\t", "");
            int index = 0;
            int oldindex = -1;
            int EndRead = 0;
            List<FCParametersStruc> FCparList = new List<FCParametersStruc>();                       
            while (index > oldindex && text.Contains("\"("))
            {
                FCParametersStruc FCPar = new FCParametersStruc();
                oldindex = index;
                index = text.IndexOf("\"(", index) + 2;
                if (index > oldindex)
                {
                    EndRead = text.IndexOf(")", index - 2);
                    String[] FCParamString = text.Substring(index, EndRead - index).Replace(Environment.NewLine, "").Split(',');
                    #region fill FC parameter list
                    foreach (string FCParameter in FCParamString)
                    {                        
                        if (FCParameter.Contains("Bipolar:="))
                        {
                            FCPar.Bipolar = FCParameter.Replace("Bipolar:=", "");
                        }
                        else if (FCParameter.Contains("Max:="))
                        {
                            FCPar.MAX = FCParameter.Replace("Max:=", "");
                        }
                        else if (FCParameter.Contains("Min:="))
                        {
                            FCPar.MIN = FCParameter.Replace("Min:=", "");
                        }
                        else if (FCParameter.Contains("AlarmInitDelayHH:="))
                        {
                            FCPar.Init_HH_Dly = FCParameter.Replace("AlarmInitDelayHH:=", "");
                        }
                        else if (FCParameter.Contains("AlarmInitDelayH:="))
                        {
                            FCPar.Init_H_Dly = FCParameter.Replace("AlarmInitDelayH:=", "");
                        }
                        else if (FCParameter.Contains("AlarmInitDelayL:="))
                        {
                            FCPar.Init_L_Dly = FCParameter.Replace("AlarmInitDelayL:=", "");
                        }
                        else if (FCParameter.Contains("AlarmInitDelayLL:="))
                        {
                            FCPar.Init_LL_Dly = FCParameter.Replace("AlarmInitDelayLL:=", "");
                        }
                        else if (FCParameter.Contains("AEDeadBand:="))
                        {
                            FCPar.AL_DB = FCParameter.Replace("AEDeadBand:=", "");
                        }
                        else if (FCParameter.Contains("AEConfigHH:="))
                        {
                            FCPar.AE_HH_Cfg = FCParameter.Replace("AEConfigHH:=", "");
                        }
                        else if (FCParameter.Contains("AEConfigH:="))
                        {
                            FCPar.AE_H_Cfg = FCParameter.Replace("AEConfigH:=", "");
                        }
                        else if (FCParameter.Contains("AEConfigL:="))
                        {
                            FCPar.AE_L_Cfg = FCParameter.Replace("AEConfigL:=", "");
                        }
                        else if (FCParameter.Contains("AEConfigLL:="))
                        {
                            FCPar.AE_LL_Cfg = FCParameter.Replace("AEConfigLL:=", "");
                        }
                        else if (FCParameter.Contains("AELevelHH:="))
                        {
                            FCPar.Init_HH_Lvl = FCParameter.Replace("AELevelHH:=", "");
                        }
                        else if (FCParameter.Contains("AELevelH:="))
                        {
                            FCPar.Init_H_Lvl = FCParameter.Replace("AELevelH:=", "");
                        }
                        else if (FCParameter.Contains("AELevelL:="))
                        {
                            FCPar.Init_H_Lvl = FCParameter.Replace("AELevelHL:=", "");
                        }
                        else if (FCParameter.Contains("AELevelLL:="))
                        {
                            FCPar.Init_LL_Lvl = FCParameter.Replace("AELevelLL:=", "");
                        }
                        else if (FCParameter.Contains("NormalState:="))
                        {
                            FCPar.NormalState = FCParameter.Replace("NormalState:=", "");
                        }
                        else if (FCParameter.Contains("AEConfigDiff:="))
                        {
                            FCPar.AEConfigDiff = FCParameter.Replace("AEConfigDiff:=", "");
                        }
                        else if (FCParameter.Contains("AlarmInitDelay:="))
                        {
                            FCPar.Alarm_InitDelay = FCParameter.Replace("AlarmInitDelay:=", "");
                        }
                        else if (FCParameter.Contains("LIO:="))
                        {
                            FCPar.DbTagName = FCParameter.Replace("LIO:=", "").Replace("\"", "");
                        }
                        else if (FCParameter.Contains("FBConf:="))
                        {
                            FCPar.FBConfig = FCParameter.Replace("FBConf:=", "");
                        }
                        else if (FCParameter.Contains("PulseTime:="))
                        {
                            FCPar.PulseTime = FCParameter.Replace("PulseTime:=", "");
                        }
                        else if (FCParameter.Contains("IBF:="))
                        {
                            FCPar.IBF = FCParameter.Replace("IBF:=", "");
                        }
                        else if (FCParameter.Contains("EnAuto:="))
                        {
                            FCPar.EnAuto = FCParameter.Replace("EnAuto:=", "");
                        }
                        else if (FCParameter.Contains("RBID:="))
                        {
                            FCPar.Presel_RBID = FCParameter.Replace("RBID:=", "");
                        }
                        else if (FCParameter.Contains("GMData:="))
                        {
                            FCPar.DbTagName = FCParameter.Replace("GMData:=", "").Replace("\"","");
                        }
                        else if (FCParameter.Contains("GrpStartStep:="))
                        {
                            FCPar.StartStep = FCParameter.Replace("GrpStartStep:=", "");
                        }
                        else if (FCParameter.Contains("GrpStopStep:="))
                        {
                            FCPar.StopStep = FCParameter.Replace("GrpStopStep:=", "");
                        }
                        else if (FCParameter.Contains("GrpStartDelay:="))
                        {
                            FCPar.StartDelay = FCParameter.Replace("GrpStartDelay:=", "");
                        }
                        else if (FCParameter.Contains("GrpStopDelay:="))
                        {
                            FCPar.StopDelay = FCParameter.Replace("GrpStopDelay:=", "");
                        }
                    }
                    #endregion
                    FCparList.Add(FCPar);
                }
            }            
            foreach (BasicObject BO in bolist)
            {
                FCParametersStruc FP = new FCParametersStruc();
                FP = FCparList.Find(x => x.DbTagName == (BO.DBName + "." + BO.TagName));
                BO.AEConfigDiff = FP.AEConfigDiff;
                BO.AESeverity = FP.AESeverity;
                BO.AE_HH_Cfg = FP.AE_HH_Cfg;
                BO.AE_H_Cfg = FP.AE_H_Cfg;
                BO.AE_L_Cfg = FP.AE_L_Cfg;
                BO.AE_LL_Cfg = FP.AE_LL_Cfg;
                BO.Alarm_InitDelay = FP.Alarm_InitDelay;
                BO.AL_DB = FP.AL_DB;
                BO.Bipolar = FP.Bipolar;
                BO.EnAuto = FP.EnAuto;
                BO.FBConfig = FP.FBConfig;
                BO.IBF = FP.IBF;
                BO.Init_HH_Dly = FP.Init_HH_Dly;
                BO.Init_H_Dly = FP.Init_H_Dly;
                BO.Init_L_Dly = FP.Init_L_Dly;
                BO.Init_LL_Dly = FP.Init_LL_Dly;
                BO.Init_HH_Lvl = FP.Init_HH_Lvl;
                BO.Init_H_Lvl = FP.Init_H_Lvl;
                BO.Init_L_Lvl = FP.Init_L_Lvl;
                BO.Init_LL_Lvl = FP.Init_LL_Lvl;
                BO.MAX = FP.MAX;
                BO.MIN = FP.MIN;
                BO.NormalState = FP.NormalState;
                BO.Presel_RBID = FP.Presel_RBID;
                BO.PulseTime = FP.PulseTime;
                FCParametersStruc FP1 = new FCParametersStruc();
                FP1 = FCparList.Find(x => x.DbTagName == (BO.DBName + "." + BO.TagName + ".SGCLink"));                
                BO.StartDelay = FP1.StartDelay;
                BO.StopDelay = FP1.StopDelay;
                BO.StartStep = FP1.StartStep;
                BO.StopStep = FP1.StopStep;
            }
        }    

        #region Public method - Calculate Data blocks mapping
        /// <summary>
        /// Calculate Data blocks mapping
        /// </summary>
        /// <param name="ImportData"></param>
        public void CalculateDBMapping(BindingList<BasicObject> ImportData)
        {
            GetDBRefList();
            double DBPos = 0;
            foreach (TIADBNmNrRef DB in dbRefList)
            {
                DBPos = 0;
                foreach (BasicObject BO in ImportData)
                {
                    if (BO.DBName == DB.DBName)
                    {
                        BO.DBNr = DB.DBNr;
                        BO.PositionInDB = DBPos.ToString();
                        try
                        {
                            DBPos = DBPos + DTL.Find(x => x.Name == BO.DataType).Size;
                        }
                        catch
                        {
                            //MessageBox.Show("Unrecognized data type: " + BO.DataType);                    
                        }
                    }
                }
            }
        }
        #endregion

        #region Private class - PLC data type - Name, size and element list (DataTypeElement)
        /// <summary>
        /// Data type size calculation type
        /// </summary>
        private class Z45PLCDataType
        {
            public string Name;
            public double Size;
            public List<DTElement> DTElements;        
            public ControllerDatatype PLCDataType;
            public override string ToString()
            {
                return (this.Name + "/"+ this.Size);
            }
        }
        #endregion

        #region Private class - Data block element structure
        /// <summary>
        /// Private class Data block element structure
        /// </summary>
        private class DTElement
        {
            public string Name;
            public string DataType;
        }
        #endregion     

        #region Private struc function - default parameters
        /// <summary>
        /// Private struc function - default parameters
        /// </summary>
        private struct FCParametersStruc
        {
            public string DbTagName { get; set; }
            public string NormalState { get; set; }
            public string AEConfigDiff { get; set; }
            public string Alarm_InitDelay { get; set; }
            public string AESeverity { get; set; }
            public string PulseTime { get; set; }
            public string Bipolar { get; set; }
            public string MIN { get; set; }
            public string MAX { get; set; }            
            public string Init_HH_Dly { get; set; }
            public string Init_H_Dly { get; set; }
            public string Init_L_Dly { get; set; }
            public string Init_LL_Dly { get; set; }
            public string AL_DB { get; set; }
            public string AE_HH_Cfg { get; set; }
            public string AE_H_Cfg { get; set; }
            public string AE_L_Cfg { get; set; }
            public string AE_LL_Cfg { get; set; }
            public string Init_HH_Lvl { get; set; }
            public string Init_H_Lvl { get; set; }
            public string Init_L_Lvl { get; set; }
            public string Init_LL_Lvl { get; set; }                                    
            public string IBF { get; set; }
            public string EnAuto { get; set; }
            public string FBConfig { get; set; }                        
            public string Presel_RBID { get; set; }
            public string StartStep { get; set; }
            public string StopStep { get; set; }
            public string StartDelay { get; set; }
            public string StopDelay { get; set; }
        }
        #endregion

        public void CompileSave()
        {
            
            SelectedPLC.Compile(Siemens.Engineering.Compiler.CompilerOptions.Software, Siemens.Engineering.Compiler.BuildOptions.Rebuild);
            myProject.Save();
            MessageBox.Show("Project compile and save done");
        }

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool turnon);
        private void switchWindow(string ProcessName)
        {
            Process[] proc = Process.GetProcessesByName(ProcessName);
            foreach (Process item in proc)
            {
                Console.WriteLine(item.ProcessName);
                SwitchToThisWindow(item.MainWindowHandle, false);                                
            } 
        }
    }

    #region Public structure - Data block name and number reference table 
    /// <summary>
    /// Data block name and number reference table 
    /// </summary>
    public struct TIADBNmNrRef
    {
        /// <summary>
        /// Data block name
        /// </summary>
        public string DBName;
        /// <summary>
        /// Data block number
        /// </summary>
        public string DBNr;

        public DataBlock DBInstance;
    }
    #endregion

    #region Public structure - TIA process data structure
    /// <summary>
    /// Tia process instance data structure
    /// </summary>
    public struct TIAPrcInst
    {
        /// <summary>
        /// TIA process instance name
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// Location path of the project
        /// </summary>
        public string ProjectPath { get; set; }
        /// <summary>
        /// Name of the project
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// Tia portal process (class).
        /// </summary>
        public TiaPortalProcess Process { get; set; }
        public override string ToString()
        {
            return this.ProjectName;
        }              
    }
    #endregion 
}

