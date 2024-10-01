/*
' Copyright (c) 2024  GIBS.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Common.Lists;
using System;
using System.Web.UI.WebControls;
using System.Reflection.Emit;
using static DotNetNuke.Entities.Portals.PortalSettings;
using System.Globalization;
using DotNetNuke.Common.Utilities;
using System.Web;
using System.Web.UI;
using System.Linq;
using DotNetNuke.Collections;

namespace GIBS.Modules.GIBS_ListManager
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from GIBS_ListManagerModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : GIBS_ListManagerModuleBase, IActionable
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               

                if (!IsPostBack)
                {
                    Label1.Text = "<=== Select List to Manage";
                    FillDropDown();

                    LinkButtonAddNewItem.Visible = false;
                    LinkButtonDeleteList.Visible = false;


                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public void FillDropDown()
        {


            try
            {


                var ctlLists = new ListController();
                var colLists = ctlLists.GetListInfoCollection(string.Empty, string.Empty, PortalSettings.ActiveTab.PortalID);

                DropDownListSelectList.DataTextField = "DisplayName";
                DropDownListSelectList.DataValueField = "DisplayName";

                DropDownListSelectList.DataSource = colLists;

                DropDownListSelectList.DataBind();
                DropDownListSelectList.Items.Insert(0, new ListItem("-- Select List --", "0"));

                ddlSelectList.DataTextField = "DisplayName";
                ddlSelectList.DataValueField = "Key";
                ddlSelectList.DataSource = colLists;
                ddlSelectList.DataBind();
                ddlSelectList.Items.Insert(0, new ListItem("-- Select Parent List --", ""));


                //                ddlSelectList.Items
                //.Cast<ListItem>()
                //.Where(item => !permittedChoices.Contains(item.Text))
                //.ToList()
                //.ForEach(ddlSelectList.Items.Remove);

                ddlSelectList.Items.Cast<ListItem>()
                         .Where(x => x.Value.Contains(":"))
                        .ToList()
                        .ForEach(ddlSelectList.Items.Remove);


                //for (int i = 0; i < ddlSelectList.Items.Count; i++)
                //{


                //    //ddlSelectList.Items.Remove(removeItem);
                //}

                //for (int i = ddlSelectList.Items.Count - 1; i >= 0; i--)
                //{
                //    if (ddlSelectList.Items[i].Text.ToString().Contains(":"))
                //    {
                //        ddlSelectList.Items.Remove(ddlSelectList.Items[i].Value.ToString());
                //        break;
                //    }
                // }


            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }



        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

            FillGrid(DropDownListSelectList.SelectedValue.ToString());
            DisableEditView();
          
        }

        public void FillGrid(string selectedList)
        {


            try
            {
                // get the ID of the clicked row
                string key = selectedList.ToString();
                string parentKey = string.Empty;
                string myList = key.ToString();
                Label1.Text = "";

                
                

                if (key.ToString().Contains(":"))
                {
                    char separator = ':'; // Space character
                    string[] keys = key.Split(separator); // returned array
                    parentKey = keys[0].ToString();
                    myList = keys[1].ToString();
                   // Label1.Text = myList + "s in " + parentKey.ToString().Replace(".", " of ");
                    lblListParent.Text = parentKey;
                }
                else
                {
                    lblListParent.Text = "- none -";
                    HiddenFieldParentID.Value = string.Empty;
                }
                lblListName.Text = myList;
                HiddenFieldListName.Value = myList;
                HiddenFieldParentKey.Value = parentKey;
                var ctlLists = new ListController();
                //    ListInfo selList = (ListInfo)ctlLists.GetListEntryInfoItems(selectedList.ToString(), string.Empty, this.PortalId);

                ;
          //      GV_ListItems.DataSource = ctlLists.GetListEntryInfoItems(selectedList.ToString(), string.Empty, this.PortalId);
                GV_ListItems.DataSource = ctlLists.GetListEntryInfoItems(myList.ToString(), parentKey.ToString(), this.PortalId);
                GV_ListItems.DataBind();
                rowListdetails.Visible = true;

                if (GV_ListItems.Rows.Count < 1)
                {
                    // rowListdetails.Visible = true;
                    lblEntryCount.Text = "0";   // + Localization.GetString("Entries", LocalResourceFile);
                }
                else
                {
                    // GV_ListItems.Visible = true;

                    lblEntryCount.Text = GV_ListItems.Rows.Count.ToString();    // + " " + Localization.GetString("Entries", LocalResourceFile);
                    HiddenFieldMode.Value = "EditList";
                }

                LinkButtonAddNewItem.Visible = true;
                LinkButtonDeleteList.Visible = true;


            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }

        protected void GV_ListItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var ctlLists = new ListController();
            int entryID = Int32.Parse(e.CommandArgument.ToString());

            switch (e.CommandName.ToLower())
            {
                case "deleteme":
                   
                    ctlLists.DeleteListEntryByID(entryID, true);
               

                    break;
                case "edititem":
                    //Mode = "EditEntry";
                    EnableEditView();

                    ListEntryInfo entry = ctlLists.GetListEntryInfo(entryID);

                   
                    rowSelectList.Visible = false;
                    rowSelectParent.Visible = false;
                    rowListName.Visible = false;
                    rowEnableSortOrder.Visible = false;

                    HiddenFieldEntryID.Value = entryID.ToString(CultureInfo.InvariantCulture);
                    HiddenFieldParentKey.Value = entry.ParentKey;
                   
                    txtEntryValue.Text = entry.Value;
                    txtEntryText.Text = entry.Text;
                    txtEntryName.Text = entry.ListName;
                  //  rowListName.Visible = false;
                   // cmdSaveEntry.CommandName = "Update";

                    //if (!SystemList)
                    //{
                    //    cmdDelete.Visible = true;
                    //    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"));
                    //}
                    //else
                    //{
                    //    cmdDelete.Visible = false;
                    //}
                    ////  e.Canceled = true;  //stop the grid from providing inline editing
                    DataBind();
                    break;
                case "up":
                    ctlLists.UpdateListSortOrder(entryID, true);
                    DataBind();
                   // FillGrid(DropDownListSelectList.SelectedValue.ToString());
                    break;
                case "down":
                    ctlLists.UpdateListSortOrder(entryID, false);
                    DataBind();
                    break;
            }

            FillGrid(DropDownListSelectList.SelectedValue.ToString());
        }

        protected void GV_ListItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }



        private void EnableEditView()
        {
            //rowListdetails.Visible = true;
            GV_ListItems.Visible = false;
            rowAddNewItemDeleteList.Visible = false;
            rowEntryEdit.Visible = true;
        }

        private void DisableEditView()
        {
            //rowListdetails.Visible = true;
            GV_ListItems.Visible = true;
            rowEntryEdit.Visible = false;
            rowAddNewItemDeleteList.Visible = true;
            
        }

        protected void cmdSaveEntry_Click1(object sender, EventArgs e)
        {
            var entry = new ListEntryInfo();
            var listController = new ListController();

            var Mode = HiddenFieldMode.Value.ToString();
            var currentCountOrder = lblEntryCount.Text.ToString().Trim();

           
            string entryValue;
            string entryText;
            if (UserInfo.IsSuperUser)
            {
                entryValue = txtEntryValue.Text;
                entryText = txtEntryText.Text;
            }
            else
            {
                //    var ps = new PortalSecurity();

                entryValue = HttpUtility.HtmlEncode(txtEntryValue.Text);
                entryText = HttpUtility.HtmlEncode(txtEntryText.Text);
            }

            entry.Value = entryValue;
            entry.Text = entryText;
            entry.Description = string.Empty;


            var listName = HiddenFieldListName.Value.ToString();
            var parentKey = HiddenFieldParentKey.Value.ToString();

            switch (Mode.ToLower())
            {
                case "editlist":
                    entry.EntryID = Int32.Parse(HiddenFieldEntryID.Value.ToString());
                    entry.ListName = listName.ToString();
                    listController.UpdateListEntry(entry);

                    break;

                case "addnewitem":
                    entry.ListName = listName.ToString();
                    if (currentCountOrder.ToString().Length > 0)
                    {
                        entry.SortOrder = Int32.Parse(currentCountOrder.ToString()) + 1;
                    }
                    else
                    {
                        entry.SortOrder = 0;
                    }
                    if (HiddenFieldParentID.Value.ToString().Length > 0 && HiddenFieldParentID.Value.ToString() != "0")
                    {
                        entry.ParentID = Int32.Parse(HiddenFieldParentID.Value.ToString());
                        entry.Level = 1;
                    }
                    else
                    {
                        entry.ParentID = 0;
                        entry.Level = 0;
                    }
                    
                    entry.DefinitionID = -1;
                    entry.PortalID = this.PortalId;
                    entry.SystemList = false;

                    listController.AddListEntry(entry);
                    break;

                case "addnewlist":
                    entry.ListName = txtEntryName.Text.ToString();
                    listName = txtEntryName.Text.ToString();

                    if (chkEnableSortOrder.Checked)
                    {
                        entry.SortOrder = 1;
                    }
                    else
                    {
                        entry.SortOrder = 0;
                    }

                    if (ddlSelectParent.SelectedIndex != -1)
                    {
                        entry.ParentID = Int32.Parse(ddlSelectParent.SelectedValue.ToString());
                        entry.Level = 1;
                        parentKey = ddlSelectParent.SelectedItem.ToString().Replace(":", ".");
                    }
                    else
                    {
                        entry.ParentID = 0;
                        entry.Level = 0;
                        parentKey = "";
                    }

                    entry.DefinitionID = -1;
                    entry.PortalID = this.PortalId;
                    entry.SystemList = false;                

                    listController.AddListEntry(entry);

                    ddlSelectList.SelectedIndex = -1;
                    ddlSelectParent.DataSource = null;
                    ddlSelectParent.DataBind();
                    ddlSelectParent.Enabled = false;

                    break;

            }

            GV_ListItems.DataSource = null;
            GV_ListItems.DataBind();

            DataCache.ClearCache();
            string parent = string.Empty;
            if (parentKey.ToString().Length > 0)
            {
                parent = parentKey.ToString() + ":";
            }

            FillGrid(parent + listName.ToString());
            FillDropDown();
            DropDownListSelectList.SelectedValue = parent + listName;
            DisableEditView();


        }

        protected void cmdSaveEntry_Click(object sender, EventArgs e)
        {
            
            
            string entryValue;
            string entryText;
            if (UserInfo.IsSuperUser)
            {
                entryValue = txtEntryValue.Text;
                entryText = txtEntryText.Text;
            }
            else
            {
            //    var ps = new PortalSecurity();

                entryValue = HttpUtility.HtmlEncode(txtEntryValue.Text);
                entryText = HttpUtility.HtmlEncode(txtEntryText.Text);
            }
            var listController = new ListController();
            
            var listName = HiddenFieldListName.Value.ToString();
            var parentKey = HiddenFieldParentKey.Value.ToString();
            var entry = new ListEntryInfo();
            var currentCountOOrder = lblEntryCount.Text.ToString().Trim();
            if(currentCountOOrder == "")
            {
                currentCountOOrder = "0";
            }

            {
                if (HiddenFieldEntryID.Value.ToString().Length > 0)
                {
                    entry.EntryID = Int32.Parse(HiddenFieldEntryID.Value.ToString());
                }
                
                entry.Value = entryValue;
                entry.Text = entryText;
                entry.Description = string.Empty;
                
            }
            if (HiddenFieldEntryID.Value.ToString().Length > 0)
            {

                entry.ListName = listName.ToString();
                listController.UpdateListEntry(entry);
            }
            else
            {
                
                if (txtEntryName.Text.ToString().Length > 0)
                {
                    entry.ListName = txtEntryName.Text.ToString();
                    listName = txtEntryName.Text.ToString();
                }
                else
                {
                    entry.ListName = listName.ToString();
                }
                if(chkEnableSortOrder.Checked || currentCountOOrder.ToString().Length > 0)
                {
                    entry.SortOrder = Int32.Parse(currentCountOOrder.ToString()) + 1;
                }
                else
                {
                    entry.SortOrder = 0;
                }
                if(HiddenFieldParentID.Value.ToString().Length > 0 && HiddenFieldParentID.Value.ToString() != "0")
                { 
                    entry.ParentID = Int32.Parse(HiddenFieldParentID.Value.ToString());
                    entry.Level = 1;
                }
                else
                {
                    entry.ParentID = 0;
                    entry.Level = 0;
                }
                if (HiddenFieldMode.Value == "AddNewList")
                {
                    if (ddlSelectParent.SelectedIndex != -1)
                    {
                        entry.ParentID = Int32.Parse(ddlSelectParent.SelectedValue.ToString());
                        entry.Level = 1;
                    //    parentKey = ddlSelectList.SelectedValue.ToString();
                    }
                    else
                    {
                        entry.ParentID = 0;
                        entry.Level = 0;
                   //     parentKey = "";
                    }
                }

                entry.DefinitionID = -1;
                entry.PortalID = this.PortalId;
                entry.SystemList = false;

                

                listController.AddListEntry(entry);
            }
            GV_ListItems.DataSource = null;
            GV_ListItems.DataBind();

            DataCache.ClearCache();
            string parent = string.Empty;
            if (parentKey.ToString().Length > 0)
            {
                parent= parentKey.ToString() + ":";
            }

            FillGrid(parent + listName.ToString());
            FillDropDown();
            DropDownListSelectList.SelectedValue = parent + listName;
            DisableEditView();
        }

        //public string ListName
        //{
        //    get
        //    {
        //        return HttpUtility.HtmlEncode(ViewState["ListName"] != null ? ViewState["ListName"].ToString() : "");
        //    }
        //    set
        //    {
        //        ViewState["ListName"] = value;
        //    }
        //}
        protected void LinkButtonAddNewList_Click(object sender, EventArgs e)
        {
           
            HiddenFieldMode.Value = "AddNewList";
            HiddenFieldListName.Value = string.Empty;
            HiddenFieldParentKey.Value = string.Empty;
            HiddenFieldParentID.Value = string.Empty;
            HiddenFieldEntryID.Value = string.Empty;

            Label1.Text = "Add a New List";

            txtEntryValue.Text = "";
            txtEntryText.Text = "";
            txtEntryName.Text = "";


            rowSelectList.Visible = true;
            rowSelectParent.Visible = true;
            rowListName.Visible = true;
            rowEnableSortOrder.Visible = true;

            DropDownListSelectList.SelectedIndex = 0;
            
            EnableEditView();
            rowListdetails.Visible = false;


        }

        protected void LinkButtonAddNewItem_Click(object sender, EventArgs e)
        {
           
           
            rowSelectList.Visible = false;
            rowSelectParent.Visible = false;
            rowListName.Visible = false;
            rowEnableSortOrder.Visible = false;
            
            HiddenFieldEntryID.Value = string.Empty;
            //HiddenFieldListName.Value = string.Empty;
            //HiddenFieldParentKey.Value = string.Empty;
            //HiddenFieldParentID.Value = string.Empty;

            txtEntryText.Text = string.Empty;
            txtEntryValue.Text = string.Empty;
            HiddenFieldMode.Value = "AddNewItem";
            valEntryName.Enabled = false;
            Label1.Text = "Add New Item";
            EnableEditView();
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            DisableEditView();
            if (HiddenFieldMode.Value == "AddNewList")
            {
                ddlSelectList.SelectedIndex = -1;
                ddlSelectParent.Items.Clear();
                ddlSelectParent.Enabled = false;
                HiddenFieldMode.Value = "";
                Label1.Text = "<=== Select List to Manage";
            }
            if (HiddenFieldMode.Value == "AddNewItem")
            {
                valEntryName.Enabled = true;
                string myList = HiddenFieldListName.Value.ToString();
                Label1.Text = "";
            }
        }

        protected void GV_ListItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField myhidParentID = (HiddenField)e.Row.FindControl("Hid_ParentID");

                if (myhidParentID.Value.ToString() != "0")
                {
                    HiddenFieldParentID.Value = myhidParentID.Value.ToString();
                }
                else
                {
                    HiddenFieldParentID.Value = "";
                }
                
            }
        }

        protected void LinkButtonDeleteList_Click(object sender, EventArgs e)
        {
            var listController = new ListController();
            string myList = HiddenFieldListName.Value;
            string parentList = HiddenFieldParentKey.Value;
            listController.DeleteList(myList, parentList);

            GV_ListItems.DataSource = null;
            GV_ListItems.DataBind();

            DataCache.ClearCache();

            GV_ListItems.Visible = false;
            rowListdetails.Visible = false;
            rowAddNewItemDeleteList.Visible = false;
            FillDropDown();
            Label1.Text = "<=== Select List to Manage";



        }

        protected void ddlSelectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ctlLists = new ListController();
            if (!string.IsNullOrEmpty(ddlSelectList.SelectedValue))
            {
                ListInfo selList = GetList(ddlSelectList.SelectedItem.Value, false);
                {
                    ddlSelectParent.Enabled = true;
                    ddlSelectParent.DataSource = ctlLists.GetListEntryInfoItems(selList.Name, selList.ParentKey);
                    ddlSelectParent.DataTextField = "DisplayName";
                    ddlSelectParent.DataValueField = "EntryID";
                    ddlSelectParent.DataBind();

                    //var removeItem = ddlSelectParent.Items.Cast<ListItem>()
                    //      .Where(x => x.Value.Contains(":"))
                    //      .FirstOrDefault();

                    //ddlSelectParent.Items.Remove(removeItem);
                }
            }
            else
            {
                ddlSelectParent.Enabled = false;
                ddlSelectParent.Items.Clear();
            }
        }

        private ListInfo GetList(string key, bool update)
        {
            var ctlLists = new ListController();
            int index = key.IndexOf(":", StringComparison.Ordinal);
            string listName = key.Substring(index + 1);
            string parentKey = Null.NullString;
            if (index > 0)
            {
                parentKey = key.Substring(0, index);
            }
            if (update)
            {
                
                HiddenFieldListName.Value = listName;
                HiddenFieldParentKey.Value = parentKey;
            }
            return ctlLists.GetListInfo(listName, parentKey, this.PortalId);
        }

    }
}