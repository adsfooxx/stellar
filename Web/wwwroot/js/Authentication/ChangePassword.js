$(document).ready(function () {
    $('#changePasswordForm').on('submit', function (event) {
        var currentPassword = $('#OriginPassword').val();
        var newPassword = $('#NewPassword').val();
        var checkPassword = $('#CheckPassword').val();
        var errMessage = $('#errorMessage');

        if (newPassword !== checkPassword  ) {
            event.preventDefault();
            console.log("hi");
            errMessage.text("新密碼與確認密碼不符合");
            errMessage.show();

        } else if (newPassword == currentPassword) {
            event.preventDefault();
            errMessage.text("舊密碼與新密碼不可一樣");
            errMessage.show();
        }
        else {
            errMessage.hide();
        }

    })
})

