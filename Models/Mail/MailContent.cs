/*
 * Mail Content
 * Author: Veronica Vu
 * Date: 9/2/2021
 * Purpose: Provides basic foundation for the emails: To, Subject, and Body parts of an email 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Mail
{
    // Content for out emails, doesn't support file attachments
    public class MailContent
    {
        public string To { get; set; }              
        public string Subject { get; set; }         
        public string Body { get; set; }    

    }
}
