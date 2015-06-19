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

using System.Drawing; 

/// <summary>
/// Summary description for DispatchSession
/// </summary>
public class DispatchSession
{
    string _colorEven, _colorOdd, _colorET, _colorHotComplaint, _colorYesterday, _colorRedelivered;
    string _sortOrder, _viewType, _selectedDistricts, _currentDistrict, _currentZone;

    bool _changesMade = false;

    public String ColorEven
    {
        get { return _colorEven; }
        set {
            if (_colorEven != value)
            {
                _changesMade = true;
                _colorEven = value;
            }
        }
    }

    public String ColorOdd
    {
        get { return _colorOdd; }
        set {
            if (_colorOdd != value)
            {
                _changesMade = true; 
                _colorOdd = value;
            }
        }
    }

    public String ColorET
    {
        get { return _colorET; }
        set {
            if (_colorET != value)
            {
                _changesMade = true;
                _colorET = value;
            }
        }
    }

    public String ColorHotComplaint
    {
        get { return _colorHotComplaint; }
        set
        {
            if (_colorHotComplaint != value)
            {
                _changesMade = true; 
                _colorHotComplaint = value;
            }
        }
    }

    public String ColorYesterday
    {
        get { return _colorYesterday; }
        set {
            if (_colorYesterday != value)
            {
                _changesMade = true; 
                _colorYesterday = value;
            }
        }
    }

    public String ColorRedelivered
    {
        get { return _colorRedelivered; }
        set
        {
            if (_colorRedelivered != value)
            {
                _changesMade = true; 
                _colorRedelivered = value;
            }
        }
    }

    public string SortOrder
    {
        get { return _sortOrder; }
        set {
            if (_sortOrder != value)
            {
                _changesMade = true; 
                _sortOrder = value;
            }
        }
    }

    public string ViewType
    {
        get { return _viewType; }
        set {
            if (_viewType != value)
            {
                _changesMade = true; 
                _viewType = value;
            }
        }
    }

    public string SelectedDistricts
    {
        get { return _selectedDistricts; }
        set
        {
            if (_selectedDistricts != value)
            {
                _changesMade = true;
                _selectedDistricts = value;
            }
        }
    }

    public void RemoveDistrictFromSelection(string inToRemove)
    {
        string[] districtList = _selectedDistricts.Split(new char[]{','});

        _selectedDistricts = ""; 

        foreach (string district in districtList)
        {
            if (district != inToRemove)
            {
                if (_selectedDistricts.Length == 0)
                    _selectedDistricts = district;
                else
                    _selectedDistricts = _selectedDistricts + "," + district;
            }
            else
            {
                _changesMade = true; 
            }
        }
    }

    public string CurrentDistrict
    {
        get { return _currentDistrict; }
        set {
            if (_currentDistrict != value)
            {
                _changesMade = true;
                _currentDistrict = value;

                if (!SelectedDistricts.Contains(_currentDistrict))
                {
                    if (SelectedDistricts.Trim().Length == 0)
                        SelectedDistricts = _currentDistrict;
                    else
                        SelectedDistricts = SelectedDistricts + "," + _currentDistrict;
                }
            }
        }
    }

    public string CurrentZone
    {
        get { return _currentZone; }
        set
        {
            if (_currentZone != value)
            {
                _changesMade = true;
                _currentZone = value;
            }
        }
    }


    public DispatchSession()
    {
        ColorEven = "ColorWhite";
        ColorOdd = "ColorBlue"; 
        ColorET = "ColorYellow";
        ColorHotComplaint = "ColorRed";
        ColorYesterday = "ColorOrange";
        ColorRedelivered = "ColorGreen"; 
        SortOrder = "RouteCode";
        ViewType = "All";
        SelectedDistricts = "";
        CurrentDistrict = "";
        CurrentZone = ""; 
    }

    public void ClearChangesMade()
    {
        _changesMade = false; 
    }

    public bool GetChangesMade()
    {
        return _changesMade; 
    }
}
