using Amazon.S3.Model;
using MyBackedApi.DTOs.Forum;
using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetStatsMentorResponse
    {
        public int TotalAnswers { get; set; }
        public List<ShortUserDto> TopUsers { get; set; }
        public List<LatestQuestion> LatestQuestions { get; set; }
        public List<GraphDataPoint> ActivityGraph { get; set; }
        public List<TopicEnum> FavoriteTopics { get; set; }
    }
}
