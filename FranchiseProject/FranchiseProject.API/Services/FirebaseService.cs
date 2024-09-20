using Firebase.Storage;
using FranchiseProject.Application.Interfaces;

namespace FranchiseProject.API.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseStorage _firebaseStorage;

        public FirebaseService()
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
