using System;

namespace DateApp.Core.DataModels
{
    public class MessageForCreationDm
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }

        public MessageForCreationDm()
        {
            MessageSent = DateTime.Now;
        }
    }
}
