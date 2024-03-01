using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class MessagesResponse
    {
        public static class Success
        {
            public const string Completed = "Hoạt động hoàn tất thành công.";
            public const string Created = "Tạo thành công.";
            public const string Updated = "Cập nhật thành công.";
            public const string Deleted = "Xóa thành công.";
            public const string Uploaded = "Tải lên thành công.";
            public const string Approved = "Duyệt thành công.";
            public const string SignedOut = "Đăng xuất thành công.";
            public const string RegisterSuccess = "Đăng ký thành công.";
        }

        public static class Error
        {

            public const string InvalidInput = "Đầu vào không hợp lệ. Vui lòng kiểm tra yêu cầu của bạn.";
            public const string NotFound = "Không tìm thấy tài nguyên.";
            public const string OperationFailed = "Thao tác thất bại. Vui lòng thử lại.";
            public const string Unauthorized = "Truy cập không được ủy quyền.";
            public const string DuplicateResource = "Tài nguyên đã tồn tại.";
            public const string LoginFailed = "Đăng nhập thất bại.";
            public const string RegisterFailed = "Đăng ký thất bại.";
            public const string FileNotFound = "Không tìm thấy tệp.";
            public const string BookingNotFound = "Không có booking nào được tìm thấy.";
            public const string DatabaseConnectionFailed = "Kết nối cơ sở dữ liệu thất bại.";
            public const string UnableToCreatePaymentLink = "Không thể tạo liên kết thanh toán.";
            public const string MedicalReportExists = "Báo cáo y tế có cùng tên đã tồn tại.";
            public const string UserEmailNotFound = "Không thể tìm thấy email người dùng.";
            public const string AddressExists = "Địa chỉ có cùng tên đã tồn tại.";
            public const string EmailAlreadyExists = "Email đã tồn tại trong hệ thống.";
            public const string PasswordMismatch = "Xác nhận mật khẩu và mật khẩu không khớp.";
            public const string BannedAccount = "Tài khoản đã bị cấm.";
            public const string TokenNotFound = "Mã thông báo không tìm thấy.";
            public const string TokenGenerationFailed = "Quá trình tạo mã thông báo thất bại.";
            public const string RefreshTokenGenerationFailed = "Quá trình tạo mã làm mới thất bại.";
            public const string AccessTokenNotFound = " AccessToken không tìm thấy.";
            public const string RefreshTokenNotFound = "RefreshToken không tìm thấy.";
        }

        public static class Validation
        {
            public const string FieldIsRequired = "Trường {0} là bắt buộc.";
            public const string InvalidEmailFormat = "Định dạng email không hợp lệ.";
            public const string PasswordRequired = "Mật khẩu là bắt buộc.";
            public const string EmailRequired = "Email là bắt buộc.";
            public const string EmailNotFound = "Không tìm thấy email.";
            public const string InvalidCredentials = "Email hoặc mật khẩu không hợp lệ.";
            public const string TokenInvalid = "Token không hợp lệ.";
        }
    }
}
