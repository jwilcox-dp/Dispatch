<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GreeleyDeliveryReport.aspx.cs" Inherits="GreeleyDeliveryReport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Denver Post</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="Label1" runat="server" BackColor="#D7E0F4" BorderColor="#000066" 
            BorderStyle="Groove" Font-Bold="True" ForeColor="#000066" 
            Text="Dispatch Delivery Report For Greeley" Width="798px"></asp:Label>
        <br />
        <asp:Calendar ID="calendar" runat="server" BackColor="White" 
            BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" 
            DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
            ForeColor="#003399" Height="200px" 
            onselectionchanged="calendar_SelectionChanged" Width="220px">
            <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
            <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
            <WeekendDayStyle BackColor="#CCCCFF" />
            <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
            <OtherMonthDayStyle ForeColor="#999999" />
            <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
            <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
            <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" 
                Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
        </asp:Calendar>
    <asp:GridView ID="gvGreeleyDispatches" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="RequestID" DataSourceID="sdsGreeleyDispatches" 
        EmptyDataText="There are no data records to display." BackColor="White" 
        BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        GridLines="Vertical" AllowPaging="True" AllowSorting="True">
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <Columns>
            <asp:BoundField DataField="DISTRICT" HeaderText="District" 
                SortExpression="DISTRICT" />
            <asp:BoundField DataField="ROUTE_CODE" HeaderText="Route Code" 
                SortExpression="ROUTE_CODE" />
            <asp:BoundField DataField="CITY" HeaderText="City" SortExpression="CITY" />
            <asp:BoundField DataField="FormattedAddress" HeaderText="Address" 
                SortExpression="FormattedAddress" />
            <asp:BoundField DataField="PUBLICATION" HeaderText="Pub" 
                SortExpression="PUBLICATION" />
            <asp:BoundField DataField="PHONE" HeaderText="Phone" SortExpression="PHONE" />
            <asp:BoundField DataField="SUB_NAME" HeaderText="Subscriber" 
                SortExpression="SUB_NAME" />
            <asp:BoundField DataField="MESSAGE" HeaderText="Delivery Instructions" 
                SortExpression="MESSAGE" />
            <asp:BoundField DataField="DATE_ENTERED" HeaderText="Date Entered" 
                SortExpression="DATE_ENTERED" />
            <asp:BoundField DataField="TIME_ENTERED" HeaderText="Time Entered" 
                SortExpression="TIME_ENTERED" />
        </Columns>
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
    <asp:SqlDataSource ID="sdsGreeleyDispatches" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchConnectionString2 %>" 
        DeleteCommand="DELETE FROM [Request] WHERE [RequestID] = @RequestID" 
        InsertCommand="INSERT INTO [Request] ([OP_ID], [REQ_NUM], [PUBLICATION], [ADDRESS_KEY], [PHONE], [SUB_NAME], [TIME_DISPATCHED], [TIME_DELIVERED], [RESERVED_AREA], [ROUTE_CODE], [DISTRICT], [DATE_ENTERED], [TIME_ENTERED], [STATUS], [MESSENGER_ID], [MESSAGE], [CITY], [FormattedAddress]) VALUES (@OP_ID, @REQ_NUM, @PUBLICATION, @ADDRESS_KEY, @PHONE, @SUB_NAME, @TIME_DISPATCHED, @TIME_DELIVERED, @RESERVED_AREA, @ROUTE_CODE, @DISTRICT, @DATE_ENTERED, @TIME_ENTERED, @STATUS, @MESSENGER_ID, @MESSAGE, @CITY, @FormattedAddress)" 
        ProviderName="<%$ ConnectionStrings:DispatchConnectionString2.ProviderName %>" 
        SelectCommand="SELECT [RequestID], [OP_ID], [REQ_NUM], [PUBLICATION], [ADDRESS_KEY], [PHONE], [SUB_NAME], [TIME_DISPATCHED], [TIME_DELIVERED], [RESERVED_AREA], [ROUTE_CODE], [DISTRICT], [DATE_ENTERED], [TIME_ENTERED], [STATUS], [MESSENGER_ID], [MESSAGE], [CITY], [FormattedAddress] FROM [Request] WHERE (([DATE_ENTERED] = @DATE_ENTERED) AND ([DISTRICT] LIKE '%' + @DISTRICT + '%'))" 
        
        
            UpdateCommand="UPDATE [Request] SET [OP_ID] = @OP_ID, [REQ_NUM] = @REQ_NUM, [PUBLICATION] = @PUBLICATION, [ADDRESS_KEY] = @ADDRESS_KEY, [PHONE] = @PHONE, [SUB_NAME] = @SUB_NAME, [TIME_DISPATCHED] = @TIME_DISPATCHED, [TIME_DELIVERED] = @TIME_DELIVERED, [RESERVED_AREA] = @RESERVED_AREA, [ROUTE_CODE] = @ROUTE_CODE, [DISTRICT] = @DISTRICT, [DATE_ENTERED] = @DATE_ENTERED, [TIME_ENTERED] = @TIME_ENTERED, [STATUS] = @STATUS, [MESSENGER_ID] = @MESSENGER_ID, [MESSAGE] = @MESSAGE, [CITY] = @CITY, [FormattedAddress] = @FormattedAddress WHERE [RequestID] = @RequestID">
        <SelectParameters>
            <asp:Parameter DefaultValue="?" Name="DATE_ENTERED" Type="String" />
            <asp:Parameter DefaultValue="GR%" Name="DISTRICT" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
        Page Length:
        <asp:DropDownList ID="ddlPageLength" runat="server" AutoPostBack="True" 
            onselectedindexchanged="calendar_SelectionChanged">
            <asp:ListItem Selected="True">10</asp:ListItem>
            <asp:ListItem>20</asp:ListItem>
            <asp:ListItem>50</asp:ListItem>
        </asp:DropDownList>
    </form>
</body>
</html>
