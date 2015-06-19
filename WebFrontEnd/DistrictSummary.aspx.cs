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

public partial class DistrictSummary : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string gotoDistrict = Request["GoToDistrict"];

        if (gotoDistrict != null && gotoDistrict.Trim().Length > 0)
        {
            Response.Redirect("ToDispatch.aspx?DistrictCode=" + gotoDistrict); 
        }

        ucDispatchHeader.SetTitle("District Summary"); 
    }

    protected string GotoDistrictSummary(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        return "DistrictSummary.aspx?GoToDistrict=" + view["DistrictCode"]; 
    }



    protected string Alerts(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 alerts = (Int32) view["Alerts"]; 

        return "A:" + alerts.ToString("00");
    }

    protected string AlertItemStyle(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 alerts = (Int32)view["Alerts"];
        Int32 undelivered = (Int32)view["Undelivered"];

        if (alerts > 0)
            return "AlertItem";
        else
            return "CompletedItem";
    }


    protected string ComplaintItemStyle(Object inObject)
    {
        DataRowView view = inObject as DataRowView;
        Int32 undelivered = (Int32)view["Undelivered"];

        if (undelivered == 0)
            return "CompletedItem";
        else
            return "ComplaintItem";
    }


    protected string ViewedItemStyle(Object inObject)
    {
        DataRowView view = inObject as DataRowView;
        Int32 undelivered = (Int32)view["Undelivered"];

        if (undelivered == 0)
            return "CompletedItem";
        else
            return "ViewedItem";
    }

    protected string UndeliveredItemStyle(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 undelivered = (Int32)view["Undelivered"];

        if (undelivered == 0)
            return "CompletedItem";
        else
            return "UnviewedItem";
    }



    protected string Complaints(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 complaints = (Int32)view["Complaints"];

        return "C:" + complaints.ToString("00");
    }

    protected string Delivered(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 delivered = (Int32)view["Delivered"];

        return "D:" + delivered.ToString("00");
    }

    protected string Viewed(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 viewed = (Int32)view["Viewed"];

        return "V:" + viewed.ToString("00");
    }

    protected string Undelivered(Object inObject)
    {
        DataRowView view = inObject as DataRowView;

        Int32 undelivered = (Int32)view["Undelivered"];

        return "U:" + undelivered.ToString("00");
    }

    protected void btnDrillDown_Click(object sender, EventArgs e)
    {
        Button btnPressed = sender as Button;

        if (btnPressed != null)
        {
            Response.Redirect("ToDispatch.aspx?DistrictCode=" + btnPressed.Text); 
        }
    }
}
