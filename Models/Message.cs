using System;
using System.ComponentModel.DataAnnotations;

namespace SharkTalk.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ChannelId { get; set; }
        public User? User { get; set; }
        public DateTime Created { get; set; }
        public Channel? Channel { get; set; }
    }
}