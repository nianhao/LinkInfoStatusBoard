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
        /// 存放绘制的内容的区域，刷新的时候使用 0 ：车 1：server
        /// [x y w h ]
        /// </summary>
        private ArrayList itemsHaveDraw = new ArrayList();
        /// <summary>
        /// 存放已经绘制的线 {Point,Point}
        /// </summary>
        private ArrayList linesHaveDraw = new ArrayList();
        /// <summary>
        /// 存放上次点击的，用于强调的线存放和替代
        /// </summary>
        private ArrayList empLines = new ArrayList();
        /// <summary>
        /// 界面能显示的最多元素
        /// </summary>
        private int maxShowNum = 11;
        /// <summary>
        /// 选择的无人机显示位置
        /// </summary>
        private Hashtable uvaSelected = new Hashtable();
        /// <summary>
        /// 选择的服务器的连接点
        /// </summary>
        private Hashtable serverPintsSelected = new Hashtable();
        /// <summary>
        /// 车和服务器占屏幕的比例,0:width比例 1:height比例
        /// </summary>
        private double[] carRect = new [] {1/6.0,1/4.0};
        private double[] serverRect = new[] { 1 / 6.0, 1 / 4.0 };
        /// <summary>
        /// 标记窗口是否是第一次加载
        /// </summary>
        private bool firstLoad = false;
        /// <summary>
        /// 标记窗口大小是否发生变化
        /// </summary>
        private bool windowResized = false;
        /// <summary>
        /// 普通线的颜色
        /// </summary>
        private Color lineColor = Color.Blue;
        /// <summary>
        /// 强调线的颜色
        /// </summary>
        private Color empColor = Color.Red;
        public Form1()
        {
            InitializeComponent();
            firstLoad = true;
            windowResized = false;
            
            //drawPoint();
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
        /// <summary>
        /// 在server上选择11个点
        /// </summary>
        /// <param name="serverCenter"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private ArrayList calcServerLinesPoint(Point serverCenter,int offset=5)
        {
            ArrayList serverLinePoints = new ArrayList();
            //从center分别向上向下取5个点
            serverLinePoints.Add(serverCenter);
            
            for(int i=0;i<5;i++)
            {
                int upY = serverCenter.Y + (i+1) * offset;
                int downY = serverCenter.Y - (i + 1) * offset;
                Point upP = new Point(serverCenter.X, upY);
                Point downP = new Point(serverCenter.X, downY);
                serverLinePoints.Add(upP);
                serverLinePoints.Add(downP);
            }
            return serverLinePoints;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            
            if (firstLoad)
            {
                labelTips.Text = "当前有无人机在线，单击无人机\n查看链路状态";
                labelTips.Visible = true;
                var windowHeight = this.Size.Height;
                var windowWidth = this.Size.Width;

                Graphics g = this.CreateGraphics();
                //清除之前绘制的内容
                clearWindow(g);
                //绘制新的内容
                drawNew(g, windowHeight, windowWidth);
                firstLoad = !firstLoad;
                this.Width = this.Width - 1;
            }
            
           
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
            windowResized = false;
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
            if (linesHaveDraw.Count > 0)
                linesHaveDraw.Clear();
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
            drawNetCar2Server(g,carCenter, serverCenter);
        }

        private void drawNetCar2Server(Graphics g,Point carCenter, Point serverCenter)
        {
            //throw new NotImplementedException();

            ArrayList serverPints = calcServerLinesPoint(serverCenter,10);
            int uvaNum = itemsHaveDraw.Count - 2;
            if(linesHaveDraw.Count>uvaNum)
            {
                for(int i=uvaNum;i<linesHaveDraw.Count;i++)
                {
                    linesHaveDraw.RemoveAt(i);
                }
            }
            if(serverPintsSelected.Count==0||serverPintsSelected.Count!=uvaNum)
            {
                serverPintsSelected.Clear();
                serverPintsSelected = Tool.randSelectIndex(maxShowNum, uvaNum);
            }
           
            foreach(int i in serverPintsSelected.Values)
            {
                Point serverP = (Point) serverPints[i];
                Pen mpen = new Pen(Color.Blue);
                Point[] line = new Point[] { carCenter,serverP };
                linesHaveDraw.Add(line);
                g.DrawLine(mpen,carCenter.X, carCenter.Y,serverP.X,serverP.Y);
            }
        }

        /// <summary>
        /// 计算服务器绘制的中心
        /// </summary>
        /// <param name="v"></param>
        /// <returns>Point（int X,int Y）</returns>
        private Point calcServerCenter(object v)
        {
            //throw new NotImplementedException();
            int[] serverR = v as int[];
            return new Point(Convert.ToInt32(serverR[0] + (serverR[2] * 0.5)), Convert.ToInt32(serverR[1] + (serverR[3] * 0.5)));
        }
        /// <summary>
        /// 计算通信车绘制的中心
        /// </summary>
        /// <param name="v"></param>
        /// <returns>Point（int X,int Y）</returns>
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
            var uvaItems = calcUVAPoints(windowHeight, windowWidth,0);

            
            Random rm = new Random();
            if(uvaNum>maxShowNum)
            {
                uvaNum = maxShowNum;
                MessageBox.Show("最多支持"+maxShowNum+"个无人机");
            }
            if(uvaSelected.Count==0|| uvaSelected.Count!=uvaNum)
            {
                uvaSelected.Clear();
                uvaSelected = Tool.randSelectIndex(11, uvaNum);
            }
            foreach(int i in uvaSelected.Values)
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
            //清除uva car server
            foreach(int [] item in itemsHaveDraw)
            {
                //实心刷
                SolidBrush mysbrush = new SolidBrush(Control.DefaultBackColor);
                //获取矩形框
                Rectangle myrect = new Rectangle(item[0],item[1], item[2], item[3]);
                //用背景色填充矩形
                g.FillRectangle(mysbrush, myrect);
            }
            itemsHaveDraw.Clear();
            //清除 uva-car car-server之间的连线
            foreach (Point[] line in linesHaveDraw)
            {
                Pen mp = new Pen(Control.DefaultBackColor);
                g.DrawLine(mp,line[0], line[1]);
            }
            linesHaveDraw.Clear();
            //清除 强调的线
            foreach(Point [] line in empLines)
            {
                Pen mp = new Pen(Control.DefaultBackColor);
                g.DrawLine(mp, line[0], line[1]);
            }
            empLines.Clear();
        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {
            Point formPoint = this.PointToClient(Control.MousePosition);
            Trace.WriteLine(formPoint);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            var windowHeight = this.Size.Height;
            var windowWidth = this.Size.Width;
            Point clickPosition = this.PointToClient(Control.MousePosition);
            int uvaNo = matchPosition2UvaNo(clickPosition);
            if(uvaNo>-1)
            {
                this.labelTips.Text = "无人机"+uvaNo+"号";
                this.labelTips.Visible = true;
                Graphics g = this.CreateGraphics();
                if(windowResized)
                {
                    clearWindow(g);
                    drawNew(g, windowHeight, windowWidth);
                }

                emphasizeLine(g, uvaNo);
            }
            


            /*Graphics g = this.CreateGraphics();
            //清除之前绘制的内容
            clearWindow(g);
            //绘制新的内容
            drawNew(g, windowHeight, windowWidth);*/
            //this.Height = this.Height-1;

        }

        private void emphasizeLine(Graphics g, int uvaNo)
        {
            //throw new NotImplementedException();
            clearEmphasizeLine(g);
            Pen p = new Pen(Color.Red);
            int uvaNum = itemsHaveDraw.Count - 2;
            Point[] line1 = linesHaveDraw[uvaNo] as Point[];
            Point[] line2 = linesHaveDraw[uvaNo + uvaNum] as Point[];
            Trace.WriteLine("重新画这两条线\n"+line1[0]+" "+line1[1] + "\n" + line2);
            g.DrawLine(p, line1[0],line1[1]);

            g.DrawLines(p, line2);
            empLines.Add(line1);
            empLines.Add(line2);

        }

        private void clearEmphasizeLine(Graphics g)
        {
            //throw new NotImplementedException();
            Pen p = new Pen(lineColor);
            foreach(Point [] line in empLines)
            {
                g.DrawLines(p, line);
            }
            empLines.Clear(); 
        }

        /// <summary>
        /// 获取到点击的位置，是否有无人机。如果有，匹配无人机的
        /// 编号
        /// </summary>
        /// <param name="clickPosition"></param>
        /// <returns>-1 点击的位置不是无人机 >-1 点击的位置的无人机的编号</returns>
        private int matchPosition2UvaNo(Point clickPosition)
        {
            if (itemsHaveDraw.Count < 3) return -1;
            //throw new NotImplementedException();
            for(int i=2;i<itemsHaveDraw.Count;i++)
            {
                int[] uvaR = itemsHaveDraw[i] as int[];
                if(pointInRect(clickPosition,uvaR))
                {
                    return i - 2;
                }
            }
            return -1;
            
        }
        /// <summary>
        /// 判断一个点是否在一个 x，y，w，h四元组确定的矩形内
        /// </summary>
        /// <param name="clickPosition"></param>
        /// <param name="uvaR"></param>
        /// <returns></returns>
        private bool pointInRect(Point clickPosition, int[] uvaR)
        {
            //throw new NotImplementedException();
            int x = clickPosition.X;
            int y = clickPosition.Y;
            if (x >= uvaR[0] && x <= uvaR[0] + uvaR[2] && y >= uvaR[1] && y <= uvaR[1] + uvaR[3])
                return true;
            return false;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            windowResized = true;
            //MessageBox.Show(this.Height.ToString()+" "+this.Width.ToString());
            //Graphics g = this.CreateGraphics();
            //Pen p = new Pen(Color.Red);
            //g.DrawLine(p, 100, 100, 200, 200);
            var windowHeight = this.Size.Height;
            var windowWidth = this.Size.Width;
            setControlPosition();
            Graphics g = this.CreateGraphics();
            //清除之前绘制的内容
            clearWindow(g);
            //绘制新的内容
            drawNew(g, windowHeight, windowWidth);
            //labelTips
        }
        /// <summary>
        /// 修改
        /// </summary>
        private void setControlPosition()
        {
            var windowHeight = this.Height;
            var windowWidth = this.Width;
            //throw new NotImplementedException();
            //将labelTips放置到窗体中间正上方
            labelTips.Location = new Point(Convert.ToInt32((5 / 12.0) * windowWidth), 0);

        }
    }

}
