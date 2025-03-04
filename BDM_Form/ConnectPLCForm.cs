using System.Windows.Forms;
using TIAIFCNS;

namespace BDM_Form
{
    public partial class ConnectPLCForm : Form
    {
        public Z45TIAIFC TIAIFC;
        public string selectedplc;
        public string selectedproj
        {
            get
            {
                return TIAIFC.AttachedTIAPrj;
            }
        }
        public bool Valid;

        public ConnectPLCForm()
        {
            InitializeComponent();
            TIAIFC = new Z45TIAIFC();
            CenterToParent();
        }

        private void ConnectPLCForm_Load(object sender, System.EventArgs e)
        {
            TIAIFC.Update();
            TIAList.DataSource = TIAIFC.TIAList;
            TIAList.SelectedIndex = -1;
        }

        private void OpenPrj_Click(object sender, System.EventArgs e)
        {
            TIAIFC.OpenProject();
            TIAIFC.Update();
            TIAList.DataSource = TIAIFC.TIAList;
            foreach (var item in TIAIFC.TIAList)
            {
                if (item.ProjectName == TIAIFC.AttachedTIAPrj)
                {
                    TIAList.SelectedItem = item;
                }
            }
            PLCList.DataSource = TIAIFC.PrjDeviceList;
            PLCList.SelectedIndex = -1;
            PLCList.Enabled = true;
            btnOK.Enabled = false;
        }

        private void btnConnect_Click(object sender, System.EventArgs e)
        {
            if (TIAList.SelectedIndex >= 0)
            {
                TIAIFC.ConnectTiaPortal(TIAList.SelectedItem);
                PLCList.DataSource = TIAIFC.PrjDeviceList;
                PLCList.SelectedIndex = -1;
            }            
            PLCList.Enabled = true;
            btnOK.Enabled = false;                   
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            selectedplc = PLCList.SelectedItem.ToString();
            TIAIFC.SelectControllerTarget(selectedplc);
            TIAIFC.GetDataTypes();
            Valid = true;            
            MessageBox.Show("Connection successful");
            Close();
        }

        private void TIAList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (TIAList.SelectedIndex >= 0)
                btnConnect.Enabled = true;
        }

        private void PLCList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (PLCList.SelectedIndex >= 0)
                btnOK.Enabled = true;
        }
    }
}
