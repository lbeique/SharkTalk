using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SharkTalk.Models;
using SharkTalk.Hubs;

namespace SharkTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly DatabaseContext _context;

  private readonly IHubContext<ChatHub> _hub;

  public UsersController(DatabaseContext context, IHubContext<ChatHub> hub)
  {
    _context = context;
    _hub = hub;
  }

  // GET: api/Users
  [HttpGet]
  public async Task<ActionResult<IEnumerable<User>>> GetUsers()
  {
    return await _context.Users.ToListAsync();
  }

  // GET: api/Users/5
  [HttpGet("{id}")]
  public async Task<ActionResult<User>> GetUser(int id)
  {
    var User = await _context.Users.FindAsync(id);

    if (User == null)
    {
      return NotFound();
    }

    return User;
  }

  [HttpPost("login")]
  public async Task<ActionResult<User>> Login(User loginUser)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == loginUser.Name && u.Password == loginUser.Password);

    if (user == null)
    {
      return NotFound();
    }

    return user;
  }

  [HttpPost("signup")]
  public async Task<ActionResult<User>> SignUp(User newUser)
  {
    if (await _context.Users.AnyAsync(u => u.Name == newUser.Name))
    {
      return Conflict("Username already exists.");
    }

    newUser.Created = DateTime.UtcNow;
    _context.Users.Add(newUser);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
  }

  // POST: api/Users
  [HttpPut("{id}")]
  public async Task<IActionResult> PutUser(int id, User User)
  {
    if (id != User.Id)
    {
      return BadRequest();
    }

    _context.Entry(User).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    await _hub.Clients.All.SendAsync("UserUpdated", User);


    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    var User = await _context.Users.FindAsync(id);
    if (User == null)
    {
      return NotFound();
    }

    _context.Users.Remove(User);
    await _context.SaveChangesAsync();

    return NoContent();
  }
}