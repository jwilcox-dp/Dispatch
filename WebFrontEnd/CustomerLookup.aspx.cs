using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

//using WcfSISServices;
using EmailService; 

public partial class CustomerLookup : BasePage
{
    string _searchType = string.Empty; 
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        if (Request.QueryString["AddressKey"] != null && Request.QueryString["AddressKey"].Trim().Length > 0)
        {
            _searchType = "Address"; 
            ParseAddressKey(Request.QueryString["AddressKey"].Replace("!", " "));
            btnSearch_Click(null, null); 
        }

        _searchType = Request.QueryString["Type"]; 
        if (_searchType == null || _searchType.Trim().Length == 0)
            _searchType = "Address"; 
        
        if (_searchType == "Address")
        {
            mvSearchType.SetActiveView(viewAddress); 
        }
        else if (_searchType == "Phone")
        {
            mvSearchType.SetActiveView(viewPhone); 
        }
        else 
        {
            mvSearchType.SetActiveView(viewAccount); 
        }



        ucDispatchHeader.SetTitle("Customer Lookup"); 
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        /*DTOHousehold household;
        DTOComplaintList complaintList;
        DTOMemoList memoList; 
        DTOTransactionStatus transactionStatus;

        EmailService.EmailServiceClient emailer = new EmailServiceClient(); 


        string
            areaCode, phonePrefix, phoneSuffix, accountNumber, streetName, streetType, apartment, cityCode, streetNumber, streetDirection, streetFraction;

        areaCode = string.Empty;
        phonePrefix = string.Empty;
        phoneSuffix = string.Empty;
        accountNumber = string.Empty;
        streetName = string.Empty;
        streetType = string.Empty;
        apartment = string.Empty;
        cityCode = string.Empty;
        streetNumber = string.Empty;
        streetDirection = string.Empty;
        streetFraction = string.Empty;

        lblError.Visible = false;

        if (_searchType == "Account")
        {
            accountNumber = tbAccountNumber.Text.Trim();

            emailer.SendEmail("tzehrbach@denverpost.com", "CustomerSearch@DenverPost.com", "Customer Search Performed",
                            string.Format("Account Number: {0} \n\r\n\r UserAgent: {1} \n\r UserHostAddress: {2} \n\r UserHostName: {3} ", 
                            accountNumber, 
                            Request.UserAgent, 
                            Request.UserHostAddress, 
                            Request.UserHostName), "bumble.dna.root.ad"); 


            transactionStatus = RequestClient.SearchSubscriberByAccount(accountNumber,
                out household, out complaintList, out memoList); 
        }
        else if (_searchType == "Address")
        {
            streetName = tbStreetName.Text.Trim().Replace(";", "");
            streetType = tbStreetType.Text.Trim().Replace(";", "");
            apartment = tbApartment.Text.Trim().Replace(";", "");
            cityCode = tbCityCode.Text.Trim().Replace(";", "");
            streetNumber = tbStreetNumber.Text.Trim().Replace(";", "");
            streetDirection = tbStreetDirection.Text.Trim().Replace(";", "");
            streetFraction = tbStreetFraction.Text.Trim().Replace(";", "");

            emailer.SendEmail("tzehrbach@denverpost.com", "CustomerSearch@DenverPost.com", "Customer Search Performed", 
                string.Format("Street Number: {0} \n\r Street Fraction:{1} \n\r Street Direction: {2} \n\r Street Name: {3} \n\r Street Type: {4} \n\r Apartment: {5} \n\r City Code: {6} \n\r\n\r UserAgent: {7} \n\r UserHostAddress: {8} \n\r UserHostName: {9}", 
                streetNumber, streetFraction, streetDirection, streetName, streetType, apartment, cityCode, 
                Request.UserAgent, 
                Request.UserHostAddress, 
                Request.UserHostName)
                , "bumble.dna.root.ad"); 


            transactionStatus = RequestClient.SearchSubscriberByAddress(streetNumber, streetFraction,
                streetDirection, streetName, streetType, apartment, cityCode,
                out household, out complaintList, out memoList); 


        }
        else
        {
            string phoneNumber = tbPhoneNumber.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
            areaCode = phoneNumber.Substring(0, 3);
            phonePrefix = phoneNumber.Substring(3, 3);
            phoneSuffix = phoneNumber.Substring(6, 4);

            emailer.SendEmail("tzehrbach@denverpost.com", "CustomerSearch@DenverPost.com", "Customer Search Performed",
                string.Format("Phone Number: ({0}) {1}-{2} \r\r UserAgent: {3} \r UserHostAddress: {4} \r UserHostName: {5}", areaCode, phonePrefix, phoneSuffix, 
                Request.UserAgent, 
                Request.UserHostAddress, 
                Request.UserHostName)
                , "bumble.dna.root.ad"); 

            transactionStatus = RequestClient.SearchSubscriberByPhone(areaCode, phonePrefix, phoneSuffix,
                out household, out complaintList, out memoList); 
        }

        if (!transactionStatus.IsSuccess)
        {
            lblError.Text = transactionStatus.ErrorDescription; 
            lblError.Visible = true;
            pnlCustomerInfo.Visible = false;
        }
        else 
        {
            List<DTOSubscription> filteredSubList = FilterSubscriptions(household);

            if (filteredSubList.Count == 0)
            {
                lblError.Text = "No Subscriber Found";
                lblError.Visible = true;
                pnlCustomerInfo.Visible = false;

                emailer.SendEmail("tzehrbach@denverpost.com", "CustomerSearch@DenverPost.com", "Customer Search Failed", "Subsequent Customer Search Failed", 
                    "bumble.dna.root.ad"); 

            }
            else
            {
                tbCusomterName.Text = household.HouseholdName;

                tbDeliveryInstructions.Text = household.DeliveryInstruction;
                tbAddress.Text = household.DisplayAddressWithoutName;
                tbBuildingType.Text = household.BuildingTypeDesc;
                tbPhoneNumber2.Text = household.DisplayPhoneNumber;
                int phoneNumberLength = tbPhoneNumber2.Text.Length;


                emailer.SendEmail("tzehrbach@denverpost.com", "CustomerSearch@DenverPost.com", "Customer Search Succeeded", 
                    "Phone Number For Customer: " + household.DisplayPhoneNumber,
                    "bumble.dna.root.ad"); 


                if (tbPhoneNumber2.Text[tbPhoneNumber2.Text.Length - 1] == '0')
                    tbPhoneNumber2.Text = tbPhoneNumber2.Text.Remove(phoneNumberLength - 1) + '5';
                else
                    tbPhoneNumber2.Text = tbPhoneNumber2.Text.Remove(phoneNumberLength - 1) + '0';

                gvSubscriptions.DataSource = filteredSubList;
                gvSubscriptions.DataBind();

                gvComplaints.DataSource = complaintList.List;
                gvComplaints.DataBind();

                gvMemos.DataSource = memoList.List;
                gvMemos.DataBind();

                pnlCustomerInfo.Visible = true;


            }
        }*/
    }


    protected string ComplaintAdditionalInfo(Object inObject)
    {
        /*DTOComplaint complaint = inObject as DTOComplaint;

        if (complaint == null)
            return string.Empty;

        string toReturn = string.Empty;

        toReturn = complaint.AdditionalText; 

        if (complaint.DeleteDate != DateTime.MinValue)
        {
            toReturn = toReturn + "<br>" + "This complaint was deleted on " + complaint.DeleteDate.ToShortDateString(); 
        }
        return toReturn; */ return null;
    }

    protected string ComplaintRedeliveryInfo(Object inObject)
    {
        /*DTOComplaint complaint = inObject as DTOComplaint;

        if (complaint == null)
            return string.Empty;

        string toReturn = string.Empty;


        if (complaint.RedeliverFlag == "T" || complaint.RedeliverFlag == "Y")
        {
            toReturn = "Redelivery Required"; 
        }

        if (complaint.RedeliverDate != DateTime.MinValue)
        {
            toReturn =  string.Format("Redelivered at {0}/{1} {2}:{3}", complaint.RedeliverDate.Month, 
                complaint.RedeliverDate.Day, complaint.RedeliverDate.Hour, complaint.RedeliverDate.Minute); 
            return toReturn; 
        }
        else if (complaint.RedeliverFlag == "T" || complaint.RedeliverFlag == "Y")
        {
            return "Redelivery Required";
        }
        else
        {
            return "Redelivery not Required"; 
        }*/ return null;
    }
    protected string MemoInfo(Object inObject)
    {
        /*DTOMemo memo = inObject as DTOMemo;

        if (memo == null)
            return string.Empty;

        string toReturn = string.Empty;

        toReturn = memo.MemoText;

        if (memo.DeleteDate != DateTime.MinValue)
        {
            toReturn = toReturn + "<br>" + "This memo was deleted on " + memo.DeleteDate.ToShortDateString();
        }
        return toReturn;*/ return null;
    }

    protected string MemoRedeliveryInfo(Object inObject)
    {
        /*DTOMemo memo = inObject as DTOMemo;

        if (memo == null)
            return string.Empty;

        string toReturn = string.Empty;

        if (memo.RedeliverDate != DateTime.MinValue)
        {
            toReturn = string.Format("Dispatched at {0}/{1} {2}:{3}", memo.RedeliverDate.Month,
                memo.RedeliverDate.Day, memo.RedeliverDate.Hour, memo.RedeliverDate.Minute);

            return toReturn; 

        }
        else if (memo.DispatchFlag == "T" || memo.DispatchFlag == "Y")
        {
            return "Dispatch Required";
        }
        else
        {
            return "Dispatch Not Required";
        }*/ return null;
    }
    protected void btnAddressSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerLookup.aspx?Type=Address"); 
    }
    protected void btnPhoneSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerLookup.aspx?Type=Phone"); 
    }
    protected void btnAccount_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerLookup.aspx?Type=Account");
    }

    /*private List<DTOSubscription> FilterSubscriptions(DTOHousehold inHousehold)
    {
        List<DTOSubscription> toReturn = new List<DTOSubscription>(); 

        foreach (DTOSubscription subInfo in inHousehold.SubscriptionList.List)
        {
            if (PartnerPubZone == null || PartnerPubZone.Trim().Length == 0 || 
                subInfo.RouteZoneCode.Trim() == PartnerPubZone.Trim())
                toReturn.Add(subInfo); 
        }

        return toReturn; 
    }*/

    private void ParseAddressKey(string inAddressKey)
    {
        inAddressKey = inAddressKey + "                          "; 
        char[] whiteSpace = { ' ' };
        char[] zero = { '0' };
        tbCityCode.Text = inAddressKey.Substring(0, 4).TrimEnd(whiteSpace);
        tbStreetName.Text = inAddressKey.Substring(4, 16).TrimEnd(whiteSpace);
        tbStreetType.Text = inAddressKey.Substring(20, 4).TrimEnd(whiteSpace);
        tbStreetDirection.Text = inAddressKey.Substring(24, 2).TrimEnd(whiteSpace);
        tbStreetNumber.Text = inAddressKey.Substring(26, 5).TrimStart(zero);
        tbStreetFraction.Text = inAddressKey.Substring(31, 1).TrimEnd(whiteSpace);
        tbApartment.Text = inAddressKey.Substring(32, 6).TrimEnd(whiteSpace);
    }
}
