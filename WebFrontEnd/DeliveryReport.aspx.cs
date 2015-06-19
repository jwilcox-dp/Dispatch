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

public partial class DeliveryReport : BasePage
{
    private static string _deliveryReportSearchType = "DeliveryReportSearchType"; 
    private static string _deliveryReportZone = "DelveryReportZone"; 
    private static string _deliveryReportDistrict = "DeliveryReportDistrict"; 

    private static string _deliveryReportStreetNumber = "DeliveryReportStreetNumber"; 
    private static string _deliveryReportStreetFraction = "DeliveryReportStreetNumberFraction"; 
    private static string _deliveryReportStreetName = "DeliveryReportStreetName"; 
    private static string _deliveryReportStreetType = "DeliveryReportStreetType"; 
    private static string _deliveryReportStreetDirection = "DeliveryReportStreetDirection"; 
    private static string _deliveryReportApartment = "DeliveryReportApartment"; 
    private static string _deliveryReportCityCode = "DeliveryReportCityCode"; 

    private static string _deliveryReportPhoneNumber = "DeliveryReportPhoneNumber";

    private static string _deliveryReportRouteCode = "DeliveryReportRouteCode";


    private static string _deliveryReportSortExpression = "DeliveryReportSortExpression";
    private static string _deliveryReportSortDirection = "DeliveryReportSortDirection";

    private static string _deliveryReportRecordsPerPage = "DeliveryReportRecordsPerPage";

    private string _searchType = string.Empty; 



    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e); 
        if (!IsPostBack)
        {
            calFromDate.SelectedDate = DateTime.Now;
            calToDate.SelectedDate = DateTime.Now;

            if (PartnerPubZone != string.Empty)
            {
                pnlSelectZoneAndDistrict.Visible = false;
                pnlSelectDistrict.Visible = true;
                sdsGetDistricts.SelectParameters[0].DefaultValue = PartnerPubZone;
                lbPartnerDistricts.DataBind(); 
            }
            else
            {
                pnlSelectZoneAndDistrict.Visible = true;
                pnlSelectDistrict.Visible = false;
                lbZone.DataBind(); 
            }

            GetUserSettings();

            SetSearchType(); 
        }
        ucDispatchHeader.SetTitle("Delivery Report"); 
    }

//    public void SetSearchType(object sender, EventArgs e)
    public void SetSearchType()
    {
        lblSearchCriteriaError.Visible = false;
        gvZoneManager.Visible = false;

        _searchType = Request.QueryString["Type"];

        if (_searchType == null || _searchType.Trim().Length == 0)
        {
            _searchType = "ZoneDistrict"; 
        }

        switch (_searchType)
        {
            case "ZoneDistrict":
                mvSearchType.SetActiveView(viewZoneDistrict);
                cbUseDates.Checked = true;
                cbUseDates.Enabled = false;
                calFromDate.SelectedDate = DateTime.Now;
                calToDate.SelectedDate = DateTime.Now;
                if (lbZone.Visible)
                {
                    if (lbZone.SelectedIndex < 0)
                        lbZone.SelectedIndex = 0;

                    if (lbDistrict.SelectedIndex < 0)
                    {
                        lblDistrict.Visible = false;
                        lbDistrict.Visible = false;
                    }
                }
                break;
            case "Address":
                mvSearchType.SetActiveView(viewAddress);
                cbUseDates.Enabled = true;
                break;
            case "Phone":
                mvSearchType.SetActiveView(viewPhone);
                cbUseDates.Enabled = true;
                break;
            case "Route":
                mvSearchType.SetActiveView(viewRoute);
                cbUseDates.Enabled = false;
                cbUseDates.Checked = true;
                break;
        }
    }

    protected void btnSearch_Click1(object sender, EventArgs e)
    {
        lblSearchCriteriaError.Visible = false;

        #region Address Search
        if (mvSearchType.ActiveViewIndex == 0)
        {
            // If searching only by city code, then force use of dates to prevent to much data being retrieved
            if (tbStreetNumber.Text.Trim().Length == 0 &&
                tbStreetFraction.Text.Trim().Length == 0 &&
                tbStreetName.Text.Trim().Length == 0 &&
                tbStreetType.Text.Trim().Length == 0 &&
                tbStreetDirection.Text.Trim().Length == 0 &&
                tbApartment.Text.Trim().Length == 0)
            {
                if (tbCityCode.Text.Trim().Length == 0)
                {
                    lblSearchCriteriaError.Visible = true;
                    return;
                }
                else
                {
                    cbUseDates.Checked = true;
                }
            }

            if (cbUseDates.Checked)
            {
                sdsSearchByAddressAndDate.SelectParameters[0].DefaultValue = CleanseInput(tbStreetNumber.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[1].DefaultValue = CleanseInput(tbStreetFraction.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[2].DefaultValue = CleanseInput(tbStreetName.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[3].DefaultValue = CleanseInput(tbStreetType.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[4].DefaultValue = CleanseInput(tbStreetDirection.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[5].DefaultValue = CleanseInput(tbApartment.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[6].DefaultValue = CleanseInput(tbCityCode.Text) + "%";
                sdsSearchByAddressAndDate.SelectParameters[7].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
                sdsSearchByAddressAndDate.SelectParameters[8].DefaultValue = calToDate.SelectedDate.ToShortDateString(); 

                gvZoneManager.DataSourceID = "sdsSearchByAddressAndDate";

                if (PartnerPubZone != string.Empty)
                {
                    sdsSearchByAddressAndDate.SelectCommand = "GetRequestsByAddressDateAndZone"; 
                    if (sdsSearchByAddressAndDate.SelectParameters.Count == 9)
                        sdsSearchByAddressAndDate.SelectParameters.Add("ZoneCode", PartnerPubZone);
                    else 
                        sdsSearchByAddressAndDate.SelectParameters[9].DefaultValue = PartnerPubZone; 
                }
            }
            else
            {
                sdsSearchByAddress.SelectParameters[0].DefaultValue = CleanseInput(tbStreetNumber.Text) + "%";
                sdsSearchByAddress.SelectParameters[1].DefaultValue = CleanseInput(tbStreetFraction.Text) + "%";
                sdsSearchByAddress.SelectParameters[2].DefaultValue = CleanseInput(tbStreetName.Text) + "%";
                sdsSearchByAddress.SelectParameters[3].DefaultValue = CleanseInput(tbStreetType.Text) + "%";
                sdsSearchByAddress.SelectParameters[4].DefaultValue = CleanseInput(tbStreetDirection.Text) + "%";
                sdsSearchByAddress.SelectParameters[5].DefaultValue = CleanseInput(tbApartment.Text) + "%";
                sdsSearchByAddress.SelectParameters[6].DefaultValue = CleanseInput(tbCityCode.Text) + "%";

                if (PartnerPubZone != string.Empty)
                {
                    sdsSearchByAddress.SelectCommand = "GetRequestsByAddressAndZone"; 

                    if (sdsSearchByAddress.SelectParameters.Count == 7)
                        sdsSearchByAddress.SelectParameters.Add("ZoneCode", PartnerPubZone);
                    else 
                        sdsSearchByAddress.SelectParameters[7].DefaultValue = PartnerPubZone;
                }

                gvZoneManager.DataSourceID = "sdsSearchByAddress"; 
            }
        }
        #endregion
        #region Phone Search
        else if (mvSearchType.ActiveViewIndex == 1)
        {
            if (tbPhoneNumber.Text.Trim().Length == 0)
            {
                lblSearchCriteriaError.Visible = true;
                gvZoneManager.Visible = false; 

                return;
            }

            if (cbUseDates.Checked)
            {
                sdsSearchByPhoneAndDate.SelectParameters[0].DefaultValue = CleanseInput(tbPhoneNumber.Text);
                sdsSearchByPhoneAndDate.SelectParameters[1].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
                sdsSearchByPhoneAndDate.SelectParameters[2].DefaultValue = calToDate.SelectedDate.ToShortDateString();

                if (PartnerPubZone != string.Empty)
                {
                    sdsSearchByPhoneAndDate.SelectCommand = "GetRequestsByPhoneNumberDateAndZone"; 
                    
                    if (sdsSearchByPhoneAndDate.SelectParameters.Count == 3)
                        sdsSearchByPhoneAndDate.SelectParameters.Add("ZoneCode", PartnerPubZone);
                    else 
                        sdsSearchByPhoneAndDate.SelectParameters[3].DefaultValue = PartnerPubZone;
                }
                gvZoneManager.DataSourceID = "sdsSearchByPhoneAndDate";
            }
            else
            {
                sdsSearchByPhone.SelectParameters[0].DefaultValue = CleanseInput(tbPhoneNumber.Text);

                if (PartnerPubZone != string.Empty)
                {
                    sdsSearchByPhone.SelectCommand = "GetRequestsByPhoneNumberAndZone";

                    if (sdsSearchByPhone.SelectParameters.Count == 1)
                        sdsSearchByPhone.SelectParameters.Add("ZoneCode", PartnerPubZone);
                    else 
                        sdsSearchByPhone.SelectParameters[1].DefaultValue = PartnerPubZone;
                }

                gvZoneManager.DataSourceID = "sdsSearchByPhone";
            }
        }
        #endregion 
        #region Route Search
        else if (mvSearchType.ActiveViewIndex == 2)
        {
            if (tbRouteCode.Text.Trim().Length == 0)
            {
                lblSearchCriteriaError.Visible = true;
                gvZoneManager.Visible = false; 
                return;
            }
            sdsSearchByRouteCodeAndDate.SelectParameters[0].DefaultValue = CleanseInput(tbRouteCode.Text).Trim() + '%';
            sdsSearchByRouteCodeAndDate.SelectParameters[1].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
            sdsSearchByRouteCodeAndDate.SelectParameters[2].DefaultValue = calToDate.SelectedDate.ToShortDateString();

            if (PartnerPubZone != string.Empty)
            {
                sdsSearchByRouteCodeAndDate.SelectCommand = "GetRequestsByRouteCodeDateAndZone";
                if (sdsSearchByRouteCodeAndDate.SelectParameters.Count == 3)
                    sdsSearchByRouteCodeAndDate.SelectParameters.Add("ZoneCode", PartnerPubZone);
                else 
                    sdsSearchByRouteCodeAndDate.SelectParameters[3].DefaultValue = PartnerPubZone;  
            }

            gvZoneManager.DataSourceID = "sdsSearchByRouteCodeAndDate";
        }
        #endregion
        #region Partner District Search
        else if (lbPartnerDistricts.Visible)
        {
            if (lbPartnerDistricts.SelectedValue == string.Empty)
            {
                sdsSearchByZoneAndDate.SelectParameters[0].DefaultValue = PartnerPubZone; ;
                sdsSearchByZoneAndDate.SelectParameters[1].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
                sdsSearchByZoneAndDate.SelectParameters[2].DefaultValue = calToDate.SelectedDate.ToShortDateString();

                gvZoneManager.DataSourceID = "sdsSearchByZoneAndDate";
            }
            else
            {
                sdsSearchByDistrictAndDate.SelectParameters[0].DefaultValue = lbPartnerDistricts.SelectedValue as string;
                sdsSearchByDistrictAndDate.SelectParameters[1].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
                sdsSearchByDistrictAndDate.SelectParameters[2].DefaultValue = calToDate.SelectedDate.ToShortDateString();

                gvZoneManager.DataSourceID = "sdsSearchByDistrictAndDate";
            }

        }
        #endregion
        #region District Search (DNA)
        else if (lbDistrict.Visible == true)
        {
            Session["District"] = lbDistrict.SelectedValue;
            sdsSearchByDistrictAndDate.SelectParameters[0].DefaultValue = lbDistrict.SelectedValue as string;

            sdsSearchByDistrictAndDate.SelectParameters[1].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
            sdsSearchByDistrictAndDate.SelectParameters[2].DefaultValue = calToDate.SelectedDate.ToShortDateString();

            gvZoneManager.DataSourceID = "sdsSearchByDistrictAndDate";

        }
        #endregion
        #region Zone Search
        else
        {
            sdsSearchByZoneAndDate.SelectParameters[0].DefaultValue = lbZone.SelectedValue as string;
            sdsSearchByZoneAndDate.SelectParameters[1].DefaultValue = calFromDate.SelectedDate.ToShortDateString();
            sdsSearchByZoneAndDate.SelectParameters[2].DefaultValue = calToDate.SelectedDate.ToShortDateString();

            gvZoneManager.DataSourceID = "sdsSearchByZoneAndDate";

            lblDistrict.Visible = false;
            lbDistrict.Visible = false;
        }
        #endregion
        gvZoneManager.PageSize = Int32.Parse(ddlRowsPerPage.Text); 
        gvZoneManager.Visible = true;
        gvZoneManager.EnableSortingAndPagingCallbacks = false; 
        gvZoneManager.DataBind();
        SaveUserSettings(); 
    }

    protected void btnSelectZone_Click(object sender, EventArgs e)
    {
        gvZoneManager.Visible = false;
        lblSearchCriteriaError.Visible = false; 
        Session["Zone"] = lbZone.SelectedValue;
        sdsGetDistricts.SelectParameters[0].DefaultValue = Session["Zone"] as string;
        lbDistrict.DataBind();
        lbDistrict.SelectedIndex = 0; 
        lbDistrict.Focus();
        lblDistrict.Visible = true; 
        lbDistrict.Visible = true;
        lblFilterZoneInfo.Visible = false; 
    }
    protected void lbZone_SelectedIndexChanged(object sender, EventArgs e)
    {
        lbDistrict.Visible = false;
        lblDistrict.Visible = false;
        lblFilterZoneInfo.Visible = true;
        gvZoneManager.Visible = false;
    }


    private void GetUserSettings()
    {
        HttpCookie userId = Request.Cookies["UserId"];

        if (userId == null)
        {
            HttpCookie newUser = Response.Cookies["UserId"];
            newUser.Expires = DateTime.Now.AddYears(1);
            newUser.Value = Guid.NewGuid().ToString();
            userId = newUser;
        }
        DSUserSettings dsSettings = DataUserSettings.GetUserSettingsByUserId(userId.Value);

        DSUserSettings.UserSettingsRow[] settingRows = dsSettings.UserSettings.Select(
            string.Format("Setting = '{0}'", _deliveryReportSearchType)) as DSUserSettings.UserSettingsRow[];

        string searchType = string.Empty; 
        if (settingRows.Length == 0)
            return; 
        else 
            searchType = settingRows[0].Value;

        string sortExpression = string.Empty;
        SortDirection sortDirection = SortDirection.Ascending; 

        settingRows = dsSettings.UserSettings.Select(
            string.Format("Setting = '{0}'", _deliveryReportSortExpression)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
            sortExpression = settingRows[0].Value; 

        settingRows = dsSettings.UserSettings.Select(
            string.Format("Setting = '{0}'", _deliveryReportSortDirection)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
        {
            if (settingRows[0].Value.ToUpper().Contains("ASC"))
                sortDirection = SortDirection.Ascending;
            else
                sortDirection = SortDirection.Descending;
        }

        if (sortExpression != string.Empty)
        {
            gvZoneManager.Sort(sortExpression, sortDirection); 
        }

        settingRows = dsSettings.UserSettings.Select(
              string.Format("Setting = '{0}'", _deliveryReportRecordsPerPage)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
        {
            ListItem toSelect = ddlRowsPerPage.Items.FindByValue(settingRows[0].Value); 
            if (toSelect != null)
                ddlRowsPerPage.SelectedIndex = ddlRowsPerPage.Items.IndexOf(toSelect); 
        }

        switch (searchType)
        {
            case "Address":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetNumber)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                     tbStreetNumber.Text = settingRows[0].Value;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetFraction)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbStreetFraction.Text = settingRows[0].Value;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetName)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbStreetName.Text = settingRows[0].Value;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetType)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbStreetType.Text = settingRows[0].Value;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetDirection)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbStreetDirection.Text = settingRows[0].Value;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportApartment)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbApartment.Text = settingRows[0].Value;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportCityCode)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbCityCode.Text = settingRows[0].Value;

                break;

            case "Phone":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportPhoneNumber)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbPhoneNumber.Text = settingRows[0].Value; 

                break;

            case "Route":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportRouteCode)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    tbRouteCode.Text = settingRows[0].Value;

                break;

            case "ZoneDistrict":
                lbDistrict.Visible = false;
                lblDistrict.Visible = false;
                lblFilterZoneInfo.Visible = true;

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportZone)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                {
                    ListItem zoneToSelect = lbZone.Items.FindByValue(settingRows[0].Value);
                    if (zoneToSelect != null)
                    {
                        lbZone.SelectedIndex = lbZone.Items.IndexOf(zoneToSelect);

                        btnSelectZone_Click(null, null);

                        settingRows = dsSettings.UserSettings.Select(
                            string.Format("Setting = '{0}'", _deliveryReportDistrict)) as DSUserSettings.UserSettingsRow[];

                        if (settingRows.Length > 0)
                        {
                            ListItem districtToSelect = lbDistrict.Items.FindByValue(settingRows[0].Value);
                            if (districtToSelect != null)
                            {
                                lbDistrict.SelectedIndex = lbDistrict.Items.IndexOf(districtToSelect);
                                lbDistrict.Visible = true;
                                lblDistrict.Visible = true;
                                lblFilterZoneInfo.Visible = false;
                            }
                        }
                    }
                }
                else
                {
                    lbZone.SelectedIndex = 0; 
                }
                break;
        }
        SetSearchType(); 
    }




    private void SaveUserSettings()
    {
        HttpCookie userId = Request.Cookies["UserId"];

        if (userId == null)
        {
            HttpCookie newUser = Response.Cookies["UserId"];
            newUser.Expires = DateTime.Now.AddYears(1);
            newUser.Value = Guid.NewGuid().ToString();
            userId = newUser;
        }

        string sortExpression = gvZoneManager.SortExpression;
        string sortDirection = gvZoneManager.SortDirection.ToString();

        DSUserSettings dsSettings = DataUserSettings.GetUserSettingsByUserId(userId.Value);

        string searchType = string.Empty;
        switch (mvSearchType.ActiveViewIndex)
        {
            case 0:
                searchType = "Address";
                break;
            case 1:
                searchType = "Phone";
                break;
            case 2:
                searchType = "Route";
                break;
            case 3:
                searchType = "ZoneDistrict";
                break;

        }

        DSUserSettings.UserSettingsRow[] settingRows = dsSettings.UserSettings.Select(
            string.Format("Setting = '{0}'", _deliveryReportSearchType)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
            settingRows[0].Value = searchType;
        else
            dsSettings.UserSettings.AddUserSettingsRow(userId.Value, 
                _deliveryReportSearchType, searchType, DateTime.Now, Request.UserAgent);

        settingRows = dsSettings.UserSettings.Select(
            string.Format("Setting = '{0}'", _deliveryReportSortExpression)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
            settingRows[0].Value = sortExpression;
        else
            dsSettings.UserSettings.AddUserSettingsRow(userId.Value, 
                _deliveryReportSortExpression, sortExpression, DateTime.Now, Request.UserAgent);

        settingRows = dsSettings.UserSettings.Select(
            string.Format("Setting = '{0}'", _deliveryReportSortDirection)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
            settingRows[0].Value = sortDirection;
        else
            dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                _deliveryReportSortDirection, sortDirection, DateTime.Now, Request.UserAgent);

        switch (searchType)
        {
            case "Address":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetNumber)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbStreetNumber.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportStreetNumber, CleanseInput(tbStreetNumber.Text), DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetFraction)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbStreetFraction.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportStreetFraction, CleanseInput(tbStreetFraction.Text), DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetName)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbStreetName.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportStreetName, CleanseInput(tbStreetName.Text), DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetType)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbStreetType.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportStreetType, CleanseInput(tbStreetType.Text), DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportStreetDirection)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbStreetDirection.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportStreetDirection, CleanseInput(tbStreetDirection.Text), DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportApartment)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbApartment.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportApartment, CleanseInput(tbApartment.Text), DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportCityCode)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbCityCode.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportCityCode, CleanseInput(tbCityCode.Text), DateTime.Now, Request.UserAgent);
                break; 

            case "Phone":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportPhoneNumber)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbPhoneNumber.Text);
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportPhoneNumber, CleanseInput(tbPhoneNumber.Text), DateTime.Now, Request.UserAgent);


                break;

            case "Route":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportRouteCode)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = CleanseInput(tbRouteCode.Text); 
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportRouteCode, CleanseInput(tbRouteCode.Text), DateTime.Now, Request.UserAgent);


                break; 

            case "ZoneDistrict":
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportZone)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = lbZone.SelectedValue; 
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportZone, lbZone.SelectedValue, DateTime.Now, Request.UserAgent);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = '{0}'", _deliveryReportDistrict)) as DSUserSettings.UserSettingsRow[];

                if (settingRows.Length > 0)
                    settingRows[0].Value = lbDistrict.SelectedValue;
                else
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        _deliveryReportDistrict, lbDistrict.SelectedValue, DateTime.Now, Request.UserAgent);
                break; 
        }


        settingRows = dsSettings.UserSettings.Select(
              string.Format("Setting = '{0}'", _deliveryReportRecordsPerPage)) as DSUserSettings.UserSettingsRow[];

        if (settingRows.Length > 0)
            settingRows[0].Value = ddlRowsPerPage.SelectedValue; 
        else
            dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                  _deliveryReportRecordsPerPage, ddlRowsPerPage.SelectedValue, DateTime.Now, Request.UserAgent);


        DataUserSettings.SaveUserSettings(dsSettings.UserSettings); 
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
            if (toReturn.Length > 0)
                toReturn = toReturn + " - ";

            toReturn = toReturn + view["Message"].ToString();
        }

        if (toReturn.Length > 0)
            toReturn = toReturn + "<br />";
        return toReturn;
    }


    string CleanseInput(string inInput)
    {
        return inInput.Replace(";", "").Replace("(", "").Replace(")", "").Replace("-", "").Trim(); 
    }
    protected void btnZoneDistrictSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("DeliveryReport.aspx?Type=ZoneDistrict"); 
    }
    protected void btnAddressSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("DeliveryReport.aspx?Type=Address");
    }
    protected void btnPhoneSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("DeliveryReport.aspx?Type=Phone");
    }
    protected void btnRouteSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("DeliveryReport.aspx?Type=Route");
    }
}
