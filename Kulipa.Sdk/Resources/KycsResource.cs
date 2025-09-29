namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Implementation of KYC operations.
    /// </summary>
    public class KycsResource : BaseResource, IKycsResource
    {
        /// <inheritdoc />
        public KycsResource(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}