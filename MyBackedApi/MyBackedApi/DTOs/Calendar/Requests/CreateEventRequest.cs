namespace MyBackedApi.DTOs.Calendar.Requests
{
    public class CreateEventRequest
    {
        public string Title { get; set; }
        public string ColorPrimary { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

}
