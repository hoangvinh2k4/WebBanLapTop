$(document).on("click", ".add-to-cart", function (e) {
    e.preventDefault();

    var id = $(this).data("id");

    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { id: id },
        success: function (res) {
            $("#cart-count").text(res.totalItems);
            $("#cart-total").text(res.totalPrice.toLocaleString() + " VNĐ");

            Swal.fire({
                icon: 'success',
                title: 'Thêm sản phẩm thành công!',
                text: `Sản phẩm đã được thêm vào giỏ hàng.`,
                showConfirmButton: false,
                timer: 1500,
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

$(document).on("click", ".add-to-wishlist", function (e) {
    e.preventDefault();

    var id = $(this).data("id");

    $.ajax({
        url: '/WishList/AddToWishList',
        type: 'POST',
        data: { id: id },
        success: function (res) {
            Swal.fire({
                icon: 'success',
                title: 'Thêm sản phẩm thành công!',
                text: `Sản phẩm đã được thêm vào danh sách yêu thích.`,
                showConfirmButton: false,
                timer: 1500,
                position: 'top',
                toast: true
            });           
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Có lỗi!',
                text: 'Không thể thêm sản phẩm vào danh sách yêu thích.',
                timer: 1500,
                toast: true,
                position: 'top-end',
                showConfirmButton: false
            });
        }
    });
});



















