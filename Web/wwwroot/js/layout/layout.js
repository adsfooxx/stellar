$(document).ready(function () {
    $('#chatBubble').on('click', function () {
        const chatBox = $('#chatBox');
        chatBox.css('display', 'flex');
        setTimeout(() => {
            chatBox.addClass('open');
        }, 10); // 給定少許延遲以觸發過渡效果
        $('#chatBubble').css('display', 'none');
    });

    $('#closeChat').on('click', function () {
        const chatBox = $('#chatBox');
        chatBox.removeClass('open');
        setTimeout(() => {
            chatBox.css('display', 'none');
            $('#chatBubble').css('display', 'flex'); // 關閉後重新顯示氣泡
        }, 300); // 300ms 的延遲與過渡時間同步
    });

    $('#sendBtn').on('click', function () {
        sendMessage();
    });

    $('#chatInput').on('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault(); // 防止換行
            sendMessage();
        }
    });

    function sendMessage() {
        let input = $('#chatInput').val();
        if (input) {
            //displayMessage(`${input}`)

        
            displayMessage(`${input}`, false);
            // 呼叫 API
            callApi(input);
            
            // 清空輸入框
            $('#chatInput').val('');
        }
    }

    function callApi(userMessage) {
        const apiUrl = '/api/LayoutChat'; 

        $.ajax({
            url: apiUrl,
            type: 'POST',
            contentType: 'application/json', 
            data: JSON.stringify({ content: userMessage }), // 傳遞純文字訊息
            success: function (response) {
                // 將 API 回應顯示在聊天室中
                //displayMessage(`${response.response}`);

                displayMessage(`${response.response}`, true);
            },
            error: function (xhr, status, error) {
                console.error('API 錯誤:', error);
                //displayMessage('很抱歉，發生錯誤，請稍後再試。');

                displayMessage('很抱歉，發生錯誤，請稍後再試。', true);
            }
        });
    }

    function displayMessage(message, isFromAssistant) {
        let chatContent = $('.chat-content');
        let newMessage = $('<p style="margin-bottom:8px"></p>').html(`${message}`);

        // 根據是客服 AI 還是使用者來添加不同的類
        if (isFromAssistant) {
            newMessage.addClass('assistant-message');
        } else {
            newMessage.addClass('user-message');
        }
        chatContent.append(newMessage);
        chatContent.scrollTop(chatContent.prop('scrollHeight')); // 自動滾動到最底部
    }
});
