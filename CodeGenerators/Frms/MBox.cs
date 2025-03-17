using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BDM_Form
{
    public partial class MBox : Form
    {
        //Result of clicked button, true if OK
        public bool Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caption">Caption of messagebox window</param>
        /// <param name="msg">Text of messagebox window</param>
        public MBox(string caption, string msg)
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;  //turn off resizing
            MaximizeBox = false;                            //turn off minimize and maximize
            MinimizeBox = false;

            Text = caption;
            label.Text = msg;

            ShowDialog();   //show form as dialog window - must be closed to work with forms behind
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
