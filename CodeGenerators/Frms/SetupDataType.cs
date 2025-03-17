using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BDMdata;
using BDM_Form;
using System.Xml.Serialization;
using System.IO;
using AIFCNS;
using System.Collections;

namespace BDM_Form
{
    public partial class SetupDataType : Form
    {
        IList<AObjectType> _aobjecttypes;
        BindingList<AObjectType> _datatypes;
        public SetupDataType(IList<AObjectType> incomingForm)
        {
            InitializeComponent();
            _aobjecttypes = incomingForm;
            _datatypes = new BindingList<AObjectType>(_aobjecttypes);
            dataGridView1.DataSource = _datatypes;                        
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow item in this.dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.RemoveAt(item.Index);
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetupDataType_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool close = true;
            DataGridViewRow rw;
            for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
            {
                rw = this.dataGridView1.Rows[i];
                try
                {
                    for (int j = 0; j < rw.Cells.Count; j++)
                    {
                        if (rw.Cells[j].Value == null || rw.Cells[j].Value == DBNull.Value || string.IsNullOrWhiteSpace(rw.Cells[j].Value.ToString()))
                        {
                            throw new Exception("Empty cell!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    close = false;
                    break;
                }
            }
            e.Cancel = (close == false);
        }
    }
}
