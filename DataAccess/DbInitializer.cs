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
    public static class DbInitializer
    {
        public static void Initialize(TCMS_Context context)
        {
            context.Database.Migrate();
            context.SaveChanges();
        }
    }
}
