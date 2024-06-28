using Google.Cloud.Firestore;
using Model;

namespace Repository
{
    public class EventTypeRepository
    {
        private readonly Database database;

        public EventTypeRepository()
        {
            database = new Database();
        } 

        public async Task<List<Dictionary<string, object>>> GetAllEventTypesAsync()
        {
            return await database.GetAll("eventType");
        }

        public async Task<List<Dictionary<string, object>>> GetEventTypesByIdsAsync(List<string> ids)
        {
            return await database.GetByIds("eventType", ids);
        }

        public async Task<Dictionary<string, object>> GetEventTypeByIdAsync(string id)
        {
            return await database.GetById("eventType", id);
        }

        public async Task<bool> CreateEventTypeAsync(EventType eventType) 
        {
            DocumentReference documentReference = database.db.Collection("eventType").Document();
            eventType.Id = documentReference.Id;

            var result = await documentReference.SetAsync(EventType.MapEventTypeToDictionary(eventType));
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateEventTypeAsync(EventType eventType)
        {
            DocumentReference documentReference = database.db.Collection("eventType").Document(eventType.Id);

            var result = await documentReference.SetAsync(EventType.MapEventTypeToDictionary(eventType));
            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteEventTypeAsync(string id)
        {
            return await database.DeleteById("eventType", id);
        }
    }
}