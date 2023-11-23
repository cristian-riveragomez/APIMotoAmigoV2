namespace WebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

             services.AddControllers();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443; 
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");
        }
    }
}
