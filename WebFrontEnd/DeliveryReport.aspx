<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeliveryReport.aspx.cs" Inherits="DeliveryReport" %>

<%@ Register src="SelectZoneAndDistrict.ascx" tagname="SelectZoneAndDistrict" tagprefix="uc1" %>

<%@ Register src="DispatchHeader.ascx" tagname="DispatchHeader" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Denver Post Dispatch Delivery Report</title>
    <link href="default.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            width: 269px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc2:DispatchHeader ID="ucDispatchHeader" runat="server" />
   <asp:Panel ID="pnlResearch" runat="server" Width="909px">
       <asp:Button ID="btnZoneDistrictSearch" runat="server" 
           Text="Zone/District Search" CssClass="Button" 
           onclick="btnZoneDistrictSearch_Click"/>
       <asp:Button ID="btnAddressSearch" runat="server" Text="Address Search" 
           CssClass="Button" onclick="btnAddressSearch_Click" Enabled="false" />
       <asp:Button ID="btnPhoneSearch" runat="server" Text="Phone Search" 
           CssClass="Button" onclick="btnPhoneSearch_Click"/>
       <asp:Button ID="btnRouteSearch" runat="server" Text="Route Search" 
           CssClass="Button" onclick="btnRouteSearch_Click"/>
       
    <div class="ViewDivWide" 
                    style="border-bottom-style: groove; border-bottom-width: medium; border-bottom-color: #553900">

                    
    <asp:MultiView ID="mvSearchType" runat="server" ActiveViewIndex="3" Visible="true" >
        <asp:View ID="viewAddress" runat="server">
        
        <asp:Label ID="Label10" runat="server" Text="Number/Fraction:" Font-Size="X-Small"></asp:Label>
        <asp:TextBox ID="tbStreetNumber" runat="server" MaxLength="5" Width="49px" 
                CssClass="TextBox"></asp:TextBox>
            &nbsp;/
        <asp:TextBox ID="tbStreetFraction" runat="server" MaxLength="2" Width="22px"></asp:TextBox>

        <asp:Label ID="Label11" runat="server" Text="Direction:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetDirection" runat="server" MaxLength="3" Width="27px" 
                CssClass="TextBox"></asp:TextBox>
        
        <asp:Label ID="Label4" runat="server" Text="Street Name:" 
                CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetName" runat="server" MaxLength="20" Width="156px" 
                CssClass="TextBox" ></asp:TextBox>
        <asp:Label ID="Label14" runat="server" Text="Type:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbStreetType" runat="server" MaxLength="3" Width="40px" CssClass="TextBox"></asp:TextBox>
        
        <asp:Label ID="Label6" runat="server" Text="Apt" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbApartment" runat="server" MaxLength="5" Width="58px" CssClass="TextBox"></asp:TextBox>
        <br />
        
        <asp:Label ID="Label7" runat="server" Text="City Code:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbCityCode" runat="server" MaxLength="4" CssClass="TextBox"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
        <br />
        <asp:Label ID="Label15" runat="server" Text="All searches are performed as 'Wild Card' searches - if you leave a search field blank, all data for that field will be matched.  <br>Example: to find all complaints in Denver, enter 'DENV' for the city code and leave all other fields blank." CssClass="FieldLabelSmall"></asp:Label>
        
        </asp:View>
        <asp:View ID="viewPhone" runat="server">
        <asp:Label ID="Label5" runat="server" Text="Phone Number:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbPhoneNumber" runat="server" MaxLength="15" CssClass="TextBox"></asp:TextBox>
        </asp:View>
        
        
        <asp:View ID="viewRoute" runat="server">
        <asp:Label ID="Label9" runat="server" Text="Route Code:" CssClass="FieldLabelSmall"></asp:Label>
        <asp:TextBox ID="tbRouteCode" runat="server" MaxLength="10" CssClass="TextBox"></asp:TextBox>
        </asp:View>
        
        <asp:View ID="viewZoneDistrict" runat="server">
        <asp:Panel ID="pnlSelectZoneAndDistrict" runat="server" Visible="true">
        <table>
            <tr>
                <td>
                    <table>
                    <tr>
                    <td>

                    <asp:Label ID="lblZone" runat="server" Text="Zone:" EnableViewState="False" 
                        Width="36px" CssClass="FieldLabelSmall"></asp:Label>
                    <asp:ListBox ID="lbZone" runat="server" DataSourceID="sdsGetZones" 
                            DataTextField="Zone" DataValueField="Zone" Font-Size="Small" Height="84px" 
                            AutoPostBack="True" onselectedindexchanged="lbZone_SelectedIndexChanged" 
                            Rows="6">
                    </asp:ListBox>
                    </td>
                    <td>
                    <asp:Label ID="lblFilterZoneInfo" runat="server" 
                            
                            Text="Press 'Search' to search by Zone, press 'Filter Districts' to refine search to a District within the Zone" EnableViewState="False" 
                        Width="197px" CssClass="FieldLabelSmall"></asp:Label>
                        <br />
                        <br />
                    
                    <asp:Button ID="btnSelectZone" runat="server" 
                        TabIndex="2" Text="Filter Districts" EnableViewState="False" Font-Size="Small" 
                        onclick="btnSelectZone_Click" CssClass="Button" Width="192px" />
                     </td>
                    </tr>                        
                    </table>

                </td>
                <td>
                    <asp:Label ID="lblDistrict" runat="server" Text="District:" 
                            EnableViewState="False" Width="48px" CssClass="FieldLabelSmall" 
                        Height="16px"></asp:Label>
                    <asp:ListBox ID="lbDistrict" runat="server" 
                            DataSourceID="sdsGetDistricts" DataTextField="District" 
                            DataValueField="District" Font-Size="Small" Height="80px" Rows="6">
                    </asp:ListBox>
                    <asp:Panel ID="Panel1" runat="server">
                    </asp:Panel>
                </td>
            </tr>
        </table>
        </asp:Panel>
        
        <asp:Panel ID="pnlSelectDistrict" runat="server" Visible="true">
        <table>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label13" runat="server" 
                        
                        Text="Select which district on which to search.  &lt;br&gt;Leave selection blank to search all districts." EnableViewState="False" 
                        CssClass="FieldLabelSmall"></asp:Label>
                        <br />
                </td>
                <td>
                    <asp:ListBox ID="lbPartnerDistricts" runat="server" 
                            DataSourceID="sdsGetDistricts" DataTextField="District" 
                            DataValueField="District" Font-Size="Small" Height="80px" Rows="6" 
                        Width="97px">
                    </asp:ListBox>
                </td>
            </tr>
        </table>
        </asp:Panel>
        
    <asp:SqlDataSource ID="sdsGetZones" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
        SelectCommand="GetAllZones" SelectCommandType="StoredProcedure" 
        EnableViewState="False">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsGetDistricts" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
        SelectCommand="GetDistrictsByZone" SelectCommandType="StoredProcedure" 
        EnableViewState="False">
        <SelectParameters>
            <asp:Parameter DefaultValue="MA" Name="Zone" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>        
    </asp:View>
    </asp:MultiView>
    <div class="ViewDiv">
    </div>
    <table>
        <tr>
            
            <td style="padding-left:20px; padding-right:20px">
                <asp:Label ID="Label2" runat="server" Text="From Date:"></asp:Label>
                <br />
                <asp:Calendar ID="calFromDate" runat="server" BackColor="White" 
                    BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" 
                    DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="Smaller" 
                    ForeColor="#003399" Height="200px" Width="220px" CssClass="Calendar">
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
            </td>
            <td style="padding-left:20px; padding-right:20px">
                <asp:Label ID="Label3" runat="server" Text="To Date:"></asp:Label>
                <br />
                <asp:Calendar ID="calToDate" runat="server" BackColor="White" 
                    BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" 
                    DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
                    ForeColor="#003399" Height="200px" Width="220px" CssClass="Calendar">
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
            </td>
<td>
            <asp:Label ID="lblSearchCriteriaError" runat="server" CssClass="ErrorText" 
                Text="Please enter search criteria." Visible="False"></asp:Label>
                  <br />
                  <br />
                <asp:Button ID="btnSearch" runat="server" Text="Search" 
                    onclick="btnSearch_Click1" CssClass="Button" Width="126px" />
                    <br />
                  <br />
                  <asp:CheckBox ID="cbUseDates" runat="server"  Text="Use Dates in Selection" 
                    CssClass="FieldLabelSmall"/>
                  <br />
                  <br />
                <asp:Label ID="Label8" runat="server" Text="Rows per Page:" 
                    CssClass="FieldLabelSmall"></asp:Label>
                <asp:DropDownList
                    ID="ddlRowsPerPage" runat="server" CssClass="TextBox">
                    <asp:ListItem Text="10" Selected="True" Value="10"></asp:ListItem>
                    <asp:ListItem Text="20" Selected="False" Value="20"></asp:ListItem>
                    <asp:ListItem Text="50" Selected="False" Value="50"></asp:ListItem>
                    <asp:ListItem Text="100" Selected="False" Value="100"></asp:ListItem>
                </asp:DropDownList>
            </td>            
        </tr>
    </table>    
    </div>
            </asp:Panel>    
            
            <asp:GridView ID="gvZoneManager" runat="server" AllowPaging="True" 
                AllowSorting="True" AutoGenerateColumns="False" 
                DataSourceID="sdsSearchByDistrictAndDate" 
                EmptyDataText="There are no data records to display." 
                EnableSortingAndPagingCallbacks="True" 
                PageSize="30" 
                Visible="False" BackColor="White" BorderColor="#999999" BorderStyle="None" 
                BorderWidth="1px" CellPadding="3" GridLines="Vertical" Font-Size="Smaller" 
                >
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <Columns>
                    <asp:BoundField DataField="RouteCode" HeaderText="Route" 
                        SortExpression="RouteCode" />
                    <asp:BoundField DataField="DisplayAddress" HeaderText="Address" 
                        SortExpression="DisplayAddress" />
                    <asp:BoundField DataField="PubCode" HeaderText="Pub" 
                        SortExpression="PubCode" />
                    <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" 
                        SortExpression="PhoneNumber" />
                    <asp:BoundField DataField="HouseholdName" HeaderText="Subscriber Name" 
                        SortExpression="HouseholdName" />
                    <asp:BoundField DataField="DistrictCode" HeaderText="District" 
                        SortExpression="DistrictCode" />
                    <asp:TemplateField HeaderText="Request Info" 
                        SortExpression="DeliveryInstructions">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" 
                                Text='<%# ParseMessageString(Container.DataItem) %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" 
                                Text='<%# ParseMessageString(Container.DataItem) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Draw" HeaderText="Draw" SortExpression="Draw" />
                    <asp:BoundField DataField="PaperDate" DataFormatString="{0:MM/dd}" 
                        HeaderText="Paper Date" SortExpression="PaperDate" />
                    <asp:BoundField DataField="EnteredDate" DataFormatString="{0:MM/dd HH:mm}" 
                        HeaderText="Date Entered" SortExpression="EnteredDate" />
                    <asp:BoundField DataField="DispatchDate" DataFormatString="{0:MM/dd HH:mm}" 
                        HeaderText="Dispatch Time" SortExpression="DispatchDate" />
                    <asp:BoundField DataField="DeliveredDate" DataFormatString="{0:MM/dd HH:mm}" 
                        HeaderText="Delivery Time" SortExpression="DeliveredDate" />
                    <asp:BoundField DataField="ElapsedTime" HeaderText="Elapsed Time" 
                        SortExpression="ElapsedTime" />
                </Columns>
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>
            <asp:SqlDataSource ID="sdsSearchByDistrictAndDate" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                InsertCommand="INSERT INTO [DispatchRequest] ([OperatorId], [PubCode], [AddressKey], [PhoneNumber], [HouseholdName], [RouteCode], [DistrictCode], [Status], [MessengerId], [DispatcherId], [Message], [DeliveryInstructions], [RedeliveryZone], [Draw], [PaperDate], [EnteredDate], [DispatchDate], [DeliveredDate], [RequestType], [AuditLogLinkId], [CJRequestNumber], [LockDate], [ProcessedDate], [LastUpdate], [TransferredToSIS]) VALUES (@OperatorId, @PubCode, @AddressKey, @PhoneNumber, @HouseholdName, @RouteCode, @DistrictCode, @Status, @MessengerId, @DispatcherId, @Message, @DeliveryInstructions, @RedeliveryZone, @Draw, @PaperDate, @EnteredDate, @DispatchDate, @DeliveredDate, @RequestType, @AuditLogLinkId, @CJRequestNumber, @LockDate, @ProcessedDate, @LastUpdate, @TransferredToSIS)" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByDistrictAndDate" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="DistrictCode" Type="String" />
                    <asp:Parameter Name="FromDate" Type="DateTime" />
                    <asp:Parameter Name="ToDate" Type="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>
            
            <asp:SqlDataSource ID="sdsSearchByZoneAndDate" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByZoneAndDate" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="ZoneCode" Type="String" />
                    <asp:Parameter Name="FromDate" Type="DateTime" />
                    <asp:Parameter Name="ToDate" Type="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>
            
            <asp:SqlDataSource ID="sdsSearchByAddress" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByAddress" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="StreetNumber" Type="String" />
                    <asp:Parameter Name="StreetFraction" Type="String" />
                    <asp:Parameter Name="StreetName" Type="String" />
                    <asp:Parameter Name="StreetType" Type="String" />
                    <asp:Parameter Name="StreetDirection" Type="String" />
                    <asp:Parameter Name="Apartment" Type="String" />
                    <asp:Parameter Name="CityCode" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            
            <asp:SqlDataSource ID="sdsSearchByAddressAndDate" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByAddressAndDate" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="StreetNumber" Type="String" />
                    <asp:Parameter Name="StreetFraction" Type="String" />
                    <asp:Parameter Name="StreetName" Type="String" />
                    <asp:Parameter Name="StreetType" Type="String" />
                    <asp:Parameter Name="StreetDirection" Type="String" />
                    <asp:Parameter Name="Apartment" Type="String" />
                    <asp:Parameter Name="CityCode" Type="String" />
                    <asp:Parameter Name="FromDate" Type="DateTime" />
                    <asp:Parameter Name="ToDate" Type="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>
            
            <asp:SqlDataSource ID="sdsSearchByPhoneAndDate" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByPhoneNumberAndDate" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="PhoneNumber" Type="String" />
                    <asp:Parameter Name="FromDate" Type="DateTime" />
                    <asp:Parameter Name="ToDate" Type="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>
            
            <asp:SqlDataSource ID="sdsSearchByPhone" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByPhoneNumber" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="PhoneNumber" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            
            <asp:SqlDataSource ID="sdsSearchByRouteCodeAndDate" runat="server" 
                ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:DispatchWriterConnectionString.ProviderName %>" 
                SelectCommand="GetRequestsByRouteCodeAndDate" 
                SelectCommandType="StoredProcedure" >
                <SelectParameters>
                    <asp:Parameter Name="RouteCode" Type="String" />
                    <asp:Parameter Name="FromDate" Type="DateTime" />
                    <asp:Parameter Name="ToDate" Type="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>
            
    </div>
    </form>
</body>
</html>
