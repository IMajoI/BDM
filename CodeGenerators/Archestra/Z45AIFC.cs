using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDMdata;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;

namespace AIFCNS
{
    public static class Z45AIFC
    {
        #region Public class constructor - Z45AIFC Class constructor
        /// <summary>
        /// Z45 TIA portal interface constructor
        /// <returns></returns>
        /// </summary>
        //static public Z45AIFC()
        //{
        //}
        #endregion

        #region Public method - export data to Archestra IDE
        /// <summary>
        /// Export data to csv
        /// </summary>
        /// <param name="path">Export data path</param>
        /// <param name="ExportData">Expor data content from filtered list</param>


        public static void ExportData(string path, GData pGlobalData, BindingList<HeaderCls> hdr)
        {
            string GalaxyLoadString = "";
            string TopicAliasesString = "";
            string AESeverity = "500";
            string numberString = "";
            string resultString = "";
            string[] stringArray;
            int positionInDB = 0;

            //TopicAliases

            for (int i = 0; i < pGlobalData.DataFiltered.Objects.Count; i++)
            {
                if (pGlobalData.DataFiltered.Objects[i].TagName != " ")
                {
                    bool check = true;
                    BasicObject ExportObject = pGlobalData.DataFiltered.Objects[i];
                    foreach (var item in pGlobalData.AObjectTypes)
                    {
                        if (item.DataTypeName == ExportObject.DataType)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        MessageBox.Show("DataTypes are not correct!" + Environment.NewLine + "Please check DataType list.");
                    }
                    AObjectType aot = pGlobalData.AObjectTypes.FirstOrDefault(d => d.DataTypeName == ExportObject.DataType);
                    if (!string.IsNullOrEmpty(ExportObject.AESeverity))
                    {
                        AESeverity = ExportObject.AESeverity;
                    }

                    foreach (ATopicParametr attribute in aot.Attributes)
                    {
                        numberString = Regex.Match(attribute.DataType, @"\d+").Value;
                        positionInDB = int.Parse(ExportObject.PositionInDB) + int.Parse(numberString);
                        stringArray = attribute.DataType.Split(numberString[0]);
                        resultString = string.Join("", stringArray, 1, stringArray.Count() - 2);
                        TopicAliasesString = TopicAliasesString + "\"" + ExportObject.TagName + "." + attribute.AttributeName + "\"" + CultureInfo.CurrentCulture.TextInfo.ListSeparator + "\"DB" + ExportObject.DBNr + "," + stringArray[0] + positionInDB + resultString + "\"" + Environment.NewLine;
                    }
                }
                else break;
            }

            //GalaxyLoad
            for (int i = 1; i < pGlobalData.AObjectTypes.Count; i++)
            {
                GalaxyLoadString += ":TEMPLATE=$" + pGlobalData.AObjectTypes[i].Template + Environment.NewLine;
                for (int j = 0; j < pGlobalData.AObjectTypes[i].GalaxyAttributes.Count(); j++)
                {
                    if (j == 0)
                    {
                        GalaxyLoadString += ":";
                        GalaxyLoadString += pGlobalData.AObjectTypes[i].GalaxyAttributes[0].AttributeName;
                    }
                    else
                    {
                        GalaxyLoadString += "," + pGlobalData.AObjectTypes[i].GalaxyAttributes[j].AttributeName;
                    }
                }
                GalaxyLoadString += Environment.NewLine;
                foreach (var item in pGlobalData.DataFiltered.Objects)
                {
                    PropertyInfo[] properties = item.GetType().GetProperties();
                    if (item.DataType == pGlobalData.AObjectTypes[i].DataTypeName)
                    {
                        for (int j = 0; j < pGlobalData.AObjectTypes[i].GalaxyAttributes.Count(); j++)
                        {
                            if (j == 0)
                            {
                                foreach (var property in properties)
                                {
                                    if (pGlobalData.AObjectTypes[i].GalaxyAttributes[j].propertyColumn == property.Name)
                                    {
                                        if (property.GetValue(item) == null)
                                        {
                                            GalaxyLoadString += "";
                                        }
                                        else
                                        {
                                            GalaxyLoadString += property.GetValue(item).ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var property in properties)
                                {
                                    if (pGlobalData.AObjectTypes[i].GalaxyAttributes[j].propertyColumn == property.Name)
                                    {
                                        if (property.GetValue(item) == null)
                                        {
                                            GalaxyLoadString += ",";
                                        }
                                        else
                                        {
                                            GalaxyLoadString += "," + property.GetValue(item).ToString();
                                        }

                                    }
                                }
                            }

                            if (pGlobalData.AObjectTypes[i].GalaxyAttributes[j].propertyColumn == null)
                                GalaxyLoadString += "";
                        }
                        GalaxyLoadString += Environment.NewLine;
                    }
                }
            }

            File.WriteAllText(path.Replace(".csv", "_GL.csv"), GalaxyLoadString);
            File.WriteAllText(path.Replace(".csv", "_TA.csv"), TopicAliasesString);
        }
        #endregion

        #region Public method - import data from Archestra IDE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDEExportText">Text from Archestra IDE file</param>
        /// <param name="ImportData">Data to be imported</param>
        public static void ImportData(string IDEExportText, GData ImportData)
        {
            string resultString = Regex.Replace(IDEExportText, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
            string[] SplitTemplates;
            string TemplateName;
            SplitTemplates = resultString.Split(new string[] { ":TEMPLATE=$" }, StringSplitOptions.None);
            for (int i = 1; i < SplitTemplates.Length; i++)
            {
                string[] SplitTemplate;
                string[] SplitTemplatePar;

                SplitTemplate = SplitTemplates[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                TemplateName = SplitTemplate[0];
                SplitTemplatePar = SplitTemplate[1].Split(',');
                SplitTemplatePar[0] = SplitTemplatePar[0].Replace(":", "");

                for (int j = 2; j < SplitTemplate.Length - 1; j++)
                {
                    BasicObject BO = new BasicObject();
                    string[] SplitTemplateValue = SplitTemplate[j].Split(',');
                    bool rwrite = false;


                    foreach (var item in ImportData.AObjectTypes.First(x => x.Template == TemplateName).GalaxyAttributes)
                    {
                        if (item.AttributeName == "TagName")
                        {
                            for (int x = 0; x < SplitTemplatePar.Length - 1; x++)
                            {
                                if (SplitTemplatePar[x] == "TagName")
                                {
                                    foreach (var objects in ImportData.DataFiltered.Objects)
                                    {
                                        if (objects.TagName == SplitTemplateValue[x])
                                        {
                                            PropertyInfo[] objProperties = objects.GetType().GetProperties();
                                            foreach (var gattributes in ImportData.AObjectTypes.First(a => a.Template == TemplateName).GalaxyAttributes)
                                            {
                                                for (int y = 0; y < SplitTemplatePar.Length - 1; y++)
                                                {
                                                    if (gattributes.AttributeName == SplitTemplatePar[y])
                                                    {
                                                        foreach (var property in objProperties)
                                                        {
                                                            if (property.Name == gattributes.propertyColumn)
                                                            {
                                                                if (property.GetValue(objects).ToString() == " " || property.GetValue(objects).ToString() == "")
                                                                {
                                                                    property.SetValue(objects, SplitTemplateValue[y], null);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            rwrite = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (rwrite == false)
                    {
                        foreach (var item in ImportData.AObjectTypes.First(x => x.Template == TemplateName).GalaxyAttributes)
                        {
                            for (int x = 0; x < SplitTemplatePar.Length - 1; x++)
                            {
                                if (item.AttributeName == SplitTemplatePar[x])
                                {
                                    PropertyInfo[] BOProperties = BO.GetType().GetProperties();

                                    foreach (var property in BOProperties)
                                    {
                                        if (property.Name == item.propertyColumn)
                                        {
                                            property.SetValue(BO, SplitTemplateValue[x], null);
                                            BO.DataType = ImportData.AObjectTypes.First(a => a.Template == TemplateName).DataTypeName;
                                        }
                                    }
                                }
                            }
                        }
                        ImportData.DataFiltered.Objects.Add(BO);
                    }
                }
            }
        }
        #endregion
    }
}
