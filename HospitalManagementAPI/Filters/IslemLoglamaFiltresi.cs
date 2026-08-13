using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace HospitalManagementAPI.Filters
{
    public class IslemLoglamaFiltresi : IAsyncActionFilter
    {
        private readonly ILogger<IslemLoglamaFiltresi> _logger;

        public IslemLoglamaFiltresi(
            ILogger<IslemLoglamaFiltresi> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var controllerAdi =
                context.RouteData.Values["controller"]?.ToString();

            var actionAdi =
                context.RouteData.Values["action"]?.ToString();

            _logger.LogInformation(
                "{Controller} controllerındaki {Action} işlemi başladı.",
                controllerAdi,
                actionAdi);

            var kronometre = Stopwatch.StartNew();

            var sonuc = await next();

            kronometre.Stop();

            if (sonuc.Exception is null)
            {
                _logger.LogInformation(
                    "{Controller} controllerındaki {Action} işlemi " +
                    "{Sure} milisaniyede tamamlandı.",
                    controllerAdi,
                    actionAdi,
                    kronometre.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    "{Controller} controllerındaki {Action} işleminde " +
                    "hata oluştu. Süre: {Sure} milisaniye.",
                    controllerAdi,
                    actionAdi,
                    kronometre.ElapsedMilliseconds);
            }
        }
    }
}