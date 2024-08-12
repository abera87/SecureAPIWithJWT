using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.DTO;
using UserService.InMemoryCollections;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsers _users;
        private readonly IConfiguration _configuration;

        public UserController(IUsers users, IConfiguration configuration)
        {
            _users = users;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser([FromBody] UserRegistration userRegistration)
        {
            _users.Add(userRegistration);
            return Ok(userRegistration);
        }
        [HttpGet]
        [Route("{email}")]
        public IActionResult GetUser(string email)
        {
            var user = _users.Get(email);
            return Ok(user);
        }
        [HttpPost]
        [Route("rsatoken")]
        public IActionResult RSAToken([FromBody] string email)
        {
            var userFromCollection = _users.Get(email);
            if (userFromCollection == null) return BadRequest("Please share user deatils");
            var token = GenerateRSAToken(userFromCollection);
            return Ok(token);
        }
        [HttpPost]
        [Route("token")]
        public IActionResult Token([FromBody] string email)
        {
            var userFromCollection = _users.Get(email);
            if (userFromCollection == null) return BadRequest("Please share user deatils");
            var token = GenerateToken(userFromCollection);
            return Ok(token);
        }
        [HttpGet]
        [Route("/api/issuer/metadata")]
        public IActionResult Metadata([FromQuery] string? algo)
        {
            string resourceName = "UserService.Keys.public.pem";
            var publicKey = CreateStringFromEmbeddedResource(Assembly.GetExecutingAssembly(), resourceName);
            var key = string.IsNullOrEmpty(algo) ? _configuration["Jwt:Key"] : publicKey;
            var issuer = _configuration["Jwt:Issuer"];
            return Ok(new { key, issuer });
        }


        private string GenerateRSAToken(UserRegistration userRegistration)
        {
            string token = string.Empty;
            var issuer = _configuration["Jwt:Issuer"];
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userRegistration.Name),
                new Claim(ClaimTypes.Email, userRegistration.Email)
            };

            string resourceName = "UserService.Keys.private.pem";
            var privateKey = CreateStringFromEmbeddedResource(Assembly.GetExecutingAssembly(), resourceName);
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKey.ToCharArray());
                var descriptor = new SecurityTokenDescriptor
                {
                    //Subject = new ClaimsIdentity(claims),
                    Issuer = issuer,
                    Claims = new Dictionary<string, object>()
                    {
                        [ClaimTypes.Name] = userRegistration.Name,
                        [ClaimTypes.NameIdentifier] = userRegistration.Name,
                        [ClaimTypes.Email] = userRegistration.Email,
                        ["aud"] = string.Join(" ", userRegistration.Audiences),
                    },
                    SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa),
                                                                    SecurityAlgorithms.RsaSha256)
                    {
                        CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                    },
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.Now.AddMinutes(15)
                };

                var handler = new JwtSecurityTokenHandler();
                token = handler.WriteToken(handler.CreateJwtSecurityToken(descriptor));
            }
            return token;
        }
        private string GenerateToken(UserRegistration userRegistration)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Name, userRegistration.Name),
                new Claim(ClaimTypes.NameIdentifier, userRegistration.Name),
                new Claim(ClaimTypes.Email, userRegistration.Email)
            };
            var token = new JwtSecurityToken(issuer, string.Join(" ", userRegistration.Audiences), claims, expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string CreateStringFromEmbeddedResource(Assembly assembly, string resourceName)
        {
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader fileReader = new StreamReader(stream);

            // reset the position and read the entire file
            fileReader.BaseStream.Position = 0;
            var text = fileReader.ReadToEnd();

            // create the string content
            return text;
        }
    }
}
