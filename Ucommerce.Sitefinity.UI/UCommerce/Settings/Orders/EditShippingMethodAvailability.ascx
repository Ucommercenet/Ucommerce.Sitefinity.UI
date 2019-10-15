<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditShippingMethodAvailability.ascx.cs" Inherits="UCommerce.Web.UI.Settings.Orders.EditShippingMethodAvailability" %>
<%@ Register tagPrefix="commerce" tagName="ValidationSummary" src="../../Controls/ValidationSummaryDisplay.ascx" %>

<commerce:ValidationSummary runat="server" />
<div class="propertyPane leftAligned">
    <commerce:PropertyPanel runat="server" meta:resourceKey="ProductCatalogGroups">
        <asp:CheckBoxList runat="server" ID="ProductCatalogGroupsCheckBoxList" DataSource="<%# GetStores() %>" DataValueField="Value" DataTextField="Text" ></asp:CheckBoxList>
    </commerce:PropertyPanel>
    <div class="propertyPaneFooter">-</div>
</div>

<div class="propertyPane leftAligned">
    <commerce:PropertyPanel runat="server" meta:resourceKey="ValidCountries">
        <asp:CheckBoxList runat="server" ID="CountryCheckBoxList" DataSource="<%# GetCountries() %>" DataValueField="Value" DataTextField="Text" />
    </commerce:PropertyPanel>
    <div class="propertyPaneFooter">-</div>
</div>

<div class="propertyPane leftAligned">
    <commerce:PropertyPanel runat="server" meta:resourceKey="ValidPaymentMethods">
        <asp:CheckBoxList runat="server" ID="PaymentMethodsCheckBoxList" DataSource="<%# GetPaymentMethods() %>" DataValueField="Value" DataTextField="Text" />
    </commerce:PropertyPanel>
    <div class="propertyPaneFooter">-</div>
</div>