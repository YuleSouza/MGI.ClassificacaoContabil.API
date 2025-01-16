namespace API.Config
{
    public static class ApiBuilderExtensions
    {
        public static IApplicationBuilder UseBuilderConfiguration(this IApplicationBuilder app)
        {
            IWebHostEnvironment environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            app.UseSwagger();
            if (!environment.IsProduction())
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "APIs - ClassificacaoContabil");
                });
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors("ClientPermission");
            app.UseRouting();
            app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return app;
        }
    }

}
