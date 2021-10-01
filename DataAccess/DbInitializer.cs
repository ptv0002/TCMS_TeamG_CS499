using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DbInitializer
    {
        private readonly UserManager<Employee> _userManager;
        public DbInitializer(UserManager<Employee> userManager)
        {
            _userManager = userManager;
        }
        public static void Initialize(TCMS_Context context)
        {
            context.Database.Migrate();
            Employee user = new Employee
            {
                FirstName = "Admin",
                LastName = "Test",
                UserName = "admin"
            };
            _userManager.CreateAsync(user, "admin1234");
            context.SaveChanges();
        }
    }
}
