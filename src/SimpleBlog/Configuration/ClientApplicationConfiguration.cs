namespace SimpleBlog.Configuration
{
    public class ClientApplicationConfiguration
    {
        public static string SectionName = "ClientApplications";

        public string Id { get; private set; } = string.Empty;
        public string Secret { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Permissions { get; private set; } = string.Empty;

        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Permissions));
        }
    }
}
