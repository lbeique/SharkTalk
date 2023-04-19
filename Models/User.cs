namespace SharkTalk.Models
{
    public class User
    {
        public User(int id, string name, string password, DateTime created)
        {
            Id = id;
            Name = name;
            Password = password;
            Created = created;
            Messages = new List<Message>();
            Channels = new List<Channel>();
            ChannelUsers = new List<ChannelUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Channel> Channels { get; set; }
        public ICollection<ChannelUser> ChannelUsers { get; set; }
    }
}