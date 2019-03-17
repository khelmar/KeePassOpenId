using KeePass.Forms;
using KeePass.UI;
using KeePassLib;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KeePassOpenId
{
    public partial class OpenIdOptions : Form
    {
        private PwDatabase dbPasswordDatabase;
        private ImageList ilIcons;
		private const string csAddLink = "AddLink";
		private const string csRemoveProvider = "RemoveProvider";


		public OpenIdOptions()
        {
            InitializeComponent();
        }

        public void Initialize(BindingList<OpenIdProvider> providerList, ImageList imageBindingList, PwDatabase pwDatabase)
        {
            BindingSource bsProviderList = new BindingSource()
            {
                DataSource = providerList,
                AllowNew = true,
            };

            dgvOpenIdProviders.DataSource = bsProviderList;
            dbPasswordDatabase = pwDatabase;
            ilIcons = imageBindingList;
        }

        private void DgvOpenIdProviders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvOpenIdProviders.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                if (dgvOpenIdProviders.Columns[e.ColumnIndex].Name == csAddLink)
                {
                    FieldRefForm frfForm = new FieldRefForm();
					frfForm.InitEx(dbPasswordDatabase.RootGroup, ilIcons, string.Empty);

                    Control[] grpIdentifyControlArray = frfForm.Controls.Find("m_grpIdentify", true);
                    foreach (Control rb in grpIdentifyControlArray[0].Controls)
                    {
						if (rb is RadioButton)
						{
                            rb.Enabled = false;
                        }
                    }

                    Control[] grpRefFieldControlArray = frfForm.Controls.Find("m_grpRefField", true);
                    foreach (Control rb in grpRefFieldControlArray[0].Controls)
                    {
                        if (rb is RadioButton)
                        {
                            rb.Enabled = false;
                        }
                    }

                    string strResult = string.Empty;
                    if (frfForm.ShowDialog() == DialogResult.OK)
                    {
                        strResult = frfForm.ResultReference;

                        UIUtil.DestroyForm(frfForm);

                        //get the UUID from the string (everything after the "I:"
                        string[] split = strResult.Split(new string[] { @"I:" }, StringSplitOptions.RemoveEmptyEntries);
                        int index = dgvOpenIdProviders.Columns["LinkedEntryId"].Index;
                        dgvOpenIdProviders.Rows[e.RowIndex].Cells[index].Value = split[1].Replace("}", "");
                        dgvOpenIdProviders.Refresh();
                    }
                }
                else if (dgvOpenIdProviders.Columns[e.ColumnIndex].Name == csRemoveProvider)
                {
                    ((BindingList<OpenIdProvider>)((BindingSource)dgvOpenIdProviders.DataSource).DataSource).Remove(((BindingList<OpenIdProvider>)((BindingSource)dgvOpenIdProviders.DataSource).DataSource)[e.RowIndex]);
                    dgvOpenIdProviders.Refresh();
                }
            }
        }

        private void DgvOpenIdProviders_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            foreach(DataGridViewRow row in dgvOpenIdProviders.Rows)
            {
                if (row.IsNewRow)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (dgvOpenIdProviders.Columns[cell.ColumnIndex].Name == csAddLink)
                        {
                            cell.Value = "Link";
                        }
                        else if (dgvOpenIdProviders.Columns[cell.ColumnIndex].Name == csRemoveProvider)
                        {
                            cell.Value = "Remove";
                        }
                    }
                }
            }
        }
    }
}
