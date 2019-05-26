using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkInfoStatusBoard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            drawPoint();
        }

        private void drawPoint()
        {
            //throw new NotImplementedException();

            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Black, 8.0F);
            //p.DashStyle = DashStyle.Dash;
            //g.DrawArc();
            g.DrawLine(p, 10, 20, 500, 500);
            g.DrawRectangle(p, 10, 10, 100, 100);//在画板上画矩形,起始坐标为(10,10),宽为,高为
            g.DrawEllipse(p, 10, 10, 100, 100);//在画板上画椭圆,起始坐标为(10,10),外接矩形的宽为,高为

            Console.WriteLine("ssss");

        }
        private void calcLocation()
        {
            var windowHeight = this.Size.Height;
            var windowWidth = this.Size.Width;

            //var carX;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var windowHeight = this.Size.Height;
            var windowWidth = this.Size.Width;
            /*Pen redPen = new Pen(Color.Red, 3);
            Size RectangleSize = new Size(50, 50);
            Size EllipseSize = new Size(80, 60);
            base.OnPaint(e);
            if (e.ClipRectangle.Top < 112 && e.ClipRectangle.Left < 82)
            {
                Rectangle rectangel = new Rectangle(new Point(0, 0), RectangleSize);
                Rectangle Ellipse = new Rectangle(new Point(0, 50), EllipseSize);
                var dc = e.Graphics;


                dc.DrawRectangle(redPen, rectangel);
                dc.DrawEllipse(redPen, Ellipse);
            }
            Console.WriteLine(this.Size);*/
            //this.Invalidate();
            Graphics g = this.CreateGraphics();
            g.DrawImage(LinkInfoStatusBoard.Properties.Resources.uva, 30, 40,windowHeight/5,windowWidth/6);
            g.DrawImage(LinkInfoStatusBoard.Properties.Resources.car, windowWidth / 3, windowHeight / 2-windowWidth/5, windowHeight / 3, windowWidth / 4);
            g.DrawImage(LinkInfoStatusBoard.Properties.Resources.server, windowWidth -windowWidth/5, windowHeight / 2 - windowWidth / 5, windowHeight / 3, windowWidth / 4);
            Pen p = new Pen(Color.Black, 8.0F);
            //p.DashStyle = DashStyle.Dash;
            //g.DrawArc();
            /*g.DrawLine(p, 10, 20, 500, 500);
            g.DrawRectangle(p, 10, 10, 100, 100);//在画板上画矩形,起始坐标为(10,10),宽为,高为
            g.DrawEllipse(p, 10, 10, 100, 100);//在画板上画椭圆,起始坐标为(10,10),外接矩形的宽为,高为
            
            Console.WriteLine("ssss");*/

        }
    }

}
