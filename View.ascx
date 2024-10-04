<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="GIBS.Modules.GIBS_ListManager.View" %>

<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="row ">

 <div class="col-md-3">

<asp:DropDownList ID="DropDownListSelectList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" ></asp:DropDownList>
<div style="padding:6px; text-align:center;">
<asp:LinkButton ID="LinkButtonAddNewList" runat="server" CssClass="btn btn-primary" OnClick="LinkButtonAddNewList_Click">Add New List</asp:LinkButton> 
</div>

</div>



<div class="col-md-9">

    <h5 style="text-align:center;"><asp:Label ID="Label1" runat="server" Text="Label"></asp:Label></h5>


    <div id="rowListdetails" runat="server" visible="false">
        <div class="dnnFormItem">
            <dnn:Label ID="plListName" runat="server" ControlName="lblListName" />
            <asp:Label ID="lblListName" runat="server" />
        </div>
        <div id="rowListParent" runat="server" class="dnnFormItem">
            <dnn:Label ID="plListParent" runat="server" ControlName="lblListParent" />
            <asp:Label ID="lblListParent" runat="server" Text="- none -" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEntryCount" runat="server" ControlName="lblEntryCount" />
            <asp:Label ID="lblEntryCount" runat="server" /> 
        </div>
 
    </div>


<asp:GridView ID="GV_ListItems" runat="server" AutoGenerateColumns="false" OnRowDataBound="GV_ListItems_RowDataBound" OnRowCommand="GV_ListItems_RowCommand" OnRowDeleting="GV_ListItems_RowDeleting" CssClass="table table-striped table-bordered table-list" >
    <Columns>
    <asp:BoundField ItemStyle-Width="150px" DataField="Text" HeaderText="Text" />
    <asp:BoundField ItemStyle-Width="150px" DataField="Value" HeaderText="Value" />
        
    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="Move Up">
        <ItemTemplate>
            <asp:HiddenField ID="Hid_ParentID" Value='<%# Bind("ParentID")%>' runat="server" />
	        <asp:ImageButton CommandName="up" ID="btnUp" ImageUrl='/icons/Sigma/Up_16X16_Standard.png' CommandArgument='<%# Bind("EntryID")%>' runat="server" />
        </ItemTemplate>
    </asp:TemplateField>
	    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Move Down">
            <ItemTemplate>
				<asp:ImageButton CommandName="down" ID="btnDown" ImageUrl='/icons/Sigma/Dn_16X16_Standard.png' CommandArgument='<%# Bind("EntryID")%>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
	    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Edit">
            <ItemTemplate>
				<asp:ImageButton CommandName="edititem" ID="btnEdit" CausesValidation="false" ImageUrl='/icons/Sigma/Edit_16X16_Standard.png' CommandArgument='<%# Bind("EntryID")%>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
		
	    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Delete">
            <ItemTemplate>
				<asp:ImageButton CommandName="deleteme" ID="btnDelete" ImageUrl='/icons/Sigma/Delete_16X16_Standard.png' CommandArgument='<%# Bind("EntryID")%>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    <asp:BoundField ItemStyle-Width="150px" DataField="ParentKey" HeaderText="ParentKey" Visible="false" />
</Columns>

</asp:GridView>
       
    <div id="rowAddNewItemDeleteList" runat="server">
        <asp:LinkButton ID="LinkButtonAddNewItem" runat="server" OnClick="LinkButtonAddNewItem_Click" CssClass="btn btn-primary">Add New Item</asp:LinkButton> 
        
        <asp:LinkButton ID="LinkButtonDeleteList" runat="server" OnClick="LinkButtonDeleteList_Click" CssClass="btn btn-primary">Delete List</asp:LinkButton>
    </div>

 <asp:HiddenField ID="HiddenFieldEntryID" runat="server" />
    <asp:HiddenField ID="HiddenFieldListName" runat="server" />
    <asp:HiddenField ID="HiddenFieldParentKey" runat="server" />
    <asp:HiddenField ID="HiddenFieldParentID" runat="server" />
    <asp:HiddenField ID="HiddenFieldMode" runat="server" />
    <asp:HiddenField ID="HiddenFieldEnableSortOrder" runat="server" />
    <div id="rowEntryEdit" runat="server" visible="false" class="dnnForm">
       
        <div id="rowListName" runat="server" class="dnnFormItem">
            <dnn:Label ID="plEntryName" Text="Entry Name:" runat="server" ControlName="txtEntryName" CssClass="dnnFormRequired" />
			<asp:TextBox ID="txtEntryName" runat="server" MaxLength="100" />
            <asp:RequiredFieldValidator ID="valEntryName" CssClass="dnnFormMessage dnnFormError" runat="server" resourcekey="valEntryName.ErrorMessage" Display="Dynamic" ControlToValidate="txtEntryName" />
			
        </div>
        <div  id="rowSelectList" runat="server" class="dnnFormItem">
            <dnn:Label ID="plSelectList" runat="server" ControlName="ddlSelectList "/>
           
            <asp:DropDownList ID="ddlSelectList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSelectList_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div  id="rowSelectParent" runat="server" class="dnnFormItem">
            <dnn:Label ID="plSelectParent" runat="server" ControlName="ddlSelectParent" />
            
            <asp:DropDownList ID="ddlSelectParent" runat="server" Enabled="false" CausesValidation="False"></asp:DropDownList>
        </div>
        <div class="dnnFormItem" id="rowEntryText" runat="server">
            <dnn:Label ID="plEntryText" runat="server" ControlName="txtEntryText" CssClass="dnnFormRequired" />
			<asp:TextBox ID="txtEntryText" runat="server" MaxLength="100" />
			<asp:RequiredFieldValidator ID="valEntryText" CssClass="dnnFormMessage dnnFormError" runat="server" resourcekey="valEntryText.ErrorMessage" Display="Dynamic" ControlToValidate="txtEntryText" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEntryValue" runat="server" ControlName="txtEntryValue" CssClass="dnnFormRequired"/>
			<asp:TextBox ID="txtEntryValue" runat="server" MaxLength="100" />
			<asp:RequiredFieldValidator ID="valEntryValue" CssClass="dnnFormMessage dnnFormError" runat="server" resourcekey="valEntryValue.ErrorMessage" Display="Dynamic" ControlToValidate="txtEntryValue" />
        </div>
        <div id="rowEnableSortOrder" runat="server" class="dnnFormItem">
			<dnn:Label ID="plEnableSortOrder" runat="server" ControlName="chkEnableSortOrder"/>
			<asp:CheckBox ID="chkEnableSortOrder" runat="server"/>
        </div>
        <ul class="dnnActions dnnClear">
    	    <li><asp:LinkButton id="cmdSaveEntry" runat="server" OnClick="cmdSaveEntry_Click1" CssClass="dnnPrimaryAction" resourcekey="cmdSave" /></li>
    	    
    	    <li><asp:LinkButton id="cmdCancel" runat="server" OnClick="cmdCancel_Click" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" /></li>
        </ul>   
    </div>



    </div>


    </div>