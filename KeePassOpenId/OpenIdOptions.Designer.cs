namespace KeePassOpenId
{
    partial class OpenIdOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvOpenIdProviders = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ProviderName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LinkedEntryId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddLink = new System.Windows.Forms.DataGridViewButtonColumn();
            this.RemoveProvider = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOpenIdProviders)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvOpenIdProviders
            // 
            this.dgvOpenIdProviders.AllowUserToDeleteRows = false;
            this.dgvOpenIdProviders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOpenIdProviders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOpenIdProviders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProviderName,
            this.LinkedEntryId,
            this.AddLink,
            this.RemoveProvider});
            this.dgvOpenIdProviders.Location = new System.Drawing.Point(28, 32);
            this.dgvOpenIdProviders.Name = "dgvOpenIdProviders";
            this.dgvOpenIdProviders.Size = new System.Drawing.Size(643, 177);
            this.dgvOpenIdProviders.TabIndex = 0;
            this.dgvOpenIdProviders.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvOpenIdProviders_CellContentClick);
            this.dgvOpenIdProviders.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DgvOpenIdProviders_RowsAdded);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(596, 226);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(504, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ProviderName
            // 
            this.ProviderName.DataPropertyName = "Name";
            this.ProviderName.HeaderText = "Name";
            this.ProviderName.Name = "ProviderName";
            this.ProviderName.Width = 200;
            // 
            // LinkedEntryId
            // 
            this.LinkedEntryId.DataPropertyName = "LinkedEntryUUID";
            this.LinkedEntryId.HeaderText = "Linked Entry UUID";
            this.LinkedEntryId.Name = "LinkedEntryId";
            this.LinkedEntryId.ReadOnly = true;
            this.LinkedEntryId.ToolTipText = "UUID for the linked entry.";
            this.LinkedEntryId.Width = 200;
            // 
            // AddLink
            // 
            this.AddLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddLink.HeaderText = "Link to Entry";
            this.AddLink.Name = "AddLink";
            this.AddLink.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AddLink.Text = "Link";
            this.AddLink.ToolTipText = "Click to link to provider entry.";
            this.AddLink.UseColumnTextForButtonValue = true;
            // 
            // RemoveProvider
            // 
            this.RemoveProvider.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RemoveProvider.HeaderText = "Remove";
            this.RemoveProvider.Name = "RemoveProvider";
            this.RemoveProvider.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RemoveProvider.Text = "Remove";
            this.RemoveProvider.ToolTipText = "Click to remove this provider.";
            this.RemoveProvider.UseColumnTextForButtonValue = true;
            // 
            // OpenIdOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 276);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvOpenIdProviders);
            this.Name = "OpenIdOptions";
            this.Text = "OpenIdOptions";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOpenIdProviders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOpenIdProviders;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProviderName;
        private System.Windows.Forms.DataGridViewTextBoxColumn LinkedEntryId;
        private System.Windows.Forms.DataGridViewButtonColumn AddLink;
        private System.Windows.Forms.DataGridViewButtonColumn RemoveProvider;
    }
}