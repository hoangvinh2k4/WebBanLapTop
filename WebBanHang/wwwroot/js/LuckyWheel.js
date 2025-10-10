document.addEventListener("DOMContentLoaded", function () {
    if (!discounts || discounts.length === 0) {
        alert("Không có mã giảm giá khả dụng!");
        return;
    }

    const colors = ["#e74c3c", "#3498db", "#2ecc71", "#f1c40f", "#9b59b6", "#1abc9c", "#e67e22", "#34495e", "#ff6f61", "#16a085"];
    function randomColor(i) { return colors[i % colors.length]; }

    // Biến toàn cục
    let currentDiscounts = [...discounts];
    let wheel = null;

    function createWheel(discountList) {
        if (!discountList || discountList.length === 0) return null;

        const segments = discountList.map((d, i) => ({
            fillStyle: randomColor(i),
            text: d.Code + " - " + d.Percentage + "%"
        }));

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
                duration: 6,
                spins: 8,
                easing: 'Power4.easeOut'
            }
        });
    }

    // Khởi tạo wheel lần đầu
    wheel = createWheel(currentDiscounts);

    const remainingSpinsEl = document.getElementById("remainingSpins");
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

        if (!currentDiscounts || currentDiscounts.length === 0) {
            showToast("⚠️ Không còn mã giảm giá nào để quay nữa!");
            return;
        }

        spinBtn.disabled = true;

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

            // Reset bánh xe trước khi quay mới
            if (wheel) {
                wheel.stopAnimation(false);
                wheel.rotationAngle = 0;
                wheel.draw();
            }

            const segmentAngle = 360 / wheel.numSegments;
            const stopAngle = (360 - (index * segmentAngle + segmentAngle / 2)) % 360;

            wheel.animation.stopAngle = stopAngle;
            wheel.animation.callbackFinished = function () {
                const result = currentDiscounts[index];
                showToast("🎉 Bạn nhận được: " + result.Code + " - " + result.Percentage + "%");

                // Loại bỏ phần thưởng khỏi danh sách
                currentDiscounts.splice(index, 1);

                // Tái tạo wheel với danh sách mới
                wheel = createWheel(currentDiscounts);

                spinBtn.disabled = false;
            };

            wheel.startAnimation();
            remainingSpinsEl.innerText = res.remainingSpins;

        }).fail(function () {
            showToast("❌ Lỗi kết nối đến Server!");
            spinBtn.disabled = false;
        });
    });

    function showToast(message) {
        const toast = document.getElementById("toast");
        if (!toast) return;
        toast.innerText = message;
        toast.className = "toast show";
        setTimeout(() => { toast.className = toast.className.replace("show", ""); }, 3000);
    }
});