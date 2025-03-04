using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BDMdata
{
    [Serializable]
    public class BDMdataClass
    {
        public BindingList<BasicObject> Objects { get; set; }   //list of basicobjects which represents rows in component list
        public int Count { get { return Objects.Count; } }      //count of objects/rows

        private string tempFile = Path.GetTempPath() + @"\bdm.xml"; //location of temp bdm.xml file

        /// <summary>
        /// Constructor
        /// </summary>
        public BDMdataClass()
        {
            Objects = new BindingList<BasicObject>();
        }

        /// <summary>
        /// Serialize and save to temp.
        /// </summary>
        public void SaveSerialized()
        {
            using (var writer = new StreamWriter(tempFile))
            {
                var serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        /// <summary>
        /// Serialize and save to desired location
        /// </summary>
        public bool SaveSerializedToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "bdm.xml";
            sfd.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";

            bool success = false;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var writer = new StreamWriter(sfd.FileName))
                {
                    var serializer = new XmlSerializer(GetType());
                    serializer.Serialize(writer, this);
                    writer.Flush();
                }
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Load from temp and deserialize.
        /// </summary>
        public void LoadDeSerialized()
        {
            try
            {
                using (var stream = File.OpenRead(tempFile))
                {
                    var serializer = new XmlSerializer(GetType());
                    BDMdataClass tempdata = serializer.Deserialize(stream) as BDMdataClass;
                    Objects.Clear();

                    foreach (var obj in tempdata.Objects)
                    {
                        Objects.Add(obj);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Load from desired file and deserialize.
        /// </summary>
        public bool LoadDeSerializedFromFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";

            bool success = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var stream = File.OpenRead(ofd.FileName))
                    {
                        var serializer = new XmlSerializer(GetType());
                        BDMdataClass tempdata = serializer.Deserialize(stream) as BDMdataClass;
                        Objects.Clear();

                        foreach (var obj in tempdata.Objects)
                        {
                            Objects.Add(obj);
                        }
                    }
                    success = true;
                }
                catch { }
            }

            return success;
        }

        #region Data Validation methods

        /// <summary>
        /// Checks objects for validity.
        /// </summary>
        /// <returns>Corrupted objects in list</returns>
        public List<InvalidObject> Invalid()
        {
            List<InvalidObject> corruptedObjs = new List<InvalidObject>();

            corruptedObjs.AddRange(BadTagName());       //TagName check
            corruptedObjs.AddRange(BadDataType());      //DataType check
            corruptedObjs.AddRange(BadDBName());        //DBName check

            if (corruptedObjs.Count == 0)
                corruptedObjs = null;

            return corruptedObjs;
        }

        /// <summary>
        /// Check if TagName values are ok
        /// </summary>
        /// <returns></returns>
        private List<InvalidObject> BadTagName()
        {
            List<InvalidObject> list = new List<InvalidObject>();

            Regex regEx = new Regex(@"\w+\s+\w+|\b\d+");  // check for spaces in tagname or first letter is digit

            var duplicates = Objects.GroupBy(x => x.TagName).Where(group => group.Count() > 1).Select(group => group.Key).ToList(); //find all duplicities

            foreach (var obj in Objects)
            {
                //if spaces or numbers at start in tagname
                if (regEx.Match(obj.TagName).Success)
                    list.Add(new InvalidObject("TagName", obj.TagName));

                foreach (var dup in duplicates)
                {
                    //if tagname duplicated
                    if (obj.TagName.Equals(dup))
                        list.Add(new InvalidObject("TagName", obj.TagName));
                }
            }

            return list;
        }

        /// <summary>
        /// Check if DataType values are ok
        /// </summary>
        /// <returns></returns>
        private List<InvalidObject> BadDataType()
        {
            List<InvalidObject> list = new List<InvalidObject>();

            //build condition from resource to fit datatype with tiaportal lib datatypes
            Regex regEx = new Regex(Resource.AinType + "|" +
                                    Resource.AnalogPosType + "|" +
                                    Resource.AoutType + "|" +
                                    Resource.DiagCPU1500 + "|" +
                                    Resource.DiagCPU300 + "|" +
                                    Resource.DiagDev1500 + "|" +
                                    Resource.DiagDev300 + "|" +
                                    Resource.DinType + "|" +
                                    Resource.DoutType + "|" +
                                    Resource.GrpType + "|" +
                                    Resource.OnOff2DType + "|" +
                                    Resource.OnOffType + "|" +
                                    Resource.OnOffVSDType + "|" +
                                    Resource.PIDType + "|" +
                                    Resource.PreselType + "|" +
                                    Resource.SeqDataType);  

            foreach (var obj in Objects)
            {
                //if datatype has no value
                if (obj.DataType == null)
                {
                    list.Add(new InvalidObject("DataType", obj.DataType));
                }
                else
                {
                    //if datatype doesn't mach with any of valid types
                    if (!regEx.Match(obj.DataType).Success)
                        list.Add(new InvalidObject("DataType", obj.DataType));
                }
            }
            return list;
        }

        /// <summary>
        /// Check if DBname values are ok
        /// </summary>
        /// <returns></returns>
        private List<InvalidObject> BadDBName()
        {
            List<InvalidObject> list = new List<InvalidObject>();

            //build condition from resource to fit datatype with tiaportal lib datatypes
            Regex regEx = new Regex("^" + Resource.AinType.ToLower() + "$|^" +
                                    Resource.AnalogPosType.ToLower() + "$|^" +
                                    Resource.AoutType.ToLower() + "$|^" +
                                    Resource.DiagCPU1500.ToLower() + "$|^" +
                                    Resource.DiagCPU300.ToLower() + "$|^" +
                                    Resource.DiagDev1500.ToLower() + "$|^" +
                                    Resource.DiagDev300.ToLower() + "$|^" +
                                    Resource.DinType.ToLower() + "$|^" +
                                    Resource.DoutType.ToLower() + "$|^" +
                                    Resource.GrpType.ToLower() + "$|^" +
                                    Resource.OnOff2DType.ToLower() + "$|^" +
                                    Resource.OnOffType.ToLower() + "$|^" +
                                    Resource.OnOffVSDType.ToLower() + "$|^" +
                                    Resource.PIDType.ToLower() + "$|^" +
                                    Resource.PreselType.ToLower() + "$|^" +
                                    Resource.SeqDataType.ToLower() + "$");

            foreach (var obj in Objects)
            {
                //if dbname has no value
                if (obj.DBName == " ")
                {
                    list.Add(new InvalidObject("DBName", obj.DBName));
                }
                else
                {
                    //if dbname mach with any of valid types
                    if (regEx.Match(obj.DBName.ToLower()).Success)
                        list.Add(new InvalidObject("DBName", obj.DBName));
                }
            }
            return list;
        }
        #endregion
    }

    [Serializable]
    public class BasicObject
    {
        //method to get property by name
        public string GetPropValue(string propName)
        { 
            return GetType().GetProperty(propName).GetValue(this, null).ToString();
        }

        public string TagName { get; set; } = " ";
        public string Descr { get; set; } = " ";
        public string DataType { get; set; } = " ";
        public string DBNr { get; set; } = " ";
        public string PositionInDB { get; set; } = " ";
        public string DBName { get; set; } = " ";
        public string NormalState { get; set; } = " ";
        public string AEConfigDiff { get; set; } = " ";
        public string Alarm_InitDelay { get; set; } = " ";
        public string AESeverity { get; set; } = " ";
        public string PulseTime { get; set; } = " ";
        public string Bipolar { get; set; } = " ";
        public string MIN { get; set; } = " ";
        public string MAX { get; set; } = " ";
        public string Unit { get; set; } = " ";
        public string Init_HH_Dly { get; set; } = " ";
        public string Init_H_Dly { get; set; } = " ";
        public string Init_L_Dly { get; set; } = " ";
        public string Init_LL_Dly { get; set; } = " ";
        public string AL_DB { get; set; } = " ";
        public string AE_HH_Cfg { get; set; } = " ";
        public string AE_H_Cfg { get; set; } = " ";
        public string AE_L_Cfg { get; set; } = " ";
        public string AE_LL_Cfg { get; set; } = " ";
        public string Init_HH_Lvl { get; set; } = " ";
        public string Init_H_Lvl { get; set; } = " ";
        public string Init_L_Lvl { get; set; } = " ";
        public string Init_LL_Lvl { get; set; } = " ";
        public string AEHH_severity { get; set; } = " ";
        public string AEH_severity { get; set; } = " ";
        public string AEL_severity { get; set; } = " ";
        public string AELL_severity { get; set; } = " ";
        public string SigFltSeverity { get; set; } = " ";
        public string FB0_suffix { get; set; } = " ";
        public string FB1_CW_suffix { get; set; } = " ";
        public string FB1_CCW_suffix { get; set; } = " ";
        public string CMD0_suffix { get; set; } = " ";
        public string CMD1_CW_suffix { get; set; } = " ";
        public string CMD1_CCW_suffix { get; set; } = " ";
        public string IBF { get; set; } = " ";
        public string EnAuto { get; set; } = " ";
        public string FBConfig { get; set; } = " ";
        public string StartStep { get; set; } = " ";
        public string StopStep { get; set; } = " ";
        public string StartDelay { get; set; } = " ";
        public string StopDelay { get; set; } = " ";
        public string HornName { get; set; } = " ";
        public string Presel_RBID { get; set; } = " ";
        public string ProfiNet_ProfiBus_ID { get; set; } = " ";
        public string DeviceNumber { get; set; } = " ";
        public string S7Comm { get; set; } = " ";
        public string IOAddress { get; set; } = " ";
        public string HMIArea { get; set; } = " ";
        public string HMIparrent { get; set; } = " ";

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicObject()
        {
            //empty constructor for serialization purposes
        }
    }
    /// <summary>
    /// Class to store invalid objects find during invalidcheck
    /// </summary>
    public class InvalidObject
    {
        public string Column { get; set; }
        public string Value { get; set; }

        public InvalidObject(string col, string val)
        {
            Column = col;
            Value = val;
        }
    }
}
