using CI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.BusinessLogic.Repository.Interface
{
    public interface IVolunteerRepository
    {
        Task<VolunteerMissionVM> MissionList(int pageIndex, int pageSize, string? filters, string? orderBy, string currentUser);

        Task<FavMissionModel> UpsertFavoriteMissions(string email, int MissionId);

        Task<FavMissionModel> RemoveFavoriteMissions(string email, int MissionId);

        Task<MissionModel> MissionRating(string email, int MissionId, int Rate);

        Task<IEnumerable<SelectListItem>> TimeMissionList(string email);

        Task<IEnumerable<SelectListItem>> GoalMissionList(string email);

        Task<MissionVolunteeringModel> UpsertMissionVolunteer(int MissionId, string email);

        Task<VolunteerTimeModel> UpsertVolunteerTime(VolunteerTimeModel volunteerTime, string email, int? timeId);

        Task<VolunteerGoalModel> UpsertVolunteerGoal(VolunteerGoalModel volunteerGoal, string email, int? goalId);

        Task<VolunteerTimeSheet> getVolunteerTimesheet(string email);
    }
}
