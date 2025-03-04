using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BDM
{
    public partial class FormProgress : Form
    {
        public string Message { set { label.Text = value; } }
        public int ProgressValue { set { progressBar.Value = value; } }
        public TimeSpan time { set { labelTime.Text = value.ToString("mm':'ss':'fff"); } }

        public FormProgress()
        {
            InitializeComponent();
            CenterToParent();
            TopMost = true;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            OnCancel(EventArgs.Empty);
        }

        private void OnCancel(EventArgs e)
        {
            OnCancelEvent?.Invoke(null, e);
        }

        public event EventHandler OnCancelEvent;

    }
}
