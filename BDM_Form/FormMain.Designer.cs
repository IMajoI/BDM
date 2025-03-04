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
            this.tIAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectPLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.iOListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateDBMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileAndSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.archestrAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.promoticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.completeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coreFolderOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.faceplateResourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvBDM = new ADGV.AdvancedDataGridView();
            this.checkBoxValid = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.progressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelPLC = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.archestrAToolStripMenuItem,
            this.promoticToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(632, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1,
            this.undoToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.clearFilterToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(45, 20);
            this.toolStripMenuItem1.Text = "Table";
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // clearFilterToolStripMenuItem
            // 
            this.clearFilterToolStripMenuItem.Name = "clearFilterToolStripMenuItem";
            this.clearFilterToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.clearFilterToolStripMenuItem.Text = "Clear Filter";
            this.clearFilterToolStripMenuItem.Click += new System.EventHandler(this.clearFilterToolStripMenuItem_Click);
            // 
            // tIAToolStripMenuItem
            // 
            this.tIAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectPLCToolStripMenuItem,
            this.importToolStripMenuItem1,
            this.exportToolStripMenuItem1,
            this.generateSourceToolStripMenuItem,
            this.calculateDBMappingToolStripMenuItem,
            this.compileAndSaveToolStripMenuItem});
            this.tIAToolStripMenuItem.Name = "tIAToolStripMenuItem";
            this.tIAToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.tIAToolStripMenuItem.Text = "TIA";
            // 
            // connectPLCToolStripMenuItem
            // 
            this.connectPLCToolStripMenuItem.Name = "connectPLCToolStripMenuItem";
            this.connectPLCToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.connectPLCToolStripMenuItem.Text = "Connect PLC";
            this.connectPLCToolStripMenuItem.Click += new System.EventHandler(this.connectPLCToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iOListToolStripMenuItem,
            this.dBToolStripMenuItem,
            this.fCToolStripMenuItem,
            this.allToolStripMenuItem});
            this.importToolStripMenuItem1.Enabled = false;
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.importToolStripMenuItem1.Text = "Import";
            // 
            // iOListToolStripMenuItem
            // 
            this.iOListToolStripMenuItem.Name = "iOListToolStripMenuItem";
            this.iOListToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.iOListToolStripMenuItem.Text = "IO List";
            this.iOListToolStripMenuItem.Click += new System.EventHandler(this.iOListToolStripMenuItem_Click);
            // 
            // dBToolStripMenuItem
            // 
            this.dBToolStripMenuItem.Name = "dBToolStripMenuItem";
            this.dBToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.dBToolStripMenuItem.Text = "DB";
            this.dBToolStripMenuItem.Click += new System.EventHandler(this.dBToolStripMenuItem_Click);
            // 
            // fCToolStripMenuItem
            // 
            this.fCToolStripMenuItem.Name = "fCToolStripMenuItem";
            this.fCToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.fCToolStripMenuItem.Text = "FC";
            this.fCToolStripMenuItem.Click += new System.EventHandler(this.fCToolStripMenuItem_Click);
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.Enabled = false;
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.exportToolStripMenuItem1.Text = "Export";
            this.exportToolStripMenuItem1.Click += new System.EventHandler(this.exportToolStripMenuItem1_Click);
            // 
            // generateSourceToolStripMenuItem
            // 
            this.generateSourceToolStripMenuItem.Enabled = false;
            this.generateSourceToolStripMenuItem.Name = "generateSourceToolStripMenuItem";
            this.generateSourceToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.generateSourceToolStripMenuItem.Text = "Generate Source";
            this.generateSourceToolStripMenuItem.Click += new System.EventHandler(this.generateSourceToolStripMenuItem_Click);
            // 
            // calculateDBMappingToolStripMenuItem
            // 
            this.calculateDBMappingToolStripMenuItem.Enabled = false;
            this.calculateDBMappingToolStripMenuItem.Name = "calculateDBMappingToolStripMenuItem";
            this.calculateDBMappingToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.calculateDBMappingToolStripMenuItem.Text = "Calculate DB Mapping";
            this.calculateDBMappingToolStripMenuItem.Click += new System.EventHandler(this.calculateDBMappingToolStripMenuItem_Click);
            // 
            // compileAndSaveToolStripMenuItem
            // 
            this.compileAndSaveToolStripMenuItem.Enabled = false;
            this.compileAndSaveToolStripMenuItem.Name = "compileAndSaveToolStripMenuItem";
            this.compileAndSaveToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.compileAndSaveToolStripMenuItem.Text = "Compile and save";
            this.compileAndSaveToolStripMenuItem.Click += new System.EventHandler(this.compileAndSaveToolStripMenuItem_Click);
            // 
            // archestrAToolStripMenuItem
            // 
            this.archestrAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem});
            this.archestrAToolStripMenuItem.Name = "archestrAToolStripMenuItem";
            this.archestrAToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.archestrAToolStripMenuItem.Text = "ArchestrA";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // promoticToolStripMenuItem
            // 
            this.promoticToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildXMLToolStripMenuItem,
            this.installToolStripMenuItem});
            this.promoticToolStripMenuItem.Name = "promoticToolStripMenuItem";
            this.promoticToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.promoticToolStripMenuItem.Text = "Promotic";
            // 
            // buildXMLToolStripMenuItem
            // 
            this.buildXMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.completeToolStripMenuItem,
            this.coreFolderOnlyToolStripMenuItem});
            this.buildXMLToolStripMenuItem.Name = "buildXMLToolStripMenuItem";
            this.buildXMLToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.buildXMLToolStripMenuItem.Text = "Build XML";
            // 
            // completeToolStripMenuItem
            // 
            this.completeToolStripMenuItem.Name = "completeToolStripMenuItem";
            this.completeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.completeToolStripMenuItem.Text = "Complete";
            this.completeToolStripMenuItem.Click += new System.EventHandler(this.completeToolStripMenuItem_Click);
            // 
            // coreFolderOnlyToolStripMenuItem
            // 
            this.coreFolderOnlyToolStripMenuItem.Name = "coreFolderOnlyToolStripMenuItem";
            this.coreFolderOnlyToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.coreFolderOnlyToolStripMenuItem.Text = "CoreFolder only";
            this.coreFolderOnlyToolStripMenuItem.Click += new System.EventHandler(this.coreFolderOnlyToolStripMenuItem_Click);
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphicsToolStripMenuItem,
            this.faceplateResourcesToolStripMenuItem});
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            this.installToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.installToolStripMenuItem.Text = "Install";
            // 
            // graphicsToolStripMenuItem
            // 
            this.graphicsToolStripMenuItem.Name = "graphicsToolStripMenuItem";
            this.graphicsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.graphicsToolStripMenuItem.Text = "Graphics";
            this.graphicsToolStripMenuItem.Click += new System.EventHandler(this.graphicsToolStripMenuItem_Click);
            // 
            // faceplateResourcesToolStripMenuItem
            // 
            this.faceplateResourcesToolStripMenuItem.Name = "faceplateResourcesToolStripMenuItem";
            this.faceplateResourcesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.faceplateResourcesToolStripMenuItem.Text = "Faceplate Resources";
            this.faceplateResourcesToolStripMenuItem.Click += new System.EventHandler(this.faceplateResourcesToolStripMenuItem_Click);
            // 
            // dgvBDM
            // 
            this.dgvBDM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBDM.AutoGenerateContextFilters = true;
            this.dgvBDM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBDM.DateWithTime = false;
            this.dgvBDM.Location = new System.Drawing.Point(0, 27);
            this.dgvBDM.Name = "dgvBDM";
            this.dgvBDM.Size = new System.Drawing.Size(632, 378);
            this.dgvBDM.TabIndex = 6;
            this.dgvBDM.TimeFilter = false;
            this.dgvBDM.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBDM_CellValueChanged);
            this.dgvBDM.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvBDM_DataError);
            this.dgvBDM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvBDM_KeyDown);
            // 
            // checkBoxValid
            // 
            this.checkBoxValid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxValid.AutoSize = true;
            this.checkBoxValid.Location = new System.Drawing.Point(540, 411);
            this.checkBoxValid.Name = "checkBoxValid";
            this.checkBoxValid.Size = new System.Drawing.Size(92, 17);
            this.checkBoxValid.TabIndex = 12;
            this.checkBoxValid.Text = "Validity check";
            this.checkBoxValid.UseVisualStyleBackColor = true;
            this.checkBoxValid.CheckedChanged += new System.EventHandler(this.checkBoxValid_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.progressLabel,
            this.labelProject,
            this.labelPLC});
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(632, 22);
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
            this.progressLabel.Size = new System.Drawing.Size(49, 17);
            this.progressLabel.Text = "Progress";
            // 
            // labelProject
            // 
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(124, 17);
            this.labelProject.Text = "| No project connected |";
            // 
            // labelPLC
            // 
            this.labelPLC.Name = "labelPLC";
            this.labelPLC.Size = new System.Drawing.Size(87, 17);
            this.labelPLC.Text = "No PLC attached";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.checkBoxValid);
            this.Controls.Add(this.dgvBDM);
            this.Controls.Add(this.menuStrip);
            this.Name = "FormMain";
            this.Text = "BDM Interface";
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
        private System.Windows.Forms.ToolStripMenuItem connectPLCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem iOListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem generateSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculateDBMappingToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel labelProject;
        private System.Windows.Forms.ToolStripStatusLabel labelPLC;
        private System.Windows.Forms.ToolStripMenuItem compileAndSaveToolStripMenuItem;
    }
}

