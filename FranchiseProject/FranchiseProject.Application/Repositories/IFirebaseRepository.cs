using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IFirebaseRepository
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
        Task<string> UploadVideoAsync(Stream videoStream, string videoName);
        Task DeleteVideoAsync(string videoName);
    }
}
