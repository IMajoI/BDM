namespace BDM_Form
{
    partial class DBSetUp
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
            this.DBMappingGrid = new System.Windows.Forms.DataGridView();
            this.GetDBRef = new System.Windows.Forms.Button();
            this.Ok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DBMappingGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // DBMappingGrid
            // 
            this.DBMappingGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DBMappingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DBMappingGrid.Location = new System.Drawing.Point(12, 12);
            this.DBMappingGrid.Name = "DBMappingGrid";
            this.DBMappingGrid.Size = new System.Drawing.Size(454, 265);
            this.DBMappingGrid.TabIndex = 0;
            // 
            // GetDBRef
            // 
            this.GetDBRef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GetDBRef.Location = new System.Drawing.Point(310, 283);
            this.GetDBRef.Name = "GetDBRef";
            this.GetDBRef.Size = new System.Drawing.Size(75, 23);
            this.GetDBRef.TabIndex = 1;
            this.GetDBRef.Text = "Get from list";
            this.GetDBRef.UseVisualStyleBackColor = true;
            this.GetDBRef.Click += new System.EventHandler(this.GetDBRef_Click);
            // 
            // Ok
            // 
            this.Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Ok.Location = new System.Drawing.Point(391, 283);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 23);
            this.Ok.TabIndex = 2;
            this.Ok.Text = "OK";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // DBSetUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 318);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.GetDBRef);
            this.Controls.Add(this.DBMappingGrid);
            this.Name = "DBSetUp";
            this.Text = "Object Templates Definition";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DBSetUp_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DBMappingGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DBMappingGrid;
        private System.Windows.Forms.Button GetDBRef;
        private System.Windows.Forms.Button Ok;
    }
}