using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString=builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options=>

options.UseSqlServer(connectionString)
);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", info: new OpenApiInfo
    {

        Version = "v1",
        Title = "TestApi",
        Description = "My first Api",
        TermsOfService = new Uri(uriString: "https://www.google.com"),
        Contact = new OpenApiContact
        {
            Name = "Hamza Zeyad",
            Email = "hamzaziyad9@gmail.com",
            Url = new Uri(uriString: "https://www.google.com"),
        },
        License = new OpenApiLicense
        {
            Name = "My License",
            Url = new Uri(uriString: "https://www.google.com"),
        }


    });
    option.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {

        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter you JWT Key"

    });

    option.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer",
                },
                Name="Bearer",
                In=ParameterLocation.Header,


            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();
