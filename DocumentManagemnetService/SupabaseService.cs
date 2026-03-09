using DocumentManagemnetService;
using NLog;
using Supabase;
using System.Windows;

namespace DocumentManagementService
{
    public class SupabaseService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string url;
        private readonly string key;
        private Client client;
        private readonly SessionManager sessionManager = new();

        public SupabaseService()
        {
            var configuration = ((App)Application.Current).Configuration;
            key = configuration["API_KEY"];

            Logger.Info("Supabase API-ключ успешно загружен из хранилища");

            url = configuration["Supabase:Url"];

            Logger.Info("Supabase Url успешно загружен из файла конфигурации");

        }

        public async Task InitializeAsync()
        {
            try
            {
                var options = new SupabaseOptions //Настройки подключения
                {
                    AutoConnectRealtime = true,
                    AutoRefreshToken = true,
                };
                client = new Client(url, key, options);

                await client.InitializeAsync();

                Logger.Info($"Клиент Supabase успешно инициализирован");
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }

           
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
