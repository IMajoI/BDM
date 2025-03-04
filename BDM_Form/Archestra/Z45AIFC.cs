using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDMdata;
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace AIFCNS
{
    class Z45AIFC
    {
        #region Public class constructor - Z45AIFC Class constructor
        /// <summary>
        /// Z45 TIA portal interface constructor
        /// <returns></returns>
        /// </summary>
        public Z45AIFC()
        {
        }
        #endregion

        #region Public method - export data to Archestra IDE
        /// <summary>
        /// Export data to csv
        /// </summary>
        /// <param name="path">Export data path</param>
        /// <param name="ExportData">Expor data content from filtered list</param>
        public void ExportData(string path, BindingList<BasicObject> ExportData)
        {
            string GalaxyLoadString="";
            string TopicAliasesString ="";            
            GalaxyLoadString = ":TEMPLATE=$DIS" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription,HMI_SigAlarm.Priority" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.DinType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + "," + ExportObject.AESeverity + Environment.NewLine;
                    try
                    {                        
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW2\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 2).ToString() + "\"" + Environment.NewLine;
                    }
                    catch
                    {                     
                    }                    
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$DOS" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.DoutType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW2\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 2).ToString() + "\"" + Environment.NewLine;
                    }
                    catch
                    {
                    }
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$AIS" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription,HMI_SigAlarmHH.Priority,HMI_SigAlarmH.Priority,HMI_SigAlarmL.Priority,HMI_SigAlarmLL.Priority,HMI_HWSigFault.Priority,IFCDW8.EngUnits" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.AinType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + "," + ExportObject.AEHH_severity + "," + ExportObject.AEH_severity + "," + ExportObject.AEL_severity + "," + ExportObject.AELL_severity + "," + ExportObject.SigFltSeverity + "," + ExportObject.Unit + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW6\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 6).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW8\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 8).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW12\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 12).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW16\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 16).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW20\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 20).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW24\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 24).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW28\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 28).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW32\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 32).ToString() + "\"" + Environment.NewLine;
                    }
                    catch
                    {
                    }
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$AOS" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription,IFCDW2.EngUnits" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.AoutType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + ExportObject.Unit + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW2\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 2).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW6\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 6).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW10\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 10).ToString() + "\"" + Environment.NewLine;    
                    }
                    catch
                    {
                    }
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$OnOffCtrl" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription,HMI_GrStartStep,HMI_GrStopStep" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.OnOffType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + "," + ExportObject.StartStep + "," + ExportObject.StopStep + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW8\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 8).ToString() + "\"" + Environment.NewLine;                       
                    }
                    catch
                    {
                    }
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$OnOffCtrl_2D" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription,HMI_GrStartStep,HMI_GrStopStep" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.OnOff2DType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + "," + ExportObject.StartStep + "," + ExportObject.StopStep + Environment.NewLine;                    
                }
                try
                {
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW8\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + (int.Parse(ExportObject.PositionInDB) + 8).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW12\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 12).ToString() + "\"" + Environment.NewLine;
                }
                catch
                {
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$OnOffCtrl_VSD" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription,HMI_GrStartStep,HMI_GrStopStep" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.OnOffVSDType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + "," + ExportObject.StartStep + "," + ExportObject.StopStep + Environment.NewLine;                    
                }
                try
                {
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW8\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + (int.Parse(ExportObject.PositionInDB) + 8).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW12\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 12).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW14\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 14).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW16\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 16).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW20\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 20).ToString() + "\"" + Environment.NewLine;
                }
                catch
                {
                }

            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$AnalogPosCtrl" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.AnalogPosType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + Environment.NewLine;                    
                }
                try
                {
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW6\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 6).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW8\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 8).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW12\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 12).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW16\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 16).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW20\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 20).ToString() + "\"" + Environment.NewLine;
                    TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW24\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 24).ToString() + "\"" + Environment.NewLine;
                }
                catch
                {
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$PID" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.PIDType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".D" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW6\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 6).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW10\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 10).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW14\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 14).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW18\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 18).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW22\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 22).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW26\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 26).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW30\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 30).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW34\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 34).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW38\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 38).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW42\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 42).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCDW46\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".REAL" + (int.Parse(ExportObject.PositionInDB) + 46).ToString() + "\"" + Environment.NewLine;
                    }
                    catch
                    {
                    }
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$GroupCtrl" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.GrpType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW0\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW2\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 3).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW4\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 4).ToString() + "\"" + Environment.NewLine;
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW6\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + (int.Parse(ExportObject.PositionInDB) + 6).ToString() + "\"" + Environment.NewLine;                        
                    }
                    catch
                    {
                    }
                }
            }
            GalaxyLoadString = GalaxyLoadString + ":TEMPLATE=$Presel" + Environment.NewLine;
            GalaxyLoadString = GalaxyLoadString + ":Tagname,Area,SecurityGroup,ObjectDescription" + Environment.NewLine;
            foreach (var ExportObject in ExportData)
            {
                if (ExportObject.DataType == Resource.PreselType)
                {
                    GalaxyLoadString = GalaxyLoadString + ExportObject.TagName + "," + ExportObject.HMIArea + ",Administrator," + ExportObject.Descr + Environment.NewLine;
                    try
                    {
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + ".IFCW0\""+ CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + ".W" + ExportObject.PositionInDB + "\"" + Environment.NewLine;
                        
                    }
                    catch
                    {
                    }
                }
            }




            File.WriteAllText(path.Replace(".csv","_GL.csv"), GalaxyLoadString);
            File.WriteAllText(path.Replace(".csv", "_TA.csv"), TopicAliasesString);
        }
        #endregion
    }
}
