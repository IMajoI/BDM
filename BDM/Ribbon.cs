using System.Diagnostics;
using Microsoft.Office.Tools.Ribbon;
using System.IO;
using System;
using BDMdata;

namespace BDM
{
    public partial class Ribbon
    {
        FileSystemWatcher watcher = null;

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            //register event for filewatcher - checks if temp file bdm.xml was changed and notify about that with icon change
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetTempPath() + @"\";                           //where u want to watch files
            watcher.NotifyFilter = NotifyFilters.LastWrite;                     //what type of notify you want to apply
            watcher.Filter = "bdm.xml";                                         //what file should be checked
            watcher.Changed += new FileSystemEventHandler(watcher_OnChanged);   //register event on changed

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
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);  //build output folder path
            path = path.Substring(6) + @"\BDM_Form.exe";    //extend it with app name
            Process.Start(path);                            //start interface app BDM_Form.exe
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
                (sender as RibbonToggleButton).Image = Resource.switch_7;

                LoadDataBtn.Enabled = true;
                OpnIfcBtn.Enabled = true;
                SetDataBtn.Enabled = true;
                SetHeadBtn.Enabled = true;
                watcher.EnableRaisingEvents = true;
            }
            else
            {
                GlobalData.bdmOn = false;
                (sender as RibbonToggleButton).Image = Resource.switch_6;

                LoadDataBtn.Enabled = false;
                OpnIfcBtn.Enabled = false;
                SetDataBtn.Enabled = false;
                SetHeadBtn.Enabled = false;
                watcher.EnableRaisingEvents = false;
            }
        }
    }

    public static class GlobalData
    {
        //global var to check if bdm tool is turned on
        public static bool bdmOn;
    }
}
