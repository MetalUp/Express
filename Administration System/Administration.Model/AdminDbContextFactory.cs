using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Model; 

public class AdminDbContextFactory : IDesignTimeDbContextFactory<AdminDbContext>
{
    public AdminDbContext CreateDbContext(string[] args)
    {
        var db = new AdminDbContext("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ILEAdmin;Integrated Security=True;MultipleActiveResultSets=True");
        //db.Create();
        return db;
    }
}