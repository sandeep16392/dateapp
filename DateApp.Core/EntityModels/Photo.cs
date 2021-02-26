using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DateApp.Core.EntityModels
{
    public class Photo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}