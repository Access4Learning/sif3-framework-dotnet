# ASP.NET Core SIF Providers

## Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();