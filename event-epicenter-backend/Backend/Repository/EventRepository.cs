using Google.Cloud.Firestore;
using Model;

namespace Repository
{
    public class EventRepository
    {
        private readonly Database database;

        public EventRepository()
        {
            database = new Database();
        }

        public async Task<List<Dictionary<string, object>>> GetAllEventsAsync()
        {
            return await database.GetAll("event");
        }

        public async Task<Dictionary<string, object>> GetEventByIdAsync(string id)
        {
            return await database.GetById("event", id);
        }

        public async Task<bool> CreateEventAsync(Event _event) 
        {
            DocumentReference documentReference = database.db.Collection("event").Document();
            _event.Id = documentReference.Id;

            var result = await documentReference.SetAsync(Event.MapEventToDictionary(_event));
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateEventAsync(Event _event)
        {
            DocumentReference documentReference = database.db.Collection("event").Document(_event.Id);

            var result = await documentReference.SetAsync(Event.MapEventToDictionary(_event));
            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteEventAsync(string id)
        {
            return await database.DeleteById("event", id);
        }
    }
}