using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace audiocapture.net_framework_3._5
{
    public partial class Form1 : Form
    {
        private WasapiLoopbackCapture capture;
        private WaveFileWriter writer;
        private string outputFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("現在使用しているのは .NET Framework 3.5 版です。\n予期せぬ不具合が発生する可能性があります。",
                "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            button2.Enabled = false; 
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string folderPath = textBox1.Text.Trim();

           
            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("保存フォルダを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath); 
            }

           
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".wav";
            outputFilePath = Path.Combine(folderPath, fileName);

            try
            {
               
                capture = new WasapiLoopbackCapture();
                writer = new WaveFileWriter(outputFilePath, capture.WaveFormat);

                capture.DataAvailable += (s, eArgs) =>
                {
                    writer.Write(eArgs.Buffer, 0, eArgs.BytesRecorded);
                };

                capture.RecordingStopped += (s, eArgs) =>
                {
                    writer?.Dispose();
                    capture?.Dispose();
                };

                capture.StartRecording();

                button1.Enabled = false;
                button2.Enabled = true;

                MessageBox.Show("録音を開始しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("録音開始に失敗しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.StopRecording();
                }

                button1.Enabled = true;
                button2.Enabled = false;

                MessageBox.Show("録音が停止されました。\n保存先: " + outputFilePath, "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("録音停止時にエラーが発生しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("バージョン 1.0.0\n.net framework 3.5版", "audiocaptureのバージョン情報", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
