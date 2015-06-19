<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeliverItem.aspx.cs" Inherits="DeliverItem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Denver Post</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    
    <asp:SqlDataSource ID="sdsConfirmDelivery" runat="server" 
        ConnectionString="<%$ ConnectionStrings:DispatchWriterConnectionString %>" 
        SelectCommand="ConfirmDelivery" SelectCommandType="StoredProcedure" 
        UpdateCommand="ConfirmDelivery" UpdateCommandType="StoredProcedure" 
        EnableViewState="False">
        <SelectParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>    
    </form>
</body>
</html>
