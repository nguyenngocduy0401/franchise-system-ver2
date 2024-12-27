using Firebase.Storage;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;

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
        public async Task<string> UploadVideoAsync(Stream videoStream, string videoName)
        {
                var downloadUrl = await _firebaseStorage
                    .Child("videos") 
                    .Child(videoName)
                    .PutAsync(videoStream);
                return downloadUrl;
        }
        public async Task DeleteVideoAsync(string videoName)
        {
                await _firebaseStorage
                    .Child("videos")
                    .Child(videoName)
                    .DeleteAsync();
        }
    }
}
