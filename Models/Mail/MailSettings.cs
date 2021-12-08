/*
 * Mail Settings 
 * Author: Veronica Vu 
 * Date: 9/3/2021
 * Purpose: Provides the settings need to send the email confirmation link 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Mail
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

    }
}
