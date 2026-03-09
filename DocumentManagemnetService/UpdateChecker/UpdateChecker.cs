using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumentManagementService.UpdateChecker
{
    public class UpdateChecker
    {
        private readonly string manifestUrl; // e.g. "https://github.com/gleblom/Lomakin_323_PP0101/releases/latest/download/manifest.json"
        private readonly string updaterExeName = "Updater.exe"; // убедитесь, что updater рядом с app.exe

        public async Task CheckAndUpdateAsync()
        {
            try
            {
                using var http = new HttpClient();
                var manifestJson = await http.GetStringAsync(manifestUrl);
                var manifest = JsonSerializer.Deserialize<UpdateManifest>(manifestJson);

                var current = Assembly.GetEntryAssembly().GetName().Version;
                // parse manifest.version to System.Version (fallback safe)
                if (!Version.TryParse(manifest.version, out var latest)) return;

                if (latest > current)
                {
                    // download zip to temp
                    var tempFolder = Path.Combine(Path.GetTempPath(), "MyAppUpdate", manifest.version);
                    Directory.CreateDirectory(tempFolder);
                    var packageFileName = Path.GetFileName(new Uri(manifest.url).LocalPath);
                    var downloadPath = Path.Combine(tempFolder, packageFileName);

                    await DownloadFileAsync(manifest.url, downloadPath, http);

                    // verify sha256
                    var ok = await VerifySha256Async(downloadPath, manifest.sha256);
                    if (!ok) throw new Exception("SHA256 mismatch");

                    // locate updater
                    var appExe = Process.GetCurrentProcess().MainModule.FileName;
                    var updaterPath = Path.Combine(Path.GetDirectoryName(appExe), updaterExeName);
                    if (!File.Exists(updaterPath))
                        throw new FileNotFoundException("Updater not found next to app executable", updaterPath);

                    // start updater with args: packagePath appExe pid
                    var psi = new ProcessStartInfo
                    {
                        FileName = updaterPath,
                        Arguments = $"\"{downloadPath}\" \"{appExe}\" {Process.GetCurrentProcess().Id}",
                        UseShellExecute = true
                    };

                    Process.Start(psi);

                    // exit to allow updater replace files
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Update check failed: " + ex);
                // не мешаем пользователю при ошибке апдейта — логируем и продолжаем
            }
        }

        private async Task DownloadFileAsync(string url, string destPath, HttpClient client)
        {
            using var resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync();
            using var fs = File.Create(destPath);
            await stream.CopyToAsync(fs);
        }

        private async Task<bool> VerifySha256Async(string filePath, string expectedHex)
        {
            using var sha = SHA256.Create();
            using var fs = File.OpenRead(filePath);
            var hash = await sha.ComputeHashAsync(fs);
            var actualHex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return actualHex == expectedHex.ToLowerInvariant();
        }

        private class UpdateManifest
        {
            public string version { get; set; }
            public string url { get; set; }
            public string sha256 { get; set; }
            public string notes { get; set; }
        }
    }
}
