<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MainMenu.aspx.cs" Inherits="MainMenu" %>
<%@ Register src="DispatchHeader.ascx" tagname="DispatchHeader" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="default.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Denver Post</title>
</head>
<body>
    <form id="form1" runat="server" class="MainDiv" >
    
    <uc1:DispatchHeader ID="ucDispatchHeader" runat="server" />
    <br />
   
   <div>
    <asp:Button ID="btnPreferences" runat="server" Text="Set Preferences" 
        Width="170px" Class="Button"  onclick="btnPreferences_Click" 
           PostBackUrl="~/Preferences.aspx"/>
    <asp:Button ID="btnToDispatch" runat="server" Text="Dispatch List" 
        Width="170px" Class="Button" 
           onclick="btnToDispatch_Click" PostBackUrl="~/ToDispatch.aspx"/>
    <asp:Button ID="btnZoneSummary" runat="server" Text="Summary Report" 
        Width="170px" Class="Button" 
           onclick="btnZoneSummary_Click"/>
    <asp:Button ID="btnDeliveryReport" runat="server" Text="Delivery Report" 
        Width="170px" Class="Button" onclick="btnDeliveryReport_Click"/>
    <asp:Button ID="btnCustomerSearch" runat="server" Text="Customer Lookup" 
        Width="170px" Class="Button" 
           onclick="btnCustomerSearch_Click"/>
    </div>
    <br />
    <asp:Button ID="btnSyncronex" runat="server" Text="Synchronex" 
        Width="170px" 
        PostBackUrl="http://exwebserver1/SingleCopy/SelectCompany.asp" 
        Class="Button" onclick="btnSyncronex_Click"/>    
    <asp:Button ID="btnReveal" runat="server" Text="Reveal" 
        Width="170px" PostBackUrl="http://65.101.206.184/revealjavaweb/exd.html" 
        Class="Button" onclick="btnReveal_Click"/>    
        
    </form>
</body>
</html>
