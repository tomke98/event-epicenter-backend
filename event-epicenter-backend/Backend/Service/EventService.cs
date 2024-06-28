using Model;
using Repository;

namespace Service
{
    public class EventService
    {
        private readonly EventRepository repository;
        public EventService()
        {
            repository = new EventRepository();
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            var eventDictionaries = await repository.GetAllEventsAsync();

            var events = new List<Event>();

            foreach (var eventDictionary in eventDictionaries)
            {
                var _event = Event.MapDictionaryToEvent(eventDictionary);

                _event.EventType = await new EventTypeService().GetEventTypeByIdAsync(_event.EventTypeId);

                var attendaces = await new AttendanceService().GetAttendanceByEventIdAsync(_event.Id);
                var userDictionaries = await new UserRepository().GetAllUsersAsync();
                var users = new List<User>();
                foreach (var userDictionary in userDictionaries)
                {
                    users.Add(User.MapDictionaryToUser(userDictionary));
                }
                _event.Users = users.Where(u => attendaces.Any(a => a.UserId == u.Id)).ToList();

                events.Add(_event);
            }

            return events;
        }

        public async Task<Event> GetEventByIdAsync(string id)
        {
            var eventDictionary = await repository.GetEventByIdAsync(id);
            if(eventDictionary == null)
            {
                return null;
            }

            var _event = Event.MapDictionaryToEvent(eventDictionary);

            _event.EventType = await new EventTypeService().GetEventTypeByIdAsync(_event.EventTypeId);

            var attendaces = await new AttendanceService().GetAttendanceByEventIdAsync(_event.Id);
            var userDictionaries = await new UserRepository().GetAllUsersAsync();
            var users = new List<User>();
            foreach (var userDictionary in userDictionaries)
            {
                users.Add(User.MapDictionaryToUser(userDictionary));
            }
            _event.Users = users.Where(u => attendaces.Any(a => a.UserId == u.Id)).ToList();

            return _event;
        }

        public async Task<bool> CreateEventAsync(Event _event)
        {
            return await repository.CreateEventAsync(_event);
        }

        public async Task<bool> UpdateEventAsync(Event _event)
        {
            return await repository.UpdateEventAsync(_event);
        }

        public async Task<bool> DeleteEventAsync(string id)
        {
            return await repository.DeleteEventAsync(id);
        }
    }
}