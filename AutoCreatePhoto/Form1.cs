using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCreatePhoto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog1.FileNames.Length == 3)
                {
                    this.pictureBox3.ImageLocation = this.openFileDialog1.FileNames[0];
                    this.pictureBox1.ImageLocation = this.openFileDialog1.FileNames[1];
                    this.pictureBox2.ImageLocation = this.openFileDialog1.FileNames[2];
                }
                else
                {
                    this.pictureBox3.ImageLocation = this.openFileDialog1.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog1.FileNames.Length == 2)
                {
                    this.pictureBox1.ImageLocation = this.openFileDialog1.FileNames[0];
                    this.pictureBox2.ImageLocation = this.openFileDialog1.FileNames[1];
                }
                else
                {
                    this.pictureBox1.ImageLocation = this.openFileDialog1.FileName;
                }
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog1.FileNames.Length == 2)
                {
                    this.pictureBox1.ImageLocation = this.openFileDialog1.FileNames[0];
                    this.pictureBox2.ImageLocation = this.openFileDialog1.FileNames[1];
                }
                else
                {
                    this.pictureBox2.ImageLocation = this.openFileDialog1.FileName;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.textBox1.Text = this.folderBrowserDialog1.SelectedPath;
                }
                if (this.textBox2.Text.Length>0)
                {
                    this.currentPath = Path.Combine(this.folderBrowserDialog1.SelectedPath, this.textBox2.Text);
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.currentPath) && Directory.Exists(this.currentPath))
            {
                Process.Start("explorer.exe", this.currentPath);
            }
        }
        private string currentPath = string.Empty;
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox2.Text))
            {
                MessageBox.Show("设定产品编号");
            }
            else if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                MessageBox.Show("设定保存目录");
            }
            else if ((this.pictureBox1.Image == null) && (this.pictureBox2.Image == null))
            {
                MessageBox.Show("请选择图片");
            }
            else
            {
                this.progressBar1.Value = 0;
                try
                {
                    this.currentPath = Path.Combine(this.folderBrowserDialog1.SelectedPath, this.textBox2.Text);
                    if (!Directory.Exists(this.currentPath))
                    {
                        Directory.CreateDirectory(this.currentPath);
                    }
                    if (this.pictureBox3.Image != null)
                    {
                        this.pictureBox3.Image.Save(Path.Combine(this.currentPath, Path.GetFileName(this.pictureBox3.ImageLocation)));
                    }
                    double num = Math.Pow((double)this.numericUpDown2.Value, 0.33333333333333331) + 0.2;
                    //this.numericUpDown1.Value = 45M;
                    this.progressBar1.Maximum = ((int)this.numericUpDown2.Value) + 1;
                    this.backgroundWorker1.RunWorkerAsync();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = this.folderBrowserDialog1.SelectedPath;
               
                   
                    bool flag = true;
                    foreach (string str4 in Directory.GetFiles(this.currentPath, "*.jpg"))
                    {
                        try
                        {
                            File.Copy(str4, Path.Combine(selectedPath, Path.GetFileName(str4)), true);
                            builder.AppendLine(string.Format("已经复制：{0}->{1}", str4, Path.Combine(selectedPath, Path.GetFileName(str4))));
                        }
                        catch
                        {
                            builder.AppendLine(string.Format("复制失败：{0}->{1}", str4, Path.Combine(selectedPath, Path.GetFileName(str4))));
                            flag = false;
                        }
                    } 
            }
            if (builder.Length > 0)
            {
                MessageBox.Show(builder.ToString());
                //new Form2(builder.ToString()).ShowDialog(this);
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((this.pictureBox1.Image != null) && (this.pictureBox2.Image == null))
            {
                this.StartGenerator(this.pictureBox1.Image, ImageUtils.emptyImage, this.pictureBox3.Image);
            }
            else if ((this.pictureBox1.Image == null) && (this.pictureBox2.Image != null))
            {
                this.StartGenerator(ImageUtils.emptyImage, this.pictureBox2.Image, this.pictureBox3.Image);
            }
            else if ((this.pictureBox1.Image != null) && (this.pictureBox2.Image != null))
            {
                this.StartGenerator(this.pictureBox1.Image, this.pictureBox2.Image, this.pictureBox3.Image);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > this.progressBar1.Maximum)
            {
                this.progressBar1.Maximum = e.ProgressPercentage;
            }
            this.progressBar1.Value = e.ProgressPercentage;
        }
        private void StartGenerator(Image image1, Image image2, Image bgImage)
        { 
            int image1angle = 0;
            int percentProgress = 0;
            do {
                Image newimage1 = ImageUtils.Rotate(image1, image1angle);
                int image2angle = 0;
                do {
                    Image newimage2 = ImageUtils.Rotate(image2, image2angle);
                    Image resultImage = ImageUtils.NewCompose(newimage1, newimage2, bgImage, CombineMode.UptoDown);
                    resultImage.Save(Path.Combine(this.currentPath, percentProgress.ToString() + ".jpg"));
                    resultImage.Dispose();
                    percentProgress++;
                    this.backgroundWorker1.ReportProgress(percentProgress);
                    Image resultImage1 = ImageUtils.NewCompose(newimage1, newimage2, bgImage, CombineMode.LeftToRight);
                    resultImage1.Save(Path.Combine(this.currentPath, percentProgress.ToString() + ".jpg"));
                    resultImage1.Dispose();
                    percentProgress++;
                    this.backgroundWorker1.ReportProgress(percentProgress);
                    Image resultImage2 = ImageUtils.NewCompose(newimage1, newimage2, bgImage, CombineMode.Bevel);
                    resultImage2.Save(Path.Combine(this.currentPath, percentProgress.ToString() + ".jpg"));
                    resultImage2.Dispose();
                    percentProgress++;
                    this.backgroundWorker1.ReportProgress(percentProgress);
                    Image resultImage3 = ImageUtils.NewCompose(newimage1, newimage2, bgImage, CombineMode.UptoDown);
                    resultImage3.Save(Path.Combine(this.currentPath, percentProgress.ToString() + ".jpg"));
                    resultImage3.Dispose();
                    percentProgress++;

                    this.backgroundWorker1.ReportProgress(percentProgress);
                    if (percentProgress >= this.progressBar1.Maximum)
                    {
                        return;
                    }
                    GC.Collect();
                    image2angle += 45;
                } while (image2angle < 360);
                image1angle += 45;
            } while (image1angle < 360); 
        }

        private void button7_Click(object sender, EventArgs e)
        { 
            for (int angle = 45; angle < 360; angle+=45)
            {
                Image image = ImageUtils.Rotate(this.pictureBox1.Image, angle);
                image.Save(Path.Combine(this.textBox1.Text,angle.ToString()+".jpg"));
                image.Dispose();
            }
           
        }
    }
}
