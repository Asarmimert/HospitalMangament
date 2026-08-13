using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Departments;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmanServisi _departmanServisi;

        public DepartmentsController(
            IDepartmanServisi departmanServisi)
        {
            _departmanServisi = departmanServisi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departmanlar =
                await _departmanServisi.TumunuGetirAsync();

            var cevap = departmanlar
                .Select(x => new DepartmentResponseDto
                {
                    DepartmentId = x.DepartmentId,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToList();

            return Ok(cevap);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var departman =
                await _departmanServisi.IdIleGetirAsync(id);

            if (departman is null)
            {
                return NotFound(
                    new { mesaj = "Departman bulunamadı." });
            }

            var cevap = new DepartmentResponseDto
            {
                DepartmentId = departman.DepartmentId,
                Name = departman.Name,
                Description = departman.Description
            };

            return Ok(cevap);
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateDepartmentDto dto)
        {
            var yeniDepartman = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var eklenenDepartman =
                await _departmanServisi.EkleAsync(
                    yeniDepartman);

            var cevap = new DepartmentResponseDto
            {
                DepartmentId =
                    eklenenDepartman.DepartmentId,

                Name = eklenenDepartman.Name,

                Description =
                    eklenenDepartman.Description
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = cevap.DepartmentId },
                cevap);
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateDepartmentDto dto)
        {
            var guncellenecekDepartman = new Department
            {
                DepartmentId = id,
                Name = dto.Name,
                Description = dto.Description
            };

            var guncellendiMi =
                await _departmanServisi.GuncelleAsync(
                    guncellenecekDepartman);

            if (!guncellendiMi)
            {
                return NotFound(
                    new { mesaj = "Departman bulunamadı." });
            }

            return NoContent();
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var silindiMi =
                await _departmanServisi.SilAsync(id);

            if (!silindiMi)
            {
                return NotFound(
                    new { mesaj = "Departman bulunamadı." });
            }

            return NoContent();
        }
    }
}