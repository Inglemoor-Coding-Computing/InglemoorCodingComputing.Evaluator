using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Security.Claims;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IAsyncCodeExecutionService, RemoteCodeExecutionService>();
builder.Services.AddSingleton<IUserLimitService, UserLimitService>();
builder.Services.AddSingleton<IResultCheckingService, ResultCheckingService>();
builder.Services.AddScoped<IExecutionLoggingService, ExecutionLoggingService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AuthenticationStateService>();
builder.Services.AddDbContext<ApiUserDbContext>();
builder.Services.AddDbContext<ExecutionResultDbContext>();
builder.Services.AddHttpClient<IAsyncCodeExecutionService, RemoteCodeExecutionService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "IHS-CCC:Evaluator API",
        Description = "Endpoints for executing user code."
    });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration["JWT:Secret"] ?? throw new Exception("Set the JWT Secret."))),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        options.Events = new()
        {
            OnTokenValidated = async ctx =>
            {
                var id = ctx.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id is null)
                {
                    ctx.Fail("Unknown user.");
                    return;
                }

                if (!Guid.TryParse(id, out var guid))
                {
                    ctx.Fail("Unable to parse id claim.");
                    return;
                }

                using ApiUserDbContext db = new();
                var user = await db.FindAsync<ApiUser>(guid);
                if (user is null)
                    ctx.Fail("User does not exist");
            }
        };
    });
builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("PruneExecutionResultsJob");
    q.AddJob<PruneExecutionResultsJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey).WithSimpleSchedule(a => a.WithIntervalInHours(1).RepeatForever())
        .WithIdentity("PruneExecutionResults-trigger")

    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    serviceScope.ServiceProvider.GetRequiredService<ApiUserDbContext>().Database.Migrate();
    serviceScope.ServiceProvider.GetRequiredService<ExecutionResultDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapRazorPages();
app.MapControllers();

app.MapFallbackToPage("/_Host");

app.Run();
