using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace TCMS_Web.Controllers
{
    public class VehicleController : Controller
    {
        private readonly TCMS_Context veh;

        public VehicleController (TCMS_Context vehicle)
        {
            veh = vehicle;
        }

        public IActionResult Index()
        {
            return View(veh.Vehicles.ToList());
        }
    }
}
