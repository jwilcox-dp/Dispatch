<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectZoneAndDistrict.ascx.cs" Inherits="SelectZoneAndDistrict" %>
        <asp:Label ID="lblZone" runat="server" Text="Zone:" 
            Font-Size="Small" Width="80px"></asp:Label>
        &nbsp;<asp:ListBox ID="lbZone" runat="server" DataSourceID="sdsGetZones" 
            DataTextField="Zone" DataValueField="Zone" Font-Size="Small" 
    Height="84px" Width="63px">
        </asp:ListBox>
        
        <asp:Button ID="btnSelectZone" runat="server" onclick="btnSelectZone_Click" 
            TabIndex="2" Text="Go" Font-Size="Small" />
        
        <br />
        
        <asp:Label ID="lblDistrict" runat="server" Text="District:" Visible="False" 
            EnableViewState="False" Font-Size="Small" ></asp:Label>
        &nbsp;<asp:ListBox ID="lbDistrict" runat="server" 
            DataSourceID="sdsGetDistricts" DataTextField="District" 
            DataValueField="District" Font-Size="Small" Height="80px" 
    Visible="False" Width="65px">
        </asp:ListBox>
        
        <asp:Button ID="btnSelectDistrict" runat="server" 
            onclick="btnSelectDistrict_Click" Text="Go" 
            TabIndex="4" Visible="False" EnableViewState="False" Font-Size="Small"/>
        
        <br />
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
    