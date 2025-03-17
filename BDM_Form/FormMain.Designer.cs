using BDMdata;

namespace BDM_Form
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importIOTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupDataTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tIAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.iOListToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.step7ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateDBS7ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSourceS7ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.archestrAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.promoticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.completeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coreFolderOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.faceplateResourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alarmStripesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allZ45PanelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singlePanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectTemplatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvBDM = new ADGV.AdvancedDataGridView();
            this.checkBoxValid = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.progressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnNextInvalid = new System.Windows.Forms.Button();
            this.checkBoxFilterCols = new System.Windows.Forms.CheckBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBDM)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.tIAToolStripMenuItem,
            this.step7ToolStripMenuItem,
            this.archestrAToolStripMenuItem,
            this.promoticToolStripMenuItem,
            this.objectTemplatesToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(997, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1,
            this.undoToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.clearFilterToolStripMenuItem,
            this.addToolStripMenuItem,
            this.importIOTableToolStripMenuItem,
            this.setupDataTypeToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(46, 20);
            this.toolStripMenuItem1.Text = "Table";
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.undoToolStripMenuItem.Text = "Undo / Load";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // clearFilterToolStripMenuItem
            // 
            this.clearFilterToolStripMenuItem.Name = "clearFilterToolStripMenuItem";
            this.clearFilterToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.clearFilterToolStripMenuItem.Text = "Clear Filter";
            this.clearFilterToolStripMenuItem.Click += new System.EventHandler(this.clearFilterToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rowToolStripMenuItem,
            this.rowsToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // rowToolStripMenuItem
            // 
            this.rowToolStripMenuItem.Name = "rowToolStripMenuItem";
            this.rowToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.rowToolStripMenuItem.Text = "Row";
            this.rowToolStripMenuItem.Click += new System.EventHandler(this.rowToolStripMenuItem_Click);
            // 
            // rowsToolStripMenuItem
            // 
            this.rowsToolStripMenuItem.Name = "rowsToolStripMenuItem";
            this.rowsToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.rowsToolStripMenuItem.Text = "Rows";
            this.rowsToolStripMenuItem.Click += new System.EventHandler(this.rowsToolStripMenuItem_Click);
            // 
            // importIOTableToolStripMenuItem
            // 
            this.importIOTableToolStripMenuItem.Name = "importIOTableToolStripMenuItem";
            this.importIOTableToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.importIOTableToolStripMenuItem.Text = "ImportIOTable";
            this.importIOTableToolStripMenuItem.Click += new System.EventHandler(this.importIOTableToolStripMenuItem_Click);
            // 
            // setupDataTypeToolStripMenuItem
            // 
            this.setupDataTypeToolStripMenuItem.Name = "setupDataTypeToolStripMenuItem";
            this.setupDataTypeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.setupDataTypeToolStripMenuItem.Text = "Setup DataType";
            this.setupDataTypeToolStripMenuItem.Click += new System.EventHandler(this.setupDataTypeToolStripMenuItem_Click);
            // 
            // tIAToolStripMenuItem
            // 
            this.tIAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.exportToolStripMenuItem1,
            this.calculateDBToolStripMenuItem,
            this.exportSetupToolStripMenuItem});
            this.tIAToolStripMenuItem.Name = "tIAToolStripMenuItem";
            this.tIAToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.tIAToolStripMenuItem.Text = "TIA";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.importToolStripMenuItem1.Text = "Import";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iOListToolStripMenuItem1,
            this.importToolStripMenuItem2});
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem1.Text = "Export";
            // 
            // iOListToolStripMenuItem1
            // 
            this.iOListToolStripMenuItem1.Name = "iOListToolStripMenuItem1";
            this.iOListToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.iOListToolStripMenuItem1.Text = "IOList";
            this.iOListToolStripMenuItem1.Click += new System.EventHandler(this.IOListToolStripMenuItem1_Click);
            // 
            // importToolStripMenuItem2
            // 
            this.importToolStripMenuItem2.Name = "importToolStripMenuItem2";
            this.importToolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.importToolStripMenuItem2.Text = "DBs FCs";
            this.importToolStripMenuItem2.Click += new System.EventHandler(this.importToolStripMenuItem2_Click);
            // 
            // calculateDBToolStripMenuItem
            // 
            this.calculateDBToolStripMenuItem.Name = "calculateDBToolStripMenuItem";
            this.calculateDBToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.calculateDBToolStripMenuItem.Text = "Calculate DB";
            this.calculateDBToolStripMenuItem.Click += new System.EventHandler(this.calculateDBToolStripMenuItem_Click);
            // 
            // exportSetupToolStripMenuItem
            // 
            this.exportSetupToolStripMenuItem.Name = "exportSetupToolStripMenuItem";
            this.exportSetupToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportSetupToolStripMenuItem.Text = "ExportSetup";
            this.exportSetupToolStripMenuItem.Click += new System.EventHandler(this.exportSetupToolStripMenuItem_Click);
            // 
            // step7ToolStripMenuItem
            // 
            this.step7ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calculateDBS7ToolStripMenuItem,
            this.generateSourceS7ToolStripMenuItem1});
            this.step7ToolStripMenuItem.Name = "step7ToolStripMenuItem";
            this.step7ToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.step7ToolStripMenuItem.Text = "Step7";
            // 
            // calculateDBS7ToolStripMenuItem
            // 
            this.calculateDBS7ToolStripMenuItem.Name = "calculateDBS7ToolStripMenuItem";
            this.calculateDBS7ToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.calculateDBS7ToolStripMenuItem.Text = "Calculate DB Mapping";
            this.calculateDBS7ToolStripMenuItem.Click += new System.EventHandler(this.calculateDBS7ToolStripMenuItem_Click);
            // 
            // generateSourceS7ToolStripMenuItem1
            // 
            this.generateSourceS7ToolStripMenuItem1.Name = "generateSourceS7ToolStripMenuItem1";
            this.generateSourceS7ToolStripMenuItem1.Size = new System.Drawing.Size(192, 22);
            this.generateSourceS7ToolStripMenuItem1.Text = "Generate Source";
            this.generateSourceS7ToolStripMenuItem1.Click += new System.EventHandler(this.generateSourceS7ToolStripMenuItem1_Click);
            // 
            // archestrAToolStripMenuItem
            // 
            this.archestrAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.setupToolStripMenuItem});
            this.archestrAToolStripMenuItem.Name = "archestrAToolStripMenuItem";
            this.archestrAToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.archestrAToolStripMenuItem.Text = "ArchestrA";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.setupToolStripMenuItem.Text = "Setup";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // promoticToolStripMenuItem
            // 
            this.promoticToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildXMLToolStripMenuItem,
            this.installToolStripMenuItem,
            this.alarmStripesToolStripMenuItem});
            this.promoticToolStripMenuItem.Name = "promoticToolStripMenuItem";
            this.promoticToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.promoticToolStripMenuItem.Text = "Promotic";
            // 
            // buildXMLToolStripMenuItem
            // 
            this.buildXMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.completeToolStripMenuItem,
            this.coreFolderOnlyToolStripMenuItem});
            this.buildXMLToolStripMenuItem.Name = "buildXMLToolStripMenuItem";
            this.buildXMLToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.buildXMLToolStripMenuItem.Text = "Build XML";
            // 
            // completeToolStripMenuItem
            // 
            this.completeToolStripMenuItem.Name = "completeToolStripMenuItem";
            this.completeToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.completeToolStripMenuItem.Text = "Complete";
            this.completeToolStripMenuItem.Click += new System.EventHandler(this.completeToolStripMenuItem_Click);
            // 
            // coreFolderOnlyToolStripMenuItem
            // 
            this.coreFolderOnlyToolStripMenuItem.Name = "coreFolderOnlyToolStripMenuItem";
            this.coreFolderOnlyToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.coreFolderOnlyToolStripMenuItem.Text = "CoreFolder only";
            this.coreFolderOnlyToolStripMenuItem.Click += new System.EventHandler(this.coreFolderOnlyToolStripMenuItem_Click);
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphicsToolStripMenuItem,
            this.faceplateResourcesToolStripMenuItem});
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            this.installToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.installToolStripMenuItem.Text = "Install";
            // 
            // graphicsToolStripMenuItem
            // 
            this.graphicsToolStripMenuItem.Name = "graphicsToolStripMenuItem";
            this.graphicsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.graphicsToolStripMenuItem.Text = "Graphics";
            this.graphicsToolStripMenuItem.Click += new System.EventHandler(this.graphicsToolStripMenuItem_Click);
            // 
            // faceplateResourcesToolStripMenuItem
            // 
            this.faceplateResourcesToolStripMenuItem.Name = "faceplateResourcesToolStripMenuItem";
            this.faceplateResourcesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.faceplateResourcesToolStripMenuItem.Text = "Faceplate Resources";
            this.faceplateResourcesToolStripMenuItem.Click += new System.EventHandler(this.faceplateResourcesToolStripMenuItem_Click);
            // 
            // alarmStripesToolStripMenuItem
            // 
            this.alarmStripesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allZ45PanelsToolStripMenuItem,
            this.singlePanelToolStripMenuItem});
            this.alarmStripesToolStripMenuItem.Name = "alarmStripesToolStripMenuItem";
            this.alarmStripesToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.alarmStripesToolStripMenuItem.Text = "Alarm Stripes";
            // 
            // allZ45PanelsToolStripMenuItem
            // 
            this.allZ45PanelsToolStripMenuItem.Name = "allZ45PanelsToolStripMenuItem";
            this.allZ45PanelsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.allZ45PanelsToolStripMenuItem.Text = "All Z45_Panels";
            this.allZ45PanelsToolStripMenuItem.Click += new System.EventHandler(this.allZ45PanelsToolStripMenuItem_Click);
            // 
            // singlePanelToolStripMenuItem
            // 
            this.singlePanelToolStripMenuItem.Name = "singlePanelToolStripMenuItem";
            this.singlePanelToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.singlePanelToolStripMenuItem.Text = "Single panel";
            this.singlePanelToolStripMenuItem.Click += new System.EventHandler(this.singlePanelToolStripMenuItem_Click);
            // 
            // objectTemplatesToolStripMenuItem
            // 
            this.objectTemplatesToolStripMenuItem.Name = "objectTemplatesToolStripMenuItem";
            this.objectTemplatesToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.objectTemplatesToolStripMenuItem.Text = "DB set up";
            this.objectTemplatesToolStripMenuItem.Click += new System.EventHandler(this.objectTemplatesToolStripMenuItem_Click);
            // 
            // dgvBDM
            // 
            this.dgvBDM.AllowUserToAddRows = false;
            this.dgvBDM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBDM.AutoGenerateContextFilters = true;
            this.dgvBDM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBDM.DateWithTime = false;
            this.dgvBDM.Location = new System.Drawing.Point(0, 27);
            this.dgvBDM.Name = "dgvBDM";
            this.dgvBDM.Size = new System.Drawing.Size(997, 378);
            this.dgvBDM.TabIndex = 6;
            this.dgvBDM.TimeFilter = false;
            this.dgvBDM.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBDM_CellDoubleClick);
            this.dgvBDM.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBDM_CellValueChanged);
            this.dgvBDM.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvBDM_DataError);
            this.dgvBDM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvBDM_KeyDown);
            // 
            // checkBoxValid
            // 
            this.checkBoxValid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxValid.AutoSize = true;
            this.checkBoxValid.Location = new System.Drawing.Point(871, 411);
            this.checkBoxValid.Name = "checkBoxValid";
            this.checkBoxValid.Size = new System.Drawing.Size(15, 14);
            this.checkBoxValid.TabIndex = 12;
            this.checkBoxValid.UseVisualStyleBackColor = true;
            this.checkBoxValid.CheckedChanged += new System.EventHandler(this.checkBoxValid_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.progressLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(997, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // progressLabel
            // 
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(52, 17);
            this.progressLabel.Text = "Progress";
            // 
            // btnNextInvalid
            // 
            this.btnNextInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextInvalid.Enabled = false;
            this.btnNextInvalid.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.btnNextInvalid.FlatAppearance.BorderSize = 0;
            this.btnNextInvalid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextInvalid.Location = new System.Drawing.Point(892, 407);
            this.btnNextInvalid.Name = "btnNextInvalid";
            this.btnNextInvalid.Size = new System.Drawing.Size(93, 21);
            this.btnNextInvalid.TabIndex = 14;
            this.btnNextInvalid.Text = "Validity check";
            this.btnNextInvalid.UseVisualStyleBackColor = true;
            this.btnNextInvalid.Click += new System.EventHandler(this.btnNextInvalid_Click);
            // 
            // checkBoxFilterCols
            // 
            this.checkBoxFilterCols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxFilterCols.AutoSize = true;
            this.checkBoxFilterCols.Location = new System.Drawing.Point(12, 411);
            this.checkBoxFilterCols.Name = "checkBoxFilterCols";
            this.checkBoxFilterCols.Size = new System.Drawing.Size(131, 17);
            this.checkBoxFilterCols.TabIndex = 15;
            this.checkBoxFilterCols.Text = "Filter relevant columns";
            this.checkBoxFilterCols.UseVisualStyleBackColor = true;
            this.checkBoxFilterCols.CheckedChanged += new System.EventHandler(this.checkBoxFilterCols_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 453);
            this.Controls.Add(this.checkBoxFilterCols);
            this.Controls.Add(this.btnNextInvalid);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.checkBoxValid);
            this.Controls.Add(this.dgvBDM);
            this.Controls.Add(this.menuStrip);
            this.Name = "FormMain";
            this.Text = "BDM Interface TIA v15";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBDM)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tIAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem archestrAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem promoticToolStripMenuItem;
        public ADGV.AdvancedDataGridView dgvBDM;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearFilterToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxValid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel progressLabel;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem completeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem coreFolderOnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem faceplateResourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem step7ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSourceS7ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem calculateDBS7ToolStripMenuItem;
        private System.Windows.Forms.Button btnNextInvalid;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rowsToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxFilterCols;
        private System.Windows.Forms.ToolStripMenuItem importIOTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectTemplatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iOListToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem alarmStripesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allZ45PanelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singlePanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculateDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupDataTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSetupToolStripMenuItem;
    }
}

