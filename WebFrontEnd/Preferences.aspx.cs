using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Preferences : BasePage, ISavePreferences
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        ucDispatchHeader.SetTitle("Preferences"); 
        
        ucSelectZoneAndDistrict.SetRedirectLocation("~/ToDispatch.aspx");
        ucSelectZoneAndDistrict.SetParent(this);
        ucSelectedDistricts.SetParent(this);

        ucSelectZoneAndDistrict.SetSession(GetSession());
        ucSelectedDistricts.SetSession(GetSession());

        if (!IsPostBack)
        {
            lblEvenItem.CssClass = ddlColorEven.SelectedValue = GetSession().ColorEven; 
            lblOddItem.CssClass = ddlColorOdd.SelectedValue = GetSession().ColorOdd;
            lblET.CssClass = ddlColorET.SelectedValue = GetSession().ColorET;
            lblHotComplaint.CssClass = ddlColorHotComplaint.SelectedValue = GetSession().ColorHotComplaint;
            lblYesterday.CssClass = ddlColorYesterday.SelectedValue = GetSession().ColorYesterday;
            lblRedelivered.CssClass = ddlColorRedelivered.SelectedValue = GetSession().ColorRedelivered;

            rblSortOrder.SelectedValue = GetSession().SortOrder;
            rblViewType.SelectedValue = GetSession().ViewType.ToUpper();
        }
    }


    public override void SavePreferences()
    {
        // Save all the selections to the Session object
        GetSession().ColorEven = ddlColorEven.SelectedValue; 

        GetSession().ColorOdd = ddlColorOdd.SelectedValue;

        GetSession().ColorET = ddlColorET.SelectedValue;

        GetSession().ColorHotComplaint = ddlColorHotComplaint.SelectedValue;

        GetSession().ColorYesterday = ddlColorYesterday.SelectedValue;

        GetSession().ColorRedelivered = ddlColorRedelivered.SelectedValue;

        GetSession().SortOrder = rblSortOrder.SelectedValue;
        GetSession().ViewType = rblViewType.SelectedValue.ToUpper();

        // Save base SavePreferences which will save the state to the database.
        base.SavePreferences(); 
    }
}
