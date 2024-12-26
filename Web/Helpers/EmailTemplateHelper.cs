namespace Web.Helpers
{
    public class EmailTemplateHelper
    {

        public static string ChangePasswordVerifyEmail(string Reciver, string Url)
        {
            return string.Format($"""
                <header
                    style="background-color: #171d25; height:100px;display: flex; justify-content: center;align-content: center;">
                    <div class="img" style="width: 200px;display: flex; align-items: center;align-content:center;padding:20px;"><img
                            style="width: 180px; height:50px;"
                            src="https://res.cloudinary.com/dijn0xzac/image/upload/v1726978691/logo_steam_wdepd3.png"></div>

                </header>
                <div style="width: 100%; background: linear-gradient(to bottom, #214d6f 0%, #1a2a3c 400px);display: flex;justify-content: center;">
                    <div class="container" style="max-width: 600px; padding: 20px;">
                        <div class="user" style="color:#fff;font-size: 25px;font-weight: bolder;" >
                            <span>親愛的客戶</span>
                            <span>{Reciver}</span>
                            <span>您好</span>
                        </div>
                        <div class="title" style="color:#66c0f4;font-size: 19px;padding: 10px 0;">
                            您的驗證資訊如下：
                        </div>
                        <div class="verifyCode" style="padding: 10px 0;font-size: 19px;color:#BEEE11">
                            <span><a style="color:#BEEE11;" href="{Url}">驗證網址超連結</a></span>
                        </div>
                        <ul style="font-size: 12px; color:#ccc;list-style: none;padding:0 ">
                            <li class="txt">*驗證碼時效性為10分鐘，請於10分鐘內輸入驗證</li>
                            <li class="txt">*為了您的帳號安全著想，建議辦理手機等二次驗證，如非本人操作，請立即向客服中心通報。</li>
                            <li class="txt">*Stellar客服中心關心您。</li>
                        </ul>

                    </div>

                </div>
                            

                """);
        }
    }
}
