namespace BDM_Form
{
    partial class ConnectPLCForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectPLCForm));
            this.TIAList = new System.Windows.Forms.ListBox();
            this.PLCList = new System.Windows.Forms.ListBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.OpenPrj = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TIAList
            // 
            this.TIAList.FormattingEnabled = true;
            this.TIAList.Location = new System.Drawing.Point(12, 12);
            this.TIAList.Name = "TIAList";
            this.TIAList.Size = new System.Drawing.Size(244, 186);
            this.TIAList.TabIndex = 14;
            this.TIAList.SelectedIndexChanged += new System.EventHandler(this.TIAList_SelectedIndexChanged);
            // 
            // PLCList
            // 
            this.PLCList.Enabled = false;
            this.PLCList.FormattingEnabled = true;
            this.PLCList.Location = new System.Drawing.Point(318, 12);
            this.PLCList.Name = "PLCList";
            this.PLCList.Size = new System.Drawing.Size(244, 186);
            this.PLCList.TabIndex = 16;
            this.PLCList.SelectedIndexChanged += new System.EventHandler(this.PLCList_SelectedIndexChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConnect.BackgroundImage")));
            this.btnConnect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConnect.Location = new System.Drawing.Point(262, 80);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(50, 50);
            this.btnConnect.TabIndex = 17;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(487, 230);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // OpenPrj
            // 
            this.OpenPrj.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OpenPrj.BackgroundImage")));
            this.OpenPrj.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.OpenPrj.Location = new System.Drawing.Point(12, 204);
            this.OpenPrj.Name = "OpenPrj";
            this.OpenPrj.Size = new System.Drawing.Size(33, 33);
            this.OpenPrj.TabIndex = 19;
            this.OpenPrj.UseVisualStyleBackColor = true;
            this.OpenPrj.Click += new System.EventHandler(this.OpenPrj_Click);
            // 
            // ConnectPLCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 265);
            this.Controls.Add(this.OpenPrj);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.PLCList);
            this.Controls.Add(this.TIAList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectPLCForm";
            this.Text = "Select & Connect";
            this.Load += new System.EventHandler(this.ConnectPLCForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox TIAList;
        private System.Windows.Forms.ListBox PLCList;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button OpenPrj;
    }
}