$(document).ready(function () {
    $(".add-to-cart").click(function (e) {
        e.preventDefault();

        var id = $(this).data("id");

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { id: id },
            success: function (res) {
                // Cập nhật giỏ hàng nhỏ (header)
                $("#cart-count").text(res.totalItems);
                $("#cart-total").text(res.totalPrice.toLocaleString() + " VNĐ");

                // Hiển thị thông báo thành công bằng SweetAlert2
                Swal.fire({
                    icon: 'success',
                    title: 'Thêm sản phẩm thành công!',
                    text: `Sản phẩm đã được thêm vào giỏ hàng.`,
                    showConfirmButton: false,
                    timer: 1500, // tự ẩn sau 1.5 giây
                    position: 'top',
                    toast: true
                });
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Có lỗi!',
                    text: 'Không thể thêm sản phẩm vào giỏ hàng.',
                    timer: 1500,
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false
                });
            }
        });
    });
});
