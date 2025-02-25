namespace MyBackedApi.DTOs.User.Responses
{
    public class GetMyCompanyResponse
    {
        public Guid Id { get; set; }
        public int TotalUsers { get; set; }
        public int TotalResponses { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Domain { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
        public string AdminEmail { get; set; }

    }
}
