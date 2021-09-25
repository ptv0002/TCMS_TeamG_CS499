using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;
using System.Data.SqlClient;
using Models;

namespace TCMS_Web.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<Company> companies = new List<Company>();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            con.ConnectionString = "Data Source= LAPTOP-C9LT3CRH; Initial Catalog=TCMS_DB;Integrated Security=True;MultipleActiveResultSets=True";
        }

        private void FetchData()
        {
            if (companies.Count > 0)
            {
                companies.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT TOP (1000) [ID],[Name],[Status],[Address],[City],[State],[Zip],[ContactPerson] FROM [TCMS_DB].[dbo].[Company]";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    companies.Add(new Company() { ID = (int)dr["ID"], Name = (string) dr["Name"], Status = (bool) dr["Status"], Address = (string) dr["Address"], City = (string) dr["City"], State = (string) dr["State"], Zip = (int) dr["Zip"], ContactPerson = (string) dr["ContactPerson"]});
                }
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Index()
        {
            FetchData();
            return View(companies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
