<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SummaryLegend.ascx.cs" Inherits="SummaryLegend" %>
<asp:Button ID="btnViewLegend" runat="server" Text="Show Legend" 
    onclick="btnUndispatched_Click" EnableViewState="False" Height="20px" style="margin-left: 0px"
    Font-Size="Smaller"/> 
<asp:Panel ID="pnlLegend" runat="server" Visible="false">
<asp:Label Runat="server" Text="A = Total number of Alerts (route with 3 or more Dispatches for the day)" BackColor="Tomato" /> <br />
<asp:Label Runat="server" Text="C = Total Number of Dispatches" BackColor="Khaki" /><br />
<asp:Label Runat="server" Text="D = Total Number of Delivered Items" BackColor="LightGreen" /> <br />
<asp:Label Runat="server" Text="V = Total Number of VIEWED but UNDELIVERED Items" BackColor="PowderBlue" /><br />
<asp:Label Runat="server" Text="U = Total Number of Undelivered Items" BackColor="Yellow" />
</asp:Panel>


