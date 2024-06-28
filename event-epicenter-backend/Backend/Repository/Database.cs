using Google.Cloud.Firestore;

namespace Repository
{
    public class Database
    {
        public readonly FirestoreDb db = FirestoreDb.Create("event-epicenter");

        public async Task<List<Dictionary<string, object>>> GetAll(string collection)
        {
            CollectionReference collectionReference = db.Collection(collection);
            QuerySnapshot snapshot = await collectionReference.GetSnapshotAsync();

            List<Dictionary<string, object>> documentDictionary = new();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                documentDictionary.Add(document.ToDictionary());
            }

            return documentDictionary;
        }

        public async Task<Dictionary<string, object>> GetById(string collection, string id)
        {
            DocumentReference documentReference = db.Collection(collection).Document(id);
            DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();

            return snapshot.Exists ? snapshot.ToDictionary() : null;
        }

        public async Task<List<Dictionary<string, object>>> GetByIds(string collection, List<string> ids)
        {
            var entities = await GetAll(collection);

            return entities.Where(e => ids.Contains(e["id"].ToString())).ToList();
        }

        public async Task<bool> DeleteById(string collection, string id)
        {
            DocumentReference documentReference = db.Collection(collection).Document(id);

            var result = await documentReference.DeleteAsync();
            if (result != null)
            {
                return true;
            }

            return false;
        }   
    }
}
