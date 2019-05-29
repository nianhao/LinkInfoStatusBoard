using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        /// <summary>
        /// 存放绘制的内容的区域，刷新的时候使用
        /// </summary>
        private ArrayList itemsHaveDraw = new ArrayList();
        /// <summary>
        /// 存放已经绘制的线
        /// </summary>
        private ArrayList linesHaveDraw = new ArrayList();
        /// <summary>
        /// 
        /// 车和服务器占屏幕的比例,0:width比例 1:height比例
        /// </summary>
        private double[] carRect = new [] {1/6.0,1/4.0};
        private double[] serverRect = new[] { 1 / 6.0, 1 / 4.0 };

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
        /// <summary>
        /// 计算全部无人机的位置
        /// </summary>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        /// <param name="verborse"></param>
        /// <returns></returns>
        private ArrayList calcUVAPoints(int windowHeight, int windowWidth,int verborse=2)
        {
            ArrayList uvaPoints = new ArrayList();
            var heightStart = Convert.ToInt32((1 / 6.0) * windowHeight);
            var widthStart = 0;
            for(int i =0;i<3;i++ )
            {
                for(int j=0;j<4;j++)
                {
                    var pointY = windowHeight * (1 / 4.0) * j -windowHeight*(1/12.0)*i+ heightStart;
                    var pointX = windowWidth * (1 / 9.0) * i +windowWidth*(1/27.0)*i+ widthStart;
                    var widthUVA = windowWidth * (1 / 27.0);
                    var heightUVA = windowHeight * (1 / 12.0);
                    var uvaItem = new [] { pointX, pointY, widthUVA, heightUVA };
                    uvaPoints.Add(uvaItem);
                }
            }
            uvaPoints.Remove(uvaPoints[3]);
            //changeY(uvaPoints,1,9);
           // changeY(uvaPoints,3,11);
            if (verborse>1)
            {
                foreach (Double [] uvaItem in uvaPoints)
                {
                    Trace.WriteLine(string.Format("{0}--{1}--{2}--{3}",
                        uvaItem[0],
                        uvaItem[1],
                        uvaItem[2],
                        uvaItem[3]));
                }
            }
            return uvaPoints;
        }

        private void changeY(ArrayList uvaPoints, int v1, int v2)
        {
            //throw new NotImplementedException();
            var tmp = (uvaPoints[v1] as double[])[1];
            (uvaPoints[v1] as double[])[1] = (uvaPoints[v2] as double[])[1];
            (uvaPoints[v2] as double[])[1] = tmp;
        }

        private void calcLocation()
        {
            var windowHeight = this.Size.Height;
            var windowWidth = this.Size.Width;
            var arPoint = calcCarPoint(windowHeight, windowWidth);
            var serverPoint = calcServerPoint(windowHeight, windowWidth);


            //var carX;
        }
        /// <summary>
        /// 计算车的矩阵左上角
        /// </summary>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        /// <returns></returns>
        private Point calcCarPoint(int windowHeight, int windowWidth)
        {
            //throw new NotImplementedException();
            var xStart = Convert.ToInt32(windowWidth * (5 / 12.0));
            var yStart = Convert.ToInt32(windowHeight * 0.5);
            var p = new Point(xStart, yStart);
            return p;
        }
        /// <summary>
        /// 计算服务器的左上角
        /// </summary>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        /// <returns></returns>
        private Point calcServerPoint(int windowHeight, int windowWidth)
        {
            //throw new NotImplementedException();
            var xStart = Convert.ToInt32(windowWidth * (9 / 12.0));
            var yStart = Convert.ToInt32(windowHeight * 0.25);
            var p = new Point(xStart, yStart);
            return p;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var windowHeight = this.Size.Height;
            var windowWidth = this.Size.Width;
            
            Graphics g = this.CreateGraphics();
            //清除之前绘制的内容
            clearWindow(g);
            //绘制新的内容
            drawNew(g,windowHeight,windowWidth);
           
        }
        /// <summary>
        /// 进行网络结构的绘制，这里要先绘制车，在绘制服务器。这样的话，后面连线，可以
        /// 从itemHavadraw中的 0 取出车的信息， 1 取出服务器的信息
        /// </summary>
        /// <param name="g"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        private void drawNew(Graphics g,int windowHeight,int windowWidth)
        {
            //throw new NotImplementedException();
            //计算车的绘制位置
            var carPoint = calcCarPoint(windowHeight, windowWidth);
            var carW =Convert.ToInt32(carRect[0] * windowWidth);
            var carH = Convert.ToInt32(carRect[1] * windowHeight);
            //将这个位置添加到记录中，用于下次绘制清除
            var carR = new[] { carPoint.X, carPoint.Y, carW, carH };
            itemsHaveDraw.Add(carR);
            //计算server的绘制位置
            var serverPoint = calcServerPoint(windowHeight, windowWidth);
            var serverW = Convert.ToInt32(serverRect[0] * windowWidth);
            var serverH = Convert.ToInt32(serverRect[1] * windowHeight);
            //将服务器的位置添加到数组中
            var serverR = new[] { serverPoint.X, serverPoint.Y, serverW, serverH };
            itemsHaveDraw.Add(serverR);
            //g.DrawImage(LinkInfoStatusBoard.Properties.Resources.uva, carPoint.X,carPoint.Y, windowHeight / 5, windowWidth / 6);
            g.DrawImage(LinkInfoStatusBoard.Properties.Resources.car,carPoint.X,carPoint.Y,carW,carH);
            g.DrawImage(LinkInfoStatusBoard.Properties.Resources.server, serverPoint.X,serverPoint.Y,serverW,serverH);
            //绘制无人机
            drawUVA(g,windowHeight,windowWidth,5);
            //绘制连线
            drawNet(g);
        }

        private void drawNet(Graphics g)
        {
            //throw new NotImplementedException();
            Point carCenter = calcCarCenter(itemsHaveDraw[0]);
            Point serverCenter = calcServerCenter(itemsHaveDraw[1]);
            for(int i=2;i<itemsHaveDraw.Count;i++)
            {
                Pen mpen = new Pen(Color.Blue);
                int [] uvaR = itemsHaveDraw[i] as int [];
                Point uvaP = new Point(Convert.ToInt32(uvaR[0] + uvaR[2] * 0.5), Convert.ToInt32(uvaR[1] + uvaR[3] * 0.5));
                Point[] line = new Point [] { uvaP, carCenter };
                linesHaveDraw.Add(line);
                g.DrawLine(mpen, uvaP.X,uvaP.Y , carCenter.X, carCenter.Y);
            }
        }

        private Point calcServerCenter(object v)
        {
            //throw new NotImplementedException();
            int[] serverR = v as int[];
            return new Point(Convert.ToInt32(serverR[0] + (serverR[2] * 0.5)), Convert.ToInt32(serverR[1] + (serverR[3] * 0.5)));
        }

        private Point calcCarCenter(object v)
        {
            //throw new NotImplementedException();
            int[] carR = v as int[];
            return new Point(Convert.ToInt32(carR[0] + (carR[2] * 0.5)), Convert.ToInt32(carR[1] + (carR[3] * 0.5)));

        }

        /// <summary>
        /// 绘制一定数量的无人机在界面上
        /// </summary>
        /// <param name="g"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        /// <param name="uvaNum">绘制的无人机数量，默认为11，也是最大值</param>
        private void drawUVA(Graphics g, int windowHeight,int windowWidth,int uvaNum=11)
        {
            var uvaItems = calcUVAPoints(windowHeight, windowWidth);

            Hashtable hashtableUVASelected = new Hashtable();
            Random rm = new Random();
            if(uvaNum>11)
            {
                uvaNum = 11;
                MessageBox.Show("最多支持11个无人机");
            }
            for (int i = 0; hashtableUVASelected.Count < uvaNum; i++)
            {
                int nValue = rm.Next(uvaItems.Count);
                if (!hashtableUVASelected.ContainsValue(nValue))
                {
                    hashtableUVASelected.Add(nValue, nValue);
                    Trace.WriteLine("选择了 "+nValue.ToString()+"位置显示");
                }
            }
            /*foreach (Double[] uvaItem in uvaItems)
            {
                var x = Convert.ToInt32(uvaItem[0]);
                var y = Convert.ToInt32(uvaItem[1]);
                var w = Convert.ToInt32(uvaItem[2]);
                var h = Convert.ToInt32(uvaItem[3]);
                var uvaR = new[] { x, y, w, h };
                itemsHaveDraw.Add(uvaR);
                g.DrawImage(LinkInfoStatusBoard.Properties.Resources.uva, x, y, w, h);
            }*/
            foreach(int i in hashtableUVASelected.Values)
            {
                Double[] uvaItem = uvaItems[i] as Double[];
                var x = Convert.ToInt32(uvaItem[0]);
                var y = Convert.ToInt32(uvaItem[1]);
                var w = Convert.ToInt32(uvaItem[2]);
                var h = Convert.ToInt32(uvaItem[3]);
                var uvaR = new[] { x, y, w, h };
                itemsHaveDraw.Add(uvaR);
                g.DrawImage(LinkInfoStatusBoard.Properties.Resources.uva, x, y, w, h);

            }
            //throw new NotImplementedException();
        }

        private void clearWindow(Graphics g)
        {
            //throw new NotImplementedException();
            foreach(int [] item in itemsHaveDraw)
            {
                //实心刷
                SolidBrush mysbrush = new SolidBrush(Control.DefaultBackColor);
                //获取矩形框
                Rectangle myrect = new Rectangle(item[0],item[1], item[2], item[3]);
                //用背景色填充矩形
                g.FillRectangle(mysbrush, myrect);
            }
            foreach(Point[] line in linesHaveDraw)
            {
                Pen mp = new Pen(Control.DefaultBackColor);
                g.DrawLine(mp,line[0], line[1]);
            }
            itemsHaveDraw.Clear();
        }
    }

}
