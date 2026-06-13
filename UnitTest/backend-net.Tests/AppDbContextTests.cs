using backend_net.Data;
using backend_net.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_net.Tests;

public class AppDbContextTests
{
    [Fact]
    public async Task CanSaveAnalisaRequestEntity()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        context.AnalisaRequests.Add(new AnalisaRequestModel
        {
            Id = Guid.NewGuid(),
            Ticker = "BBCA",
            PrediksiTeknikal = "Bullish",
            ProbabilitasKenaikan = 0.75m,
            Sentimen = "Positif",
            Tanggal = DateTime.UtcNow
        });

        var saved = await context.SaveChangesAsync();

        Assert.Equal(1, saved);
        Assert.Equal(1, await context.AnalisaRequests.CountAsync());
    }
}
