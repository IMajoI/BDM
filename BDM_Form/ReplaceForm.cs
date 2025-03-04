using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ADGV;

namespace BDM_Form
{
    public partial class ReplaceForm : Form
    {
        AdvancedDataGridView Adgv;              //adgv passed in constructor
        int findCol = 0;                        //find cell column
        int findRow = 0;                        //find cell row
        bool replaceNow = false;                //indicatio if cell should be replaced already or it's only shown first
        DataGridViewCell cellToReplace = null;  //cell to work with while find or replace

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adgv">ADGV where we need to find or replace something</param>
        public ReplaceForm(AdvancedDataGridView adgv)
        {
            InitializeComponent();
            TopMost = true;         //leave form at top so user can see find/replace form while working with table
            CenterToParent();
            Adgv = adgv;

            try
            {
                //if selected cell has a value during invoke of form, fill with that value WHAT textbox
                if (adgv.SelectedCells[0].Value.ToString() != "")
                {
                    textBoxWhat.Text = adgv.SelectedCells[0].Value.ToString();
                }
            }
            catch { }
            

            //set column filter combobox
            comboBoxColSel.Items.Add("All");
            comboBoxColSel.SelectedItem = "All";

            foreach (DataGridViewColumn col in adgv.Columns)
            {
                comboBoxColSel.Items.Add(col.Name);
            }

        }

        private void btnFindNext_Click(object sender, System.EventArgs e)
        {
            AcceptButton = btnFindNext; //set accept button to find button, so user can find next just by pressing enter key
            FindMethod();
        }

        private DataGridViewCell FindMethod()
        {
            labelNoMore.Visible = false;        //hide no more findings label
            Adgv.ClearSelection();              //clear selection in table
            DataGridViewCell findCell = null;

            //if find in whole table
            if (comboBoxColSel.SelectedItem.Equals("All"))
            {
                findCell = Adgv.FindCell(textBoxWhat.Text, null, findRow, findCol, checkBoxWholeWord.Checked);
            }
            //if some specific column is selected
            else
            {
                findCell = Adgv.FindCell(textBoxWhat.Text, comboBoxColSel.SelectedItem.ToString(), findRow, findCol, checkBoxWholeWord.Checked);
            }

            //if no cell is found
            if (findCell == null)
            {
                labelNoMore.Visible = true;     //display no more finds
                findCol = 0;                    //reset findcell coordinations
                findRow = 0;
                return findCell;
            }

            findCol = findCell.ColumnIndex;
            findRow = findCell.RowIndex;

            Adgv[findCol, findRow].Selected = true;             //select found cell in table
            Adgv.FirstDisplayedCell = Adgv[findCol, findRow];   //scroll to found cell

            findCol++;
            return findCell;
        }
        /// <summary>
        /// Event to generate tooltip about checkbox if find EQUALS or CONTAINS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxWholeWord_MouseHover(object sender, System.EventArgs e)
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(checkBoxWholeWord, "Match whole word only");
        }

        private void btnReplaceNext_Click(object sender, System.EventArgs e)
        {
            AcceptButton = btnReplaceNext;  //set accept button to replace button so user can use just enter key to replace next

            //at first find is cell only selected and on second enter hit it's replaced and moved to next find
            if (!replaceNow)
            {
                replaceNow = true;
                cellToReplace = FindMethod();
            }
            else if(cellToReplace != null)
            {
                if (checkBoxWholeWord.Checked)
                {
                    cellToReplace.Value = textBoxWith.Text;
                }
                else
                {
                    cellToReplace.Value = cellToReplace.Value.ToString().Replace(textBoxWhat.Text, textBoxWith.Text);
                }
                cellToReplace = FindMethod();
            }
        }

        private void btnReplaceAll_Click(object sender, System.EventArgs e)
        {
            DataGridViewCell findCell = FindMethod();

            while (findCell != null)
            {
                if (checkBoxWholeWord.Checked)
                {
                    findCell.Value = textBoxWith.Text;
                }
                else
                {
                    findCell.Value = findCell.Value.ToString().Replace(textBoxWhat.Text, textBoxWith.Text);
                }
                findCell = FindMethod();
            }
        }
    }
}
