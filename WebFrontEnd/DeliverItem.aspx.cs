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

public partial class DeliverItem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.Count == 1)
        {
            string recordId = Request.QueryString["Id"]; 

            sdsConfirmDelivery.UpdateParameters[0].DefaultValue = recordId; 
            sdsConfirmDelivery.Update(); 
        }
        Response.Redirect("~/ToDispatch.aspx"); 
    }
}
