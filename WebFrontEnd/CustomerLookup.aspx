<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerLookup.aspx.cs" Inherits="CustomerLookup" %>

<%@ Register src="DispatchHeader.ascx" tagname="DispatchHeader" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>The Denver Post</title>
    <link href="default.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            width: 67%;
        }
        .style2
        {
            width: 185px;
        }
        .style3
        {
            width: 65%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:DispatchHeader ID="ucDispatchHeader" runat="server" />
    
    <asp:Label ID="lblError" runat="server" ForeColor="Maroon" Text="Label" 
        Visible="False"></asp:Label>
    
    <br />

        <table class="style3">
            <tr>
                <td>
                    <asp:Button ID="btnAddressSearch" runat="server" 
                        onclick="btnAddressSearch_Click" Text="Search by Address" CssClass="Button"/>
                </td>
                <td>
                    <asp:Button ID="btnPhoneSearch" runat="server" onclick="btnPhoneSearch_Click" 
                        Text="Search by Phone" CssClass="Button"/>
                </td>
                <td>
                    <asp:Button ID="btnAccount" runat="server" onclick="btnAccount_Click" 
                        Text="Search by Account" CssClass="Button"/>
                </td>
            </tr>
        </table>
        <div class="ViewDivWide">
    <asp:MultiView ID="mvSearchType" runat="server" ActiveViewIndex="0" Visible="true">
        <asp:View ID="viewAddress" runat="server">
        
        <asp:Label ID="Label10" runat="server" Text="Number:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetNumber" runat="server" MaxLength="5" Width="49px" CssClass="TextBox"></asp:TextBox>

        <asp:Label ID="Label13" runat="server" Text="Fraction:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetFraction" runat="server" MaxLength="2" Width="22px" CssClass="TextBox"></asp:TextBox>

        <asp:Label ID="Label11" runat="server" Text="Direction:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetDirection" runat="server" MaxLength="3" Width="27px" CssClass="TextBox"></asp:TextBox>
        
        <asp:Label ID="Label4" runat="server" Text="Name:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetName" runat="server" MaxLength="20" Width="156px" CssClass="TextBox"></asp:TextBox>
            <br />
        <asp:Label ID="Label14" runat="server" Text="Type:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetType" runat="server" MaxLength="4" Width="40px" 
                CssClass="TextBox"></asp:TextBox>
        
        <asp:Label ID="Label6" runat="server" Text="Apt" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbApartment" runat="server" MaxLength="5" Width="58px" CssClass="TextBox"></asp:TextBox>
        
        <asp:Label ID="Label7" runat="server" Text="City Code:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbCityCode" runat="server" MaxLength="4" CssClass="TextBox"></asp:TextBox>
        
        </asp:View>
        <asp:View ID="viewPhone" runat="server">
        <asp:Label ID="Label5" runat="server" Text="Phone Number:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbPhoneNumber" runat="server" MaxLength="15" CssClass="TextBox"></asp:TextBox>
        </asp:View>
        <asp:View ID="viewAccount" runat="server">
        <asp:Label ID="Label8" runat="server" Text="Account Number:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbAccountNumber" runat="server" MaxLength="8" CssClass="TextBox"></asp:TextBox>
        </asp:View>
    </asp:MultiView>
    </div>
    
    <asp:Button ID="btnSearch" runat="server" Text="Search" 
        onclick="btnSearch_Click" CssClass="Button"/>
    
    <br />
    
    <asp:Panel ID="pnlCustomerInfo" runat="server" Visible="false" 
        style="margin-right: 165px">
    
        <table class="style1">
            <tr>
                <td class="style2">
                    <asp:Label ID="Label3" runat="server" CssClass="FieldLabel" 
                        Text="Customer Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbCusomterName" runat="server" CssClass="TextBox" 
                        ReadOnly="true" Width="250px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label1" runat="server" CssClass="FieldLabel" Text="Address:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbAddress" runat="server" CssClass="TextBox" ReadOnly="true" 
                        Width="250px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label15" runat="server" CssClass="FieldLabel" Text="Phone Number:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPhoneNumber2" runat="server" CssClass="TextBox" 
                        ReadOnly="true" Width="200px"></asp:TextBox>
                    </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label9" runat="server" CssClass="FieldLabel" 
                        Text="Building Type:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbBuildingType" runat="server" CssClass="TextBox" 
                        ReadOnly="true" Width="200px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label2" runat="server" CssClass="FieldLabel" 
                        Text="Delivery Instructions:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDeliveryInstructions" runat="server" CssClass="TextBox" 
                        ReadOnly="true" Width="200px"></asp:TextBox>
                    </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>
        <br />
    
        Subscriptions<br />
        <asp:GridView ID="gvSubscriptions" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No Subscriptions Found" GridLines="Vertical">
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="SubStatus" HeaderText="Status" ReadOnly="True" 
                    SortExpression="SubStatus" />
                <asp:BoundField DataField="AccountNumber" HeaderText="Account" ReadOnly="True" 
                    SortExpression="AccountNumber" />
                <asp:BoundField DataField="SubNumber" HeaderText="Sub #" 
                    SortExpression="SubNumber" />
                <asp:BoundField DataField="SubName" HeaderText="Subscriber" 
                    SortExpression="SubName" />
                <asp:BoundField DataField="PublCode" HeaderText="Pub Code" 
                    SortExpression="PublCode" />
                <asp:BoundField DataField="PublName" HeaderText="Publication" 
                    SortExpression="PublName" />
                <asp:BoundField DataField="ServiceDays" HeaderText="Service" 
                    SortExpression="ServiceDays" />
                <asp:BoundField DataField="RouteZoneCode" HeaderText="Zone" 
                    SortExpression="RouteZoneCode" />
                <asp:BoundField DataField="RouteDistrictCode" HeaderText="District" 
                    SortExpression="RouteDistrictCode" />
                <asp:BoundField DataField="RouteCode" HeaderText="Route" 
                    SortExpression="RouteCode" />
            </Columns>
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        <br />
        Complaint History<br />
        <asp:GridView ID="gvComplaints" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="There are no complaints for this household." 
            GridLines="Vertical">
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="Description" HeaderText="Description" 
                    SortExpression="Description" />
                <asp:TemplateField HeaderText="Additional Info" SortExpression="Description">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" 
                            Text="<%# ComplaintAdditionalInfo(Container.DataItem) %>"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PublicationCode" HeaderText="Pub" ReadOnly="True" 
                    SortExpression="PublicationCode" />
                <asp:BoundField DataField="ServiceCode" HeaderText="Service" ReadOnly="True" 
                    SortExpression="ServiceCode" />
                <asp:BoundField DataField="EnterDate" DataFormatString="{0:d}" 
                    HeaderText="Date Entered" SortExpression="EnterDate" />
                <asp:BoundField DataField="EffectiveDate" DataFormatString="{0:d}" 
                    HeaderText="Pub Date" SortExpression="EffectiveDate" />
                <asp:TemplateField HeaderText="Redelivery Info" SortExpression="RedeliverFlag">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" 
                            Text="<%# ComplaintRedeliveryInfo(Container.DataItem) %>"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        <br />
        Memo History
        <br />
        <asp:GridView ID="gvMemos" runat="server" AutoGenerateColumns="False" 
            EmptyDataText="There are no memos for this household." 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" GridLines="Vertical">
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:TemplateField HeaderText="Memo" SortExpression="Memo">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# MemoInfo(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EnteredDate" HeaderText="Date Entered" DataFormatString="{0:d}"
                    SortExpression="EnteredDate" />
                <asp:BoundField DataField="EffectiveDate" HeaderText="Pub Date" DataFormatString="{0:d}"
                    SortExpression="EffectiveDate" />
                <asp:TemplateField HeaderText="Dispatch Info" SortExpression="RedliverDate">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# MemoRedeliveryInfo(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        
        
    </asp:Panel>
    </form>
</body>
</html>


