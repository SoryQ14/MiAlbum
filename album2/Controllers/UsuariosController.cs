using album.Models;
using album2.DTO;
using album2.Models;
using AutoMapper;
using login.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace album.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IConfiguration configuration;

        public UsuarioController(IMapper mapper, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(UsuarioDTO usuarioDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", usuarioDTO.Email)
            };
            var user = await userManager.FindByEmailAsync(usuarioDTO.Email);
            var claimRoles = await userManager.GetClaimsAsync(user!);

            claims.AddRange(claimRoles);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJWT"]!));

            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.Now.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);
            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion, 
                UserId = user.Id
            };

        }

        //Endpoint pa renovar el token
        [HttpGet("Renovar Token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            var user = new UsuarioDTO()
            {
                Email = emailClaim!.Value
            };
            return await ConstruirToken(user);
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(UsuarioDTO usuarioDTO)
        {
            var user = mapper.Map<Usuario>(usuarioDTO);
            user.UserName = usuarioDTO.Email;
            var resul = await userManager.CreateAsync(user, usuarioDTO.password);

            if (resul.Succeeded)
            {
                UsuarioDTO usuario = new UsuarioDTO(); 
                usuario.Email = usuarioDTO.Email;
                usuario.password = usuarioDTO.password;
                usuario.ApellidoPaterno = usuarioDTO.ApellidoPaterno; 
                usuario.ApellidoMaterno = usuarioDTO.ApellidoMaterno;

                return await ConstruirToken(usuario); 
            }
            return BadRequest(resul);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(UsuarioDTO usuarioDTO)
        {
            var resul = await signInManager.PasswordSignInAsync(
                usuarioDTO.Email, usuarioDTO.password, isPersistent: false, lockoutOnFailure: false);
            if (resul.Succeeded)
                return await ConstruirToken(usuarioDTO);

            var error = new Mensaje()
            {
                Error = "Login incorrecto"
            }; 
            return BadRequest(error);
        }
    }
}
