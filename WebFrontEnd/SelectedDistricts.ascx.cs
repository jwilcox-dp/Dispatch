using System;
using System.Collections;
using System.Collections.Generic; 
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

public partial class SelectedDistricts : BaseUserControl
{
    ISavePreferences _parent = null;

    public void SetParent(ISavePreferences inParent)
    {
        _parent = inParent; 
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            UpdateList(); 
    }

    private void UpdateList()
    {
        string selectedDistricts = GetSession.SelectedDistricts;

        cblCurrentDistricts.Items.Clear();

        if (selectedDistricts != null && selectedDistricts.Trim().Length > 0)
        {
            string[] districtList = selectedDistricts.Split(new char[] { ',' });

            foreach (string district in districtList)
            {
                if (district.Trim().Length > 0)
                    cblCurrentDistricts.Items.Add(new ListItem(district, district));
            }
        }

        if (cblCurrentDistricts.Items.Count > 0)
        {
            Visible = true;
        }
        else
        {
            Visible = false;
        }
    }

    protected void btnGoToDistrict_Click(object sender, EventArgs e)
    {
        string selectedDistricts = string.Empty; 
        foreach (ListItem item in cblCurrentDistricts.Items)
        {
            if (item.Selected)
            {
                if (selectedDistricts.Length == 0)
                    selectedDistricts = item.Value; 
                else
                    selectedDistricts = selectedDistricts + "," + item.Value; 
            }
        }
        if (selectedDistricts.Length > 0)
        {
            GetSession.CurrentDistrict = selectedDistricts; 

            if (_parent != null)
                _parent.SavePreferences(); 

            Response.Redirect("~/ToDispatch.aspx"); 
        }
    }

    protected void btnRemoveDistrict_Click(object sender, EventArgs e)
    {
        List<string> districtsToRemove = new List<string>();


        // Create a list of districts to remove from the list of Districts and from the 
        // user's saved settings.  Remember count backwards in the item list 
        // because you can't delete an item and then go to the next item in the list, you can 
        // only go to the previous item in the list.
        for (int itemIndex = cblCurrentDistricts.Items.Count -1; itemIndex >= 0; itemIndex--)
        {
            ListItem currentItem = cblCurrentDistricts.Items[itemIndex];
            if (currentItem.Selected)
            {
                districtsToRemove.Add(currentItem.Value);
                cblCurrentDistricts.Items.Remove(currentItem);
                GetSession.RemoveDistrictFromSelection(currentItem.Value); 
            }
        }

        _parent.SavePreferences(); 

    }
}
