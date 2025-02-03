using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using NAudio.Wave;

namespace audiocapture
{
    public partial class Form1 : Form
    {
        private WasapiLoopbackCapture capture;
        private WaveFileWriter writer;

        public Form1()
        {
            InitializeComponent();
            InitializeFormValues();
        }

        private void InitializeFormValues()
        {
           
            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            
            textBox2.Text = "capturedAudio";

           
            comboBox1.Items.Add(".mp3");
            comboBox1.Items.Add(".wav");
            comboBox1.Items.Add(".aac (試験的) ");
            comboBox1.SelectedIndex = 1; 

            
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

           
            UpdateComboBoxFromTextBox2();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
       
            UpdateFileNameExtension();
        }

        private void UpdateFileNameExtension()
        {
            string fileName = textBox2.Text;
            string selectedExtension = comboBox1.SelectedItem.ToString();

            if (selectedExtension == ".aac (試験的) ")
            {
                selectedExtension = ".aac";
            }

            if (fileName.Contains("."))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            textBox2.Text = fileName + selectedExtension;
        }


        private void UpdateComboBoxFromTextBox2()
        {
            string fileName = textBox2.Text;

            if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                comboBox1.SelectedItem = ".mp3";
            }
            else if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                comboBox1.SelectedItem = ".wav";
            }
            else if (fileName.EndsWith(".aac (試験的) ", StringComparison.OrdinalIgnoreCase)) 
            {
                comboBox1.SelectedItem = ".aac";
            }
            else
            {
                comboBox1.SelectedItem = ".wav"; // デフォルト
            }
        }


        private void button1_Click_1(object sender, EventArgs e) 
        {
            try
            {
               
                string folderPath = textBox1.Text;

                if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                {
                    MessageBox.Show("有効な保存フォルダを指定してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                string fileName = textBox2.Text;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    MessageBox.Show("有効なファイル名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

             
                if (!fileName.Contains("."))
                {
                    fileName = Path.GetFileNameWithoutExtension(fileName) + comboBox1.SelectedItem.ToString();
                    textBox2.Text = fileName;
                }

                string outputFilePath = Path.Combine(folderPath, fileName);
           
                capture = new WasapiLoopbackCapture();
                writer = new WaveFileWriter(outputFilePath, capture.WaveFormat);
         
                capture.DataAvailable += (s, args) =>
                {
                    writer.Write(args.Buffer, 0, args.BytesRecorded);
                };
             
                capture.RecordingStopped += (s, args) =>
                {
                    writer?.Dispose();
                    writer = null;
                    capture?.Dispose();
                    capture = null;
                };

                capture.StartRecording();
                MessageBox.Show($"録音を開始しました。\n保存先: {outputFilePath}", "情報", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e) 
        {
            try
            {
                capture?.StopRecording();
                MessageBox.Show("録音を停止しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click_1(object sender, EventArgs e) 
        {
            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.IsFolderPicker = true; 
                folderDialog.Title = "保存先のフォルダを選択してください";

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    textBox1.Text = folderDialog.FileName; 
                }
            }
        }

        private void バグ一覧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
            MessageBox.Show("現在確認が出来来ているバグ\n\n・フォルダ選択メニューを表示後ソフトが小さくなる\n・空白の音声がある場合カットされる", "バグ一覧", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        
    }

        private void バージョンToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("v1.1.0 preview版\n .net framework4.8.1 ", "バージョン情報",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
