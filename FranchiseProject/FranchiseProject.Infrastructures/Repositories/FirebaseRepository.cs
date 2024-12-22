using Firebase.Storage;
using FranchiseProject.Application.Repositories;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class FirebaseRepository : IFirebaseRepository
    {
        private readonly FirebaseStorage _firebaseStorage;

        public FirebaseRepository()
        {
            _firebaseStorage = new FirebaseStorage("futuretech-b367a.appspot.com");
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var task = await _firebaseStorage
                .Child("contracts")
                .Child(fileName)
                .PutAsync(fileStream);

            return task;
        }
    }
}
