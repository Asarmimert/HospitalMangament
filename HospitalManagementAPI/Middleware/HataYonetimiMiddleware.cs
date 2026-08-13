using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementAPI.Middleware
{
    public class HataYonetimiMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<HataYonetimiMiddleware>
            _logger;

        public HataYonetimiMiddleware(
            RequestDelegate next,
            ILogger<HataYonetimiMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException exception)
            {
                await HataCevabiYazAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Geçersiz istek",
                    exception.Message);
            }
            catch (InvalidOperationException exception)
            {
                await HataCevabiYazAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    "İşlem gerçekleştirilemedi",
                    exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Beklenmeyen bir hata oluştu.");

                await HataCevabiYazAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "Sunucu hatası",
                    "Beklenmeyen bir hata oluştu.");
            }
        }

        private static async Task HataCevabiYazAsync(
            HttpContext context,
            int durumKodu,
            string baslik,
            string detay)
        {
            context.Response.StatusCode = durumKodu;
            context.Response.ContentType =
                "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = durumKodu,
                Title = baslik,
                Detail = detay,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}