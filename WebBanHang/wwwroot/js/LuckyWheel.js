// Khi toàn bộ nội dung HTML đã tải xong
document.addEventListener("DOMContentLoaded", function () {

    // Nếu không có danh sách mã giảm giá, hiển thị cảnh báo và dừng
    if (!discounts || discounts.length === 0) {
        alert("Không có mã giảm giá khả dụng!");
        return;
    }

    // 🎨 Danh sách màu để tô cho các ô trên bánh xe
    const colors = ["#e74c3c", "#3498db", "#2ecc71", "#f1c40f", "#9b59b6", "#1abc9c", "#e67e22", "#34495e", "#ff6f61", "#16a085"];

    // Hàm chọn màu theo thứ tự (dựa vào chỉ số i)
    function randomColor(i) {
        return colors[i % colors.length];
    }

    // ====== 🔧 Biến toàn cục ======
    let currentDiscounts = [...discounts]; // clone danh sách mã giảm giá
    let wheel = null; // đối tượng bánh xe Winwheel

    // ====== 🎡 Hàm khởi tạo vòng quay ======
    function createWheel(discountList) {
        if (!discountList || discountList.length === 0) return null;

        // Chuyển mỗi discount thành một segment (một ô trên bánh xe)
        const segments = discountList.map((d, i) => ({
            fillStyle: randomColor(i),                // màu nền
            text: d.Code + " - " + d.Percentage + "%" // nội dung hiển thị
        }));

        // Tạo đối tượng Winwheel (thư viện vẽ bánh xe)
        return new Winwheel({
            canvasId: 'wheelCanvas',   // ID của thẻ <canvas> trong HTML
            numSegments: segments.length, // số lượng ô
            outerRadius: 220,          // bán kính bánh xe
            textFontSize: 14,
            textAlignment: 'center',
            textFillStyle: '#fff',
            segments: segments,        // danh sách các ô
            animation: {               // cấu hình hiệu ứng quay
                type: 'spinToStop',    // quay và dừng lại
                duration: 6,           // thời gian quay (giây)
                spins: 8,              // số vòng quay
                easing: 'Power4.easeOut' // kiểu giảm tốc
            }
        });
    }

    // 🌀 Khởi tạo bánh xe lần đầu tiên
    wheel = createWheel(currentDiscounts);

    // Các phần tử HTML liên quan
    const remainingSpinsEl = document.getElementById("remainingSpins");
    const spinBtn = document.getElementById("spinBtn");

    // ====== 🎯 Xử lý khi nhấn nút "Quay" ======
    spinBtn.addEventListener("click", function () {

        // Nếu chưa đăng nhập → chuyển sang trang Login
        if (!isLoggedIn) {
            window.location.href = "/Account/Login";
            return;
        }

        // Kiểm tra lượt quay còn lại
        if (parseInt(remainingSpinsEl.innerText) <= 0) {
            showToast("⚠️ Bạn đã hết lượt quay hôm nay!");
            return;
        }

        // Nếu hết mã
        if (!currentDiscounts || currentDiscounts.length === 0) {
            setTimeout(() => { toast.className = toast.className.replace("show", ""); }, 3000);
        }
    });