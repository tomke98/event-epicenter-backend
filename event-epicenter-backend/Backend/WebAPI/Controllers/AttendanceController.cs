using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace WebAPI.Controllers
{
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService attendanceService;

        public AttendanceController()
        {
            attendanceService = new AttendanceService();
        }

        [Authorize(Roles = "user, admin")]
        [Route("attendances")]
        [HttpPost]
        public async Task<IActionResult> CreateAttendanceAsync(AttendanceREST attendance)
        {
            var result = await attendanceService.CreateAttendanceAsync(new Attendance(attendance.EventId, attendance.UserId));

            if (result)
            {
                return Ok(attendance);
            }

            return BadRequest("Attendance not created.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("attendances")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAttendanceAsync(AttendanceREST attendance)
        {
            if (User.Claims.Any())
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id").Value;

                if(userId != attendance.UserId)
                {
                    return BadRequest("You can't delete other user's attendance.");
                }
            }

            var result = await attendanceService.DeleteAttendanceAsync(new Attendance(attendance.EventId, attendance.UserId));

            if (result)
            {
                return Ok("Attendance deleted.");
            }

            return BadRequest("Attendance not deleted.");
        }
    }

    public class AttendanceREST
    {
        public string UserId { get; set; }
        public string EventId { get; set; }
    }
}