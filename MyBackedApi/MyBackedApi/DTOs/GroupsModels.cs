namespace MyBackedApi.DTOs
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class DeleteGroupRequest
    {
        public Guid Id { get; set; }
    }
}
