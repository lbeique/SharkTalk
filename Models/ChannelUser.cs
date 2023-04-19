namespace SharkTalk.Models
{
    public class ChannelUser
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }
    }
}