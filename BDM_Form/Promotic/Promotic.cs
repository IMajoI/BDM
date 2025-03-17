using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BDMdata;

namespace BDM_Form
{
    /// <summary>
    /// Class for work with PromoticClass
    /// </summary>
    static class Promotic
    {
        private static double progStatus;   //progress of status
        private static double progStep;     //progress step - just for calculations

        #region Event OnUpdateStatus
 
        //Update status event to report out inner progress status
        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public static event StatusUpdateHandler OnUpdateStatus;

        /// <summary>
        /// Custom eventargs to pass status progress
        /// </summary>
        public class ProgressEventArgs : EventArgs
        {
            public int Status { get; private set; }     //percentage status
            public string Msg { get; private set; }     //message to help user what is being done in background

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
            //else return args with status and message
            ProgressEventArgs args = new ProgressEventArgs(status, msg);
            OnUpdateStatus(null, args);
        }
        #endregion

        #region main public methods
        /// <summary>
        /// Complete xml file
        /// </summary>
        /// <param name="data">BDMData we want to build</param>
        /// <param name="filePath">Save location</param>
        public static void BuildXML(BDMdataClass data, string filePath)
        {
            PromoticClass xmlDoc = new PromoticClass();     //root object of xml

            progStatus = 0;                 //zero progress status at start
            progStep = 80.0 / data.Count;   //calculate step : 20% of process takes copying, etc. and rest 80% takes generaiting objects, so 80.0 / number of objects 

            UpdateStatus(progStatus, "Promotic Root object");   //update inner status out

            #region PromoticClass object
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //PromoticClass object root settings
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            //Users
            PromoticClass.PropsClass usersProps = new PromoticClass.PropsClass();
            usersProps.Name = "Users";
            xmlDoc.Cfg.Content.Props = new List<PromoticClass.PropsClass>();
            xmlDoc.Cfg.Content.Props.Add(usersProps);

            PromoticClass.ListClass UsersList = new PromoticClass.ListClass();
            UsersList.Name = "Users";
            UsersList.Props = new List<PromoticClass.PropsClass>();
            usersProps.List = new List<PromoticClass.ListClass>();
            usersProps.List.Add(UsersList);

            UsersList.Props.Add(UserProps("$NOUSER_LOCAL", UserType.Local, null, 0, null, false, "asdf"));
            UsersList.Props.Add(UserProps("$NOUSER_NET", UserType.Net, null, 0, null, false, "asdf"));
            UsersList.Props.Add(UserProps("USER_ADMIN", UserType.LocalNet, "$ADMIN, $OPER,ELECT,ENGI", 3, "ADMIN", true, "sivres"));
            UsersList.Props.Add(UserProps("USER_OPER", UserType.LocalNet, "$OPER", 0, "OPER", false, ""));
            UsersList.Props.Add(UserProps("USER_ELECT", UserType.LocalNet, "$OPER,ELECT,ENGI", 2, "ELECT", true, "el"));
            UsersList.Props.Add(UserProps("USER_ENGI", UserType.LocalNet, "$OPER,ENGI", 1, "ENGI", true, "en"));
            //here can be added more users if needed

            PromoticClass.PropsClass AccessList = new PromoticClass.PropsClass();
            AccessList.Name = "AccessList";
            xmlDoc.Cfg.Content.Props.Add(AccessList);
            AccessList.List = new List<PromoticClass.ListClass>();

            PromoticClass.ListClass Actions = new PromoticClass.ListClass();
            Actions.Name = "Actions";
            AccessList.List.Add(Actions);

            //Specification which user roles can proceed: AppStop, InfoEdit, InfoShow and WebMethods
            Actions.Props = new List<PromoticClass.PropsClass>();
            Actions.Props.Add(SetPropsType3("AppStop", "Users", "$ANY_LOCAL"));
            Actions.Props.Add(SetPropsType3("InfoEdit", "Users", "$ADMIN,SERVICE"));
            Actions.Props.Add(SetPropsType3("InfoShow", "Users", "$ANY_LOCAL,$ANY_NET"));
            Actions.Props.Add(SetPropsType3("WebMethods", "Users", "$ANY_LOCAL,$ANY_NET"));

            //Groups
            PromoticClass.ListClass GroupList = new PromoticClass.ListClass();
            GroupList.Name = "UserGroups";
            GroupList.Props = new List<PromoticClass.PropsClass>();

            usersProps.List.Add(GroupList);

            GroupList.Props.Add(UserGroupsProps("$ADMIN"));
            GroupList.Props.Add(UserGroupsProps("$OPER"));
            GroupList.Props.Add(UserGroupsProps("ELECT"));
            GroupList.Props.Add(UserGroupsProps("ENGI"));
            //here can be added more groups if needed

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            //PromoticClass object root settings
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Objects in root
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            //Folders
            PromoticClass.PmObjectClass appCoreFolder = PmFolderObj("Z45AppCore");
            appCoreFolder.PmObject = new List<PromoticClass.PmObjectClass>();

            xmlDoc.Cfg.Content.PmObjects = new PromoticClass.PmObjectsClass();
            xmlDoc.Cfg.Content.PmObjects.PmObject = new List<PromoticClass.PmObjectClass>();
            xmlDoc.Cfg.Content.PmObjects.PmObject.Add(appCoreFolder);           //add Z45AppCore folder to root
            xmlDoc.Cfg.Content.PmObjects.PmObject.Add(Z45_Panels_PmFolder());   //add Z45Panels folder to root
            xmlDoc.Cfg.Content.PmObjects.PmObject.Add(DefaultCommObj());        //add default Comm obj with s7 connection to root

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            //Objects in root
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Objects in Z45AppCore folder
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            //Folders
            appCoreFolder.PmObject.Add(Z45_Graphics_PmFolder());    //add z45Graphics folder to z45appcore folder

            //PmData - tags
            //create all objects by passing only list of specific datatype to method
            appCoreFolder.PmObject.Add(Ain_PmFolder(data.Objects.Where(x => x.DataType == Resource.AinType).ToList()));
            appCoreFolder.PmObject.Add(AnalogPos_PmFolder(data.Objects.Where(x => x.DataType == Resource.AnalogPosType).ToList()));
            appCoreFolder.PmObject.Add(Aout_PmFolder(data.Objects.Where(x => x.DataType == Resource.AoutType).ToList()));
            appCoreFolder.PmObject.Add(Din_PmFolder(data.Objects.Where(x => x.DataType == Resource.DinType).ToList()));
            appCoreFolder.PmObject.Add(Dout_PmFolder(data.Objects.Where(x => x.DataType == Resource.DoutType).ToList()));
            appCoreFolder.PmObject.Add(Grp_PmFolder(data.Objects.Where(x => x.DataType == Resource.GrpType).ToList()));
            appCoreFolder.PmObject.Add(OnOff_PmFolder(data.Objects.Where(x => x.DataType == Resource.OnOffType).ToList()));
            appCoreFolder.PmObject.Add(OnOff2D_PmFolder(data.Objects.Where(x => x.DataType == Resource.OnOff2DType).ToList()));
            appCoreFolder.PmObject.Add(OnOffVSD_PmFolder(data.Objects.Where(x => x.DataType == Resource.OnOffVSDType).ToList()));
            appCoreFolder.PmObject.Add(PID_PmFolder(data.Objects.Where(x => x.DataType == Resource.PIDType).ToList()));
            appCoreFolder.PmObject.Add(PIDStep_PmFolder(data.Objects.Where(x => x.DataType == Resource.PIDStepType).ToList()));
            appCoreFolder.PmObject.Add(Presel_PmFolder(data.Objects.Where(x => x.DataType == Resource.PreselType).ToList()));

            UpdateStatus(85, "Alarms, Trends, etc..");  //update progress status and message

            //PmData - AlarmStripes
            appCoreFolder.PmObject.Add(AlarmStripes_PmData());

            //PmData - Colors
            appCoreFolder.PmObject.Add(Colors_PmData());

            //PmData - CustomData
            appCoreFolder.PmObject.Add(CustomData_PmData());

            //PmTrend - Trends

            //check if any trendability objects, if not, do not create trend object
            var match = data.Objects.Where(obj => (obj.DataType == Resource.AinType) || obj.DataType == Resource.F_AinType ||
                                                  (obj.DataType == Resource.AnalogPosType) ||
                                                  (obj.DataType == Resource.AoutType) ||
                                                  (obj.DataType == Resource.PIDType) ||
                                                  (obj.DataType == Resource.PIDStepType) ||
                                                  (obj.DataType == Resource.OnOffVSDType));

            if (match.Any())
                appCoreFolder.PmObject.Add(Trends_PmTrend());

            //PmAlarmEvent - Events
            appCoreFolder.PmObject.Add(Events_PmAlarmEvent());

            //PmAlarmEvent - Alarms
            appCoreFolder.PmObject.Add(Alarms_PmAlarmEvent(data));

            //PmData - SaveSettings
            appCoreFolder.PmObject.Add(SaveSettings_PmData(data));

            //PmWeb - Web
            appCoreFolder.PmObject.Add(Web_PmWeb());

            //PmWorkspace - Workspace
            //appCoreFolder.PmObject.Add(Workspace_PmWorkspace());

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            //Objects in Z45AppCore folder
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            UpdateStatus(90, "Serialization");  //update status progress

            #endregion

            #region Serialization + Faceplates import

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Serialization of PromoticClass class object
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            XmlWriterSettings xmlSet = new XmlWriterSettings();
            xmlSet.Encoding = Encoding.Unicode;
            xmlSet.Indent = true;
            xmlSet.IndentChars = "    ";

            XmlSerializer serializer = new XmlSerializer(typeof(PromoticClass));

            using (XmlWriter writer = XmlWriter.Create(filePath, xmlSet))
            {
                serializer.Serialize(writer, xmlDoc);
            }

            //add faceplates
            UpdateStatus(95, "Importing Faceplates");   //update progress status
            AddXML(@"\Promotic\Scripts\Faceplates.xml", filePath, "Z45_Graphics");

            //add workspace
            UpdateStatus(95, "Importing Workspace");   //update progress status
            AddXML(@"\Promotic\Scripts\Workspace.xml", filePath, "Events");



            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            //Add faceplates with XElement.AddAfterSelf method
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            #endregion

            UpdateStatus(100, "Complete");  //update progress status
        }

        /// <summary>
        /// Build only part of aplication - content of folder Z45AppCore
        /// </summary>
        /// <param name="data">BDMDataClass</param>
        /// <param name="filePath">Savefile location</param>
        public static void BuildXMLPart(BDMdataClass data, string filePath)
        {
            PromoticClass xmlDoc = new PromoticClass();

            progStatus = 0;
            progStep = 80.0 / data.Count;

            UpdateStatus(0, "Z45AppCore folder");

            //Folder
            PromoticClass.PmObjectClass appCoreFolder = PmFolderObj("Z45AppCore");
            appCoreFolder.PmObject = new List<PromoticClass.PmObjectClass>();

            xmlDoc.Cfg.Content = new PromoticClass.ContentClass();
            xmlDoc.Cfg.Content.PmObject = new List<PromoticClass.PmObjectClass>();
            xmlDoc.Cfg.Content.PmObject.Add(appCoreFolder);

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Objects in Z45AppCore folder
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            //Folders
            //appCoreFolder.PmObject.Add(PmFolderObj("Z45_Faceplates"));
            //appCoreFolder.PmObject.Add(Z45_Graphics_PmFolder());

            //PmData - tags
            appCoreFolder.PmObject.Add(Ain_PmFolder(data.Objects.Where(x => x.DataType == Resource.AinType).ToList()));
            appCoreFolder.PmObject.Add(AnalogPos_PmFolder(data.Objects.Where(x => x.DataType == Resource.AnalogPosType).ToList()));
            appCoreFolder.PmObject.Add(Aout_PmFolder(data.Objects.Where(x => x.DataType == Resource.AoutType).ToList()));
            appCoreFolder.PmObject.Add(Din_PmFolder(data.Objects.Where(x => x.DataType == Resource.DinType).ToList()));
            appCoreFolder.PmObject.Add(Dout_PmFolder(data.Objects.Where(x => x.DataType == Resource.DoutType).ToList()));
            appCoreFolder.PmObject.Add(Grp_PmFolder(data.Objects.Where(x => x.DataType == Resource.GrpType).ToList()));
            appCoreFolder.PmObject.Add(OnOff_PmFolder(data.Objects.Where(x => x.DataType == Resource.OnOffType).ToList()));
            appCoreFolder.PmObject.Add(OnOff2D_PmFolder(data.Objects.Where(x => x.DataType == Resource.OnOff2DType).ToList()));
            appCoreFolder.PmObject.Add(OnOffVSD_PmFolder(data.Objects.Where(x => x.DataType == Resource.OnOffVSDType).ToList()));
            appCoreFolder.PmObject.Add(PIDStep_PmFolder(data.Objects.Where(x => x.DataType == Resource.PIDStepType).ToList()));
            appCoreFolder.PmObject.Add(PID_PmFolder(data.Objects.Where(x => x.DataType == Resource.PIDType).ToList()));
            appCoreFolder.PmObject.Add(Presel_PmFolder(data.Objects.Where(x => x.DataType == Resource.PreselType).ToList()));

            UpdateStatus(85, "Alarms, Trends, etc..");

            //PmData - AlarmStripes
            appCoreFolder.PmObject.Add(AlarmStripes_PmData());

            //PmData - Colors
            appCoreFolder.PmObject.Add(Colors_PmData());

            //PmData - CustomData
            appCoreFolder.PmObject.Add(CustomData_PmData());

            //PmTrend - Trends

            //check if any trendability objects
            var match = data.Objects.Where(obj => (obj.DataType == Resource.AinType) || obj.DataType == Resource.F_AinType ||
                                                  (obj.DataType == Resource.AnalogPosType) ||
                                                  (obj.DataType == Resource.AoutType) ||
                                                  (obj.DataType == Resource.PIDType) ||
                                                  (obj.DataType == Resource.PIDStepType) ||
                                                  (obj.DataType == Resource.OnOffVSDType));

            if (match.Any())
                appCoreFolder.PmObject.Add(Trends_PmTrend());

            //PmAlarmEvent - Events
            appCoreFolder.PmObject.Add(Events_PmAlarmEvent());

            //PmAlarmEvent - Alarms
            appCoreFolder.PmObject.Add(Alarms_PmAlarmEvent(data));

            //PmData - SaveSettings
            appCoreFolder.PmObject.Add(SaveSettings_PmData(data));

            //PmWeb - Web
            appCoreFolder.PmObject.Add(Web_PmWeb());

            //PmWorkspace - Workspace
            //appCoreFolder.PmObject.Add(Workspace_PmWorkspace());

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            //Objects in Z45AppCore folder
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            UpdateStatus(90, "Serialization..");

            #region Serialization

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Serialization of PromoticClass class object
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            XmlWriterSettings xmlSet = new XmlWriterSettings();
            xmlSet.Encoding = Encoding.Unicode;
            xmlSet.Indent = true;
            xmlSet.IndentChars = "    ";

            XmlSerializer serializer = new XmlSerializer(typeof(PromoticClass));


            using (XmlWriter writer = XmlWriter.Create(filePath, xmlSet))
            {
                serializer.Serialize(writer, xmlDoc);
            }

            //add faceplates
            UpdateStatus(95, "Importing Faceplates");   //update progress status
            AddXML(@"\Promotic\Scripts\Faceplates.xml", filePath, "AlarmStripes");

            //add workspace
            UpdateStatus(95, "Importing Workspace");   //update progress status
            AddXML(@"\Promotic\Scripts\Workspace.xml", filePath, "Events");

            //add workspace
            UpdateStatus(99, "Importing Graphics");   //update progress status
            AddXML(@"\Promotic\Scripts\Graphics.xml", filePath, "AlarmStripes");

            UpdateStatus(100, "Complete..");

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            //Add faceplates with XElement.AddAfterSelf method
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            #endregion
        }

        /// <summary>
        /// Save copy of PmiLibs to selected location
        /// </summary>
        public static void DownloadPmiLibs(string filePath)
        {
            UpdateStatus(0, "Starting");
            Directory.CreateDirectory(filePath + @"\gpitems");

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);  //get output folder
            path = path.Substring(6) + @"\Promotic\PmiLibrary";

            int process = 10;

            foreach (string p in Directory.GetFiles(path))
            {
                UpdateStatus(process, "Copying..");
                process = process + 3;
                File.Copy(p, filePath + @"\gpitems\" + Path.GetFileName(p), true);
            }

            UpdateStatus(100, "Complete");
        }

        /// <summary>
        /// Save copy of Resources to selected location
        /// </summary>
        public static void DownloadResource(string filePath)
        {
            UpdateStatus(0, "Starting");

            Directory.CreateDirectory(filePath + @"\Resource");

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);  //get output folder
            path = path.Substring(6) + @"\Promotic\Resource";

            int process = 1;

            foreach (string p in Directory.GetFiles(path))
            {
                UpdateStatus(process, "Copying..");
                File.Copy(p, filePath + @"\Resource\" + Path.GetFileName(p), true);
                process++;
            }

            UpdateStatus(100, "Complete");
        }

        /// <summary>
        /// Calculate alarm stripes for single panel
        /// </summary>
        public static void CalcAlarmStripes()
        {
            List<TagList> alarmList = new List<TagList>();

            var loadedXML = OpenXml();
            //loadedXML.Load(@"Z45_Panels.xml");

            XDocument xDoc = XDocument.Parse(loadedXML.OuterXml);

            AlarmStripesData asData = new AlarmStripesData();
            asData.Panel = new List<AlarmStripePanel>();

            foreach (XElement n in xDoc.Descendants().Where(node => (string)node.Attribute("Type") == "PmPanel"))
            {
                AlarmStripePanel panel = new AlarmStripePanel();
                panel.Tag = new List<string>();
                panel.Name = n.Attribute("Name").Value;
                asData.Panel.Add(panel);

                foreach (XElement nn in n.Descendants().Where(node => (string)node.Attribute("Name") == "tagName"))
                {

                    try
                    {
                        string tagname = nn.Value;
                        string path = nn.Parent.Parent.Descendants().FirstOrDefault(node => node.Value.Contains("AnyAl") && (string)node.Attribute("Name") == "Value").Value;

                        panel.Tag.Add(path.Substring(path.IndexOf("Z45AppCore", 0) + 11).Substring(0, path.Substring(path.IndexOf("Z45AppCore", 0) + 11).IndexOf("\"")) + tagname);
                    }
                    catch { }

                }
            }

            //Build script data
            string atScript = "Generated: " + DateTime.Now.ToString("\"dd.MM.yyyy - HH:mm:ss\"") + Environment.NewLine;


            foreach (AlarmStripePanel panel in asData.Panel)
            {
                atScript += "'" + panel.Name + Environment.NewLine;
                atScript += "AlarmStripes.Methods.SetAlarm " + GetAlarmStripeLetter(panel.Name) + "false";

                foreach (string tag in panel.Tag)
                {
                    atScript += " or pMe.Pm(\"/Z45AppCore/" + tag + "/#vars/AnyAl\")";
                }

                atScript += ", false";

                foreach (string tag in panel.Tag)
                {
                    atScript += " or pMe.Pm(\"/Z45AppCore/" + tag + "/#vars/AnyAck\")";
                }

                atScript += Environment.NewLine + Environment.NewLine;
            }

            string cData = xDoc.Descendants().FirstOrDefault(node => (string)node.Attribute("Name") == "AlarmTimer")
                .Descendants().FirstOrDefault(node => (string)node.Attribute("Name") == "onTick")
                .Descendants().FirstOrDefault(node => node.Name == "Script").Value;

            atScript = cData + atScript;
                
            xDoc.Descendants().FirstOrDefault(node => (string)node.Attribute("Name") == "AlarmTimer")
                .Descendants().FirstOrDefault(node => (string)node.Attribute("Name") == "onTick")
                .Descendants().FirstOrDefault(node => node.Name == "Script").ReplaceNodes(new XCData(atScript));

            //Save xml
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "unknown.xml";
            savefile.Filter = "XML Files|*.xml";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                xDoc.Save(savefile.FileName);
            }
        }

        /// <summary>
        /// Calculate alarm stripes script of panels
        /// </summary>
        public static void CalcAlarmStripeSingle()
        {
            List<TagList> alarmList = new List<TagList>();

            var loadedXML = OpenXml();
            //loadedXML.Load(@"Z45_Panels.xml");

            XDocument xDoc = XDocument.Parse(loadedXML.OuterXml);

            AlarmStripesData asData = new AlarmStripesData();
            asData.Panel = new List<AlarmStripePanel>();

            foreach (XElement n in xDoc.Descendants().Where(node => (string)node.Attribute("Type") == "PmPanel"))
            {
                AlarmStripePanel panel = new AlarmStripePanel();
                panel.Tag = new List<string>();
                panel.Name = n.Attribute("Name").Value;
                asData.Panel.Add(panel);

                foreach (XElement nn in n.Descendants().Where(node => (string)node.Attribute("Name") == "tagName"))
                {

                    try
                    {
                        string tagname = nn.Value;
                        string path = nn.Parent.Parent.Descendants().FirstOrDefault(node => node.Value.Contains("AnyAl") && (string)node.Attribute("Name") == "Value").Value;

                        panel.Tag.Add(path.Substring(path.IndexOf("Z45AppCore", 0) + 11).Substring(0, path.Substring(path.IndexOf("Z45AppCore", 0) + 11).IndexOf("\"")) + tagname);
                    }
                    catch { }
                    
                }
            }

            //Build script data
            string atScript = "";
            

            foreach (AlarmStripePanel panel in asData.Panel)
            {
                atScript += "'" + panel.Name + Environment.NewLine;
                atScript += "AlarmStripes.Methods.SetAlarm " + GetAlarmStripeLetter(panel.Name) + "false";

                foreach (string tag in panel.Tag)
                {
                    atScript += " or pMe.Pm(\"/Z45AppCore/" + tag + "/#vars/AnyAl\")";
                }

                atScript += ", false";

                foreach (string tag in panel.Tag)
                {
                    atScript += " or pMe.Pm(\"/Z45AppCore/" + tag + "/#vars/AnyAck\")";
                }

                atScript += Environment.NewLine+ Environment.NewLine;
            }

            //Save txt
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "unknown.txt";
            savefile.Filter = "Text Files|*.txt";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, atScript);
            }
        }

        private static string GetAlarmStripeLetter(string panelName)
        {
            string stripeLetter = "";
            string stripeNumber = "";

            string c = panelName.Substring(0, 1);
            int n = 0;
            Int32.TryParse(panelName.Substring(1, 1), out n);

            if (n == 0)
            {
                stripeLetter = "\"Menu\"";

                if (c == "A")
                    stripeNumber = "1";
                if (c == "B")
                    stripeNumber = "2";
                if (c == "C")
                    stripeNumber = "3";
                if (c == "D")
                    stripeNumber = "4";
                if (c == "E")
                    stripeNumber = "5";
                if (c == "F")
                    stripeNumber = "6";
                if (c == "G")
                    stripeNumber = "7";
                if (c == "H")
                    stripeNumber = "8";
                if (c == "I")
                    stripeNumber = "9";
                if (c == "J")
                    stripeNumber = "10";
            }
                

                
            else
            {
                stripeLetter = "\""+c+"\"";
                stripeNumber = n.ToString();
            }

            return stripeLetter + ", "+stripeNumber+", ";
        }

        #endregion

        #region submethods PRIVATE

        private static XmlDocument OpenXml()
        {
            using (var openFileDialogXML = new OpenFileDialog())
            {
                openFileDialogXML.InitialDirectory = "C:\\";
                openFileDialogXML.Filter = "XML Files|*.xml";
                openFileDialogXML.RestoreDirectory = true;

                if (openFileDialogXML.ShowDialog() != DialogResult.OK)
                {
                    return CreateEmptyXmlDocument();
                }

                //  The stream will hold the results of opening the XML
                using (var myStream = openFileDialogXML.OpenFile())
                {
                    try
                    {
                        //  Successfully return the XML
                        XmlDocument parsedMyStream = new XmlDocument();
                        parsedMyStream.Load(myStream);
                        return parsedMyStream;
                    }
                    catch (XmlException ex)
                    {
                        MessageBox.Show("The XML could not be read. " + ex);
                        return CreateEmptyXmlDocument();
                    }
                }
            }
        }

        private static XmlDocument CreateEmptyXmlDocument()
        {
            //  Return an empty XmlDocument if the open file window was closed
            XmlDocument emptyMyStream = new XmlDocument();
            return emptyMyStream;
        }

        /// <summary>
        /// Creates PmData object AlarmStripes
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass AlarmStripes_PmData()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "AlarmStripes";
            obj.Type = "PmData";

            //properties
            obj.Prop = new List<PromoticClass.PropClass>();
            obj.Prop.Add(SetPropType1("ReferenceType", "2"));
            obj.Prop.Add(SetPropType1("ReferenceName", "AlarmStripes"));

            obj.Methods = new PromoticClass.MethodsClass();
            obj.Methods.Method = new List<PromoticClass.MethodClass>();

            obj.Methods.Method.Add(SetMethod("SetAlarm", "Group,Display,Alarm,ALAck", Script(@"Scripts\CommonScripts.xml", "AlarmStripesMethod")));

            //Data
            PromoticClass.ListClass vars = new PromoticClass.ListClass();
            vars.Name = "Vars";
            vars.Props = new List<PromoticClass.PropsClass>();

            obj.List = new List<PromoticClass.ListClass>();
            obj.List.Add(vars);

            vars.Props.Add(DataProps("Menu", DataType.String, null, "AlarmStripe memory for 1st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("A", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("B", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("C", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("D", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("E", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("F", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("G", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("H", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("I", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));
            vars.Props.Add(DataProps("J", DataType.String, null, "AlarmStripe memory for 2st level tabs - starting from 1", null));

            return obj;
        }

        private static PromoticClass.PmObjectClass DefaultCommObj()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "Comm";
            obj.Type = "PmComm";

            obj.Prop = new List<PromoticClass.PropClass>();
            obj.Prop.Add(SetPropType1("ProtocolName", "S7,ethernet"));
            obj.Prop.Add(SetPropType1("CommPar", "phys=ethernet;addr=192.168.1.160;port=102;ethtype=tcp;timSock=2000;infoMonitRows=1000"));
            obj.Prop.Add(SetPropType1("ProtocolPar", "model=1;maxpdulen=960;rack=0;slot=2;conntype=2;"));

            obj.PmObject = new List<PromoticClass.PmObjectClass>();

            PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
            obj.PmObject.Add(subObj);
            subObj.Name = "Data";
            subObj.Type = "PmCommData";

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Z45_Panels_PmFolder()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "Z45_Panels";
            obj.Type = "PmFolder";
            obj.PmObject = new List<PromoticClass.PmObjectClass>();

            //PmTimer - AlarmTimer
            obj.PmObject.Add(PmTimerObj("AlarmTimer", Script(@"\Scripts\CommonScripts.xml", "AlarmTimer")));

            //A0
            PromoticClass.PmObjectClass A0screen = new PromoticClass.PmObjectClass();
            A0screen.Name = "A0_nameExample";
            A0screen.Type = "PmPanel";
            obj.PmObject.Add(A0screen);

            A0screen.Prop = new List<PromoticClass.PropClass>();
            A0screen.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));
            A0screen.Prop.Add(SetPropType1("Title", "Panel 1"));
            A0screen.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            A0screen.Prop.Add(SetPropType1("View2AppLevel", "client"));

            A0screen.Props = new List<PromoticClass.PropsClass>();
            A0screen.Props.Add(WebProps(false));

            A0screen.GPanel = new PromoticClass.GPanelClass();
            A0screen.GPanel.Prop = new List<PromoticClass.PropClass>();
            A0screen.GPanel.Prop.Add(SetPropType1("Dx", "1920"));
            A0screen.GPanel.Prop.Add(SetPropType1("Dy", "980"));

            A0screen.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass A0localProps = new PromoticClass.PropsClass();
            A0localProps.Name = "LocalProps";
            A0screen.Props.Add(A0localProps);

            A0localProps.Prop = new List<PromoticClass.PropClass>();
            A0localProps.Prop.Add(SetPropType1("BackgroundColor", "#c0c0c0"));
            A0localProps.Prop.Add(SetPropType1("FocusColor", "#000000"));

            //web server
            A0screen.Props.Add(WebProps(true, "a0"));

            //A1
            PromoticClass.PmObjectClass A1screen = new PromoticClass.PmObjectClass();
            A1screen.Name = "A1_nameExample";
            A1screen.Type = "PmPanel";
            obj.PmObject.Add(A1screen);

            A1screen.Prop = new List<PromoticClass.PropClass>();
            A1screen.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));
            A1screen.Prop.Add(SetPropType1("Title", "Podpanel 1"));
            A1screen.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            A1screen.Prop.Add(SetPropType1("View2AppLevel", "client"));

            A1screen.Props = new List<PromoticClass.PropsClass>();
            A1screen.Props.Add(WebProps(false));

            A1screen.GPanel = new PromoticClass.GPanelClass();
            A1screen.GPanel.Prop = new List<PromoticClass.PropClass>();
            A1screen.GPanel.Prop.Add(SetPropType1("Dx", "1920"));
            A1screen.GPanel.Prop.Add(SetPropType1("Dy", "980"));

            A1screen.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass A1localProps = new PromoticClass.PropsClass();
            A1localProps.Name = "LocalProps";
            A1screen.Props.Add(A1localProps);

            A1localProps.Prop = new List<PromoticClass.PropClass>();
            A1localProps.Prop.Add(SetPropType1("BackgroundColor", "#c0c0c0"));
            A1localProps.Prop.Add(SetPropType1("FocusColor", "#000000"));

            //web server
            A1screen.Props.Add(WebProps(true, "a1"));

            //B0
            PromoticClass.PmObjectClass B0screen = new PromoticClass.PmObjectClass();
            B0screen.Name = "B0_nameExample";
            B0screen.Type = "PmPanel";
            obj.PmObject.Add(B0screen);

            B0screen.Prop = new List<PromoticClass.PropClass>();
            B0screen.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));
            B0screen.Prop.Add(SetPropType1("Title", "Panel 2"));
            B0screen.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            B0screen.Prop.Add(SetPropType1("View2AppLevel", "client"));

            B0screen.Props = new List<PromoticClass.PropsClass>();
            B0screen.Props.Add(WebProps(false));

            B0screen.GPanel = new PromoticClass.GPanelClass();
            B0screen.GPanel.Prop = new List<PromoticClass.PropClass>();
            B0screen.GPanel.Prop.Add(SetPropType1("Dx", "1920"));
            B0screen.GPanel.Prop.Add(SetPropType1("Dy", "980"));

            B0screen.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass B0localProps = new PromoticClass.PropsClass();
            B0localProps.Name = "LocalProps";
            B0screen.Props.Add(B0localProps);

            B0localProps.Prop = new List<PromoticClass.PropClass>();
            B0localProps.Prop.Add(SetPropType1("BackgroundColor", "#c0c0c0"));
            B0localProps.Prop.Add(SetPropType1("FocusColor", "#000000"));

            //web server
            B0screen.Props.Add(WebProps(true, "b0"));

            return obj;
        }

        /// <summary>
        /// Creates PmData object Colors
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass Colors_PmData()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "Colors";
            obj.Type = "PmData";

            //Data
            PromoticClass.ListClass vars = new PromoticClass.ListClass();
            vars.Name = "Vars";
            vars.Props = new List<PromoticClass.PropsClass>();

            obj.List = new List<PromoticClass.ListClass>();
            obj.List.Add(vars);

            vars.Props.Add(DataProps("alUnackColor", DataType.String, "#ff6868", "Color of unacknowledged inactive alarm", null));
            vars.Props.Add(DataProps("alActColor", DataType.String, "#ff0000", "Color of active alarm", null));
            vars.Props.Add(DataProps("alNormalColor", DataType.String, "#000000", "Color of acknowledged inactive alarm", null));
            vars.Props.Add(DataProps("alInhColor", DataType.String, "#3068ff", "Color of inhibit alarm", null));
            vars.Props.Add(DataProps("trendColorSP", DataType.String, "#3068ff", "Trend color of setpoint", null));
            vars.Props.Add(DataProps("trendColorPV", DataType.String, "#ff6800", "Trend color of process value", null));
            vars.Props.Add(DataProps("trendColorActFB", DataType.String, "#00ac00", "Trend color of actual feedback", null));


            return obj;
        }


        private static PromoticClass.PmObjectClass Z45_Graphics_PmFolder()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.PmObject = new List<PromoticClass.PmObjectClass>();
            obj.Name = "Z45_Graphics";
            obj.Type = "PmFolder";

            //_Note
            PromoticClass.PmObjectClass objNote = new PromoticClass.PmObjectClass();
            objNote.Name = "_Note";
            objNote.Type = "PmPanel";
            obj.PmObject.Add(objNote);

            objNote.Prop = new List<PromoticClass.PropClass>();

            objNote.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));
            objNote.Prop.Add(SetPropType1("Title", "Note"));
            objNote.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            objNote.Prop.Add(SetPropType1("View2AppLevel", "client"));
            objNote.Prop.Add(SetPropType1("MultiView", "1"));

            objNote.Props = new List<PromoticClass.PropsClass>();
            objNote.Props.Add(WebProps(false));

            objNote.GPanel = new PromoticClass.GPanelClass();

            objNote.GPanel.Prop = new List<PromoticClass.PropClass>();

            objNote.GPanel.Prop.Add(SetPropType1("Dx", "640"));
            objNote.GPanel.Prop.Add(SetPropType1("Dy", "480"));

            objNote.GPanel.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass noteLocalProps = new PromoticClass.PropsClass();
            noteLocalProps.Name = "LocalProps";
            objNote.GPanel.Props.Add(noteLocalProps);

            noteLocalProps.Prop = new List<PromoticClass.PropClass>();
            noteLocalProps.Prop.Add(SetPropType1("BackgroundColor", "#c0c0c0"));
            noteLocalProps.Prop.Add(SetPropType1("FocusColor", "#000000"));

            PromoticClass.PropsClass noteParams = new PromoticClass.PropsClass();
            noteParams.Name = "Params";
            objNote.GPanel.Props.Add(noteParams);

            noteParams.Prop = new List<PromoticClass.PropClass>();
            noteParams.Prop.Add(SetPropType2("tagPath", "par", "tag"));

            objNote.GPanel.GItem = new List<PromoticClass.GItemClass>();

            //Edit0
            PromoticClass.GItemClass giEdit0 = new PromoticClass.GItemClass();
            giEdit0.Name = "Edit0";
            giEdit0.Type = "PmiWEdit";
            giEdit0.Prop = new List<PromoticClass.PropClass>();
            objNote.GPanel.GItem.Add(giEdit0);

            giEdit0.Prop.Add(SetPropType1("X", "10"));
            giEdit0.Prop.Add(SetPropType1("Y", "10"));
            giEdit0.Prop.Add(SetPropType1("Dx", "620"));
            giEdit0.Prop.Add(SetPropType1("Dy", "420"));

            giEdit0.Props = new List<PromoticClass.PropsClass>();
            PromoticClass.PropsClass giEdit0_localprops = new PromoticClass.PropsClass();
            giEdit0_localprops.Name = "LocalProps";
            giEdit0.Props.Add(giEdit0_localprops);

            giEdit0_localprops.Prop = new List<PromoticClass.PropClass>();
            giEdit0_localprops.Prop.Add(SetPropType3("Note", "string", "string", null, "PP", Script(@"Scripts\GraphicsScripts.xml", "NotePath")));

            giEdit0.Prop.Add(SetPropType1("ColorItem", "#ffffff"));
            giEdit0.Prop.Add(SetPropType1("FontText", "PmMiddle"));

            giEdit0.Prop.Add(SetPropType3("Value", null, null, "Text", "GP", Script(@"Scripts\GraphicsScripts.xml", "NotePath2")));

            giEdit0.Prop.Add(SetPropType1("AlignHor", "0"));
            giEdit0.Prop.Add(SetPropType1("AttrEdit", "16421"));
            giEdit0.Prop.Add(SetPropType1("AttrEditEx", "7"));
            giEdit0.Prop.Add(SetPropType1("WndBorderType", "3"));

            giEdit0.Event = new List<PromoticClass.EventClass>();
            giEdit0.Event.Add(SetEvent("onDataEditAccept", "Pm", Script(@"Scripts\GraphicsScripts.xml", "NoteOnDataEditAcceptEvent")));

            //Butt
            PromoticClass.GItemClass giButt = new PromoticClass.GItemClass();
            giButt.Name = "Butt";
            giButt.Type = "PmiButton";
            giButt.Prop = new List<PromoticClass.PropClass>();
            objNote.GPanel.GItem.Add(giButt);

            giButt.Prop.Add(SetPropType1("X", "540"));
            giButt.Prop.Add(SetPropType1("Y", "440"));
            giButt.Prop.Add(SetPropType1("Dx", "90"));
            giButt.Prop.Add(SetPropType1("Dy", "30"));

            giButt.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            giButt.Prop.Add(SetPropType1("BorderWidth", "2"));
            giButt.Prop.Add(SetPropType1("BorderContrast", "60"));
            giButt.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giButt.Prop.Add(SetPropType1("GradientContrast", "15"));
            giButt.Prop.Add(SetPropType1("GradientDirection", "0"));
            giButt.Prop.Add(SetPropType1("BnAttributes", "4"));

            giButt.Props = new List<PromoticClass.PropsClass>();
            PromoticClass.PropsClass giButt_textProps = new PromoticClass.PropsClass();
            giButt_textProps.Name = "Text";
            giButt.Props.Add(giButt_textProps);

            giButt_textProps.Prop = new List<PromoticClass.PropClass>();
            giButt_textProps.Prop.Add(SetPropType1("StringText", "Save"));
            giButt_textProps.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            giButt_textProps.Prop.Add(SetPropType1("AlignHor", "1"));
            giButt_textProps.Prop.Add(SetPropType1("AlignVer", "1"));

            giButt.Prop.Add(SetPropType1("ImageSrv", null));
            giButt.Prop.Add(SetPropType1("ImageOper", "0"));

            giButt.Event = new List<PromoticClass.EventClass>();
            giButt.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\GraphicsScripts.xml", "NoteOnButtonUpEvent_Butt")));

            //Butt0
            PromoticClass.GItemClass giButt0 = new PromoticClass.GItemClass();
            giButt0.Name = "Butt0";
            giButt0.Type = "PmiButton";
            giButt0.Prop = new List<PromoticClass.PropClass>();
            objNote.GPanel.GItem.Add(giButt0);

            giButt0.Prop.Add(SetPropType1("X", "440"));
            giButt0.Prop.Add(SetPropType1("Y", "440"));
            giButt0.Prop.Add(SetPropType1("Dx", "90"));
            giButt0.Prop.Add(SetPropType1("Dy", "30"));

            giButt0.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giButt0_localProps = new PromoticClass.PropsClass();
            giButt0_localProps.Name = "LocalProps";
            giButt0.Props.Add(giButt0_localProps);

            giButt0_localProps.Prop = new List<PromoticClass.PropClass>();
            giButt0_localProps.Prop.Add(SetPropType3("Note", "string", "string", null, "GP", "/Edit0;Value"));

            giButt0.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            giButt0.Prop.Add(SetPropType1("BorderWidth", "2"));
            giButt0.Prop.Add(SetPropType1("BorderContrast", "60"));
            giButt0.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giButt0.Prop.Add(SetPropType1("GradientContrast", "15"));
            giButt0.Prop.Add(SetPropType1("GradientDirection", "0"));
            giButt0.Prop.Add(SetPropType1("BnAttributes", "4"));


            PromoticClass.PropsClass giButt0_textProps = new PromoticClass.PropsClass();
            giButt0_textProps.Name = "Text";
            giButt0.Props.Add(giButt0_textProps);

            giButt0_textProps.Prop = new List<PromoticClass.PropClass>();
            giButt0_textProps.Prop.Add(SetPropType1("StringText", "Clear"));
            giButt0_textProps.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            giButt0_textProps.Prop.Add(SetPropType1("AlignHor", "1"));
            giButt0_textProps.Prop.Add(SetPropType1("AlignVer", "1"));

            giButt0.Prop.Add(SetPropType1("ImageSrv", null));
            giButt0.Prop.Add(SetPropType1("ImageOper", "0"));

            giButt0.Event = new List<PromoticClass.EventClass>();
            giButt0.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\GraphicsScripts.xml", "NoteOnButtonUpEvent_Butt0")));


            //_SP
            PromoticClass.PmObjectClass objSP = new PromoticClass.PmObjectClass();
            objSP.Name = "_SP";
            objSP.Type = "PmPanel";
            obj.PmObject.Add(objSP);

            objSP.Prop = new List<PromoticClass.PropClass>();

            objSP.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));
            objSP.Prop.Add(SetPropType1("Title", "SP"));
            objSP.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            objSP.Prop.Add(SetPropType1("View2AppLevel", "client"));
            objSP.Prop.Add(SetPropType1("MultiView", "1"));

            objSP.Props = new List<PromoticClass.PropsClass>();
            objSP.Props.Add(WebProps(false));

            objSP.GPanel = new PromoticClass.GPanelClass();

            objSP.GPanel.Prop = new List<PromoticClass.PropClass>();

            objSP.GPanel.Prop.Add(SetPropType1("Dx", "120"));
            objSP.GPanel.Prop.Add(SetPropType1("Dy", "70"));

            objSP.GPanel.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass spLocalProps = new PromoticClass.PropsClass();
            spLocalProps.Name = "LocalProps";
            objSP.GPanel.Props.Add(spLocalProps);

            spLocalProps.Prop = new List<PromoticClass.PropClass>();
            spLocalProps.Prop.Add(SetPropType1("BackgroundColor", "#c0c0c0"));
            spLocalProps.Prop.Add(SetPropType1("FocusColor", "#000000"));

            PromoticClass.PropsClass spParams = new PromoticClass.PropsClass();
            spParams.Name = "Params";
            objSP.GPanel.Props.Add(spParams);

            spParams.Prop = new List<PromoticClass.PropClass>();
            spParams.Prop.Add(SetPropType2("tagPath", "par", "tag"));

            objSP.GPanel.GItem = new List<PromoticClass.GItemClass>();

            //Edit
            PromoticClass.GItemClass giEdit = new PromoticClass.GItemClass();
            giEdit.Name = "Edit";
            giEdit.Type = "PmiWEdit";
            objSP.GPanel.GItem.Add(giEdit);

            giEdit.Prop = new List<PromoticClass.PropClass>();
            giEdit.Prop.Add(SetPropType1("X", "10"));
            giEdit.Prop.Add(SetPropType1("Y", "20"));
            giEdit.Prop.Add(SetPropType1("Dx", "100"));
            giEdit.Prop.Add(SetPropType1("Dy", "30"));

            giEdit.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giEdit_localProps = new PromoticClass.PropsClass();
            giEdit_localProps.Name = "LocalProps";
            giEdit.Props.Add(giEdit_localProps);

            giEdit_localProps.Prop = new List<PromoticClass.PropClass>();
            giEdit_localProps.Prop.Add(SetPropType3("HMI_Value", "double", "double", "0", "PP", Script(@"Scripts\GraphicsScripts.xml", "SpPropBind")));

            giEdit.Prop.Add(SetPropType1("ColorItem", "#ffffff"));
            giEdit.Prop.Add(SetPropType1("FontText", "PmMiddleBold"));
            giEdit.Prop.Add(SetPropType3("Value", null, null, "99.999", "GP", Script(@"Scripts\GraphicsScripts.xml", "SpPropBind2")));
            giEdit.Prop.Add(SetPropType1("AlignHor", "1"));
            giEdit.Prop.Add(SetPropType1("AttrEdit", "4"));
            giEdit.Prop.Add(SetPropType1("AttrEditEx", "7"));
            giEdit.Prop.Add(SetPropType1("WndBorderType", "3"));

            giEdit.Event = new List<PromoticClass.EventClass>();
            giEdit.Event.Add(SetEvent("onDataEditAccept", "Pm", Script(@"Scripts\GraphicsScripts.xml", "SpOnDataEditAccept")));

            return obj;
        }

        /// <summary>
        /// Creates PmData object CustomData
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass CustomData_PmData()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "CustomData";
            obj.Type = "PmData";

            //vars
            PromoticClass.ListClass vars = new PromoticClass.ListClass();
            vars.Name = "Vars";
            vars.Props = new List<PromoticClass.PropsClass>();

            obj.List = new List<PromoticClass.ListClass>();
            obj.List.Add(vars);

            //Data
            vars.Props.Add(DataProps("trendLineWidth", DataType.Double, "3", "Trend line width", null));

            return obj;
        }

        /// <summary>
        /// Creates PmData object SaveSettings
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass SaveSettings_PmData(BDMdataClass data)
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "SaveSettings";
            obj.Type = "PmData";

            //Events
            obj.Events = new PromoticClass.EventsClass();
            obj.Events.Name = "PmEvents";
            obj.Events.Event = new List<PromoticClass.EventClass>();

            PromoticClass.EventClass onItemAfterWrite = new PromoticClass.EventClass();
            obj.Events.Event.Add(onItemAfterWrite);
            onItemAfterWrite.Name = "onItemAfterWrite";
            onItemAfterWrite.Type = "Pm";
            onItemAfterWrite.Script.Content = SaveSettingsScript(data);



            //vars
            PromoticClass.ListClass vars = new PromoticClass.ListClass();
            vars.Name = "Vars";
            vars.Props = new List<PromoticClass.PropsClass>();

            obj.List = new List<PromoticClass.ListClass>();
            obj.List.Add(vars);

            //Data
            vars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
            vars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Save values to config file", null, "1", false));
            
            return obj;
        }

        /// <summary>
        /// Build Script to onItemAfterWrite event in SaveSettings object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string SaveSettingsScript(BDMdataClass data)
        {
            string str_save = '\"' + "Save" + '\"';
            string str_load = '\"' + "Load" + '\"';


            string cont = "";

            //Save section
            cont += "if pEvent.Item.Name = " + str_save + "and pMe.Item(" + str_save + ").Value then";
            cont += Environment.NewLine;
            cont += "'set all save vars in all objects";
            cont += Environment.NewLine;

            foreach (BasicObject item in data.Objects)
            {
                if (item.DataType == Resource.AinType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/AinData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.AoutType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/AoutData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.DinType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/DinData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.DoutType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/DoutData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.GrpType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/GrpData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.PIDType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/PIDCtrlData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.PreselType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/PreselData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.AnalogPosType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/AnalogPosCtrlData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.OnOffType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/OnOffCtrlData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.OnOff2DType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/OnOffCtrlData_2D/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.OnOffVSDType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/OnOffCtrlData_VSD/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.PIDStepType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/PIDStepCtrlData/" + item.TagName + "/#vars/Save" + '\"' + ").Value = true";
                }

                cont += Environment.NewLine;
            }
            cont += "pMe.Item(" + str_save + ").Value = false";
            cont += Environment.NewLine;
            cont += "end if";

            //Load section
            cont += Environment.NewLine;
            cont += Environment.NewLine;
            cont += "if pEvent.Item.Name = " + str_load + "and pMe.Item(" + str_load + ").Value then";
            cont += Environment.NewLine;
            cont += "'set all load vars in all objects";
            cont += Environment.NewLine;

            foreach (BasicObject item in data.Objects)
            {
                if (item.DataType == Resource.AinType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/AinData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.AoutType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/AoutData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.DinType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/DinData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.DoutType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/DoutData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.GrpType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/GrpData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.PIDType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/PIDCtrlData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.PreselType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/PreselData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.AnalogPosType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/AnalogPosCtrlData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.OnOffType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/OnOffCtrlData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.OnOff2DType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/OnOffCtrlData_2D/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.OnOffVSDType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/OnOffCtrlData_VSD/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                if (item.DataType == Resource.PIDStepType)
                {
                    cont += "pMe.Pm(" + '\"' + "/Z45AppCore/PIDStepCtrlData/" + item.TagName + "/#vars/Load" + '\"' + ").Value = true";
                }

                cont += Environment.NewLine;
            }
            cont += "pMe.Item(" + str_load + ").Value = false";
            cont += Environment.NewLine;
            cont += "end if";

            return cont;
        }

        /// <summary>
        /// Creates PmWeb object Web
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass Web_PmWeb()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "Web";
            obj.Type = "PmWeb";

            //Web
            obj.Prop = new List<PromoticClass.PropClass>();

            obj.Prop.Add(SetPropType1("WebDefaultFile", "workspace/main.htm"));
            obj.Prop.Add(SetPropType1("StrictNetLogon", "1"));

            //WebInfo object
            obj.PmObject = new List<PromoticClass.PmObjectClass>();
            obj.PmObject.Add(Info_PmWebInfo());

            return obj;
        }

        /// <summary>
        /// Creates PmWebInfo object Info
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass Info_PmWebInfo()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "Info";
            obj.Type = "PmWebInfo";

            //WebInfo
            obj.Prop = new List<PromoticClass.PropClass>();
            obj.Prop.Add(SetPropType1("RefreshTime", "1"));

            return obj;
        }

        /// <summary>
        /// Add xml code to final xml export
        /// </summary>
        /// <param name="spath">Path to xml part to be included</param>
        /// <param name="filePath">Path where export xml is saved</param>
        /// <param name="addAfter">Element after which new part is added</param>
        private static void AddXML(string spath, string filePath, string addAfter)
        {
            //load faceplate node from Scripts
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6) + spath;

            XmlDocument xmlDocFP = new XmlDocument();
            xmlDocFP.Load(path);

            //XmlDocument -> XDocument
            XDocument xDocFP = XDocument.Load(new XmlNodeReader(xmlDocFP));

            //get root node
            XElement xDocFPRoot = xDocFP.Root;

            //load final xml file
            XmlDocument xmlDocFinal = new XmlDocument();
            xmlDocFinal.Load(filePath);

            //XmlDocument -> XDocument
            XDocument xDocFinal = XDocument.Load(new XmlNodeReader(xmlDocFinal));

            //get child to copy after faceplates
            XElement xDocFinalNode = xDocFinal.Root.Descendants("PmObject").First(el => (string)el.Attribute("Name") == addAfter);

            //append node
            xDocFinalNode.AddAfterSelf(xDocFPRoot);

            //XDocument -> XmlDocument
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocFinal.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            //save xml
            xmlDocument.Save(filePath);

        }


        /// <summary>
        /// Creates PmWorkspace object Workspace
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass Workspace_PmWorkspace()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Name = "Workspace";
            obj.Type = "PmWorkspace";

            //Hlavni
            obj.Prop = new List<PromoticClass.PropClass>();

            obj.Prop.Add(SetPropType1("MainWindow", "1"));
            obj.Prop.Add(SetPropType1("Caption", "0"));

            //Ramce
            obj.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass props1 = new PromoticClass.PropsClass();
            props1.Name = "MainFrame";
            props1.List = new List<PromoticClass.ListClass>();
            obj.Props.Add(props1);

            PromoticClass.ListClass list1 = new PromoticClass.ListClass();
            list1.Name = "Frames";
            list1.Props = new List<PromoticClass.PropsClass>();
            props1.List.Add(list1);

            PromoticClass.PropsClass props2 = new PromoticClass.PropsClass(); // Toolbar
            props2.Name = "toolbar";
            props2.Prop = new List<PromoticClass.PropClass>();
            list1.Props.Add(props2);

            props2.Prop.Add(SetPropType1("Size", "20px"));
            props2.Prop.Add(SetPropType1("Src", "/Z45AppCore/Workspace/Toolbar"));

            PromoticClass.PropsClass props3 = new PromoticClass.PropsClass(); // Menu
            props3.Name = "menu";
            props3.Prop = new List<PromoticClass.PropClass>();
            list1.Props.Add(props3);

            props3.Prop.Add(SetPropType1("Size", "80px"));
            props3.Prop.Add(SetPropType1("Src", "/Z45AppCore/Workspace/Menu"));

            PromoticClass.PropsClass props4 = new PromoticClass.PropsClass(); // Main
            props4.Name = "main";
            props4.Prop = new List<PromoticClass.PropClass>();
            list1.Props.Add(props4);

            props4.Prop.Add(SetPropType1("Size", "*"));

            //Web server
            obj.Props.Add(WebProps(true));


            //Podobjekty
            //
            obj.PmObject = new List<PromoticClass.PmObjectClass>();

            //Podobjekt Menu PmPanel
            PromoticClass.PmObjectClass menuObj = new PromoticClass.PmObjectClass();
            menuObj.Name = "Menu";
            menuObj.Type = "PmPanel";
            obj.PmObject.Add(menuObj);

            menuObj.Prop = new List<PromoticClass.PropClass>();
            menuObj.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            menuObj.Prop.Add(SetPropType1("View2AppLevel", "full"));

            menuObj.Props = new List<PromoticClass.PropsClass>();
            menuObj.Props.Add(WebProps(false));

            //
            menuObj.GPanel = new PromoticClass.GPanelClass();
            menuObj.GPanel.Prop = new List<PromoticClass.PropClass>();

            menuObj.GPanel.Prop.Add(SetPropType1("Dx", "1920"));
            menuObj.GPanel.Prop.Add(SetPropType1("Dy", "80"));

            //
            menuObj.GPanel.GItem = new List<PromoticClass.GItemClass>();

            PromoticClass.GItemClass gitem = new PromoticClass.GItemClass();
            gitem.Name = "menuBar";
            gitem.Type = "PmiCanvas";
            menuObj.GPanel.GItem.Add(gitem);

            gitem.Prop = new List<PromoticClass.PropClass>();

            gitem.Prop.Add(SetPropType1("Dx", "1920"));
            gitem.Prop.Add(SetPropType1("Dy", "80"));
            gitem.Prop.Add(SetPropType1("FocusType", "0"));

            gitem.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass localProps = new PromoticClass.PropsClass();
            localProps.Name = "LocalProps";
            gitem.Props.Add(localProps);

            //
            localProps.Prop = new List<PromoticClass.PropClass>();

            localProps.Prop.Add(SetPropType2("titles", "string"));
            localProps.Prop.Add(SetPropType2("pathes", "string"));
            localProps.Prop.Add(SetPropType2("names", "string"));
            localProps.Prop.Add(SetPropType2("subTitles", "string"));
            localProps.Prop.Add(SetPropType2("subPathes", "string"));
            localProps.Prop.Add(SetPropType2("titlesWidth", "double", "0"));
            localProps.Prop.Add(SetPropType2("subTitlesWidth", "double", "0"));
            localProps.Prop.Add(SetPropType2("selMain", "integer", "1000"));
            localProps.Prop.Add(SetPropType2("selSub", "integer", "1000"));
            localProps.Prop.Add(SetPropType2("blink", "bool", "0"));
            localProps.Prop.Add(SetPropType2("DefaultPanel", "integer", "0"));

            localProps.Prop.Add(SetPropType3("Stripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/Menu;Value"));

            localProps.Prop.Add(SetPropType3("AStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/A;Value"));
            localProps.Prop.Add(SetPropType3("BStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/B;Value"));
            localProps.Prop.Add(SetPropType3("CStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/C;Value"));
            localProps.Prop.Add(SetPropType3("DStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/D;Value"));
            localProps.Prop.Add(SetPropType3("EStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/E;Value"));
            localProps.Prop.Add(SetPropType3("FStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/F;Value"));
            localProps.Prop.Add(SetPropType3("GStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/G;Value"));
            localProps.Prop.Add(SetPropType3("HStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/H;Value"));
            localProps.Prop.Add(SetPropType3("IStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/I;Value"));
            localProps.Prop.Add(SetPropType3("JStripes", "string", "string", null, "PP", "/Z45AppCore/AlarmStripes/#vars/J;Value"));

            localProps.Prop.Add(SetPropType2("Alphabet", "string", "A;B;C;D;E;F;G;H;I;J;"));

            //events
            gitem.Event = new List<PromoticClass.EventClass>();

            gitem.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "MenuOnStartEvent")));
            gitem.Event.Add(SetEvent("onRefresh", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "MenuOnRefreshEvent")));
            gitem.Event.Add(SetEvent("onMousePress", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "MenuOnMousePressEvent")));

            //methods
            gitem.Methods = new List<PromoticClass.MethodsClass>();

            PromoticClass.MethodsClass methods = new PromoticClass.MethodsClass();
            gitem.Methods.Add(methods);
            methods.Method = new List<PromoticClass.MethodClass>();
            methods.Method.Add(SetMethod("Init", "", Script(@"Scripts\WorkspaceScripts.xml", "MenuInitMethod")));

            //methods - canvas
            PromoticClass.MethodsClass canvasmethods = new PromoticClass.MethodsClass();
            gitem.Methods.Add(canvasmethods);
            canvasmethods.Method = new List<PromoticClass.MethodClass>();
            canvasmethods.Name = "Canvas";

            canvasmethods.Method.Add(SetMethod("onDraw", "", Script(@"Scripts\WorkspaceScripts.xml", "MenuOnDrawMethod")));

            //Podobjekt Toolbar PmPanel
            PromoticClass.PmObjectClass toolbarObj = new PromoticClass.PmObjectClass();
            toolbarObj.Name = "Toolbar";
            toolbarObj.Type = "PmPanel";
            obj.PmObject.Add(toolbarObj);

            toolbarObj.Prop = new List<PromoticClass.PropClass>();
            toolbarObj.Prop.Add(SetPropType1("ScriptEngine", "javascript"));
            toolbarObj.Prop.Add(SetPropType1("View2AppLevel", "full"));

            toolbarObj.Props = new List<PromoticClass.PropsClass>();
            toolbarObj.Props.Add(WebProps(false));

            //
            toolbarObj.GPanel = new PromoticClass.GPanelClass();
            toolbarObj.GPanel.Prop = new List<PromoticClass.PropClass>();

            toolbarObj.GPanel.Prop.Add(SetPropType1("Dx", "1920"));
            toolbarObj.GPanel.Prop.Add(SetPropType1("Dy", "20"));

            //
            toolbarObj.GPanel.Props = new List<PromoticClass.PropsClass>();
            PromoticClass.PropsClass tbObjLocProps = new PromoticClass.PropsClass();
            tbObjLocProps.Name = "LocalProps";
            toolbarObj.GPanel.Props.Add(tbObjLocProps);

            tbObjLocProps.Prop = new List<PromoticClass.PropClass>();
            tbObjLocProps.Prop.Add(SetPropType1("BackgroundColor", "#c0c0c0"));
            tbObjLocProps.Prop.Add(SetPropType1("FocusColor", "#000000"));

            //
            toolbarObj.GPanel.Event = new List<PromoticClass.EventClass>();

            toolbarObj.GPanel.Event.Add(SetEvent("onViewerResize", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnViewerResizeEvent")));

            toolbarObj.GPanel.GItem = new List<PromoticClass.GItemClass>();

            //button 2
            PromoticClass.GItemClass giButton2 = new PromoticClass.GItemClass();
            giButton2.Name = "button2";
            giButton2.Type = "PmiButton";
            toolbarObj.GPanel.GItem.Add(giButton2);

            giButton2.Prop = new List<PromoticClass.PropClass>();

            giButton2.Prop.Add(SetPropType1("X", "0"));
            giButton2.Prop.Add(SetPropType1("Y", "0"));
            giButton2.Prop.Add(SetPropType1("Dx", "20"));
            giButton2.Prop.Add(SetPropType1("Dy", "20"));
            giButton2.Prop.Add(SetPropType1("FocusType", "0"));
            giButton2.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            giButton2.Prop.Add(SetPropType1("BorderWidth", "1"));
            giButton2.Prop.Add(SetPropType1("BorderContrast", "60"));
            giButton2.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giButton2.Prop.Add(SetPropType1("GradientContrast", "15"));
            giButton2.Prop.Add(SetPropType1("GradientDirection", "0"));
            giButton2.Prop.Add(SetPropType1("BnAttributes", "12"));

            giButton2.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giButton2Props = new PromoticClass.PropsClass();
            giButton2Props.Name = "Text";
            giButton2.Props.Add(giButton2Props);

            giButton2Props.Prop = new List<PromoticClass.PropClass>();
            giButton2Props.Prop.Add(SetPropType1("StringText"));
            giButton2Props.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            giButton2Props.Prop.Add(SetPropType1("AlignHor", "1"));
            giButton2Props.Prop.Add(SetPropType1("AlignVer", "1"));

            giButton2.Prop.Add(SetPropType1("ImageSrc", "#pmres:img/IconAlarms1.svg"));
            giButton2.Prop.Add(SetPropType1("ImageOper", "4"));

            //
            giButton2.Event = new List<PromoticClass.EventClass>();

            giButton2.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnButtonUpEvent_button2")));

            //button 3
            PromoticClass.GItemClass giButton3 = new PromoticClass.GItemClass();
            giButton3.Name = "button3";
            giButton3.Type = "PmiButton";
            toolbarObj.GPanel.GItem.Add(giButton3);

            giButton3.Prop = new List<PromoticClass.PropClass>();

            giButton3.Prop.Add(SetPropType1("X", "20"));
            giButton3.Prop.Add(SetPropType1("Y", "0"));
            giButton3.Prop.Add(SetPropType1("Dx", "20"));
            giButton3.Prop.Add(SetPropType1("Dy", "20"));
            giButton3.Prop.Add(SetPropType1("FocusType", "0"));
            giButton3.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            giButton3.Prop.Add(SetPropType1("BorderWidth", "1"));
            giButton3.Prop.Add(SetPropType1("BorderContrast", "60"));
            giButton3.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giButton3.Prop.Add(SetPropType1("GradientContrast", "15"));
            giButton3.Prop.Add(SetPropType1("GradientDirection", "0"));
            giButton3.Prop.Add(SetPropType1("BnAttributes", "12"));

            giButton3.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giButton3Props = new PromoticClass.PropsClass();
            giButton3Props.Name = "Text";
            giButton3.Props.Add(giButton3Props);

            giButton3Props.Prop = new List<PromoticClass.PropClass>();
            giButton3Props.Prop.Add(SetPropType1("StringText"));
            giButton3Props.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            giButton3Props.Prop.Add(SetPropType1("AlignHor", "1"));
            giButton3Props.Prop.Add(SetPropType1("AlignVer", "1"));

            giButton3.Prop.Add(SetPropType1("ImageSrc", "#pmres:img/IconEvents1.svg"));
            giButton3.Prop.Add(SetPropType1("ImageOper", "4"));

            //
            giButton3.Event = new List<PromoticClass.EventClass>();

            giButton3.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnButtonUpEvent_button3")));

            //button 4
            PromoticClass.GItemClass giButton4 = new PromoticClass.GItemClass();
            giButton4.Name = "button4";
            giButton4.Type = "PmiButton";
            toolbarObj.GPanel.GItem.Add(giButton4);

            giButton4.Prop = new List<PromoticClass.PropClass>();

            giButton4.Prop.Add(SetPropType1("X", "40"));
            giButton4.Prop.Add(SetPropType1("Y", "0"));
            giButton4.Prop.Add(SetPropType1("Dx", "20"));
            giButton4.Prop.Add(SetPropType1("Dy", "20"));
            giButton4.Prop.Add(SetPropType1("FocusType", "0"));
            giButton4.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            giButton4.Prop.Add(SetPropType1("BorderWidth", "1"));
            giButton4.Prop.Add(SetPropType1("BorderContrast", "60"));
            giButton4.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giButton4.Prop.Add(SetPropType1("GradientContrast", "15"));
            giButton4.Prop.Add(SetPropType1("GradientDirection", "0"));
            giButton4.Prop.Add(SetPropType1("BnAttributes", "12"));

            giButton4.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giButton4Props = new PromoticClass.PropsClass();
            giButton4Props.Name = "Text";
            giButton4.Props.Add(giButton4Props);

            giButton4Props.Prop = new List<PromoticClass.PropClass>();
            giButton4Props.Prop.Add(SetPropType1("StringText"));
            giButton4Props.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            giButton4Props.Prop.Add(SetPropType1("AlignHor", "1"));
            giButton4Props.Prop.Add(SetPropType1("AlignVer", "1"));

            giButton4.Prop.Add(SetPropType1("ImageSrc", "#pmres:img/IconInfo1.svg"));
            giButton4.Prop.Add(SetPropType1("ImageOper", "4"));

            //
            giButton4.Event = new List<PromoticClass.EventClass>();

            giButton4.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnButtonUpEvent_button4")));

            //button 5
            PromoticClass.GItemClass giButton5 = new PromoticClass.GItemClass();
            giButton5.Name = "button5";
            giButton5.Type = "PmiButton";
            toolbarObj.GPanel.GItem.Add(giButton5);

            giButton5.Prop = new List<PromoticClass.PropClass>();

            giButton5.Prop.Add(SetPropType1("X", "60"));
            giButton5.Prop.Add(SetPropType1("Y", "0"));
            giButton5.Prop.Add(SetPropType1("Dx", "20"));
            giButton5.Prop.Add(SetPropType1("Dy", "20"));
            giButton5.Prop.Add(SetPropType1("FocusType", "0"));
            giButton5.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            giButton5.Prop.Add(SetPropType1("BorderWidth", "1"));
            giButton5.Prop.Add(SetPropType1("BorderContrast", "60"));
            giButton5.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giButton5.Prop.Add(SetPropType1("GradientContrast", "15"));
            giButton5.Prop.Add(SetPropType1("GradientDirection", "0"));
            giButton5.Prop.Add(SetPropType1("BnAttributes", "12"));

            giButton5.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giButton5Props = new PromoticClass.PropsClass();
            giButton5Props.Name = "Text";
            giButton5.Props.Add(giButton5Props);

            giButton5Props.Prop = new List<PromoticClass.PropClass>();
            giButton5Props.Prop.Add(SetPropType1("StringText"));
            giButton5Props.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            giButton5Props.Prop.Add(SetPropType1("AlignHor", "1"));
            giButton5Props.Prop.Add(SetPropType1("AlignVer", "1"));

            giButton5.Prop.Add(SetPropType1("ImageSrc", "#pmres:img/IconStop2.svg?lightcolor=#00cc00"));
            giButton5.Prop.Add(SetPropType1("ImageOper", "4"));

            //
            giButton5.Event = new List<PromoticClass.EventClass>();

            giButton5.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnButtonUpEvent_button5")));

            //time
            PromoticClass.GItemClass giTime = new PromoticClass.GItemClass();
            giTime.Name = "time";
            giTime.Type = "PmiText";
            toolbarObj.GPanel.GItem.Add(giTime);

            giTime.Prop = new List<PromoticClass.PropClass>();

            giTime.Prop.Add(SetPropType1("X", "1800"));
            giTime.Prop.Add(SetPropType1("Y", "0"));
            giTime.Prop.Add(SetPropType1("Dx", "120"));
            giTime.Prop.Add(SetPropType1("Dy", "20"));
            giTime.Prop.Add(SetPropType1("DrawBg", "1"));
            giTime.Prop.Add(SetPropType1("FocusType", "0"));
            giTime.Prop.Add(SetPropType1("ColorItem", "transparent"));
            giTime.Prop.Add(SetPropType1("BorderWidth", "0"));
            giTime.Prop.Add(SetPropType1("BorderType", "1"));
            giTime.Prop.Add(SetPropType1("BorderContrast", "60"));
            giTime.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giTime.Prop.Add(SetPropType1("GradientContrast", "15"));
            giTime.Prop.Add(SetPropType1("GradientDirection", "2"));

            giTime.Prop.Add(SetPropType1("Multiline", "1"));

            giTime.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giTimeProps = new PromoticClass.PropsClass();
            giTimeProps.Name = "Text";
            giTime.Props.Add(giTimeProps);

            giTimeProps.Prop = new List<PromoticClass.PropClass>();
            giTimeProps.Prop.Add(SetPropType1("Value", "00:00:00"));
            giTimeProps.Prop.Add(SetPropType1("FontText", "PmBig"));
            giTimeProps.Prop.Add(SetPropType1("AlignHor", "1"));
            giTimeProps.Prop.Add(SetPropType1("AlignVer", "1"));

            giTime.Prop.Add(SetPropType1("ValueDataType", "0"));
            giTime.Prop.Add(SetPropType1("ValueFormatType", "0"));
            giTime.Prop.Add(SetPropType1("ValueFormat", "%.2f"));
            giTime.Prop.Add(SetPropType1("ValueMin", "0;0"));
            giTime.Prop.Add(SetPropType1("ValueMax", "100;0"));
            giTime.Prop.Add(SetPropType1("EditAttr", "2"));
            giTime.Prop.Add(SetPropType1("ShadowType", "0"));
            giTime.Prop.Add(SetPropType1("ShadowColor", "#808080"));

            //
            giTime.Event = new List<PromoticClass.EventClass>();

            giTime.Event.Add(SetEvent("onRefresh", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnRefreshEvent_time")));

            //user
            PromoticClass.GItemClass giUser = new PromoticClass.GItemClass();
            giUser.Name = "user";
            giUser.Type = "PmiText";
            toolbarObj.GPanel.GItem.Add(giUser);

            giUser.Prop = new List<PromoticClass.PropClass>();

            giUser.Prop.Add(SetPropType1("X", "1660"));
            giUser.Prop.Add(SetPropType1("Y", "0"));
            giUser.Prop.Add(SetPropType1("Dx", "140"));
            giUser.Prop.Add(SetPropType1("Dy", "20"));
            giUser.Prop.Add(SetPropType1("DrawBg", "1"));
            giUser.Prop.Add(SetPropType1("FocusType", "0"));
            giUser.Prop.Add(SetPropType1("ColorItem", "#a8a8a8"));
            giUser.Prop.Add(SetPropType1("BorderColorLight", "transparent"));
            giUser.Prop.Add(SetPropType1("BorderColorDark", "transparent"));
            giUser.Prop.Add(SetPropType1("BorderWidth", "0"));
            giUser.Prop.Add(SetPropType1("BorderType", "0"));
            giUser.Prop.Add(SetPropType1("BorderContrast", "60"));
            giUser.Prop.Add(SetPropType1("GradientEnabled", "1"));
            giUser.Prop.Add(SetPropType1("GradientContrast", "15"));
            giUser.Prop.Add(SetPropType1("GradientDirection", "2"));
            giUser.Prop.Add(SetPropType1("Multiline", "1"));

            giUser.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass giUserProps = new PromoticClass.PropsClass();
            giUserProps.Name = "Text";
            giUser.Props.Add(giUserProps);

            giUserProps.Prop = new List<PromoticClass.PropClass>();
            giUserProps.Prop.Add(SetPropType3("Value", null, null, "USER", "UP", "Name"));
            giUserProps.Prop.Add(SetPropType1("FontText", "PmBig"));
            giUserProps.Prop.Add(SetPropType1("AlignHor", "1"));
            giUserProps.Prop.Add(SetPropType1("AlignVer", "1"));

            giUser.Prop.Add(SetPropType1("ValueDataType", "0"));
            giUser.Prop.Add(SetPropType1("ValueFormatType", "0"));
            giUser.Prop.Add(SetPropType1("ValueFormat", "%.2f"));
            giUser.Prop.Add(SetPropType1("ValueMin", "0;0"));
            giUser.Prop.Add(SetPropType1("ValueMax", "100;0"));
            giUser.Prop.Add(SetPropType1("EditAttr", "2"));
            giUser.Prop.Add(SetPropType1("ShadowType", "0"));
            giUser.Prop.Add(SetPropType1("ShadowColor", "#808080"));

            giUser.Event = new List<PromoticClass.EventClass>();

            giUser.Event.Add(SetEvent("onMousePress", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "ToolbarOnMousePressEvent_user")));

            //

            //Save button
            PromoticClass.GItemClass Btn_Save = new PromoticClass.GItemClass();
            Btn_Save.Name = "Btn_Save";
            Btn_Save.Type = "PmiButton";
            toolbarObj.GPanel.GItem.Add(Btn_Save);

            Btn_Save.Prop = new List<PromoticClass.PropClass>();

            Btn_Save.Prop.Add(SetPropType1("X", "1540"));
            Btn_Save.Prop.Add(SetPropType1("Y", "0"));
            Btn_Save.Prop.Add(SetPropType1("Dx", "60"));
            Btn_Save.Prop.Add(SetPropType1("Dy", "20"));
            Btn_Save.Prop.Add(SetPropType1("FocusType", "0"));
            Btn_Save.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            Btn_Save.Prop.Add(SetPropType1("BorderWidth", "1"));
            Btn_Save.Prop.Add(SetPropType1("BorderContrast", "60"));
            Btn_Save.Prop.Add(SetPropType1("GradientEnabled", "1"));
            Btn_Save.Prop.Add(SetPropType1("GradientContrast", "15"));
            Btn_Save.Prop.Add(SetPropType1("GradientDirection", "0"));
            Btn_Save.Prop.Add(SetPropType1("BnAttributes", "12"));

            Btn_Save.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass Btn_SaveProps = new PromoticClass.PropsClass();
            Btn_SaveProps.Name = "Text";
            Btn_Save.Props.Add(Btn_SaveProps);

            Btn_SaveProps.Prop = new List<PromoticClass.PropClass>();
            Btn_SaveProps.Prop.Add(SetPropType1("StringText", "Save"));
            Btn_SaveProps.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            Btn_SaveProps.Prop.Add(SetPropType1("AlignHor", "1"));
            Btn_SaveProps.Prop.Add(SetPropType1("AlignVer", "1"));

            PromoticClass.PropsClass Btn_SaveLocalProps = new PromoticClass.PropsClass();
            Btn_SaveLocalProps.Name = "localProps";
            Btn_Save.Props.Add(Btn_SaveLocalProps);

            Btn_SaveLocalProps.Prop = new List<PromoticClass.PropClass>();
            Btn_SaveLocalProps.Prop.Add(SetPropType3("Value", "bool", "bool", null, "PP", "/Z45AppCore/SaveSettings/#vars/Save;Value"));

            //
            Btn_Save.Event = new List<PromoticClass.EventClass>();

            Btn_Save.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "SetValueTrue")));
            //

            //Load button
            PromoticClass.GItemClass Btn_Load = new PromoticClass.GItemClass();
            Btn_Load.Name = "Btn_Load";
            Btn_Load.Type = "PmiButton";
            toolbarObj.GPanel.GItem.Add(Btn_Load);

            Btn_Load.Prop = new List<PromoticClass.PropClass>();

            Btn_Load.Prop.Add(SetPropType1("X", "1600"));
            Btn_Load.Prop.Add(SetPropType1("Y", "0"));
            Btn_Load.Prop.Add(SetPropType1("Dx", "60"));
            Btn_Load.Prop.Add(SetPropType1("Dy", "20"));
            Btn_Load.Prop.Add(SetPropType1("FocusType", "0"));
            Btn_Load.Prop.Add(SetPropType1("ColorItem", "#c0c0c0"));
            Btn_Load.Prop.Add(SetPropType1("BorderWidth", "1"));
            Btn_Load.Prop.Add(SetPropType1("BorderContrast", "60"));
            Btn_Load.Prop.Add(SetPropType1("GradientEnabled", "1"));
            Btn_Load.Prop.Add(SetPropType1("GradientContrast", "15"));
            Btn_Load.Prop.Add(SetPropType1("GradientDirection", "0"));
            Btn_Load.Prop.Add(SetPropType1("BnAttributes", "12"));

            Btn_Load.Props = new List<PromoticClass.PropsClass>();

            PromoticClass.PropsClass Btn_LoadProps = new PromoticClass.PropsClass();
            Btn_LoadProps.Name = "Text";
            Btn_Load.Props.Add(Btn_LoadProps);

            Btn_LoadProps.Prop = new List<PromoticClass.PropClass>();
            Btn_LoadProps.Prop.Add(SetPropType1("StringText", "Load"));
            Btn_LoadProps.Prop.Add(SetPropType1("FontText", "PmMiddle"));
            Btn_LoadProps.Prop.Add(SetPropType1("AlignHor", "1"));
            Btn_LoadProps.Prop.Add(SetPropType1("AlignVer", "1"));

            PromoticClass.PropsClass Btn_LoadLocalProps = new PromoticClass.PropsClass();
            Btn_LoadLocalProps.Name = "localProps";
            Btn_Load.Props.Add(Btn_LoadLocalProps);

            Btn_LoadLocalProps.Prop = new List<PromoticClass.PropClass>();
            Btn_LoadLocalProps.Prop.Add(SetPropType3("Value", "bool", "bool", null, "PP", "/Z45AppCore/SaveSettings/#vars/Load;Value"));

            //
            Btn_Load.Event = new List<PromoticClass.EventClass>();

            Btn_Load.Event.Add(SetEvent("onButtonUp", "Pm", Script(@"Scripts\WorkspaceScripts.xml", "SetValueTrue")));
            //

            
            

            return obj;
        }

        /// <summary>
        /// Create Props element in PmData, without extensions
        /// </summary>
        /// <param name="propsname"></param>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        /// <param name="note"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static PromoticClass.PropsClass SetPropsType1(string propsname, string datatype, string value, string note, string unit, string wactVal, bool evExt)
        {
            PromoticClass.PropsClass props = new PromoticClass.PropsClass();
            props.Name = propsname;

            props.Prop = new List<PromoticClass.PropClass>();
            props.Prop.Add(SetPropType1("DataType", datatype));
            props.Prop.Add(SetPropType1("Value", value));
            props.Prop.Add(SetPropType1("Note", note));
            props.Prop.Add(SetPropType1("Unit", unit));

            if (wactVal != null || evExt)
            {
                props.List = new List<PromoticClass.ListClass>();

                PromoticClass.ListClass extList = new PromoticClass.ListClass();
                extList.Name = "Extens";
                extList.Props = new List<PromoticClass.PropsClass>();
                props.List.Add(extList);


                if (wactVal != null)
                {


                    //write action extension
                    PromoticClass.PropsClass wactProps = new PromoticClass.PropsClass();
                    wactProps.Name = "wact";
                    wactProps.Type = "WriteAction";
                    extList.Props.Add(wactProps);

                    wactProps.Prop = new List<PromoticClass.PropClass>();
                    wactProps.Prop.Add(SetPropType1("After", wactVal));
                }

                if (evExt)
                {
                    //Event extension
                    PromoticClass.PropsClass evProps = new PromoticClass.PropsClass();
                    evProps.Name = "ev";
                    evProps.Type = "Event";
                    extList.Props.Add(evProps);

                    evProps.Prop = new List<PromoticClass.PropClass>();
                    evProps.Prop.Add(SetPropType1("PmObject", "/Z45AppCore/Events"));
                    evProps.Prop.Add(SetPropType1("Template", "event0"));
                    evProps.Prop.Add(SetPropType1("Source", Script(@"Scripts\PmDataScripts.xml", "EvExtSource")));
                    evProps.Prop.Add(SetPropType1("Desc", Script(@"Scripts\PmDataScripts.xml", "EvExtDesc")));
                    evProps.Prop.Add(SetPropType1("ValType", "0"));
                    evProps.Prop.Add(SetPropType1("ValPar", null));
                    evProps.Prop.Add(SetPropType1("ActWhen", "1"));
                }
            }

            return props;
        }

        
        private static PromoticClass.PropsClass SetPropsType3(string name, string propName, string propVal)
        {
            PromoticClass.PropsClass props = new PromoticClass.PropsClass();
            props.Name = name;

            props.Prop = new List<PromoticClass.PropClass>();
            props.Prop.Add(SetPropType1(propName, propVal));

            return props;
        }

        /// <summary>
        /// Create Props element in PmData with extensions: comm (and wact)
        /// </summary>
        /// <param name="propsname"></param>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        /// <param name="note"></param>
        /// <param name="unit"></param>
        /// <param name="commPath"></param>
        /// <param name="commItemId"></param>
        /// <param name="wactVal"></param>
        /// <returns></returns>
        private static PromoticClass.PropsClass SetPropsType2(string propsname, string datatype, string value, string note, string commPath, string commItemId, string wactVal, bool evExt, bool trendExt, string trendScriptName = "TrendExtName")
        {
            PromoticClass.PropsClass props = new PromoticClass.PropsClass();
            props.Name = propsname;

            props.Prop = new List<PromoticClass.PropClass>();
            props.Prop.Add(SetPropType1("DataType", datatype));
            props.Prop.Add(SetPropType1("Value", value));
            props.Prop.Add(SetPropType1("Note", note));
            props.Prop.Add(SetPropType1("Unit", ""));

            props.List = new List<PromoticClass.ListClass>();

            PromoticClass.ListClass extList = new PromoticClass.ListClass();
            extList.Name = "Extens";
            props.List.Add(extList);

            extList.Props = new List<PromoticClass.PropsClass>();

            if (commPath != null)
            {
                //comm extension
                PromoticClass.PropsClass commProps = new PromoticClass.PropsClass();
                commProps.Name = "comm";
                commProps.Type = "Comm";
                extList.Props.Add(commProps);

                commProps.Prop = new List<PromoticClass.PropClass>();
                if (commPath != "")
                {
                    if (commPath == " ")
                        commPath = @"/Comm/data";

                    commProps.Prop.Add(SetPropType1("PmObject", commPath));

                }
                else
                {
                    commProps.Prop.Add(SetPropType1("PmObject", "/Comm/Data"));
                }
                commProps.Prop.Add(SetPropType1("ItemID", commItemId));
            }

            if (wactVal != null)
            {
                //write action extension
                PromoticClass.PropsClass wactProps = new PromoticClass.PropsClass();
                wactProps.Name = "wact";
                wactProps.Type = "WriteAction";
                extList.Props.Add(wactProps);

                wactProps.Prop = new List<PromoticClass.PropClass>();
                wactProps.Prop.Add(SetPropType1("After", wactVal));
            }

            if (evExt)
            {
                //Event extension
                PromoticClass.PropsClass evProps = new PromoticClass.PropsClass();
                evProps.Name = "ev";
                evProps.Type = "Event";
                extList.Props.Add(evProps);

                evProps.Prop = new List<PromoticClass.PropClass>();
                evProps.Prop.Add(SetPropType1("PmObject", "/Z45AppCore/Events"));
                evProps.Prop.Add(SetPropType1("Template", "event0"));
                evProps.Prop.Add(SetPropType1("Source", Script(@"Scripts\PmDataScripts.xml", "EvExtSource")));
                evProps.Prop.Add(SetPropType1("Desc", Script(@"Scripts\PmDataScripts.xml", "EvExtDesc")));
                evProps.Prop.Add(SetPropType1("ValType", "0"));
                evProps.Prop.Add(SetPropType1("ValPar", null));
                evProps.Prop.Add(SetPropType1("ActWhen", "1"));
            }

            if (trendExt)
            {
                //Trend extension
                PromoticClass.PropsClass trendProps = new PromoticClass.PropsClass();
                trendProps.Name = "trend";
                trendProps.Type = "Trend";
                extList.Props.Add(trendProps);

                trendProps.Prop = new List<PromoticClass.PropClass>();
                trendProps.Prop.Add(SetPropType1("PmObject", "/Z45AppCore/Trends"));
                trendProps.Prop.Add(SetPropType1("Order", "1"));
                trendProps.Prop.Add(SetPropType1("Name", Script(@"Scripts\PmDataScripts.xml", trendScriptName)));
                trendProps.Prop.Add(SetPropType1("Min", "0"));
                trendProps.Prop.Add(SetPropType1("Max", "100"));
                trendProps.Prop.Add(SetPropType1("Sensitivity", "0"));
                trendProps.Prop.Add(SetPropType1("Unit", null));
                trendProps.Prop.Add(SetPropType1("Color", "#0000ff"));
                trendProps.Prop.Add(SetPropType1("DisplayName", Script(@"Scripts\PmDataScripts.xml", trendScriptName)));
                trendProps.Prop.Add(SetPropType1("Params", null));
                trendProps.Prop.Add(SetPropType1("ValType", "0"));
                trendProps.Prop.Add(SetPropType1("ValPar", null));
            }

            return props;
        }

        /// <summary>
        /// Create Prop element with value only
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PromoticClass.PropClass SetPropType1(string name, string value = "")
        {
            PromoticClass.PropClass prop = new PromoticClass.PropClass();
            prop.Name = name;
            prop.Value = value;
            return prop;
        }

        /// <summary>
        /// Create Prop element with type and optional value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PromoticClass.PropClass SetPropType2(string name, string type, string value = "")
        {
            PromoticClass.PropClass prop = new PromoticClass.PropClass();
            prop.Name = name;
            prop.Type = type;
            prop.Value = value;
            return prop;
        }

        /// <summary>
        /// Create Prop element with type and bind prop
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="dstatictype"></param>
        /// <param name="dbindtype"></param>
        /// <param name="dbindval"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PromoticClass.PropClass SetPropType3(string name, string type, string dstatictype, string dstaticval, string dbindtype, string dbindval, string value = "")
        {
            PromoticClass.PropClass prop = new PromoticClass.PropClass();
            prop.DStatic = new PromoticClass.DStaticClass();
            prop.DBind = new PromoticClass.DBindClass();
            prop.DBind.Prop = new PromoticClass.PropClass();

            prop.Name = name;
            prop.Type = type;
            prop.Value = value;

            prop.DStatic.Type = dstatictype;
            prop.DStatic.Value = dstaticval;

            prop.DBind.Type = dbindtype;
            prop.DBind.Prop.Name = "Value";
            prop.DBind.Prop.Value = dbindval;

            return prop;
        }

        /// <summary>
        /// Create Event element with script
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private static PromoticClass.EventClass SetEvent(string name, string type, string script)
        {
            PromoticClass.EventClass graphEvent = new PromoticClass.EventClass();
            graphEvent.Name = name;
            graphEvent.Type = type;
            graphEvent.Script.Content = script;

            return graphEvent;
        }

        /// <summary>
        /// Create Method element with script and parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pars"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private static PromoticClass.MethodClass SetMethod(string name, string pars, string script)
        {
            PromoticClass.MethodClass gMethod = new PromoticClass.MethodClass();
            gMethod.Name = name;

            gMethod.Prop = new PromoticClass.PropClass();
            gMethod.Prop.Name = "Params";
            gMethod.Prop.Value = pars;

            gMethod.Script.Content = script;

            return gMethod;
        }

        /// <summary>
        /// Creates PmTrend object Trends
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass Trends_PmTrend()
        {
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Prop = new List<PromoticClass.PropClass>();
            obj.Props = new List<PromoticClass.PropsClass>();
            obj.Name = "Trends";
            obj.Type = "PmTrend";

            //Objekt
            obj.Prop.Add(SetPropType1("ReferenceType", "2"));
            obj.Prop.Add(SetPropType1("ReferenceName", "Trends"));
            obj.Prop.Add(SetPropType1("GroupID", "IO"));
            obj.Prop.Add(SetPropType1("SaveChangeEnabled", "1"));
            obj.Prop.Add(SetPropType1("TimerPeriod", "1"));
            obj.Prop.Add(SetPropType1("TimerEnabled", "1"));

            //Ulozeni
            PromoticClass.PropsClass saveTypeParamsProps = new PromoticClass.PropsClass();
            saveTypeParamsProps.Name = "SaveTypeParams";
            obj.Props.Add(saveTypeParamsProps);
            saveTypeParamsProps.Prop = new List<PromoticClass.PropClass>();

            saveTypeParamsProps.Prop.Add(SetPropType1("BackupSizeByCount", "2000"));
            saveTypeParamsProps.Prop.Add(SetPropType1("BackupCount", "-1"));
            saveTypeParamsProps.Prop.Add(SetPropType1("Connection", Script(@"Scripts\CommonScripts.xml", "TrendConnection")));

            //Web server
            obj.Props.Add(WebProps(false));

            return obj;
        }

        /// <summary>
        /// Creates PmAlarmEvent object Events
        /// </summary>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass Events_PmAlarmEvent()
        {
            //Objekt
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Prop = new List<PromoticClass.PropClass>();
            obj.Props = new List<PromoticClass.PropsClass>();
            obj.List = new List<PromoticClass.ListClass>();

            obj.Name = "Events";
            obj.Type = "PmAlarmEvent";

            obj.Prop.Add(SetPropType1("ReferenceType", "2"));
            obj.Prop.Add(SetPropType1("ReferenceName", "Events"));
            obj.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));
            obj.Prop.Add(SetPropType1("InactType", "0"));
            obj.Prop.Add(SetPropType1("AckType", "0"));
            obj.Prop.Add(SetPropType1("DeleteInactive", "1"));

            //Hlavni
            obj.Prop.Add(SetPropType1("Id", "events"));
            obj.Prop.Add(SetPropType1("Title", "Events"));
            obj.Prop.Add(SetPropType1("Type", "0"));//0-events, 1-alarms

            //Seznam
            PromoticClass.ListClass seznamList = new PromoticClass.ListClass();
            seznamList.Name = "Items";
            seznamList.Props = new List<PromoticClass.PropsClass>();
            obj.List.Add(seznamList);

            PromoticClass.PropsClass event0 = new PromoticClass.PropsClass();
            event0.Name = "event0";
            event0.Prop = new List<PromoticClass.PropClass>();
            seznamList.Props.Add(event0);

            event0.Prop = new List<PromoticClass.PropClass>();
            event0.Prop.Add(SetPropType1("InactType", "0"));
            event0.Prop.Add(SetPropType1("AckType", "0"));

            event0.Prop.Add(SetPropType1("Template", "1"));
            event0.Prop.Add(SetPropType1("DeleteInactive", "$default"));

            //Prohlizec stavu
            PromoticClass.PropsClass stateBrowserProps = new PromoticClass.PropsClass();
            stateBrowserProps.Name = "StateBrowser";
            stateBrowserProps.Prop = new List<PromoticClass.PropClass>();
            obj.Props.Add(stateBrowserProps);

            stateBrowserProps.Prop.Add(SetPropType1("Enabled", "0"));
            stateBrowserProps.Prop.Add(SetPropType1("HideInactive", "1"));
            stateBrowserProps.Prop.Add(SetPropType1("CfgFile", "#cfg:WndStat.eg"));
            stateBrowserProps.Prop.Add(SetPropType1("Title", "$.text('sys','evStatTitle')"));

            //Prohlizec historie
            PromoticClass.PropsClass histBrowserProps = new PromoticClass.PropsClass();
            histBrowserProps.Name = "HistoryBrowser";
            histBrowserProps.List = new List<PromoticClass.ListClass>();
            obj.Props.Add(histBrowserProps);

            histBrowserProps.Prop = new List<PromoticClass.PropClass>();
            histBrowserProps.Prop.Add(SetPropType1("CfgFile", "#cfg:WndHist.eg"));
            histBrowserProps.Prop.Add(SetPropType1("Title", "$.text('sys','evHistTitle')"));

            PromoticClass.ListClass histBrowserList = new PromoticClass.ListClass();
            histBrowserList.Name = "Columns";
            histBrowserList.Props = new List<PromoticClass.PropsClass>();
            histBrowserProps.List.Add(histBrowserList);

            //
            PromoticClass.PropsClass histBrowserProps1 = new PromoticClass.PropsClass();
            histBrowserProps1.Name = "Source";
            histBrowserProps1.Prop = new List<PromoticClass.PropClass>();
            histBrowserList.Props.Add(histBrowserProps1);

            histBrowserProps1.Prop.Add(SetPropType1("Title", "$.text('sys','source')"));
            histBrowserProps1.Prop.Add(SetPropType1("Width", "60"));

            //
            PromoticClass.PropsClass histBrowserProps2 = new PromoticClass.PropsClass();
            histBrowserProps2.Name = "Desc";
            histBrowserProps2.Prop = new List<PromoticClass.PropClass>();
            histBrowserList.Props.Add(histBrowserProps2);

            histBrowserProps2.Prop.Add(SetPropType1("Title", "$.text('sys','description')"));
            histBrowserProps2.Prop.Add(SetPropType1("Width", "240"));

            //
            PromoticClass.PropsClass histBrowserProps3 = new PromoticClass.PropsClass();
            histBrowserProps3.Name = "TimeOn";
            histBrowserProps3.Prop = new List<PromoticClass.PropClass>();
            histBrowserList.Props.Add(histBrowserProps3);

            histBrowserProps3.Prop.Add(SetPropType1("Title", "$.text('sys','timeOn')"));
            histBrowserProps3.Prop.Add(SetPropType1("Width", "113"));

            //Ulozeni
            PromoticClass.PropsClass persistProps = new PromoticClass.PropsClass();
            persistProps.Name = "Persist";
            persistProps.Props = new List<PromoticClass.PropsClass>();
            persistProps.Prop = new List<PromoticClass.PropClass>();
            obj.Props.Add(persistProps);

            persistProps.Prop.Add(SetPropType1("Type", "filedbf"));

            PromoticClass.PropsClass dataProps = new PromoticClass.PropsClass();
            dataProps.Name = "Data";
            dataProps.List = new List<PromoticClass.ListClass>();

            PromoticClass.ListClass columns = new PromoticClass.ListClass();
            dataProps.List.Add(columns);
            columns.Name = "Columns";

            columns.Props = new List<PromoticClass.PropsClass>();
            columns.Props.Add(SetPropsType3("TimeOn", "Width", "26"));
            columns.Props.Add(SetPropsType3("Source", "Width", "25"));
            columns.Props.Add(SetPropsType3("Desc", "Width", "35"));

            dataProps.Props = new List<PromoticClass.PropsClass>();
            persistProps.Props.Add(dataProps);

            dataProps.Prop = new List<PromoticClass.PropClass>();
            dataProps.Prop.Add(SetPropType1("LangMode", "macro:"));

            PromoticClass.PropsClass backupProps = new PromoticClass.PropsClass();
            backupProps.Name = "SaveTypeParams";
            backupProps.Prop = new List<PromoticClass.PropClass>();
            dataProps.Props.Add(backupProps);

            backupProps.Prop.Add(SetPropType1("Charset", "0"));
            backupProps.Prop.Add(SetPropType1("File", "Events"));
            backupProps.Prop.Add(SetPropType1("Directory", "#data:Event/"));
            backupProps.Prop.Add(SetPropType1("RecordsEnabled", "-1"));
            backupProps.Prop.Add(SetPropType1("Records", "2000"));
            backupProps.Prop.Add(SetPropType1("TimeEnabled", "0"));
            backupProps.Prop.Add(SetPropType1("Time", "1"));
            backupProps.Prop.Add(SetPropType1("BackupsEnabled", "0"));
            backupProps.Prop.Add(SetPropType1("Backups", "12"));

            //Web server
            obj.Props.Add(WebProps(false));

            return obj;
        }

        /// <summary>
        /// Creates PmAlarmEvent object Alarms
        /// </summary>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Alarms_PmAlarmEvent(BDMdataClass data)
        {
            //Objekt
            PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
            obj.Prop = new List<PromoticClass.PropClass>();
            obj.Props = new List<PromoticClass.PropsClass>();
            obj.List = new List<PromoticClass.ListClass>();

            obj.Name = "Alarms";
            obj.Type = "PmAlarmEvent";

            obj.Prop.Add(SetPropType1("ReferenceType", "2"));
            obj.Prop.Add(SetPropType1("ReferenceName", "Alarms"));
            obj.Prop.Add(SetPropType1("MemberOfLogicalGroups", "menu"));

            //Events
            obj.Events = new PromoticClass.EventsClass();
            obj.Events.Name = "PmEvents";
            obj.Events.Event = new List<PromoticClass.EventClass>();

            PromoticClass.EventClass onStatChangeEv = new PromoticClass.EventClass();
            obj.Events.Event.Add(onStatChangeEv);
            onStatChangeEv.Name = "onStateChange";
            onStatChangeEv.Type = "Pm";
            onStatChangeEv.Script.Content = AlarmEventScript(data);

            //Methods
            obj.Methods = new PromoticClass.MethodsClass();
            obj.Methods.Method = new List<PromoticClass.MethodClass>();

            obj.Methods.Method.Add(SetMethod("Ain_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "Ain_AlarmsMethod")));
            obj.Methods.Method.Add(SetMethod("AnalogPos_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "AnalogPos_AlarmsMethod")));
            obj.Methods.Method.Add(SetMethod("Aout_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "Aout_AlarmsMethod")));
            obj.Methods.Method.Add(SetMethod("Din_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "Din_AlarmsMethod")));
            obj.Methods.Method.Add(SetMethod("OnOffCtrl_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "OnOffCtrl_AlarmsMethod")));
            obj.Methods.Method.Add(SetMethod("OnOffCtrl2D_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "OnOffCtrl2D_AlarmsMethod")));
            obj.Methods.Method.Add(SetMethod("OnOffCtrlVSD_Alarms", "tagname", Script(@"Scripts\AlarmScripts.xml", "OnOffCtrlVSD_AlarmsMethod")));

            //Hlavni
            obj.Prop.Add(SetPropType1("Id", "alarms"));
            obj.Prop.Add(SetPropType1("Title", "Alarms"));
            obj.Prop.Add(SetPropType1("Type", "1"));//0-events, 1-alarms

            //Seznam
            PromoticClass.ListClass seznamList = new PromoticClass.ListClass();
            seznamList.Name = "Items";
            seznamList.Props = new List<PromoticClass.PropsClass>();
            obj.List.Add(seznamList);

            PromoticClass.PropsClass item0 = new PromoticClass.PropsClass();
            item0.Name = "IO";
            item0.Prop = new List<PromoticClass.PropClass>();
            seznamList.Props.Add(item0);
            item0.Prop.Add(SetPropType1("Source", "IO"));
            item0.Prop.Add(SetPropType1("Desc", "Alarmy z IO"));
            item0.Prop.Add(SetPropType1("Priority", "10"));

            //Prohlizec stavu
            PromoticClass.PropsClass stateBrowserProps = new PromoticClass.PropsClass();
            stateBrowserProps.Name = "StateBrowser";
            stateBrowserProps.Prop = new List<PromoticClass.PropClass>();
            stateBrowserProps.List = new List<PromoticClass.ListClass>();
            obj.Props.Add(stateBrowserProps);

            stateBrowserProps.Prop.Add(SetPropType1("Enabled", "1"));
            stateBrowserProps.Prop.Add(SetPropType1("HideInactive", "1"));
            stateBrowserProps.Prop.Add(SetPropType1("ColorMode", "1"));

            stateBrowserProps.Props = new List<PromoticClass.PropsClass>();
            PromoticClass.PropsClass colors = new PromoticClass.PropsClass();
            colors.Name = "Colors";
            stateBrowserProps.Props.Add(colors);

            colors.Prop = new List<PromoticClass.PropClass>();
            colors.Prop.Add(SetPropType1("ActAck", "#ff0000"));
            colors.Prop.Add(SetPropType1("InactUnack", "#ff6868"));
            colors.Prop.Add(SetPropType1("InactAck", "#000000"));

            PromoticClass.ListClass columns = new PromoticClass.ListClass();
            columns.Name = "Columns";
            columns.Props = new List<PromoticClass.PropsClass>();
            stateBrowserProps.List.Add(columns);

            //
            PromoticClass.PropsClass colprops1 = new PromoticClass.PropsClass();
            colprops1.Name = "Source";
            colprops1.Prop = new List<PromoticClass.PropClass>();
            columns.Props.Add(colprops1);

            colprops1.Prop.Add(SetPropType1("Title", "$.text('sys','source')"));
            colprops1.Prop.Add(SetPropType1("Width", "120"));

            //
            PromoticClass.PropsClass colprops2 = new PromoticClass.PropsClass();
            colprops2.Name = "Desc";
            colprops2.Prop = new List<PromoticClass.PropClass>();
            columns.Props.Add(colprops2);

            colprops2.Prop.Add(SetPropType1("Title", "$.text('sys','description')"));
            colprops2.Prop.Add(SetPropType1("Width", "120"));

            //
            PromoticClass.PropsClass colprops3 = new PromoticClass.PropsClass();
            colprops3.Name = "Comment";
            colprops3.Prop = new List<PromoticClass.PropClass>();
            columns.Props.Add(colprops3);

            colprops3.Prop.Add(SetPropType1("Title", "$.text('sys','comment')"));
            colprops3.Prop.Add(SetPropType1("Width", "120"));

            //
            PromoticClass.PropsClass colprops4 = new PromoticClass.PropsClass();
            colprops4.Name = "TimeOn";
            colprops4.Prop = new List<PromoticClass.PropClass>();
            columns.Props.Add(colprops4);

            colprops4.Prop.Add(SetPropType1("Title", "$.text('sys','timeOn')"));
            colprops4.Prop.Add(SetPropType1("Width", "113"));

            //
            PromoticClass.PropsClass colprops5 = new PromoticClass.PropsClass();
            colprops5.Name = "TimeOff";
            colprops5.Prop = new List<PromoticClass.PropClass>();
            columns.Props.Add(colprops5);

            colprops5.Prop.Add(SetPropType1("Title", "$.text('sys','timeOff')"));
            colprops5.Prop.Add(SetPropType1("Width", "113"));

            //
            PromoticClass.PropsClass colprops6 = new PromoticClass.PropsClass();
            colprops6.Name = "TimeAck";
            colprops6.Prop = new List<PromoticClass.PropClass>();
            columns.Props.Add(colprops6);

            colprops6.Prop.Add(SetPropType1("Title", "$.text('sys','timeAck')"));
            colprops6.Prop.Add(SetPropType1("Width", "113"));

            //Prohlizec historie
            PromoticClass.PropsClass histBrowserProps = new PromoticClass.PropsClass();
            histBrowserProps.Name = "HistoryBrowser";
            histBrowserProps.Prop = new List<PromoticClass.PropClass>();
            obj.Props.Add(histBrowserProps);

            histBrowserProps.Prop.Add(SetPropType1("Enabled", "0"));

            //Ulozeni
            PromoticClass.PropsClass persistProps = new PromoticClass.PropsClass();
            persistProps.Name = "Persist";
            persistProps.Props = new List<PromoticClass.PropsClass>();
            persistProps.Prop = new List<PromoticClass.PropClass>();
            obj.Props.Add(persistProps);

            persistProps.Prop.Add(SetPropType1("Type", "none"));

            //Web server
            obj.Props.Add(WebProps(false));

            return obj;
        }

        /// <summary>
        /// Build Script to onStateChange event
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string AlarmEventScript(BDMdataClass data)
        {
            string cont = "";
            foreach (BasicObject item in data.Objects)
            {
                if (item.DataType == Resource.AinType)
                {
                    cont += "pMe.Methods.Ain_Alarms " + '\"' + item.TagName + '\"';
                }

                if (item.DataType == Resource.AoutType)
                {
                    cont += "pMe.Methods.Aout_Alarms " + '\"' + item.TagName + '\"';
                }

                if (item.DataType == Resource.DinType)
                {
                    cont += "pMe.Methods.Din_Alarms " + '\"' + item.TagName + '\"';
                }

                if (item.DataType == Resource.AnalogPosType)
                {
                    cont += "pMe.Methods.AnalogPos_Alarms " + '\"' + item.TagName + '\"';
                }

                if (item.DataType == Resource.OnOffType)
                {
                    cont += "pMe.Methods.OnOffCtrl_Alarms " + '\"' + item.TagName + '\"';
                }

                if (item.DataType == Resource.OnOff2DType)
                {
                    cont += "pMe.Methods.OnOffCtrl2D_Alarms " + '\"' + item.TagName + '\"';
                }

                if (item.DataType == Resource.OnOffVSDType)
                {
                    cont += "pMe.Methods.OnOffCtrlVSD_Alarms " + '\"' + item.TagName + '\"';
                }

                cont += Environment.NewLine;
            }


            return cont;
        }

        /// <summary>
        /// Builds xml data for user.
        /// </summary>
        /// <param name="userID">User ID without spaces.</param>
        /// <param name="userType">Use Constant: Local, Net, LocalNet.</param>
        /// <param name="groups">User Group.</param>
        /// <param name="priority">Priority 0-100. 100 = highest.</param>
        /// <param name="userName">User Name used to log on.</param>
        /// <param name="pwdEn">Set to enable password protection.</param>
        /// <param name="pwd">Password.</param>
        /// <returns>PromoticClass.PropsClass</returns>
        private static PromoticClass.PropsClass UserProps(string userID, string userType, string groups, int priorioty, string userName, bool pwdEn, string pwd)
        {
            PromoticClass.PropsClass props = new PromoticClass.PropsClass();

            props.Name = userID;
            props.Prop = new List<PromoticClass.PropClass>();

            props.Prop.Add(SetPropType1("UserType", userType.ToString()));
            props.Prop.Add(SetPropType1("Stored", "Pra"));
            props.Prop.Add(SetPropType1("Groups", groups));
            props.Prop.Add(SetPropType1("Priority", priorioty.ToString()));
            props.Prop.Add(SetPropType1("Name", userName));
            props.Prop.Add(SetPropType1("PasswordEnabled", Convert.ToInt32(pwdEn).ToString()));
            props.Prop.Add(SetPropType1("Password", pwd));
            props.Prop.Add(SetPropType1("IpAddressEnabled", "0"));
            props.Prop.Add(SetPropType1("IPAddress", ""));

            return props;
        }


        /// <summary>
        /// Builds xml data for user groups.
        /// </summary>
        /// <param name="groupName">User Group name without spaces.</param>
        /// <returns>PromoticClass.PropsClass</returns> 
        private static PromoticClass.PropsClass UserGroupsProps(string groupName)
        {
            PromoticClass.PropsClass props = new PromoticClass.PropsClass();

            props.Name = groupName;
            props.Prop = new List<PromoticClass.PropClass>();
            props.Prop.Add(SetPropType1("Stored", "Pra"));

            return props;
        }


        /// <summary>
        /// Builds xml data for folder.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass PmFolderObj(string folderName)
        {
            PromoticClass.PmObjectClass pmObject = new PromoticClass.PmObjectClass();

            pmObject.Name = folderName;
            pmObject.Type = "PmFolder";

            return pmObject;
        }

        /// <summary>
        /// Builds xml data for timer.
        /// </summary>
        /// <param name="name">Timer name.</param>
        /// <param name="timerScript">OnTick event script.</param>
        /// <returns>PromoticClass.PmObjectClass</returns>
        private static PromoticClass.PmObjectClass PmTimerObj(string name, string timerScript)
        {
            PromoticClass.PmObjectClass pmObject = new PromoticClass.PmObjectClass();
            PromoticClass.EventClass pmEvent = new PromoticClass.EventClass();

            pmObject.Events = new PromoticClass.EventsClass();
            pmObject.Events.Event = new List<PromoticClass.EventClass>();

            pmObject.Name = name;
            pmObject.Type = "PmTimer";

            pmObject.Events.Name = "PmEvents";
            pmObject.Events.Event.Add(pmEvent);

            pmEvent.Name = "onTick";
            pmEvent.Type = "Pm";
            pmEvent.Script.Content = timerScript;

            return pmObject;
        }


        /// <summary>
        /// Creates Props object of vars in PmData
        /// </summary>
        /// <param name="varName">Var name</param>
        /// <param name="dataType">Data type</param>
        /// <param name="value">Default value</param>
        /// <param name="note">Note</param>
        /// <param name="unit">Unit</param>
        /// <returns>PromoticClass.PropsClass</returns>
        private static PromoticClass.PropsClass DataProps(string varName, string dataType, string value, string note, string unit)
        {
            PromoticClass.PropsClass props = new PromoticClass.PropsClass();

            props.Name = varName;
            props.Prop = new List<PromoticClass.PropClass>();

            props.Prop.Add(SetPropType1("DataType", dataType));
            props.Prop.Add(SetPropType1("Value", value));
            props.Prop.Add(SetPropType1("Note", note));
            props.Prop.Add(SetPropType1("Prec", null));
            props.Prop.Add(SetPropType1("Unit", unit));

            return props;
        }

        /// <summary>
        /// Generate web object
        /// </summary>
        /// <param name="addToList">True if visible in overview list.</param>
        /// <returns>PromoticClass.PropsClass</returns>
        private static PromoticClass.PropsClass WebProps(bool addToList, string webName = "$.expr(\"pMe.Name\")")
        {
            PromoticClass.PropsClass obj = new PromoticClass.PropsClass();
            obj.Name = "WebServer";
            obj.Prop = new List<PromoticClass.PropClass>();

            obj.Prop.Add(SetPropType1("Enable", "1"));
            obj.Prop.Add(SetPropType1("Id", webName));
            obj.Prop.Add(SetPropType1("Server", "/Z45AppCore/Web"));
            obj.Prop.Add(SetPropType1("RefreshPeriod", "0.5"));
            obj.Prop.Add(SetPropType1("AddToList", Convert.ToInt32(addToList).ToString()));

            return obj;
        }

        /// <summary>
        /// Get data from Scripts.xml
        /// </summary>
        /// <param name="scriptName">Name of script.</param>
        /// <returns>Script in string value.</returns>
        private static string Script(string scriptFile, string scriptName)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6) + @"\Promotic\";

            scriptFile = path + scriptFile;

            XDocument message = new XDocument(XDocument.Load(scriptFile));
            string cDataContent;

            try
            {
                XCData cdata = message.DescendantNodes().OfType<XCData>().Where(m => m.Parent.Name == scriptName).ToList()[0];
                cDataContent = cdata.Value;
            }
            catch
            {
                cDataContent = null;
            }

            return cDataContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass AnalogPos_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "AnalogPosCtrlData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "AnalogPosCtrlDataOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "AnalogPosCtrlDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Seq",             DataType.Boolean, null, "Sequence",                             null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cen",             DataType.Boolean, null, "Central",                              null, "1", true));
                    listVars.Props.Add(SetPropsType1("Loc",             DataType.Boolean, null, "Local",                                null, "1", true));
                    listVars.Props.Add(SetPropsType1("E1",              DataType.Boolean, null, "E1",                                   null, "1", true));
                    listVars.Props.Add(SetPropsType1("E2",              DataType.Boolean, null, "E2",                                   null, "1", true));
                    listVars.Props.Add(SetPropsType1("Man",             DataType.Boolean, null, "Manual",                               null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFault",         DataType.Boolean, null, "FB Fault",                             null, "1", false));
                    listVars.Props.Add(SetPropsType1("PosError",        DataType.Boolean, null, "Position error",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("HWSigFault_FB",   DataType.Boolean, null, "Hardware signal fault",                null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_ManSel",      DataType.Boolean, null, "Manual from FP",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_E1Sel",       DataType.Boolean, null, "E1 from FP",                           null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_E2Sel",       DataType.Boolean, null, "E2 from FP",                           null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_CenSel",      DataType.Boolean, null, "Central from FP",                      null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Sim",         DataType.Boolean, null, "Simulation from FP",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_OutOfSrv",    DataType.Boolean, null, "Out of service from FP",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd0",            DataType.Boolean, null, "Command 0",                            null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cmd1",            DataType.Boolean, null, "Command 1",                            null, "1", true));
                    listVars.Props.Add(SetPropsType1("CmdInc_Int",      DataType.Boolean, null, "Command increment",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("CmdDec_Int",      DataType.Boolean, null, "Command decrement",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBLoc",           DataType.Boolean, null, "Feedback local",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB0",             DataType.Boolean, null, "Feedback 0",                           null, "1", true));
                    listVars.Props.Add(SetPropsType1("FB1",             DataType.Boolean, null, "Feedback 1",                           null, "1", true));
                    listVars.Props.Add(SetPropsType1("SetSeq",          DataType.Boolean, null, "Sequence from logic",                  null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetCen",          DataType.Boolean, null, "Cenetral from logic",                  null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetMan",          DataType.Boolean, null, "Manual from logic",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetE1",           DataType.Boolean, null, "E1 from logic",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetE2",           DataType.Boolean, null, "E2 from logic",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("ConPuAct",        DataType.Boolean, null, "Activate controlled pulse commands",   null, "1", false));

                    listVars.Props.Add(SetPropsType2("HMI_DevDelay",    DataType.Integer, null, "Deviation delay from FP",              commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"),   null, false, false));
                    listVars.Props.Add(SetPropsType2("PF",              DataType.Integer, null, "Pulse freq factor",                    commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "INT"),   null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_DevDB",       DataType.Double,  null, "Deviation deadband from FP",           commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 8, "REAL"),  null, false, false));
                    listVars.Props.Add(SetPropsType2("CmdValue",        DataType.Double,  null, "Command value",                        commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 12, "REAL"), "1", false, true, "TrendExtNameCmdValue"));
                    listVars.Props.Add(SetPropsType2("ActFB",           DataType.Double,  null, "Actual feedback",                      commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 16, "REAL"), null, false, true, "TrendExtNameActFB"));
                    listVars.Props.Add(SetPropsType2("ConPuK",          DataType.Double,  null, "Pulse lenght factor",                  commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 20, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("ACTT",            DataType.Double,  null, "Actuator time",                        commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 24, "REAL"), null, false, false));

                    listVars.Props.Add(SetPropsType1("HWSigFault_FB_ALAck", DataType.Boolean, null,     "Acknowledge on false - HMI only",  null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFault_ALAck",       DataType.Boolean, null,     "Ackowledge on false - HMI only",   null, "1", false));
                    listVars.Props.Add(SetPropsType1("PosError_ALAck",      DataType.Boolean, null,     "Acknowledge on false - HMI only",  null, "1", false));
                    listVars.Props.Add(SetPropsType1("Description",         DataType.String, tag.Descr, "HMI only",                         null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",               DataType.Boolean, null,     "HMI only",                         null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",              DataType.Boolean, null,     "HMI only",                         null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",                DataType.String, null,      "HMI only",                         null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "AnalogPosCtrl..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Aout_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "AoutData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "AoutDataOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "AoutDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("ManualMode",      DataType.Boolean, null, "Manual mode",              null, "1", true));
                    listVars.Props.Add(SetPropsType1("SetAuto",         DataType.Boolean, null, "Set Auto mode",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("HWSigFault",      DataType.Boolean, null, "Hardware signal fault",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetManual",       DataType.Boolean, null, "Set Manual mode",          null, "1", false));

                    listVars.Props.Add(SetPropsType2("HMI_Value",       DataType.Double,  null, "Value from FP",            commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("Min",             DataType.Double,  null, "Min",                      commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 8, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("Max",             DataType.Double,  "100","Max",                      commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 12, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("ValueIn",         DataType.Double,  null, "Value from logic",         commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 16, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("ValueOut",        DataType.Double,  null, "Value out to channel",     commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 20, "REAL"), null, false, true));

                    listVars.Props.Add(SetPropsType1("HWSigFault_ALAck",    DataType.Boolean, null,      "HMI only", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Description",         DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Unit",                DataType.String, tag.Unit,  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",               DataType.Boolean, null,      "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",              DataType.Boolean, null,      "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",                DataType.String, null,      "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("barGraphColor1",      DataType.String, "#30ccff", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("barGraphColor2",      DataType.String, "#a8ccf0", "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "Aout..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Dout_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "DoutData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "DoutDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("ManualMode",      DataType.Boolean, null, "Manual mode",      null, "1", true));
                    listVars.Props.Add(SetPropsType1("Value",           DataType.Boolean, null, "Value to channel", null, "1", true));
                    listVars.Props.Add(SetPropsType1("InValue",         DataType.Boolean, null, "Value from logic", null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetAuto",         DataType.Boolean, null, "Set auto mode",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetManual",       DataType.Boolean, null, "Set manual mode",  null, "1", false));

                    listVars.Props.Add(SetPropsType2("PulseTime",       DataType.Integer, null, "Pulse time",       commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), null, false, false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String, null,      "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "Dout..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Grp_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "GrpData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "GrpCtrlDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("GrRFS",           DataType.Boolean, null, "Group ready for start",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cen",             DataType.Boolean, null, "Central",                  null, "1", true));
                    listVars.Props.Add(SetPropsType1("Seq",             DataType.Boolean, null, "Sequence",                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("SetCen",          DataType.Boolean, null, "Set Central from logic",   null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetSeq",          DataType.Boolean, null, "Set Sequence from logic",  null, "1", false));
                    listVars.Props.Add(SetPropsType1("SeqStart",        DataType.Boolean, null, "Sequence start",           null, "1", false));
                    listVars.Props.Add(SetPropsType1("SeqStop",         DataType.Boolean, null, "Sequence stop",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_SetCen",      DataType.Boolean, null, "Set Central from FP",      null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_SetSeq",      DataType.Boolean, null, "Set Sequence from FP",     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Start",       DataType.Boolean, null, "Start from FP",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Stop",        DataType.Boolean, null, "Stop from FP",             null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Hold",        DataType.Boolean, null, "Hold from FP",             null, "1", false));
                    listVars.Props.Add(SetPropsType1("PreselMiss",      DataType.Boolean, null, "Preselection missing",     null, "1", false));
                    listVars.Props.Add(SetPropsType1("GrpStarted",      DataType.Boolean, null, "Group started",            null, "1", true));
                    listVars.Props.Add(SetPropsType1("GrpStopped",      DataType.Boolean, null, "Group stopped",            null, "1", true));
                    listVars.Props.Add(SetPropsType1("GroupHold",       DataType.Boolean, null, "Group in hold",            null, "1", true));
                    listVars.Props.Add(SetPropsType1("GrpStarting",     DataType.Boolean, null, "Group is starting",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("GrpStopping",     DataType.Boolean, null, "Group is stopping",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("RFS",             DataType.Boolean, null, "Ready for start",          null, "1", false));
                    listVars.Props.Add(SetPropsType1("GrRR",            DataType.Boolean, null, "Group restart",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("PGM",             DataType.Boolean, null, "Prepare Group Members",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("Sim",             DataType.Boolean, null, "Simulation",               null, "1", false));

                    listVars.Props.Add(SetPropsType2("ActualStep",      DataType.Integer, null, "Actual Step",               commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), null, false, false));
                    listVars.Props.Add(SetPropsType2("StepTimer",       DataType.Integer, null, "Step Timer",                commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 26, "INT"), null, false, false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String, null,      "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "GroupCtrl..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass OnOff_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "OnOffCtrlData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "OnOffCtrlDataOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "OnOffCtrlDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W3", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W4", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Cmd0Int",         DataType.Boolean, null, "Internal command 0",       null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd1Int",         DataType.Boolean, null, "Intenral command 1",       null, "1", false));
                    listVars.Props.Add(SetPropsType1("IC",              DataType.Boolean, null, "Interlock critical",       null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB1",             DataType.Boolean, null, "Interlock blockable 1",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB2",             DataType.Boolean, null, "Interlock blockable 2",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB3",             DataType.Boolean, null, "Interlock blockable 3",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB4",             DataType.Boolean, null, "Interlock blockable 4",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("PD",              DataType.Boolean, null, "Previous Drive",           null, "1", true));
                    listVars.Props.Add(SetPropsType1("ME",              DataType.Boolean, null, "Main power supply ok",     null, "1", true));
                    listVars.Props.Add(SetPropsType1("Auto",            DataType.Boolean, null, "Auto",                     null, "1", true));
                    listVars.Props.Add(SetPropsType1("Seq",             DataType.Boolean, null, "Sequence",                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cen",             DataType.Boolean, null, "Central",                  null, "1", true));
                    listVars.Props.Add(SetPropsType1("Loc",             DataType.Boolean, null, "Local",                    null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_OutOfSrv",    DataType.Boolean, null, "Out of service",           null, "1", false));
                    listVars.Props.Add(SetPropsType1("Run",             DataType.Boolean, null, "Run",                      null, "1", true));
                    listVars.Props.Add(SetPropsType1("RFS",             DataType.Boolean, null, "Ready for start",          null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFault",         DataType.Boolean, null, "Feedback fault",           null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_CenSel",      DataType.Boolean, null, "Set Central from FP",      null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_AutoSel",     DataType.Boolean, null, "Set Auto from FP",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_ManSel",      DataType.Boolean, null, "Set Man from FP",          null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB1Block",    DataType.Boolean, null, "Block IB1 from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB2Block",    DataType.Boolean, null, "Block IB2 from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB3Block",    DataType.Boolean, null, "Block IB3 from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB4Block",    DataType.Boolean, null, "Block IB4 from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IABlock",     DataType.Boolean, null, "Block IA from FP",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_PDBlock",     DataType.Boolean, null, "Block PD from FP",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Sim",         DataType.Boolean, null, "Simulation",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Start",       DataType.Boolean, null, "Start from FP",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Stop",        DataType.Boolean, null, "Stop from FP",             null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_EnAuto",      DataType.Boolean, null, "Enable auto mode",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd0",            DataType.Boolean, null, "Command 0",                null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cmd1",            DataType.Boolean, null, "Command 1",                null, "1", true));
                    listVars.Props.Add(SetPropsType1("FBLoc",           DataType.Boolean, null, "Feedback local",           null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB0",             DataType.Boolean, null, "Feedback 0",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetSeq",          DataType.Boolean, null, "Set seq from logic",       null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetCen",          DataType.Boolean, null, "Set central from logic",   null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetAuto",         DataType.Boolean, null, "Set auto from logic",      null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetMan",          DataType.Boolean, null, "Set man from logic",       null, "1", false));
                    listVars.Props.Add(SetPropsType1("Presel",          DataType.Boolean, null, "Preselected",              null, "1", false));
                    listVars.Props.Add(SetPropsType1("ALAck",           DataType.Boolean, null, "Acknowledge on false",     null, "1", false));
                    listVars.Props.Add(SetPropsType1("IA",              DataType.Boolean, null, "Interlock auto",           null, "1", true));
                    listVars.Props.Add(SetPropsType1("FB1",             DataType.Boolean, null, "Feedback 1",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("InterlockOffCls", DataType.Boolean, null, "Combined interlock off",   null, "1", false));
                    listVars.Props.Add(SetPropsType1("InterlockOnOpn",  DataType.Boolean, null, "Combined interlock on",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_LOC",         DataType.Boolean, null, "Local from FP",            null, "1", false));

                    listVars.Props.Add(SetPropsType2("FBTime",          DataType.Integer,  null, "Feedback time",            commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 8, "INT"), null, false, false));
                    
                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr,     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IC_Text",     DataType.String, "ic text",     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB1_Text",    DataType.String, "ib1 text",    "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB2_Text",    DataType.String, "ib2 text",    "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB3_Text",    DataType.String, "ib3 text",    "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB4_Text",    DataType.String, "ib4 text",    "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IA_Text",     DataType.String, "ia text",     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("PD_Text",     DataType.String, "pd text",     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",       DataType.Boolean, null,         "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",      DataType.Boolean, null,         "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String,  null,         "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "OnOffCtrl..");
                }
    
                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass OnOff2D_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "OnOffCtrlData_2D";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "OnOffCtrlData2DOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "OnOffCtrlData2DOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W3", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W4", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W5", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 8, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Cmd0Int",         DataType.Boolean, null, "Command 0 internal",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd1IntCW",       DataType.Boolean, null, "Command 1 cw internal",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd1IntCCW",      DataType.Boolean, null, "Command 1 ccw internal",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("ICCW",            DataType.Boolean, null, "Interlock critical clockwise",             null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB1CW",           DataType.Boolean, null, "Interlock blockable clockwise",            null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB2CW",           DataType.Boolean, null, "Interlock blockable clockwise",            null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB3CW",           DataType.Boolean, null, "Interlock blockable clockwise",            null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB1CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB2CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB3CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB4CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",    null, "1", true));
                    listVars.Props.Add(SetPropsType1("IACW",            DataType.Boolean, null, "Interlock auto clockwise",                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("IACCW",           DataType.Boolean, null, "Interlock auto counter clockwise",         null, "1", true));
                    listVars.Props.Add(SetPropsType1("PDCW",            DataType.Boolean, null, "Previous drive clockwise",                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("PDCCW",           DataType.Boolean, null, "Previous drive counter clockwise",         null, "1", true));
                    listVars.Props.Add(SetPropsType1("ME",              DataType.Boolean, null, "Main power supply ok",                     null, "1", true));
                    listVars.Props.Add(SetPropsType1("Auto",            DataType.Boolean, null, "Auto",                                     null, "1", true));
                    listVars.Props.Add(SetPropsType1("Seq",             DataType.Boolean, null, "Sequence",                                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cen",             DataType.Boolean, null, "Central",                                  null, "1", true));
                    listVars.Props.Add(SetPropsType1("Loc",             DataType.Boolean, null, "Local",                                    null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_OutOfSrv",    DataType.Boolean, null, "Out of service",                           null, "1", false));
                    listVars.Props.Add(SetPropsType1("RunCW",           DataType.Boolean, null, "Run clockwise",                            null, "1", true));
                    listVars.Props.Add(SetPropsType1("RunCCW",          DataType.Boolean, null, "Run counter clockwise",                    null, "1", true));
                    listVars.Props.Add(SetPropsType1("RFSCW",           DataType.Boolean, null, "Ready for start clockwise",                null, "1", false));
                    listVars.Props.Add(SetPropsType1("RFSCCW",          DataType.Boolean, null, "Ready for start counter clockwise",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFaultCW",       DataType.Boolean, null, "Feedback fault clockwise",                 null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFaultCCW",      DataType.Boolean, null, "Feedback fault counter clockwise",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFaultStop",     DataType.Boolean, null, "Feedback fault stop",                      null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_CenSel",      DataType.Boolean, null, "Set central from FP",                      null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_AutoSel",     DataType.Boolean, null, "Set auto from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_ManSel",      DataType.Boolean, null, "Set man from FP",                          null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB1BlockCW",  DataType.Boolean, null, "Block IB1 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB2BlockCW",  DataType.Boolean, null, "Block IB2 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB3BlockCW",  DataType.Boolean, null, "Block IB3 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IABlockCW",   DataType.Boolean, null, "Block IA from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_PDBlockCW",   DataType.Boolean, null, "Block PD from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB1BlockCCW", DataType.Boolean, null, "Block IB1 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB2BlockCCW", DataType.Boolean, null, "Block IB2 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB3BlockCCW", DataType.Boolean, null, "Block IB3 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB4BlockCCW", DataType.Boolean, null, "Block IB4 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IABlockCCW",  DataType.Boolean, null, "Block IA from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_PDBlockCCW",  DataType.Boolean, null, "Block PD from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Sim",         DataType.Boolean, null, "Simulation",                               null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_StartCW",     DataType.Boolean, null, "Start CW from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_StartCCW",    DataType.Boolean, null, "Start CCW from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Stop",        DataType.Boolean, null, "Stop from FP",                             null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_EnAuto",      DataType.Boolean, null, "Enable auto mode",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd0",            DataType.Boolean, null, "Command 0",                                null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cmd1CW",          DataType.Boolean, null, "Command 1 clockwise",                      null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cmd1CCW",         DataType.Boolean, null, "Command 1 counter clockwise",              null, "1", true));
                    listVars.Props.Add(SetPropsType1("FBLoc",           DataType.Boolean, null, "Feedback local",                           null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB0",             DataType.Boolean, null, "Feedback 0",                               null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB1CW",           DataType.Boolean, null, "Feedback 1 clockwise",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB1CCW",          DataType.Boolean, null, "Feedback 1 counter clockwise",             null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetSeq",          DataType.Boolean, null, "Set Seq from logic",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("SeqStartCmd",     DataType.Boolean, null, "Sequence start command",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("SeqStopCmd",      DataType.Boolean, null, "Sequence stop command",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetCen",          DataType.Boolean, null, "Set Cen from logic",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetAuto",         DataType.Boolean, null, "Set Auto from logic",                      null, "1", false));
                    listVars.Props.Add(SetPropsType1("ALAck",           DataType.Boolean, null, "Acknowledge on false",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("IB4CW",           DataType.Boolean, null, "Interlock blockable 4 clockwise",          null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB4BlockCW",  DataType.Boolean, null, "Block IB4 from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_LOC",         DataType.Boolean, null, "Set Local from FP",                        null, "1", false));
                    listVars.Props.Add(SetPropsType1("ICCCW",           DataType.Boolean, null, "Interlock critical counter clockwise",     null, "1", true));

                    listVars.Props.Add(SetPropsType2("SetMan",          DataType.Boolean, null, "Set manual from logic",                    commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 10.1, "X"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("FBTime",          DataType.Integer, null, "Feedback time",                            commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 12, "INT"), null, false, false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr,     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("ICCW_text",   DataType.String, "ICCW text",   "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB1CW_text",  DataType.String, "IB1CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB2CW_text",  DataType.String, "IB2CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB3CW_text",  DataType.String, "IB3CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB4CW_text",  DataType.String, "IB4CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IACW_text",   DataType.String, "IACW text",   "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("PDCW_text",   DataType.String, "PDCW text",   "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("ICCCW_text",  DataType.String, "ICCCW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB1CCW_text", DataType.String, "IB1CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB2CCW_text", DataType.String, "IB2CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB3CCW_text", DataType.String, "IB3CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB4CCW_text", DataType.String, "IB4CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IACCW_text",  DataType.String, "IACCW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("PDCCW_text",  DataType.String, "PDCCW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",       DataType.Boolean, null,         "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",      DataType.Boolean, null,         "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String,  null,         "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "OnOffCtrl2D..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass OnOffVSD_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "OnOffCtrlData_VSD";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "OnOffCtrlDataVSDOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "OnOffCtrlDataVSDOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();
                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W3", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W4", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W5", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 8, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W6", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 10, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W7", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 14, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Cmd0Int",         DataType.Boolean, null, "Command 0 internal",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd1IntCW",       DataType.Boolean, null, "Command 1 cw internal",                null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd1IntCCW",      DataType.Boolean, null, "Command 1 ccw internal",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB1CCW",          DataType.Boolean, null, "Feedback 1 counter clockwise",         null, "1", true));
                    listVars.Props.Add(SetPropsType1("ICCW",            DataType.Boolean, null, "Interlock critical clockwise",         null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB1CW",           DataType.Boolean, null, "Interlock blockable clockwise",        null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB2CW",           DataType.Boolean, null, "Interlock blockable clockwise",        null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB4CW",           DataType.Boolean, null, "Interlock blockable clockwise",        null, "1", true));                    
                    listVars.Props.Add(SetPropsType1("IB1CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB2CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB3CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",null, "1", true));
                    listVars.Props.Add(SetPropsType1("IB4CCW",          DataType.Boolean, null, "Interlock blockable counter clockwise",null, "1", true));
                    listVars.Props.Add(SetPropsType1("IACW",            DataType.Boolean, null, "Interlock auto clockwise",             null, "1", true));
                    listVars.Props.Add(SetPropsType1("IACCW",           DataType.Boolean, null, "Interlock auto counter clockwise",     null, "1", true));
                    listVars.Props.Add(SetPropsType1("PDCW",            DataType.Boolean, null, "Previous drive clockwise",             null, "1", true));
                    listVars.Props.Add(SetPropsType1("PDCCW",           DataType.Boolean, null, "Previous drive counter clockwise",     null, "1", true));
                    listVars.Props.Add(SetPropsType1("ME",              DataType.Boolean, null, "Main power supply ok",                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("Auto",            DataType.Boolean, null, "Auto",                                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("Seq",             DataType.Boolean, null, "Sequence",                             null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cen",             DataType.Boolean, null, "Central",                              null, "1", true));
                    listVars.Props.Add(SetPropsType1("Loc",             DataType.Boolean, null, "Local",                                null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_OutOfSrv",    DataType.Boolean, null, "Out of service",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("RunCW",           DataType.Boolean, null, "Run clockwise",                        null, "1", true));
                    listVars.Props.Add(SetPropsType1("RunCCW",          DataType.Boolean, null, "Run counter clockwise",                null, "1", true));
                    listVars.Props.Add(SetPropsType1("RFSCW",           DataType.Boolean, null, "Ready for start clockwise",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("RFSCCW",          DataType.Boolean, null, "Ready for start counter clockwise",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFaultCW",       DataType.Boolean, null, "Feedback fault clockwise",             null, "1", false));
                    listVars.Props.Add(SetPropsType1("FBFaultCCW",      DataType.Boolean, null, "Feedback fault counter clockwise",     null, "1", false));                 
                    listVars.Props.Add(SetPropsType1("HMI_CenSel",      DataType.Boolean, null, "Set central from FP",                  null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_AutoSel",     DataType.Boolean, null, "Set auto from FP",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_ManSel",      DataType.Boolean, null, "Set man from FP",                      null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB1BlockCW",  DataType.Boolean, null, "Block IB1 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB2BlockCW",  DataType.Boolean, null, "Block IB2 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB3BlockCW",  DataType.Boolean, null, "Block IB3 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB4BlockCW",  DataType.Boolean, null, "Block IB4 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_PDBlockCW",   DataType.Boolean, null, "Block PD from FP",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB1BlockCCW", DataType.Boolean, null, "Block IB1 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB2BlockCCW", DataType.Boolean, null, "Block IB2 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB3BlockCCW", DataType.Boolean, null, "Block IB3 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IB4BlockCCW", DataType.Boolean, null, "Block IB4 from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IABlockCCW",  DataType.Boolean, null, "Block IA from FP",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_PDBlockCCW",  DataType.Boolean, null, "Block PD from FP",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Sim",         DataType.Boolean, null, "Simulation",                           null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_StartCW",     DataType.Boolean, null, "Start CW from FP",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_StartCCW",    DataType.Boolean, null, "Start CCW from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd1CCW",         DataType.Boolean, null, "Command 1 counter clockwise",          null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_Stop",        DataType.Boolean, null, "Stop from FP",                         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_EnAuto",      DataType.Boolean, null, "Enable auto mode",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_VSDReset",    DataType.Boolean, null, "VSD reset from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("Cmd0",            DataType.Boolean, null, "Command 0",                            null, "1", true));
                    listVars.Props.Add(SetPropsType1("Cmd1CW",          DataType.Boolean, null, "Command 1 clockwise",                  null, "1", true));
                    listVars.Props.Add(SetPropsType1("FBLoc",           DataType.Boolean, null, "Feedback local",                       null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB0",             DataType.Boolean, null, "Feedback 0",                           null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB1CW",           DataType.Boolean, null, "Feedback 1 clockwise",                 null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetSeq",          DataType.Boolean, null, "Set Seq from logic",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("SeqStartCmd",     DataType.Boolean, null, "Sequence start command",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("SeqStopCmd",      DataType.Boolean, null, "Sequence stop command",                null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetCen",          DataType.Boolean, null, "Set Cen from logic",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetAuto",         DataType.Boolean, null, "Set Auto from logic",                  null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetMan",          DataType.Boolean, null, "Set manual from logic",                null, "1", false));
                    listVars.Props.Add(SetPropsType1("ALAck",           DataType.Boolean, null, "Acknowledge on false",                 null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_IABlockCW",   DataType.Boolean, null, "Block IA",                             null, "1", false));                 
                    listVars.Props.Add(SetPropsType1("HMI_InSV",        DataType.Boolean, null, "Set Internal setpoint from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_ExtSV",       DataType.Boolean, null, "Set External setpoint from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("InSV",            DataType.Boolean, null, "Ind Internal setpoint from FP",        null, "1", true));
                    listVars.Props.Add(SetPropsType1("ExtSV",           DataType.Boolean, null, "Ind External setpoint from logic",     null, "1", true));
                    listVars.Props.Add(SetPropsType1("SetExtSV",        DataType.Boolean, null, "Set Internal setpoint from logic",     null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetInSV",         DataType.Boolean, null, "Set External setpoint from logic",     null, "1", false));
                    listVars.Props.Add(SetPropsType1("IB3CW",           DataType.Boolean, null, "Interlock blockable IB3",              null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_LOC",         DataType.Boolean, null, "Set Local from FP",                    null, "1", false));
                    listVars.Props.Add(SetPropsType1("ICCCW",           DataType.Boolean, null, "Interlock critical counter clockwise", null, "1", true));
                    listVars.Props.Add(SetPropsType1("Comd",            DataType.Boolean, null, "VSD link - command",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("Dir",             DataType.Boolean, null, "VSD link - direction",                 null, "1", false));
                    listVars.Props.Add(SetPropsType1("Reset",           DataType.Boolean, null, "VSD link - reset",                     null, "1", false));
                    listVars.Props.Add(SetPropsType1("Interlock",       DataType.Boolean, null, "VSD link - interlock",                 null, "1", false));
                    listVars.Props.Add(SetPropsType1("Ready_to_start",  DataType.Boolean, null, "VSD link - ready to start",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("FB",              DataType.Boolean, null, "VSD link - feedback",                  null, "1", false));
                    listVars.Props.Add(SetPropsType1("DirFB",           DataType.Boolean, null, "VSD link - direction feedback",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("DriveFault",      DataType.Boolean, null, "VSD link - drive fault",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("DriveWar",        DataType.Boolean, null, "VSD link - drive warning",             null, "1", true));
                    listVars.Props.Add(SetPropsType1("DriveLoc",        DataType.Boolean, null, "VSD link - drive local",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("ComFault",        DataType.Boolean, null, "VSD link - communication fault",       null, "1", false));

                    listVars.Props.Add(SetPropsType2("FBTime",          DataType.Integer, null, "Feedback time",                        commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 12, "INT"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_PV",          DataType.Double,  null, "VSD link - process value",             commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 20, "REAL"), null, false, true, "TrendExtNameHMI_PV"));
                    listVars.Props.Add(SetPropsType2("HMI_SV",          DataType.Double,  null, "VSD link - setpoint value",            commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 16, "REAL"), "1", false, true, "TrendExtNameHMI_SV"));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr,     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("ICCW_text",   DataType.String, "ICCW text",   "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB1CW_text",  DataType.String, "IB1CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB2CW_text",  DataType.String, "IB2CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB3CW_text",  DataType.String, "IB3CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB4CW_text",  DataType.String, "IB4CW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IACW_text",   DataType.String, "IACW text",   "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("PDCW_text",   DataType.String, "PDCW text",   "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("ICCCW_text",  DataType.String, "ICCCW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB1CCW_text", DataType.String, "IB1CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB2CCW_text", DataType.String, "IB2CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB3CCW_text", DataType.String, "IB3CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IB4CCW_text", DataType.String, "IB4CCW text", "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("IACCW_text",  DataType.String, "IACCW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("PDCCW_text",  DataType.String, "PDCCW text",  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",       DataType.Boolean, null,         "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",      DataType.Boolean, null,         "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String,  null,         "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "OnOffCtrlVSD..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass PID_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "PIDCtrlData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "PIDCtrlDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();
                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("OUT_Seq",         DataType.Boolean, null, "Sequence",                     null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Cen",         DataType.Boolean, null, "Central",                      null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_E1",          DataType.Boolean, null, "E1",                           null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_E2",          DataType.Boolean, null, "E2",                           null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Track",       DataType.Boolean, null, "Track",                        null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Auto",        DataType.Boolean, null, "Auto",                         null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Man",         DataType.Boolean, null, "Manual",                       null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_ManSel",      DataType.Boolean, null, "Set manual from FP",           null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_E1Sel",       DataType.Boolean, null, "Set E1 from FP",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_E2Sel",       DataType.Boolean, null, "Set E2 from FP",               null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Track",       DataType.Boolean, null, "Set track from FP",            null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_CenSel",      DataType.Boolean, null, "Set central from FP",          null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_AutoSel",     DataType.Boolean, null, "Set auto from FP",             null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Sim",         DataType.Boolean, null, "Simulation",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Dir",         DataType.Boolean, null, "Set direction from FP",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Apply",       DataType.Boolean, null, "Apply PID parameters from FP", null, "1", false));

                    listVars.Props.Add(SetPropsType2("HMI_PIDType",     DataType.Integer, null, "PID type from FP",              commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), null, false, false));
                    listVars.Props.Add(SetPropsType2("ActSP",           DataType.Double, null, "Actual setpoint",               commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "REAL"), "1", false, true, "TrendExtNameActSP"));
                    listVars.Props.Add(SetPropsType2("ActPV",           DataType.Double, null, "Actual process value",          commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 10, "REAL"), null, false, true, "TrendExtNameActPV"));
                    listVars.Props.Add(SetPropsType2("ValueOut",        DataType.Double, null, "Value out",                     commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 14, "REAL"), "1", false, true, "TrendExtNameValueOut"));
                    listVars.Props.Add(SetPropsType2("HMI_PIDOutMax",   DataType.Double, null, "Limit max for value out",       commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 18, "REAL"),  "1", false, false));
                    listVars.Props.Add(SetPropsType2("HMI_PIDOutMin",   DataType.Double, null, "Limit min for value out",       commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 22, "REAL"),  "1", false, false));
                    listVars.Props.Add(SetPropsType2("HMI_Ti",          DataType.Double, null, "Integration constant from FP",  commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 26, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_Td",          DataType.Double, null, "Derivation constant from FP",   commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 30, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_DB",          DataType.Double, null, "Deadband from FP",              commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 34, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_Gain",        DataType.Double, null, "Gain from FP",                  commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 38, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("Min",             DataType.Double, null, "Process value min",             commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 102, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("Max",             DataType.Double, null, "Process value max",             commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 106, "REAL"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Unit",        DataType.String, tag.Unit,  "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String, "",        "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "PIDCtrl..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass PIDStep_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "PIDStepCtrlData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "PIDStepCtrlDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();
                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("OUT_Seq", DataType.Boolean, null, "Sequence", null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Cen", DataType.Boolean, null, "Central", null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_E1", DataType.Boolean, null, "E1", null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_E2", DataType.Boolean, null, "E2", null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Track", DataType.Boolean, null, "Track", null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Auto", DataType.Boolean, null, "Auto", null, "1", true));
                    listVars.Props.Add(SetPropsType1("OUT_Man", DataType.Boolean, null, "Manual", null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_ManSel", DataType.Boolean, null, "Set manual from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_E1Sel", DataType.Boolean, null, "Set E1 from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_E2Sel", DataType.Boolean, null, "Set E2 from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Track", DataType.Boolean, null, "Set track from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_CenSel", DataType.Boolean, null, "Set central from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_AutoSel", DataType.Boolean, null, "Set auto from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Sim", DataType.Boolean, null, "Simulation", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Dir", DataType.Boolean, null, "Set direction from FP", null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_Apply", DataType.Boolean, null, "Apply PID parameters from FP", null, "1", false));

                    listVars.Props.Add(SetPropsType2("OUT_QLMNUP", DataType.Boolean, null, "Pulse up", null, null, "1", true, true, "TrendExtNameQLMNUP"));
                    listVars.Props.Add(SetPropsType2("OUT_QLMNDN", DataType.Boolean, null, "Pulse down", null, null, "1", true, true, "TrendExtNameQLMNDN"));

                    listVars.Props.Add(SetPropsType1("HMI_ValveOpen", DataType.Boolean, null, "Manual command Open Valve", null, "1", true));
                    listVars.Props.Add(SetPropsType1("HMI_ValveClose", DataType.Boolean, null, "Manual command Close Valve", null, "1", true));

                    listVars.Props.Add(SetPropsType2("HMI_PIDType", DataType.Integer, null, "PID type from FP", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), null, false, false));
                    listVars.Props.Add(SetPropsType2("ActSP", DataType.Double, null, "Actual setpoint", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "REAL"), "1", false, true, "TrendExtNameActSP"));
                    listVars.Props.Add(SetPropsType2("ActPV", DataType.Double, null, "Actual process value", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 10, "REAL"), null, false, true, "TrendExtNameActPV"));
                    listVars.Props.Add(SetPropsType2("HMI_MTR", DataType.Double, null, "Manipulate time", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 14, "REAL"), null, true, false));
                    listVars.Props.Add(SetPropsType2("HMI_Break", DataType.Double, null, "Minimum break time", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 18, "REAL"), null, true, false));
                    listVars.Props.Add(SetPropsType2("HMI_Pulse", DataType.Double, null, "Minimum pulse time", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 22, "REAL"), null, true, false));
                    listVars.Props.Add(SetPropsType2("HMI_Ti", DataType.Double, null, "Integration constant from FP", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 26, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_Td", DataType.Double, null, "Derivation constant from FP", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 30, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_DB", DataType.Double, null, "Deadband from FP", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 34, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HMI_Gain", DataType.Double, null, "Gain from FP", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 38, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("Min", DataType.Double, null, "Process value min", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 102, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("Max", DataType.Double, null, "Process value max", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 106, "REAL"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Unit", DataType.String, tag.Unit, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note", DataType.String, "", "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "PIDStepCtrl..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Presel_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "PreselData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "PreselDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();
                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Presel",          DataType.Boolean, null, "Preselection",                 null, "1", true));
                    listVars.Props.Add(SetPropsType1("PreValid",        DataType.Boolean, null, "Preselection valid",           null, "1", false));
                    listVars.Props.Add(SetPropsType1("SetP",            DataType.Boolean, null, "Set presel",                   null, "1", false));
                    listVars.Props.Add(SetPropsType1("ResP",            DataType.Boolean, null, "Reset presel",                 null, "1", false));
                    listVars.Props.Add(SetPropsType1("PreselEn",        DataType.Boolean, null, "Preselection enabled",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("HMI_PreselEn",    DataType.Boolean, null, "Preselection enabled from FP", null, "1", false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String, "",        "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "PreselCtrl..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Ain_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "AinData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm.Equals(" ") ? "/Comm/Data" : tag.S7Comm;
                    
                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "AinDataOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "AinDataOnItemAfterWriteEvent")));
                    subObj.Events.Event.Add(SetEvent("onStop", "Pm", Script(@"Scripts\PmDataScripts.xml", "AinDataOnStopEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();

                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("W2", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("AlarmHH",         DataType.Boolean,   null, "Alarm active",               "", "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmH",          DataType.Boolean,   null, "Alarm active",               "", "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmL",          DataType.Boolean,   null, "Alarm active",               "", "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmLL",         DataType.Boolean,   null, "Alarm active",               "", "1", false));
                    listVars.Props.Add(SetPropsType1("HWSigFault",      DataType.Boolean,   null, "HW Fault Alarm active",      "", "1", false));
                    listVars.Props.Add(SetPropsType1("HWSigFaultInhibit",DataType.Boolean,  null, "HW Fault Alarm active",      "", "1", false));
                    listVars.Props.Add(SetPropsType1("Force",           DataType.Boolean,   null, "Value is forced from FP",    "", "1", true));
                    listVars.Props.Add(SetPropsType1("AlarmLInhibit",   DataType.Boolean,   null, "Alarm L block",              "", "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmLLInhibit",  DataType.Boolean,   null, "Alarm LL block",             "", "1", false));
                    listVars.Props.Add(SetPropsType1("HH_ALEn",         DataType.Boolean,   null, "If true HMI generates alarm", "", "1", false));
                    listVars.Props.Add(SetPropsType1("H_ALEn",          DataType.Boolean,   null, "If true HMI generates alarm", "", "1", false));
                    listVars.Props.Add(SetPropsType1("L_ALEn",          DataType.Boolean,   null, "If true HMI generates alarm", "", "1", false));
                    listVars.Props.Add(SetPropsType1("LL_ALEn",         DataType.Boolean,   null, "If true HMI generates alarm", "", "1", false));
                    listVars.Props.Add(SetPropsType1("HH_En",           DataType.Boolean,   null, "If true HMI generates event", "", "1", false));
                    listVars.Props.Add(SetPropsType1("H_En",            DataType.Boolean,   null, "If true HMI generates event", "", "1", false));
                    listVars.Props.Add(SetPropsType1("L_En",            DataType.Boolean,   null, "If true HMI generates event", "", "1", false));
                    listVars.Props.Add(SetPropsType1("LL_En",           DataType.Boolean,   null, "If true HMI generates event", "", "1", false));
                    listVars.Props.Add(SetPropsType1("HH_ALAck",        DataType.Boolean,   null, "Alarm acknowledge on false", "", "1", false));
                    listVars.Props.Add(SetPropsType1("H_ALAck",         DataType.Boolean,   null, "Alarm acknowledge on false", "", "1", false));
                    listVars.Props.Add(SetPropsType1("L_ALAck",         DataType.Boolean,   null, "Alarm acknowledge on false", "", "1", false));
                    listVars.Props.Add(SetPropsType1("LL_ALAck",        DataType.Boolean,   null, "Alarm acknowledge on false", "", "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmHHInhibit",  DataType.Boolean,   null, "Alarm HH block",             "", "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmHInhibit",   DataType.Boolean,   null, "Alarm H block",              "", "1", false));

                    listVars.Props.Add(SetPropsType2("AlarmDelay",      DataType.Integer,    null, "Alarm delay",                commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 4, "INT"), null, false, false));
                    listVars.Props.Add(SetPropsType2("HWSig",           DataType.Integer,    null, "Real hw signal on channel",  commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 6, "INT"), null, false, false));
                    listVars.Props.Add(SetPropsType2("Value",           DataType.Double,    null, "Value",                      commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 8, "REAL"), null, false, true));
                    listVars.Props.Add(SetPropsType2("Min",             DataType.Double,    "0",  "Min",                        commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 12, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("Max",             DataType.Double,    "10", "Max",                        commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 16, "REAL"), "1", false, false));
                    listVars.Props.Add(SetPropsType2("AELevelHH",       DataType.Double,    null, "Alarm limit",                commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 20, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("AELevelH",        DataType.Double,    null, "Alarm limit",                commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 24, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("AELevelL",        DataType.Double,    null, "Alarm limit",                commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 28, "REAL"), null, false, false));
                    listVars.Props.Add(SetPropsType2("AELevelLL",       DataType.Double,    null, "Alarm limit",                commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 32, "REAL"), null, false, false));

                    listVars.Props.Add(SetPropsType1("HWSigFault_ALAck",DataType.Boolean,   null,       "Alarm acknowledge on false - HMI only",    null, "1", false));
                    listVars.Props.Add(SetPropsType1("Description",     DataType.String,    tag.Descr,  "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("Unit",            DataType.String,    tag.Unit,   "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",           DataType.Boolean,   null,       "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",          DataType.Boolean,   null,       "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyInh",          DataType.Boolean,   null,       "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",             DataType.String,   null,       "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("barGraphColor1",   DataType.String,   "#30ccff",  "HMI only",                                 null, null, false));
                    listVars.Props.Add(SetPropsType1("barGraphColor2",   DataType.String,   "#a8ccf0",  "HMI only",                                 null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "Ain..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PromoticClass.PmObjectClass Din_PmFolder(List<BasicObject> data)
        {
            if (data.Count > 0)
            {
                PromoticClass.PmObjectClass obj = new PromoticClass.PmObjectClass();
                obj.Name = "DinData";
                obj.Type = "PmFolder";
                obj.PmObject = new List<PromoticClass.PmObjectClass>();

                string commPath = null;

                foreach (BasicObject tag in data)
                {
                    commPath = tag.S7Comm;

                    PromoticClass.PmObjectClass subObj = new PromoticClass.PmObjectClass();
                    obj.PmObject.Add(subObj);

                    subObj.Name = tag.TagName;
                    subObj.Type = "PmData";

                    //events
                    subObj.Events = new PromoticClass.EventsClass();
                    subObj.Events.Name = "PmEvents";
                    subObj.Events.Event = new List<PromoticClass.EventClass>();

                    subObj.Events.Event.Add(SetEvent("onStart", "Pm", Script(@"Scripts\PmDataScripts.xml", "DinDataOnStartEvent")));
                    subObj.Events.Event.Add(SetEvent("onItemAfterWrite", "Pm", Script(@"Scripts\PmDataScripts.xml", "DinDataOnItemAfterWriteEvent")));

                    //vars
                    subObj.List = new List<PromoticClass.ListClass>();

                    PromoticClass.ListClass listVars = new PromoticClass.ListClass();
                    listVars.Name = "Vars";
                    subObj.List.Add(listVars);

                    listVars.Props = new List<PromoticClass.PropsClass>();
                    listVars.Props.Add(SetPropsType2("W1", DataType.Integer, null, "Comm word", commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 0, "INT"), "1", false, false));

                    listVars.Props.Add(SetPropsType1("Alarm",           DataType.Boolean, null, "Alarm active",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("Force",           DataType.Boolean, null, "Forced",               null, "1", true));
                    listVars.Props.Add(SetPropsType1("NormalState",     DataType.Boolean, null, "Normal state",         null, "1", false));
                    listVars.Props.Add(SetPropsType1("Value",           DataType.Boolean, null, "Value",                null, "1", true));
                    listVars.Props.Add(SetPropsType1("HWSig",           DataType.Boolean, null, "Channel value",        null, "1", false));
                    listVars.Props.Add(SetPropsType1("AlarmInhibit",    DataType.Boolean, null, "Alarm block",          null, "1", false));
                    listVars.Props.Add(SetPropsType1("ALAck",           DataType.Boolean, null, "Acknowladge on false", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Sig_ALEn",        DataType.Boolean, null, "Enable alarm",         null, "1", false));

                    listVars.Props.Add(SetPropsType2("AlarmDelay",      DataType.Integer,  null, "Alarm delay",         commPath, BuildS7Conn(tag.DBNr, tag.PositionInDB, 2, "INT"), null, false, false));

                    listVars.Props.Add(SetPropsType1("Description", DataType.String, tag.Descr, "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAl",       DataType.Boolean, null,     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("AnyAck",      DataType.Boolean, null,     "HMI only", null, null, false));
                    listVars.Props.Add(SetPropsType1("Note",        DataType.String, null,      "HMI only", null, null, false));

                    listVars.Props.Add(SetPropsType1("Save", DataType.Boolean, "", "Save values to config file", null, "1", false));
                    listVars.Props.Add(SetPropsType1("Load", DataType.Boolean, "", "Load values to config file", null, "1", false));

                    progStatus = (progStatus + progStep);
                    UpdateStatus(progStatus, "Din..");
                }

                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Builds string s7 communication
        /// </summary>
        /// <param name="dbnr"></param>
        /// <param name="posindb"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string BuildS7Conn(string dbnr, string posindb, double offset, string type)
        {
            string address = null;
            try
            {
                if (type == "X")
                {
                    address = String.Format("{0:0.0}", (double.Parse(posindb) + offset));
                    address = address.Replace(',', '.');
                }
                else
                {
                    address = (double.Parse(posindb) + offset).ToString();
                }

                return "DB" + dbnr + "," + type + address;
            }
            catch
            {
                return "null";
            }
        }

        #endregion
    }

    class TagList
    {
        public List<string> tag = new List<string>();
    }

    class AlarmStripesData
    {
        public List<AlarmStripePanel> Panel = new List<AlarmStripePanel>();
    }

    class AlarmStripePanel
    {
        public string Name;
        public List<string> Tag = new List<string>();
    }
}
