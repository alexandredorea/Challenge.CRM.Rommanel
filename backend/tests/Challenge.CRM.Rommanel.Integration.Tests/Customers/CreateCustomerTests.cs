using Challenge.CRM.Rommanel.Api.DTOs;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Integration.Tests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Challenge.CRM.Rommanel.Integration.Tests.Customers;

[Collection("Integration")]
public sealed class CreateCustomerTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Post_WithValidPf_ShouldReturn201AndCustomerDto()
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
            .PostAsJsonAsync("/api/customers", request, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be(request.Name);
        result.Data.DocumentFormatted.Should().Be(request.Document);
        result.Data.Active.Should().BeTrue();

        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_WithDuplicateDocument_ShouldReturn409()
    {
        var email1 = $"{Guid.NewGuid()}@email.com";
        var email2 = $"{Guid.NewGuid()}@email.com";

        var request = new CreateCustomerRequest(
            "Teste Duplicado",
            "529.982.247-25",
            new DateOnly(1982, 5, 6),
            email1,
            "71999998888",
            "41810010",
            "Rua das Flores",
            "123",
            "Pituba",
            "Salvador",
            "BA");

        // Primeira inserção
        await fixture.Client.PostAsJsonAsync("/api/customers", request, CancellationToken.None);

        // Segunda com mesmo documento
        var duplicate = request with { Email = email2 };
        var response = await fixture.Client
            .PostAsJsonAsync("/api/customers", duplicate, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Post_WithInvalidDocument_ShouldReturn422()
    {
        var request = new CreateCustomerRequest(
            "Teste Inválido",
            "111.111.111-11",
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
            .PostAsJsonAsync("/api/customers", request, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Post_WithValidPj_ShouldReturnLegalEntityType()
    {
        var request = new CreateCustomerRequest(
            "Empresa LTDA",
            "11.222.333/0001-81",
            new DateOnly(1982, 5, 6),
            $"{Guid.NewGuid()}@email.com",
            "71999998888",
            "41810010",
            "Rua das Flores",
            "123",
            "Pituba",
            "Salvador",
            "BA",
            "110.042.490.114");

        var response = await fixture.Client
            .PostAsJsonAsync("/api/customers", request, CancellationToken.None);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        result!.Data!.DocumentType.Should().Be(nameof(DocumentType.LegalEntity));
    }
}
