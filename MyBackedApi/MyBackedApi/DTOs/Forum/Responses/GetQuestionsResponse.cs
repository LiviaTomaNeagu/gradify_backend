namespace MyBackedApi.DTOs.Forum.Responses
{
    public class GetQuestionsResponse
    {
        public List<GetQuestionResponse> Questions { get; set; }
        public int totalQuestions { get; set; }
        public int totalFilteredQuestions { get; set; }
    }
}
