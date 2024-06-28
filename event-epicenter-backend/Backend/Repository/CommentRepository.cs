using Google.Cloud.Firestore;
using Model;

namespace Repository
{
    public class CommentRepository
    {
        private readonly Database database;

        public CommentRepository()
        {
            database = new Database();
        }

        public async Task<List<Dictionary<string, object>>> GetAllCommentsAsync()
        {
            return await database.GetAll("comment");
        }

        public async Task<Dictionary<string, object>> GetCommentByIdAsync(string id)
        {
            return await database.GetById("comment", id);
        }

        public async Task<bool> CreateCommentAsync(Comment comment) 
        {
            DocumentReference documentReference = database.db.Collection("comment").Document();
            comment.Id = documentReference.Id;

            var result = await documentReference.SetAsync(Comment.MapCommentToDictionary(comment));
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            DocumentReference documentReference = database.db.Collection("comment").Document(comment.Id);

            var result = await documentReference.SetAsync(Comment.MapCommentToDictionary(comment));
            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteCommentAsync(string id)
        {
            return await database.DeleteById("comment", id);
        }
    }
}