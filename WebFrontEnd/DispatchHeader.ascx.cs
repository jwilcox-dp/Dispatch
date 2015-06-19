using System;
using System.Collections;
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

public partial class DispatchHeader : BaseUserControl
{
    public void SetTitle(string inTitle)
    {
        if (PartnerPubName != string.Empty)
            lblTitle.Text = string.Format("{0} ({1})", inTitle, PartnerPubName);
        else
        {
            lblTitle.Text = inTitle;
            hyperHeader.Text = "Denver Post Dispatch System"; 
        }

        if (lblTitle.Text.Trim().Length > 0)
            lblTitle.Visible = true; 
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MainMenu.aspx"); 
    }
}
