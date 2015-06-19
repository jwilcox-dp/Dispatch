using System;
using System.Collections;
using System.Collections.Generic; 
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class ViewType : BaseUserControl
{
    ISetViewType _parent = null;

    public void SetParent(ISetViewType inParent)
    {
        _parent = inParent;
        SetButtonAttributes();
    }

    public void SetButtonAttributes()
    {
        btnUndelivered.Font.Bold = false;
        btnUndelivered.Font.Underline = false;

        btnAll.Font.Bold = false;
        btnAll.Font.Underline = false; 

        switch (_parent.GetSession().ViewType.ToUpper())
        {
            case "UNDELIVERED":
                btnUndelivered.Font.Bold = true;
                btnUndelivered.Font.Underline = true; 
                break; 
            default:
                btnAll.Font.Bold = true;
                btnAll.Font.Underline = true; 
                break;
        }
    }

    protected void btnUndelivered_Click(object sender, EventArgs e)
    {
        Server.Transfer("~/ToDispatch.aspx?ViewType=UNDELIVERED");
    }
    protected void btnAll_Click(object sender, EventArgs e)
    {
        Server.Transfer("~/ToDispatch.aspx?ViewType=ALL"); 
    }
    protected void btnSettings(object sender, EventArgs e)
    {
        Server.Transfer("~/Login.aspx"); 
    }
}
