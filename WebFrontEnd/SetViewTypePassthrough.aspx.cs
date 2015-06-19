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

public partial class SetViewTypePassthrough : BasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        if (Request.QueryString["ViewType"] != null &&
            Request.QueryString["ViewType"].Trim().Length > 0)
        {
            GetSession().ViewType = Request.QueryString["ViewType"];

            SavePreferences();

            Response.Redirect("ToDispatch.aspx"); 

//            Server.Transfer("ToDispatch.aspx"); 
        }
    }
}
