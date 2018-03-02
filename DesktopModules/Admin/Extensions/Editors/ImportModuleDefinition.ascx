<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.ImportModuleDefinition" CodeFile="ImportModuleDefinition.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<br>
<table width="560" cellspacing="0" cellpadding="0" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" width="100" valign="top"><dnn:label id="plControl" text="Control:" controlname="cboControl" runat="server" /></td>
        <td valign="top"><asp:dropdownlist id="cboControl" runat="server" width="390" cssclass="NormalTextBox" /></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <dnn:commandbutton id="cmdImportControl" resourcekey="cmdImportControl" text="Import Control" runat="server" class="CommandButton" ImageUrl="~/images/add.gif" CausesValidation="False" />
        </td>
    </tr>
    <tr>
        <td colspan="2">&nbsp;</td>
    </tr>
    <tr>
        <td class="SubHead" width="100" valign="top"><dnn:label id="plManifest" text="Manifest:" controlname="cboManifest" runat="server" /></td>
        <td valign="top">
            <asp:dropdownlist id="cboManifest" runat="server" width="390" cssclass="NormalTextBox"></asp:dropdownlist>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <dnn:commandbutton id="cmdImportManifest" resourcekey="cmdImportManifest" text="Import Manifest" runat="server" class="CommandButton" ImageUrl="~/images/add.gif" CausesValidation="False" />
        </td>
    </tr>
</table>
<p align="center">
	<dnn:commandbutton id="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" ResourceKey="cmdCancel" causesvalidation="False" />&nbsp;
</p>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
    <tr><td align="left" colspan="2"><asp:Label ID="lblInstallMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /></td></tr>
    <tr><td><asp:PlaceHolder ID="phInstallLogs" runat="server" /></td></tr>
</table>
