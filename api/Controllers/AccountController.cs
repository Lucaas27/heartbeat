
using System.Security.Cryptography;
using System.Text;
using api.Data;
using api.DTOs;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;

        }

        [HttpPost("register")] // POST api/account/register?username=sam&password=password
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is already in use");

            using var hmac = new HMACSHA512(); // Hashing algorithm 

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key

            }; //new user object

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")] // POST api/account/login
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            // Single or default returns the only record that it encounters or null if it doesnt exist. If there are multiple records it will return an exception
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            //if user is null that means the username doesn't exist in the db. Return http unauthorized then
            if (user == null) return Unauthorized("Invalid Username");

            // It uses a specific key to create a hash. The key corresponds to the password salt in the database.
            // So the hash created should match the password hash
            using var hmac = new HMACSHA512(user.PasswordSalt);

            //  The password passed needs to be converted into a hash and then we can compare each element in the array
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //compare each element in the array
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}