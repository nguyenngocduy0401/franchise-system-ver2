using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum WorkTypeEnum
    {
        //------Procesing------------
        Interview,                   // Phỏng vấn
        AgreementSigned,             // Ký thỏa thuận 2 bên
        BusinessRegistered,          // Đăng ký doanh nghiệp
        SiteSurvey,                  // Khảo sát mặt bằng
        Design,                      // Thiết kế
        Quotation,                   // báo giá cho khách hàng
        SignedContract,              // Ký hơp đồng thành công
        //------Approved-------------
        ConstructionAndTrainning,    // Đào tạo và thi công
        Handover,                    // Bàn Giao     
        EducationLicenseRegistered,  // Đăng ký giấy phép giáo dục
        //------AfterFranchise-------
        TrainningInternal,    // Đào tạo tạo định kì
        RepairingEquipment,                    // Sửa chữa thiết bị
        EducationalSupervision,  // Giám sát hoạt động giáo dục
    }
}
