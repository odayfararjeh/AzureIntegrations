namespace CosmosHelper.Domain.DTO
{
    public record CosmosConfigurationDTO(
        string? CosmosConnectionString,
        string? CosmosDatabaseName,
        string? CosmosContainerName
        );
}
