using AutoMapper;
using CI.Models;
using CI.Utility;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CI.BusinessLogic.Repository.Interface;
using CI.Data;
using Microsoft.EntityFrameworkCore;

namespace CI.BusinessLogic.Repository
{
    public class AdminRepository: IAdminRepository 
    {
        private readonly CIDBContext _cIDB;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AdminRepository(CIDBContext cIDB, IMapper mapper, UserManager<ApplicationUser> userManager,
            IConfiguration config)
        {
            _cIDB = cIDB;
            _mapper = mapper;
            _userManager = userManager;
            _config = config;
        }

        public async Task<UserVM> UserData(int? id)
        {
            UserVM uservm = new UserVM();
            if (id != null)
            {
                var query = "select * from UserMaster where UserId = @Id";
                //uservm.UserRequest = _mapper.Map<UserRequestModel>(_cIDB.Users.Find(Convert.ToInt32(id)));//using Entity Framework
                using(var connection = _cIDB.CreateConnection())
                {
                    uservm.UserRequest = await connection.QueryFirstOrDefaultAsync<UserRequestModel>(query, new { id });//Parameterized 
                }
                uservm.CityList = CityList(uservm.UserRequest.CountryId); 
            }
            else
            {
                uservm.CityList = Enumerable.Empty<SelectListItem>();
            }
            uservm.DepartmentList = _cIDB.Departments.Select(i => new SelectListItem
            {
                Text = i.DepartmentName.ToString().Trim(),
                Value = i.DepartmentId.ToString()
            });
            uservm.CountryList = CountryList();

            return uservm;
        }

        public async Task<IEnumerable<SelectListItem>> BindCity(int? Id)
        {
             return CityList(Id);
        }

        public IEnumerable<SelectListItem> CityList(int? Id)
        {
            return _cIDB.Cities.Where(x => x.CountryId == Id).Select(i => new SelectListItem
            {
                Text = i.CityName.ToString().Trim(),
                Value = i.CityId.ToString()
            });
        }

        public IEnumerable<SelectListItem> CountryList()
        {
            return _cIDB.Countries.Select(i => new SelectListItem
            {
                Text = i.CountryName.ToString().Trim(),
                Value = i.CountryId.ToString()
            });
        }


        public async Task<UserRequestModel> PostUserData(UserRequestModel userData, int? id)
        {
            if(id != null)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    ApplicationUser user1 = await _userManager.FindByEmailAsync(userData.Email);
                    user1.FirstName = userData.FirstName;
                    user1.LastName = userData.LastName;
                    await _userManager.UpdateAsync(user1);

                    UserModel user = new UserModel();
                    user = _mapper.Map<UserModel>(userData);
                    user.UserId = Convert.ToInt32(id);
                    user.ModifiedBy = "Admin";
                    user.ModifiedDate = DateTime.Now;
                    _cIDB.Users.Update(user);
                    _cIDB.SaveChanges();
                    scope.Complete();
                }
            }
            else
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user1 = new ApplicationUser()
                    {
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        Email = userData.Email,
                        UserName = userData.Email
                    };
                    await _userManager.CreateAsync(user1, userData.Password);
                    _ = await _userManager.AddToRoleAsync(user1, "User");

                    //Using Entity Framework
                    //UserModel user = new UserModel();
                    //user = _mapper.Map<UserModel>(userData);
                    //user.CreatedBy = "Admin";
                    //user.CreatedDate = DateTime.Now;
                    //await _cIDB.Users.AddAsync(user);
                    //await _cIDB.SaveChangesAsync();

                    //Using Dapper 
                    //var query = "INSERT INTO UserMaster (FirstName, LastName, Email, EmployeeId, " +
                    //    "DepartmentId, CountryId, CityId, ProfileSummary, Status) VALUES (@Fname, @Lname, " +
                    //    "@Email, @Empid, @Deptid, @Countryid, @Cityid, @Profile, @Status)";

                    //insert with get id 
                    //var query = "INSERT INTO UserMaster (FirstName, LastName, Email, EmployeeId, " +
                        //"DepartmentId, CountryId, CityId, ProfileSummary, Status) VALUES (@Fname, @Lname, " +
                        //"@Email, @Empid, @Deptid, @Countryid, @Cityid, @Profile, @Status) SELECt CAST(SCOPE_IDENTITY() AS Int)";
                    
                    //var parameters = new DynamicParameters();
                    //parameters.Add("Fname", userData.FirstName, DbType.String);
                    //parameters.Add("Lname", userData.LastName, DbType.String);
                    //parameters.Add("Email", userData.Email, DbType.String);
                    //parameters.Add("Empid", userData.EmployeeId, DbType.String);
                    //parameters.Add("Deptid", userData.DepartmentId, DbType.Int32);
                    //parameters.Add("Countryid", userData.CountryId, DbType.Int32);
                    //parameters.Add("Cityid", userData.CityId, DbType.Int32);
                    //parameters.Add("Profile", userData.ProfileSummary, DbType.String);
                    //parameters.Add("Status", userData.Status, DbType.Boolean);

                    //using (var connection = _cIDB.CreateConnection())
                    //{
                    //    await connection.ExecuteAsync(query, parameters);
                    //    //var id = await connection.QuerySingleAsync<int>(query, parameters);
                    //    //UserRequestModel userData = new UserRequestModel{
                    //    //  UserId = id,
                    //    //  FirstName = userData.FirstName
                    //    // .... Bind All Values
                    //    //}
                    //}

                    //using Dapper SP CALL
                    var SPName = "InsertUser";
                    var parameters = new DynamicParameters();
                    parameters.Add("FirstName", userData.FirstName, DbType.String);
                    parameters.Add("LastName", userData.LastName, DbType.String);
                    parameters.Add("Email", userData.Email, DbType.String);
                    parameters.Add("EmployeeId", userData.EmployeeId, DbType.String);
                    parameters.Add("DepartmentId", userData.DepartmentId, DbType.Int32);
                    parameters.Add("CountryId", userData.CountryId, DbType.Int32);
                    parameters.Add("CityId", userData.CityId, DbType.Int32);
                    parameters.Add("ProfileSummary", userData.ProfileSummary, DbType.String);
                    parameters.Add("Status", userData.Status, DbType.Boolean);

                    using (var connection = _cIDB.CreateConnection())
                    {
                        await connection.ExecuteAsync(SPName, parameters, commandType: CommandType.StoredProcedure);
                    }

                    scope.Complete();
                }
            }
            
            
            return userData;
        }

        public async Task<List<UserResponseModel>> GetUserData()
        {
            List<UserResponseModel> userRes = new List<UserResponseModel>();
            var query = "select UserId, FirstName, LastName, Email, EmployeeId, Department.DepartmentName," +
                        " Case when Status = 1 then 'Active' else 'In Active' end as Status from UserMaster" +
                        " inner join Department on UserMaster.DepartmentId = Department.DepartmentId ";

            //using (var connection = _cIDB.CreateConnection())//using method
            //using (var connection = _cIDB.Connection)//using IDBConnection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))) //using sql Connection
            {
                userRes = (await connection.QueryAsync<UserResponseModel>(query)).ToList(); //Basic
            }
            //List<UserResponseModel> userRes = (from user in _cIDB.Users
            //               join department in _cIDB.Departments on user.DepartmentId equals department.DepartmentId
            //               select new UserResponseModel()
            //               {
            //                   UserId = user.UserId,
            //                   FirstName = user.FirstName,
            //                   LastName = user.LastName,
            //                   Email = user.Email,
            //                   EmployeeId = user.EmployeeId,
            //                   DepartmentName = department.DepartmentName,
            //                   Status = user.Status ? "Active" : "In Active"
            //               }).ToList(); //using Entity FrameWorkCore
            return userRes;
        }

        public async Task<UserModel> DeleteUser(int id)
        {
            UserModel user = _cIDB.Users.Find(id);
            _cIDB.Users.Remove(user);
            _cIDB.SaveChanges();

            //UserModel user = new UserModel();
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            //    user = _cIDB.Users.Find(id);
            //    _cIDB.Users.Remove(user);
            //    _cIDB.SaveChanges();

            //    ApplicationUser applicationUser = await _userManager.FindByEmailAsync(user.Email);
            //    IdentityResult result = await _userManager.DeleteAsync(applicationUser);
            //    scope.Complete();
            //}

            return user;
        }

        public async Task<MissionVM> MissionData(int? id)
        {
            MissionVM missionvm = new MissionVM();
            if (id != null)
            {
                missionvm.Mission = _mapper.Map<MissionRequestModel>( _cIDB.Missions.Find(Convert.ToInt32(id)));
                missionvm.CityList = CityList(missionvm.Mission.CountryId);
            }
            else
            {
                missionvm.CityList = Enumerable.Empty<SelectListItem>();
            }
            missionvm.CountryList = CountryList();
            missionvm.ThemeList = _cIDB.MissionThemes.Where(x => x.Status == true).Select(i => new SelectListItem
            {
                Text = i.ThemeName.ToString().Trim(),
                Value = i.ThemeId.ToString()
            });
            missionvm.SkillList = _cIDB.MissionSkills.Where(x => x.Status == true).Select(i => new SelectListItem
            {
                Text = i.SkillTitle.ToString().Trim(),
                Value = i.SkillTitle.ToString()
            });
            missionvm.AvailabilityList = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Daily",
                    Value = "Daily"
                },
                new SelectListItem()
                {
                    Text = "Weekly",
                    Value = "Weekly"
                },
                new SelectListItem()
                {
                    Text = "Week-end",
                    Value = "Week-end"
                }
            };
            return missionvm;
        }

        public async Task<MissionRequestModel> postMissionData(MissionRequestModel missionreq, int? id)
        {
            MissionModel mission = _mapper.Map<MissionModel>(missionreq);
            if (id != null)
            {
                mission.MissionId = Convert.ToInt32(id);
                mission.ModifiedBy = "Admin";
                mission.ModifiedDate = DateTime.Now;
                _cIDB.Missions.Update(mission);
                _cIDB.SaveChanges();                
            }
            else
            {
                mission.CreatedBy = "Admin";
                mission.CreatedDate = DateTime.Now;
                await _cIDB.Missions.AddAsync(mission);
                await _cIDB.SaveChangesAsync();
            }
            return missionreq;
        }

        public async Task<ViewMissionResponse> GetMissionData(int pageIndex, int pageSize, string? filterValue)
        {
            List<MissionResponseModel> missionList = new List<MissionResponseModel>();
            int totalRecords = 0;

            if(!string.IsNullOrEmpty(filterValue))
            {
                missionList = (from mission in _cIDB.Missions
                               where mission.MissionTitle.ToLower().Contains(filterValue.ToLower())
                               || mission.MissionType.ToLower().Contains(filterValue.ToLower())
                               || mission.StartDate.ToString().Contains(filterValue)
                               || mission.EndDate.ToString().Contains(filterValue)
                               select new MissionResponseModel()
                               {
                                   MissionId = mission.MissionId,
                                   MissionTitle = mission.MissionTitle,
                                   MissionType = mission.MissionType,
                                   StartDate = mission.StartDate,
                                   EndDate = mission.EndDate
                               }).Skip(pageIndex * pageSize).Take(pageSize).ToList();

                string t = _cIDB.Missions.Select(mission => mission.StartDate.ToString()).FirstOrDefault();

                totalRecords = _cIDB.Missions.Where(x => x.MissionTitle.ToLower().Contains(filterValue.ToLower()) ||
                                    x.MissionType.ToLower().Contains(filterValue.ToLower()) || x.StartDate.ToString().Contains(filterValue)
                                    || x.EndDate.ToString().Contains(filterValue)).Count();
            }
            else
            {
                missionList = (from mission in _cIDB.Missions
                               select new MissionResponseModel()
                               {
                                   MissionId = mission.MissionId,
                                   MissionTitle = mission.MissionTitle,
                                   MissionType = mission.MissionType,
                                   StartDate = mission.StartDate,
                                   EndDate = mission.EndDate
                               }).Skip((pageIndex) * pageSize).Take(pageSize).ToList();

                totalRecords = _cIDB.Missions.Count();
            }

            //List<MissionResponseModel> missionList = _mapper.Map<List<MissionResponseModel>>(
            //    _cIDB.Missions.Skip((pageIndex - 1) * pageSize).Take(pageSize));

            ViewMissionResponse missionres = new ViewMissionResponse();
            missionres.Missions = missionList;
            missionres.totalRecords = totalRecords;
            return missionres;
            //int total = userRes.Count;
            //userRes 
            //return userRes;
        }

        public async Task<MissionModel> deleteMission(int? id)
        {
            MissionModel mission = _cIDB.Missions.Find(id);
            _cIDB.Missions.Remove(mission);
            _cIDB.SaveChanges();
            return mission;
        }

        public async Task<MissionThemeVM> getMissionThemes(int pageIndex, int pageSize)
        {
            string query = "SELECT ThemeId, ThemeName, CASE WHEN Status = 1 THEN 'Active' else 'In Active' end as Status" +
                " FROM MissionTheme order by ThemeId OFFSET " +
                (pageIndex * pageSize) + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY " +
                "SELECT count(*) from MissionTheme";
            List<MissionThemeResponse> missionThemes = new List<MissionThemeResponse>();
            int totalRecords = 0;

            using (var connection = _cIDB.CreateConnection())
            using(var multi = await connection.QueryMultipleAsync(query))
            {
                missionThemes = (await multi.ReadAsync<MissionThemeResponse>()).ToList();
                totalRecords = (await multi.ReadAsync<int>()).SingleOrDefault();
            }

            MissionThemeVM missionThemeVM = new MissionThemeVM()
            {
                MissionThemes = missionThemes,
                TotalCount = totalRecords
            };

            return missionThemeVM;
        }

        public async Task<MissionThemeModel> postMissionTheme(MissionThemeModel theme, int? themeId)
        {
            string spName = "UpsertTheme";
            var parameters = new DynamicParameters();
            parameters.Add("ThemeName", theme.ThemeName, DbType.String);
            parameters.Add("status", theme.Status, DbType.Boolean);
            parameters.Add("ActionBy", "Admin", DbType.String);
            if(themeId != null)
            {
                parameters.Add("Action", "Update", DbType.String);
                parameters.Add("ThemeId", themeId, DbType.Int32);
            }
            else
            {
                parameters.Add("Action", "Insert", DbType.String);
            }

            using(var connection = _cIDB.CreateConnection())
            {
                await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            }

            return theme;
        }

        public async Task<MissionThemeModel> deleteMissionThemeById(int ThemeId)
        {
            string query = "DELETE from MissionTheme WHERE ThemeId = @ThemeId";
            MissionThemeModel missionTheme = new MissionThemeModel();

            using (var connection = _cIDB.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { ThemeId=ThemeId });
            }

            return missionTheme;
        }

        public async Task<MissionSkillVM> getMissionSkills(int pageIndex, int pageSize)
        {
            string query = "SELECT SkillId, SkillTitle, CASE WHEN Status = 1 THEN 'Active' else 'In Active' end as Status" +
                " from MissionSkills order by SkillId OFFSET " +
                (pageIndex * pageSize) + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY " +
                "SELECT count(*) from MissionSkills";
            List<MissionSkillResponse> missionSkills = new List<MissionSkillResponse>();
            int totalRecords = 0;

            using (var connection = _cIDB.CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query))
            {
                missionSkills = (await multi.ReadAsync<MissionSkillResponse>()).ToList();
                totalRecords = (await multi.ReadAsync<int>()).SingleOrDefault();
            }

            MissionSkillVM missionSkillVM = new MissionSkillVM()
            {
                MissionSkills = missionSkills,
                TotalCount = totalRecords
            };

            return missionSkillVM;
        }

        public async Task<MissionSkillModel> postMissionSkill(MissionSkillModel skill, int? skillId)
        {
            string spName = "UpsertSkill";
            var parameters = new DynamicParameters();
            parameters.Add("SkillTitle", skill.SkillTitle, DbType.String);
            parameters.Add("status", skill.Status, DbType.Boolean);
            parameters.Add("ActionBy", "Admin", DbType.String);
            if (skillId != null)
            {
                parameters.Add("Action", "Update", DbType.String);
                parameters.Add("SkillId", skillId, DbType.Int32);
            }
            else
            {
                parameters.Add("Action", "Insert", DbType.String);
            }

            using (var connection = _cIDB.CreateConnection())
            {
                await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            }

            return skill;
        }

        public async Task<MissionSkillModel> deleteMissionSkillById(int SkillId)
        {
            string query = "DELETE from MissionSkills WHERE SkillId = @SkillId";
            MissionSkillModel missionSkill = new MissionSkillModel();

            using (var connection = _cIDB.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { SkillId=SkillId });
            }

            return missionSkill;
        }        

        public async Task<List<MissionVolunteeringResponse>> getMissionVoluneer()
        {
            List<MissionVolunteeringResponse> missionVolunteeringList = await (from missionVolunteer in _cIDB.MissionVolunteering
                                                                               join user in _cIDB.Users on missionVolunteer.UserId equals user.UserId
                                                                               join mission in _cIDB.Missions on missionVolunteer.MissionId equals mission.MissionId
                                                                               select new MissionVolunteeringResponse()
                                                                               {
                                                                                   VolunteerId = missionVolunteer.VolunteerId,
                                                                                   MissionTitle = mission.MissionTitle,
                                                                                   MissionId = missionVolunteer.MissionId,
                                                                                   UserId = missionVolunteer.UserId,
                                                                                   UserName = user.FirstName + " " + user.LastName,
                                                                                   AppliedDate = missionVolunteer.CreatedDate
                                                                               }).ToListAsync();

            return missionVolunteeringList;
        }

        public async Task<MissionVolunteeringModel> updateMissionVolunteer(MissionVolunteeringModel missionVolunteering, string email)
        {
            MissionVolunteeringModel dbData = _cIDB.MissionVolunteering.Find(missionVolunteering.VolunteerId);
            if (dbData != null)
            {
                dbData.Action = missionVolunteering.Action;
                dbData.ModifiedBy = email;
                dbData.ModifiedDate = DateTime.Now;
                _cIDB.Update(dbData);
                _cIDB.SaveChanges();
            }
            return dbData;
        }
    }
}
