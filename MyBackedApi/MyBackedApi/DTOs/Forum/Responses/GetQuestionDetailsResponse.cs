namespace MyBackedApi.DTOs.Forum.Responses
{
    public class GetQuestionDetailsResponse : GetQuestionResponse
    {
        public List<GetAnswerResponse> Answers { get; set; }
    }
}
