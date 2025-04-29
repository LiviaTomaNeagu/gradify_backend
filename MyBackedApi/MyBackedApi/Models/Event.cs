namespace MyBackedApi.Models
{
    public class Event : BaseModel
    {
        public string Title { get; set; }
        public string ColorPrimary { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

}
