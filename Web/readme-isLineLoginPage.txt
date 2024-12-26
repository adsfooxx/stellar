How to use this template
===

### modify  file  \appsettings.json

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "LINE-Login-Setting": {
    "Channel_ID": "1655339238",
    "Channel_Secret": "49dec3951dfe6759dda05728a7c07a56",
    "CallbackURL": "https://localhost:5001/SSOcallback"
  }
}

### modify  file  \Startup.cs
#region ForSignIn
using Microsoft.AspNetCore.Authentication.Cookies;
#endregion

public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            #region ForSignIn
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            #endregion
        }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            #region ForSignIn
            app.UseCookiePolicy();
            app.UseAuthentication();
            #endregion
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

### modify  file \Pages\Shared\_Layout.cshtml
                <div class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Login">Login/Logout</a>
                        </li>
                    </ul>
                </div>
                <div>
                    current user : @User.Identity.Name
                </div>