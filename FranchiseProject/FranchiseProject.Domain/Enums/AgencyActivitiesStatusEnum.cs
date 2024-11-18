using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public  enum AgencyActivitiesStatusEnum
    {
        //------Procesing------------
        Interview,                   // Phỏng vấn -Manager
        AgreementSigned,             // Ký thỏa thuận 2 bên     -Manager
        BusinessRegistered,          // Đăng ký doanh nghiệp    - khách hàng đăng kí cho mình 
        SiteSurvey,                  // Khảo sát mặt bằng       -Technican        -Manger duyệt khảo sát
        Design,                      // Thiết kế                -Technican design -Manager duyệt design và báo giá
        Quotation,                   // báo giá cho khách hàng  -Manager & Technican
        SignedContract,              // Ký hơp đồng thành công  -Manager trực tiếp kí chụp lại gửi lên hệ thống
        //------Approved------------
        ConstructionAndTrainning,    // Đào tạo và thi công         -Technican(Thi Công) & SystemInstructor(Đào tạo) & Manager (Đào tạo)
        Handover,                    // Bàn Giao                    -Manager & Techinican
        EducationLicenseRegistered,  // Đăng ký giấy phép giáo dục   -Manager
        Finish,                      // hoàn thành
        //-----After-Frachise-------
        TrainningInternal,           //Đào tạo định kì
        RepairingEquipment,          //Sửa chữa thiết bị
        EducationalSupervision       //Giám sát hoạt động
    }
}
