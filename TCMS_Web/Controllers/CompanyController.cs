using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Controllers
{
    public class Company : Controller
    {
        public Company (Company com)
        {
            
        }
        public IActionResult Index()
        {
            string poop = "This is a webpage";
            return View(poop);
        }
    }
}
