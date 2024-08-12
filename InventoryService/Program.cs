var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 builder.Services.AddControllers(options => options.Filters.Add(new AuthorizeFilter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// to enable authentication
builder.Services.AddAuthentication(authOptions =>
                {
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {

                    bool isRSA = bool.Parse(builder.Configuration["isRSA"]);
                    var metadata = GetIssuerAndKeys(isRSA, builder.Configuration["TokenIssuerUrl"]);
                    var audiences = builder.Configuration["audiences"];
                    string[] auds = audiences.Split(" ");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudiences = auds,
                        //ValidAudience = audiences,
                        ValidIssuer = metadata.issuer,
                        IssuerSigningKey = isRSA ? GetRSASecurityKey(metadata.key) : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(metadata.key)),
                        AudienceValidator = ValidateAudience
                    };
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();


static SecurityKey GetRSASecurityKey(string key)
{
    RSA rsa = RSA.Create();
    rsa.ImportFromPem(key);
    return new RsaSecurityKey(rsa);
}

static bool ValidateAudience(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
{
    var audInToken = audiences.SelectMany(x => x.Split(" "));
    var matchedAud = audInToken.Where(x => validationParameters.ValidAudiences.Contains(x));
    return matchedAud.Any();
}

static (string key, string issuer) GetIssuerAndKeys(bool isRSA, string issuerUrl)
{
    using (var client = new HttpClient())
    {
        var queryString = isRSA ? "?algo=RSA" : "";
        client.BaseAddress = new Uri(issuerUrl);
        var metadataResponse = Task.Run(() => client.GetAsync($"/api/issuer/metadata{queryString}")).ConfigureAwait(false).GetAwaiter().GetResult();
        var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(metadataResponse.Content.ReadAsStringAsync().Result);
        var key = metadata["key"];
        var issuer = metadata["issuer"];
        return (key, issuer);
    }
}