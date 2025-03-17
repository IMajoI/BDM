using AIFCNS;
using BDMdata;
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
    public partial class ExportSetup : Form
    {
        public ExportSetup(List<AObjectType> data)
        {
            InitializeComponent();
            DataTypeComboBox.DataSource = data;
            BindingSource bs = new BindingSource((DataTypeComboBox.SelectedItem as AObjectType).DTProperties, null);
            dataGridView1.DataSource = bs;
            dataGridView1.Columns[1].Width += 23;
        }

        private void DataTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource((DataTypeComboBox.SelectedItem as AObjectType).DTProperties, null);
            dataGridView1.DataSource = bs;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
