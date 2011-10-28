<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IpAddressGrant.aspx.cs" Inherits="IisIpSecurity.IpAddressGrant" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>IP Address Grant</title>
</head>
<body>
    <h1>Deny All Access Except IP Address Below</h1>
    <a href="IpAddressGrant.aspx">Reload Page</a>
    <form id="form1" runat="server">
    
    <div>    
        
        <table cellpadding="10">
        <tr><td valign="top">
        
        <asp:Button runat="server" ID="btnChange" Text="Update" OnClick="btnUpdate_Click"/>
        <br />
        <asp:TextBox ID="txtAddresses" runat="server" TextMode="MultiLine" 
                     Height="264px" Width="250"></asp:TextBox><br />        
        <small><asp:Label runat="server" ID="lblIpCount"></asp:Label></small>
        </td>
        <td valign="top" style="padding:30px;">        
        <b>Metabase Path:</b><br />
        <asp:TextBox runat="server" ID="txtMetaBasePath" width="300"></asp:TextBox>
        <br />
        <b>Select a Web Sites:</b><br />
        <asp:DropDownList runat="server" ID="lstSites" Width="300" onchange="onSiteChanged()"/>
        <br />
        <br />
        <b>Login Name / System Account:</b><br />
         <%= User.Identity.Name %> - <%= Environment.UserName %>
         <br />
         <br />
         <asp:Button runat="server" ID="btnRefresh" Text="Refresh" OnClick="btnRefresh_Click"/>
        </td>
        </tr>
        </table>
        
    </div>
<script type="text/javascript">
function onSiteChanged()
{
    var list = document.getElementById("lstSites");
    var txt = document.getElementById("txtMetaBasePath");    
    txt.value = list.value + "/ROOT";
}
</script>    
    </form>
</body>
</html>
