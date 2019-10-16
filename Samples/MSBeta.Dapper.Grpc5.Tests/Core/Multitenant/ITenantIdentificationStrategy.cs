namespace MSBeta.Dapper.Grpc5.Tests.Core.Multitenant
{
    public interface ITenantIdentificationStrategy
    {
        // Try to get TenantId
        bool TryIdentifyTenant(out object tenantId);
    }
}
