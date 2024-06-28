using Model;
using Repository;

namespace Service
{
    public class EventTypeService
    {
        private readonly EventTypeRepository repository;
        public EventTypeService()
        {
            repository = new EventTypeRepository();
        }

        public async Task<List<EventType>> GetAllEventTypesAsync()
        {
            var eventTypeDictionaries = await repository.GetAllEventTypesAsync();

            var eventTypes = new List<EventType>();

            foreach (var eventTypeDictionary in eventTypeDictionaries)
            {
                eventTypes.Add(EventType.MapDictionaryToEventType(eventTypeDictionary));
            }

            return eventTypes;
        }

        public async Task<EventType> GetEventTypeByIdAsync(string id)
        {
            var eventTypeDictionary = await repository.GetEventTypeByIdAsync(id);

            return eventTypeDictionary == null ? null : EventType.MapDictionaryToEventType(eventTypeDictionary);
        }

        public async Task<bool> CreateEventTypeAsync(EventType eventType)
        {
            return await repository.CreateEventTypeAsync(eventType);
        }

        public async Task<bool> UpdateEventTypeAsync(EventType eventType)
        {
            return await repository.UpdateEventTypeAsync(eventType);
        }

        public async Task<bool> DeleteEventTypeAsync(string id)
        {
            return await repository.DeleteEventTypeAsync(id);
        }
    }
}