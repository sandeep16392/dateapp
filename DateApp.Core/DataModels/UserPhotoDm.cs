using System;
using Microsoft.AspNetCore.Http;

namespace DateApp.Core.DataModels
{
    public class UserPhotoDm
    {
        public string url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }

        public UserPhotoDm()
        {
            DateAdded = DateTime.Now;
        }
    }
}
