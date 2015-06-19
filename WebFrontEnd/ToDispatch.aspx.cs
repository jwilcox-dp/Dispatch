using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using System.Xml.Linq;


public partial class ToDispatch : BasePage
{
    private bool isReadOnly = false;
    private DSTodaysComplaints dsForAlerts = null; 
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        ucDispatchHeader.SetTitle("Dispatch Items"); 

        BindDataGrid();
    }

    private void BindDataGrid()
    {
        if (Request.QueryString["DistrictCode"] != null)
        {
            string districtCode = string.Empty;
            dlToDispatch.DataSourceID = "sdsGetTodaysDispatches";
            sdsGetTodaysDispatches.SelectParameters[1].DefaultValue = districtCode = Request.QueryString["DistrictCode"];
            sdsGetTodaysDispatches.SelectParameters[0].DefaultValue = "RouteCode";
            isReadOnly = true;

            dsForAlerts = new DSTodaysComplaints();
            DSTodaysComplaintsTableAdapters.ViewTodaysAlertsByDistrictTableAdapter forFill = new DSTodaysComplaintsTableAdapters.ViewTodaysAlertsByDistrictTableAdapter();
            forFill.FillByDistrictCode(dsForAlerts.ViewTodaysAlertsByDistrict, districtCode); 
        }
        else if (GetSession().ViewType.ToUpper() == "UNDISPATCHED")
        {
            dlToDispatch.DataSourceID = "sdsGetUndispatched";
            sdsGetUndispatched.SelectParameters[1].DefaultValue = GetSession().CurrentDistrict;
            sdsGetUndispatched.SelectParameters[0].DefaultValue = GetSession().SortOrder;

        }
        else if (GetSession().ViewType.ToUpper() == "UNDELIVERED")
        {
            dlToDispatch.DataSourceID = "sdsGetUndelivered";
            sdsGetUndelivered.SelectParameters[1].DefaultValue = GetSession().CurrentDistrict;
            sdsGetUndelivered.SelectParameters[0].DefaultValue = GetSession().SortOrder;
        }
        else
        {
            dlToDispatch.DataSourceID = "sdsGetTodaysDispatches";
            sdsGetTodaysDispatches.SelectParameters[1].DefaultValue = GetSession().CurrentDistrict;
            sdsGetTodaysDispatches.SelectParameters[0].DefaultValue = GetSession().SortOrder;
        }

        dlToDispatch.DataBind();

        SetButtonAttributes();
        dlToDispatch.Focus(); 
    }

    protected string ParseCreateTime(object inObject)
    {
        DataRowView view = inObject as DataRowView;

        return view["ElapsedTime"].ToString() + " min"; 
    }

    protected string CustomerLookupURL(Object inObject)
    {
        string toReturn = "CustomerLookup.aspx";

        DataRowView view = inObject as DataRowView;

        if (view["AddressKey"] != null)
            toReturn = toReturn + "?AddressKey=" + view["AddressKey"].ToString().Replace(" ", "!"); 

        return toReturn; 
    }

    protected string ParseMessageString(Object inObject)
    {
        DataRowView view = inObject as DataRowView;
        string toReturn = "";

        if (view["ComplaintDescription"] != null)
            toReturn = view["ComplaintDescription"].ToString();
        else if (view["ComplaintCode"] != null)
            toReturn = view["ComplaintCode"].ToString();

        if (toReturn.Length > 0)
        {
            toReturn = "(C) " + toReturn;
        }
        else
        {
            toReturn = "(M) " + toReturn;
        }

        if (view["Message"] != null && view["Message"].ToString().Trim().Length > 0)
        {
            toReturn = toReturn + view["Message"].ToString();
        }

        if (toReturn.Length > 0)
            toReturn = toReturn + "<br />";
        return toReturn;
    }

    protected string GetEvenColor(Object inObject)
    {
        return GetRowColor(inObject, "ColorEven");
    }

    protected string GetOddColor(Object inObject)
    {
        return GetRowColor(inObject, "ColorOdd");
    }


    protected string GetRowColor(Object inObject, string inEvenOdd)
    {
        if (inObject != null)
        {
            DataRowView rowView = inObject as DataRowView;

            if (dsForAlerts != null)
            {
                if (rowView["RouteCode"] != null)
                {
                    if (dsForAlerts.ViewTodaysAlertsByDistrict.Select(string.Format("RouteCode = '{0}'", rowView["RouteCode"] as String)).Length > 0)
                        return "AlertColor";
                }
            }

            if (rowView["ComplaintCode"] != null && rowView["ComplaintCode"].ToString().Contains("HC"))
                return GetSession().ColorHotComplaint;

            if (rowView["DeliveredDate"] != null)
            {
                Nullable<DateTime> redeliverDate = rowView["DeliveredDate"] as Nullable<DateTime>;
                if (redeliverDate.HasValue)
                    return GetSession().ColorRedelivered;
            }
            if (rowView["ElapsedTime"] != null)
            {
                Nullable<Int32> elapsedTime = rowView["ElapsedTime"] as Nullable<Int32>;
                if (elapsedTime.Value > 60)
                    return GetSession().ColorET;
            }
            if (rowView.Row["ProcessedDate"] != null)
            {
                Nullable<DateTime> pubDate = rowView.Row["ProcessedDate"] as Nullable<DateTime>;
                if (pubDate.HasValue)
                {
                    if (pubDate.Value < DateTime.Now.Date.AddHours(-8))
                        return GetSession().ColorYesterday;
                }
            }
            if (inEvenOdd == "ColorEven")
                return GetSession().ColorEven;
            else
                return GetSession().ColorOdd;
        }
        return GetSession().ColorEven; 
    }

    protected void dlToDispatch_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        Button delButton = e.Item.FindControl("btnDel") as Button;

        if (isReadOnly)
            delButton.Visible = false; 

        // If request has already been delivered, disable the Deliver button.
        System.Data.DataRowView dataRow = e.Item.DataItem as System.Data.DataRowView;
        if (dataRow != null)
        {
            Nullable<DateTime> redeliverDate = dataRow["DeliveredDate"] as Nullable<DateTime>;
            if (redeliverDate.HasValue)
                delButton.Enabled = false;
        }
    }

    public void SetButtonAttributes()
    {
        if (isReadOnly)
        {
            btnTopUndelivered.Visible = false;
            btnTopAll.Visible = false;
            btnTopSettings.Visible = false; 
            btnBottomUndelivered.Visible = false;
            btnBottomAll.Visible = false;
            btnBottomSettings.Visible = false; 
        }
        else
        {
            btnTopUndelivered.Font.Bold = false;
            btnTopUndelivered.Font.Underline = false;

            btnTopAll.Font.Bold = false;
            btnTopAll.Font.Underline = false;

            btnBottomUndelivered.Font.Bold = false;
            btnBottomUndelivered.Font.Underline = false;

            btnBottomAll.Font.Bold = false;
            btnBottomAll.Font.Underline = false;


            switch (GetSession().ViewType.ToUpper())
            {
                case "UNDELIVERED":
                    btnTopUndelivered.Font.Bold = true;
                    btnTopUndelivered.Font.Underline = true;
                    btnBottomUndelivered.Font.Bold = true;
                    btnBottomUndelivered.Font.Underline = true;
                    break;
                default:
                    btnTopAll.Font.Bold = true;
                    btnTopAll.Font.Underline = true;
                    btnBottomAll.Font.Bold = true;
                    btnBottomAll.Font.Underline = true;
                    break;
            }
        }
    }
    protected void btnTopUndelivered_Click(object sender, EventArgs e)
    {
        GetSession().ViewType = "UNDELIVERED";

        SavePreferences();

        BindDataGrid(); 
    }
    protected void btnTopAll_Click(object sender, EventArgs e)
    {
        GetSession().ViewType = "ALL";

        SavePreferences();

        BindDataGrid(); 
    }
    protected void btnTopSettings_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Preferences.aspx"); 
    }



    protected void dlToDispatch_ItemCommand(object source, DataListCommandEventArgs e)
    {
        string recordId = e.CommandArgument.ToString();

        /*var DispatchRecord = from dataRow in ((DataTable)dlToDispatch.DataSource).AsEnumerable()
                             where dataRow.Field<string>("Id") == recordId
                             select dataRow;*/
        Label pubCodeLabel = (Label)e.Item.FindControl("PubCodeLabel");
        Label ncsDispatchIDLabel = (Label)e.Item.FindControl("NCSDispatchIDLabel");

        int returnValue = UpdateNcsDispatchRequest(
            pubCodeLabel.Text,
            ncsDispatchIDLabel.Text);

        sdsConfirmDelivery.UpdateParameters[0].DefaultValue = recordId;
        sdsConfirmDelivery.UpdateParameters[1].DefaultValue = (returnValue == 0).ToString();
        sdsConfirmDelivery.Update();
        BindDataGrid(); 
    }
    protected void dlToDispatch_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="dispatchId"></param>
    /// <returns></returns>
    protected int UpdateNcsDispatchRequest(
        string productId
        , string dispatchId)
    {
        int returnValue;

        try
        {
            
            string complianceUri = @"{0}?ProductID={1}&DispatchID={2}&ComplianceDate={3}&ComplianceTime={4}&ComplianceRemarks={5}";
            DateTime _timestamp = DateTime.Now;
            string ComplianceDate = _timestamp.ToString("yyyyMMdd");
            string ComplianceTime = _timestamp.ToString("HHmmss");
            string ComplianceRemarks = "Redelivered Paper";

            string ComplianceURL = String.Format(
                complianceUri,
                ConfigurationManager.AppSettings["NCSComplianceURL"],
                productId,
                dispatchId,
                ComplianceDate,
                ComplianceTime,
                ComplianceRemarks);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ComplianceURL);
            string responseString = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();

            //trim whitespace and comments
            XElement xmlResponse = XElement.Load(new StringReader(
                    responseString = responseString.Substring(responseString.IndexOf(@"<Compliance>"), responseString.IndexOf(@"</Compliance>") - responseString.IndexOf(@"<Compliance>") + @"</Compliance>".Length)));

            if (xmlResponse.Elements("CompletionStatus").Elements("ReturnStatus").Single<XElement>().Value == @"Completed")
            {
                returnValue = 0;
            }
            else
            {
                returnValue = Convert.ToInt32(xmlResponse.Elements("CompletionStatus").Elements("ErrorCode").Single().Value);
            }
        }
        catch
        {
            returnValue = -1;
        }
        
        return returnValue;
    }
}
