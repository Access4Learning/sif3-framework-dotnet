using Sif.Framework.AspNetCore.Formatters;
using Sif.Framework.Demo.AspNet.Provider.Models;
using Sif.Framework.Demo.AspNet.Provider.Utils;
using Sif.Framework.Model.Settings;

IFrameworkSettings settings = FrameworkConfigFactory.CreateSettings();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options =>
    {
        options.OutputFormatters.Add(new ArrayOfOutputFormatter<SchoolInfo>(settings));
        options.OutputFormatters.Add(new ArrayOfOutputFormatter<StudentPersonal>(settings));
    })
    .AddXmlSerializerFormatters();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();