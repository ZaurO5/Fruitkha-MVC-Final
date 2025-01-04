$(function () {
    $('.addToBasket').on("click", function () {
        console.log("clicked")
        $.ajax({
            method: "POST",
            url: "/basket/addproduct",
            data: {
                productId: $(this).data('id')
            },
            success: function (response) {
                alert(response)
            },
            error: function (xhr) {
                alert(xhr.responseText)
            }
        })
    })

    $(".updateCart").on("click", function () {

        updatedProducts = []

        $(".product-count").each(function () {
            updatedProducts.push({
                ProductId: $(this).data("id"),
                Count: $(this).val()
            });
        });

        console.log(updatedProducts)

        $.ajax({
            method: "POST",
            url: "/Basket/UpdateCart",
            data: JSON.stringify(updatedProducts),
            contentType: "application/json",
            success: function (response) {
                alert(response);
                location.reload();
            },
            error: function (xhr) {
                alert(xhr.responseText)
                location.reload();
            }
        })
    })

    $('.deleteProduct').on("click", function () {
        var btn = $(this)
        var basketProductId = $(this).data('id')

        console.log(btn)

        $.ajax({
            method: "POST",
            url: "/Basket/Delete",
            data: {
                basketProductId: $(this).data('id')
            },
            success: function (response) {
                $(btn).parent().parent().remove()
                location.reload()
            },
            error: function (xhr) {
                alert(xhr.responseText)
            }
        })
    })
})