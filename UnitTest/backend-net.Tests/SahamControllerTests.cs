using backend_net.Data;
using backend_net.Objects.Dtos;
using backend_net.Objects.Models;
using backend_net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_net.Tests;

public class SahamControllerTests
{
    [Fact]
    public async Task RequestAnalisa_ReturnsAccepted_AndPersistsRequest()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var request = new AnalisaRequestDto
        {
            Ticker = "BBCA",
            PrediksiTeknikal = "Bullish",
            ProbabilitasKenaikan = 0.9m,
            Sentimen = "Positif",
            Tanggal = new DateTime(2026, 06, 13, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = await controller.RequestAnalisa(request);

        var accepted = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(202, accepted.StatusCode);

        var savedEntity = await context.AnalisaRequests.SingleAsync();
        Assert.Equal("BBCA", savedEntity.Ticker);
    }

    [Fact]
    public async Task GetAnalisaResult_ReturnsOk_WithAllRequests()
    {
        await using var context = CreateContext();
        SeedAnalisaRequest(context, Guid.Parse("11111111-1111-1111-1111-111111111111"), "BBCA");
        SeedAnalisaRequest(context, Guid.Parse("22222222-2222-2222-2222-222222222222"), "TLKM");
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        var result = await controller.GetAnalisaResult();

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsAssignableFrom<IEnumerable<backend_net.Objects.ViewModels.AnalisaRequestVM>>(ok.Value);
        Assert.Equal(2, payload.Count());
    }

    [Fact]
    public async Task GetAnalisaResultById_ReturnsOk_WhenRequestExists()
    {
        var id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        await using var context = CreateContext();
        SeedAnalisaRequest(context, id, "ASII");
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        var result = await controller.GetAnalisaResultById(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<backend_net.Objects.ViewModels.AnalisaRequestVM>(ok.Value);
        Assert.Equal(id, payload.Id);
        Assert.Equal("ASII", payload.Ticker);
    }

    [Fact]
    public async Task GetAnalisaResultById_ReturnsNotFound_WhenRequestMissing()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.GetAnalisaResultById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAnalisaResult_ReturnsNoContent_AndRemovesEntity()
    {
        var id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        await using var context = CreateContext();
        SeedAnalisaRequest(context, id, "UNVR");
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        var result = await controller.DeleteAnalisaResult(id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.AnalisaRequests);
    }

    [Fact]
    public async Task DeleteAnalisaResult_ReturnsNotFound_WhenRequestMissing()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.DeleteAnalisaResult(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    private static SahamController CreateController(AppDbContext context)
    {
        var service = new AnalisaService(context);
        return new SahamController(service);
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