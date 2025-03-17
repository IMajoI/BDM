using BDMdata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;



namespace TIAIFCNS
{
    public static class Z45TIAIFC
    {

        #region Public class constructor - Z45TIAIFC Class constructor
        /// <summary>
        /// Z45 TIA portal interface constructor
        /// <returns></returns>
        /// </summary>
        //public static Z45TIAIFC()
        //{

        //}
        #endregion


        public static string GenerateExtSrcDB(BindingList<BasicObject> ImportData, string DBName)
        {
            string SrcCode = "";
            int cnt = 0;
            SrcCode = "DATA_BLOCK \"" + DBName + "\"" + Environment.NewLine;
            SrcCode = SrcCode + "{ S7_Optimized_Access := 'FALSE' }" + Environment.NewLine;
            SrcCode = SrcCode + "VERSION : 0.1" + Environment.NewLine;
            SrcCode = SrcCode + "NON_RETAIN" + Environment.NewLine;
            SrcCode = SrcCode + "   STRUCT" + Environment.NewLine;
            foreach (var item in ImportData)
            {
                if (item.DBName == DBName)
                {
                    cnt++;
                    SrcCode = SrcCode + "       " + item.TagName + ":" + item.DataType + "; //" + item.Descr + Environment.NewLine;
                }
            }
            SrcCode = SrcCode + "   END_STRUCT;" + Environment.NewLine;
            SrcCode = SrcCode + "BEGIN" + Environment.NewLine;
            SrcCode = SrcCode + "END_DATA_BLOCK" + Environment.NewLine;
            if (cnt == 0)
            {
                SrcCode = "";
            }

            return SrcCode;
        }

        public static string GenerateExtSrcFC(BindingList<BasicObject> ImportData, string DBName, List<AObjectType> dataTypes)
        {
            string SrcCode = "";
            int cnt = 0;
            bool check;
            int rowNumber = 0;
            SrcCode = "FUNCTION \"" + DBName + "_FC\" : Void" + Environment.NewLine;
            SrcCode = SrcCode + "{ S7_Optimized_Access := 'false' }" + Environment.NewLine;
            SrcCode = SrcCode + "VERSION : 0.1" + Environment.NewLine;
            SrcCode = SrcCode + "BEGIN" + Environment.NewLine;
            foreach (var item in ImportData)
            {
                rowNumber++;
                string LIO = "";
                string[] stringArray;
                int indexDT = 0;
                int j;
                Type t = typeof(BasicObject);
                PropertyInfo[] properties = item.GetType().GetProperties();
                if (item.DBName == DBName)
                {
                    cnt++;
                    for (int i = 0; i < dataTypes.Count; i++)
                    {
                        if (item.DataType == dataTypes[i].DataTypeName)
                            break;
                        else
                            indexDT++;
                    }
                    check = true;
                    foreach (var itemm in dataTypes)
                    {
                        if (itemm.DataTypeName == item.DataType)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        MessageBox.Show("DataTypes are not correct! Row: " + (rowNumber+1).ToString() + Environment.NewLine + "Please check DataType list.");
                    }


                    SrcCode += "//" + item.TagName + "-" + item.Descr + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.IOAddress))
                    { SrcCode += "\"" + item.DBName + "\"." + item.TagName + @".HWSig:=""" + item.TagName + @""";" + Environment.NewLine; }
                    SrcCode += "\"" + dataTypes[indexDT].FunctionName + "\"(";

                    for (j = 0; j < dataTypes[indexDT].DTProperties.Count; j++)
                    {
                        foreach (var property in properties)
                        {
                            if (dataTypes[indexDT].DTProperties[j].propertyColumn == property.Name)
                                SrcCode += dataTypes[indexDT].DTProperties[j].propertyName + ":=" + property.GetValue(item).ToString() + "," + Environment.NewLine;
                        }

                        if (dataTypes[indexDT].DTProperties[j].propertyColumn == null)
                            SrcCode += dataTypes[indexDT].DTProperties[j].propertyName + ":=" + "," + Environment.NewLine;
                        if (dataTypes[indexDT].DTProperties[j].propertyName == "LIO")
                            LIO = dataTypes[indexDT].DTProperties[j].propertyColumn;
                    }
                    LIO = LIO.Replace(" ", "");
                    stringArray = LIO.Split('+');
                    for (int i = 0; i < stringArray.Count(); i++)
                    {
                        if (i == 0)
                        {
                            foreach (var property in properties)
                            {
                                if (property.Name == stringArray[0])
                                {
                                    SrcCode += "LIO:=" + property.GetValue(item).ToString();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var property in properties)
                            {
                                if (property.Name == stringArray[i])
                                {
                                    SrcCode += "." + property.GetValue(item).ToString();
                                    break;
                                }
                            }
                        }
                    }
                    SrcCode += ");" + Environment.NewLine;
                    SrcCode += Environment.NewLine;
                }
            }



            SrcCode += "END_FUNCTION" + Environment.NewLine;
            //MessageBox.Show(cnt.ToString());
            if (cnt == 0)
            {
                SrcCode = "";
            }
            return SrcCode;
        }

        public static void GeneratePLCTags(BindingList<BasicObject> ImportData, string FilePath)
        {
            int index = 1;
            Excel.Application xlApp = new Excel.Application();
            xlApp.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)xlApp.ActiveSheet;
            int rows = ImportData.Count + 1;
            int DataRow = 1;
            int columns = 7;
            var data = new object[rows, columns];
            Console.WriteLine(rows);
            ws.Name = "PLC Tags";
            data[0, 0] = "Name";
            data[0, 1] = "Path";
            data[0, 2] = "Data Type";
            data[0, 3] = "Logical Address";
            data[0, 4] = "Comment";
            data[0, 5] = "Hmi Visible";
            data[0, 6] = "Hmi Accessible";

            foreach (BasicObject BO in ImportData)
            {
                index++;
                if (!string.IsNullOrEmpty(BO.IOAddress) && BO.IOAddress!=" ")
                {
                    try
                    {
                        data[DataRow, 0] = BO.TagName;
                        data[DataRow, 1] = FilePath.Split('\\').Last();
                        if (BO.IOAddress.Contains("w") || BO.IOAddress.Contains("W"))
                        {
                            data[DataRow, 2] = "Int";
                        }
                        else
                        {
                            data[DataRow, 2] = "Bool";
                        }
                        data[DataRow, 3] = "%" + BO.IOAddress;
                        data[DataRow, 4] = BO.Descr;
                        data[DataRow, 5] = "True";
                        data[DataRow, 6] = "True";
                    }
                    catch
                    {
                    }
                    DataRow++;
                }
            }

            var startCell = (Excel.Range)ws.Cells[1, 1];
            var endCell = (Excel.Range)ws.Cells[rows, columns];
            var writeRange = ws.Range[startCell, endCell];
            writeRange.Value2 = data;

            xlApp.Workbooks[1].SaveAs(FilePath);
            xlApp.Workbooks[1].Close();
            xlApp.Quit();

        }

        public static void ImportTIAIOListSRC(BindingList<BasicObject> bolist, string FilePath)
        {
            Excel.Application xlApp = new Excel.Application();
            xlApp.Workbooks.Open(FilePath);
            Excel.Worksheet ws = (Excel.Worksheet)xlApp.ActiveSheet;
            Excel.Range ER = ws.UsedRange;
            Array DA = ER.Cells.Value;
            for (int i = 1; i < DA.GetLength(0); i++)
            {
                BasicObject bo;
                try
                {
                    bo = bolist.Single(k => (k.TagName == DA.GetValue(i + 1, 1).ToString()));
                }
                catch
                {
                    bo = new BasicObject();
                    if (DA.GetValue(i + 1, 4).ToString().ToLower().Contains("w"))
                    {
                        bo.DataType = Resource.Int;
                    }
                    else
                    {
                        bo.DataType = Resource.Bool;
                    }
                    bolist.Add(bo);
                }

                bo.TagName = DA.GetValue(i + 1, 1).ToString();
                bo.IOAddress = DA.GetValue(i + 1, 4).ToString().Replace("%", "");
                bo.Descr = DA.GetValue(i + 1, 5).ToString();
            }
            xlApp.Workbooks[1].Close();
            xlApp.Quit();
        }

        public static void ImportTIADBSRC(BindingList<BasicObject> bolist, string FilePath)
        {
            string DBImportTXT;
            using (TextReader reader = new StreamReader(FilePath))
            {
                DBImportTXT = reader.ReadToEnd();
                DBImportTXT = DBImportTXT.Replace("END_DATA_BLOCK", "");
                DBImportTXT = DBImportTXT.Replace("END_STRUCT", "");
                //DBImportTXT = DBImportTXT.Replace(" ", "");
            }
            string[] Splitter = { "DATA_BLOCK", "FUNCTION" };
            string[] DataBlocks = DBImportTXT.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
            foreach (string DataBlock in DataBlocks)
            {
                if (DataBlock.Contains("STRUCT"))
                {
                    Splitter.SetValue(Environment.NewLine, 0);
                    string ActDBName;
                    string[] SplittedDataBlock = DataBlock.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                    ActDBName = SplittedDataBlock[0].Replace(@"""", "").Replace(" ", string.Empty);
                    Splitter.SetValue("STRUCT", 0);
                    SplittedDataBlock = DataBlock.Replace(@"""", "").Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                    Splitter.SetValue(Environment.NewLine, 0);
                    SplittedDataBlock = SplittedDataBlock[1].Replace("BEGIN", "").Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string DBItem in SplittedDataBlock)
                    {
                        BasicObject bo;
                        if (DBItem.Contains(":"))
                        {
                            try
                            {
                                bo = bolist.Single(k => (k.TagName == DBItem.Substring(0, DBItem.IndexOf(":")).Replace(" ", "")));
                            }
                            catch
                            {
                                bo = new BasicObject();
                                bo.TagName = DBItem.Substring(0, DBItem.IndexOf(":")).Replace(" ", "");
                                bolist.Add(bo);
                            }
                            bo.DataType = DBItem.Substring(DBItem.IndexOf(":") + 1, DBItem.IndexOf(";") - DBItem.IndexOf(":") - 1);
                            if (DBItem.Contains("//"))
                            {
                                bo.Descr = DBItem.Substring(DBItem.IndexOf("//") + 2, DBItem.Length - DBItem.IndexOf("//") - 2);
                            }
                            else { bo.Descr = "Description missing"; }
                            bo.DBName = ActDBName;
                        }
                    }
                }

            }
        }

        public static void ImportTIAFCSRC(BindingList<BasicObject> bolist, string FilePath)
        {
            string DBImportTXT;
            using (TextReader reader = new StreamReader(FilePath))
            {
                DBImportTXT = reader.ReadToEnd();
            }
            DBImportTXT = DBImportTXT.Replace(" ", "");
            DBImportTXT = DBImportTXT.Replace(",", "");
            DBImportTXT = DBImportTXT.Replace(@"""", "");

            int StrtIndex = 0;
            int EndIndex = 0;
            string FCText;
            string[] FCTxtSplit;
            string[] Splitter = { Environment.NewLine };
            while (StrtIndex >= 0)
            {
                StrtIndex = DBImportTXT.IndexOf("FUNCTION", StrtIndex);
                while (StrtIndex >= 0)
                {
                    StrtIndex = DBImportTXT.IndexOf("(", StrtIndex + 1);
                    EndIndex = DBImportTXT.IndexOf(")", StrtIndex + 1);
                    if (StrtIndex >= 0)
                    {
                        BasicObject BO = new BasicObject();
                        FCText = DBImportTXT.Substring(StrtIndex + 1, EndIndex - StrtIndex - 1);
                        FCTxtSplit = FCText.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string FCLine in FCTxtSplit)
                        {

                            if (FCLine.ToLower().Contains("lio:="))
                            {
                                int indx = FCLine.IndexOf(".");
                                BO.TagName = FCLine.Substring(indx + 1, FCLine.Length - indx - 1);
                            }
                            else if (FCLine.ToLower().Contains("normalstate:="))
                            {
                                BO.NormalState = FCLine.ToLower().Replace("normalstate:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aeconfigdiff:="))
                            {
                                BO.AEConfigDiff = FCLine.ToLower().Replace("aeconfigdiff:=", "");
                            }
                            else if (FCLine.ToLower().Contains("bipolar:="))
                            {
                                BO.Bipolar = FCLine.ToLower().Replace("bipolar:=", "");
                            }
                            else if (FCLine.ToLower().Contains("max:="))
                            {
                                BO.MAX = FCLine.ToLower().Replace("max:=", "");
                            }
                            else if (FCLine.ToLower().Contains("min:="))
                            {
                                BO.MIN = FCLine.ToLower().Replace("min:=", "");
                            }
                            else if (FCLine.ToLower().Contains("alarminitdelayhh:="))
                            {
                                BO.Init_HH_Dly = FCLine.ToLower().Replace("alarminitdelayhh:=", "");
                            }
                            else if (FCLine.ToLower().Contains("alarminitdelayh:="))
                            {
                                BO.Init_H_Dly = FCLine.ToLower().Replace("alarminitdelayh:=", "");
                            }
                            else if (FCLine.ToLower().Contains("alarminitdelayl:="))
                            {
                                BO.Init_L_Dly = FCLine.ToLower().Replace("alarminitdelayl:=", "");
                            }
                            else if (FCLine.ToLower().Contains("alarminitdelayll:="))
                            {
                                BO.Init_LL_Dly = FCLine.ToLower().Replace("alarminitdelayll:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aedeadband:="))
                            {
                                BO.AL_DB = FCLine.ToLower().Replace("aedeadband:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aeconfighh:="))
                            {
                                BO.AE_HH_Cfg = FCLine.ToLower().Replace("aeconfighh:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aeconfigh:="))
                            {
                                BO.AE_H_Cfg = FCLine.ToLower().Replace("aeconfigh:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aeconfigl:="))
                            {
                                BO.AE_L_Cfg = FCLine.ToLower().Replace("aeconfigl:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aeconfigll:="))
                            {
                                BO.AE_LL_Cfg = FCLine.ToLower().Replace("aeconfigll:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aelevelhh:="))
                            {
                                BO.Init_HH_Lvl = FCLine.ToLower().Replace("aelevelhh:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aelevelh:="))
                            {
                                BO.Init_H_Lvl = FCLine.ToLower().Replace("aelevelh:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aelevell:="))
                            {
                                BO.Init_L_Lvl = FCLine.ToLower().Replace("aelevell:=", "");
                            }
                            else if (FCLine.ToLower().Contains("aelevelll:="))
                            {
                                BO.Init_LL_Lvl = FCLine.ToLower().Replace("aelevelll:=", "");
                            }
                            else if (FCLine.ToLower().Contains("fbconf:="))
                            {
                                BO.FBConfig = FCLine.ToLower().Replace("fbconf:=", "");
                            }
                            else if (FCLine.ToLower().Contains("pulsetime:="))
                            {
                                BO.PulseTime = FCLine.ToLower().Replace("pulsetime:=", "");
                            }
                            else if (FCLine.ToLower().Contains("alarminitdelay:="))
                            {
                                BO.Alarm_InitDelay = FCLine.ToLower().Replace("alarminitdelay:=", "");
                            }
                            else if (FCLine.ToLower().Contains("ibf:="))
                            {
                                BO.IBF = FCLine.ToLower().Replace("ibf:=", "");
                            }
                            else if (FCLine.ToLower().Contains("enauto:="))
                            {
                                BO.EnAuto = FCLine.ToLower().Replace("enauto:=", "");
                            }
                            else if (FCLine.ToLower().Contains("rbid:="))
                            {
                                BO.Presel_RBID = FCLine.ToLower().Replace("rbid:=", "");
                            }
                            else if (FCLine.ToLower().Contains("grpstartstep:="))
                            {
                                BO.StartStep = FCLine.ToLower().Replace("grpstartstep:=", "");
                            }
                            else if (FCLine.ToLower().Contains("grpstopstep:="))
                            {
                                BO.StopStep = FCLine.ToLower().Replace("grpstopstep:=", "");
                            }
                            else if (FCLine.ToLower().Contains("grpstartdelay:="))
                            {
                                BO.StartDelay = FCLine.ToLower().Replace("grpstartdelay:=", "");
                            }
                            else if (FCLine.ToLower().Contains("grpstopdelay:="))
                            {
                                BO.StopDelay = FCLine.ToLower().Replace("grpstopdelay:=", "");
                            }
                        }
                        try
                        {
                            BasicObject bo = (BasicObject)(bolist.Single(k => (k.TagName == BO.TagName)));
                            bo.AEConfigDiff = BO.AEConfigDiff;
                            bo.AE_HH_Cfg = BO.AE_HH_Cfg;
                            bo.AE_H_Cfg = BO.AE_H_Cfg;
                            bo.AE_LL_Cfg = BO.AE_LL_Cfg;
                            bo.AE_L_Cfg = BO.AE_L_Cfg;
                            bo.Alarm_InitDelay = BO.Alarm_InitDelay;
                            bo.AL_DB = BO.AL_DB;
                            bo.Bipolar = BO.Bipolar;
                            bo.EnAuto = BO.EnAuto;
                            bo.FBConfig = BO.FBConfig;
                            bo.IBF = BO.IBF;
                            bo.Init_HH_Dly = BO.Init_HH_Dly;
                            bo.Init_HH_Lvl = BO.Init_HH_Lvl;
                            bo.Init_H_Dly = BO.Init_H_Dly;
                            bo.Init_H_Lvl = BO.Init_H_Lvl;
                            bo.Init_L_Dly = BO.Init_L_Dly;
                            bo.Init_LL_Lvl = BO.Init_L_Lvl;
                            bo.Init_LL_Dly = BO.Init_LL_Dly;
                            bo.Init_LL_Lvl = BO.Init_LL_Lvl;
                            bo.MAX = BO.MAX;
                            bo.MIN = BO.MIN;
                            bo.NormalState = BO.NormalState;
                            bo.Presel_RBID = BO.Presel_RBID;
                            bo.PulseTime = BO.PulseTime;
                            bo.StartDelay = BO.StartDelay;
                            bo.StartStep = BO.StartStep;
                            bo.StopDelay = BO.StopDelay;
                            bo.StopStep = BO.StopStep;
                            bo.TagName = BO.TagName;
                        }
                        catch
                        {
                            bolist.Add(BO);
                        }
                    }
                }
            }
        }


        #region Public method - Calculate Data blocks mapping
        /// <summary>
        /// Calculate Data blocks mapping
        /// </summary>
        /// <param name="ImportData"></param>
        public static void CalculateDBMapping(GData BD)
        {
            int DBPosition;
            bool check;
            int rowNumber = 0;
            foreach (var DB in BD.DBMappingRef)
            {
                DBPosition = Convert.ToInt32(DB.DBOffset);
                foreach (BasicObject BO in BD.DataFiltered.Objects)
                {
                    if (BO.DBName == DB.DBName)
                    {
                        rowNumber++;
                        check = true;
                        BO.DBNr = DB.DBnr;
                        BO.PositionInDB = DBPosition.ToString();
                        foreach (var item in BD.AObjectTypes)
                        {
                            if (BO.DataType == item.DataTypeName)
                            {
                                check = false;
                            }
                        }
                        if (check)
                        {
                            MessageBox.Show("DataTypes are not correct! Row: " + (rowNumber+1).ToString() + Environment.NewLine + "Please check DataType list.");
                        }
                        DBPosition = DBPosition + int.Parse(BD.AObjectTypes.ToList<AObjectType>().Find(x => x.DataTypeName == BO.DataType).DataTypeSize);
                    }
                }
            }
        }
        #endregion       
    }

}

