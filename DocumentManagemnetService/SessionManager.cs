using Supabase.Gotrue;
using System.IO;
using System.Text.Json;

namespace DocumentManagementService
{
    public class SessionManager //Класс для управления сессией
    {
        private readonly string sessionFilePath;

        public SessionManager()
        {
            //Путь для сохранения cессии - AppData/Roaming/DocumentManagementService
            var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DocumentManagementService");
            Directory.CreateDirectory(folder);
            sessionFilePath = Path.Combine(folder, "session.json");
        }
        public void DestroySession() //Удаление сессии
        {
            if (File.Exists(sessionFilePath))
            {
                File.Delete(sessionFilePath);
            }

        }

        public async Task<Session?> LoadSessionAsync()
        {
            if (!File.Exists(sessionFilePath))
            {
                return null;
            }
            try
            {
                var content = await File.ReadAllTextAsync(sessionFilePath);
                return JsonSerializer.Deserialize<Session>(content);
            }
            catch
            {
                return null;
            }

        }

        public async Task SaveSessionAsync(Session session)
        {
            var content = JsonSerializer.Serialize(session);
            await File.WriteAllTextAsync(sessionFilePath, content);
        }
    }
}