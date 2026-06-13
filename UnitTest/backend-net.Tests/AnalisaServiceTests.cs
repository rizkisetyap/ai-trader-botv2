using backend_net.Data;
using backend_net.Objects.Dtos;
using backend_net.Objects.Models;
using backend_net.Services;
using Microsoft.EntityFrameworkCore;

namespace backend_net.Tests;

public class AnalisaServiceTests
{
    [Fact]
    public async Task GetAllAnalisaRequestsAsync_ReturnsAllSeededItems()
    {
        await using var context = CreateContext();
        SeedAnalisaRequest(context, Guid.Parse("11111111-1111-1111-1111-111111111111"), "BBCA");
        SeedAnalisaRequest(context, Guid.Parse("22222222-2222-2222-2222-222222222222"), "TLKM");
        await context.SaveChangesAsync();

        var service = new AnalisaService(context);

        var result = await service.GetAllAnalisaRequestsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Ticker == "BBCA");
        Assert.Contains(result, item => item.Ticker == "TLKM");
    }

    [Fact]
    public async Task GetAnalisaRequestByIdAsync_ReturnsMappedViewModel_WhenEntityExists()
    {
        var id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        await using var context = CreateContext();
        SeedAnalisaRequest(context, id, "ASII");
        await context.SaveChangesAsync();

        var service = new AnalisaService(context);

        var result = await service.GetAnalisaRequestByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.Equal("ASII", result.Ticker);
    }

    [Fact]
    public async Task GetAnalisaRequestByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
    {
        await using var context = CreateContext();
        var service = new AnalisaService(context);

        var result = await service.GetAnalisaRequestByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAnalisaRequestAsync_SavesNewEntity()
    {
        await using var context = CreateContext();
        var service = new AnalisaService(context);

        var request = new AnalisaRequestDto
        {
            Ticker = "BMRI",
            PrediksiTeknikal = "Bullish",
            ProbabilitasKenaikan = 0.82m,
            Sentimen = "Positif",
            Tanggal = new DateTime(2026, 06, 13, 0, 0, 0, DateTimeKind.Utc)
        };

        await service.AddAnalisaRequestAsync(request);

        var savedEntity = await context.AnalisaRequests.SingleAsync();

        Assert.Equal("BMRI", savedEntity.Ticker);
        Assert.Equal("Bullish", savedEntity.PrediksiTeknikal);
        Assert.Equal(0.82m, savedEntity.ProbabilitasKenaikan);
        Assert.Equal("Positif", savedEntity.Sentimen);
    }

    [Fact]
    public async Task DeleteAnalisaRequestAsync_ReturnsTrueAndRemovesEntity_WhenEntityExists()
    {
        var id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        await using var context = CreateContext();
        SeedAnalisaRequest(context, id, "UNVR");
        await context.SaveChangesAsync();
        var service = new AnalisaService(context);

        var result = await service.DeleteAnalisaRequestAsync(id);

        Assert.True(result);
        Assert.Empty(context.AnalisaRequests);
    }

    [Fact]
    public async Task DeleteAnalisaRequestAsync_ReturnsFalse_WhenEntityDoesNotExist()
    {
        await using var context = CreateContext();
        var service = new AnalisaService(context);

        var result = await service.DeleteAnalisaRequestAsync(Guid.NewGuid());

        Assert.False(result);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static void SeedAnalisaRequest(AppDbContext context, Guid id, string ticker)
    {
        context.AnalisaRequests.Add(new AnalisaRequestModel
        {
            Id = id,
            Ticker = ticker,
            PrediksiTeknikal = "Neutral",
            ProbabilitasKenaikan = 0.55m,
            Sentimen = "Netral",
            Tanggal = new DateTime(2026, 06, 13, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}