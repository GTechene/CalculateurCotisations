using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de cotisations URSSAF",
        Version = "v1",
        Description = "API pour calculer les cotisations URSSAF pour une profession libérale non réglementée en métropole. Suit les règles de https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html. Attention, la retraite complémentaire est calculée comme celle des artisans/commerçants car c'est ce que fait l'URSSAF en août 2024. J'ai demandé vérification à l'URSSAF, le code sera mis à jour s'il s'avère qu'il s'agit bien d'une erreur."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.EnableTryItOutByDefault();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseFileServer();

app.Run();


public partial class Program;
