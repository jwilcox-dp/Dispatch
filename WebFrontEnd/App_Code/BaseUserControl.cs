using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for BaseControl
/// </summary>
public class BaseUserControl : System.Web.UI.UserControl
{
    DispatchSession _dispatchSession; 
    string _partnerPubZone = string.Empty;
    string _partnerPubName = string.Empty; 


    public BaseUserControl()
    {
        _dispatchSession = null;

        if (ConfigurationSettings.AppSettings["PartnerPubName"] != null)
            _partnerPubName = ConfigurationSettings.AppSettings["PartnerPubName"];

        if (ConfigurationSettings.AppSettings["PartnerPubZone"] != null)
            _partnerPubZone = ConfigurationSettings.AppSettings["PartnerPubZone"]; 
    }

    public DispatchSession GetSession
    {
        get
        {
            return _dispatchSession; 
        }
    }

    public void SetSession(DispatchSession inSesssion)
    {
        _dispatchSession = inSesssion; 
    }

    public string PartnerPubZone
    {
        get { return _partnerPubZone; }
    }

    public string PartnerPubName
    {
        get { return _partnerPubName; }
    }

}
