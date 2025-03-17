using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BDMdata
{
    #region GlobalData
    /// <summary>
    /// Global data - used in whole project
    /// </summary>
    public class GData
    {
        public BDMdataClass Data = new BDMdataClass();           //Full data, as a backup for filtering and sorting
        public BDMdataClass DataFiltered = new BDMdataClass();   //Used data for generating and binded to datagridview on MAIN tab, at start are same as Data
        public List<AObjectType> AObjectTypes { get; set; }     // Archestra object mapping information
        public BindingList<DBMapDef> DBMappingRef { get; set; } // Data block mapping information
        private string tempFile = Path.GetTempPath();
        /// <summary>
        /// Constructor
        /// </summary>
        public GData()
        {
            AObjectTypes = new List<AObjectType>();
            DBMappingRef = new BindingList<DBMapDef>();
            //At beggining load data and copy to datafiltered
            Data.LoadDeSerialized();
            CopyBDM(true);

        }
        public void CopyBDM(bool direction)
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

        public void SerializeDataTypes()
        {
            using (var writer = new StreamWriter("XMLSources/AObjectTypes.xml"))
            {
                var serializer = new XmlSerializer(AObjectTypes.GetType());
                serializer.Serialize(writer, AObjectTypes);
                writer.Flush();
            }
        }
        public void SerializeDataTypes(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(AObjectTypes.GetType());
                serializer.Serialize(writer, AObjectTypes);
                writer.Flush();
            }
        }

        public void DeserializeDataTypes()
        {
            try
            {
                if (File.Exists("XMLSources/AObjectTypes.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(AObjectTypes.GetType());
                    using (StreamReader sr = new StreamReader("XMLSources/AObjectTypes.xml"))
                    {
                        AObjectTypes = (List<AObjectType>)serializer.Deserialize(sr);
                    }
                }
                else
                {
                    throw new FileNotFoundException("File not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void DeserializeDataTypes(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    XmlSerializer serializer = new XmlSerializer(AObjectTypes.GetType());
                    using (StreamReader sr = new StreamReader(path))
                    {
                        AObjectTypes = (List<AObjectType>)serializer.Deserialize(sr);
                    }
                }
                else
                {
                    throw new FileNotFoundException("File not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public BindingList<string> aotToString()
        {
            BindingList<string> ListDataTypes = new BindingList<string>();
            foreach (var item in AObjectTypes)
            {
                try
                {
                    ListDataTypes.Add(item.DataTypeName.ToString());
                }
                catch
                {
                }

            }
            return ListDataTypes;
        }
        public void GetDBMapping()
        {
            DBMappingRef.Clear();
            foreach (var item in DataFiltered.Objects)
            {
                if (DBMappingRef.ToList<DBMapDef>().Find(x => x.DBName == item.DBName) == null)
                {
                    DBMapDef DBMD = new DBMapDef(item.DBName, item.DBNr, 0.ToString());
                    DBMappingRef.Add(DBMD);
                }
            }
        }

    }
    #endregion

    [Serializable]
    public class BDMdataClass
    {
        public BindingList<BasicObject> Objects { get; set; }   //list of basicobjects which represents rows in component list
        //(IDictionary<string, Object>
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
                var serializer = new XmlSerializer(Objects.GetType());
                serializer.Serialize(writer, Objects);
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
                    var serializer = new XmlSerializer(Objects.GetType());
                    serializer.Serialize(writer, Objects);
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
                    var serializer = new XmlSerializer(Objects.GetType());
                    Objects = serializer.Deserialize(stream) as BindingList<BasicObject>;
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
                        var serializer = new XmlSerializer(Objects.GetType());
                        Objects = serializer.Deserialize(stream) as BindingList<BasicObject>;
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
            corruptedObjs.AddRange(MissingDBInfo());    //Missing DBNr or PositionInDB

            if (corruptedObjs.Count == 0)
                corruptedObjs = null;

            return corruptedObjs;
        }

        /// <summary>
        /// Check if DBNr and PosInDB is set
        /// </summary>
        /// <returns></returns>
        private List<InvalidObject> MissingDBInfo()
        {
            List<InvalidObject> list = new List<InvalidObject>();

            foreach (var obj in Objects)
            {
                //check empty DBNr
                if (obj.DBNr == "")
                    list.Add(new InvalidObject("DBNr", obj.DBNr));

                //check empty PositionInDB
                if (obj.PositionInDB == "")
                    list.Add(new InvalidObject("PositionInDB", obj.PositionInDB));
            }

            return list;
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
            Regex regEx = new Regex(Resource.Bool + "|" +
                                    Resource.Int + "|" +
                                    Resource.AinType + "|" +
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
                                    Resource.PIDStepType + "|" +
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
        public string DataType { get; set; } = "empty";
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
        public string ValueDecimalPlaces { get; set; } = " ";
        public string HMI_HWSigFault_severity { get; set; } = " ";
        public string SecurityDefinition { get; set; } = " ";

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

    #region Public structure DB mapping
    /// <summary>
    /// Ctructure to define DB name db number and initial DB offset
    /// </summary>
    public class DBMapDef
    {
        public string DBName { get; set; }
        public string DBnr { get; set; }
        public string DBOffset { get; set; }
        public DBMapDef()
        {
        }
        public DBMapDef(string dbname, string dbnr, string dboffset)
        {
            DBName = dbname;
            DBnr = dbnr;
            DBOffset = dboffset;
        }

    }
    #endregion
    public class DataTypeProperties
    {
        public string propertyName { get; set; }
        public string propertyColumn { get; set; }
    }


    public class AObjectType
    {
        public string DataTypeName { get; set; }
        public string DataTypeSize { get; set; }
        public string FunctionName { get; set; }
        public string Template { get; set; }
        public BindingList<ATopicParametr> Attributes { get; set; }
        public BindingList<DataTypeProperties> DTProperties { get; set; }
        public BindingList<AGalaxyParametr> GalaxyAttributes { get; set; }
        public AObjectType()
        {
            Attributes = new BindingList<ATopicParametr>();
            GalaxyAttributes = new BindingList<AGalaxyParametr>();
        }
        public AObjectType(string name, string size)
        {
            DataTypeName = name;
            DataTypeSize = size;
            Attributes = new BindingList<ATopicParametr>();
            GalaxyAttributes = new BindingList<AGalaxyParametr>();
        }
        public override string ToString()
        {
            return DataTypeName;
        }
    }

    public class ATopicParametr
    {
        public string AttributeName { get; set; }
        public string DataType { get; set; }

        public ATopicParametr()
        {
        }

        public ATopicParametr(string attributename, string datatype)
        {
            AttributeName = attributename;
            DataType = datatype;
        }
    }
    public class AGalaxyParametr
    {
        public string AttributeName { get; set; }

        public string propertyColumn { get; set; }
        public AGalaxyParametr()
        {
        }
        public AGalaxyParametr(string pAttributeName)
        {
            AttributeName = pAttributeName;
        }
    }
    

    public class HeaderCls
    {

        public string HeaderName { get; set; }
        public bool Visible { get; set; }


    }

}
