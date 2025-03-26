namespace MyBackedApi.DTOs
{
    public class MessagePayload
    {
        public string Id { get; set; }               // ID-ul destinatarului
        public string From { get; set; }             // Numele expeditorului
        public string Subject { get; set; }          // Subiectul mesajului
        public string Photo { get; set; }            // Avatar expeditor
        public List<ChatEntry> Chat { get; set; }    // Lista de mesaje (1 element în cazul trimiterii)

        public class ChatEntry
        {
            public string Type { get; set; }         // 'odd' sau 'even'
            public string Msg { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
