using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using BDMdata;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

namespace BDM
{
    public static class Excel
    {
        static BackgroundWorker bwSetData;      //bw for saving data to temp bdm.xml
        static BackgroundWorker bwGetData;      //bw for loading data from temp bdm.xml
        static FormProgress formProgSetData;    //custom form to show progress
        static FormProgress formProgGetData;
        static Stopwatch stopWatch;

        /// <summary>
        /// Constructor
        /// </summary>
        static Excel()
        {
            //set backgroundworkers and register events
            bwSetData = new BackgroundWorker();
            bwSetData.WorkerSupportsCancellation = true;                    //enable this to have option to cancel progress anytime
            bwSetData.WorkerReportsProgress = true;                         //enable this to report progress from bw
            bwSetData.DoWork += bwSetData_DoWork;
            bwSetData.ProgressChanged += bwSetData_ProgressChanged;
            bwSetData.RunWorkerCompleted += bwSetData_RunWorkerCompleted;

            bwGetData = new BackgroundWorker();
            bwGetData.WorkerSupportsCancellation = true;
            bwGetData.WorkerReportsProgress = true;
            bwGetData.DoWork += bwGetData_DoWork;
            bwGetData.ProgressChanged += bwGetData_ProgressChanged;
            bwGetData.RunWorkerCompleted += bwGetData_RunWorkerCompleted;

            //stopwatch
            stopWatch = new Stopwatch();
        }

        #region bwSetData events

        static void bwSetData_DoWork(object sender, DoWorkEventArgs e)
        {
            SetExcelData_private_v2();
        }

        static void bwSetData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update progress bar, elapsed time and message in progress form
            formProgSetData.ProgressValue = e.ProgressPercentage;
            formProgSetData.Message = e.ProgressPercentage.ToString() + " %";
            formProgSetData.time = stopWatch.Elapsed;
        }

        static void bwSetData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnDataSetDone(EventArgs.Empty);     //call event data are set
            formProgSetData.Close();            //close progress form

            //init stopwatch
            stopWatch.Stop();
            stopWatch.Reset();
        }

        #endregion

        #region bwGetData events

        static void bwGetData_DoWork(object sender, DoWorkEventArgs e)
        {
            GetExcelData_private();
        }

        static void bwGetData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update progress bar, elapsed time and message in progress form
            formProgGetData.ProgressValue = e.ProgressPercentage;
            formProgGetData.Message = e.ProgressPercentage.ToString() + " %";
            formProgGetData.time = stopWatch.Elapsed;
        }

        static void bwGetData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnDataGetDone(EventArgs.Empty);     //call event data are loaded
            formProgGetData.Close();            //close progress form

            //init stopwatch
            stopWatch.Stop();
            stopWatch.Reset();
        }

        #endregion

        #region custom events - data are saved or load from bdm.xml
        //
        static void OnDataSetDone(EventArgs e)
        {
            //if someone is listening for this event, call it
            DataSetDoneEvent?.Invoke(null, e);
        }

        public static event EventHandler DataSetDoneEvent;

        //
        static void OnDataGetDone(EventArgs e)
        {
            //if someone is listening for this event, call it
            DataGetDoneEvent?.Invoke(null, e);
        }

        public static event EventHandler DataGetDoneEvent;

        #endregion

        /// <summary>
        /// Set heading of BDM table. Columns names according to Resources
        /// </summary>
        /// <returns>True if OK</returns>
        public static bool SetExcelHead()
        {
            Worksheet activeSheet = Globals.ThisAddIn.Application.ActiveSheet;

            try
            {
                //start from first cell and continue
                Range startRange = activeSheet.Cells[1, 1];

                startRange.Value = Resource.Col1;
                startRange.Offset[0, 1].Value = Resource.Col2;
                startRange.Offset[0, 2].Value = Resource.Col3;
                startRange.Offset[0, 3].Value = Resource.Col4;
                startRange.Offset[0, 4].Value = Resource.Col5;
                startRange.Offset[0, 5].Value = Resource.Col6;
                startRange.Offset[0, 6].Value = Resource.Col7;
                startRange.Offset[0, 7].Value = Resource.Col8;
                startRange.Offset[0, 8].Value = Resource.Col9;
                startRange.Offset[0, 9].Value = Resource.Col19;
                startRange.Offset[0, 10].Value = Resource.Col11;
                startRange.Offset[0, 11].Value = Resource.Col12;
                startRange.Offset[0, 12].Value = Resource.Col13;
                startRange.Offset[0, 13].Value = Resource.Col14;
                startRange.Offset[0, 14].Value = Resource.Col15;
                startRange.Offset[0, 15].Value = Resource.Col16;
                startRange.Offset[0, 16].Value = Resource.Col17;
                startRange.Offset[0, 17].Value = Resource.Col18;
                startRange.Offset[0, 18].Value = Resource.Col19;
                startRange.Offset[0, 19].Value = Resource.Col20;
                startRange.Offset[0, 20].Value = Resource.Col21;
                startRange.Offset[0, 21].Value = Resource.Col22;
                startRange.Offset[0, 22].Value = Resource.Col23;
                startRange.Offset[0, 23].Value = Resource.Col24;
                startRange.Offset[0, 24].Value = Resource.Col25;
                startRange.Offset[0, 25].Value = Resource.Col26;
                startRange.Offset[0, 26].Value = Resource.Col27;
                startRange.Offset[0, 27].Value = Resource.Col28;
                startRange.Offset[0, 28].Value = Resource.Col29;
                startRange.Offset[0, 29].Value = Resource.Col30;
                startRange.Offset[0, 30].Value = Resource.Col31;
                startRange.Offset[0, 31].Value = Resource.Col32;
                startRange.Offset[0, 32].Value = Resource.Col33;
                startRange.Offset[0, 33].Value = Resource.Col34;
                startRange.Offset[0, 34].Value = Resource.Col35;
                startRange.Offset[0, 35].Value = Resource.Col36;
                startRange.Offset[0, 36].Value = Resource.Col37;
                startRange.Offset[0, 37].Value = Resource.Col38;
                startRange.Offset[0, 38].Value = Resource.Col39;
                startRange.Offset[0, 39].Value = Resource.Col40;
                startRange.Offset[0, 40].Value = Resource.Col41;
                startRange.Offset[0, 41].Value = Resource.Col42;
                startRange.Offset[0, 42].Value = Resource.Col43;
                startRange.Offset[0, 43].Value = Resource.Col44;
                startRange.Offset[0, 44].Value = Resource.Col45;
                startRange.Offset[0, 45].Value = Resource.Col46;
                startRange.Offset[0, 46].Value = Resource.Col47;
                startRange.Offset[0, 47].Value = Resource.Col48;
                startRange.Offset[0, 48].Value = Resource.Col49;
                startRange.Offset[0, 49].Value = Resource.Col50;
                startRange.Offset[0, 50].Value = Resource.Col51;
                startRange.Offset[0, 51].Value = Resource.Col52;
                startRange.Offset[0, 52].Value = Resource.Col53;
                startRange.Offset[0, 53].Value = Resource.Col54;
            }
            catch (Exception ex)
            {
                //show exception if fail
                MessageBox.Show(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set excel data method - save to temp file bdm.xml
        /// </summary>
        public static void SetExcelData()
        {
            if (!bwSetData.IsBusy)
            {
                //check if excel is ready, if not infor
                if (!ExcelRdy())
                {
                    MessageBox.Show("Excel application not ready. Don't edit cell during use of BDM.");
                    return;
                }

                //check if tagname column exists, else return from method
                if (!TagNameExist())
                    return;

                formProgSetData = new FormProgress();
                formProgSetData.OnCancelEvent += formProgSetData_OnCancelEvent;     //register event while cancel is pressed on progress form
                formProgSetData.Message = "Replacing null values";
                formProgSetData.Show();

                stopWatch.Start();          //start stopwatch to have elapsed time dipslayed
                bwSetData.RunWorkerAsync(); //start bw to save data to temp bdm.xml
            }
        }

        /// <summary>
        /// Method to stop saving to temp bdm.xml, invoked from progress form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void formProgSetData_OnCancelEvent(object sender, EventArgs e)
        {
            bwSetData.CancelAsync();
        }

        /// <summary>
        /// Private method to save data to temp bdm.xml
        /// </summary>
        /// <returns></returns>
        private static bool SetExcelData_private_v2()
        {
            Globals.ThisAddIn.Application.ScreenUpdating = false;               //turn off screen updating of excel - faster progress
            Globals.ThisAddIn.Application.DisplayAlerts = false;                //turn off alerts to skip some not important messages
            BDMdataClass outData = new BDMdataClass();

            Worksheet activeSheet = Globals.ThisAddIn.Application.ActiveSheet;
            Range rngLastDataType = activeSheet.get_Range("A1").EntireColumn.SpecialCells(XlCellType.xlCellTypeLastCell); //gets last used cell in any column

            //replace nulls with white space to avoid errors during next operations
            Range allRng = (Range)activeSheet.Range[activeSheet.Cells[1, 1], activeSheet.Cells[rngLastDataType.Row, 54]];
            allRng.Replace(null, " ");


            Globals.ThisAddIn.Application.DisplayAlerts = true;                 //turn on alerts to show important messages

            int dataCount = rngLastDataType.Row;

            //loop thru all rows 
            for (int i = 2; i <= dataCount; i++)
            {
                //if cancel wasn't pressed on progress form continue
                if (!bwSetData.CancellationPending)
                {
                    Range range = activeSheet.Range[activeSheet.Cells[i, 1], activeSheet.Cells[i, 54]];     //set range whole row of data
                    Array cellArray = (Array)range.Cells.Value2;                                            //cast range to array of strings

                    //add as new object to bdmdata
                    outData.Objects.Add(
                        SetBasicObject(
                                cellArray.GetValue(1, 1).ToString(),
                                cellArray.GetValue(1, 2).ToString(),
                                cellArray.GetValue(1, 3).ToString(),
                                cellArray.GetValue(1, 4).ToString(),
                                cellArray.GetValue(1, 5).ToString(),
                                cellArray.GetValue(1, 6).ToString(),
                                cellArray.GetValue(1, 7).ToString(),
                                cellArray.GetValue(1, 8).ToString(),
                                cellArray.GetValue(1, 9).ToString(),
                                cellArray.GetValue(1, 10).ToString(),
                                cellArray.GetValue(1, 11).ToString(),
                                cellArray.GetValue(1, 12).ToString(),
                                cellArray.GetValue(1, 13).ToString(),
                                cellArray.GetValue(1, 14).ToString(),
                                cellArray.GetValue(1, 15).ToString(),
                                cellArray.GetValue(1, 16).ToString(),
                                cellArray.GetValue(1, 17).ToString(),
                                cellArray.GetValue(1, 18).ToString(),
                                cellArray.GetValue(1, 19).ToString(),
                                cellArray.GetValue(1, 20).ToString(),
                                cellArray.GetValue(1, 21).ToString(),
                                cellArray.GetValue(1, 22).ToString(),
                                cellArray.GetValue(1, 23).ToString(),
                                cellArray.GetValue(1, 24).ToString(),
                                cellArray.GetValue(1, 25).ToString(),
                                cellArray.GetValue(1, 26).ToString(),
                                cellArray.GetValue(1, 27).ToString(),
                                cellArray.GetValue(1, 28).ToString(),
                                cellArray.GetValue(1, 29).ToString(),
                                cellArray.GetValue(1, 30).ToString(),
                                cellArray.GetValue(1, 31).ToString(),
                                cellArray.GetValue(1, 32).ToString(),
                                cellArray.GetValue(1, 33).ToString(),
                                cellArray.GetValue(1, 34).ToString(),
                                cellArray.GetValue(1, 35).ToString(),
                                cellArray.GetValue(1, 36).ToString(),
                                cellArray.GetValue(1, 37).ToString(),
                                cellArray.GetValue(1, 38).ToString(),
                                cellArray.GetValue(1, 39).ToString(),
                                cellArray.GetValue(1, 40).ToString(),
                                cellArray.GetValue(1, 41).ToString(),
                                cellArray.GetValue(1, 42).ToString(),
                                cellArray.GetValue(1, 43).ToString(),
                                cellArray.GetValue(1, 44).ToString(),
                                cellArray.GetValue(1, 45).ToString(),
                                cellArray.GetValue(1, 46).ToString(),
                                cellArray.GetValue(1, 47).ToString(),
                                cellArray.GetValue(1, 48).ToString(),
                                cellArray.GetValue(1, 49).ToString(),
                                cellArray.GetValue(1, 50).ToString(),
                                cellArray.GetValue(1, 51).ToString(),
                                cellArray.GetValue(1, 52).ToString(),
                                cellArray.GetValue(1, 53).ToString(),
                                cellArray.GetValue(1, 54).ToString()
                            )
                        );

                    //report progress
                    int progress = (int)(100.0 / (double)(dataCount) * (i-1));
                    bwSetData.ReportProgress(progress);
                }
            }

            try
            {
                outData.SaveSerialized();                               //save to temp file bdm.xml
                Globals.ThisAddIn.Application.ScreenUpdating = true;    //turn on screen updating
                return true;
            }
            catch
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                return false;
            }
        }

        /// <summary>
        /// Public method to load data from temp file bdm.xml
        /// </summary>
        public static void GetExcelData()
        {
            //if backgroundworker isn't running start it
            if (!bwGetData.IsBusy)
            {
                //check if excel is ready
                if (!ExcelRdy())
                {
                    return;
                }

                formProgGetData = new FormProgress();
                formProgGetData.OnCancelEvent += FormProgGetData_OnCancelEvent;     //register event to cancel progress from progress form
                formProgGetData.Show();

                stopWatch.Start();
                bwGetData.RunWorkerAsync();     //start bw
            }
        }

        /// <summary>
        /// Method to cancel backgroundworker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FormProgGetData_OnCancelEvent(object sender, EventArgs e)
        {
            bwGetData.CancelAsync();
        }

        /// <summary>
        /// Private method to get data from bdm.xml to excel sheet
        /// </summary>
        /// <returns>True if ok</returns>
        private static bool GetExcelData_private()
        {
            Globals.ThisAddIn.Application.ScreenUpdating = false;       //turn off screen updating - faster progress

            BDMdataClass inData = new BDMdataClass();
            inData.LoadDeSerialized();                                  //load data from temp bdm.xml

            Worksheet activeSheet = Globals.ThisAddIn.Application.ActiveSheet;
            activeSheet.Cells.Clear();      //clear list
            SetExcelHead();                 //set columns names

            int i = 1;                      //iterator for foreach loop

            //loop thru all objects loaded from xml file
            foreach (var obj in inData.Objects)
            {
                //if cancel from progress form isn't pressed
                if (!bwGetData.CancellationPending)
                {
                    i++;    //iterate i to have row index
                    Range rng = (Range)activeSheet.Range[activeSheet.Cells[i, 1], activeSheet.Cells[i, 54]];    //set range whole row
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
                                                obj.Init_HH_Lvl,
                                                obj.AE_H_Cfg,
                                                obj.Init_H_Lvl,
                                                obj.AE_L_Cfg,
                                                obj.Init_L_Lvl,
                                                obj.AE_LL_Cfg,
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
                                        };
                    //report progress
                    int progress = (int)(100.0 / (double)(inData.Objects.Count) * (i - 1));
                    bwGetData.ReportProgress(progress);
                }
            }

            Globals.ThisAddIn.Application.ScreenUpdating = true;        //turn on screen updating
            return true;
        }

        /// <summary>
        /// Checks if column TagName exists
        /// </summary>
        /// <returns>True if ok</returns>
        private static bool TagNameExist()
        {
            RangeObj rngTagName = new RangeObj(Resource.Col1);

            if (!rngTagName.Valid)
            {
                MessageBox.Show("TagName column not found.");
                return false;
            }
            else
            {
                return true;
            }
        }

        //Metohd to set object from array(row)
        private static BasicObject SetBasicObject(string tagname,
                            string descr,
                            string datatype,
                            string dbnr,
                            string positionindb,
                            string dbname,
                            string normalstate = "",
                            string aeconfigdiff = "",
                            string alarm_initdelay = "",
                            string aeseverity = "",
                            string pulsetime = "",
                            string bipolar = "",
                            string min = "",
                            string max = "",
                            string unit = "",
                            string inithhdly = "",
                            string inithdly = "",
                            string initldly = "",
                            string initlldly = "",
                            string aldb = "",
                            string aehhcfg = "",
                            string aehcfg = "",
                            string aelcfg = "",
                            string aellcfg = "",
                            string inithhlvl = "",
                            string inithlvl = "",
                            string initllvl = "",
                            string initlllvl = "",
                            string aehhseverity = "",
                            string aehseverity = "",
                            string aelseverity = "",
                            string aellseverity = "",
                            string sigfltseverity = "",
                            string fb0suffix = "",
                            string fb1cwsuffix = "",
                            string fb1ccwsuffix = "",
                            string cmd0suffix = "",
                            string cmd1cwsuffix = "",
                            string cmd1ccwsuffix = "",
                            string ibf = "",
                            string enauto = "",
                            string fbconfig = "",
                            string startstep = "",
                            string stopstep = "",
                            string startdelay = "",
                            string stopdelay = "",
                            string hornname = "",
                            string preselrbid = "",
                            string profinet_profibus_ID = "",
                            string devicenumber = "",
                            string s7comm = "",
                            string ioaddress = "",
                            string hmiarea = "",
                            string hmiparrent = "")
        {
            BasicObject obj = new BasicObject();

            obj.TagName = tagname;
            obj.Descr = descr;
            obj.DataType = datatype;
            obj.DBNr = dbnr;
            obj.PositionInDB = positionindb;
            obj.DBName = dbname;

            obj.NormalState = normalstate;
            obj.AEConfigDiff = aeconfigdiff;
            obj.Alarm_InitDelay = alarm_initdelay;
            obj.AESeverity = aeseverity;
            obj.PulseTime = pulsetime;

            obj.Bipolar = bipolar;
            obj.MIN = min;
            obj.MAX = max;
            obj.Unit = unit;
            obj.Init_HH_Dly = inithhdly;
            obj.Init_H_Dly = inithdly;
            obj.Init_L_Dly = initldly;
            obj.Init_LL_Dly = initlldly;
            obj.AL_DB = aldb;
            obj.AE_HH_Cfg = aehhcfg;
            obj.AE_H_Cfg = aehcfg;
            obj.AE_L_Cfg = aelcfg;
            obj.AE_LL_Cfg = aellcfg;
            obj.Init_HH_Lvl = inithhlvl;
            obj.Init_H_Lvl = inithlvl;
            obj.Init_L_Lvl = initllvl;
            obj.Init_LL_Lvl = initlllvl;
            obj.AEHH_severity = aehhseverity;
            obj.AEH_severity = aehseverity;
            obj.AEL_severity = aelseverity;
            obj.AELL_severity = aellseverity;
            obj.SigFltSeverity = sigfltseverity;

            obj.FB0_suffix = fb0suffix;
            obj.FB1_CW_suffix = fb1cwsuffix;
            obj.FB1_CCW_suffix = fb1ccwsuffix;
            obj.CMD0_suffix = cmd0suffix;
            obj.CMD1_CW_suffix = cmd1cwsuffix;
            obj.CMD1_CCW_suffix = cmd1ccwsuffix;
            obj.IBF = ibf;
            obj.EnAuto = enauto;
            obj.FBConfig = fbconfig;
            obj.StartStep = startstep;
            obj.StopStep = stopstep;
            obj.StartDelay = startdelay;
            obj.StopDelay = stopdelay;
            obj.HornName = hornname;

            obj.Presel_RBID = preselrbid;
            obj.ProfiNet_ProfiBus_ID = profinet_profibus_ID;
            obj.DeviceNumber = devicenumber;

            obj.IOAddress = ioaddress;

            obj.S7Comm = s7comm;

            obj.HMIArea = hmiarea;
            obj.HMIparrent = hmiparrent;

            return obj;
        }

        /// <summary>
        /// Checks if excel sheet is ready (not in cell edit mode)
        /// </summary>
        /// <returns>True if ready</returns>
        private static bool ExcelRdy()
        {
            object m = Type.Missing;
            const int MENU_ITEM_TYPE = 1;
            const int NEW_MENU = 18;
            bool result = false;

            CommandBarControl oNewMenu =
              Globals.ThisAddIn.Application.CommandBars["Worksheet Menu Bar"].FindControl(
                      MENU_ITEM_TYPE,   //the type of item to look for
                      NEW_MENU,         //the item to look for
                      m,                //the tag property (in this case missing)
                      m,                //the visible property (in this case missing)
                      true);            //we want to look for it recursively
                                        //so the last argument should be true.

            if (oNewMenu != null)
                if (oNewMenu.Enabled)
                    result = true;

            return result;
        }
    }

    //Class to simplify cell finding
    class RangeObj
    {
        public bool Valid { get; } = false;
        public int Row { get { return rng.Row; } }

        private Range rng;

        private Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;
        private Range rngAll;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellVal"></param>
        public RangeObj(string cellVal)
        {
            rngAll = sheet.get_Range("A1").EntireRow.EntireColumn;
            rng = rngAll.Find(cellVal);
            if (rng != null && !rng.EntireColumn.Hidden)
                Valid = true;
        }

        /// <summary>
        /// Offset to rangeobj
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetOffset(int i)
        {
            string result = "";

            if (Valid)
                result = rng.Offset[i, 0].Text;

            return result;
        }
    }
}
