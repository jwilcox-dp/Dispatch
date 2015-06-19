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

public interface ISavePreferences
{
    void SavePreferences();

    DispatchSession GetSession(); 
}

public interface ISetViewType
{
    void SetViewType(string inViewType);

    DispatchSession GetSession(); 
}

