namespace BDM_Form
{
    partial class ArchestraSetup
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AttrList = new System.Windows.Forms.DataGridView();
            this.GalaxyGridView = new System.Windows.Forms.DataGridView();
            this.TopicAliasesLabel = new System.Windows.Forms.Label();
            this.GalaxyLoadLabel = new System.Windows.Forms.Label();
            this.ApplyButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AttrList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GalaxyGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(31, 52);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "DataType:";
            // 
            // AttrList
            // 
            this.AttrList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.AttrList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AttrList.Location = new System.Drawing.Point(175, 52);
            this.AttrList.Name = "AttrList";
            this.AttrList.Size = new System.Drawing.Size(245, 392);
            this.AttrList.TabIndex = 10;
            // 
            // GalaxyGridView
            // 
            this.GalaxyGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GalaxyGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GalaxyGridView.Location = new System.Drawing.Point(438, 52);
            this.GalaxyGridView.Name = "GalaxyGridView";
            this.GalaxyGridView.Size = new System.Drawing.Size(243, 392);
            this.GalaxyGridView.TabIndex = 11;
            // 
            // TopicAliasesLabel
            // 
            this.TopicAliasesLabel.AutoSize = true;
            this.TopicAliasesLabel.Location = new System.Drawing.Point(175, 26);
            this.TopicAliasesLabel.Name = "TopicAliasesLabel";
            this.TopicAliasesLabel.Size = new System.Drawing.Size(73, 13);
            this.TopicAliasesLabel.TabIndex = 12;
            this.TopicAliasesLabel.Text = "TopicAliases: ";
            // 
            // GalaxyLoadLabel
            // 
            this.GalaxyLoadLabel.AutoSize = true;
            this.GalaxyLoadLabel.Location = new System.Drawing.Point(438, 27);
            this.GalaxyLoadLabel.Name = "GalaxyLoadLabel";
            this.GalaxyLoadLabel.Size = new System.Drawing.Size(66, 13);
            this.GalaxyLoadLabel.TabIndex = 13;
            this.GalaxyLoadLabel.Text = "GalaxyLoad:";
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyButton.Location = new System.Drawing.Point(606, 450);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 14;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // ArchestraSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 485);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.GalaxyLoadLabel);
            this.Controls.Add(this.TopicAliasesLabel);
            this.Controls.Add(this.GalaxyGridView);
            this.Controls.Add(this.AttrList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Name = "ArchestraSetup";
            this.Text = "ArchestraSetup";
            this.SizeChanged += new System.EventHandler(this.ArchestraSetup_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.AttrList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GalaxyGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView AttrList;
        private System.Windows.Forms.DataGridView GalaxyGridView;
        private System.Windows.Forms.Label TopicAliasesLabel;
        private System.Windows.Forms.Label GalaxyLoadLabel;
        private System.Windows.Forms.Button ApplyButton;
    }
}