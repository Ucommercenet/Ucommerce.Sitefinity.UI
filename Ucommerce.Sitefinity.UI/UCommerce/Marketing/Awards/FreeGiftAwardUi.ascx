<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FreeGiftAwardUi.ascx.cs" Inherits="UCommerce.Web.UI.UCommerce.Marketing.Awards.FreeGiftAwardUi" %>
<table cellpadding="0" cellspacing="0" style="width:100%;">
	<tr>
		<td>
			<asp:PlaceHolder runat="server" id="NoProductSelectedPlaceHolder">
				<asp:Localize id="NoProductSelectedLabel" runat="server" meta:resourcekey="NoProductSelected" />
			</asp:PlaceHolder>

			<asp:PlaceHolder runat="server" id="ProductInfoPlaceHolder" visible="false">
				<asp:Localize id="NameLabel" runat="server" meta:resourcekey="Name" />: <asp:Label runat="server" id="ProductNameLabel" /><br />
				<asp:Localize id="SkuLabel" runat="server" meta:resourcekey="Sku" />: <asp:Label runat="server" id="ProductSkuLabel" /><br />
				<asp:PlaceHolder runat="server" id="VariantSkuPlaceholder" Visible="false">
					<asp:Localize id="VariantSkuLabel" runat="server" meta:resourcekey="VariantSku" />: <asp:Label runat="server" id="ProductVariantSkuLabel" /><br />
				</asp:PlaceHolder>
			</asp:PlaceHolder>
		</td>
		<td style="width:50px; text-align:right; vertical-align:top;">
			<asp:ImageButton id="SelectProductButton" runat="server" ImageUrl="../../Images/ui/pencil.png" meta:resourcekey="Edit" />
			<asp:ImageButton id="DeleteTargetButton" runat="server" ImageUrl="../../Images/ui/cross.png" meta:resourcekey="Delete" onclick="DeleteTargetButton_Click" /><br />
		</td>
	</tr>
</table>