using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SkiStore.API.DTOs.SkiStoreDB.Account;
using SkiStore.API.Models.API;
using SkiStore.API.Models.SkiStoreDB;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkiStore.API.Services.SkiStoreDB;

public class AccountService
{
    private readonly UserManager<User> _userManager;

    public AccountService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<APIResponse<UserDTO>> LogInUser(LoginDTO loginDTO)
    {
        try
        {
            APIResponse<UserDTO> APIResponse = new();

            User? user = await _userManager.FindByNameAsync(loginDTO.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                APIResponse.StatusCode = 401;
                APIResponse.ErrorMessage = "Unauthorized";
                APIResponse.IsSuccessful = false;

                return APIResponse;
            }


            UserDTO userDto = new()
            {
                Email = user.Email,
                Token = await GenerateToken(user)
            };

            if (userDto.Token == null) throw new Exception("Problem constructing token!");

            APIResponse.IsSuccessful = true;
            APIResponse.SuccessMessage = "Logged in !";
            APIResponse.StatusCode = 200;
            APIResponse.Data = userDto;

            return APIResponse;
        }
        catch (Exception ex)
        {
            APIResponse<UserDTO> APIResponse = new()
            {
                StatusCode = 500,
                ErrorMessage = "-" + ex.Message,
                Data = null,
                IsSuccessful = true
            };

            return APIResponse;
        }
    }


    public async Task<APIResponse<bool>> RegisterUser(RegisterDTO registerDTO)
    {
        try
        {
            APIResponse<bool> APIResponse = new();

            User user = new() { UserName = registerDTO.Username, Email = registerDTO.Email };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.Data = false;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"Problem registering User - {registerDTO.Email} !";

            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Member");

                APIResponse.IsSuccessful = true;
                APIResponse.Data = true;
                APIResponse.StatusCode = 201;
                APIResponse.SuccessMessage = $"User - {registerDTO.Email} registered !";
            }

            return APIResponse;
        }
        catch (Exception ex)
        {
            APIResponse<bool> APIResponse = new()
            {
                StatusCode = 500,
                ErrorMessage = "-" + ex.Message,
                Data = false,
                IsSuccessful = true
            };

            return APIResponse;
        }
    }

    public async Task<APIResponse<UserDTO>> GetCurrentUser(string userName)
    {
        try
        {
            APIResponse<UserDTO> APIResponse = new();

            if (userName == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.Data = null;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"Token not found!";

                return APIResponse;
            }

            User? user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.Data = null;
                APIResponse.StatusCode = 401;
                APIResponse.ErrorMessage = $"User not found!";

                return APIResponse;
            }


            UserDTO userDTO = new()
            {
                Email = user.Email,
                Token = await GenerateToken(user)
            };

            if (userDTO.Token == null) throw new Exception("Problem constructing token!");

            APIResponse.IsSuccessful = true;
            APIResponse.Data = userDTO;
            APIResponse.StatusCode = 201;
            APIResponse.SuccessMessage = $"User - {userDTO.Email} found !";
            return APIResponse;
        }
        catch (Exception ex)
        {
            APIResponse<UserDTO> APIResponse = new()
            {
                StatusCode = 500,
                ErrorMessage = "-" + ex.Message,
                Data = null,
                IsSuccessful = true
            };

            return APIResponse;
        }
    }
    public async Task<string> GenerateToken(User user)
    {
        try
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.UserName),
            };

            IList<string> roles = await _userManager.GetRolesAsync(user);

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(Settings.TokenKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512);

            JwtSecurityToken tokenOptions = new(issuer: null, audience: null, claims: claims, expires: DateTime.Now.AddDays(7), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        catch
        {
            return null;
        }
    }
}
