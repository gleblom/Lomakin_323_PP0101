using NLog;
using Supabase;

namespace DocumentManagementService
{
    public class SupabaseService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string url = "https://kphkeykctqyqgfotrqoy.supabase.co";
        private readonly string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtwaGtleWtjdHF5cWdmb3RycW95Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA0ODc4NTQsImV4cCI6MjA2NjA2Mzg1NH0.N9tcPHuG6VXdOBiiC8UWS7ISxTTZnNKoWIarWX9bOAw";
        private Client client;
        private readonly SessionManager sessionManager = new();

        public async Task InitializeAsync()
        {

            var options = new SupabaseOptions //Настройки подключения
            {
                AutoConnectRealtime = true,
                AutoRefreshToken = true,            
            };
            client = new Client(url, key, options);
            await client.InitializeAsync();

           
            var session = await sessionManager.LoadSessionAsync();
            if (session != null)
            {
                try
                {
                    await client.Auth.SetSession(session.AccessToken, session.RefreshToken);
                }
                catch(Exception ex) 
                {
                    Logger.Warn("Не удалось загрузить сессию");
                    Logger.Error(ex.Message);
                    DestroySession();
                }
            }
        }
        public async Task SaveSessionAsync()
        {
            var session = client.Auth.CurrentSession;
            if (session != null)
            {
                await sessionManager.SaveSessionAsync(session);
            }
        }
        public void DestroySession()
        {
            sessionManager.DestroySession();
        }
        public async Task EnsureSessionIsValidAsync() //Проверка валидности сессии
        {
            var session = client.Auth.CurrentSession;

            if (session == null || string.IsNullOrWhiteSpace(session.AccessToken))
                return;

            if (session.Expired()) //Если время сессии истекло
            {
                try
                {
                    await client.Auth.RefreshSession(); //обновляем её
                    await SaveSessionAsync();
                }
                catch(Exception ex)
                {
                    Logger.Warn("Ошибка проверки валидности сессии");
                    Logger.Error(ex.Message);
                    DestroySession(); //Если что-то пошло не так, то удаляем
                }
            }
        }
        public Client Client => client;
        public bool IsAuthenticated => client.Auth.CurrentUser != null;
    }
}
