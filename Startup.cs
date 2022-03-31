using System.Web.Http;
using Owin;

namespace Host_Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll); 
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            appBuilder.UseWebApi(config); 
        } 
    }
}
