using MyBackedApi.Models;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetOccupationsResponse
    {
        public List<ShortOccupationDTO> Occupations { get; set; }
        public int TotalNumber { get; set; }
        public int TotalActive { get; set; }
    }
}
