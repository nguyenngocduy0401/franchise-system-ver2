using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.HomePageViewModels
{
    public class HomePageViewModel
    {
        public Guid? Id { get; set; }
        public string? FranchiseTitle { get; set; }  // Tiêu đề của trang chủ
        public string? FranchiseDescription { get; set; }  // Mô tả ngắn gọn
        public string? FranchiseBannerImageUrl { get; set; }  // URL của hình ảnh banner
        public string? FranchiseMainContent { get; set; }  // Nội dung chính trên trang chủ (HTML, Markdown, v.v.)
        public string? CourseTitle { get; set; }  // Tiêu đề của trang chủ
        public string? CourseDescription { get; set; }  // Mô tả ngắn gọn
        public string? CourseBannerImageUrl { get; set; }  // URL của hình ảnh banner
        public string? CourseMainContent { get; set; }  // Nội dung chính trên trang chủ (HTML, Markdown, v.v.)
        public string? ContactEmail { get; set; }  // Email liên hệ
        public double? FeeAmount { get; set; }  // Phí nhượng quyền
        public string? PhoneNumber { get; set; }  // Số điện thoại liên hệ
    }
}
