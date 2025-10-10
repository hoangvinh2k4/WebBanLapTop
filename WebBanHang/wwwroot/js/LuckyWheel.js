document.addEventListener("DOMContentLoaded", function () {
    if (!discounts || discounts.length === 0) {
        alert("Không có mã giảm giá khả dụng!");
        return;
    }

    const colors = ["#e74c3c", "#3498db", "#2ecc71", "#f1c40f", "#9b59b6", "#1abc9c", "#e67e22", "#34495e", "#ff6f61", "#16a085"];
    function randomColor(i) { return colors[i % colors.length]; }

    // Biến toàn cục (trong hàm DOMContentLoaded) để theo dõi các segments hiện tại
    let currentDiscounts = [...discounts]; // Sao chép danh sách ban đầu
    let wheel = null; // Biến wheel được khởi tạo sau

    // ✅ HÀM TẠO VÀ VẼ LẠI VÒNG QUAY
    function createWheel(discountList) {
        const segments = discountList.map((d, i) => ({
            fillStyle: randomColor(i),
            text: d.Code + " - " + d.Percentage + "%"
        }));

        // Nếu wheel đã tồn tại, xóa nó đi trước khi tạo lại (Tránh trùng Canvas ID)
        if (wheel) {
            wheel.deleteSegment();
        }

        return new Winwheel({
            canvasId: 'wheelCanvas',
            numSegments: segments.length,
            outerRadius: 220,
            textFontSize: 14,
            textAlignment: 'center',
            textFillStyle: '#fff',
            segments: segments,
            animation: {
                type: 'spinToStop',
                duration: 6,    // Thời gian quay (giây)
                spins: 8,       // Số vòng quay
                easing: 'Power4.easeOut' // Quay mượt (tăng tốc rồi chậm dần)
            }
        });
    }

    // ✅ Khởi tạo vòng quay lần đầu
    wheel = createWheel(currentDiscounts);

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

        // Kiểm tra số lượng segment hiện tại
        if (currentDiscounts.length === 0) {
            showToast("⚠️ Không còn mã giảm giá nào để quay nữa!");
            return;
        }

        // Tắt nút quay trong khi gọi API
        spinBtn.disabled = true;

        // ✅ Gọi API Spin để lấy kết quả quay từ server
        $.post('/LuckyWheel/Spin', function (res) {
            if (res.error) {
                showToast(res.message);
                spinBtn.disabled = false;
                return;
            }

            const winningCode = res.code;
            const index = currentDiscounts.findIndex(d => d.Code === winningCode);

            if (index === -1) {
                showToast("❌ Lỗi: Không tìm thấy voucher trong danh sách hiện tại!");
                spinBtn.disabled = false;
                return;
            }

            // ✅ Reset bánh xe trước khi quay mới
            wheel.stopAnimation(false);
            wheel.rotationAngle = 0;
            wheel.draw();

            // Lấy segment angle dựa trên currentDiscounts
            const segmentAngle = 360 / wheel.numSegments;
            const stopAngle = (360 - (index * segmentAngle + segmentAngle / 2)) % 360;

            // ✅ Gán lại animation cho mỗi lần quay
            wheel.animation.stopAngle = stopAngle;
            wheel.animation.callbackFinished = function () {
                const result = currentDiscounts[index];
                showToast("🎉 Bạn nhận được: " + result.Code + " - " + result.Percentage + "%");

                // 1. Loại bỏ phần thưởng khỏi danh sách hiện tại
                currentDiscounts.splice(index, 1);

                // 2. Tái tạo vòng quay với danh sách mới
                wheel = createWheel(currentDiscounts);

                // 3. Cho phép quay lại
                spinBtn.disabled = false;
            };

            // ✅ Bắt đầu quay
            wheel.startAnimation();

            // ✅ Cập nhật lượt quay còn lại
            remainingSpinsEl.innerText = res.remainingSpins;
        }).fail(function () {
            showToast("❌ Lỗi kết nối đến Server!");
            spinBtn.disabled = false;
        });
    });

    // ✅ Hàm hiện thông báo (toast)
    function showToast(message) {
        const toast = document.getElementById("toast");
        if (!toast) return;
        toast.innerText = message;
        toast.className = "toast show";
        setTimeout(() => { toast.className = toast.className.replace("show", ""); }, 3000);
    }
});