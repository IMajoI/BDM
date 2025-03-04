namespace BDM_Form
{
    partial class ReplaceForm
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
            this.textBoxWhat = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnReplaceNext = new System.Windows.Forms.Button();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.textBoxWith = new System.Windows.Forms.TextBox();
            this.checkBoxWholeWord = new System.Windows.Forms.CheckBox();
            this.comboBoxColSel = new System.Windows.Forms.ComboBox();
            this.labelNoMore = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxWhat
            // 
            this.textBoxWhat.Location = new System.Drawing.Point(13, 24);
            this.textBoxWhat.Name = "textBoxWhat";
            this.textBoxWhat.Size = new System.Drawing.Size(294, 20);
            this.textBoxWhat.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Replace what";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Replace with";
            // 
            // btnFindNext
            // 
            this.btnFindNext.Location = new System.Drawing.Point(313, 24);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(110, 23);
            this.btnFindNext.TabIndex = 4;
            this.btnFindNext.Text = "Find Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // btnReplaceNext
            // 
            this.btnReplaceNext.Location = new System.Drawing.Point(313, 53);
            this.btnReplaceNext.Name = "btnReplaceNext";
            this.btnReplaceNext.Size = new System.Drawing.Size(110, 23);
            this.btnReplaceNext.TabIndex = 5;
            this.btnReplaceNext.Text = "Replace Next";
            this.btnReplaceNext.UseVisualStyleBackColor = true;
            this.btnReplaceNext.Click += new System.EventHandler(this.btnReplaceNext_Click);
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Location = new System.Drawing.Point(313, 82);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(110, 23);
            this.btnReplaceAll.TabIndex = 6;
            this.btnReplaceAll.Text = "Replace ALL";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // textBoxWith
            // 
            this.textBoxWith.Location = new System.Drawing.Point(12, 77);
            this.textBoxWith.Name = "textBoxWith";
            this.textBoxWith.Size = new System.Drawing.Size(295, 20);
            this.textBoxWith.TabIndex = 1;
            // 
            // checkBoxWholeWord
            // 
            this.checkBoxWholeWord.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxWholeWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxWholeWord.Location = new System.Drawing.Point(12, 103);
            this.checkBoxWholeWord.Name = "checkBoxWholeWord";
            this.checkBoxWholeWord.Size = new System.Drawing.Size(30, 25);
            this.checkBoxWholeWord.TabIndex = 8;
            this.checkBoxWholeWord.Text = "\"ab\"";
            this.checkBoxWholeWord.UseVisualStyleBackColor = true;
            this.checkBoxWholeWord.MouseHover += new System.EventHandler(this.checkBoxWholeWord_MouseHover);
            // 
            // comboBoxColSel
            // 
            this.comboBoxColSel.FormattingEnabled = true;
            this.comboBoxColSel.Location = new System.Drawing.Point(186, 107);
            this.comboBoxColSel.Name = "comboBoxColSel";
            this.comboBoxColSel.Size = new System.Drawing.Size(121, 21);
            this.comboBoxColSel.TabIndex = 9;
            // 
            // labelNoMore
            // 
            this.labelNoMore.AutoSize = true;
            this.labelNoMore.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelNoMore.Location = new System.Drawing.Point(224, 47);
            this.labelNoMore.Name = "labelNoMore";
            this.labelNoMore.Size = new System.Drawing.Size(83, 24);
            this.labelNoMore.TabIndex = 10;
            this.labelNoMore.Text = "That\'s all";
            this.labelNoMore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelNoMore.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Column filter";
            // 
            // ReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 133);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelNoMore);
            this.Controls.Add(this.comboBoxColSel);
            this.Controls.Add(this.checkBoxWholeWord);
            this.Controls.Add(this.textBoxWith);
            this.Controls.Add(this.btnReplaceAll);
            this.Controls.Add(this.btnReplaceNext);
            this.Controls.Add(this.btnFindNext);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxWhat);
            this.Name = "ReplaceForm";
            this.Text = "Find & Replace";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxWhat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Button btnReplaceNext;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.TextBox textBoxWith;
        private System.Windows.Forms.CheckBox checkBoxWholeWord;
        private System.Windows.Forms.ComboBox comboBoxColSel;
        private System.Windows.Forms.Label labelNoMore;
        private System.Windows.Forms.Label label3;
    }
}