using Models;
using System.Data.Entity;

namespace DataAccess
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<TCMS_Context>
    {
        protected override void Seed(TCMS_Context context)
        {
            context.SaveChanges();
        }
    }
}