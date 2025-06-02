using CI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.BusinessLogic.Repository.Interface
{
    public interface IAdminRepository
    {
        Task<UserVM> UserData(int? id);

        Task<IEnumerable<SelectListItem>> BindCity(int? Id);

        Task<UserRequestModel> PostUserData(UserRequestModel userData, int? id);

        Task<List<UserResponseModel>> GetUserData();

        Task<UserModel> DeleteUser(int id);

        Task<MissionVM> MissionData(int? id);

        Task<MissionRequestModel> postMissionData(MissionRequestModel mission, int? id);

        Task<ViewMissionResponse> GetMissionData(int pageIndex, int pageSize, string? filterValue);

        Task<MissionModel> deleteMission(int? id);

        Task<MissionThemeVM> getMissionThemes(int pageIndex, int pageSize);

        Task<MissionThemeModel> postMissionTheme(MissionThemeModel theme, int? themeId);

        Task<MissionThemeModel> deleteMissionThemeById(int ThemeId);

        Task<MissionSkillVM> getMissionSkills(int pageIndex, int pageSize);

        Task<MissionSkillModel> postMissionSkill(MissionSkillModel skill, int? skillId);

        Task<MissionSkillModel> deleteMissionSkillById(int SkillId);

        Task<List<MissionVolunteeringResponse>> getMissionVoluneer();

        Task<MissionVolunteeringModel> updateMissionVolunteer(MissionVolunteeringModel missionVolunteering, string email);
    }
}
