using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win_Paint
{
    public partial class Form1 : Form
    {
        string FullName= "C:\\Users\\user\\source\\repos\\Win_Paint\\bin\\Debug\\image.jpg";
        string logPath = @"c:\PaintBoard\log.txt";
        string logfolderPath = @"c:\PaintBoard";
        string ShortName="image.jpg";
        //public List<Bitmap> listBitmap = new List<Bitmap>();
        int bitmap_count = 0;
        int log_count = 0;
        public Dictionary<string, Bitmap> map_bitmap = new Dictionary<string, Bitmap>();

        string[] paint_act = {"draw","erase","rectangle","circle","rotate","resize","negative","floodfill", "load", "save", "initial"};
        enum DRAW_MODE : int
        {
            PENMODE = 0,
            ERASERMODE = 1,
            SHAPEMODE = 2,
            FLOODMODE = 3,
            CIRCLEMODE = 4,
        }
        int curMode;
        Color curColor = Color.Black;
        int curLineSize = 3;

        Point mouseDownPoint;

        Bitmap pictureBoxBmp;

        private Cursor LoadCursor(byte[] cursorFile)
        {
            MemoryStream cursorMemoryStream = new MemoryStream(cursorFile);
            Cursor hand = new Cursor(cursorMemoryStream);

            return hand;
        }
        private void SetDrawMode(int mode)
        {
            switch (mode)
            {
                case (int)DRAW_MODE.PENMODE:
                    curMode = (int)DRAW_MODE.PENMODE;
                    this.Cursor = LoadCursor(Properties.Resources.PenCursor_small);
                    break;
                case (int)DRAW_MODE.ERASERMODE:
                    curMode = (int)DRAW_MODE.ERASERMODE;
                    this.Cursor = LoadCursor(Properties.Resources.EraserCursor);
                    break;
                case (int)DRAW_MODE.SHAPEMODE:
                    curMode = (int)DRAW_MODE.SHAPEMODE;
                    this.Cursor = LoadCursor(Properties.Resources.ShapesCursor);
                    break;
                case (int)DRAW_MODE.FLOODMODE:
                    curMode = (int)DRAW_MODE.FLOODMODE;
                    this.Cursor = LoadCursor(Properties.Resources.PaintCursor);
                    break;
                case (int)DRAW_MODE.CIRCLEMODE:
                    curMode = (int)DRAW_MODE.CIRCLEMODE;
                    this.Cursor = LoadCursor(Properties.Resources.ShapesCursor);
                    break;
                default:
                    this.Cursor = Cursors.Default;
                    break;

            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.PENMODE);
            pictureBoxBmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.BackColor = Color.Wheat;
            btnColor.BackColor = curColor;

            DirectoryInfo di = new DirectoryInfo(logfolderPath);

            if(di.Exists == false)
            {
                di.Create();
            }

            // file is not exist,
            if (!File.Exists(logPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(logPath))
                {
                    sw.WriteLine("create at" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
            }
            else
            {
                FileStream fs = new FileStream(logPath, FileMode.Append);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("program start at " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();
            }




            Color init_co;  //initialize pictureBox with white
            int colorR = 0;
            int colorG = 0;
            int colorB = 0;

            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    colorR = 255; 
                    colorG = 255;
                    colorB = 255;

                    init_co = Color.FromArgb(colorR, colorG, colorB);

                    pictureBoxBmp.SetPixel(x, y, init_co); 
                }
            }

            map_bitmap.Add(paint_act[10] + bitmap_count, new Bitmap(pictureBoxBmp)); //history create
            bitmap_count++;

            pictureBox1.Image = pictureBoxBmp;
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.PENMODE);
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.ERASERMODE);
        }

        private void btnShape_Click(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.SHAPEMODE);
        }

        private void btnFlood_Click(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.FLOODMODE);
        }
        private void btnCircle_Click(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.CIRCLEMODE);
        }


        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                curColor = colorDialog.Color;
                btnColor.BackColor = curColor;
            }
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            pictureBoxBmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
            //pictureBox1.ClientSize = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);

            //listBitmap.Add(pictureBoxBmp);  //list add bitmap at rotation
            map_bitmap.Add(paint_act[4]+ bitmap_count, new Bitmap(pictureBoxBmp));  //history create
            bitmap_count++;

            FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
            using (StreamWriter sw = new StreamWriter(fs))
            {
                log_count++;
                sw.WriteLine(log_count+","+ paint_act[4]+ "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                sw.Close();
            }
            fs.Close();

            pictureBox1.Image = pictureBoxBmp;
            this.ClientSize = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);  //form size change
        }

        private void btnNegative_Click(object sender, EventArgs e)
        {
            //pictureBoxBmp
            
            Color co;
            int colorR = 0;
            int colorG = 0;
            int colorB = 0;


            for (int x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Image.Height; y++)
                {
                    co = pictureBoxBmp.GetPixel(x, y);
                    colorR = co.R ^ 255;   // 255를 XOR 하여 반전된 색상을 만듬.
                    colorG = co.G ^ 255;
                    colorB = co.B ^ 255;

                    co = Color.FromArgb(colorR, colorG, colorB);

                    pictureBoxBmp.SetPixel(x, y, co);     // 해당 좌표 픽셀의 컬러값을 변경
                }
            }
            //listBitmap.Add(pictureBoxBmp);  //list add bitmap at negative image
            map_bitmap.Add(paint_act[6] + bitmap_count, new Bitmap(pictureBoxBmp)); //history create
            bitmap_count++;

            FileStream fs = new FileStream(logPath, FileMode.Append);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                log_count++;
                sw.WriteLine(log_count + "," + paint_act[6] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                sw.Close();
            }
            fs.Close();

            pictureBox1.Image = pictureBoxBmp;

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (curMode == (int)DRAW_MODE.PENMODE && e.Button == MouseButtons.Left || curMode == (int)DRAW_MODE.ERASERMODE && e.Button == MouseButtons.Left)
            {
                Point curPoint = pictureBox1.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y));

                Pen p;
                if (curMode == (int)DRAW_MODE.ERASERMODE)
                {
                    p = new Pen(Color.White);
                }
                else
                {
                    p = new Pen(curColor);
                }
                p.Width = curLineSize;

                Graphics g = Graphics.FromImage(pictureBoxBmp);
                g.DrawEllipse(p, curPoint.X, curPoint.Y, p.Width, p.Width);
                
                pictureBox1.Image = pictureBoxBmp;
                p.Dispose();
                g.Dispose();
            }
        }
      
        private void doFloodFill(Point startPoint, Color preColor)
        {
            try
            {
                Stack<Point> pixels = new Stack<Point>();
                preColor = pictureBoxBmp.GetPixel(startPoint.X, startPoint.Y);
                //Console.Write("preColor first : ");
               // Console.WriteLine(preColor);
               pixels.Push(startPoint);
                while (pixels.Count > 0)
                {
                    //Console.WriteLine(pixels.Count);
                    Point k = pixels.Pop();
                    if (k.X < pictureBoxBmp.Width && k.X > 0 && k.Y < pictureBoxBmp.Height && k.Y > 0)
                    {
                        if (pictureBoxBmp.GetPixel(k.X, k.Y) == preColor && pictureBoxBmp.GetPixel(k.X, k.Y) != curColor  )
                        {
                            if (preColor.A == curColor.A && preColor.R == curColor.R && preColor.G == curColor.G && preColor.B == curColor.B)   //double check
                                break;
                            pictureBoxBmp.SetPixel(k.X, k.Y, curColor);
                            pixels.Push(new Point(k.X - 1, k.Y));
                            pixels.Push(new Point(k.X + 1, k.Y));
                            pixels.Push(new Point(k.X, k.Y - 1));
                            pixels.Push(new Point(k.X, k.Y + 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (curMode == (int)DRAW_MODE.SHAPEMODE && e.Button == MouseButtons.Left)
            {
                mouseDownPoint = new Point(e.X, e.Y);
            }
            if (curMode == (int)DRAW_MODE.CIRCLEMODE && e.Button == MouseButtons.Left)   //new function 
            {
                mouseDownPoint = new Point(e.X, e.Y);
            }
            else if (curMode == (int)DRAW_MODE.FLOODMODE && e.Button == MouseButtons.Left)
            {
                Point startPoint = new Point(e.X, e.Y);
                Color preColor = pictureBoxBmp.GetPixel(startPoint.X, startPoint.Y);
                doFloodFill(startPoint, preColor);
                //listBitmap.Add(pictureBoxBmp);  //list add at floodfill

                map_bitmap.Add(paint_act[7] + bitmap_count, new Bitmap(pictureBoxBmp)); //history create
                bitmap_count++;

                FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    log_count++;
                    sw.WriteLine(log_count + "," + paint_act[7] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();

                pictureBox1.Image = pictureBoxBmp;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)   //draw pen or box or circle
        {
            if (curMode == (int)DRAW_MODE.SHAPEMODE && e.Button == MouseButtons.Left)
            {
                Pen p = new Pen(curColor);
                p.Width = curLineSize;
                Point mouseUpPoint = new Point(e.X, e.Y);
                Graphics g = Graphics.FromImage(pictureBoxBmp);
                g.DrawRectangle(p, new Rectangle(mouseDownPoint.X, mouseDownPoint.Y,
                Math.Abs(mouseUpPoint.X - mouseDownPoint.X), Math.Abs(mouseUpPoint.Y - mouseDownPoint.Y)));
                //listBitmap.Add(pictureBoxBmp);  //list add at draw rectangle

                map_bitmap.Add(paint_act[2] + bitmap_count, new Bitmap(pictureBoxBmp)); //history
                bitmap_count++;

                FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    log_count++;
                    sw.WriteLine(log_count + "," + paint_act[2] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();

                pictureBox1.Image = pictureBoxBmp;
                p.Dispose();
                g.Dispose();
            }
            else if (curMode == (int)DRAW_MODE.CIRCLEMODE && e.Button == MouseButtons.Left)  //new function
            {
                Pen p = new Pen(curColor);
                p.Width = curLineSize;
                Point mouseUpPoint = new Point(e.X, e.Y);
                Graphics g = Graphics.FromImage(pictureBoxBmp);
                g.DrawEllipse(p, new Rectangle(mouseDownPoint.X, mouseDownPoint.Y,
                Math.Abs(mouseUpPoint.X - mouseDownPoint.X), Math.Abs(mouseUpPoint.Y - mouseDownPoint.Y)));
                //listBitmap.Add(pictureBoxBmp);//list add at draw circle

                map_bitmap.Add(paint_act[3] + bitmap_count, new Bitmap(pictureBoxBmp));
                bitmap_count++;

                FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    log_count++;
                    sw.WriteLine(log_count + "," + paint_act[3] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();

                pictureBox1.Image = pictureBoxBmp;
                p.Dispose();
                g.Dispose();
            }
            else if(curMode == (int)DRAW_MODE.PENMODE && e.Button == MouseButtons.Left)
            {
                //listBitmap.Add(pictureBoxBmp); //list add bitmap at draw
                map_bitmap.Add(paint_act[0] + bitmap_count, new Bitmap(pictureBoxBmp));
                bitmap_count++;
                FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    log_count++;
                    sw.WriteLine(log_count + "," + paint_act[0] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();
            }
            else if (curMode == (int)DRAW_MODE.ERASERMODE && e.Button == MouseButtons.Left)
            {
                //listBitmap.Add(pictureBoxBmp); //list add bitmap at erase
                map_bitmap.Add(paint_act[1] + bitmap_count, new Bitmap(pictureBoxBmp));
                bitmap_count++;
                FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    log_count++;
                    sw.WriteLine(log_count + "," + paint_act[1] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFile = new SaveFileDialog();

            SaveFile.Title = "다른 이름으로 저장";
            SaveFile.InitialDirectory = Environment.CurrentDirectory;
            SaveFile.OverwritePrompt = true;
            SaveFile.Filter = "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png| All Files(*.*)|*.*";
            SaveFile.FileName = ShortName;

            if (SaveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                FullName = SaveFile.FileName;
                FileInfo fi = new FileInfo(FullName);
                ShortName = fi.Name;

                this.Text = ShortName + " - Paint Board";

                
                try
                {
                   
                    pictureBox1.Image.Save(FullName);

                    FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        log_count++;
                        sw.WriteLine(log_count + "," + paint_act[9] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                        sw.Close();
                    }
                    fs.Close();

                }
                catch(Exception ex)
                {
                    
                    MessageBox.Show(ex.Message);
                }
                

            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

            OpenFileDialog OpenFile = new OpenFileDialog();

            OpenFile.Title = "Origin Image Open";
            OpenFile.InitialDirectory = Environment.CurrentDirectory;
            OpenFile.Filter = "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png| All Files(*.*)|*.*";

            if (OpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FullName = OpenFile.FileName;
                FileInfo fi = new FileInfo(FullName);
                ShortName = fi.Name;

                //Image image= Image.FromFile(FullName);

                //var bitmap = new Bitmap(FullName);
                var bitmap = (Bitmap)System.Drawing.Image.FromFile(FullName, true);
                Bitmap tempimage = bitmap;
                tempimage = bitmap.Clone(new Rectangle(0, 0, tempimage.Width, tempimage.Height), PixelFormat.Format32bppArgb);
                pictureBoxBmp = tempimage;
                pictureBox1.Image = pictureBoxBmp;
                bitmap.Dispose();
                //listBitmap.Clear(); //list clear when load is done
                map_bitmap.Clear();
                bitmap_count = 0;
                //pictureBox1.Image = Image.FromFile(FullName);
                this.Text = ShortName + " - Paint Board"; 
                this.ClientSize = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);  //form이 이미지 크기에 맞춤
                pictureBox1.Refresh();

                map_bitmap.Add(paint_act[10] + bitmap_count, new Bitmap(pictureBoxBmp)); //history create
                bitmap_count++;

                FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    log_count++;
                    sw.WriteLine(log_count + "," + paint_act[8] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    sw.Close();
                }
                fs.Close();
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //listBitmap.Clear();
            map_bitmap.Clear();
            bitmap_count = 0;
            Application.Exit();
        }

        private void SizeTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int size_value = Convert.ToInt16(SizeTextBox.Text);
                if (size_value > 50)
                {
                    size_value = 50;
                    SizeTextBox.Text = "50";
                }
                curLineSize = size_value;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SizeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar)  && !char.IsControl(e.KeyChar ))
            {
                e.Handled = true;
            }
        }

        private void btnResize_Click(object sender, EventArgs e)    //form size control
        {
            Form2 form2 = new Form2();
            form2.Owner = this;
            form2.Show();

        }
        public Bitmap ResizeBitmap(Bitmap originalBitmap, int newWidth, int newHeight)  //resize method
        {
            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
            }
            return resizedBitmap;
        }

        public void Form1_resize(int width, int height)
        {

            Bitmap resize_bitmap = ResizeBitmap(pictureBoxBmp, width, height);
            pictureBoxBmp= resize_bitmap;
            pictureBox1.Image = pictureBoxBmp;
            map_bitmap.Add(paint_act[5] + bitmap_count, new Bitmap(pictureBoxBmp));
            bitmap_count++;
            FileStream fs = new FileStream(logPath, FileMode.Append);   //log create
            using (StreamWriter sw = new StreamWriter(fs))
            {
                log_count++;
                sw.WriteLine(log_count + "," + paint_act[5] + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                sw.Close();
            }
            fs.Close();
            this.ClientSize = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);  //form이 이미지 크기에 맞춤
            pictureBox1.Refresh();

        }

        private void Form3_ValueSelected(object sender, string selectedValue)
        {
            //pictureBoxBmp = map_bitmap[selectedValue];
            //pictureBox1.Image = pictureBoxBmp;
            //pictureBox1.Refresh();
            Bitmap selectedImage;
            if (map_bitmap.TryGetValue(selectedValue, out selectedImage))
            {
                pictureBoxBmp = new Bitmap(selectedImage);
                pictureBox1.Image = pictureBoxBmp;
                this.ClientSize = new Size(selectedImage.Width, selectedImage.Height);
            }
           
         }
        private void btnHistory_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(map_bitmap);
            form3.Show();

            form3.ValueSelected += Form3_ValueSelected;
        }
    }
}
