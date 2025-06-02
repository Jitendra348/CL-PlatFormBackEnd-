using CI.BusinessLogic.Repository.Interface;
using CI.Data;
using CI.Models;
using CI.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CI.BusinessLogic.Repository
{
    public class VolunteerRepository: IVolunteerRepository 
    {
        private readonly CIDBContext _cIDB;
        private readonly IConfiguration _config;

        public VolunteerRepository(CIDBContext cIDB, IConfiguration config)
        {
            _cIDB = cIDB;
            _config = config;
        }

        public async Task<VolunteerMissionVM> MissionList(int pageIndex, int pageSize, string? filters, string? orderBy, string currentUser)
        {
            List<VolnteerMissionModel> volnteerMission = new List<VolnteerMissionModel>();
            int totalCount = 0;

            int UserId = await _cIDB.Users.Where(x => x.Email == currentUser).Select(x => x.UserId).FirstOrDefaultAsync();
            string missionIds = await (from fav in _cIDB.FavMissions
                                       where fav.UserId ==  UserId
                                       select fav.MissionId).FirstOrDefaultAsync();

            List<int> favMissions = new List<int>();

            string spName = "VolunteerMission";
            var parameters = new DynamicParameters();
            parameters.Add("pageIndex", pageIndex, DbType.Int32);
            parameters.Add("pageSize", pageSize, DbType.Int32);

            if (!string.IsNullOrEmpty(filters))
                parameters.Add("filter", filters, DbType.String); 
            if(!string.IsNullOrEmpty(orderBy))
                parameters.Add("orderBy", orderBy, DbType.String);
            if (!string.IsNullOrEmpty(missionIds))
            {
                favMissions = missionIds.Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToList<int>();
                parameters.Add("favMissions", missionIds, DbType.String);
            }                

            using (var connection = _cIDB.CreateConnection())
            using (var reader = await connection.ExecuteReaderAsync(spName, parameters, commandType: CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    VolnteerMissionModel vm = new VolnteerMissionModel()
                    {
                        MissionId = Convert.ToInt32(reader["MissionId"]),
                        MissionTitle = reader["MissionTitle"].ToString(),
                        ShortDescription = reader["ShortDescription"].ToString(),
                        MissionDescription = reader["MissionDescription"].ToString(),
                        MissionType = reader["MissionType"].ToString(),
                        Theme = reader["ThemeName"].ToString(),
                        Skills = reader["Skills"].ToString(),
                        MissionTypeData = reader["MissionTypeData"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        OrganizationName = reader["OrganizationName"].ToString(),
                        OrganizationDetail = reader["OrganizationDetail"].ToString(),
                        City = reader["CityName"].ToString(),
                        Country = reader["CountryName"].ToString(),
                        TotalSeats = reader["TotalSeats"] != DBNull.Value ? Convert.ToInt32(reader["TotalSeats"]) : null,
                        RegistrationDeadline = reader["RegistrationDeadline"] != DBNull.Value ? Convert.ToDateTime(reader["RegistrationDeadline"]) : null,
                        Availability = reader["Availability"].ToString(),
                        Images = new Base64strConversion(_config).convertToBase64(reader["Images"].ToString(), "images", "Mission"),
                        Documnets = new Base64strConversion(_config).convertToBase64(reader["Documents"].ToString(), "docs", "Mission"),
                        GoalObjective = reader["GoalObjective"].ToString(),
                        Rate = Convert.ToInt32(reader["Rate"])
                    };

                    volnteerMission.Add(vm);
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        totalCount = Convert.ToInt32(reader["TotalRecords"]);
                    }
                }
            }            

            List<string> countrylst = await (from mission in _cIDB.Missions
                                          join country in _cIDB.Countries on mission.CountryId equals country.CountryId
                                          select country.CountryName).Distinct().ToListAsync();

            List<string> citylst = await (from mission in _cIDB.Missions
                                             join city in _cIDB.Cities on mission.CityId equals city.CityId
                                             select city.CityName).Distinct().ToListAsync();

            List<string> themelst = await (from mission in _cIDB.Missions
                                             join theme in _cIDB.MissionThemes on mission.ThemeId equals theme.ThemeId
                                             select theme.ThemeName).Distinct().ToListAsync();

            List<string> tempskilll = await (from mission in _cIDB.Missions
                                             select mission.Skills).Distinct().ToListAsync();

            List<string> skilllst = new List<string>();

            foreach (string s in tempskilll)
            {
                if (s.Contains(','))
                {
                    foreach (string str in s.Split(','))
                        skilllst.Add(str);
                }
                else
                    skilllst.Add(s);
            }

            skilllst = skilllst.Distinct().ToList();            

            VolunteerMissionVM volunteerMissionVM = new VolunteerMissionVM()
            {
                volnteerMissions = volnteerMission,
                country = countrylst,
                city = citylst,
                themes = themelst,
                skills = skilllst,
                totalCount = totalCount,
                favMissions = favMissions
            };

            return volunteerMissionVM;
        }

        public async Task<FavMissionModel> UpsertFavoriteMissions(string email, int MissionId)
        {
            FavMissionModel favMission = new FavMissionModel();
            int UserId = await _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefaultAsync();

            if (UserId != 0)
            {
                var records = await  _cIDB.FavMissions.Where(x => x.UserId == UserId)
                                   .FirstOrDefaultAsync();

                if (records == null)
                {
                    favMission.UserId = UserId;
                    favMission.MissionId = MissionId.ToString();
                    favMission.CreatedBy = email;
                    favMission.CreatedDate = DateTime.Now;
                    await _cIDB.AddAsync(favMission);
                    await _cIDB.SaveChangesAsync();
                }
                else
                {
                    if (string.IsNullOrEmpty(records.MissionId))
                    {
                        records.MissionId = MissionId.ToString();
                        records.ModifiedBy = email;
                        records.ModifiedDate = DateTime.Now;
                        _cIDB.Update(records);
                        await _cIDB.SaveChangesAsync();
                    }
                    else if (!records.MissionId.Contains(MissionId.ToString()))
                    {                        
                        records.MissionId = records.MissionId + ", " + MissionId.ToString();
                        records.ModifiedBy = email;
                        records.ModifiedDate = DateTime.Now;
                        _cIDB.Update(records);
                        await _cIDB.SaveChangesAsync();
                    }
                }
            }

            return favMission;
        }

        public async Task<FavMissionModel> RemoveFavoriteMissions(string email, int MissionId)
        {
            FavMissionModel favMission = new FavMissionModel();
            int UserId = await _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefaultAsync();

            if (UserId != 0)
            {
                var records = await _cIDB.FavMissions.Where(x => x.UserId == UserId)
                                   .FirstOrDefaultAsync();

                if (records != null)
                {
                    if (records.MissionId.Contains(MissionId.ToString()))
                    {
                       
                        var missionIds = records.MissionId.Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToList<int>();
                        missionIds.Remove(MissionId);
                        records.MissionId = String.Join(",", missionIds);
                        records.ModifiedBy = email;
                        records.ModifiedDate = DateTime.Now;
                        _cIDB.Update(records);
                        await _cIDB.SaveChangesAsync();
                    }
                }
            }

            return favMission;
        }

        

        public async Task<MissionModel> MissionRating(string email, int MissionId, int Rate)
        {
            MissionRatingModel missionRating = new MissionRatingModel();
            missionRating.MissionId = MissionId;
            missionRating.Rate = Rate;
            missionRating.CreatedBy = email;
            missionRating.CreatedDate = DateTime.Now;
            await _cIDB.AddAsync(missionRating);
            await _cIDB.SaveChangesAsync();

            int rate = Convert.ToInt32(_cIDB.MissionRatings.Where(x => x.MissionId == MissionId).Average(x => x.Rate));

            MissionModel missionRecord = _cIDB.Missions.Find(MissionId);

            if (missionRecord != null)
            {
                missionRecord.Rate = rate;
                _cIDB.Update(missionRecord);
                _cIDB.SaveChanges();

                missionRecord = await _cIDB.Missions.Where(x => x.MissionId == MissionId).Select(x => new MissionModel()
                {
                    MissionId = MissionId,
                    Rate = rate
                }).FirstOrDefaultAsync();
            }

            return missionRecord;
        }

        public async Task<IEnumerable<SelectListItem>> TimeMissionList(string email)
        {
            int userId = _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
            return (from mission in _cIDB.Missions
                    join missionVolunteer in _cIDB.MissionVolunteering on mission.MissionId equals missionVolunteer.MissionId
                    where missionVolunteer.UserId == userId && missionVolunteer.Action == "Accepted" && mission.MissionType == "Time"
                    select new SelectListItem
                    {
                        Text = mission.MissionTitle.ToString(),
                        Value = mission.MissionId.ToString()
                    });
        }

        public async Task<IEnumerable<SelectListItem>> GoalMissionList(string email)
        {
            int userId = _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
            return (from mission in _cIDB.Missions
                    join missionVolunteer in _cIDB.MissionVolunteering on mission.MissionId equals missionVolunteer.MissionId
                    where missionVolunteer.UserId == userId && missionVolunteer.Action == "Accepted" && mission.MissionType == "Goal"
                    select new SelectListItem
                    {
                        Text = mission.MissionTitle.ToString(),
                        Value = mission.MissionId.ToString()
                    });
        }

        public async Task<MissionVolunteeringModel> UpsertMissionVolunteer(int MissionId, string email)
        {
            MissionVolunteeringModel missionVolunteering = new MissionVolunteeringModel();
            int UserId = _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
            missionVolunteering.MissionId = MissionId;
            missionVolunteering.UserId = UserId;
            missionVolunteering.Action = "Applied";
            missionVolunteering.CreatedBy = email;
            missionVolunteering.CreatedDate = DateTime.Now;
            await _cIDB.AddAsync(missionVolunteering);
            await _cIDB.SaveChangesAsync();
            return missionVolunteering;
        }
        public async Task<VolunteerTimeModel> UpsertVolunteerTime(VolunteerTimeModel volunteerTime, string email, int? timeId) 
        {
            int UserId = _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
            if (timeId != null)
            {
                volunteerTime.VolunteerId = Convert.ToInt32(timeId);
                volunteerTime.ModifiedBy = email;
                volunteerTime.ModifiedDate = DateTime.Now;
                _cIDB.volunteerTimes.Update(volunteerTime);
                _cIDB.SaveChanges();
            }
            else
            {
                volunteerTime.UserId = UserId;  
                volunteerTime.CreatedBy = email;
                volunteerTime.CreatedDate = DateTime.Now;
                await _cIDB.AddAsync(volunteerTime);
                await _cIDB.SaveChangesAsync();
            }
            return volunteerTime;
        }

        public async Task<VolunteerGoalModel> UpsertVolunteerGoal(VolunteerGoalModel volunteerGoal, string email, int? goalId) 
        {
            int UserId = _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
            if (goalId != null)
            {
                volunteerGoal.Id = Convert.ToInt32(goalId);
                volunteerGoal.ModifiedBy = email;
                volunteerGoal.ModifiedDate = DateTime.Now;
                _cIDB.VolunteerGoal.Update(volunteerGoal);
                _cIDB.SaveChanges();
            }
            else
            {
                volunteerGoal.UserId = UserId;
                volunteerGoal.CreatedBy = email;
                volunteerGoal.CreatedDate = DateTime.Now;
                await _cIDB.AddAsync(volunteerGoal);
                await _cIDB.SaveChangesAsync();
            }

            return volunteerGoal;
        }

        public async Task<VolunteerTimeSheet> getVolunteerTimesheet(string email)
        {
            int UserId = _cIDB.Users.Where(x => x.Email == email).Select(x => x.UserId).FirstOrDefault();
            List<VolunteerTimeResponse> timeResponses = await (from time in _cIDB.volunteerTimes
                                                    join mission in _cIDB.Missions on time.MissionId equals mission.MissionId
                                                    where time.UserId == UserId select new VolunteerTimeResponse()
                                                    {
                                                        VolunteerId = time.VolunteerId,
                                                        Mission = mission.MissionTitle,
                                                        Date = time.Date,
                                                        Hours = time.Hours.ToString()+"h",
                                                        Minutes = time.Minutes.ToString()+"m",
                                                        Message = time.Message 
                                                    }).ToListAsync();

            List<VolunteerGoalResponse> goalResponses = await (from goal in _cIDB.VolunteerGoal
                                                               join mission in _cIDB.Missions on goal.MissionId equals mission.MissionId
                                                               where goal.UserId == UserId
                                                               select new VolunteerGoalResponse()
                                                               {
                                                                   Id = goal.Id,
                                                                   Mission = mission.MissionTitle,
                                                                   Date= goal.Date,
                                                                   Actions = goal.Actions,
                                                                   Message = goal.Message
                                                               }).ToListAsync();

            VolunteerTimeSheet timeSheet = new VolunteerTimeSheet()
            {
                volunteerTime = timeResponses,
                volunteerGoal = goalResponses
            };

            return timeSheet;
        }
    }
}


//EntityFramework Core Code for Mission Listing with filtering, pagination and sorting


//    public async Task<VolunteerMissionVM> MissionList(int pageIndex, int pageSize, string? filters, string? orderBy, string currentUser)
//{
//    List<VolnteerMissionModel> volnteerMission = new List<VolnteerMissionModel>();
//    int totalCount = 0;

//    if (!string.IsNullOrEmpty(filters))
//    {
//        string spName = "VolunteerMissionSearch";
//        var parameters = new DynamicParameters();
//        parameters.Add("filter", filters, DbType.String);
//        parameters.Add("pageIndex", pageIndex, DbType.Int32);
//        parameters.Add("pageSize", pageSize, DbType.Int32);

//        using (var connection = _cIDB.CreateConnection())
//        using (var reader = await connection.ExecuteReaderAsync(spName, parameters, commandType: CommandType.StoredProcedure))
//        {
//            var readparser = reader.GetRowParser<int>();
//            while (reader.Read())
//            {
//                VolnteerMissionModel vm = new VolnteerMissionModel()
//                {
//                    MissionId = Convert.ToInt32(reader["MissionId"]),
//                    MissionTitle = reader["MissionTitle"].ToString(),
//                    MissionDescription = reader["MissionDescription"].ToString(),
//                    MissionType = reader["MissionType"].ToString(),
//                    Theme = reader["ThemeName"].ToString(),
//                    Skills = reader["Skills"].ToString(),
//                    StartDate = Convert.ToDateTime(reader["StartDate"]),
//                    EndDate = Convert.ToDateTime(reader["EndDate"]),
//                    OrganizationName = reader["OrganizationName"].ToString(),
//                    OrganizationDetail = reader["OrganizationDetail"].ToString(),
//                    City = reader["CityName"].ToString(),
//                    Country = reader["CountryName"].ToString(),
//                    TotalSeats = Convert.ToInt32(reader["TotalSeats"]),
//                    RegistrationDeadline = Convert.ToDateTime(reader["RegistrationDeadline"]),
//                    Availability = reader["Availability"].ToString(),
//                    Images = new Base64strConversion(_config).convertToBase64(reader["Images"].ToString(), "images", "Mission"),
//                    Documnets = new Base64strConversion(_config).convertToBase64(reader["Documents"].ToString(), "docs", "Mission")
//                };

//                volnteerMission.Add(vm);
//            }

//            //int newID = parameters.Get<int>("count");

//            if (reader.NextResult())
//            {
//                while (reader.Read())
//                {
//                    totalCount = Convert.ToInt32(reader["TotalRecords"]);
//                }
//            }
//        }

//    }
//    else
//    {
//        volnteerMission = await (from mission in _cIDB.Missions
//                                 join country in _cIDB.Countries on mission.CountryId equals country.CountryId
//                                 join city in _cIDB.Cities on mission.CityId equals city.CityId
//                                 join theme in _cIDB.MissionThemes on mission.ThemeId equals theme.ThemeId
//                                 select new VolnteerMissionModel()
//                                 {
//                                     MissionId = mission.MissionId,
//                                     MissionTitle = mission.MissionTitle,
//                                     MissionDescription = mission.MissionDescription,
//                                     MissionType = mission.MissionType,
//                                     Theme = theme.ThemeName,
//                                     Skills = mission.Skills,
//                                     StartDate = mission.StartDate,
//                                     EndDate = mission.EndDate,
//                                     OrganizationName = mission.OrganizationName,
//                                     OrganizationDetail = mission.OrganizationDetail,
//                                     City = city.CityName,
//                                     Country = country.CountryName,
//                                     TotalSeats = Convert.ToInt32(mission.TotalSeats),
//                                     RegistrationDeadline = mission.RegistrationDeadline,
//                                     Availability = mission.Availability,
//                                     Images = new Base64strConversion(_config).convertToBase64(mission.Images, "images", "Mission"),
//                                     Documnets = new Base64strConversion(_config).convertToBase64(mission.Documents, "docs", "Mission")
//                                 }).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

//        totalCount = await _cIDB.Missions.CountAsync();
//    }


//    List<string> countrylst = await (from mission in _cIDB.Missions
//                                     join country in _cIDB.Countries on mission.CountryId equals country.CountryId
//                                     select country.CountryName).Distinct().ToListAsync();

//    List<string> citylst = await (from mission in _cIDB.Missions
//                                  join city in _cIDB.Cities on mission.CityId equals city.CityId
//                                  select city.CityName).Distinct().ToListAsync();

//    List<string> themelst = await (from mission in _cIDB.Missions
//                                   join theme in _cIDB.MissionThemes on mission.ThemeId equals theme.ThemeId
//                                   select theme.ThemeName).Distinct().ToListAsync();

//    List<string> tempskilll = await (from mission in _cIDB.Missions
//                                     select mission.Skills).Distinct().ToListAsync();

//    List<string> skilllst = new List<string>();

//    int UserId = await _cIDB.Users.Where(x => x.Email == currentUser).Select(x => x.UserId).FirstOrDefaultAsync();
//    string missionIds = await (from fav in _cIDB.FavMissions
//                               where fav.UserId ==
//UserId
//                               select fav.MissionId).FirstOrDefaultAsync();

//    foreach (string s in tempskilll)
//    {
//        if (s.Contains(','))
//        {
//            foreach (string str in s.Split(','))
//                skilllst.Add(str);
//        }
//        else
//            skilllst.Add(s);
//    }

//    skilllst = skilllst.Distinct().ToList();

//    List<int> favMissions = missionIds.Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToList<int>();

//    if (!string.IsNullOrEmpty(orderBy))
//    {
//        if (orderBy == "Newest")
//        {
//            volnteerMission = volnteerMission.OrderBy(x => x.MissionId).ToList<VolnteerMissionModel>();
//        }
//        else if (orderBy == "Oldest")
//        {
//            volnteerMission = volnteerMission.OrderByDescending(x => x.MissionId).ToList<VolnteerMissionModel>();
//        }
//        else if (orderBy == "Lowest available seats")
//        {
//            volnteerMission = volnteerMission.OrderByDescending(x => x.TotalSeats).ToList<VolnteerMissionModel>();
//        }
//        else if (orderBy == "Highest available seats")
//        {
//            volnteerMission = volnteerMission.OrderBy(x => x.TotalSeats).ToList<VolnteerMissionModel>();
//        }
//        else if (orderBy == "My favourites")
//        {
//            volnteerMission = volnteerMission.OrderBy(x => favMissions.Contains(x.MissionId) ? 0 : 1).ToList<VolnteerMissionModel>();
//        }
//        else if (orderBy == "Registration deadline")
//        {
//            volnteerMission = volnteerMission.OrderBy(x => x.RegistrationDeadline).ToList<VolnteerMissionModel>();
//        }
//    }

//    VolunteerMissionVM volunteerMissionVM = new VolunteerMissionVM()
//    {
//        volnteerMissions = volnteerMission,
//        country = countrylst,
//        city = citylst,
//        themes = themelst,
//        skills = skilllst,
//        totalCount = totalCount,
//        favMissions = favMissions
//    };

//    return volunteerMissionVM;
//}