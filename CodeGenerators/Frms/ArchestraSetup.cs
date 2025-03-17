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
    public partial class ArchestraSetup : Form
    {
        public int sizeWidth;
        public ArchestraSetup(List<AObjectType> data)
        {
            InitializeComponent();
            sizeWidth = this.Size.Width;
            comboBox1.DataSource = data;

            BindingSource bs = new BindingSource((comboBox1.SelectedItem as AObjectType).Attributes, null);
            AttrList.DataSource = bs;
            BindingSource gbs = new BindingSource((comboBox1.SelectedItem as AObjectType).GalaxyAttributes, null);
            GalaxyGridView.DataSource = gbs;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource((comboBox1.SelectedItem as AObjectType).Attributes, null);
            AttrList.DataSource = bs;
            BindingSource gbs = new BindingSource((comboBox1.SelectedItem as AObjectType).GalaxyAttributes, null);
            GalaxyGridView.DataSource = gbs;
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ArchestraSetup_SizeChanged(object sender, EventArgs e)
        {
            int position;
            position = this.Size.Width - sizeWidth;
            GalaxyGridView.Location = new Point(GalaxyGridView.Location.X + position/2, GalaxyGridView.Location.Y);
            AttrList.Size = new Size(AttrList.Size.Width + position / 2, AttrList.Size.Height);
            GalaxyGridView.Size = new Size(GalaxyGridView.Size.Width - position / 2, GalaxyGridView.Size.Height);
            GalaxyLoadLabel.Location = new Point(GalaxyLoadLabel.Location.X + position / 2, GalaxyLoadLabel.Location.Y);
            sizeWidth = this.Size.Width;
        }
    }
}
