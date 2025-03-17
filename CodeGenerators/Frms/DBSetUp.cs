using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIAIFCNS;
using BDMdata;
using BDM_Form;
using Microsoft.Win32;

namespace BDM_Form
{
    public partial class DBSetUp : Form
    {
        private GData _globaldata;
        public DBSetUp(GData pGlobalData)
        {            
            InitializeComponent();
            _globaldata = pGlobalData;
            string path = "";
            RegistryKey RegK = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Excel\\Addins\\BDM"); // get instalation path from registry
            if (RegK != null)
            {
                path = RegK.GetValue("Manifest").ToString();
                path = path.Replace("BDM.vsto", "XMLSources/AObjectTypes.xml");
                path = path.Replace("file:///", "");
            }

            _globaldata.DeserializeDataTypes(path);
            _globaldata.GetDBMapping();
            DBMappingGrid.DataSource = _globaldata.DBMappingRef;
            if (DBMappingGrid.ColumnCount == 0)
            {
                DBMappingGrid.Columns[0].HeaderText = "DB Name";
                DBMappingGrid.Columns[1].HeaderText = "DB Number";
                DBMappingGrid.Columns[2].HeaderText = "DB Offset";             
             
            }
            DataGridViewRow rw = new DataGridViewRow();
            for (int i = 0; i < DBMappingGrid.Rows.Count; i++)
            {
                rw = DBMappingGrid.Rows[i];
                if ((string)rw.Cells[0].Value == " " && (string)rw.Cells[1].Value == " ")
                {
                    DBMappingGrid.Rows.RemoveAt(i);
                }
            }
        }

        private void GetDBRef_Click(object sender, EventArgs e)
        {
            _globaldata.GetDBMapping();        
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DBSetUp_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
