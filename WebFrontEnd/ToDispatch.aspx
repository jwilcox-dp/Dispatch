<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ToDispatch.aspx.cs" Inherits="ToDispatch" EnableEventValidation="false" EnableViewState="false" %>
<%@ Register src="ViewType.ascx" tagname="ViewType" tagprefix="uc1" %>
<%@ Register src="DispatchHeader.ascx" tagname="DispatchHeader" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Denver Post Dispatch Items</title>
    <link href="CPhone.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server" enableviewstate="true">
    <uc2:DispatchHeader ID="ucDispatchHeader" runat="server" />
    <asp:Button ID="btnTopUndelivered" runat="server" 
        Text="To Do" 
        Font-Size="Smaller" 
        onclick="btnTopUndelivered_Click"  />
    <asp:Button ID="btnTopAll" runat="server" Text="All" 
        Font-Size="Smaller" 
        onclick="btnTopAll_Click"  />
    <asp:Button ID="btnTopSettings" runat="server" Text="Preferences" 
        EnableViewState="true" 
        Font-Size="Smaller" 
        onclientclick="window.location.href=&quot;Preferences.aspx&quot;;return false;" />
    <br />
    <asp:DataList ID="dlToDispatch" runat="server" BorderColor="Gray" 
        BorderWidth="2px" DataKeyField="Id" DataSourceID="sdsGetTodaysDispatches" 
        GridLines="Horizontal" RepeatColumns="1" RepeatLayout="Flow" ShowFooter="False" 
        ShowHeader="False" EnableViewState="true" 
        onitemdatabound="dlToDispatch_ItemDataBound" 
        onitemcommand="dlToDispatch_ItemCommand" 
        onselectedindexchanged="dlToDispatch_SelectedIndexChanged">
        <ItemTemplate>
            <asp:Panel ID="Panel1" runat="server" 
            CssClass="<%# GetEvenColor(Container.DataItem) %>" 
            EnableViewState="true">
            
            <asp:Button ID="btnDel" 
                runat="server" 
                Text="Del" 
                CommandArgument='<%# Eval("Id") %>' 
                EnableViewState="True" 
                CausesValidation="False" CommandName="  " />

            &nbsp;
            <asp:Label ID="DistrictCode" runat="server" Text='<%# Eval("DistrictCode") %>'  EnableViewState="false" />
            <asp:Label ID="RouteCodeLabel" runat="server" Text='<%# Eval("RouteCode") %>'  EnableViewState="false" />
            <asp:Label ID="PubCodeLabel" runat="server" Text='<%# Eval("PubCode") %>' EnableViewState="false" />
            <asp:Label ID="PaperDateLabel" runat="server" Text='<%# Eval("DayAndMonth") %>' EnableViewState="false" />
            <asp:Label ID="NCSDispatchIDLabel" runat="server" Text='<%# Eval("NCS_DispatchID") %>' EnableViewState="false" Visible="false" />

            <asp:Label ID="CreateTimeLabel" runat="server" 
                Text='<%# Eval("CreateTime") %>' EnableViewState="false" />
            <br />
            <asp:Label ID="MessageLabel" runat="server" 
                Text='<%# ParseMessageString(Container.DataItem) %>' EnableViewState="false" />                
                
               
            <asp:Label ID="Address" runat="server" 
                Text='<%# Eval("DisplayAddress") %>' EnableViewState="false" />
            <br />
                
            <asp:HyperLink ID="HouseholdName" 
            runat="server" 
            Text='<%# Eval("HouseholdName") %>' 
            EnableViewState="false" NavigateUrl='<%# CustomerLookupURL(Container.DataItem) %>'
            Enabled="false"></asp:HyperLink>   

            <br />
            <asp:Label ID="PhoneNumberLabel" runat="server" 
                Text='<%# Eval("PhoneNumber") %>' EnableViewState="true" />
                                   
            <asp:Label ID="ElapsedTimeLabel" runat="server" 
                Text='<%# ParseCreateTime(Container.DataItem) %>'            
                EnableViewState="false" />
             <hr />
            </asp:Panel>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <asp:Panel ID="Panel2" runat="server" CssClass="<%# GetOddColor(Container.DataItem) %>" EnableViewState="false" >
            <asp:Button ID="btnDel" runat="server" Text="Del" 
                CommandArgument='<%# Eval("Id") %>' EnableViewState="True" CausesValidation="False"  CommandName="DeliverIt"/>
            &nbsp;
            
            <asp:Label ID="DistrictCode" runat="server" Text='<%# Eval("DistrictCode") %>'  EnableViewState="false" />
            <asp:Label ID="RouteCodeLabel" runat="server" Text='<%# Eval("RouteCode") %>' EnableViewState="false"  />
            <asp:Label ID="PubCodeLabel" runat="server" Text='<%# Eval("PubCode") %>' EnableViewState="false" />
            <asp:Label ID="PaperDateLabel" runat="server" Text='<%# Eval("DayAndMonth") %>' EnableViewState="false" />
            <asp:Label ID="NCSDispatchIDLabel" runat="server" Text='<%# Eval("NCS_DispatchID") %>' EnableViewState="false" Visible="false" />

            <asp:Label ID="CreateTimeLabel" runat="server" 
                Text='<%# Eval("CreateTime") %>' EnableViewState="false" />

            <br />
           <asp:Label ID="MessageLabel" runat="server" 
                Text='<%# ParseMessageString(Container.DataItem) %>' EnableViewState="false" />
                
            <asp:Label ID="Address" runat="server" 
                Text='<%# Eval("DisplayAddress") %>' EnableViewState="false" />
            <br />
            
            <asp:HyperLink ID="HouseholdName" 
            runat="server" 
            Text='<%# Eval("HouseholdName") %>' 
            EnableViewState="false" NavigateUrl='<%# CustomerLookupURL(Container.DataItem) %>'
            Enabled="false"></asp:HyperLink>   
                
            <br />
                
            <asp:Label ID="PhoneNumberLabel" runat="server" 
                Text='<%# Eval("PhoneNumber") %>' EnableViewState="false" />
            
            <asp:Label ID="ElapsedTimeLabel" runat="server" 
                Text='<%# ParseCreateTime(Container.DataItem) %>'            
                EnableViewState="false" />
                
            <hr />                
            </asp:Panel>
        </AlternatingItemTemplate>
    </asp:DataList>
    <br />
    
    <asp:Button ID="btnBottomUndelivered" runat="server" Text="To Do" 
        Font-Size="Smaller" 
        PostBackUrl="SetViewTypePassthrough.aspx?ViewType=UNDELIVERED"  />
    <asp:Button ID="btnBottomAll" runat="server" 
        Text="All" 
        Font-Size="Smaller" 
        PostBackUrl="SetViewTypePassthrough.aspx?ViewType=ALL"  />
    <asp:Button ID="btnBottomSettings" runat="server" 
        Text="Preferences" 
        Font-Size="Smaller" 
        onclientclick="window.location.href=&quot;Preferences.aspx&quot;;return false;" />
        
    <br />
    <asp:SqlDataSource ID="sdsGetTodaysDispatches" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
        SelectCommand="GetTodaysRequestsByDistrictCode" 
        SelectCommandType="StoredProcedure" EnableViewState="False">
        <SelectParameters>
            <asp:Parameter Name="SortOrder" Type="String" />
            <asp:Parameter Name="DistrictCodeList" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsGetUndelivered" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
        SelectCommand="GetTodaysUndeliveredRequestsByDistrictCode" 
        SelectCommandType="StoredProcedure" EnableViewState="False">
        <SelectParameters>
            <asp:Parameter Name="SortOrder" Type="String" />
            <asp:Parameter Name="DistrictCodeList" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsGetUndispatched" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
        SelectCommand="GetTodaysUndispatchedRequestsByDistrictCode" 
        SelectCommandType="StoredProcedure" EnableViewState="False">
        <SelectParameters>
            <asp:Parameter Name="SortOrder" Type="String" />
            <asp:Parameter Name="DistrictCodeList" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <asp:SqlDataSource ID="sdsGetAlertRoutes" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
        SelectCommand="GetTodaysUndispatchedRequestsByDistrictCode" 
        EnableViewState="False">
        <SelectParameters>
            <asp:Parameter Name="SortOrder" Type="String" />
            <asp:Parameter Name="DistrictCodeList" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="sdsConfirmDelivery" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchWriterConnectionString %>" 
        SelectCommand="ConfirmDelivery" SelectCommandType="StoredProcedure" 
        UpdateCommand="ConfirmDelivery" UpdateCommandType="StoredProcedure" 
        EnableViewState="False">
        <SelectParameters>
            <asp:Parameter Name="Id" Type="Int32" />
            <asp:Parameter Name="UploadedToNCS" Type="Boolean" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Id" Type="Int32" />
            <asp:Parameter Name="UploadedToNCS" Type="Boolean" />
        </UpdateParameters>
    </asp:SqlDataSource>     
   <br />
    </form>
</body>
</html>
