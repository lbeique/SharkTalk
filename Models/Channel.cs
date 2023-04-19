namespace SharkTalk.Models
{
    public class Channel
    {
        public Channel(int id, int creatorId, string name, DateTime created)
        {
            Id = id;
            CreatorId = creatorId;
            Name = name;
            Created = created;
            Messages = new List<Message>();
            Users = new List<User>();
            ChannelUsers = new List<ChannelUser>();
        }
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<ChannelUser> ChannelUsers { get; set; }
    }
}