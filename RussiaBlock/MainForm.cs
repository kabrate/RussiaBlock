using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using  System.Runtime.Serialization.Formatters.Binary;


namespace RussiaBlock
{
	public class MainForm : System.Windows.Forms.Form
	{
        #region 变量
        public static double position= 0;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private Block block;//方块实例
		private Block nextBlock;//下一基本块实例
		private int nextShapeNO;//下一基本块的形状号
		private bool paused;//已暂停
		private DateTime atStart;//开始时间
		private DateTime atPause;//暂停时间
		private TimeSpan pauseTime;//暂停间隔时间
		private SettingForm sform;
		private Keys[] keys;//按键
		private int level;//级别
		private int startLevel;//开始级别
		private bool trans;//改变级别
		private int rowDelNum;//消除的行数
		private bool failed;//是否失败
        public static int num= 0;
		private System.ComponentModel.IContainer components;
        #endregion

		public MainForm()
		{
			InitializeComponent();
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		static void Main()
		{
			Application.Run(new MainForm());
		}
        //初始化，读取配置
		private void Initiate()
		{
			try
			{
				XmlDocument doc = new XmlDocument();
                doc.Load("d:\\setting.txt");
                XmlNodeList nodes=doc.DocumentElement.ChildNodes;
				this.startLevel=Convert.ToInt32(nodes[0].InnerText);
				this.level=this.startLevel;
				this.trans=Convert.ToBoolean(nodes[1].InnerText);

				keys=new Keys[5];
				for(int i=0;i<nodes[2].ChildNodes.Count;i++)
				{
					KeysConverter kc=new KeysConverter();
					this.keys[i]=(Keys)(kc.ConvertFromString(nodes[2].ChildNodes[i].InnerText));
				}			
			}
			catch
			{
				this.trans=false;
				keys=new Keys[5];
				keys[0]=Keys.Left;
				keys[1]=Keys.Right;
				keys[2]=Keys.Down;
				keys[3]=Keys.NumPad8;
				keys[4]=Keys.NumPad9;
				this.level=1;
				this.startLevel=1;
			}
			
			this.timer1.Interval=500-50*(level-1);	
			this.label4.Text="级别： "+this.startLevel;
			if(trans)
			{
				this.TransparencyKey=Color.Black;
			}	
		}
        //开始游戏后加载
		private void Start()
		{
			this.block=null;
			this.nextBlock=null;
			this.label1.Text="手速： 0";
			this.label2.Text="块数： 0";
			this.label3.Text="行数： 0";
			this.label4.Text="级别： "+this.startLevel;
			this.level=this.startLevel;
			this.timer1.Interval=500-50*(level-1);
			this.paused=false;
			this.failed=false;
			this.panel1.Invalidate();
			this.panel2.Invalidate();
			this.nextShapeNO=0;
			this.CreateBlock();
			this.CreateNextBlock();//
			this.timer1.Enabled=true;
			this.atStart=DateTime.Now;
			this.pauseTime=new TimeSpan(0);
            if (num == 0)
                Music.PlayMusic("bgm.mid");
            else
                Music.PlayMusic("bgm.mid", 0.0);
        }

        //游戏失败
		private void Fail()
		{
			this.failed=true;
			this.panel1.Invalidate(new Rectangle(0,0,panel1.Width,100));
			this.timer1.Enabled=false;
			this.paused=true;
            Music.StopMusic();
		}
        //创建基本块
		private bool CreateBlock()
		{
			Point firstPos;
			Color color;
			if(this.nextShapeNO==0)
			{
				Random rand=new Random();		
				this.nextShapeNO=rand.Next(1,8);
			}
			switch(this.nextShapeNO)
			{
				case 1://田
					firstPos=new Point(4,0);
					color=Color.Turquoise;
					break;
				case 2://一
					firstPos=new Point(3,0);
					color=Color.Red;
					break;
				case 3://土
					firstPos=new Point(4,0);
					color=Color.Silver;
					break;
				case 4://z
					firstPos=new Point(4,0);
					color=Color.LawnGreen;
					break;
				case 5://倒z
					firstPos=new Point(4,1);
					color=Color.DodgerBlue;
					break;
				case 6://L
					firstPos=new Point(4,0);
					color=Color.Yellow;
					break;
				default://倒L
					firstPos=new Point(4,0);
					color=Color.Salmon;
					break;
			}
			if(this.block==null)
			{
				block=new Block(this.panel1,9,19,25,this.nextShapeNO,firstPos,color);
			}
			else
			{
				if(!block.GeneBlock(this.nextShapeNO,firstPos,color))
				{
					return false;
				}
			}
			block.EraseLast();
			block.Move(2);
			return true;
		}
        //建立下一块基本块
		private void CreateNextBlock()
		{
			Random rand=new Random();		
			this.nextShapeNO=rand.Next(1,8);
			Point firstPos;
			Color color;
			switch(this.nextShapeNO)
			{
				case 1://田
					firstPos=new Point(1,0);
					color=Color.Turquoise;
					break;
				case 2://一
					firstPos=new Point(0,1);
					color=Color.Red;
					break;
				case 3://土
					firstPos=new Point(0,0);
					color=Color.Silver;
					break;
				case 4://z
					firstPos=new Point(0,0);
					color=Color.LawnGreen;
					break;
				case 5://倒z
					firstPos=new Point(0,1);
					color=Color.DodgerBlue;
					break;
				case 6://L
					firstPos=new Point(0,0);
					color=Color.Yellow;
					break;
				default://倒L
					firstPos=new Point(0,0);
					color=Color.Salmon;
					break;
			}
			if(nextBlock==null)
				nextBlock=new Block(this.panel2,3,1,20,this.nextShapeNO,firstPos,color);
			else
			{
				nextBlock.GeneBlock(this.nextShapeNO,firstPos,color);
				nextBlock.EraseLast();
			}
		}
        //填补视图块并产生下一块
		private void FixAndCreate()
		{
			block.FixBlock();
			this.label1.Text="手速："+Math.Round((double)block.BlockNum/((TimeSpan)(DateTime.Now-this.atStart)).Subtract(this.pauseTime).TotalSeconds,3)+"块/秒";
			this.label2.Text="块数： "+block.BlockNum;
			this.label3.Text="行数： "+block.RowDelNum;
			if(this.level<10 && block.RowDelNum-this.rowDelNum>=30)
			{
				this.rowDelNum+=30;
				this.level++;
				this.timer1.Interval=500-50*(level-1);
                this.label4.Text="级别：  "+this.level;
			}	
			bool createOK=this.CreateBlock();
			this.CreateNextBlock();				
			if(!createOK)
				this.Fail();
		}
        //保存配置
		private void SaveSetting()
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				XmlDeclaration xmlDec=doc.CreateXmlDeclaration ("1.0","gb2312",null);

				XmlElement setting=doc.CreateElement("SETTING");
				doc.AppendChild(setting);

				XmlElement level=doc.CreateElement("LEVEL");
				level.InnerText=this.startLevel.ToString();
				setting.AppendChild(level);

				XmlElement trans=doc.CreateElement("TRANSPARENT");
				trans.InnerText=this.trans.ToString();
				setting.AppendChild(trans);
	
				XmlElement keys=doc.CreateElement("KEYS");    
				setting.AppendChild(keys);
				foreach(Keys k in this.keys)
				{
					KeysConverter kc=new KeysConverter();	
					XmlElement x=doc.CreateElement("SUBKEYS");
					x.InnerText=kc.ConvertToString(k);
					keys.AppendChild(x);
				}

				XmlElement root=doc.DocumentElement;
				doc.InsertBefore(xmlDec,root);
                doc.Save("d:\\setting.txt");
            }
			catch(Exception xe)
			{
				MessageBox.Show(xe.Message);
			}
		}

		#region Windows 窗体设计器生成的代码
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(251, 501);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(16, 8);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(81, 41);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Magenta;
            this.label1.Location = new System.Drawing.Point(264, 248);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "手速：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.label2.Location = new System.Drawing.Point(264, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "块数：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(264, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "行数：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.button1.Location = new System.Drawing.Point(288, 304);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 40);
            this.button1.TabIndex = 10;
            this.button1.Text = "开始(F2)";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.button3.Location = new System.Drawing.Point(288, 368);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 40);
            this.button3.TabIndex = 7;
            this.button3.Text = "环境设置";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.button4.Location = new System.Drawing.Point(288, 432);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 40);
            this.button4.TabIndex = 8;
            this.button4.Text = "暂停(F3)";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Location = new System.Drawing.Point(272, 8);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(112, 56);
            this.panel3.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(288, 240);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(0, 21);
            this.textBox1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(264, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "级别：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(400, 517);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "RussiaBlock";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        //按键事件
		private void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(block!=null && this.paused==false && !this.failed)
			{
				if(e.KeyCode==this.keys[0])
				{
					if(block.Move(0))
					{
						block.EraseLast();
					}
				}
				else if(e.KeyCode==this.keys[1])
				{
					if(block.Move(1))
					{
						block.EraseLast();
					}
				}
				else if(e.KeyCode==this.keys[2])
				{
					if(!block.Move(2))
					{
						this.FixAndCreate();
					}
					else
					{
						block.EraseLast();
					}
				}
				else if(e.KeyCode==this.keys[3])
				{
					if(block.Rotate())
					{
						block.EraseLast();
					}
				}
				else if(e.KeyCode==this.keys[4])
				{
					block.Drop();
					block.EraseLast();
					this.FixAndCreate();
				}
			}
			if(e.KeyCode==Keys.F2)
			{
				this.Start();
			}
			else if(e.KeyCode==Keys.F3)
			{
				this.button4_Click(null,null);
			}
		}
        //时钟触发处理函数，使方块自动向下移动
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (block != null && !this.failed)
            {
                if (!block.Move(2))//如果不是向下加速
                {
                    this.FixAndCreate();
                }
                else
                {
                    block.EraseLast();
                }
            }
        }
        //游戏窗体绘制
        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(block!=null)
			{
				block.DrawBlocks(e.ClipRectangle);
			}
			if(this.failed)
			{
				Graphics gra=e.Graphics;
				gra.DrawString("Game Over",new Font("Arial Black",25f),new SolidBrush(Color.Gray),30,30);
			}
		}
        //绘制下一块
		private void panel2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{		
			if(nextBlock!=null)
			{
				nextBlock.DrawBlocks(e.ClipRectangle);
			}
		}
        //点击开始游戏按钮
        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Start();
            this.textBox1.Focus();
            num++;
        }
        private void button2_Click(object sender, System.EventArgs e)
        {
            this.textBox1.Focus();
        }
        private void button4_Click(object sender, System.EventArgs e)
		{
            
			if(!this.failed)
			{
				if(paused)
				{
					this.pauseTime+=DateTime.Now-this.atPause;
					paused=false;
					this.timer1.Start();
                    Music.PlayMusic("bgm.mid", position);
				}
				else
				{
					this.atPause=DateTime.Now;
					paused=true;
					this.timer1.Stop();
                    position = Music.SaveMusic();
                    Music.StopMusic();
                  
                }
			}
			this.textBox1.Focus();
		}
		private void button3_Click(object sender, System.EventArgs e)
		{
			if(!paused)
			{
				this.atPause=DateTime.Now;
				this.paused=true;
				this.timer1.Stop();
                position = Music.SaveMusic();
                Music.StopMusic();

                    
            }		
			sform=new SettingForm();
			sform.SetOptions(this.keys,this.startLevel,this.trans);
			sform.DialogResult=DialogResult.Cancel;
				
			sform.ShowDialog();
			if(sform.DialogResult==DialogResult.OK)
			{
				sform.GetOptions(ref this.keys,ref this.startLevel,ref this.trans);
				this.level=this.startLevel;
				this.label4.Text="级别： "+this.level;
				this.timer1.Interval=500-50*(level-1);
				if(this.trans)
				{
					this.TransparencyKey=Color.Black;
				}
				else
					this.TransparencyKey=Color.Transparent;
			}
			this.paused=false;
			this.pauseTime+=DateTime.Now-this.atPause;
			this.timer1.Start();

			this.textBox1.Focus();	
		}
        //窗体加载
		private void MainForm_Load(object sender, System.EventArgs e)

		{
			this.Initiate();
            
        }
        //关闭窗体后加载
		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.SaveSetting();
		}


	}
}


		    
