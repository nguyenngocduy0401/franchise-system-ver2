using Azure;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Services
{
    public class VimeoService : IVimeoService
    {
        private readonly AppConfiguration _appConfiguration;

        public VimeoService(AppConfiguration appConfiguration) 
        {
            _appConfiguration = appConfiguration; 
        }

        public async Task<ApiResponse<string>> UploadVideo(IFormFile videoFile)
        {
            var response = new ApiResponse<string>();
            var clientId = _appConfiguration.VimeoConfiguration.ClientId;
            var clientSecret = _appConfiguration.VimeoConfiguration.ClientSecret;
            var accessToken = _appConfiguration.VimeoConfiguration.AccessToken;

            if (videoFile == null || videoFile.Length == 0)
                return ResponseHandler.Failure<string>("File không hợp lệ.");

            try
            {
                // 1. Tạo URL tải lên từ Vimeo API
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Tạo request để tạo upload URL
                    var createUploadRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.vimeo.com/me/videos")
                    {
                        Content = new StringContent($"{{\"upload\": {{\"approach\": \"tus\", \"size\": {videoFile.Length}}}}}")
                    };
                    createUploadRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Gửi request tạo URL upload
                    var createUploadResponse = await httpClient.SendAsync(createUploadRequest);

                    // Check if the response is not successful
                    if (!createUploadResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await createUploadResponse.Content.ReadAsStringAsync();
                        return ResponseHandler.Failure<string>($"Lỗi khi tạo upload URL: {createUploadResponse.StatusCode} - {errorContent}");
                    }

                    // Phân tích JSON response
                    var uploadResponseJson = await createUploadResponse.Content.ReadAsStringAsync();
                    var uploadData = JObject.Parse(uploadResponseJson);
                    var uploadLink = uploadData["upload"]["upload_link"].ToString();
                    var videoUrl = uploadData["link"].ToString();

                    // 2. Tải file video lên sử dụng URL tải lên
                    using (var uploadClient = new HttpClient())
                    {
                        using (var fileStream = videoFile.OpenReadStream())
                        {
                            var tusRequest = new HttpRequestMessage(HttpMethod.Patch, uploadLink)
                            {
                                Content = new StreamContent(fileStream)
                            };
                            tusRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/offset+octet-stream");
                            tusRequest.Content.Headers.ContentLength = videoFile.Length;

                            // Sending the upload request to Vimeo
                            var uploadResponse = await uploadClient.SendAsync(tusRequest);

                            // Check if upload was successful
                            if (!uploadResponse.IsSuccessStatusCode)
                            {
                                var uploadErrorContent = await uploadResponse.Content.ReadAsStringAsync();
                                return ResponseHandler.Failure<string>($"Lỗi khi tải video: {uploadResponse.StatusCode} - {uploadErrorContent}");
                            }
                        }
                    }

                    // 3. Trả về kết quả thành công
                    return ResponseHandler.Success(videoUrl, "Tải video thành công!");
                }
            }
            catch (HttpRequestException ex)
            {
                return ResponseHandler.Failure<string>($"Lỗi khi tải video: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"Lỗi không xác định: {ex.Message}");
            }
        }
    }
}
