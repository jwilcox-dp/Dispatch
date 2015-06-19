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

public partial class SummaryLegend : BaseUserControl
{

    protected void btnUndispatched_Click(object sender, EventArgs e)
    {
        if (!pnlLegend.Visible)
        {
            pnlLegend.Visible = true;
            btnViewLegend.Text = "Hide Legend";
        }
        else 
        {
            pnlLegend.Visible = false;
            btnViewLegend.Text = "Show Legend";
        }
    }
}
