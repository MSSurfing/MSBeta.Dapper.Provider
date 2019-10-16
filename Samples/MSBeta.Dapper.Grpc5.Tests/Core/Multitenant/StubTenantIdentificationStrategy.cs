namespace MSBeta.Dapper.Grpc5.Tests.Core.Multitenant
{
    public class StubTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        public StubTenantIdentificationStrategy()
        {
            this.IdentificationSuccess = true;
        }

        public bool IdentificationSuccess { get; set; }

        public object TenantId { get; set; }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = this.TenantId;
            return this.IdentificationSuccess;
        }
    }
}
