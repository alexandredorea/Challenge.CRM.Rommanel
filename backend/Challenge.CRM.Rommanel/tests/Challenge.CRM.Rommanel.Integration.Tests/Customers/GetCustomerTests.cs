using Challenge.CRM.Rommanel.Api.DTOs;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Events;
using Challenge.CRM.Rommanel.Integration.Tests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Challenge.CRM.Rommanel.Integration.Tests.Customers;

[Collection("Integration")]
public sealed class GetCustomerTests(IntegrationTestFixture fixture)
: IClassFixture<IntegrationTestFixture>
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private async Task<CustomerDto> CreateCustomerAsync()
    {
        var request = new CreateCustomerRequest(
                "Alexandre Dórea",
                "529.982.247-25",
                new DateOnly(1982, 5, 6),
                $"{Guid.NewGuid()}@email.com",
                "71999998888",
                "41810010",
                "Rua das Flores",
                "123",
                "Pituba",
                "Salvador",
                "BA");

        var response = await fixture.Client
            .PostAsJsonAsync("/api/customers", request);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts);

        return result!.Data!;
    }

    [Fact]
    public async Task GetById_WithExistentId_ShouldReturn200()
    {
        var customer = await CreateCustomerAsync();

        // Busca por Id
        var response = await fixture.Client
            .GetAsync($"/api/customers/{customer.Id}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ShouldReturn404()
    {
        var response = await fixture.Client
            .GetAsync($"/api/customers/{Guid.NewGuid()}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task List_ShouldReturnPagedResult()
    {
        var response = await fixture.Client
            .GetAsync("/api/customers?page=1&pageSize=10", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<Result<PagedResult<CustomerDto>>>(JsonOpts, CancellationToken.None);

        result!.Success.Should().BeTrue();
        result.Data!.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetEventHistory_AfterCreate_ShouldReturnOneEvent()
    {
        var customer = await CreateCustomerAsync();

        var response = await fixture.Client
            .GetAsync($"/api/customers/{customer.Id}/events", CancellationToken.None);

        var result = await response.Content
            .ReadFromJsonAsync<Result<List<CustomerEventDto>>>(JsonOpts, CancellationToken.None);

        result!.Data.Should().HaveCount(1);
        result.Data![0].EventType.Should().Be(nameof(CustomerCreated));
    }
}