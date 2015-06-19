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

public partial class SelectZoneAndDistrict : BaseUserControl
{
    string _redirectLocation = string.Empty; 
    ISavePreferences _parent = null; 

    public void SetParent(ISavePreferences inParent)
    {
        _parent = inParent; 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (PartnerPubZone != string.Empty)
            {
                lblZone.Visible = false;
                lbZone.Visible = false;
                btnSelectZone.Visible = false; 

                lblDistrict.Visible = true;
                lbDistrict.Visible = true;
                btnSelectDistrict.Visible = true; 

                sdsGetDistricts.SelectParameters[0].DefaultValue = PartnerPubZone;
                lbDistrict.DataBind();
                btnSelectDistrict.Focus();
            }
        }
        else
        {
            lblDistrict.Visible = btnSelectDistrict.Visible = lbDistrict.Visible;
        }
    }

    public void SetRedirectLocation(string inRedirectLocation)
    {
        _redirectLocation = inRedirectLocation; 
    }


    protected void btnSelectZone_Click(object sender, EventArgs e)
    {
        GetSession.CurrentZone = lbZone.SelectedValue;
        sdsGetDistricts.SelectParameters[0].DefaultValue = GetSession.CurrentZone; 
        lbDistrict.DataBind();
        btnSelectDistrict.Focus();

        lbDistrict.Visible = true;
        btnSelectDistrict.Visible = true;
        lblDistrict.Visible = true;

        _parent.SavePreferences(); 
    }
    protected void btnSelectDistrict_Click(object sender, EventArgs e)
    {
        GetSession.CurrentDistrict = lbDistrict.SelectedValue;

        _parent.SavePreferences(); 

        Response.Redirect(_redirectLocation); 
    }
}
