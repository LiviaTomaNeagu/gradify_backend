using MyBackedApi.DTOs.Forum;
using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetStudentDashboardResponse
    {
        public int TotalQuestionsAsked { get; set; }
        public int TotalMentorsAnswered { get; set; }
        public List<LatestQuestion> LatestQuestions { get; set; }
        public List<ShortUserDto> MentorInfo { get; set; }
        public List<TopicEnum> FavoriteTopics { get; set; }
    }
}
