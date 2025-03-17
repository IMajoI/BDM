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
using System.Resources;
using System.IO;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace CodeGenerators.Frms
{
    public partial class HeaderSetup : Form
    {
        public BindingList<HeaderCls> headerList;
        private string path;
        public HeaderSetup(BindingList<HeaderCls> header, ref int headerCnt)
        {
            InitializeComponent();

            headerList = new BindingList<HeaderCls>();
            headerList = header;
            DeserializeHeader();
            headerCnt = headerList.Count;

            dataGridView1.DataSource = headerList;

        }
        

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SerializeHeaders();
            this.Close();
        }
        public void DeserializeHeader()
        {
            RegistryKey RegK = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Excel\\Addins\\BDM"); // get instalation path from registry
            if (RegK != null)
            {
                path = RegK.GetValue("Manifest").ToString();
                path = path.Replace("BDM.vsto", "XMLSources/Headers.xml");
                path = path.Replace("file:///", "");
            }
            try
            {
                if (File.Exists(path))
                {
                    XmlSerializer serializer = new XmlSerializer(headerList.GetType());
                    using (StreamReader sr = new StreamReader(path))
                    {
                        headerList = (BindingList<HeaderCls>)serializer.Deserialize(sr);
                    }
                }
                else
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        if (Resource.ResourceManager.GetString("Col" + i) != null)
                        {
                            HeaderCls he = new HeaderCls();
                            he.HeaderName = Resource.ResourceManager.GetString("Col" + i);
                            headerList.Add(he);
                        }
                    }
                    foreach (var item in headerList)
                    {
                        item.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void SerializeHeaders()
        {
            using (var writer = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(headerList.GetType());
                serializer.Serialize(writer, headerList);
                writer.Flush();
            }
        }

    }

    


}
