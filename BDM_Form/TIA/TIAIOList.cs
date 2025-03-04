using System.Collections.Generic;
using System.Xml.Serialization;

namespace TIAIFCNS
{
    //[Serializable]
    [XmlRoot("Document")]
    public class TIAIOList
    {
        private int ID;
        [XmlElement("SW.ControllerTagTable")]
        public XMLOMTSWC ControllerTagTable;
        [XmlElement("ObjectList")]
        public XMLOMTSWC ObjectList;
        public TIAIOList()
        {
            ControllerTagTable = new XMLOMTSWC();
            ControllerTagTable.ID = "0";
            ControllerTagTable.AttributeList = new XMLAttributeList();
            ControllerTagTable.AttributeList.Name = "IOTable";
            ControllerTagTable.ObjectList = new XMLOMTSWC();
            ControllerTagTable.ObjectList.SWControllerTag = new List<XMLOMTSWC>();

        }
        #region Create new record in IO list method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TagName"> New record tag name</param>
        /// <param name="IOaddress">New record physical address</param>
        /// <param name="DataType">New record data type</param>
        /// <param name="Description">New record description</param>
        public void AddTag(string TagName, string IOaddress, string DataType, string Description)
        {
            XMLOMTSWC NewIORecord = new XMLOMTSWC();
            NewIORecord = new XMLOMTSWC();
            ID = ID + 1;
            NewIORecord.ID = ID.ToString();
            NewIORecord.AggregationName = "Tags";
            //New record Attribute list inicialization
            NewIORecord.AttributeList = new XMLAttributeList();
            NewIORecord.AttributeList.IOAddres = "%" + IOaddress;
            NewIORecord.AttributeList.Name = TagName;
            //New record Link List inicialization
            NewIORecord.LinkList = new XMLLinkList();
            NewIORecord.LinkList.DataType = new XMLDataType();
            NewIORecord.LinkList.DataType.DataType = DataType;
            //New record Object list inicialization
            NewIORecord.ObjectList = new XMLOMTSWC();
            NewIORecord.ObjectList.MultilingualText = new XMLOMTSWC();
            NewIORecord.ObjectList.MultilingualText.ID = "A" + ID.ToString();
            NewIORecord.ObjectList.MultilingualText.AggregationName = "Comment";
            NewIORecord.ObjectList.MultilingualText.AttributeList = new XMLAttributeList();
            NewIORecord.ObjectList.MultilingualText.AttributeList.TextItems = new XMLTextItems();
            NewIORecord.ObjectList.MultilingualText.AttributeList.TextItems.Value = new XMLValue();
            NewIORecord.ObjectList.MultilingualText.AttributeList.TextItems.Value.Description = Description;
            NewIORecord.ObjectList.MultilingualText.AttributeList.TextItems.Value.lang = "en-US";
            this.ControllerTagTable.ObjectList.SWControllerTag.Add(NewIORecord);
        }
        #endregion
    }

    #region ObjectList, MulstilingualText, SW.ControllerTag, SW.ControllerTagTable and  XML element
    /// <summary>
    /// ObjectList, MulstilingualText, SW.ControllerTag, SW.ControllerTagTable  XML element
    /// </summary>
    public class XMLOMTSWC
    {
        [XmlAttribute]
        public string ID;
        [XmlAttribute]
        public string AggregationName;
        [XmlElement("AttributeList")]
        public XMLAttributeList AttributeList;
        [XmlElement("LinkList")]
        public XMLLinkList LinkList;
        [XmlElement("SW.ControllerTag")]
        public List<XMLOMTSWC> SWControllerTag;
        [XmlElement("SW.ControllerTagTable")]
        public XMLOMTSWC SWControllerTagTable;
        [XmlElement("ObjectList")]
        public XMLOMTSWC ObjectList;
        [XmlElement("MultilingualText")]
        public XMLOMTSWC MultilingualText;
    }
    #endregion

    #region AttributeList XML element
    /// <summary>
    /// AttributeList XML element
    /// </summary>
    public class XMLAttributeList
    {
        [XmlElement("LogicalAddress")]
        public string IOAddres;
        [XmlElement("Name")]
        public string Name;
        [XmlElement("TextItems")]
        public XMLTextItems TextItems;
        public XMLAttributeList()
        {
            //TextItems = new XMLTextItems();
        }
    }
    #endregion

    #region TextItems XML element
    /// <summary>
    /// TextItems XML element
    /// </summary>
    public class XMLTextItems
    {
        [XmlElement("Value")]
        public XMLValue Value;
        public XMLTextItems()
        {
            //Value = new XMLValue();
        }
    }
    #endregion

    #region Value XML element
    /// <summary>
    /// Value XML element
    /// </summary>
    public class XMLValue
    {
        [XmlAttribute("lang")]
        public string lang { get; set; }
        [XmlText]
        public string Description { get; set; }
        public XMLValue()
        {
            lang = "en-US";
        }

    }
    #endregion

    #region DataType XML element
    /// <summary>
    /// DataType XML element
    /// </summary>
    public class XMLDataType
    {
        [XmlAttribute("TargetID")]
        public string TargedID;
        [XmlElement("Name")]
        public string DataType;
        public XMLDataType()
        {
            this.TargedID = "@OpenLink";
        }
    }
    #endregion

    #region LinkList XML element
    /// <summary>
    /// LinkList XML element
    /// </summary>
    public class XMLLinkList
    {
        [XmlElement("DataType")]
        public XMLDataType DataType;
    }
    #endregion
}
