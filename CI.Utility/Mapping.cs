using AutoMapper;
using CI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.Utility
{
    public class UserDataMapping: Profile
    {
        public UserDataMapping()
        {
            CreateMap<UserRequestModel, UserModel>();
            CreateMap<UserModel, UserRequestModel>();
            CreateMap<MissionRequestModel, MissionModel>().ForMember(mission => mission.Skills, 
                            missionreq => missionreq.MapFrom(x => String.Join(", ", x.Skills)));
            CreateMap<MissionModel, MissionRequestModel>().ForMember(missionreq => missionreq.Skills, 
                            mission => mission.MapFrom(x => x.Skills.Split(',', System.StringSplitOptions.TrimEntries)));
        }
    }
}
