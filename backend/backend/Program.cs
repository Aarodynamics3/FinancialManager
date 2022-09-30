using backend.Models;
using Going.Plaid;
using Environment = Going.Plaid.Environment;

var builder = WebApplication.CreateBuilder(args);

var plaidOptions = new PlaidOptions() {
    Environment = Environment.Sandbox,
    ClientId = builder.Configuration["Plaid:ClientId"],
    Secret = builder.Configuration["Plaid:SandboxKey"]
};

var plaidClient = new PlaidClient(
    plaidOptions.Environment,
    clientId: plaidOptions.ClientId,
    secret: plaidOptions.Secret
);

// Add services to the container.
builder.Services.AddSingleton(plaidOptions);
builder.Services.AddSingleton(new PlaidCredentials());
builder.Services.AddSingleton(x => plaidClient);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", policy => {
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
// https://www.nolanbradshaw.ca/net-react-typescript-template
// https://learn.microsoft.com/en-us/visualstudio/javascript/tutorial-asp-net-core-with-react?view=vs-2022
// https://github.com/plaid/react-plaid-link/blob/665835e398a45b349ad6a92c6af8b2ac9f6d6d7b/examples/simple.tsx
// https://www.npmjs.com/package/react-plaid-link

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
