﻿namespace MyBackedApi.DTOs.User.Requests
{
    public class AddStudentDetails
    {
        public string Specialization { get; set; }
        public Guid GroupId { get; set; }
    }
}
