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
            this.OnOffBtn = this.Factory.CreateRibbonToggleButton();
            this.MainGroup = this.Factory.CreateRibbonGroup();
            this.SetHeadBtn = this.Factory.CreateRibbonButton();
            this.SetDataBtn = this.Factory.CreateRibbonButton();
            this.LoadDataBtn = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.OpnIfcBtn = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group2.SuspendLayout();
            this.MainGroup.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.Groups.Add(this.group2);
            this.tab1.Groups.Add(this.MainGroup);
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "BDM";
            this.tab1.Name = "tab1";
            // 
            // group2
            // 
            this.group2.Items.Add(this.OnOffBtn);
            this.group2.Label = "Main";
            this.group2.Name = "group2";
            // 
            // OnOffBtn
            // 
            this.OnOffBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.OnOffBtn.Label = "On/Off";
            this.OnOffBtn.Name = "OnOffBtn";
            this.OnOffBtn.ShowImage = true;
            this.OnOffBtn.Image = Resource.switch_6;
            this.OnOffBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OnOffBtn_Click);
            // 
            // MainGroup
            // 
            this.MainGroup.Items.Add(this.SetHeadBtn);
            this.MainGroup.Items.Add(this.SetDataBtn);
            this.MainGroup.Items.Add(this.LoadDataBtn);
            this.MainGroup.Label = "Data";
            this.MainGroup.Name = "MainGroup";
            // 
            // SetHeadBtn
            // 
            this.SetHeadBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.SetHeadBtn.Enabled = false;
            this.SetHeadBtn.Image = Resource.user;
            this.SetHeadBtn.Label = "Set Head";
            this.SetHeadBtn.Name = "SetHeadBtn";
            this.SetHeadBtn.ShowImage = true;
            this.SetHeadBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.SetHeadBtn_Click);
            // 
            // SetDataBtn
            // 
            this.SetDataBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.SetDataBtn.Enabled = false;
            this.SetDataBtn.Label = "Set Data";
            this.SetDataBtn.Name = "SetDataBtn";
            this.SetDataBtn.ShowImage = true;
            this.SetDataBtn.Image = Resource.cloud_computing_2;
            this.SetDataBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.SetDataBtn_Click);
            // 
            // LoadDataBtn
            // 
            this.LoadDataBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.LoadDataBtn.Enabled = false;
            this.LoadDataBtn.Label = "Load Data";
            this.LoadDataBtn.Name = "LoadDataBtn";
            this.LoadDataBtn.ShowImage = true;
            this.LoadDataBtn.Image = Resource.cloud_computing;
            this.LoadDataBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.LoadDataBtn_Click);
            // 
            // group1
            // 
            this.group1.Items.Add(this.OpnIfcBtn);
            this.group1.Label = "Interface";
            this.group1.Name = "group1";
            // 
            // OpnIfcBtn
            // 
            this.OpnIfcBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.OpnIfcBtn.Enabled = false;
            this.OpnIfcBtn.Image = ((System.Drawing.Image)(resources.GetObject("OpnIfcBtn.Image")));
            this.OpnIfcBtn.Label = "Interface";
            this.OpnIfcBtn.Name = "OpnIfcBtn";
            this.OpnIfcBtn.ShowImage = true;
            this.OpnIfcBtn.Image = Resource.controls_1;
            this.OpnIfcBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OpnIfcBtn_Click);
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
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
