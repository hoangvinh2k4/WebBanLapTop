document.addEventListener("DOMContentLoaded", function () {
    if (!discounts || discounts.length === 0) {
        alert("Không có mã giảm giá khả dụng!");
        return;
    }

    const colors = ["#e74c3c", "#3498db", "#2ecc71", "#f1c40f", "#9b59b6", "#1abc9c", "#e67e22", "#34495e", "#ff6f61", "#16a085"];
    function randomColor(i) { return colors[i % colors.length]; }

    // ✅ Tạo các phần (segments) của vòng quay từ danh sách discounts
    const segments = discounts.map((d, i) => ({
        fillStyle: randomColor(i),
        text: d.Code + " - " + d.Percentage + "%"
    }));

    // ✅ Khởi tạo vòng quay
    const wheel = new Winwheel({
        canvasId: 'wheelCanvas',
        numSegments: segments.length,
        outerRadius: 220,
        textFontSize: 14,
        textAlignment: 'center',
        textFillStyle: '#fff',
        segments: segments,
        animation: {
            type: 'spinToStop',
            duration: 6,   // Thời gian quay (giây)
            spins: 8,      // Số vòng quay
            easing: 'Power4.easeOut' // Quay mượt (tăng tốc rồi chậm dần)
        }
    });

    // ✅ Thêm biến DOM
    const remainingSpinsEl = document.getElementById("remainingSpins");
    const spinBtn = document.getElementById("spinBtn");

    // ✅ Sự kiện khi bấm nút quay
    spinBtn.addEventListener("click", function () {
        if (!isLoggedIn) {
            window.location.href = "/Account/Login";
            return;
        }

        if (parseInt(remainingSpinsEl.innerText) <= 0) {
            showToast("⚠️ Bạn đã hết lượt quay hôm nay!");
            return;
        }

        // ✅ Gọi API Spin để lấy kết quả quay từ server
        $.post('/LuckyWheel/Spin', function (res) {
            if (res.error) {
                showToast(res.message);
                return;
            }

            const index = discounts.findIndex(d => d.Code === res.code);
            if (index === -1) {
                showToast("❌ Lỗi: Không tìm thấy voucher!");
                return;
            }

            // ✅ Reset bánh xe trước khi quay mới
            wheel.stopAnimation(false);
            wheel.rotationAngle = 0;
            wheel.draw();

            // ✅ Tính góc dừng của ô trúng thưởng
            const segmentAngle = 360 / wheel.numSegments;
            const stopAngle = (360 - (index * segmentAngle + segmentAngle / 2)) % 360;

            // ✅ Gán lại animation cho mỗi lần quay (tốc độ như nhau)
            wheel.animation = {
                type: 'spinToStop',
                duration: 6,   // Thời gian quay (giây)
                spins: 8,      // Số vòng quay
                stopAngle: stopAngle,
                easing: 'Power4.easeOut',
                callbackFinished: function () {
                    const result = discounts[index];
                    showToast("🎉 Bạn nhận được: " + result.Code + " - " + result.Percentage + "%");
                }
            };

            // ✅ Bắt đầu quay
            wheel.startAnimation();

            // ✅ Cập nhật lượt quay còn lại
            remainingSpinsEl.innerText = res.remainingSpins;
        });
    });

    // ✅ Hàm hiện thông báo (toast)
    function showToast(message) {
        const toast = document.getElementById("toast");
        toast.innerText = message;
        toast.className = "toast show";
        setTimeout(() => { toast.className = toast.className.replace("show", ""); }, 3000);
    }
});
