using ApplicationCore.Entities;
using Infrastructure.Data.Mongo.Entity;
using Infrastructure.Data.Mongo.Repository;
using Infrastructure.Services.Linebot.SemanticKernel;
using isRock.LineBot;
using Microsoft.VisualBasic;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Messages = Infrastructure.Data.Mongo.Entity.Messages;
using User = Infrastructure.Data.Mongo.Entity.User;

namespace Infrastructure.Services.Linebot.LineMessage
{
    public class MessageService
    {
        private readonly MongoRepository<Products> _productRepository;
        private const int MaxMessagesPerConversation = 30; // 設定對話紀錄上限
        private readonly ChatSummarizationService _chatSummarizationService;
        private readonly StellarChatService _CustomerSupportChatService;
        private readonly MongoRepository<Messages> _messageRepository;
        public MessageService(MongoRepository<Products> repository, ChatSummarizationService chatSummarizationService,
            StellarChatService travelChatService, MongoRepository<Messages> messageRepository)
        {
            _productRepository = repository;
            _chatSummarizationService = chatSummarizationService;
            _CustomerSupportChatService = travelChatService;
            _messageRepository = messageRepository;
        }

        public async Task<string> ProcessMessageAsync(string LoginUserId, string displayName, string messageContent)
        {
            // 確認使用者是否存在，若不存在新增一個
            var user = await CreateUserIfNotExisted(LoginUserId, displayName);
            // 取得使用者 id，根據使用者 id 搜尋是否有存在的對話紀錄
            var (conversation, reminderMessage) = await GetOrCreateConversation(user.UseObjectrId);

            if (conversation == null)
            {
                // Handle the unexpected case where no conversation is returned
                return "取得對談紀錄失敗，請重新嘗試";
            }

            var summarization = conversation.Summarization ?? "目前沒有相關對話紀錄";
            // 將對話紀錄新增一筆訊息
            await CreateMessageByConversationIdAsync(conversation.ConversationId, messageContent, MessageType.User);
            var historyMessage = await _messageRepository.GetByIdAsync("ConversationId", conversation.ConversationId);
            var historyMessageJson= JsonSerializer.Serialize(historyMessage);
            // 根據對話紀錄的摘要和使用者輸入取得回應
            var chatResponse = await _CustomerSupportChatService.GetChatResponseByHistoryAndInput(LoginUserId, displayName, summarization, historyMessageJson, messageContent);
            // 將回應新增一筆訊息
            await CreateMessageByConversationIdAsync(conversation.ConversationId, chatResponse, MessageType.System);
            // 更新對話紀錄的摘要
            await UpdateSummarization(conversation.ConversationId);
            // 回傳訊息 If there's a reminder message, prepend it to the chat response
            if (!string.IsNullOrEmpty(reminderMessage))
            {
                chatResponse = $"{reminderMessage}{Environment.NewLine}{Environment.NewLine}{chatResponse}";
            }

            return chatResponse;
        }

        private async Task<Conversation> UpdateSummarization(string conversationId)
        {
            var conversation = await _productRepository.GetConversationAsync(conversationId);
            if (conversation.Summarization is null)
            {
                conversation.Summarization = "目前沒有相關對話紀錄";
            }
            else
            {
                var latestMessages = await _productRepository.GetMessagesByConversationIdAsync(conversationId);
                var chatHistory = latestMessages.Select(m => $"{m.MessageType.ToString()} > {m.Content}").ToList();
                var stringChatHistory = string.Join(Environment.NewLine, chatHistory);
                var newSummarization = await _chatSummarizationService.GetSummarization(stringChatHistory);
                conversation.Summarization = newSummarization;
            }

            await _productRepository.UpdateConversationAsync(conversation);
            return conversation;
        }

        private async Task<User> CreateUserIfNotExisted(string UserId, string displayName)
        {
            var user = await _productRepository.GetUserByLineUserIdAsync(UserId);
            if (user == null)
            {
                user = new User { UserId = UserId, Name = displayName, CreateAt = DateTime.Now };
                await _productRepository.CreateUserAsync(user);
            }

            return user;
        }

        private async Task<(Conversation? Conversation, string? ReminderMessage)> GetOrCreateConversation(string userId)
        {
            var latestConversation = await _productRepository.GetLatestConversationByUserIdAsync(userId);

            if (latestConversation == null)
            {
                // If there's no conversation, create a new one
                var newConversation = await CreateConversationByUserId(userId);
                return (newConversation, null);
            }

            if (await IsConversationFull(latestConversation.ConversationId))
            {
                // If the conversation is full, create a new one and return a reminder message
                var newConversation = await CreateConversationByUserId(userId);
                string reminderMessage = "目前對話紀錄已滿，已建立新的對話紀錄。";
                return (newConversation, reminderMessage);
            }

            // If the conversation exists and is not full, return it without a reminder message
            return (latestConversation, null);
        }

        private async Task<bool> IsConversationFull(string conversationId)
        {
            var messages = await _productRepository.GetMessagesByConversationIdAsync(conversationId);
            return messages.Count >= MaxMessagesPerConversation;
        }

        private async Task<Conversation> CreateConversationByUserId(string userId)
        {
            var conversation = new Conversation { UserId = userId, CreateAt = DateTime.Now };
            await _productRepository.CreateConversationByUserIdAsync(userId, conversation);
            return conversation;
        }

        private async Task<Messages> CreateMessageByConversationIdAsync(string conversationId, string messageContent,
            MessageType messageType)
        {
            var message = new Messages
            {
                MessageType = messageType,
                Content = messageContent,
                Timestamp = DateTime.UtcNow
            };
            var result = await _productRepository.CreateMessageByConversationIdAsync(conversationId, message);
            return result;
        }
    }
}
