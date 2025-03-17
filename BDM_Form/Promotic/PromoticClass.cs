using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace BDM_Form
{
    //Level - 1
    [Serializable]
    [XmlRoot("Document")]
    public class PromoticClass
    {
        [XmlElement("Cfg")]
        public CfgClass Cfg = new CfgClass();

        //Level - 2
        public class CfgClass
        {
            [XmlAttribute("Name")]
            public string CfgName = "Default";
            [XmlElement("Content")]
            public ContentClass Content = new ContentClass();
        }

        //Level - 3
        public class ContentClass
        {
            [XmlAttribute("ver")]
            public string Ver { get; set; }
            [XmlElement("Props")]
            public List<PropsClass> Props { get; set; }
            [XmlElement]
            public PmObjectsClass PmObjects { get; set; }
            [XmlElement]
            public List<PmObjectClass> PmObject { get; set; }
        }

        //ListClass
        public class ListClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlElement]
            public List<PropsClass> Props { get; set; }
        }

        //PmObjects : basic object
        public class PmObjectsClass
        {
            [XmlElement]
            public List<PmObjectClass> PmObject { get; set; }
        }

        //PmObject : basic object
        public class PmObjectClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlElement]
            public List<PropClass> Prop { get; set; }
            [XmlElement]
            public List<PropsClass> Props { get; set; }
            [XmlElement]
            public EventsClass Events { get; set; }
            [XmlElement]
            public MethodsClass Methods { get; set; }
            [XmlElement]
            public List<ListClass> List { get; set; }
            [XmlElement]
            public List<PmObjectClass> PmObject { get; set; }
            [XmlElement]
            public GPanelClass GPanel { get; set; }
        }

        //Props : basic object
        public class PropsClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlElement]
            public List<PropClass> Prop { get; set; }
            [XmlElement]
            public List<ListClass> List { get; set; }
            [XmlElement]
            public List<PropsClass> Props { get; set; }
        }

        //Prop : basic object
        public class PropClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlText]
            public string Value { get; set; }
            [XmlElement]
            public DStaticClass DStatic { get; set; }
            [XmlElement]
            public DBindClass DBind { get; set; }
        }

        //DStatic : basic object
        public class DStaticClass
        {
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlText]
            public string Value { get; set; }
        }

        //DBind : basic object
        public class DBindClass
        {
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlElement]
            public PropClass Prop { get; set; }
        }

        //GPanel : basic object
        public class GPanelClass
        {
            [XmlElement]
            public List<PropClass> Prop { get; set; }
            [XmlElement]
            public List<PropsClass> Props { get; set; }
            [XmlElement]
            public EventsClass Events { get; set; }
            [XmlElement]
            public List<EventClass> Event { get; set; }
            [XmlElement]
            public MethodsClass Methods { get; set; }
            [XmlElement]
            public List<GItemClass> GItem { get; set; }
        }

        //GItem : basic object
        public class GItemClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlElement]
            public List<PropClass> Prop { get; set; }
            [XmlElement]
            public List<PropsClass> Props { get; set; }
            [XmlElement]
            public List<EventClass> Event { get; set; }
            [XmlElement]
            public List<MethodsClass> Methods { get; set; }
        }

        //Events : basic object
        public class EventsClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; } = "PmEvents";
            [XmlElement]
            public List<EventClass> Event { get; set; }
        }

        //Event : basic object
        public class EventClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Type")]
            public string Type { get; set; }
            [XmlElement("Script")]
            public ScriptClass Script = new ScriptClass();
        }

        //Methods : basic object
        public class MethodsClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlElement]
            public List<MethodClass> Method { get; set; }
        }

        //Method : basic object
        public class MethodClass
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlElement("Prop")]
            public PropClass Prop = new PropClass();
            [XmlElement("Script")]
            public ScriptClass Script = new ScriptClass();
        }

        //Script : basic object
        public class ScriptClass
        {
            [XmlIgnore]
            public string Content { get; set; }

            [XmlText]
            public XmlNode[] CDataContent
            {
                get { var dummy = new XmlDocument(); return new XmlNode[] { dummy.CreateCDataSection(Content) }; }
                set { Content = value[0].Value; }
            }
        }
    }

    #region Constants

    public static class UserType
    {

        public static string Local { get; } = "Local";
        public static string Net { get; } = "Net";
        public static string LocalNet { get; } = "Local,Net";

    }

    public static class DataType
    {

        public static string Boolean { get; } = "Boolean";
        public static string Byte { get; } = "Byte";
        public static string Integer { get; } = "Integer";
        public static string Long { get; } = "Long";
        public static string Single { get; } = "Single";
        public static string Double { get; } = "Double";
        public static string String { get; } = "String";
        public static string Date { get; } = "Date";
        public static string Object { get; } = "Object";
        public static string Variant { get; } = "Variant";
    }

    #endregion
}
