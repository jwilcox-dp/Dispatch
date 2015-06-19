<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectedDistricts.ascx.cs" Inherits="SelectedDistricts" %>
<asp:CheckBoxList ID="cblCurrentDistricts" EnableViewState="true" 
    Font-Size="Small" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" TabIndex="0">
</asp:CheckBoxList>
<br />
<asp:Button ID="btnGoToDistrict" runat="server" Text="Go" 
    onclick="btnGoToDistrict_Click" Font-Size="Small" TabIndex="1" />
<asp:Button ID="btnRemoveDistrict" runat="server" Text="Remove" 
    onclick="btnRemoveDistrict_Click" Font-Size="Small" TabIndex="2"/>
<hr />
<asp:SqlDataSource ID="sdsUserSettingReader" runat="server" 
    ConnectionString="<%$ ConnectionStrings:DispatchReaderConnectionString %>" 
    InsertCommandType="StoredProcedure" 
    SelectCommand="GetUserSettingByRequestorAndSetting" 
    SelectCommandType="StoredProcedure" UpdateCommand="UpdateUserSetting" 
    UpdateCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="Requestor" Type="String" />
        <asp:Parameter Name="Setting" Type="String" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="sdsUserSettingWriter" runat="server" 
    ConnectionString="<%$ ConnectionStrings:DispatchWriterConnectionString %>" 
    InsertCommand="InsertUserSetting" InsertCommandType="StoredProcedure" 
    SelectCommand="Select * from dbo.UserSetting" UpdateCommand="UpdateUserSetting" 
    UpdateCommandType="StoredProcedure">
    <UpdateParameters>
        <asp:Parameter Name="Id" Type="Int32" />
        <asp:Parameter Name="Value" Type="String" />
        <asp:Parameter Name="LastUpdate" Type="DateTime" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="Requestor" Type="String" />
        <asp:Parameter Name="Setting" Type="String" />
        <asp:Parameter Name="Value" Type="String" />
    </InsertParameters>
</asp:SqlDataSource>

