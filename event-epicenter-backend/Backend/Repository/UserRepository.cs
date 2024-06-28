using Google.Cloud.Firestore;
using Model;

namespace Repository
{
    public class UserRepository
    {
        private readonly Database database;

        public UserRepository()
        {
            database = new Database();
        }

        public async Task<List<Dictionary<string, object>>> GetAllUsersAsync()
        {
            return await database.GetAll("user");
        }

        public async Task<Dictionary<string, object>> GetUserByIdAsync(string id)
        {
            return await database.GetById("user", id);
        }

        public async Task<bool> CreateUserAsync(User user) 
        {
            DocumentReference documentReference = database.db.Collection("user").Document(user.Id);
            var result = await documentReference.SetAsync(User.MapUserToDictionary(user));
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            DocumentReference documentReference = database.db.Collection("user").Document(user.Id);

            var result = await documentReference.SetAsync(User.MapUserToDictionary(user));
            if (result != null)
            {
                return true;
            }

            return false;
        }
    }
}