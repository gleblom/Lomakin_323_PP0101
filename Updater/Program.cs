
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: updater <package.zip> <appExePath> <pid> [waitMs]");
            return 1;
        }

        var package = args[0];
        var appExe = args[1];
        if (!int.TryParse(args[2], out var pid)) return 2;
        var waitMs = 15000; // default 15s to observe new process
        if (args.Any(a => a.StartsWith("--wait-ms=")))
        {
            var w = args.First(a => a.StartsWith("--wait-ms=")).Split('=')[1];
            if (int.TryParse(w, out var wv)) waitMs = wv;
        }

        var appDir = Path.GetDirectoryName(appExe);
        var tempRoot = Path.Combine(Path.GetTempPath(), "MyAppUpdater", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var newExtractDir = Path.Combine(tempRoot, "app-new");
        Directory.CreateDirectory(newExtractDir);
        var backupDir = appDir + "-backup-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        try
        {
            Console.WriteLine($"Extracting {package} -> {newExtractDir}");
            ZipFile.ExtractToDirectory(package, newExtractDir, overwriteFiles: true);

            Console.WriteLine($"Waiting for PID {pid} to exit...");
            WaitForProcessExit(pid, timeoutMs: 60_000);

            Console.WriteLine("Creating backup by renaming current app folder...");
            // attempt atomic rename
            if (Directory.Exists(backupDir)) Directory.Delete(backupDir, true);
            Directory.Move(appDir, backupDir);

            Console.WriteLine("Moving new version into place...");
            // move newExtractDir -> appDir
            try
            {
                Directory.Move(newExtractDir, appDir); // fast rename if same volume
            }
            catch
            {
                // fallback: copy files
                CopyDirectory(newExtractDir, appDir);
            }

            Console.WriteLine("Starting new application...");
            var proc = Process.Start(new ProcessStartInfo { FileName = appExe, UseShellExecute = true });
            if (proc == null) throw new Exception("Failed to start new app");

            // wait a bit and check if process stays alive
            Thread.Sleep(waitMs);
            if (!IsProcessRunning(proc.Id))
            {
                Console.WriteLine("New process exited quickly -> rollback");
                Rollback(backupDir, appDir, appExe);
                return 3;
            }

            Console.WriteLine("New process seems alive. Removing backup...");
            TryDeleteDirectory(backupDir);
            Console.WriteLine("Update applied successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Update failed: " + ex);
            // rollback attempt
            try { RestoreBackupIfNeeded(backupDir, appDir, appExe); } catch { }
            return 4;
        }
        finally
        {
            // cleanup temp
            TryDeleteDirectory(tempRoot);
            TryDeleteFile(package); // remove downloaded package
        }
    }

    static void WaitForProcessExit(int pid, int timeoutMs)
    {
        try
        {
            var proc = Process.GetProcessById(pid);
            var sw = Stopwatch.StartNew();
            while (!proc.HasExited)
            {
                if (sw.ElapsedMilliseconds > timeoutMs) break;
                Thread.Sleep(500);
            }
        }
        catch (ArgumentException) { /* already exited */ }
    }

    static bool IsProcessRunning(int pid)
    {
        try
        {
            var p = Process.GetProcessById(pid);
            return !p.HasExited;
        }
        catch { return false; }
    }

    static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dir.Replace(source, target));
        foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
        {
            var dest = file.Replace(source, target);
            File.Copy(file, dest, true);
        }
    }

    static void Rollback(string backupDir, string appDir, string appExe)
    {
        try
        {
            // kill any partially started new app
            foreach (var p in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appExe)))
            {
                try { p.Kill(); } catch { }
            }

            if (Directory.Exists(appDir))
            {
                var failedDir = appDir + "-failed-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                TryDeleteDirectory(failedDir);
                Directory.Move(appDir, failedDir);
            }

            Directory.Move(backupDir, appDir);
            Process.Start(new ProcessStartInfo { FileName = appExe, UseShellExecute = true });
            Console.WriteLine("Rollback completed. Old version restored.");
            TryDeleteDirectory(backupDir); // cleanup
        }
        catch (Exception ex)
        {
            Console.WriteLine("Rollback failed: " + ex);
        }
    }

    static void RestoreBackupIfNeeded(string backupDir, string appDir, string appExe)
    {
        if (Directory.Exists(backupDir) && !Directory.Exists(appDir))
        {
            Directory.Move(backupDir, appDir);
            Process.Start(new ProcessStartInfo { FileName = appExe, UseShellExecute = true });
        }
    }

    static void TryDeleteDirectory(string dir)
    {
        try { if (Directory.Exists(dir)) Directory.Delete(dir, true); } catch { }
    }
    static void TryDeleteFile(string file)
    {
        try { if (File.Exists(file)) File.Delete(file); } catch { }
    }
}