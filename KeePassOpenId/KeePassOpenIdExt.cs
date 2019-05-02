using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using Newtonsoft.Json;

namespace KeePassOpenId
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class KeePassOpenIdExt : Plugin
	{
		private bool IsOpenIdEntry = false;
		private BindingList<OpenIdProvider> providers = new BindingList<OpenIdProvider>();
		private const string csAddOpenIdEntry = "Add OpenId Entry";
		private Assembly aKeePassOpenIdExtAssembly = typeof(KeePassOpenIdExt).Assembly;
		private MainForm mfMainForm;
		private PwDatabase dbPwDatabase = null;
		private TextBox tbUserName = null;
		private TextBox tbPassword = null;
		private TextBox tbRepeatPassword = null;
		private ToolStripMenuItem tsmiAddOpenIdEntry;
		private ToolStripMenuItem tsmiOptions = new ToolStripMenuItem();

		#region overrides

		public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
		{
			// Provide a menu item for the main location(s)
			if (t == PluginMenuType.Main)
			{
				tsmiOptions.Text = "OpenId Plugin Options";
				tsmiOptions.Image = new Bitmap(aKeePassOpenIdExtAssembly.GetManifestResourceStream("KeePassOpenId.Resources.Options.png"));
				tsmiOptions.Click += OnOptionsClicked;
				tsmiOptions.Enabled = dbPwDatabase != null;
				return tsmiOptions;
			}

			return null; // No menu items in other locations
		}

		public override bool Initialize(IPluginHost host)
		{
			if (host == null)
			{
				return false;
			}

			mfMainForm = host.MainWindow;
			mfMainForm.FileOpened += MainForm_FileOpened;

			//menu tool strip
			ToolStripMenuItem tsmiMenuToolStripEdit = (ToolStripMenuItem)mfMainForm.MainMenu.Items["m_menuEntry"];

			//tool strip
			CustomToolStripEx tsmiToolStripEdit = (CustomToolStripEx)mfMainForm.Controls["m_toolMain"];

			//item context menu
			Control[] lvEntries = mfMainForm.Controls.Find("m_lvEntries", true);

			//new window event
			GlobalWindowManager.WindowAdded += OnWindowAdded;

			//create menu item
			tsmiAddOpenIdEntry = new ToolStripMenuItem
			{
				Text = csAddOpenIdEntry,
				Image = new Bitmap(aKeePassOpenIdExtAssembly.GetManifestResourceStream("KeePassOpenId.Resources.New.png")),
			};

			tsmiAddOpenIdEntry.Click += new EventHandler(OnEntryAdd);

			//add in our menu item to the menu tool strip
			if (tsmiMenuToolStripEdit != null)
			{
				//find the regular "Add Entry" item so we can put ours right after it
				for (int i = 0; i < tsmiMenuToolStripEdit.DropDownItems.Count; i++)
				{
					if (tsmiMenuToolStripEdit.DropDownItems[i] is ToolStripMenuItem)
					{
						//if (((ToolStripMenuItem)item).Name == "m_ctxEntryAdd")
						if (((ToolStripMenuItem)tsmiMenuToolStripEdit.DropDownItems[i]).Text == "&Add Entry...")
						{
							tsmiMenuToolStripEdit.DropDownItems.Insert(i + 1, tsmiAddOpenIdEntry);
							break;
						}
					}
				}
			}

			//add in our menu item to the tool strip
			if (tsmiToolStripEdit != null)
			{
				//find the regular "Add Entry" item so we can put ours right after it
				for (int i = 0; i < tsmiToolStripEdit.Items.Count; i++)
				{
					if (tsmiToolStripEdit.Items[i] is ToolStripSplitButton)
					{
						if (((ToolStripSplitButton)tsmiToolStripEdit.Items[i]).Name == "m_tbAddEntry")
						{
							ToolStripMenuItem tsmiAddOpenId = new ToolStripMenuItem
							{
								Text = csAddOpenIdEntry,
								Image = new Bitmap(aKeePassOpenIdExtAssembly.GetManifestResourceStream("KeePassOpenId.Resources.New.png")),
							};

							tsmiAddOpenId.Click += new EventHandler(OnEntryAdd);
							((ToolStripSplitButton)tsmiToolStripEdit.Items[i]).DropDownItems.Add(tsmiAddOpenId);

							break;
						}
					}
				}
			}

			//add in our menu item to the context menu
			if (lvEntries != null && lvEntries.Length == 1)
			{
				CustomContextMenuStripEx ccmsItemContextMenu = (CustomContextMenuStripEx)((CustomListViewEx)lvEntries[0]).ContextMenuStrip;
				//find the regular "Add Entry" item so we can put ours right after it
				for (int i = 0; i < ccmsItemContextMenu.Items.Count; i++)
				{
					if (ccmsItemContextMenu.Items[i] is ToolStripMenuItem)
					{
						if (((ToolStripMenuItem)ccmsItemContextMenu.Items[i]).Name == "m_ctxEntryAdd")
						{
							ToolStripMenuItem tsmiAddOpenId = new ToolStripMenuItem
							{
								Text = csAddOpenIdEntry,
								Image = new Bitmap(aKeePassOpenIdExtAssembly.GetManifestResourceStream("KeePassOpenId.Resources.New.png")),
							};

							tsmiAddOpenId.Click += new EventHandler(OnEntryAdd);
							ccmsItemContextMenu.Items.Insert(i + 1, tsmiAddOpenId);

							((ToolStripMenuItem)ccmsItemContextMenu.Items[i]).EnabledChanged += KeePassOpenIdExt_EnabledChanged;
							break;
						}
					}
				}
			}

			return true;
		}

		public override void Terminate()
		{
			tsmiAddOpenIdEntry = null;
			base.Terminate();
		}

		#endregion
		
		private void EntryForm_EntrySaved(object sender, EventArgs e)
		{
			PwEntryForm pefEntryForm = (sender as PwEntryForm);
			if (pefEntryForm != null)
			{
				Control[] tbOpenIdArray = pefEntryForm.Controls.Find("m_tbOpenId", true);

				if (tbOpenIdArray.Length == 1)
				{
					pefEntryForm.EntryRef.CustomData.Set("IsOpenIdEntry", "true");
					pefEntryForm.EntryRef.CustomData.Set("OpenIdProvider", tbOpenIdArray[0].Text);
					pefEntryForm.EntryRef.Touch(true, false);
				}
			}
		}

		private void ExtractProviders()
		{
			providers = new BindingList<OpenIdProvider>();
			
			//deserialize the entry
			string[] p = dbPwDatabase.CustomData.Get("OpenIdProviders").Split('|');
			foreach (string s in p)
			{
				if (s != string.Empty)
				{
					providers.Add(JsonConvert.DeserializeObject<OpenIdProvider>(s));
				}
			}
		}

		private void KeePassOpenIdExt_EnabledChanged(object sender, EventArgs e)
		{
			tsmiAddOpenIdEntry.Enabled = ((ToolStripMenuItem)sender).Enabled;
		}

		private void MainForm_FileOpened(object sender, FileOpenedEventArgs e)
		{
			//check to see if we've loaded our providers into the db custom data, if not load them up.
			dbPwDatabase = e.Database;
			if (!dbPwDatabase.CustomData.Exists("OpenIdProviders"))
			{
				string[] p = new string[] { "AOL", "Blogger", "BuddyPress", "Flickr", "Google", "LiveJournal", "Microsoft ", "WordPress", "Yahoo", "Facebook" };
				foreach (string s in p)
				{
					OpenIdProvider provider = new OpenIdProvider() { Name = s };
					providers.Add(provider);
				}

				SaveCustomData();
			}
			else
			{
				ExtractProviders();
			}

			tsmiOptions.Enabled = dbPwDatabase != null;
		}

		private void OnEntryAdd(object sender, EventArgs e)
		{
			ToolStripMenuItem tsmiEdit = (ToolStripMenuItem)mfMainForm.MainMenu.Items["m_menuEntry"];
			foreach (var item in tsmiEdit.DropDownItems)
			{
				if (item is ToolStripMenuItem)
				{
					//if (((ToolStripMenuItem)item).Name == "m_ctxEntryAdd")
					if (((ToolStripMenuItem)item).Text == "&Add Entry...")
					{
						IsOpenIdEntry = true;
						((ToolStripMenuItem)item).PerformClick();
					}
				}
			}
		}

		private void OnOptionsClicked(object sender, EventArgs e)
		{
			// Called when the menu item is clicked
			OpenIdOptions oioForm = new OpenIdOptions();
			oioForm.Initialize(providers, mfMainForm.ClientIcons, dbPwDatabase);
			if (UIUtil.ShowDialogAndDestroy(oioForm) == DialogResult.OK)
			{
				//save the custom data
				SaveCustomData();
			}
			else
			{
				//reload the custom data
				ExtractProviders();
			}

			UIUtil.DestroyForm(oioForm);
		}

		private void OnTbOpenId_Leave(object sender, EventArgs e)
		{
			OpenIdProvider provider = providers.Where(p => p.Name.Equals(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();

			SetProviderReferences(provider);
		}

		private void OnWindowAdded(object sender, GwmWindowEventArgs e)
		{
			PwEntryForm pefEntryForm = (e.Form as PwEntryForm);
			if (pefEntryForm != null)
			{
				//check to see if it's an OpenId entry either through the flag from clicking the correct entry, or check the custom data
				//only do this check if we haven't already set the flag
				if (!IsOpenIdEntry)
				{
					IsOpenIdEntry = pefEntryForm.EntryRef.CustomData.Exists("IsOpenIdEntry") && pefEntryForm.EntryRef.CustomData.Get("IsOpenIdEntry") == "true";
				}

				if (IsOpenIdEntry)
				{
					//find our provider
					OpenIdProvider provider = null;

					//hook up to the saved event so we can update custom data
					pefEntryForm.EntrySaved += EntryForm_EntrySaved;

					//create our controls and add to form
					Label lblOpenId = new Label()
					{
						Location = new Point(6, 40),
						Size = new Size(61, 13),
						Text = "OpenId Provider",
					};

					TextBox tbOpenId = new TextBox()
					{
						Location = new Point(81, 37),
						Name = "m_tbOpenId",
						Size = new Size(373, 20),
						TabIndex = 5,
					};

					tbOpenId.Leave += OnTbOpenId_Leave;

					string[] providerNames = providers.Select(p => p.Name).ToArray();
					UIUtil.EnableAutoCompletion(tbOpenId, false, providerNames);

					//if editing, set the OpenID textbox value
					if (pefEntryForm.EditModeEx == PwEditMode.EditExistingEntry)
					{
						tbOpenId.Text = pefEntryForm.EntryRef.CustomData.Get("OpenIdProvider");
						provider = providers.Where(p => p.Name.Equals(tbOpenId.Text, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
					}
					Control[] tabEntryArray = pefEntryForm.Controls.Find("m_tabEntry", true);

					if (tabEntryArray.Length == 1)
					{
						tabEntryArray[0].Controls.Add(lblOpenId);
						tabEntryArray[0].Controls.Add(tbOpenId);
					}

					//hide the controls we don't want
					//password repeat
					Control[] lblPasswordRepeatArray = pefEntryForm.Controls.Find("m_lblPasswordRepeat", true);
					Control[] tbRepeatPasswordArray = pefEntryForm.Controls.Find("m_tbRepeatPassword", true);
					Control[] btnGenPwArray = pefEntryForm.Controls.Find("m_btnGenPw", true);

					if (lblPasswordRepeatArray.Length == 1)
					{
						lblPasswordRepeatArray[0].Visible = false;
					}

					if (tbRepeatPasswordArray.Length == 1)
					{
						tbRepeatPassword = (TextBox)tbRepeatPasswordArray[0];
						tbRepeatPassword.Visible = false;
					}

					if (btnGenPwArray.Length == 1)
					{
						btnGenPwArray[0].Visible = false;
					}

					//quality
					Control[] lblQualityArray = pefEntryForm.Controls.Find("m_lblQuality", true);
					Control[] pbQualityArray = pefEntryForm.Controls.Find("m_pbQuality", true);
					Control[] lblQualityInfoArray = pefEntryForm.Controls.Find("m_lblQualityInfo", true);

					if (lblQualityArray.Length == 1)
					{
						lblQualityArray[0].Visible = false;
					}

					if (pbQualityArray.Length == 1)
					{
						pbQualityArray[0].Visible = false;
					}

					if (lblQualityInfoArray.Length == 1)
					{
						lblQualityInfoArray[0].Visible = false;
					}

					//move the ones we keep
					//username
					Control[] lblUserNameArray = pefEntryForm.Controls.Find("m_lblUserName", true);
					Control[] tbUserNameArray = pefEntryForm.Controls.Find("m_tbUserName", true);

					if (lblUserNameArray.Length == 1)
					{
						lblUserNameArray[0].Location = new Point(6, 67);
					}

					if (tbUserNameArray.Length == 1)
					{
						tbUserName = (TextBox)tbUserNameArray[0];
						tbUserName.Location = new Point(81, 64);
						tbUserName.Enabled = false;
					}

					//password
					Control[] lblPasswordArray = pefEntryForm.Controls.Find("m_lblPassword", true);
					Control[] tbPasswordArray = pefEntryForm.Controls.Find("m_tbPassword", true);
					Control[] cbHidePasswordArray = pefEntryForm.Controls.Find("m_cbHidePassword", true);

					if (lblPasswordArray.Length == 1)
					{
						lblPasswordArray[0].Location = new Point(6, 94);
					}

					if (tbPasswordArray.Length == 1)
					{
						tbPassword = (TextBox)tbPasswordArray[0];
						tbPassword.Location = new Point(81, 91);
						tbPassword.Enabled = false;
					}

					if (cbHidePasswordArray.Length == 1)
					{
						cbHidePasswordArray[0].Location = new Point(423, 90);
						cbHidePasswordArray[0].Enabled = false;
					}

					//url
					Control[] lblUrlArray = pefEntryForm.Controls.Find("m_lblUrl", true);
					Control[] tbUrlArray = pefEntryForm.Controls.Find("m_tbUrl", true);

					if (lblUrlArray.Length == 1)
					{
						lblUrlArray[0].Location = new Point(6, 118);
					}

					if (tbUrlArray.Length == 1)
					{
						tbUrlArray[0].Location = new Point(81, 118);
					}

					//notes
					Control[] lblNotesArray = pefEntryForm.Controls.Find("m_lblNotes", true);
					Control[] rtNotesArray = pefEntryForm.Controls.Find("m_rtNotes", true);

					if (lblNotesArray.Length == 1)
					{
						lblNotesArray[0].Location = new Point(6, 144);
					}

					if (rtNotesArray.Length == 1)
					{
						rtNotesArray[0].Location = new Point(81, 144);
						rtNotesArray[0].Height = 160;
					}

					//set the references if any
					SetProviderReferences(provider);

					//reset our flag
					IsOpenIdEntry = false;
				}
			}
		}

		private void SaveCustomData()
		{
			string serializedProviders = string.Empty;
			foreach (OpenIdProvider provider in providers)
			{
				serializedProviders += string.Format("{0}|", JsonConvert.SerializeObject(provider));
			}

			dbPwDatabase.CustomData.Set("OpenIdProviders", serializedProviders);
			dbPwDatabase.Save(null);
		}

		private void SetProviderReferences(OpenIdProvider provider)
		{
			if (provider != null && !String.IsNullOrEmpty(provider.LinkedEntryUUID))
			{
				tbUserName.Text = string.Format(@"{{REF:U@I:{0}}}", provider.LinkedEntryUUID);
				tbPassword.Text = string.Format(@"{{REF:P@I:{0}}}", provider.LinkedEntryUUID);
				tbRepeatPassword.Text = string.Format(@"{{REF:P@I:{0}}}", provider.LinkedEntryUUID);
			}
		}
	}

	[Serializable]
	public class OpenIdProvider
	{
		public OpenIdProvider() { }
		public string Name { get; set; }
		public string LinkedEntryUUID { get; set; }
	}
}
