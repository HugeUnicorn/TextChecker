using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextChecker2204
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
          
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|Binary files(.bin; .dat)|*.bin;*.dat";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isText = false;
            byte[] buffer = new byte[1024];

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            
            string fileName = openFileDialog1.FileName;
            string extension = System.IO.Path.GetExtension(fileName);
            System.IO.FileInfo file = new System.IO.FileInfo(fileName);
            long fileSize = file.Length;

            if (fileSize < 2 || fileSize > 1024)
            {
                MessageBox.Show("Приемлимый размер файла от 2 байт до 1 килобайта");
                return;
            }          

            if (extension == ".txt")
            {
                using (FileStream stream = File.Open(fileName, FileMode.Open))
                {                 
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // Проверяем, является ли файл текстовым на русском или английском языке
                    isText = IsText(buffer, bytesRead);                
                }
            }
            else if (extension == ".bin" || extension == ".dat")
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    string fileString = "";
                    while (reader.BaseStream.Position < reader.BaseStream.Length) 
                    {
                        try
                        {
                            fileString += reader.ReadString();                           
                        }
                        catch (Exception)
                        {
                            continue;                           
                        }                       
                    }
                    buffer = System.Text.Encoding.ASCII.GetBytes(fileString);
                    isText= IsText(buffer, buffer.Length);
                }
            }
            else
            {
                MessageBox.Show("Некорректный файл");
            }

            //Вывод сообщения с результатом
            if (isText)
            {
                MessageBox.Show("В файле есть текст на русском или английском языке");
            }
            else
            {
                MessageBox.Show("В файле нет текста на русском или английском языке");
            }
        }

        static bool IsText(byte[] buffer, int length)
        {
            // Проверяем, является ли содержимое файла текстом на одном из языков латиницы или кириллицы
            try
            {
                string text = Encoding.UTF8.GetString(buffer, 0, length);
                foreach (char c in text)
                {
                    //
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
