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
    public partial class ExcelListSelector : Form
    {
        public int SelIndex;
        public event EventHandler SelectionChanged;
        public event EventHandler SelectionAccepted;
        public ExcelListSelector(List<string> L)
        {
            InitializeComponent();
            listBox1.DataSource = L;
            listBox1.SelectedIndex = -1;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            SelectionAccepted?.Invoke(sender, e);
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelIndex = listBox1.SelectedIndex + 1;
            if (listBox1.SelectedIndex >= 0)
            {
                OKBtn.Enabled = true;
            }
            else
            {
                OKBtn.Enabled = false;
            }

            SelectionChanged?.Invoke(sender, e);
        }
    }
}
