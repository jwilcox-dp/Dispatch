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

public partial class MainMenu : BasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e); 
        ucDispatchHeader.SetTitle("");

        // If serving up for a Partner Publication, they only have one zone, so skip the Zone Summary and 
        // go to the district summary - need to pass in the (single) zone code in that request.
        if (PartnerPubName != string.Empty)
        {
            btnZoneSummary.PostBackUrl = string.Format("DistrictSummary.aspx?ZoneCode={0}", PartnerPubZone);
        }
        else
        {
            btnReveal.Visible = false;
            btnSyncronex.Visible = false; 
        }
    }
    protected void btnCustomerSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerLookup.aspx"); 
    }
    protected void btnPreferences_Click(object sender, EventArgs e)
    {
        Response.Redirect("Preferences.aspx"); 
    }
    protected void btnToDispatch_Click(object sender, EventArgs e)
    {
        Response.Redirect("ToDispatch.aspx"); 
    }
    protected void btnZoneSummary_Click(object sender, EventArgs e)
    {
        Response.Redirect("ZoneSummary.aspx"); 
    }
    protected void btnDeliveryReport_Click(object sender, EventArgs e)
    {
        Response.Redirect("DeliveryReport.aspx"); 
    }
    protected void btnSyncronex_Click(object sender, EventArgs e)
    {
        Response.Redirect("http://65.101.206.184/SingleCopy/SelectCompany.asp"); 
    }
    protected void btnReveal_Click(object sender, EventArgs e)
    {
        Response.Redirect("http://65.101.206.184/revealjavaweb/exd.html"); 
    }
}
