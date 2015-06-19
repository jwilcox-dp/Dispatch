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

public partial class GreeleyDeliveryReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            gvGreeleyDispatches.PageSize = Int32.Parse(ddlPageLength.SelectedValue);

            sdsGreeleyDispatches.SelectParameters[0].DefaultValue =
                DateTime.Now.Year.ToString("0000") +
                DateTime.Now.Month.ToString("00") +
                DateTime.Now.Day.ToString("00"); 
            gvGreeleyDispatches.DataBind(); 
        }
    }

    protected void calendar_SelectionChanged(object sender, EventArgs e)
    {
        gvGreeleyDispatches.PageSize = Int32.Parse(ddlPageLength.SelectedValue);

        sdsGreeleyDispatches.SelectParameters[0].DefaultValue =
            calendar.SelectedDate.Year +
            calendar.SelectedDate.Month.ToString("00") +
            calendar.SelectedDate.Day.ToString("00");
        gvGreeleyDispatches.DataBind(); 
    }
}
