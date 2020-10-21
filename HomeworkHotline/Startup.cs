using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Builder;

[assembly: OwinStartupAttribute(typeof(HomeworkHotline.Startup))]
namespace HomeworkHotline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
