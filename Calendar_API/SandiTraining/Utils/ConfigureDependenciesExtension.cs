using SandiTraining.Business;
using SandiTraining.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SandiTraining.Utils
{
    public static class ConfigureDependenciesExtension
    {
        public static void ConfigureDependencyInjections(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IAppointmentBL, AppointmentBL>();
            services.AddScoped<IAppointmentDAL, AppointmentDAL>();
        }
    }
}
