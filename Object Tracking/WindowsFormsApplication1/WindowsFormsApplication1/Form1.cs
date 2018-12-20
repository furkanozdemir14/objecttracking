using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Math.Geometry;
using System.IO.Ports;


using Point = System.Drawing.Point; 

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;
                  

        public Form1()
        {
            InitializeComponent();
        }

        int R; //Trackbarın değişkeneleri
        int G;
        int B;
        
       
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.DataSource = SerialPort.GetPortNames();

            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {

                comboBox1.Items.Add(VideoCaptureDevice.Name);

            }

            comboBox1.SelectedIndex = 0;

        }
        

        

        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
  
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            image1.RotateFlip(RotateFlipType.RotateNoneFlipX);

            pictureBox1.Image = image;


                EuclideanColorFiltering filter = new EuclideanColorFiltering();
         
                filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
                filter.Radius = 100;
            
                filter.ApplyInPlace(image1);

                nesnebul(image1);

                    
          
        }
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
     
            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
     
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
       
            image.UnlockBits(objectsData);


            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;

            
          
                //Cisim Takibi

                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0];
                        Graphics g = pictureBox1.CreateGraphics();
                        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                        //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                        int objectX = objectRect.X + (objectRect.Width / 2);
                        int objectY = objectRect.Y + (objectRect.Height / 2);
                       
                        g.Dispose(); 

                        if (objectX > 0 && objectX < 106 && objectY < 80)
                        {
                            Console.WriteLine("nesne sol kösede kor:" + objectX + "," + objectY);
                            
                            serialPort1.Write("1");
                        }
                        if (objectX > 106 && objectX < 212 && objectY < 80)
                        {
                            Console.WriteLine("nesne ortada kor:" + objectX + "," + objectY);
                            serialPort1.Write("2");
                        }
                        if (objectX > 212 && objectX < 320 && objectY < 80)
                        {
                            Console.WriteLine("nesne sagda kösede kor:" + objectX + "," + objectY);
                            serialPort1.Write("3");
                        }                                              
                                   
                        if (objectX > 0 && objectX < 106 && objectY > 80 && objectY < 160)
                        {
                            Console.WriteLine("nesne sol ortada kor:" + objectX + "," + objectY);
                            serialPort1.Write("4");
                        }
                        if (objectX > 107 && objectX < 211 && objectY > 80 && objectY < 160)
                        {
                            Console.WriteLine("nesne ortada kor:" + objectX + "," + objectY);
                            serialPort1.Write(" 5 ");
                        }

                        if (objectX > 212 && objectX < 320 && objectY > 80 && objectY < 160)
                        {
                            Console.WriteLine("nesne sağ ortada kor:" + objectX + "," + objectY);
                            serialPort1.Write("6");
                        }

                        if (objectX > 0 && objectX < 106 && objectY > 160 && objectY < 240)
                        {
                            Console.WriteLine("nesne sol alt köşede kor:" + objectX + "," + objectY);
                            serialPort1.Write("7");
                        }
                        if (objectX > 106 && objectX < 212 && objectY > 160 && objectY < 240)
                        {
                            Console.WriteLine("nesne alt ortada kor:" + objectX + "," + objectY);
                            serialPort1.Write("8");
                        }
                        if (objectX > 212 && objectX < 320 && objectY > 160 && objectY < 240)
                        {
                            Console.WriteLine("nesne sağ altta kor:" + objectX + "," + objectY);
                            serialPort1.Write("9");
                        }
                        
                        

                        if(chkKoordinatiGoster.Checked){
                        this.Invoke((MethodInvoker)delegate
                        {
                            richTextBox1.Text = objectRect.Location.ToString() + "\n" + richTextBox1.Text + "\n"; ;
                        });
                        }
                    }
                }
            
                        
                   
          
        }

       
        private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }

       

        
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            G = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            B = trackBar3.Value;
        }

      

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        private void başlatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20;//saniyede kaç görüntü alsın istiyorsanız. 
            Finalvideo.DesiredFrameSize = new Size(320, 240);//görüntü boyutları
            Finalvideo.Start();
        }

        private void durdurToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }
        }

        private void kapatToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();
        }

        private void seriPortBağlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            serialPort1.BaudRate = 9600;
            serialPort1.PortName = comboBox2.SelectedItem.ToString();
            serialPort1.Open();

            if (serialPort1.IsOpen)
            {
                MessageBox.Show("Port Acıldı");
            }
        }

        private void seriPortDurdurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
                       
            MessageBox.Show("Port Kapandı");
            
            
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        
            

    }


}


