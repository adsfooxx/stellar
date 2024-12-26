using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Linebot.SemanticKernel
{

    public class ChatSummarizationService
    {
        private readonly Kernel _kernel;
        private readonly ChatHistory _chatHistory;
        private readonly IChatCompletionService _chatCompletionService;

        public ChatSummarizationService(Kernel kernel)
        {
            _kernel = kernel;
            _chatHistory = new ChatHistory();
            _chatHistory.AddSystemMessage("Summarize the following chat history in 800 words or less. Keep the summary in the original language of the chat, focusing on main topics discussed and any decisions made.");
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> GetSummarization(string latestSummarization)
        {
            _chatHistory.AddSystemMessage(latestSummarization);
            var executionSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };
            var result = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory, executionSettings, _kernel);
            return result.Content;
        }
    }
}
