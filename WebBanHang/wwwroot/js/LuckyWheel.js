document.addEventListener("DOMContentLoaded", function () {
    if (!discounts || discounts.length === 0) {
        alert("Không có mã giảm giá khả dụng!");
        return;
    }

    const colors = ["#e74c3c", "#3498db", "#2ecc71", "#f1c40f", "#9b59b6", "#1abc9c", "#e67e22", "#34495e", "#ff6f61", "#16a085"];
    function randomColor(i) { return colors[i % colors.length]; }

    // Tạo segments từ discounts
    const segments = discounts.map((d, i) => ({
        fillStyle: randomColor(i),
        text: d.Code + " - " + d.Percentage + "%"
    }));

    // Khởi tạo vòng quay
    const wheel = new Winwheel({
        canvasId: 'wheelCanvas',
        numSegments: segments.length,
        outerRadius: 220,
        textFontSize: 14,
        textAlignment: 'center',
        textFillStyle: '#fff',
        segments: segments,
        animation: { type: 'spinToStop', duration: 6, spins: 10 }
    });

    // Thêm span hiển thị lượt còn lại
    const remainingSpinsEl = document.getElementById("remainingSpins");

    // Nút quay
    const spinBtn = document.getElementById("spinBtn");
    spinBtn.addEventListener("click", function () {
        if (!isLoggedIn) {
            window.location.href = "/Account/Login";
            return;
        }

        if (parseInt(remainingSpinsEl.innerText) <= 0) {
            showToast("⚠️ Bạn đã hết lượt quay hôm nay!");
            return;
        }

        // Gọi API Spin để lấy kết quả và giảm lượt
        $.post('/LuckyWheel/Spin', function (res) {
            if (res.error) {
                showToast(res.message);
                return;
            }

            // Start animation
            const index = Math.floor(Math.random() * discounts.length);
            const segmentAngle = 360 / wheel.numSegments;
            const stopAngle = (index * segmentAngle + segmentAngle / 2) % 360;

            wheel.animation.stopAngle = stopAngle;
            wheel.animation.callbackFinished = function () {
                const result = discounts[index];
                showToast("🎉 Bạn nhận được: " + result.Code + " - " + result.Percentage + "%");
            };

            wheel.startAnimation();

            // Cập nhật lượt còn lại
            remainingSpinsEl.innerText = res.remainingSpins;
        });
    });

    function showToast(message) {
        const toast = document.getElementById("toast");
        toast.innerText = message;
        toast.className = "toast show";
        setTimeout(() => { toast.className = toast.className.replace("show", ""); }, 3000);
    }
});
