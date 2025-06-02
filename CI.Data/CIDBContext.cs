using CI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Data
{
    public class CIDBContext : IdentityDbContext<ApplicationUser>
    {
        public CIDBContext(DbContextOptions<CIDBContext> options) 
            : base(options) { }

        public IDbConnection CreateConnection() => new SqlConnection(Database.GetConnectionString());
        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CityModel> Cities { get; set; }
        public DbSet<CountryModel> Countries { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<MissionModel> Missions { get; set; }
        public DbSet<MissionThemeModel> MissionThemes { get; set; }
        public DbSet<MissionSkillModel> MissionSkills { get; set; }
        public DbSet<FavMissionModel> FavMissions { get; set;}
        public DbSet<ExceptionModel> Exceptions { get; set;}
        public DbSet<MissionRatingModel> MissionRatings { get; set;}
        public DbSet<VolunteerGoalModel> VolunteerGoal { get; set; }
        public DbSet<VolunteerTimeModel> volunteerTimes { get; set; }
        public DbSet<MissionVolunteeringModel> MissionVolunteering { get; set; }
    }
}
