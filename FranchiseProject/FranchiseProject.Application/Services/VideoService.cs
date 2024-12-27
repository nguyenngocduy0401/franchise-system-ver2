using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.VideoViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Http;

namespace FranchiseProject.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly IFirebaseRepository _firebaseRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public VideoService(IFirebaseRepository firebaseRepository, IUnitOfWork unitOfWork) 
        {
            _firebaseRepository = firebaseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<VideoViewModel>>> GetAllVideoAsync()
        {
            var response = new ApiResponse<List<VideoViewModel>>();
            try
            { 

                var videos = await _unitOfWork.VideoRepository.GetAllAsync();

                var videoModels = videos.Select(video => new VideoViewModel
                {
                    Id = video.Id,
                    Name = video.Name,
                    Url = video.URL
                }).ToList();


                response = ResponseHandler.Success(videoModels);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<VideoViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteVideoAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var video = await _unitOfWork.VideoRepository.GetByIdAsync(id);
                if (video == null) throw new Exception("Video not found!");
                await _firebaseRepository.DeleteVideoAsync(video.URL);

                response = ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<VideoViewModel>> UploadVideoAsync(IFormFile videoFile, string name)
        {
            var response = new ApiResponse<VideoViewModel>();
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                {
                    response = ResponseHandler.Failure<VideoViewModel>("No file uploaded.");
                    return response;
                }

                
                var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{videoFile.FileName}");

                // Lưu tệp video vào tạm thời để phân tích
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(fileStream);
                }

                // Lấy thời lượng video bằng TagLib hoặc phương thức khác
                var duration = GetVideoDuration(tempFilePath);

                // Định dạng lại thời gian cho tên video
                var durationString = $"{(int)duration.TotalMinutes}:{duration.Seconds}";

                // Tạo ID duy nhất cho video
                var id = Guid.NewGuid();
                var uniqueVideoName = $"{name}_{id}_duration={durationString}";

                // Đọc lại tệp và upload lên Firebase
                using (var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
                {
                    var videoUrl = await _firebaseRepository.UploadVideoAsync(fileStream, uniqueVideoName);

                    // Tạo video object để lưu vào database
                    var video = new Video
                    {
                        Id = id,
                        Name = name,
                        URL = videoUrl
                    };

                    // Lưu video vào database
                    await _unitOfWork.VideoRepository.AddAsync(video);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                    if (!isSuccess) throw new Exception("Create failed!");

                    // Tạo VideoViewModel để trả về kết quả
                    var videoModel = new VideoViewModel
                    {
                        Id = id,
                        Name = name,
                        Url = videoUrl,
                    };

                    response = ResponseHandler.Success(videoModel);
                }

                // Xóa tệp tạm sau khi xử lý xong
                System.IO.File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi thất bại
                response = ResponseHandler.Failure<VideoViewModel>(ex.Message);
            }

            return response;
        }

        private TimeSpan GetVideoDuration(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found: " + filePath);

            var file = TagLib.File.Create(filePath);
            return file.Properties.Duration;
        }

    }

}
