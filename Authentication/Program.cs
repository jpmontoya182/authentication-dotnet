using Microsoft.AspNetCore.DataProtection;
using System.Runtime.Intrinsics.Arm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();


var app = builder.Build();

app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");

    string authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth=")) ?? string.Empty;
    if (string.IsNullOrEmpty(authCookie))
    {
        return "Authenticate first !!";
    }

    string? protectedPayload = authCookie.Split('=').Last();
    var payload = protector.Unprotect(protectedPayload);
    string[]? parts = payload.Split(':');
    string key = parts[0];
    string value = parts[1];
    return value;
});
app.MapGet("/login", (HttpContext httpContext, IDataProtectionProvider idp) => {
    var protector = idp.CreateProtector("auth-cookie");
    httpContext.Response.Headers.SetCookie = $"auth={protector.Protect("usr:jpmontoya182")}";
    return "login successful";
});

app.MapGet("/", (HttpContext httpContext) => {
    return "home";
});



app.Run();
