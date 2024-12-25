﻿using FranchiseProject.Application.ViewModels.EmailViewModels;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Utils
{
    public static class EmailTemplate
    {
        #region AgencyEmail
        public static MessageModel AgencyRegistrationSuccess(string to, string name)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Đăng ký thành công [futuretech-noreply]",
                Body = $"<p>Chào {name},</p>" +
                       $"<p>Cảm ơn bạn đã đăng ký trở thành đối tác của chúng tôi.</p>" +
                       $"<p>Thông tin của bạn đã được ghi nhận thành công.</p>" +
                       $"<p>Trân trọng,</p>" +
                       $"<p>Đội ngũ Futuretech</p>"
            };
        }

        public static MessageModel FailureNotification(string to, string errorMessage)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Thông báo lỗi [futuretech-noreply]",
                Body = $"<p>Chào bạn,</p>" +
                       $"<p>Đã xảy ra lỗi: {errorMessage}</p>" +
                       $"<p>Vui lòng thử lại sau hoặc liên hệ với chúng tôi.</p>" +
                       $"<p>Trân trọng,</p>" +
                       $"<p>Đội ngũ Futuretech</p>"
            };
        }
        #endregion
        #region RegisterCourseEmail
        public static MessageModel SuccessRegisterCourseEmaill(string to, string name, string courseName, string username, string password, decimal amount)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Đăng ký và thanh toán khóa học thành công [futuretech-noreply]",
                Body = $@"<p>Chào bạn {name},</p>
                  <p>Cảm ơn bạn đã đăng ký và thanh toán thành công khóa học <strong>{courseName}</strong>.</p>
                  <p>Thông tin thanh toán:</p>
                  <ul>
                    <li>Số tiền đã thanh toán: {amount:N0} VNĐ</li>
                  </ul>
                  <p>Thông tin tài khoản của bạn:</p>
                  <ul>
                    <li><strong>Tên đăng nhập:</strong> {username}</li>
                    <li><strong>Mật khẩu:</strong> {password}</li>
                  </ul>
                  <p>Vui lòng bảo mật thông tin đăng nhập này và đổi mật khẩu sau khi đăng nhập lần đầu.</p>
                  <p>Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất để cung cấp thêm thông tin về lớp học.</p>
                  <p>Trân trọng,</p>
                  <p>Đội ngũ FutureTech</p>"
            };
        }
        public static MessageModel StudentPaymentSuccsess(string to, string name, int amount, string UserName, string password)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Xác Nhận Thanh Toán Thành Công [futuretech-noreply]",
                Body = $"<p>Chào bạn {name},</p>" +
                       $"<p>Bạn đã thanh toán khóa học thành công </p>" +
                       $"<p>Số tiền đã thanh toán:{amount} VNĐ </p>" +
                       $"<li><strong>Username:</strong> {UserName}</li>" +
                       $"<li><strong>Password:</strong> {password}</li>" +
                       $"</ul>" +
                       $"<p>Vui lòng bảo mật thông tin đăng nhập này.</p>" +
                       $"<p>Chúng tôi sẽ liên hệ và gửi thông báo về thông tin lớp học trong thời gian sớm nhất </p>" +
                       $"<p>Trân trọng,</p>" +
                       $"<p>Đội ngũ FutureTech</p>"
            };
        }
        public static MessageModel StudentPaymentSuccsessNotCompleted(string to, string name, int amount, string agencyName)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Xác Nhận Thanh Toán Thành Công [futuretech-noreply]",
                Body = $"<p>Chào bạn {name},</p>" +
                       $"<p>Bạn đã đăng Thanh khóa học  thành công </p>" +
                       $"<p>Số tiền đã thanh toán:{amount} VNĐ </p>" +
                       $"<p>Chúng tôi sẽ liên hệ và gửi thông báo về thông tin lớp học trong thời gian sớm nhất </p>" +
                       $"<p>Trân trọng,</p>" +
                       $"<p>Đội ngũ FutureTech</p>"
            };
        }
        #endregion
        #region ClassSchedule
        public static MessageModel ClassScheduleChange(string to, string name, string ClassName)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Lịch Học Thay Đổi [futuretech-noreply]",
                Body = $"<p>Chào bạn {name},</p>" +
                       $"<p>Lịch học của lớp {ClassName} đã thay đổi  </p>" +
                       $"<p>chúng tôi sẽ thông tin đến bạn lịch học của Lớp {ClassName}  trong thời gian sớm nhất  </p>" +
                       $"<p>Trân trọng,</p>" +
                       $"<p>Đội ngũ FutureTech</p>"
            };
        }
        public static MessageModel ClassScheduleCreated(string to, string name, string ClassName)
        {
            return new MessageModel
            {
                To = to,
                Subject = "Thông báo lịch học  [futuretech-noreply]",
                Body = $"<p>Chào bạn {name},</p>" +
                       $"<p>Lịch học của lớp {ClassName} đã có! </p>" +
                       $"<p>Hãy truy cập vào hệ thông để xem thông tin  lịch học của Lớp {ClassName}.</p>" +
                       $"<p>Trân trọng,</p>" +
                       $"<p>Đội ngũ FutureTech</p>"
            };
        }
        #endregion
        #region Assignment

        #endregion
    }
}
