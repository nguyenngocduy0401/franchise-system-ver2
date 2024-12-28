using Firebase.Storage;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class FirebaseRepository : IFirebaseRepository
    {
        private readonly FirebaseStorage _firebaseStorage;
        private string bucket = "futuretech-b367a.appspot.com";
        public FirebaseRepository()
        {
            _firebaseStorage = new FirebaseStorage(bucket);
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
        public async Task DeleteVideoAsync(string fullURL)
        {
            try
            {
                
                var filePath = ExtractFilePathFromURL(fullURL);
                await _firebaseStorage
                    .Child(filePath)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting video: {ex.Message}");
            }
        }

        private string ExtractFilePathFromURL(string url)
        {
            // Define the base URL part to remove from the full URL
            var baseUrl = "https://firebasestorage.googleapis.com/v0/b/" + bucket + "/o/";

            // Remove the base URL part from the provided URL
            var filePath = url.Replace(baseUrl, "");

            // Remove any query parameters after the '?' symbol
            var filePathWithoutQuery = filePath.Split('?')[0];

            // URL decode the file path (to convert %2F to '/', etc.)
            var decodedFilePath = Uri.UnescapeDataString(filePathWithoutQuery);

            return decodedFilePath;
        }
    }
}
