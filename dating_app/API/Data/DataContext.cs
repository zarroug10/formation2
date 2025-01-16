using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;// namespace is used to make all the file visible to each other

public class DataContext(DbContextOptions options) : DbContext(options) //class for datacontext the uses the dependency injection to use the dbCOntextoptions 
{
    public  DbSet <AppUser> Users {get; set;} // decalting a database table using the debset with return type of AppUser and a table name of Users
}
