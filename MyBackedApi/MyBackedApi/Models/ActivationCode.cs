namespace MyBackedApi.Models
{
    public class ActivationCode : BaseModel
    {
        public int Code { get; set; }
        public string Email { get; set; }
    }
}
