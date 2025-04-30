using System;
using MyBackedApi.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyBackedApi.Extensions
{
    public static class TopicEnumExtensions
    {
        public static TopicEnum KeyToTopicEnum(this int key)
        {
            if (Enum.IsDefined(typeof(TopicEnum), key))
                return (TopicEnum)key;

            throw new ArgumentOutOfRangeException(nameof(key), $"Invalid topic id: {key}");
        }

        public static TopicEnum KeyToTopicEnum(this string key)
        {
            if (int.TryParse(key, out var intValue) && Enum.IsDefined(typeof(TopicEnum), intValue))
                return (TopicEnum)intValue;

            throw new ArgumentOutOfRangeException(nameof(key), $"Invalid topic id: {key}");
        }

    }

}
