using Supabase;

namespace DocumentManagemnetService
{
    public class SupabaseService
    {
        private string url = "https://kphkeykctqyqgfotrqoy.supabase.co";
        private string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtwaGtleWtjdHF5cWdmb3RycW95Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA0ODc4NTQsImV4cCI6MjA2NjA2Mzg1NH0.N9tcPHuG6VXdOBiiC8UWS7ISxTTZnNKoWIarWX9bOAw";
        private Client client;

        public async Task InitializeAsync()
        {

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false,
                SessionHandler = new FileSessionHandler()
            };
            client = new Client(url, key, options);
            await client.InitializeAsync();
        }

        public Client Client => client;
        public bool IsAuthenticated => client.Auth.CurrentUser != null;
    }
}
