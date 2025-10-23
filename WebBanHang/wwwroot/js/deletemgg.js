document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll(".delete-form");

    forms.forEach(form => {
        form.addEventListener("submit", function (e) {
            e.preventDefault(); // Ngăn reload

            // **1. KIỂM TRA MGG ĐÃ HẾT HẠN TRƯỚC KHI HIỂN THỊ SWAL**
            const isExpiredInput = form.querySelector('input[name="is_expired"]');
            const isExpired = isExpiredInput && isExpiredInput.value === 'true';

            if (!isExpired) {
                // Nếu chưa hết hạn, hiển thị thông báo lỗi và ngăn xóa
                Swal.fire({
                    title: 'Không thể xoá!',
                    text: 'Chỉ có thể xoá mã giảm giá đã **hết hạn**.',
                    icon: 'error',
                    confirmButtonText: 'Đóng'
                });
                return; // Ngừng xử lý submit
            }

            // **2. HIỂN THỊ XÁC NHẬN XOÁ (CHỈ KHI ĐÃ HẾT HẠN)**
            Swal.fire({
                title: 'Bạn có chắc muốn xoá mã này không?',
                text: "Hành động này không thể hoàn tác!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Xoá',
                cancelButtonText: 'Huỷ'
            }).then((result) => {
                if (result.isConfirmed) {
                    // ✅ Gửi request bằng fetch (chỉ khi đã hết hạn và xác nhận)
                    fetch(form.action, {
                        method: 'POST',
                        body: new FormData(form)
                    }).then(res => {
                        if (res.ok) {
                            // Xử lý thành công
                            Swal.fire({
                                title: 'Đã xoá thành công!',
                                text: 'Mã giảm giá đã được xóa khỏi hệ thống.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            }).then(() => {
                                location.reload(); // Reload lại trang
                            });
                        } else {
                            // Xử lý lỗi từ server (ví dụ: server trả về 403/404/500)
                            throw new Error('Không thể xoá');
                        }
                    }).catch(err => {
                        // Xử lý lỗi mạng hoặc lỗi từ throw new Error
                        Swal.fire({
                            title: 'Lỗi!',
                            text: 'Đã xảy ra lỗi khi xoá mã giảm giá.',
                            icon: 'error',
                            confirmButtonText: 'Đóng'
                        });
                    });
                }
            });
        });
    });
});