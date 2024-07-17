using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddScoped<AuthService>();


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
app.MapGet("/login", (AuthService authService) => {
    authService.SignIn();
    return "login successful";
});

app.MapGet("/", (HttpContext httpContext) => {
    return "home";
});

app.Run();

public class AuthService
{
    private readonly IDataProtectionProvider _idp;
    private readonly IHttpContextAccessor _accessor; 

    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
    {
        _idp = idp;
        _accessor = accessor;
    }

    public void SignIn()
    {
        var protector = _idp.CreateProtector("auth-cookie");
        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:jpmontoya182")}";

    }

    
}