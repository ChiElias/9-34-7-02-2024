using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(IMapper mapper,DataContext dataContext, ITokenService tokenService,UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _dataContext = dataContext; 
        _tokenService = tokenService;
        _userManager = userManager;
   

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _dataContext.Users
                        // .Include(photo => photo.Photos)
                        .SingleOrDefaultAsync(user =>
                            user.UserName == loginDto.UserName);

        if (user is null) return Unauthorized("invalid username");

        // using var hmacSHA256 = new HMACSHA256(user.PasswordSalt!);

        // var computedHash = hmacSHA256.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password!.Trim()));
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash?[i]) 
        //     return Unauthorized("invalid password");
        // }
        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            // PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url,
            // Aka = user.Aka,
            // Gender = user.Gender
        };
    }

    [HttpPost("register")] //ApiController automatically binds the object
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await isUserExists(registerDto.Username!)) 
        return BadRequest("username is already exists");
        var user = _mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username!.Trim().ToLower();

        var AppUser = await _userManager.CreateAsync(user,registerDto.Password);
        if(!AppUser.Succeeded) return BadRequest(AppUser.Errors);
        // _dataContext.Users.Add(user);
        var role = await _userManager.AddToRoleAsync(user, "Member");//
        if (!role.Succeeded) return BadRequest(role.Errors);
        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            // Aka = user.Aka,
            // Gender = user.Gender
        };
    }       

    private async Task<bool> isUserExists(string username)
    {
        return await _dataContext.Users.AnyAsync(user => user.UserName == username.ToLower());
    }
}