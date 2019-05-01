using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{
    public class Email
    {
        public string SenderEmailAddress { get; set; }
        public string ReplayEmailAddress { get; set; }
        public string SenderPassword { get; set; }
        public string SenderHostName { get; set; }
        public int SenderPort { get; set; }
        public bool EnableSSL { get; set; }
        private Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

        public Email()
        {

        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="senderEmailAddress"></param>
        /// <param name="senderPassword"></param>
        /// <param name="senderHostName"></param>
        /// <param name="senderPort"></param>
        public Email(string senderEmailAddress, string senderPassword, string senderHostName, int senderPort, bool enableSSL, string replayEmailAddress)
        {
            try
            {
                this.SenderEmailAddress = senderEmailAddress;
                this.ReplayEmailAddress = replayEmailAddress;
                this.SenderPassword = senderPassword;
                this.SenderHostName = senderHostName;
                this.SenderPort = senderPort;
                this.EnableSSL = enableSSL;
            }
            catch (Exception)
            {
                //BusinessClass.LogException(ex, ErrorLevel.ClassLevel);
            }
        }


        /// <summary>
        /// Validate email address
        /// </summary>
        /// <param name="EmailAddress"></param>
        /// <returns></returns>
        public bool IsValidEmailAddress(string EmailAddress)
        {
            try
            {
                return regex.IsMatch(EmailAddress) && !EmailAddress.EndsWith(".");
            }
            catch (Exception)
            {
                //BusinessClass.LogException(ex, ErrorLevel.ClassLevel);
                return false;
            }
        }

        /// <summary>
        /// Send email message to single recepient with some attachements
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="EmailTO"></param>
        /// <param name="Priority"></param>
        /// <param name="AttchementPath"></param>
        /// <returns></returns>
        public bool SendEmail(string DisplayName, string Subject, string Body, string LinkedResources, string EmailTO, MailPriority Priority = MailPriority.Normal, List<string> AttachmentPath = null)
        {
            try
            {
                List<string> ListEmailTO = new List<string>();
                if (EmailTO.Contains(','))
                {
                    ListEmailTO = EmailTO.Split(',').ToList();
                }
                else
                {
                    ListEmailTO.Add(EmailTO);
                }
                return SendEmailMessage(DisplayName, Subject, Body, LinkedResources, ListEmailTO, null, null, Priority, AttachmentPath);
            }
            catch (Exception)
            {
                //BusinessClass.LogException(ex, ErrorLevel.ClassLevel);
                return false;
            }
        }

        /// <summary>
        /// Method to send Email
        /// </summary>
        /// <param name="objMail"></param>
        /// <returns></returns>
        private bool SendEmailMessage(string DisplayName, string Subject, string Body, string LinkedResources, List<string> ListEmailTO, List<string> ListEmailCC = null, List<string> ListEmailBCC = null, MailPriority Priority = MailPriority.Normal, List<string> AttachmentPath = null)
        {
            SmtpClient SMTPClientObj = new SmtpClient();
            SMTPClientObj.UseDefaultCredentials = false;
            System.Net.Mail.MailMessage objMail = new System.Net.Mail.MailMessage();
            try
            {
                //Set from address
                objMail.From = new MailAddress(this.SenderEmailAddress, DisplayName);
                // Set to addresses
                if (ListEmailTO.Count > 0)
                {
                    foreach (string emailTO in ListEmailTO)
                    {
                        objMail.To.Add(new MailAddress(emailTO));
                        if (this.ReplayEmailAddress != null && this.ReplayEmailAddress != "")
                        {
                            objMail.ReplyToList.Add(new MailAddress(this.ReplayEmailAddress));
                        }
                    }
                }

                // Set CC addresses
                if (ListEmailCC != null)
                {
                    if (ListEmailCC.Count > 0)
                    {
                        foreach (string emailCC in ListEmailCC)
                        {
                            objMail.CC.Add(new MailAddress(emailCC));
                        }
                    }
                }

                // Set BCC addresses
                if (ListEmailBCC != null)
                {
                    if (ListEmailBCC.Count > 0)
                    {
                        foreach (string emailBCC in ListEmailBCC)
                        {
                            objMail.Bcc.Add(new MailAddress(emailBCC));
                        }
                    }
                }

                // Set the content
                // Set subject
                objMail.Subject = Subject;
                objMail.IsBodyHtml = true;
                // Set body/message
                // objMail.Body = Body;
                AlternateView htmlMail = null;
                htmlMail = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);

                // Set priority default is "Normal"
                objMail.Priority = Priority;

                if (LinkedResources != null)
                {
                    List<string> _LinkedResources = LinkedResources.Split(',').ToList();
                    foreach (string Cid in _LinkedResources)
                    {
                        if (!string.IsNullOrEmpty(Cid))
                        {
                            string[] _Cid = Cid.Split('^').ToArray();
                            string path = System.Web.HttpContext.Current.Server.MapPath(_Cid[0]);
                            string contentid = _Cid[1];

                            LinkedResource myimage = new LinkedResource(path);
                            myimage.ContentId = contentid;

                            // Create HTML view
                            // Set ContentId property. Value of ContentId property must be the same as
                            // the src attribute of image tag in email body. 
                            htmlMail.LinkedResources.Add(myimage);
                        }
                    }
                }

                objMail.AlternateViews.Add(htmlMail);

                // Set priority default is "Normal"
                objMail.Priority = Priority;

                // Set attachement 
                if (AttachmentPath != null)
                {
                    if (AttachmentPath.Count > 0)
                    {
                        foreach (string path in AttachmentPath)
                        {
                            objMail.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath(path)));
                        }
                    }
                }

                SMTPClientObj.Credentials = new System.Net.NetworkCredential(this.SenderEmailAddress, this.SenderPassword);
                SMTPClientObj.Host = this.SenderHostName;
                SMTPClientObj.Port = this.SenderPort;
                SMTPClientObj.EnableSsl = this.EnableSSL;
                SMTPClientObj.DeliveryMethod = SmtpDeliveryMethod.Network;
                SMTPClientObj.Send(objMail);
         
                return true;
            }
            catch (Exception ex)
            {
                string fileLoc = @"C:\Websites\ePROM_DEV\error.txt";

                FileStream fs = null;

                if (!File.Exists(fileLoc))
                {
                    using (fs = File.Create(fileLoc))
                    {
                    }
                }

                if (File.Exists(fileLoc))
                {
                    using (StreamWriter sw = new StreamWriter(fileLoc))
                    {
                        sw.Write("Message: " + ex.Message);
                        sw.Write("StackTrace: " + ex.StackTrace);
                    }
                }

                string msg = ex.Message;
                return false;
            }
            finally
            {
                SMTPClientObj.Dispose();
                objMail.Dispose();
            }
        }
    }
}