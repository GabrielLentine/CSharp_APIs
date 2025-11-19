using CatagoloAPI.DTOs;
using CatagoloAPI.Models;
using CatagoloAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatagoloAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _cfg;
    private readonly ILogger<AuthController> _logger;

    // tratar com os usuários
    private readonly UserManager<ApplicationUser> _userManager;
    
    // tratar com os perfis dos usuários
    private readonly RoleManager<IdentityRole> _roleMapper;

    public AuthController(ITokenService tokenService, IConfiguration cfg, ILogger<AuthController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleMapper)
    {
        _tokenService = tokenService;
        _cfg = cfg;
        _logger = logger;
        _userManager = userManager;
        _roleMapper = roleMapper;
    }

    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy = "SuperAdminOnly")] // apenas usuários na role SuperAdmin podem criar novas roles
    public async Task<IActionResult> CreateRole(string roleName)
    {
        // verificar se a role existe
        var roleExist = await _roleMapper.RoleExistsAsync(roleName);
        
        // caso a role não existir...
        if(!roleExist)
        {
            // vai criar a role
            var roleResult = await _roleMapper.CreateAsync(new IdentityRole(roleName));

            // se a criação da role for bem sucedida, retorna um 200 OK
            if(roleResult.Succeeded)
            {
                _logger.LogInformation(1 , "Role Added");
                return StatusCode(StatusCodes.Status200OK , new ResponseDTO { Status = "Success" , Message = $"Role {roleName} added successfully" });
            }

            // se a criação da role não for bem sucedida, retorna um 400 Bad Request
            else 
            {
                _logger.LogInformation(2, "Role Error");
                return StatusCode(StatusCodes.Status400BadRequest , new ResponseDTO { Status = "Error" , Message = $"Issue adding the new {roleName} role" });
            }
        }

        // caso a role já exista
        return StatusCode(StatusCodes.Status400BadRequest , new ResponseDTO { Status = "Error" , Message = "Role already exist" });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(Policy = "SuperAdminOnly")] // apenas usuários na role SuperAdmin podem criar novas roles
    public async Task<IActionResult> AddUserToRole(string email, string roleName) 
    {
        // o usuário e a role precisam existir para adicionar o usuário à role

        // localizar o usuário pelo email
        var user = await _userManager.FindByEmailAsync(email);

        // se o usuário for encontrado
        if(user != null)
        {
            // adicionar o usuário à role
            var result = await _userManager.AddToRoleAsync(user, roleName);

            // se a adição for bem sucedida, retorna um 200 OK
            if(result.Succeeded) return StatusCode(StatusCodes.Status200OK , new ResponseDTO { Status = "Success" , Message = $"User {email} added to role {roleName} successfully" });

            // se a adição não for bem sucedida, retorna um 400 Bad Request
            else return StatusCode(StatusCodes.Status400BadRequest, new ResponseDTO { Status = "Error" , Message = $"Issue adding user {email} to role {roleName}" });
        }

        // se o usuário não for encontrado, retorna um 400 Bad Request
        return BadRequest(new { Error = "Unable to find user" });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO model) 
    {
        // verificar se o usuário existe
        var user = await _userManager.FindByNameAsync(model.Username!);
        
        // caso o usuário exista e a senha está correta
        if(user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            // obtendo os perfis do usuário
            var userRoles = await _userManager.GetRolesAsync(user);

            // registrando as informações do usuário
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id" , user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // claim padrão JWT (Jti - JSON WebTokensId), ela fornece um ID exclusivo p/ o token (com guid)
            };

            // iterando sobre os perfis obtidos e colocando cada claim correspondente ao cliente
            foreach(var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role , userRole));
            }

            // gerando o token
            var token = _tokenService.GenerateAccessToken(authClaims , _cfg);

            // gerando o refresh token
            var refreshToken = _tokenService.GenerateRefreshToken();

            // obtendo a data de validação do refresh token (sem retornar a operação (por isso usar o '_')
            _ = int.TryParse(_cfg["JWT:RefreshTokenValidityInMinutes"] , out int refreshTokenValidityInMinutes);

            // salvando o refresh token (com a sua data de validação)
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            // persistindo o token no bd
            await _userManager.UpdateAsync(user);

            // retornando o token, o refresh token e a data de expiração (JSON)
            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token) ,
                RefreshToken = refreshToken ,
                Expiration = token.ValidTo
            });
        }

        // caso o if for inválido, vai retornar um 401
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
    {
        // verificar se o nome do usuário já existe
        var userExists = await _userManager.FindByNameAsync(model.Username!);

        // se o usuário já existir, retornar um erro
        if(userExists is not null) return StatusCode(StatusCodes.Status500InternalServerError , new  ResponseDTO { Status = "Error" , Message = "Usuário já existe!" });

        // criando um novo usuário
        ApplicationUser user = new()
        {
            Email = model.Email , // email do usuário
            SecurityStamp = Guid.NewGuid().ToString() , // identificador exclusivo para o usuário
            UserName = model.Username // nome do usuário
        };

        // criando o usuário com a senha fornecida
        var result = await _userManager.CreateAsync(user, model.Password!);

        // se a criação falhar, retornar um erro
        if(!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError , new ResponseDTO { Status = "Error" , Message = "Criação do usuário falhou!" });

        return Ok(new ResponseDTO { Status = "Success" , Message = "Usuário criado com sucesso!" });
    }

    [HttpPost]
    [Route("refresh-token")]
    // obter um novo token de acesso quando o antigo expirar
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    { 
        if(tokenModel is null) return BadRequest("Requisição do cliente inválido!");
        
        // obtendo tanto o token JWT quanto o Refresh Token
        string? accessToken = tokenModel.AccessToken ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));
        
        // obtendo, a partir do token JWT expirado, as claims do token expirado p/ gerar um novo token
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken! , _cfg);
        if(principal is null) return BadRequest("Token/Refresh Token de acesso inválido!");

        // obtendo o nome do usuário a partir das claims e localizando o usuário no BD
        string? username = principal!.Identity!.Name;
        var user = await _userManager.FindByNameAsync(username!);

        // se não existir usuário
        // OU o RefreshToken for diferente do que está no salvo no BD
        // OU a data de expiração do Refresh Token é menor que a data atual
        // então o token é inválido (erro 400)!
        if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Token/Refresh Token de acesso inválido!");
        
        // se o usuário existir e o token for válido, vai gerar um novo Token de acesso (passando as claims obtidas) assim como um novo Refresh Token
        var newAccessToekn = _tokenService.GenerateAccessToken(principal.Claims.ToList() , _cfg);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // o valor do Refresh Token presente no BD é atualizado com o novo gerado, depois salvo no BD
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        // retornando o Token de acesso e o Refresh Token
        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToekn),
            refreshToken = newRefreshToken
        });
    }

    [HttpPost]
    [Route("revoke/{username}")]
    [Authorize(Policy = "ExclusiveOnly")] // apenas o usuário "lentine" ou todos aqueles que estão na role SuperAdmin
    public async Task<IActionResult> Revoke(string username)
    {
        // localizo se o usuário está presente no BD e verifico se ele existe
        var user = await _userManager.FindByNameAsync(username);
        if(user is null) return BadRequest("Nome do usuário inválido!");
        
        // se o usuário for encontrado, o RefreshToken é nulo, e isso é atualizado no BD
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}
