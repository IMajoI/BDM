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
    public partial class ImportTableView : Form
    {
        private int currentMouseOverClmn;
        private ContextMenu m;
        private DataTable DT;
        private GData _globaldata;

        public ImportTableView(DataTable dt, GData pGlobalData)
        {
            InitializeComponent();
            _globaldata = pGlobalData;
            DT = new DataTable();
            DT = dt;
            dataGridView1.DataSource = dt;
            m = new ContextMenu();
            m.MenuItems.Add("TagName");
            m.MenuItems.Add("Description");
            m.MenuItems.Add("IO Address");            
            m.MenuItems.Add("Min");
            m.MenuItems.Add("Max");
            m.MenuItems.Add("Unit");
            m.MenuItems.Add("NC/NO");
            m.MenuItems.Add("Reset");            
            m.MenuItems[0].Click += new EventHandler(TNClick);
            m.MenuItems[1].Click += new EventHandler(DClick);
            m.MenuItems[2].Click += new EventHandler(IOClick);
            m.MenuItems[3].Click += new EventHandler(MinClick);
            m.MenuItems[4].Click += new EventHandler(MaxClick);
            m.MenuItems[5].Click += new EventHandler(UnitClick);
            m.MenuItems[6].Click += new EventHandler(NCNOClick);
            m.MenuItems[7].Click += new EventHandler(RClick);                   
        }
              
        
        private void TNClick(object sender, EventArgs e)
        {

            DT.Columns[currentMouseOverClmn].ColumnName = "TagName";
            m.MenuItems[0].Enabled = false;
        }
        private void DClick(object sender, EventArgs e)
        {
            DT.Columns[currentMouseOverClmn].ColumnName = "Description";
            m.MenuItems[1].Enabled = false;
        }

        private void IOClick(object sender, EventArgs e)
        {
            DT.Columns[currentMouseOverClmn].ColumnName = "IO Address";
            m.MenuItems[2].Enabled = false;
        }
        private void MinClick(object sender, EventArgs e)
        {
            DT.Columns[currentMouseOverClmn].ColumnName = "Min";
            m.MenuItems[3].Enabled = false;
        }
        private void MaxClick(object sender, EventArgs e)
        {
            DT.Columns[currentMouseOverClmn].ColumnName = "Max";
            m.MenuItems[4].Enabled = false;
        }
        private void UnitClick(object sender, EventArgs e)
        {
            DT.Columns[currentMouseOverClmn].ColumnName = "Unit";
            m.MenuItems[5].Enabled = false;
        }
        private void NCNOClick(object sender, EventArgs e)
        {
            DT.Columns[currentMouseOverClmn].ColumnName = "NC/NO";
            m.MenuItems[6].Enabled = false;
        }


        private void RClick(object sender, EventArgs e)
        {
            if (DT.Columns[currentMouseOverClmn].ColumnName == "TagName")
            {
                m.MenuItems[0].Enabled = true;
            }
            else if (DT.Columns[currentMouseOverClmn].ColumnName == "Description")
            {
                m.MenuItems[1].Enabled = true;
            }
            else if (DT.Columns[currentMouseOverClmn].ColumnName == "IO Address")
            {
                m.MenuItems[2].Enabled = true;
            }
            else if (DT.Columns[currentMouseOverClmn].ColumnName == "Min")
            {
                m.MenuItems[3].Enabled = true;
            }
            else if (DT.Columns[currentMouseOverClmn].ColumnName == "Max")
            {
                m.MenuItems[4].Enabled = true;
            }
            else if (DT.Columns[currentMouseOverClmn].ColumnName == "Unit")
            {
                m.MenuItems[5].Enabled = true;
            }
            else if (DT.Columns[currentMouseOverClmn].ColumnName == "NC/NO")
            {
                m.MenuItems[6].Enabled = true;
            }
            DT.Columns[currentMouseOverClmn].ColumnName = "Column" + (currentMouseOverClmn + 1).ToString();
        }
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                currentMouseOverClmn = dataGridView1.HitTest(e.X, e.Y).ColumnIndex;
                if (!(currentMouseOverRow >= 0) & currentMouseOverClmn >= 0)
                {                    
                    m.Show(dataGridView1, new Point(e.X, e.Y));
                }
            }

        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {

            _globaldata.DataFiltered.Objects.Clear();
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(DT.Rows[i]["TagName"].ToString()))
                {

                    BasicObject BO = new BasicObject();
                    if (!m.MenuItems[0].Enabled)
                    {
                        BO.TagName = DT.Rows[i]["TagName"].ToString();
                    }
                    if (!m.MenuItems[1].Enabled)
                    {
                        BO.Descr = DT.Rows[i]["Description"].ToString();
                    }
                    if (!m.MenuItems[2].Enabled)
                    {
                        BO.IOAddress = DT.Rows[i]["IO Address"].ToString();
                        if (DT.Rows[i]["IO Address"].ToString().ToLower().Contains("w"))
                        {
                            BO.DataType = Resource.Int;
                        }
                        else
                        {
                            BO.DataType = Resource.Bool;
                        }
                    }
                    if (!m.MenuItems[3].Enabled)
                    {
                        BO.MIN = DT.Rows[i]["Min"].ToString();
                    }
                    if (!m.MenuItems[4].Enabled)
                    {
                        BO.MAX = DT.Rows[i]["Max"].ToString();
                    }
                    if (!m.MenuItems[5].Enabled)
                    {
                        BO.Unit = DT.Rows[i]["Unit"].ToString();
                    }
                    if (!m.MenuItems[6].Enabled)
                    {
                        BO.Unit = DT.Rows[i]["NC/NO"].ToString();
                    }
                    _globaldata.DataFiltered.Objects.Add(BO);
                }
            }
            this.Close();
        }
    }
}
