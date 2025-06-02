using CI.BusinessLogic.Repository.Interface;
using CI.Models;
using CI.Utility;
using CI.Utility.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CIWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [HttpResponseExceptionFilter]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _config;

        public AdminController(IAdminRepository adminRepository, IConfiguration config)
        {
            _adminRepository = adminRepository;
            _config = config;
        }

        [HttpGet]
        [Route("getUser")]
        public async Task<IActionResult> getUser(int? id)
        {
            UserVM userVM = await _adminRepository.UserData(id);
            return Ok(userVM);
        }

        [HttpGet]
        [Route("getCity")]
        public async Task<IActionResult> getCity(int? id)
        {
            return Ok(await _adminRepository.BindCity(id));
        }

        [HttpPost]
        [Route("postUserData")]
        public async Task<IActionResult> postUserData([FromBody] UserRequestModel userData, int? id)
        {
            return Ok(await _adminRepository.PostUserData(userData, id));
        }

        [HttpGet]
        [Route("getUserData")]
        public async Task<IActionResult> getUserData()
        {
            return Ok(await _adminRepository.GetUserData());
        }

        [HttpDelete]
        [Route("deleteUser")]
        public async Task<IActionResult> deleteUser(int id)
        {            
            return Ok(await _adminRepository.DeleteUser(id));
        }

        [HttpGet]
        [Route("getMission")]
        public async Task<IActionResult> getMission(int? id)
        {
            MissionVM missionVM = await _adminRepository.MissionData(id);
            return Ok(missionVM);
        }

        [HttpPost]
        [Route("postMission")]
        public async Task<ActionResult> postMission([FromBody] MissionRequestModel missionData, int? id)
        {
            return Ok(await _adminRepository.postMissionData(missionData, id));
        }

        [HttpGet]
        [Route("getMissionData")]
        public async Task<IActionResult> getMissionData(int pageIndex, int pageSize, string? filterValue)
        {
            return Ok(await _adminRepository.GetMissionData(pageIndex, pageSize, filterValue));
        }

        [HttpDelete]
        [Route("deleteMission")]
        public async Task<IActionResult> deleteMission(int id)
        {
            return Ok(await _adminRepository.deleteMission(id));
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadImage")]
        public async Task<IActionResult> uploadImage(List<IFormFile> images)
        {
            try
            {
                //var formCollection = await Request.ReadFormAsync();
                //IFormFile file = formCollection.Files.First();
                string validationmsg = string.Empty;
                if (images.Count > 0)
                {
                    string Result = new FileUpload(_config).uploadFile(images, "Mission");
                    if(Result == "1")
                    {
                        validationmsg = "Please Select Valid Images (.png, .jpg, .jpeg)";
                        return BadRequest(new { validationmsg });
                    }
                    else
                    {
                        string Imagebase64 = string.Empty;
                        //if (!string.IsNullOrEmpty(Result))
                        //{
                        //    Imagebase64 = new Base64strConversion(_config).convertToBase64String(Result, "Image", "Mission");
                        //}
                        //return Ok(new { Result, Imagebase64 });
                        return Ok(new { Result});
                    }    
                }
                else
                {
                    validationmsg = "Please Upload Images";
                    return BadRequest(new { validationmsg });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadDoc")]
        public async Task<IActionResult> uploadDoc(List<IFormFile> docs)
        {
            try
            {
                //var formCollection = await Request.ReadFormAsync();
                //IFormFile file = formCollection.Files.First();
                string validationmsg = string.Empty;
                if (docs.Count > 0)
                {
                    string Result = new FileUpload(_config).uploadFile(docs, "Mission");
                    if (Result == "1")
                    {
                        validationmsg = "Please Select Valid Documents (.pdf, .doc, .docx, .xls, .xlsx)";
                        return BadRequest(new { validationmsg });
                    }
                    else
                    {
                        //string Docbase64 = string.Empty;
                        //if (!string.IsNullOrEmpty(Result))
                        //{
                        //    Docbase64 = new Base64strConversion(_config).convertToBase64String(Result, "Doc", "Mission");
                        //}
                        //return Ok(new { Result, Docbase64 });
                        return Ok(new { Result});
                    }
                }
                else
                {
                    validationmsg = "Please Upload Documents";
                    return BadRequest(new { validationmsg });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadFiles")]
        public async Task<IActionResult> uploadFiles(List<IFormFile> files)
        {
            string msg = string.Empty;
            try
            {
                var formCollection = await Request.ReadFormAsync();
                IFormFile file = (IFormFile)formCollection.Files;
                msg = "Done";
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
            return Ok(new { msg });
        }

        [HttpGet]
        [Route("getThemes")]
        public async Task<IActionResult> getThemes(int pageIndex, int pageSize)
        {
            return Ok(await _adminRepository.getMissionThemes(pageIndex, pageSize));
        }

        [HttpPost]
        [Route("postThemes")]
        public async Task<IActionResult> postThemes([FromBody] MissionThemeModel themeModel, int? themeId)
        {
            return Ok(await _adminRepository.postMissionTheme(themeModel, themeId));
        }

        [HttpDelete]
        [Route("deleteTheme")]
        public async Task<IActionResult> deleteTheme(int themeId)
        {
            return Ok(await _adminRepository.deleteMissionThemeById(themeId));
        }

        [HttpGet]
        [Route("getSkills")]
        public async Task<IActionResult> getSkills(int pageIndex, int pageSize)
        {
            return Ok(await _adminRepository.getMissionSkills(pageIndex, pageSize));
        }

        [HttpPost]
        [Route("postSkills")]
        public async Task<IActionResult> postSkills([FromBody] MissionSkillModel skillModel, int? skillId)
        {
            return Ok(await _adminRepository.postMissionSkill(skillModel, skillId));
        }

        [HttpDelete]
        [Route("deleteSkill")]
        public async Task<IActionResult> deleteSkill(int skillId)
        {
            return Ok(await _adminRepository.deleteMissionSkillById(skillId));
        }

        [HttpGet]
        [Route("missionApplication")]
        public async Task<ActionResult> missionApplication()
        {
            return Ok(await _adminRepository.getMissionVoluneer());
        }

        [HttpPost]
        [Route("updateApplication")]
        public async Task<IActionResult> updateApplication(MissionVolunteeringModel missionVolunteering)
        {
            return Ok(await _adminRepository.updateMissionVolunteer(missionVolunteering, User.Identity.Name));
        }
    }
}
