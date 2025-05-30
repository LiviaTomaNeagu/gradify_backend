﻿using MyBackedApi.Enums;

namespace MyBackedApi.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public TopicEnum Topic { get; set; }

        public string? ImageText { get; set; }
        public string? DocumentText { get; set; }
        public float[]? EmbeddingVector { get; set; }
        public float[]? TitleEmbedding { get; set; }
        public float[]? ContentEmbedding { get; set; }
        public float[]? ImageEmbedding { get; set; }
        public float[]? DocumentEmbedding { get; set; }


        public ICollection<Answer> Answers { get; set; }
        public User User { get; set; }
    }

}
