namespace MyBackedApi.DTOs.User.Requests
{
    public class UpdateOccupationRequest
    {
        public Guid OccupationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
    }
}
