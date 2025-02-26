namespace MyBackedApi.DTOs.User
{
    public class ShortOccupationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AdminEmail { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
        public bool IsActive { get; set; }

    }
}
