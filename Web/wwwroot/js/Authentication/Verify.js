
$(document).ready(function () {
    $('#sendmail').on('submit', function (event) {
        event.preventDefault(); // 阻止表單的默認提交行為
        console.log("準備寄送")
        

        // 發送 AJAX 請求
        $.ajax({
            url: '/Authentication/SendMail', // 後端 API 路徑
            method: 'POST',
            contentType: 'application/json',
            data: "", 
            success: function (response) {
                console.log("寄送成功")
                $('#responseMessage').text(response.message); // 顯示回應訊息
            },
            error: function (xhr, status, error) {
                $('#responseMessage').text('寄送失敗，請稍後再試。');
            }
        });
    });

    $('#userVerifyCodeForm').on('submit', function (event) {
        event.preventDefault(); // 防止表單的默認提交，防止頁面刷新
        console.log("我到後面了")
        var userVerifyCode = $('#userVerifyCode').val();

        $.ajax({
            url: '/Authentication/Verify', // 後端 MVC Action 的 URL
            type: 'POST',
             // 指定請求內容為 JSON
            data: { userVerifyCode: userVerifyCode }, // 將驗證碼轉換為 JSON 格式
            success: function (response) {
                console.log("我到前面了!")
                if (response.success) {

                    if (response.redirectUrl) {
                        window.location.href = response.redirectUrl; // 手動重定向
                    } else {
                        $('#resultMessage').text(response.message).css('color', 'green');
                    }
                } else {
                    console.log(response.success);
                    $('#resultMessage').text(response.message).css('color', 'red');
                }
            },
            error: function (xhr, status, error) {
                $('#resultMessage').text('發生錯誤，請稍後再試').css('color', 'red');
            }
        });
    });
    var interval; // 用於存儲計時器
    var isRunning = false; // 防止多次點擊開始多個計時器

    function startTimer(duration, display) {
        var timer = duration, minutes, seconds;
        interval = setInterval(function () {
            minutes = Math.floor(timer / 60);
            seconds = Math.floor(timer % 60);

            minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;

            $(display).text(minutes + ":" + seconds);

            if (--timer < 0) {
                clearInterval(interval); // 停止計時

                isRunning = false; // 重置狀態，允許重新啟動計時器
            }
        }, 1000);
    }

    // 監聽按鈕點擊事件
    $('#sendmail').on('click', function () {
        if (!isRunning) { // 防止重複啟動多個計時器
            var tenMinutes = 60 * 10; // 10 分鐘的秒數
            var display = $('#timer'); // jQuery 選擇器選擇要顯示的元素
            startTimer(tenMinutes, display); // 開始計時
            isRunning = true; // 設置為運行中狀態
        }
    }
    )
});


