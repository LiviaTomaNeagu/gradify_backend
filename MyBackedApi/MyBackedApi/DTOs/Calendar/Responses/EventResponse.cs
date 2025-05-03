namespace MyBackedApi.DTOs.Calendar.Responses
{
    public class EventResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ColorPrimary { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid CoordinatorId { get; set; }
    }

}
