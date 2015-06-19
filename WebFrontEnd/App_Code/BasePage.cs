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

public partial class BasePage : System.Web.UI.Page
{
    DispatchSession _dispatchSession = null;
    string _partnerPubZone = string.Empty;
    string _partnerPubName = string.Empty; 

    public string PartnerPubZone
    {
        get { return _partnerPubZone; }
    }

    public string PartnerPubName
    {
        get { return _partnerPubName; }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
//        SaveStateComplete += new EventHandler(BasePage_SaveStateComplete);

        if (ConfigurationSettings.AppSettings["PartnerPubName"] != null)
            _partnerPubName = ConfigurationSettings.AppSettings["PartnerPubName"];

        if (ConfigurationSettings.AppSettings["PartnerPubZone"] != null)
            _partnerPubZone = ConfigurationSettings.AppSettings["PartnerPubZone"]; 


        // If we can load the user's session, do so....
        _dispatchSession = Session["DispatchSession"] as DispatchSession;

        // Otherwise, create a new DispatchSession instance 
        if (_dispatchSession == null)
        {
            _dispatchSession = new DispatchSession();

            // And load the preferences into the DispatchSession object
            LoadPreferences(); 
        }
    }


    public DispatchSession GetSession()
    {
            return _dispatchSession;
    }

    /// <summary>
    /// Load the user's preferences, which are kept in a SQL Server database table.  
    /// </summary>
    private void LoadPreferences()
    {
        try
        {
            HttpCookie userId = Request.Cookies["UserId"];

            if (userId != null && userId.Value != null && userId.Value.Trim().Length > 0)
            {
                DSUserSettings dsSettings = DataUserSettings.GetUserSettingsByUserId(userId.Value);

                string today = DateTime.Now.DayOfWeek.ToString();

                DSUserSettings.UserSettingsRow[] settingRows;
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ColorEven{0}'", today)) as DSUserSettings.UserSettingsRow[];

                _dispatchSession.ColorEven = ParseSettingColor(settingRows);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ColorOdd{0}'", today)) as DSUserSettings.UserSettingsRow[];
                _dispatchSession.ColorOdd = ParseSettingColor(settingRows);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ColorET{0}'", today)) as DSUserSettings.UserSettingsRow[];
                _dispatchSession.ColorET = ParseSettingColor(settingRows);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ColorHotComplaint{0}'", today)) as DSUserSettings.UserSettingsRow[];
                _dispatchSession.ColorHotComplaint = ParseSettingColor(settingRows);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ColorYesterday{0}'", today)) as DSUserSettings.UserSettingsRow[];
                _dispatchSession.ColorYesterday = ParseSettingColor(settingRows);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ColorRedelivered{0}'", today)) as DSUserSettings.UserSettingsRow[];
                _dispatchSession.ColorRedelivered = ParseSettingColor(settingRows);

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'SortOrder{0}'", today)) as DSUserSettings.UserSettingsRow[];
                if (settingRows.Length > 0)
                {
                    _dispatchSession.SortOrder = settingRows[0].Value;
                }

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'ViewType{0}'", today)) as DSUserSettings.UserSettingsRow[];
                if (settingRows.Length > 0)
                {
                    _dispatchSession.ViewType = settingRows[0].Value;
                }

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'SelectedDistricts{0}'", today)) as DSUserSettings.UserSettingsRow[];
                if (settingRows.Length > 0)
                {
                    _dispatchSession.SelectedDistricts = settingRows[0].Value;
                }

                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'CurrentDistrict{0}'", today)) as DSUserSettings.UserSettingsRow[];
                if (settingRows.Length > 0)
                {
                    _dispatchSession.CurrentDistrict = settingRows[0].Value;
                }
                _dispatchSession.ClearChangesMade();
            }
        }
        catch (Exception ex)
        {
            _dispatchSession = new DispatchSession();
            Email.ReportErrorToProgrammers("Dispatch Problem", "BasePage.LoadPreferences() triggered the following exception: " + ex.ToString());
        }
    }

    private string ParseSettingColor(DSUserSettings.UserSettingsRow[] settingRows)
    {
        if (settingRows.Length > 0)
        {
            if (settingRows[0].Value.ToString().Contains("Color"))
            {
                return settingRows[0].Value.ToString();
            }
            else
            {
                // The original system set the Background Color of the control item, so the 
                // color code for the background color was kept.  Background color is now set 
                // via a CssClass, so we now use CssClass names.  If we encounter an "old style" setting, 
                // migrate that setting to the "new style".  
                switch (settingRows[0].Value.ToString())
                {
                    case "CC9900": return "ColorBrown"; 
                    case "FFCC99": return "ColorOrange"; 
                    case "FFFF66": return "ColorYellow"; 
                    case "CCFF66": return "ColorGreen"; 
                    case "99CCFF": return "ColorBlue"; 
                    case "CCCCCC": return "ColorGray"; 
                    case "FF99CC": return "ColorRed"; 
                    case "CC99FF": return "ColorPurple"; 
                    case "FFFFFF": return "ColorWhite";
                    default: return "ColorWhite"; 
                }
            }
        }
        else
            return "ColorWhite"; 
    }

    virtual public void SavePreferences()
    {
        try
        {
            // If no changes made, don't save preferences.  
            if (!_dispatchSession.GetChangesMade())
                return;

            HttpCookie userId = Request.Cookies["UserId"];

            if (userId == null)
            {
                HttpCookie newUser = Response.Cookies["UserId"];
                newUser.Expires = DateTime.Now.AddYears(1);
                newUser.Value = Guid.NewGuid().ToString();
                userId = newUser;
            }

            DSUserSettings dsSettings = DataUserSettings.GetUserSettingsByUserId(userId.Value);

            string today = DateTime.Now.DayOfWeek.ToString();

            DSUserSettings.UserSettingsRow[] settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ColorEven{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ColorEven;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ColorEven{0}", today),
                    GetSession().ColorEven, DateTime.Now, Request.UserAgent);

            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ColorOdd{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ColorOdd;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ColorOdd{0}", today),
                    GetSession().ColorOdd, DateTime.Now, Request.UserAgent);

            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ColorET{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ColorET;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ColorET{0}", today),
                    GetSession().ColorET, DateTime.Now, Request.UserAgent);

            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ColorHotComplaint{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ColorHotComplaint;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ColorHotComplaint{0}", today),
                    GetSession().ColorHotComplaint, DateTime.Now, Request.UserAgent);

            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ColorYesterday{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ColorYesterday;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ColorYesterday{0}", today),
                    GetSession().ColorYesterday, DateTime.Now, Request.UserAgent);

            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ColorRedelivered{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ColorRedelivered;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ColorRedelivered{0}", today),
                    GetSession().ColorRedelivered, DateTime.Now, Request.UserAgent);


            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'SortOrder{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().SortOrder;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("SortOrder{0}", today),
                    GetSession().SortOrder, DateTime.Now, Request.UserAgent);

            settingRows = dsSettings.UserSettings.Select(
                string.Format("Setting = 'ViewType{0}'", today)) as DSUserSettings.UserSettingsRow[];
            if (settingRows.Length > 0)
                settingRows[0].Value = GetSession().ViewType;
            else
                dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                    string.Format("ViewType{0}", today),
                    GetSession().ViewType, DateTime.Now, Request.UserAgent);


            if (GetSession().CurrentDistrict.Trim().Length > 0)
            {
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'CurrentDistrict{0}'", today)) as DSUserSettings.UserSettingsRow[];
                if (settingRows.Length > 0)
                {
                    settingRows[0].Value = GetSession().CurrentDistrict;
                }
                else
                {
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        string.Format("CurrentDistrict{0}", today),
                        GetSession().CurrentDistrict, DateTime.Now, Request.UserAgent);
                }
            }

            if (GetSession().SelectedDistricts.Trim().Length > 0)
            {
                settingRows = dsSettings.UserSettings.Select(
                    string.Format("Setting = 'SelectedDistricts{0}'", today)) as DSUserSettings.UserSettingsRow[];
                if (settingRows.Length > 0)
                {
                    settingRows[0].Value = GetSession().SelectedDistricts;
                }
                else
                {
                    dsSettings.UserSettings.AddUserSettingsRow(userId.Value,
                        string.Format("SelectedDistricts{0}", today),
                        GetSession().SelectedDistricts, DateTime.Now, Request.UserAgent);
                }
            }

            DataUserSettings.SaveUserSettings(dsSettings.UserSettings);
        }
        catch (Exception ex)
        {
            Email.ReportErrorToProgrammers("Dispatch Problem", "BasePage.SavePreferences() triggered the following exception: " + ex.ToString());
            _dispatchSession = new DispatchSession(); 
        }
    }
}
