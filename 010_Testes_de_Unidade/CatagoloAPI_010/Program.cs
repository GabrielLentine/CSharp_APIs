using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CatagoloAPI.Context;
using CatagoloAPI.Extensions;
using CatagoloAPI.Filters;
using CatagoloAPI.Logs;
using CatagoloAPI.Models;
using CatagoloAPI.Repositories;
using CatagoloAPI.Services;
using CatagoloAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(op => { op.Filters.Add(typeof(ApiExceptionFilter)); })
    .AddJsonOptions(op => { op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

// habilitando o CORS
var origensComAcessoPermitido = "_origensComAcessoPermitido";
builder.Services.AddCors(op => 
{
    // definindo uma política de CORS com um nome específico (origensComAcessoPermitido) e configurando as origens permitidas
    op.AddPolicy(name: origensComAcessoPermitido , policy =>
    {
        policy.WithOrigins("https://apirequest.io"); // permitindo apenas requisições vindas do domínio "https://apirequest.io"
        // policy.AllowAnyOrigin(); permitindo requisições de qualquer origem (descomente esta linha se quiser permitir todas as origens)
        // policy.WithOrigins("https://apirequest.io").AllowAnyHeader(); permitindo requisições vindas do domínio "https://apirequest.io" e permitindo qualquer cabeçalho nas requisições
        // policy.WithOrigins("https://apirequest.io").WithHeaders("HeadersName.ContentyType", "x-meu-header"); permitindo requisições vindas do domínio "https://apirequest.io" e permitindo apenas os cabeçalhos "HeadersName.ContentyType" e "x-meu-header" nas requisições
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1" , new OpenApiInfo { Title = "CatagoloAPI" , Version = "v1" }); // versão e nome da API (documentação)

    // adicionando uma definição de segurança (chamada Bearer) p/ indicar que a API usa autenticação JWT Bearer
    c.AddSecurityDefinition("Bearer" , new OpenApiSecurityScheme()
    {
        Name = "Authorization" , // nome do cabeçalho onde o token será enviado
        Type = SecuritySchemeType.ApiKey , // indica o tipo de esquema de segurança (usando uma chave de API - ApiKey)
        Scheme = "Bearer" , // portador do token (Bearer)
        BearerFormat = "JWT" , // formato do token (JWT)
        In = ParameterLocation.Header , // o token será incluído no cabeçalho do request (header)
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    // adicionando requeisitos de segurança p/ indicar que a API requer autenticação via Bearer p/ acessar os endpoints protegidos
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme , // referenciando o esquema de segurança
                    Id = "Bearer" // id do esquema de segurança (Bearer), definino em AddSecurityDefinition("Bearer")
                }
            } ,
            new string[] { }
        }
    });
});

// configurando o banco de dados com MySQL
var mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// obtendo a string de conex�o do arquivo appsettings.json

// definindo o contexto + o provedor (MySQL) + a string de conex�o = DI do AppDbContext aonde eu precisar dela (neste caso, nos reposit�rios)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionString,
        ServerVersion.AutoDetect(mySqlConnectionString)));

// registrando o filtro global ApiLoggingFilter para registrar logs de requisi��es e respostas
builder.Services.AddScoped<ApiLoggingFilter>();

// registrando o servi�o MeuServico como uma dependência transitória (uma nova inst�ncia ser� criada a cada vez que for injetada)
builder.Services.AddTransient<IMeuServico, MeuServico>();

builder.Services.AddScoped<ITokenService, TokenService>();

// registrando o repositório para as categorias
// toda vez que eu referenciar ICategoriaRepository, eu vou receber uma instância com as implementações presentes em CategoriaRepository
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// registrando o repositório para os produtos
// toda vez que eu referenciar IProdutoRepository, eu vou receber uma instância com as implementações presentes em ProdutoRepository
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// registrando o repositório genérico
// toda vez que eu referenciar IRepository, eu vou receber uma instância com as implementações presentes em Repository, de acordo com a classe a ser chamada
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// registrando o repositório de UnityOfWork
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();

// registrando o AutoMapper e mapeando todos os perfis de mapeamento presentes no projeto
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// registrando o Identity
// configurando usando o "IdentityUser" p/ representar os usuários e "IdentityRole" p/ representar as funções/perfis de cada usuário
builder.Services.AddIdentity<ApplicationUser , IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>() // EF Core configurado p/ armazenar os dados com base no AppDbContex
    .AddDefaultTokenProviders(); // provedores de tokens padrão p/ lidar com os processos de autenticação

// configurando o JWT Bearrer na aplicação: primeiro, estou obtendo a chave secreta do arquivo appsettings.json
var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Chave secreta inválida!");
builder.Services.AddAuthentication(op =>
{
    // por padrão, o esquema de autenticação e desafio será o JwtBearer
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    // se alguém tentar usar um recurso protegido sem fornecer o token, o desafio é solicitar as credenciais do usuário
}).AddJwtBearer(op =>
{
    op.SaveToken = true; // salva o token se a autenticação é bem sucedida
    op.RequireHttpsMetadata = false; // indica se é preciso HTTPS p/ transmitir o token (em produção, deve ser true)
    op.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true , // valida o emissor do token
        ValidateAudience = true , // valida o público do token
        ValidateLifetime = true , // valida o tempo de vida do token
        ValidateIssuerSigningKey = true , // valida a chave de assinatura do emissor do token
        ClockSkew = TimeSpan.Zero , // ajustar o tempo p/ tratar algumas diferenças de tempo entre o servidor de autenticação e o da aplicação
        ValidAudience = builder.Configuration["JWT:ValidAudience"] , // obtendo o valor do público válido do token a partir do appsettings.json
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"] , // obtendo o valor do emissor válido do token a partir do appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
        // obtendo a chave secreta do token a partir do appsettings.json, além de condificá-la em UTF8 e criar a chave simétrica de segurança
    };
});

// registrando a autenticação com JWT Bearer (e a política de autorização)
builder.Services.AddAuthorization(op =>
{
    // apenas usuários com a role Admin podem acessar determinado endpoint
    op.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    // RequireClaim() = o usuário terá uma claim específica p/ acessar um recurso protegido
    op.AddPolicy("SuperAdminOnly" , policy => policy.RequireRole("Admin").RequireClaim("id" , "lentine"));

    // apenas usuários com a role User podem visualizar determinado endpoint
    op.AddPolicy("UserOnly" , policy => policy.RequireRole("User"));

    // RequireAssertion() = permite definir uma expressão lambda e com uma condição customizada p/ autorização
    // neste caso, apenas o usuário "lentine" ou todos aqueles que estão na role SuperAdmin
    op.AddPolicy("ExclusiveOnly" , policy => policy.RequireAssertion
                                                    (context => 
                                                        context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "lentine"
                                                        || context.User.IsInRole("SuperAdmin"))));
});

// criando o Rate Limiting
builder.Services.AddRateLimiter(rateLimitOp =>
{
    rateLimitOp.AddFixedWindowLimiter("FixedWindow" , op =>
    {
        op.PermitLimit = 1; // número máximo de requisições permitidas dentro da janela de tempo definida (neste caso, 1 requisição)
        op.Window = TimeSpan.FromSeconds(5); // duração da janela de tempo (neste caso, 5 segundos). Após esse período, o contador de requisições é resetado e o cliente pode fazer novas requisições
        op.QueueLimit = 2; // número máximo de requisições que podem ser enfileiradas quando o limite de requisições é atingido (neste caso, 2 requisições). Se o limite for atingido e a fila estiver cheia, as requisições adicionais serão rejeitadas imediatamente
        op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // ordem de processamento das requisições enfileiradas (neste caso, as requisições mais antigas são processadas primeiro)
    });
    rateLimitOp.RejectionStatusCode = 429; // status code para indicar que a requisição foi rejeitada por causa do limite de taxa (Too Many Requests)
});

builder.Services.AddRateLimiter(op =>
{
    op.RejectionStatusCode = 429; // status code para indicar que a requisição foi rejeitada por causa do limite de taxa (Too Many Requests)

    // criando um limitador de taxa global que é aplicado a todas as requisições, usando o PartitionedRateLimiter para criar partições de limite de taxa com base em uma chave específica (neste caso, o nome do usuário autenticado ou o host da requisição)
    op.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext , string>(httpContext => 
                       RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString() ,
                       factory: partition => new FixedWindowRateLimiterOptions
                       {
                           AutoReplenishment = true , // indica se o contador de requisições deve ser reabastecido automaticamente após a janela de tempo definida (neste caso, true)
                           PermitLimit = 5 , // número máximo de requisições permitidas dentro da janela de tempo definida (neste caso, 5 requisições)
                           QueueLimit = 0 , // número máximo de requisições que podem ser enfileiradas quando o limite de requisições é atingido (neste caso, 0 requisições). Se o limite for atingido e a fila estiver cheia, as requisições adicionais serão rejeitadas imediatamente
                           Window = TimeSpan.FromSeconds(10) // duração da janela de tempo (neste caso, 10 segundos). Após esse período, o contador de requisições é resetado e o cliente pode fazer novas requisições
                       }));
});

// obtendo os valores das chaves do arquivo appsettings.json
var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave2"];


// registrando a configura��o do provedor de log personalizado
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // middleware para gerar a documenta��o Swagger
    app.UseSwaggerUI(); // middleware para exibir a interface do Swagger
    // app.UseDeveloperExceptionPage(); -> middleware para exibir a p�gina de erro detalhada
    app.ConfigureExceptionHandler(); // middleware para tratar exce��es de forma personalizada
}

app.UseHttpsRedirection(); // middleware para redirecionar requisi��es HTTP para HTTPS
app.UseStaticFiles(); // middleware para servir arquivos estáticos (como imagens, CSS, JavaScript) a partir da pasta wwwroot
app.UseRouting(); // middleware para habilitar o roteamento das requisições
app.UseRateLimiter(); // middleware para habilitar o Rate Limiting com a política definida anteriormente
app.UseCors(origensComAcessoPermitido); // middleware para habilitar o CORS com a política definida anteriormente
app.UseAuthentication(); // middleware para autenticar requisições
app.UseAuthorization(); // middleware para autorizar requisições (necessário se você tiver autenticações/autorizações configuradas)
app.MapControllers(); // mapear os controladores para as rotas
app.Run();
