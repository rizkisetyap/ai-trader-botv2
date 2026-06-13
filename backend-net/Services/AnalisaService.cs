using backend_net.Data;
using backend_net.Objects.Dtos;
using backend_net.Objects.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace backend_net.Services
{
    public class AnalisaService
    {
        private readonly AppDbContext _context;

        public AnalisaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AnalisaRequestVM>> GetAllAnalisaRequestsAsync()
        {
            return await _context.AnalisaRequests.Select(a => new AnalisaRequestVM
            {
                Id = a.Id,
                Ticker = a.Ticker,
                PrediksiTeknikal = a.PrediksiTeknikal,
                ProbabilitasKenaikan = a.ProbabilitasKenaikan,
                Sentimen = a.Sentimen,
                Tanggal = a.Tanggal
            }).ToListAsync();
        }
        public async Task<AnalisaRequestVM?> GetAnalisaRequestByIdAsync(Guid id)
        {
            var entity = await _context.AnalisaRequests.FindAsync(id);
            if (entity == null) return null;

            return new AnalisaRequestVM
            {
                Id = entity.Id,
                Ticker = entity.Ticker,
                PrediksiTeknikal = entity.PrediksiTeknikal,
                ProbabilitasKenaikan = entity.ProbabilitasKenaikan,
                Sentimen = entity.Sentimen,
                Tanggal = entity.Tanggal
            };
        }
        public async Task AddAnalisaRequestAsync(AnalisaRequestDto vm)
        {
            var entity = new Objects.Models.AnalisaRequestModel
            {
                Id = Guid.NewGuid(),
                Ticker = vm.Ticker,
                PrediksiTeknikal = vm.PrediksiTeknikal,
                ProbabilitasKenaikan = vm.ProbabilitasKenaikan,
                Sentimen = vm.Sentimen,
                Tanggal = DateTime.UtcNow
            };

            _context.AnalisaRequests.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAnalisaRequestAsync(Guid id)
        {
            var entity = await _context.AnalisaRequests.FindAsync(id);
            if (entity == null) return false;

            _context.AnalisaRequests.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}