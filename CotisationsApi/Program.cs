using Microsoft.OpenApi.Models;

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
        Description = "API pour calculer les cotisations URSSAF pour une profession lib�rale non r�glement�e en m�tropole. Suit les r�gles de https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html. Attention, la retraite compl�mentaire est calcul�e comme celle des artisans/commer�ants car c'est ce que fait l'URSSAF en ao�t 2024. J'ai demand� v�rification � l'URSSAF, le code sera mis � jour s'il s'av�re qu'il s'agit bien d'une erreur."
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

app.Run();


public partial class Program;