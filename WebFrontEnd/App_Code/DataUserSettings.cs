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
/// Summary description for DataUserSettings
/// </summary>
public class DataUserSettings
{
    public DataUserSettings()
    {
    }

    static public DSUserSettings GetUserSettingsByUserId(string inId)
    {
        DSUserSettings toReturn = new DSUserSettings();
        DSUserSettingsTableAdapters.UserSettingsTableAdapter forLoad = new DSUserSettingsTableAdapters.UserSettingsTableAdapter();
        forLoad.GetUserSettingsByRequestor(toReturn.UserSettings, inId);
        return toReturn; 
    }


    static public void SaveUserSettings(DSUserSettings.UserSettingsDataTable inToSave)
    {
        DSUserSettings forUpdate = new DSUserSettings(); 
        forUpdate.UserSettingWriter.Merge(inToSave); 

        DSUserSettingsTableAdapters.UserSettingWriterTableAdapter forSave = new DSUserSettingsTableAdapters.UserSettingWriterTableAdapter();
        forSave.Update(forUpdate.UserSettingWriter); 
    }
}
