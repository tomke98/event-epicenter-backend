using Model;
using Repository;

namespace Service
{
    public class UserService
    {
        private readonly UserRepository repository;
        public UserService()
        {
            repository = new UserRepository();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var userDictionaries = await repository.GetAllUsersAsync();

            var users = new List<User>();

            foreach (var userDictionary in userDictionaries)
            {
                var user = User.MapDictionaryToUser(userDictionary);
                var eventTypeDictionaries = await new EventTypeRepository().GetEventTypesByIdsAsync(user.EventTypeIds);

                var eventTypes = new List<EventType>();
                foreach (var eventTypeDictionary in eventTypeDictionaries)
                {
                    eventTypes.Add(EventType.MapDictionaryToEventType(eventTypeDictionary));
                }
                user.EventTypes = eventTypes;

                var attendaces = await new AttendanceService().GetAttendanceByUserIdAsync(user.Id);

                var eventDictionaries = await new EventRepository().GetAllEventsAsync();
                var events = new List<Event>();
                foreach (var eventDictionary in eventDictionaries)
                {
                    events.Add(Event.MapDictionaryToEvent(eventDictionary));
                }
                user.Events = events.Where(e => attendaces.Any(a => a.EventId == e.Id)).ToList();

                users.Add(user);
            }

            return users;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var userDictionary = await repository.GetUserByIdAsync(id);
            var user = userDictionary == null ? null : User.MapDictionaryToUser(userDictionary);
            var eventTypeDictionaries = await new EventTypeRepository().GetEventTypesByIdsAsync(user.EventTypeIds);

            var eventTypes = new List<EventType>();
            foreach (var eventTypeDictionary in eventTypeDictionaries)
            {
                eventTypes.Add(EventType.MapDictionaryToEventType(eventTypeDictionary));
            }
            user.EventTypes = eventTypes;

            var attendaces = await new AttendanceService().GetAttendanceByUserIdAsync(user.Id);
            var eventDictionaries = await new EventRepository().GetAllEventsAsync();
            var events = new List<Event>();
            foreach (var eventDictionary in eventDictionaries)
            {
                events.Add(Event.MapDictionaryToEvent(eventDictionary));
            }
            user.Events = events.Where(e => attendaces.Any(a => a.EventId == e.Id)).ToList();

            return user;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            return await repository.CreateUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await repository.UpdateUserAsync(user);
        }
    }
}