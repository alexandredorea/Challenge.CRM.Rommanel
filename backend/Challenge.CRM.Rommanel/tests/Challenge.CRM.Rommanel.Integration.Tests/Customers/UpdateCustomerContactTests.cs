using Challenge.CRM.Rommanel.Api.DTOs;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Integration.Tests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Challenge.CRM.Rommanel.Integration.Tests.Customers;

[Collection("Integration")]
public sealed class UpdateCustomerContactTests(IntegrationTestFixture fixture)
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
            .PostAsJsonAsync("/api/customers", request, CancellationToken.None);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        return result!.Data!;
    }

    [Fact]
    public async Task Patch_WithNewAddress_ShouldUpdateAndReturn200()
    {
        var customer = await CreateCustomerAsync();
        var newAddr = new UpdateCustomerAddressRequest("40255-265", "Rua Lalita Costa", "145", "Vila Laura", "Salvador", "BA");

        var response = await fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}/address",
            newAddr, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        result!.Data!.Address.PostalCode.Should().Be(newAddr.PostalCode);
        result!.Data!.Address.Street.Should().Be(newAddr.Street);
        result!.Data!.Address.Number.Should().Be(newAddr.Number);
        result!.Data!.Address.Neighborhood.Should().Be(newAddr.Neighborhood);
        result!.Data!.Address.City.Should().Be(newAddr.City);
        result!.Data!.Address.FederativeUnit.Should().Be(newAddr.FederativeUnit);
    }

    [Fact]
    public async Task Patch_WithNewEmail_ShouldUpdateAndReturn200()
    {
        var customer = await CreateCustomerAsync();
        var newEmail = $"{Guid.NewGuid()}@novo.com.br";

        var response = await fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}/email",
            new UpdateCustomerEmailRequest(newEmail), CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        result!.Data!.Email.Should().Be(newEmail);
    }

    [Fact]
    public async Task Patch_WithNewTelephone_ShouldUpdateAndReturn200()
    {
        var customer = await CreateCustomerAsync();
        var newTelephone = "71998877665";

        var response = await fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}/telephone",
            new UpdateCustomerTelephoneRequest(newTelephone), CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CustomerDto>>(JsonOpts, CancellationToken.None);

        result!.Data!.Telephone.Should().Be(newTelephone);
    }

    [Fact]
    public async Task Patch_WithNonExistentId_ShouldReturn404()
    {
        var response = await fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{Guid.NewGuid()}/telephone",
            new UpdateCustomerTelephoneRequest("71999998888"), CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}