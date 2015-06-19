<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Preferences.aspx.cs" Inherits="Preferences" EnableViewState="true" %>
<%@ Register src="SelectedDistricts.ascx" tagname="SelectedDistricts" tagprefix="uc1" %>
<%@ Register src="SelectZoneAndDistrict.ascx" tagname="SelectZoneAndDistrict" tagprefix="uc2" %>
<%@ Register src="DispatchHeader.ascx" tagname="DispatchHeader" tagprefix="uc3" %>
<link href="CPhone.css" rel="stylesheet" type="text/css" />
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Denver Post</title>
</head>
<body>
    <form id="form1" runat="server" enableviewstate="true">
    <div>
        <uc3:DispatchHeader ID="ucDispatchHeader" runat="server" />
        <uc1:SelectedDistricts ID="ucSelectedDistricts" runat="server" />
        <br />
        <uc2:SelectZoneAndDistrict ID="ucSelectZoneAndDistrict" runat="server" />
    </div>

    <hr />
    <br />

    <br />

    <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Underline="True" 
        Text="Preferences" EnableViewState="False" Font-Size="Medium"></asp:Label>
    <br />
    <br />
    <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Requests to Display:" 
        EnableViewState="False" Font-Size="Small"></asp:Label>
    <br />
    <asp:RadioButtonList ID="rblViewType" runat="server" EnableViewState="false" >
        <asp:ListItem Value="UNDELIVERED" Selected="True">Undelivered Requests</asp:ListItem>
        <asp:ListItem Value="ALL">All</asp:ListItem>
    </asp:RadioButtonList>
    <br />
    <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Sort by:" 
        EnableViewState="False" Font-Size="Small"></asp:Label>
    <br />
    <asp:RadioButtonList ID="rblSortOrder" runat="server" TabIndex="5" 
        EnableViewState="False" Font-Size="Small">
        <asp:ListItem Value="RouteCode" Selected="True">Route</asp:ListItem>
        <asp:ListItem>Address</asp:ListItem>
        <asp:ListItem Value="PublicationDate">Publication Date</asp:ListItem>
        <asp:ListItem Value="ElapsedTime">Elapsed Time</asp:ListItem>
    </asp:RadioButtonList>
    <br />
    <asp:Label ID="Label5" runat="server" Font-Bold="True" Text="Colors:" 
        EnableViewState="False" Font-Size="Small"></asp:Label>
    <br />
    <asp:Label ID="lblEvenItem" runat="server" Text="Even Item" 
        EnableViewState="False" Font-Size="Small" Width="100px"></asp:Label>
    <asp:DropDownList ID="ddlColorEven" runat="server" TabIndex="7" 
        EnableViewState="False" Font-Size="Small">
        <asp:ListItem Value="ColorBrown">Brown</asp:ListItem>
        <asp:ListItem Value="ColorOrange">Orange</asp:ListItem>
        <asp:ListItem Value="ColorYellow">Yellow</asp:ListItem>
        <asp:ListItem Value="ColorGreen">Green</asp:ListItem>
        <asp:ListItem Value="ColorBlue">Blue</asp:ListItem>
        <asp:ListItem Value="ColorGray">Gray</asp:ListItem>
        <asp:ListItem Value="ColorRed">Red</asp:ListItem>
        <asp:ListItem Value="ColorPurple">Purple</asp:ListItem>
        <asp:ListItem Value="ColorWhite">White</asp:ListItem>
    </asp:DropDownList>
    <br />
    <asp:Label ID="lblOddItem" runat="server" Text="Odd Item" 
        EnableViewState="False" Font-Size="Small" Width="100px"></asp:Label>
    <asp:DropDownList ID="ddlColorOdd" runat="server" TabIndex="8" 
        EnableViewState="False" Font-Size="Small">
        <asp:ListItem Value="ColorBrown">Brown</asp:ListItem>
        <asp:ListItem Value="ColorOrange">Orange</asp:ListItem>
        <asp:ListItem Value="ColorYellow">Yellow</asp:ListItem>
        <asp:ListItem Value="ColorGreen">Green</asp:ListItem>
        <asp:ListItem Value="ColorBlue">Blue</asp:ListItem>
        <asp:ListItem Value="ColorGray">Gray</asp:ListItem>
        <asp:ListItem Value="ColorRed">Red</asp:ListItem>
        <asp:ListItem Value="ColorPurple">Purple</asp:ListItem>
        <asp:ListItem Value="ColorWhite">White</asp:ListItem>
    </asp:DropDownList>
    <br />
    <asp:Label ID="lblET" runat="server" Text="ET &gt; 60 min." 
        EnableViewState="False" Font-Size="Small" Width="100px"></asp:Label>
    <asp:DropDownList ID="ddlColorET" runat="server" TabIndex="9" 
        EnableViewState="False" Font-Size="Small">
         <asp:ListItem Value="ColorBrown">Brown</asp:ListItem>
        <asp:ListItem Value="ColorOrange">Orange</asp:ListItem>
        <asp:ListItem Value="ColorYellow">Yellow</asp:ListItem>
        <asp:ListItem Value="ColorGreen">Green</asp:ListItem>
        <asp:ListItem Value="ColorBlue">Blue</asp:ListItem>
        <asp:ListItem Value="ColorGray">Gray</asp:ListItem>
        <asp:ListItem Value="ColorRed">Red</asp:ListItem>
        <asp:ListItem Value="ColorPurple">Purple</asp:ListItem>
        <asp:ListItem Value="ColorWhite">White</asp:ListItem>
    </asp:DropDownList>
    <br />
        <asp:Label ID="lblHotComplaint" runat="server" Text="Hot Complaint" 
        EnableViewState="False" Font-Size="Small" Width="100px"></asp:Label>
    <asp:DropDownList ID="ddlColorHotComplaint" runat="server" TabIndex="9" 
        EnableViewState="False" Font-Size="Small">
        <asp:ListItem Value="ColorBrown">Brown</asp:ListItem>
        <asp:ListItem Value="ColorOrange">Orange</asp:ListItem>
        <asp:ListItem Value="ColorYellow">Yellow</asp:ListItem>
        <asp:ListItem Value="ColorGreen">Green</asp:ListItem>
        <asp:ListItem Value="ColorBlue">Blue</asp:ListItem>
        <asp:ListItem Value="ColorGray">Gray</asp:ListItem>
        <asp:ListItem Value="ColorRed">Red</asp:ListItem>
        <asp:ListItem Value="ColorPurple">Purple</asp:ListItem>
        <asp:ListItem Value="ColorWhite">White</asp:ListItem>
    </asp:DropDownList>
    <br />

    <asp:Label ID="lblYesterday" runat="server" Text="Yesterday's Paper" 
        EnableViewState="False" Font-Size="Small" Width="100px"></asp:Label>
    <asp:DropDownList ID="ddlColorYesterday" runat="server" 
        TabIndex="10" EnableViewState="False" Font-Size="Small">
        <asp:ListItem Value="ColorBrown">Brown</asp:ListItem>
        <asp:ListItem Value="ColorOrange">Orange</asp:ListItem>
        <asp:ListItem Value="ColorYellow">Yellow</asp:ListItem>
        <asp:ListItem Value="ColorGreen">Green</asp:ListItem>
        <asp:ListItem Value="ColorBlue">Blue</asp:ListItem>
        <asp:ListItem Value="ColorGray">Gray</asp:ListItem>
        <asp:ListItem Value="ColorRed">Red</asp:ListItem>
        <asp:ListItem Value="ColorPurple">Purple</asp:ListItem>
        <asp:ListItem Value="ColorWhite">White</asp:ListItem>
    </asp:DropDownList>
    <br />
    <asp:Label ID="lblRedelivered" runat="server" Text="Redelivered" 
        EnableViewState="False" Font-Size="Small" Width="100px"></asp:Label>
    <asp:DropDownList ID="ddlColorRedelivered" runat="server" 
        TabIndex="11" EnableViewState="False" Font-Size="Small">
        <asp:ListItem Value="ColorBrown">Brown</asp:ListItem>
        <asp:ListItem Value="ColorOrange">Orange</asp:ListItem>
        <asp:ListItem Value="ColorYellow">Yellow</asp:ListItem>
        <asp:ListItem Value="ColorGreen">Green</asp:ListItem>
        <asp:ListItem Value="ColorBlue">Blue</asp:ListItem>
        <asp:ListItem Value="ColorGray">Gray</asp:ListItem>
        <asp:ListItem Value="ColorRed">Red</asp:ListItem>
        <asp:ListItem Value="ColorPurple">Purple</asp:ListItem>
        <asp:ListItem Value="ColorWhite">White</asp:ListItem>
    </asp:DropDownList>
    <br />
    <br />
    </form>
</body>
</html>
