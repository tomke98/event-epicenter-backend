using Google.Cloud.Firestore;
using Model;

namespace Repository
{
    public class AttendanceRepository
    {
        private readonly Database database;

        public AttendanceRepository()
        {
            database = new Database();
        }

        public async Task<List<Dictionary<string, object>>> GetAllAttendancesAsync()
        {
            return await database.GetAll("attendance");
        }

        public async Task<bool> CreateAttendanceAsync(Attendance attendance) 
        {
            var attendaceId = attendance.EventId + attendance.UserId;
            DocumentReference documentReference = database.db.Collection("attendance").Document(attendaceId);

            var result = await documentReference.SetAsync(Attendance.MapAttendanceToDictionary(attendance));
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAttendanceAsync(Attendance attendance)
        {
            var attendaceId = attendance.EventId + attendance.UserId;
            DocumentReference documentReference = database.db.Collection("attendance").Document(attendaceId);

            if(documentReference != null)
            {
                await documentReference.DeleteAsync();
                return true;
            }

            return false;
        }
    }
}