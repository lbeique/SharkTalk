using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SharkTalk.Models;
using SharkTalk.Hubs;

namespace SharkTalk.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChannelsController : ControllerBase
{
  private readonly DatabaseContext _context;
  private readonly IHubContext<ChatHub> _hub;
  public ChannelsController(DatabaseContext context, IHubContext<ChatHub> hub)
  {
    _context = context;
    _hub = hub;
  }

  // GET: api/Channels
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
  {
    return await _context.Channels.ToListAsync();
  }

  // GET: api/Channels/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Channel>> GetChannel(int id)
  {
    var channel = await _context.Channels
        .Include(c => c.ChannelUsers)
            .ThenInclude(cu => cu.User)
        .Include(c => c.Messages)
            .ThenInclude(m => m.User)
        .SingleOrDefaultAsync(c => c.Id == id);

    if (channel == null)
    {
      return NotFound();
    }
    return channel;
  }

  // GET: api/Channels/Users/5
  [HttpGet("Users/{id}")]
  public async Task<ActionResult<IEnumerable<Channel>>> GetChannelsByUser(int id)
  {
    var channels = await _context.Channels
        .Include(c => c.ChannelUsers)
            .ThenInclude(cu => cu.User)
        .Include(c => c.Messages)
            .ThenInclude(m => m.User)
        .Where(c => c.ChannelUsers.Any(cu => cu.UserId == id))
        .ToListAsync();

    if (channels == null)
    {
      return NotFound();
    }
    return channels;
  }


  // POST: api/Channels
  [HttpPost]
  public async Task<IActionResult> PostChannel(Channel Channel)
  {
    _context.Channels.Add(Channel);
    await _context.SaveChangesAsync();

    await _hub.Clients.All.SendAsync("ChannelCreated", Channel);

    return Ok();
  }

  // POST: api/Channels/5/Users
  [HttpPost("{channelId}/Users")]
  public async Task<IActionResult> PostChannelUser(int channelId, UserDTO userDTO)
  {
    var channel = await _context.Channels.FindAsync(channelId);
    var user = await _context.Users.FindAsync(userDTO.Id);

    if (channel == null || user == null)
    {
      return NotFound();
    }

    // // if user is already in channel, join
    // var existingUser = await _context.ChannelUsers
    //     .Where(cu => cu.ChannelId == channelId && cu.UserId == user.Id)
    //     .FirstOrDefaultAsync();

    // if (existingUser != null)
    // {
    //   var debugUser = await _context.Users
    // .Where(u => u.Id == user.Id)
    // .Include(u => u.ChannelUsers)
    // .FirstOrDefaultAsync();

    //   var debugUserDTO = new UserDTO
    //   {
    //     Id = debugUser.Id,
    //     Name = debugUser.Name

    //   };

    //   await _hub.Clients.Group(channelId.ToString()).SendAsync("UserJoined", debugUserDTO);
    // }

    var channelUser = new ChannelUser { UserId = userDTO.Id, ChannelId = channelId };

    _context.ChannelUsers.Add(channelUser);
    await _context.SaveChangesAsync();

    var newUser = await _context.Users
        .Where(u => u.Id == user.Id)
        .Include(u => u.ChannelUsers)
        .FirstOrDefaultAsync();

    var newUserDTO = new UserDTO
    {
      Id = newUser.Id,
      Name = newUser.Name

    };

    await _hub.Clients.Group(channelId.ToString()).SendAsync("UserJoined", newUserDTO);

    return Ok();
  }

  // DELETE: api/Channels/5
  [HttpDelete("{channelId}/Users/{userId}")]
  public async Task<IActionResult> DeleteChannelUser(int channelId, int userId)
  {
    var channelUser = await _context.ChannelUsers
        .Where(cu => cu.ChannelId == channelId && cu.UserId == userId)
        .FirstOrDefaultAsync();

    if (channelUser == null)
    {
      return NotFound();
    }

    _context.ChannelUsers.Remove(channelUser);
    await _context.SaveChangesAsync();

    var user = await _context.Users.FindAsync(userId);

    var userDTO = new UserDTO
    {
      Id = user.Id,
      Name = user.Name
    };

    await _hub.Clients.Group(channelId.ToString()).SendAsync("UserLeft", userDTO);

    return Ok();
  }

  // // POST: api/Channels/5/Messages
  // [HttpPost("{channelId:int}/Messages")]
  // public async Task<IActionResult> PostChannelMessage(int channelId, [FromBody] Message message)
  // {

  //   message.Created = DateTime.Now;

  //   _context.Messages.Add(message);
  //   await _context.SaveChangesAsync();

  //   var channel = await _context.Channels.FindAsync(message.ChannelId);
  //   var newMessage = await _context.Messages.Where(m => m.Id == message.Id).Include(m => m.User).FirstOrDefaultAsync();
  //   await _hub.Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", newMessage);

  //   return Ok();
  // }


  // PUT: api/Channels/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateChannel(int id, Channel Channel)
  {
    var channel = await _context.Channels.FindAsync(id);
    if (channel == null)
    {
      return NotFound();
    }

    channel.Name = Channel.Name;
    _context.Entry(channel).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    await _hub.Clients.All.SendAsync("ChannelUpdated", channel);

    return Ok();
  }

  // DELETE: api/Channels/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteChannel(int id)
  {
    var Channel = await _context.Channels.FindAsync(id);
    if (Channel == null)
    {
      return NotFound();
    }
    _context.Channels.Remove(Channel);
    await _context.SaveChangesAsync();

    await _hub.Clients.All.SendAsync("ChannelDeleted", Channel.Id);

    return Ok();
  }
}