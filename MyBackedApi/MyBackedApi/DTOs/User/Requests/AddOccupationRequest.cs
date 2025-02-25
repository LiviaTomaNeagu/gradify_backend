namespace MyBackedApi.DTOs.User.Requests
{
    public class AddOccupationRequest
    {
        public string Name { get; set;}
        public string Domain { get; set; }
        public string AdminEmail { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
    }
}
