using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwtwebapiwithswagger.Models
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {

        }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Userinfo> Userinfo { get; set; }

    }
}
