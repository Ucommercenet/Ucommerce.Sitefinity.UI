<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogItemSelector.ascx.cs" Inherits="UCommerce.Web.UI.Catalog.Dialogs.CatalogItemSelector" %>

<div class="propertyItem">
	<table class="CatalogItemSelectorTable">
		<tr>
			<th><%= GetLocalResourceObject("CatalogItems.Text") %></th>
			<th><%= GetLocalResourceObject("Items.Text") %></th>
			<th></th>
			<th><asp:PlaceHolder id="SelectedItemsLabelPlaceHolder" runat="server"><%= GetLocalResourceObject("SelectedItems.Text") %></asp:PlaceHolder></th>
		</tr>
		<tr>
			<td valign="top" style="width:99%; padding-right:10px;">
			    <div class="TreeViewWrapper CatalogItemSelectorTreeView row-hover large">
				    <asp:TreeView ID="SelectorTreeView" runat="server" OnSelectedNodeChanged="SelectorTreeView_OnSelectedNodeChanged"  />		   
			    </div>
			</td>
			<td valign="top" class="CatalogItemsTd">
				<asp:ListBox ID="CatalogItemsListBox" runat="server" SelectionMode="Multiple" DataTextField="Text" DataValueField="Value" CssClass="CatalogItemsListBox"/>
			</td>
			<td>
				<asp:Button ID="AddRelationsButton" CssClass="tinyButton" runat="server" Text=" > " OnClick="AddRelationsButton_Click" />
				<br />
				<asp:Button ID="RemoveRelationsButton" CssClass="tinyButton" runat="server" Text=" < " OnClick="RemoveRelationsButton_Click" />
			</td>
			<td valign="top" class="CatalogItemsTd">
				<asp:ListBox ID="SelectedCatalogItemListBox" runat="server" SelectionMode="Multiple" DataTextField="Text" DataValueField="Value" CssClass="SelectedCatalogItemListBox" />		
			</td>
		</tr>
	</table>
</div>