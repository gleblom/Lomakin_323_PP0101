using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System.IO;
using System.Text.Json;

public class FileSessionHandler : IGotrueSessionPersistence<Session>
{
    private const string FilePath = "session.json";

    public void DestroySession()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    public Session? LoadSession()
    {
        if (!File.Exists(FilePath))
        {
            return null;
        }

        var content = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<Session>(content);
    }

    public void SaveSession(Session session)
    {
        var content = JsonSerializer.Serialize(session);
        File.WriteAllText(FilePath, content);
    }
}
