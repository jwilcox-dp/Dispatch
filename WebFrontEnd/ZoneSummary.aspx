<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ZoneSummary.aspx.cs" Inherits="ZoneSummary" %>

<%@ Register src="SummaryLegend.ascx" tagname="SummaryLegend" tagprefix="uc1" %>

<%@ Register src="DispatchHeader.ascx" tagname="DispatchHeader" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="default.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DNA Dispatch Summary (by Zone)</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
-        <center>
            <uc2:DispatchHeader ID="ucDispatchHeader" runat="server" />
        </center>
        <uc1:SummaryLegend ID="SummaryLegend1" runat="server" />
        
        <asp:DataList ID="dlZoneSummary" runat="server" BackColor="White" 
            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="5px" CellPadding="4" 
            DataSourceID="sdsAllZones" ForeColor="Black" GridLines="Vertical" 
            Font-Names="Courier">
            <FooterStyle BackColor="#CCCC99" />
            <AlternatingItemStyle BackColor="White" />
            <ItemStyle BackColor="#F7F7DE" />
            <SelectedItemStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                <asp:Button ID="btnDrillDown" runat="server" Text='<%# Eval("ZoneCode") %>'  
                    onclick="btnDrillDown_Click"
                    style="margin-left: 0px" Width="40px"
                    />
                <asp:Label ID="AlertsLabel" runat="server" Text='<%# Alerts(Container.DataItem) %>' CssClass='<%# AlertItemStyle(Container.DataItem) %>'/>
                <asp:Label ID="ComplaintsLabel" runat="server" 
                    Text='<%# Complaints(Container.DataItem) %>' 
                    CssClass='<%# ComplaintItemStyle(Container.DataItem) %>'
                    />
                <asp:Label ID="DeliveredLabel" runat="server" Text='<%# Delivered(Container.DataItem) %>' 
                    CssClass="CompletedItem"
                />
                <asp:Label ID="ViewedLabel" runat="server" Text='<%# Viewed(Container.DataItem) %>' 
                    CssClass='<%# ViewedItemStyle(Container.DataItem) %>'
                />
                <asp:Label ID="UndeliveredLabel" runat="server" 
                    Text='<%# Undelivered(Container.DataItem) %>' 
                    CssClass='<%# UndeliveredItemStyle(Container.DataItem) %>'
                    />
            </ItemTemplate>
        </asp:DataList>
        <asp:SqlDataSource ID="sdsAllZones" runat="server" 
            ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
            
            
            SelectCommand="SELECT [ZoneCode], [Alerts], [Complaints], [Delivered], [Viewed], [Undelivered] FROM [ViewStatusSummaryByZone] order by [ZoneCode]">
        </asp:SqlDataSource>
    
        <asp:SqlDataSource ID="sdsSelectZones" runat="server" 
            ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
            
            SelectCommand="SELECT [ZoneCode], [Alerts], [Complaints], [Delivered], [Viewed], [Undelivered] FROM [ViewStatusSummaryByZone] 
where ZoneCode = @ZoneCode
order by [ZoneCode]">
            <SelectParameters>
                <asp:SessionParameter Name="ZoneCode" SessionField="ZoneCode" />
            </SelectParameters>
        </asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
