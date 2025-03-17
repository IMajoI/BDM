using BDMdata;

namespace BDM
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ribbon));
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.MainGroup = this.Factory.CreateRibbonGroup();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.TIAGroup = this.Factory.CreateRibbonGroup();
            this.S7Group = this.Factory.CreateRibbonGroup();
            this.ArchestraGroup = this.Factory.CreateRibbonGroup();
            this.PGroup = this.Factory.CreateRibbonGroup();
            this.OnOffBtn = this.Factory.CreateRibbonToggleButton();
            this.SetHeadBtn = this.Factory.CreateRibbonButton();
            this.SetDataBtn = this.Factory.CreateRibbonButton();
            this.LoadDataBtn = this.Factory.CreateRibbonButton();
            this.SaveToFileBtn = this.Factory.CreateRibbonButton();
            this.LoadFromBtn = this.Factory.CreateRibbonButton();
            this.AddColBtn = this.Factory.CreateRibbonButton();
            this.OpnIfcBtn = this.Factory.CreateRibbonButton();
            this.DTBtn = this.Factory.CreateRibbonButton();
            this.DBsetupBtn = this.Factory.CreateRibbonButton();
            this.TIAImportBtn = this.Factory.CreateRibbonButton();
            this.CalcDBBtn = this.Factory.CreateRibbonButton();
            this.ExportStpBtn = this.Factory.CreateRibbonButton();
            this.ExportMenu = this.Factory.CreateRibbonMenu();
            this.IOListBtn = this.Factory.CreateRibbonButton();
            this.DBsFCsBtn = this.Factory.CreateRibbonButton();
            this.CalculateDBBtn = this.Factory.CreateRibbonButton();
            this.GenerateSrcBtn = this.Factory.CreateRibbonButton();
            this.AImportBtn = this.Factory.CreateRibbonButton();
            this.AExportBtn = this.Factory.CreateRibbonButton();
            this.ASetupBtn = this.Factory.CreateRibbonButton();
            this.BuildXMLBtn = this.Factory.CreateRibbonMenu();
            this.CompleteBtn = this.Factory.CreateRibbonButton();
            this.CoreFldrBtn = this.Factory.CreateRibbonButton();
            this.InstallBtn = this.Factory.CreateRibbonMenu();
            this.GraphicsBtn = this.Factory.CreateRibbonButton();
            this.FcPltResBtn = this.Factory.CreateRibbonButton();
            this.AlarmSpsBtn = this.Factory.CreateRibbonMenu();
            this.AllZ45PanelsBtn = this.Factory.CreateRibbonButton();
            this.SinglePnlBtn = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group2.SuspendLayout();
            this.MainGroup.SuspendLayout();
            this.group1.SuspendLayout();
            this.TIAGroup.SuspendLayout();
            this.S7Group.SuspendLayout();
            this.ArchestraGroup.SuspendLayout();
            this.PGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.Groups.Add(this.group2);
            this.tab1.Groups.Add(this.MainGroup);
            this.tab1.Groups.Add(this.group1);
            this.tab1.Groups.Add(this.TIAGroup);
            this.tab1.Groups.Add(this.S7Group);
            this.tab1.Groups.Add(this.ArchestraGroup);
            this.tab1.Groups.Add(this.PGroup);
            this.tab1.Label = "BDM";
            this.tab1.Name = "tab1";
            // 
            // group2
            // 
            this.group2.Items.Add(this.OnOffBtn);
            this.group2.Label = "Main";
            this.group2.Name = "group2";
            // 
            // MainGroup
            // 
            this.MainGroup.Items.Add(this.SetHeadBtn);
            this.MainGroup.Items.Add(this.SetDataBtn);
            this.MainGroup.Items.Add(this.LoadDataBtn);
            this.MainGroup.Items.Add(this.SaveToFileBtn);
            this.MainGroup.Items.Add(this.LoadFromBtn);
            this.MainGroup.Items.Add(this.AddColBtn);
            this.MainGroup.Label = "Data";
            this.MainGroup.Name = "MainGroup";
            // 
            // group1
            // 
            this.group1.Items.Add(this.OpnIfcBtn);
            this.group1.Items.Add(this.DTBtn);
            this.group1.Items.Add(this.DBsetupBtn);
            this.group1.Label = "Interface";
            this.group1.Name = "group1";
            // 
            // TIAGroup
            // 
            this.TIAGroup.Items.Add(this.TIAImportBtn);
            this.TIAGroup.Items.Add(this.CalcDBBtn);
            this.TIAGroup.Items.Add(this.ExportStpBtn);
            this.TIAGroup.Items.Add(this.ExportMenu);
            this.TIAGroup.Label = "TIA";
            this.TIAGroup.Name = "TIAGroup";
            // 
            // S7Group
            // 
            this.S7Group.Items.Add(this.CalculateDBBtn);
            this.S7Group.Items.Add(this.GenerateSrcBtn);
            this.S7Group.Label = "Step7";
            this.S7Group.Name = "S7Group";
            // 
            // ArchestraGroup
            // 
            this.ArchestraGroup.Items.Add(this.AImportBtn);
            this.ArchestraGroup.Items.Add(this.AExportBtn);
            this.ArchestraGroup.Items.Add(this.ASetupBtn);
            this.ArchestraGroup.Label = "Archestra";
            this.ArchestraGroup.Name = "ArchestraGroup";
            // 
            // PGroup
            // 
            this.PGroup.Items.Add(this.BuildXMLBtn);
            this.PGroup.Items.Add(this.InstallBtn);
            this.PGroup.Items.Add(this.AlarmSpsBtn);
            this.PGroup.Label = "Promotic";
            this.PGroup.Name = "PGroup";
            // 
            // OnOffBtn
            // 
            this.OnOffBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.OnOffBtn.Image = ((System.Drawing.Image)(resources.GetObject("OnOffBtn.Image")));
            this.OnOffBtn.Label = "On/Off";
            this.OnOffBtn.Name = "OnOffBtn";
            this.OnOffBtn.ShowImage = true;
            this.OnOffBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OnOffBtn_Click);
            // 
            // SetHeadBtn
            // 
            this.SetHeadBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.SetHeadBtn.Enabled = false;
            this.SetHeadBtn.Image = ((System.Drawing.Image)(resources.GetObject("SetHeadBtn.Image")));
            this.SetHeadBtn.Label = "Set Head";
            this.SetHeadBtn.Name = "SetHeadBtn";
            this.SetHeadBtn.ShowImage = true;
            this.SetHeadBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.SetHeadBtn_Click);
            // 
            // SetDataBtn
            // 
            this.SetDataBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.SetDataBtn.Enabled = false;
            this.SetDataBtn.Image = ((System.Drawing.Image)(resources.GetObject("SetDataBtn.Image")));
            this.SetDataBtn.Label = "Set Data";
            this.SetDataBtn.Name = "SetDataBtn";
            this.SetDataBtn.ShowImage = true;
            this.SetDataBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.SetDataBtn_Click);
            // 
            // LoadDataBtn
            // 
            this.LoadDataBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.LoadDataBtn.Enabled = false;
            this.LoadDataBtn.Image = ((System.Drawing.Image)(resources.GetObject("LoadDataBtn.Image")));
            this.LoadDataBtn.Label = "Load Data";
            this.LoadDataBtn.Name = "LoadDataBtn";
            this.LoadDataBtn.ShowImage = true;
            this.LoadDataBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.LoadDataBtn_Click);
            // 
            // SaveToFileBtn
            // 
            this.SaveToFileBtn.Image = ((System.Drawing.Image)(resources.GetObject("SaveToFileBtn.Image")));
            this.SaveToFileBtn.Label = "Save as";
            this.SaveToFileBtn.Name = "SaveToFileBtn";
            this.SaveToFileBtn.ShowImage = true;
            this.SaveToFileBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.SaveToFileBtn_Click);
            // 
            // LoadFromBtn
            // 
            this.LoadFromBtn.Image = ((System.Drawing.Image)(resources.GetObject("LoadFromBtn.Image")));
            this.LoadFromBtn.Label = "Load from";
            this.LoadFromBtn.Name = "LoadFromBtn";
            this.LoadFromBtn.ShowImage = true;
            this.LoadFromBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.LoadFromBtn_Click);
            // 
            // AddColBtn
            // 
            this.AddColBtn.Image = ((System.Drawing.Image)(resources.GetObject("AddColBtn.Image")));
            this.AddColBtn.Label = "Add Header";
            this.AddColBtn.Name = "AddColBtn";
            this.AddColBtn.ShowImage = true;
            this.AddColBtn.Visible = false;
            this.AddColBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AddColBtn_Click);
            // 
            // OpnIfcBtn
            // 
            this.OpnIfcBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.OpnIfcBtn.Enabled = false;
            this.OpnIfcBtn.Image = ((System.Drawing.Image)(resources.GetObject("OpnIfcBtn.Image")));
            this.OpnIfcBtn.Label = "Interface";
            this.OpnIfcBtn.Name = "OpnIfcBtn";
            this.OpnIfcBtn.ShowImage = true;
            this.OpnIfcBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OpnIfcBtn_Click);
            // 
            // DTBtn
            // 
            this.DTBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.DTBtn.Image = ((System.Drawing.Image)(resources.GetObject("DTBtn.Image")));
            this.DTBtn.Label = "DataType Setup";
            this.DTBtn.Name = "DTBtn";
            this.DTBtn.ShowImage = true;
            this.DTBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.DTBtn_Click);
            // 
            // DBsetupBtn
            // 
            this.DBsetupBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.DBsetupBtn.Image = ((System.Drawing.Image)(resources.GetObject("DBsetupBtn.Image")));
            this.DBsetupBtn.Label = "DB Setup";
            this.DBsetupBtn.Name = "DBsetupBtn";
            this.DBsetupBtn.ShowImage = true;
            this.DBsetupBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.DBsetupBtn_Click);
            // 
            // TIAImportBtn
            // 
            this.TIAImportBtn.Image = ((System.Drawing.Image)(resources.GetObject("TIAImportBtn.Image")));
            this.TIAImportBtn.Label = "Import";
            this.TIAImportBtn.Name = "TIAImportBtn";
            this.TIAImportBtn.ShowImage = true;
            this.TIAImportBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.TIAImportBtn_Click);
            // 
            // CalcDBBtn
            // 
            this.CalcDBBtn.Image = ((System.Drawing.Image)(resources.GetObject("CalcDBBtn.Image")));
            this.CalcDBBtn.Label = "CalculateDB";
            this.CalcDBBtn.Name = "CalcDBBtn";
            this.CalcDBBtn.ShowImage = true;
            this.CalcDBBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CalcDBBtn_Click);
            // 
            // ExportStpBtn
            // 
            this.ExportStpBtn.Image = ((System.Drawing.Image)(resources.GetObject("ExportStpBtn.Image")));
            this.ExportStpBtn.Label = "ExportSetup";
            this.ExportStpBtn.Name = "ExportStpBtn";
            this.ExportStpBtn.ShowImage = true;
            this.ExportStpBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ExportStpBtn_Click);
            // 
            // ExportMenu
            // 
            this.ExportMenu.Image = ((System.Drawing.Image)(resources.GetObject("ExportMenu.Image")));
            this.ExportMenu.Items.Add(this.IOListBtn);
            this.ExportMenu.Items.Add(this.DBsFCsBtn);
            this.ExportMenu.Label = "Export";
            this.ExportMenu.Name = "ExportMenu";
            this.ExportMenu.ShowImage = true;
            // 
            // IOListBtn
            // 
            this.IOListBtn.Label = "IOList";
            this.IOListBtn.Name = "IOListBtn";
            this.IOListBtn.ShowImage = true;
            this.IOListBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.IOListBtn_Click);
            // 
            // DBsFCsBtn
            // 
            this.DBsFCsBtn.Label = "DBs FCs";
            this.DBsFCsBtn.Name = "DBsFCsBtn";
            this.DBsFCsBtn.ShowImage = true;
            this.DBsFCsBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.DBsFCsBtn_Click);
            // 
            // CalculateDBBtn
            // 
            this.CalculateDBBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.CalculateDBBtn.Image = ((System.Drawing.Image)(resources.GetObject("CalculateDBBtn.Image")));
            this.CalculateDBBtn.Label = "Calculate DB Mapping";
            this.CalculateDBBtn.Name = "CalculateDBBtn";
            this.CalculateDBBtn.ShowImage = true;
            // 
            // GenerateSrcBtn
            // 
            this.GenerateSrcBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.GenerateSrcBtn.Image = ((System.Drawing.Image)(resources.GetObject("GenerateSrcBtn.Image")));
            this.GenerateSrcBtn.Label = "Generate Source";
            this.GenerateSrcBtn.Name = "GenerateSrcBtn";
            this.GenerateSrcBtn.ShowImage = true;
            // 
            // AImportBtn
            // 
            this.AImportBtn.Image = ((System.Drawing.Image)(resources.GetObject("AImportBtn.Image")));
            this.AImportBtn.Label = "Import";
            this.AImportBtn.Name = "AImportBtn";
            this.AImportBtn.ShowImage = true;
            this.AImportBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AImportBtn_Click);
            // 
            // AExportBtn
            // 
            this.AExportBtn.Image = ((System.Drawing.Image)(resources.GetObject("AExportBtn.Image")));
            this.AExportBtn.Label = "Export";
            this.AExportBtn.Name = "AExportBtn";
            this.AExportBtn.ShowImage = true;
            this.AExportBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AExportBtn_Click);
            // 
            // ASetupBtn
            // 
            this.ASetupBtn.Image = ((System.Drawing.Image)(resources.GetObject("ASetupBtn.Image")));
            this.ASetupBtn.Label = "Setup";
            this.ASetupBtn.Name = "ASetupBtn";
            this.ASetupBtn.ShowImage = true;
            this.ASetupBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ASetupBtn_Click);
            // 
            // BuildXMLBtn
            // 
            this.BuildXMLBtn.Image = ((System.Drawing.Image)(resources.GetObject("BuildXMLBtn.Image")));
            this.BuildXMLBtn.Items.Add(this.CompleteBtn);
            this.BuildXMLBtn.Items.Add(this.CoreFldrBtn);
            this.BuildXMLBtn.Label = "Build XML";
            this.BuildXMLBtn.Name = "BuildXMLBtn";
            this.BuildXMLBtn.ShowImage = true;
            // 
            // CompleteBtn
            // 
            this.CompleteBtn.Label = "Complete";
            this.CompleteBtn.Name = "CompleteBtn";
            this.CompleteBtn.ShowImage = true;
            // 
            // CoreFldrBtn
            // 
            this.CoreFldrBtn.Label = "CoreFolder only";
            this.CoreFldrBtn.Name = "CoreFldrBtn";
            this.CoreFldrBtn.ShowImage = true;
            // 
            // InstallBtn
            // 
            this.InstallBtn.Image = ((System.Drawing.Image)(resources.GetObject("InstallBtn.Image")));
            this.InstallBtn.Items.Add(this.GraphicsBtn);
            this.InstallBtn.Items.Add(this.FcPltResBtn);
            this.InstallBtn.Label = "Install";
            this.InstallBtn.Name = "InstallBtn";
            this.InstallBtn.ShowImage = true;
            // 
            // GraphicsBtn
            // 
            this.GraphicsBtn.Label = "Graphics";
            this.GraphicsBtn.Name = "GraphicsBtn";
            this.GraphicsBtn.ShowImage = true;
            // 
            // FcPltResBtn
            // 
            this.FcPltResBtn.Label = "Faceplate Resources";
            this.FcPltResBtn.Name = "FcPltResBtn";
            this.FcPltResBtn.ShowImage = true;
            // 
            // AlarmSpsBtn
            // 
            this.AlarmSpsBtn.Image = ((System.Drawing.Image)(resources.GetObject("AlarmSpsBtn.Image")));
            this.AlarmSpsBtn.Items.Add(this.AllZ45PanelsBtn);
            this.AlarmSpsBtn.Items.Add(this.SinglePnlBtn);
            this.AlarmSpsBtn.Label = "Alarm Stripes";
            this.AlarmSpsBtn.Name = "AlarmSpsBtn";
            this.AlarmSpsBtn.ShowImage = true;
            // 
            // AllZ45PanelsBtn
            // 
            this.AllZ45PanelsBtn.Label = "All Z45_Panels";
            this.AllZ45PanelsBtn.Name = "AllZ45PanelsBtn";
            this.AllZ45PanelsBtn.ShowImage = true;
            // 
            // SinglePnlBtn
            // 
            this.SinglePnlBtn.Label = "Single Panel";
            this.SinglePnlBtn.Name = "SinglePnlBtn";
            this.SinglePnlBtn.ShowImage = true;
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook, Microsoft.Outlook.Task";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.MainGroup.ResumeLayout(false);
            this.MainGroup.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.TIAGroup.ResumeLayout(false);
            this.TIAGroup.PerformLayout();
            this.S7Group.ResumeLayout(false);
            this.S7Group.PerformLayout();
            this.ArchestraGroup.ResumeLayout(false);
            this.ArchestraGroup.PerformLayout();
            this.PGroup.ResumeLayout(false);
            this.PGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup MainGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton SetHeadBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton OpnIfcBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton SetDataBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton LoadDataBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton OnOffBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup TIAGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton TIAImportBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton CalcDBBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ExportStpBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton DTBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton DBsetupBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu ExportMenu;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton IOListBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton DBsFCsBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup S7Group;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton CalculateDBBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton GenerateSrcBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ArchestraGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AImportBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AExportBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ASetupBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup PGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu BuildXMLBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu InstallBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu AlarmSpsBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton CompleteBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton CoreFldrBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton GraphicsBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton FcPltResBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AllZ45PanelsBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton SinglePnlBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton SaveToFileBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton LoadFromBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AddColBtn;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
