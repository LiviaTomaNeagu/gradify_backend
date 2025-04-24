namespace MyBackedApi.DTOs
{
    public class CreateNoteDto
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public DateTime Datef { get; set; }
    }

    public class NoteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public DateTime Datef { get; set; }
    }

}
