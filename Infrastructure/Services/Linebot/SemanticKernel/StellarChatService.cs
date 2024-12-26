using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;

namespace Infrastructure.Services.Linebot.SemanticKernel
{
    public class StellarChatService
    {
        private readonly Kernel _kernel;
        private readonly ChatHistory _chatHistory;
        private readonly IChatCompletionService _chatCompletionService;

        public StellarChatService(Kernel kernel)
        {
            _kernel = kernel;
            _chatHistory = new ChatHistory();
            string systemPrompt = """
                              # Role: Stellar gamers platform CustomerSupport Export
                              ## Profile
                              - Language: 繁體中文
                              - Description: 你是Stellar遊戲平台的客服人員
                              ## Skill-1
                              - 熟悉我們網站的各種產品。
                              - 能夠根據使用者的需求提供相關的產品。
                              - 請用繁體中文回答
                              - 你是遊戲平台的客服人員，因此只能回答與遊戲及本網站相關的問題。
                              - 你的名字是Stellar小幫手。
                              - 聊天室小姊姊(流星)只會陪你聊天及對話。
                              - 當想問流星時，指的就是我們的聊天室小姊姊。
                              - 回答時請幫我用html標籤語法標記，範例如下：
                              <div>
                              我是機器人<br>
                              -我有一顆聰明的腦袋<br>
                              -我能為您解決各種問題<br>
                              </div>
                              '
                              ## Skill-2
                              - 了解Stellar平台的所有遊戲，並能依客戶的問題提出專業且易懂的回答。
                              - 熟悉本網站的各個路徑。
                              ## Skill-3
                              - 根據提供的 Function GetProductRecommendationsByUserInput 取得相關的遊戲資訊。
                              - 請一次最多提供3種遊戲
                              - 若是呼叫 Function GetProductRecommendationsByUserInput 的回應，請回應參考格式如下，並且依據以下格式進行換行 
                              - 遊戲: {遊戲名稱}
                              - 分類：{遊戲分類}
                              - 標籤：{遊戲標籤}
                              - 金額: {價錢}
                              - 如果{價錢}是0元，請回答'免費'
                              ## Skill-4
                              - 如果使用者為非登入狀態(visitor)，請勿從 Function GetOrderData 取得使用者訂單資訊，回應格式參考如下
                              - 很抱歉，您目前並非登入狀態，請登入在進行詢問。
                              - 若使用者為登入狀態時，僅能取得自己{userId}的訂單資訊。
                              ## Skill-5
                              - 如果使用者提出與網頁路徑及遊戲網站顧客服務相關的問題，請根據 Function GetStellarPath 回答。
                              

                              ## 用戶歷史對談紀錄如下
                              """;
            _chatHistory.AddSystemMessage(systemPrompt);
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            
        }

        public async Task<string> GetChatResponseByHistoryAndInput(string userId,string userName,string summarization, string history, string input)
        {
            if (userId == "0")
            {
                _chatHistory.AddSystemMessage("使用者為非登入狀態，請勿讓他詢問使用者個人相關的問題");
                _chatHistory.AddUserMessage(input);
            }
            else
            {

                _chatHistory.AddSystemMessage(summarization);
                _chatHistory.AddSystemMessage(history);
                _chatHistory.AddSystemMessage($"已登入的使用者{userName}，使用者ID為{userId}，請根據歷史紀錄與輸入提供個性化的回答");
                _chatHistory.AddUserMessage(input);
            }
            var executionSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };
            var result = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory, executionSettings, _kernel);
            return result.Content;
        }
    }
}
