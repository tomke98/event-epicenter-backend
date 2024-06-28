using Model;
using Repository;

namespace Service
{
    public class AttendanceService
    {
        private readonly AttendanceRepository repository;
        public AttendanceService()
        {
            repository = new AttendanceRepository();
        }

        public async Task<List<Attendance>> GetAttendanceByEventIdAsync(string eventId)
        {
            var attendanceDictionaries = await repository.GetAllAttendancesAsync();

            var attendances = new List<Attendance>();

            foreach (var attendanceDictionary in attendanceDictionaries)
            {
                attendances.Add(Attendance.MapDictionaryToAttendance(attendanceDictionary));
            }

            return attendances.Where(a => a.EventId == eventId).ToList();
        }

        public async Task<List<Attendance>> GetAttendanceByUserIdAsync(string userId)
        {
            var attendanceDictionaries = await repository.GetAllAttendancesAsync();

            var attendances = new List<Attendance>();

            foreach (var attendanceDictionary in attendanceDictionaries)
            {
                attendances.Add(Attendance.MapDictionaryToAttendance(attendanceDictionary));
            }

            return attendances.Where(a => a.UserId == userId).ToList();
        }

        public async Task<bool> CreateAttendanceAsync(Attendance attendance)
        {
            return await repository.CreateAttendanceAsync(attendance);
        }

        public async Task<bool> DeleteAttendanceAsync(Attendance attendance)
        {
            return await repository.DeleteAttendanceAsync(attendance);
        }
    }
}