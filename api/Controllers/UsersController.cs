

using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // endpoint:  GET api/users
    public class UsersController : ControllerBase
    {
        //In order to use the DB session inside this class we need to assign the session below to a property
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
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            var users = _context.Users.ToList();

            return users;
        }

        //This method returns a single user
        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUser(int id)
        {

            return _context.Users.Find(id);

        }
    }
}