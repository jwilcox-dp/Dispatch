using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Net.Mail;
using System.Collections.Generic;

    /// <summary>
    /// Summary description for Email
    /// </summary>
    public class Email
    {
        public Email()
        {
        }

        /// <summary>
        /// Send an email to a list of people.  Each item in the "inToList" either refers to
        /// either an actual email address or an entry in the Config file such as: 
        ///     <add key="SISMailTo" value="tmccarthy;bstone;bwhitaker;csvec;cchristian;charmon;dwebb;bgitt;jborland;jmdickson;khart;ldascalos;rromero;sbolke;tdickerson;vshaffer"/>
        /// If an "inToList" entry refers to an entry in the Web.Config or App.Config file, then 
        /// the users in that list are added to the distribution list.
        /// 
        /// If an entry is found in the Config file for the given "inFrom" string, 
        /// that entry is used as the "From" address, otherwise the actual value of the 
        /// "inFrom" string is used as the "From" address.
        /// 
        /// If an entry is found in the Config file for the given "inSmtpServer" string, 
        /// that entry is used as the Smtp Server name, otherwise the actual value of the 
        /// "inSmtpServer" string is used as the Smtp Server name. 
        /// 
        /// 
        /// </summary>
        /// <param name="inToList">List of: .Config keys and/or specific Email Addresses</param>
        /// <param name="inFrom">Either key name of a .Config file entry or an actual email address of the "From" email address.</param>
        /// <param name="inSubject">Subject for the email message.</param>
        /// <param name="inBody">Body for the email message.</param>
        /// <param name="inSmtpServer">Either a key name of a .Config file entry or an actual server name of the mail server.</param>
        public static void SendEmail(
            List<string> inToList,
            string inFrom,
            string inSubject,
            string inBody,
            string inSmtpServer)
        {
            try
            {
                if (ConfigurationManager.AppSettings[inSmtpServer] != null)
                    inSmtpServer = ConfigurationManager.AppSettings[inSmtpServer];

                if (ConfigurationManager.AppSettings[inFrom] != null)
                    inFrom = ConfigurationManager.AppSettings[inFrom];

                SmtpClient mailer = new SmtpClient(inSmtpServer);
                MailMessage toSend = new MailMessage();

                toSend.From = new MailAddress(inFrom);

                AddToAddresses(inToList, toSend);
                toSend.Subject = inSubject;
                toSend.Body = inBody;
                mailer.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailer.SendCompleted += new SendCompletedEventHandler(mailer_SendCompleted);
                mailer.SendAsync(toSend, toSend);
                //                mailer.Send(toSend);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void mailer_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string result = string.Empty;
            if (e.Error != null)
                result = "Error";
            else if (e.Cancelled)
                result = "Cancelled";
            else
                result = "Success";


        }


        /// <summary>
        /// Send an email to a single "entity".  The inTo value can be one of the following
        /// 3 items: 
        ///     1. Complete Email Address (i.e. tzehrbach@denvernewspaperagency.com)
        ///     2. Name component of an email address at DNA (i.e. tzehrbach - this method will automatically 
        ///         append "denvernewspaperagency.com" to the end of the address)
        ///     3. Entry in the Config file such as: 
        ///         <add key="SISMailTo" value="tmccarthy;bstone;bwhitaker;csvec;cchristian;charmon;dwebb;bgitt;jborland;jmdickson;khart;ldascalos;rromero;sbolke;tdickerson;vshaffer"/>
        /// 
        /// If an entry is found in the Config file for the given "inFrom" string, 
        /// that entry is used as the "From" address, otherwise the actual value of the 
        /// "inFrom" string is used as the "From" address.
        /// 
        /// If an entry is found in the Config file for the given "inSmtpServer" string, 
        /// that entry is used as the Smtp Server name, otherwise the actual value of the 
        /// "inSmtpServer" string is used as the Smtp Server name. 
        /// 
        /// 
        /// </summary>
        /// <param name="inToList">Email address or .Config key for whom to send the email.</param>
        /// <param name="inFrom">Either key name of a .Config file entry or an actual email address of the "From" email address.</param>
        /// <param name="inSubject">Subject for the email message.</param>
        /// <param name="inBody">Body for the email message.</param>
        /// <param name="inSmtpServer">Either a key name of a .Config file entry or an actual server name of the mail server.</param>
        public static void SendEmail(
            string inTo,
            string inFrom,
            string inSubject,
            string inBody,
            string inSmtpServer)
        {
            List<string> toList = new List<string>();
            toList.Add(inTo);
            SendEmail(toList, inFrom, inSubject, inBody, inSmtpServer);
        }


        /// <summary>
        /// Send an email to support staff (programmers) about a problem found in SIS.
        /// </summary>
        /// <param name="inSubject">Subject of Email</param>
        /// <param name="inBody">Body of Email</param>
        public static void ReportErrorToProgrammers(
            string inSubject,
            string inBody)
        {
            Email.SendEmail("MailErrorsTo", "MailFrom", inSubject, inBody, "SMTPMailServer");
        }



        /// <summary>
        /// For each email address in the list, create an MailAddress and add that
        /// MailAddress to the mail message's To list.  
        /// 
        /// Each email address can represent 1 of 2 types of email address specifiers:
        /// 1. An actual email address, such as user@denvernewspaperagency.com
        /// 2. An entry in a web.config or app.config file that contains a list of 
        ///    email addresses.
        /// </summary>
        /// <param name="inToList">A list of "To" email addresses.  Each item in the list can be an actual email address or the Key Name of
        /// an entry in the App.Config or Web.Config file.</param>
        /// <param name="toSend">Mail Message for which each email address will be added to the "To" list.</param>
        private static void AddToAddresses(List<string> inToList, MailMessage toSend)
        {
            foreach (string toSpecifier in inToList)
            {
                string mailTo;
                // If the entry in the "To" list contains a '@' sign, assume 
                // the entry is for an actual email address
                if (toSpecifier.Contains("@"))
                {
                    mailTo = toSpecifier;
                }
                // If the entry in the "To" list does NOT contain a '@' sign, see if 
                // the entry is for a key the Web.Config file.
                else
                {
                    mailTo = ConfigurationManager.AppSettings[toSpecifier];
                    // If no entry found in config file, set the email 
                    // address to the actual entry in the recipient list
                    if (mailTo == null)
                        mailTo = toSpecifier;
                }

                if (mailTo != null && mailTo.Length > 0)
                {
                    // mailTo can contain multiple email addresses, split the contents into 
                    // individual components (email addresses)...
                    string[] mailAddresses = mailTo.Split(new char[] { ';', ',' });
                    foreach (string address in mailAddresses)
                    {
                        string emailAddress = address;
                        if (emailAddress.Trim().Length == 0)
                            continue;
                        // If an address does not contain an '@' character, the entry is 
                        // shorthand for just the addressee, append "@denvernewspaperagency.com" to the 
                        // end of the address.
                        if (!emailAddress.Contains("@"))
                            emailAddress += "@denvernewspaperagency.com";

                        // Add each individual email address to the distribution list.
                        toSend.To.Add(new MailAddress(emailAddress));
                    }
                }
            }
        }
    }
