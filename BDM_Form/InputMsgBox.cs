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
    public partial class InputMsgBox : Form
    {
        public int newRowsNmb = 0;

        public InputMsgBox()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;  //turn off resizing
            MaximizeBox = false;                            //turn off minimize and maximize
            MinimizeBox = false;
            ShowDialog();   //show form as dialog window - must be closed to work with forms behind
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Int32.TryParse(textBox1.Text, out newRowsNmb);
            Close();
        }

        private void InputMsgBox_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
