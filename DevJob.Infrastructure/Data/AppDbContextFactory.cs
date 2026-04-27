using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Infrastructure.Data
{
    public class AppDbContextFactory
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // اكتبي هنا الـ Connection String الجديدة بإيدك (Hardcoded) للتجربة فقط
            optionsBuilder.UseSqlServer("Server=db48017.databaseasp.net; Database=db48017; User Id=db48017; Password=Jm4=b8@G?aZ7; Encrypt=False; MultipleActiveResultSets=True;  ");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
