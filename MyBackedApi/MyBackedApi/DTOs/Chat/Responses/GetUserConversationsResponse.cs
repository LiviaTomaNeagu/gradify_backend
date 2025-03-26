namespace MyBackedApi.DTOs.Chat.Responses
{
    public class GetUserConversationsResponse
    {
        public List<GetConversation> Conversations { get; set; }
    }

    public class GetConversation

    {
        public string Id { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Photo { get; set; }
        public List<ChatEntryDto> Chat { get; set; }
    }

    public class ChatEntryDto
    {
        public string Type { get; set; }
        public string Msg { get; set; }
        public DateTime Date { get; set; }
    }
}
