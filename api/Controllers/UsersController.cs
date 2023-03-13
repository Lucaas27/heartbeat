

using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // endpoint:  GET api/users
    public class UsersController : ControllerBase
    {
        //In order to use the DB session inside this class we need to assign the session below to a private property
        private readonly DataContext _context;

        //dependency injection
        /* When this class is created (by EF when the user calls the endpoint ) 
        it also creates an instance of data context called context. 
        This means we will then have available a session with the DB. */
        public UsersController(DataContext context)
        {
            _context = context;

        }

        // ActionResult allows us to return a standard http response (i.e. return BadRequest())
        //This method returns a list of all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }

        //This method returns a single user
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {

            return await _context.Users.FindAsync(id);

        }
    }
}