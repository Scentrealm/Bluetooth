
namespace SKII.BluetoothAuto
{
    partial class mainFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainFrm));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.tpgHome = new System.Windows.Forms.TabPage();
            this.tblAutoCheck = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtShow = new System.Windows.Forms.TextBox();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.gpbCheck = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSList = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.pnlBluetoothList = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lvDevices = new System.Windows.Forms.ListView();
            this.gpbCmd = new System.Windows.Forms.GroupBox();
            this.btnAutoCheck = new System.Windows.Forms.Button();
            this.tblInfo = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtOprate = new System.Windows.Forms.TextBox();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBuildPlay = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBuildStop = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtCmd = new System.Windows.Forms.TextBox();
            this.tpgHome.SuspendLayout();
            this.tblAutoCheck.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.gpbCheck.SuspendLayout();
            this.pnlBluetoothList.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gpbCmd.SuspendLayout();
            this.tblInfo.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tbcMain.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "ble1.png");
            // 
            // tpgHome
            // 
            this.tpgHome.Controls.Add(this.tblAutoCheck);
            this.tpgHome.Location = new System.Drawing.Point(4, 28);
            this.tpgHome.Name = "tpgHome";
            this.tpgHome.Padding = new System.Windows.Forms.Padding(3);
            this.tpgHome.Size = new System.Drawing.Size(1000, 609);
            this.tpgHome.TabIndex = 0;
            this.tpgHome.Text = "蓝牙测试";
            this.tpgHome.UseVisualStyleBackColor = true;
            // 
            // tblAutoCheck
            // 
            this.tblAutoCheck.ColumnCount = 3;
            this.tblAutoCheck.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 273F));
            this.tblAutoCheck.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tblAutoCheck.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tblAutoCheck.Controls.Add(this.groupBox3, 2, 0);
            this.tblAutoCheck.Controls.Add(this.groupBox1, 1, 1);
            this.tblAutoCheck.Controls.Add(this.pnlSearch, 0, 0);
            this.tblAutoCheck.Controls.Add(this.pnlBluetoothList, 0, 1);
            this.tblAutoCheck.Controls.Add(this.tblInfo, 2, 1);
            this.tblAutoCheck.Controls.Add(this.gpbCmd, 1, 0);
            this.tblAutoCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAutoCheck.Location = new System.Drawing.Point(3, 3);
            this.tblAutoCheck.Name = "tblAutoCheck";
            this.tblAutoCheck.RowCount = 2;
            this.tblAutoCheck.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblAutoCheck.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblAutoCheck.Size = new System.Drawing.Size(994, 603);
            this.tblAutoCheck.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtShow);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(276, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 477);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "测试消息";
            // 
            // txtShow
            // 
            this.txtShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtShow.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtShow.Location = new System.Drawing.Point(3, 17);
            this.txtShow.Multiline = true;
            this.txtShow.Name = "txtShow";
            this.txtShow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtShow.Size = new System.Drawing.Size(276, 457);
            this.txtShow.TabIndex = 0;
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.gpbCheck);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(3, 3);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(267, 114);
            this.pnlSearch.TabIndex = 0;
            // 
            // gpbCheck
            // 
            this.gpbCheck.Controls.Add(this.label6);
            this.gpbCheck.Controls.Add(this.label5);
            this.gpbCheck.Controls.Add(this.txtSearch);
            this.gpbCheck.Controls.Add(this.btnSList);
            this.gpbCheck.Controls.Add(this.btnSearch);
            this.gpbCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbCheck.Location = new System.Drawing.Point(0, 0);
            this.gpbCheck.Name = "gpbCheck";
            this.gpbCheck.Size = new System.Drawing.Size(267, 114);
            this.gpbCheck.TabIndex = 0;
            this.gpbCheck.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(37, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "例：scent78e36d08b576";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "ID:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(35, 81);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(141, 21);
            this.txtSearch.TabIndex = 2;
            // 
            // btnSList
            // 
            this.btnSList.Location = new System.Drawing.Point(182, 80);
            this.btnSList.Name = "btnSList";
            this.btnSList.Size = new System.Drawing.Size(80, 23);
            this.btnSList.TabIndex = 1;
            this.btnSList.Text = "列表里搜索";
            this.btnSList.UseVisualStyleBackColor = true;
            this.btnSList.Click += new System.EventHandler(this.btnSList_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearch.Location = new System.Drawing.Point(58, 17);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(138, 41);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "搜索设备";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // pnlBluetoothList
            // 
            this.pnlBluetoothList.Controls.Add(this.groupBox4);
            this.pnlBluetoothList.Location = new System.Drawing.Point(3, 123);
            this.pnlBluetoothList.Name = "pnlBluetoothList";
            this.pnlBluetoothList.Size = new System.Drawing.Size(267, 477);
            this.pnlBluetoothList.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lvDevices);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(267, 477);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "设备列表";
            // 
            // lvDevices
            // 
            this.lvDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDevices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDevices.HideSelection = false;
            this.lvDevices.Location = new System.Drawing.Point(3, 17);
            this.lvDevices.MultiSelect = false;
            this.lvDevices.Name = "lvDevices";
            this.lvDevices.Size = new System.Drawing.Size(261, 457);
            this.lvDevices.SmallImageList = this.imgList;
            this.lvDevices.TabIndex = 5;
            this.lvDevices.UseCompatibleStateImageBehavior = false;
            this.lvDevices.View = System.Windows.Forms.View.SmallIcon;
            // 
            // gpbCmd
            // 
            this.gpbCmd.Controls.Add(this.btnAutoCheck);
            this.gpbCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbCmd.Location = new System.Drawing.Point(276, 3);
            this.gpbCmd.Name = "gpbCmd";
            this.gpbCmd.Size = new System.Drawing.Size(282, 114);
            this.gpbCmd.TabIndex = 2;
            this.gpbCmd.TabStop = false;
            // 
            // btnAutoCheck
            // 
            this.btnAutoCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoCheck.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAutoCheck.Location = new System.Drawing.Point(19, 35);
            this.btnAutoCheck.Name = "btnAutoCheck";
            this.btnAutoCheck.Size = new System.Drawing.Size(241, 47);
            this.btnAutoCheck.TabIndex = 1;
            this.btnAutoCheck.Text = "连接设备";
            this.btnAutoCheck.UseVisualStyleBackColor = true;
            this.btnAutoCheck.Click += new System.EventHandler(this.btnAutoCheck_Click);
            // 
            // tblInfo
            // 
            this.tblInfo.ColumnCount = 1;
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Controls.Add(this.groupBox6, 0, 1);
            this.tblInfo.Controls.Add(this.panel1, 0, 0);
            this.tblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInfo.Location = new System.Drawing.Point(564, 123);
            this.tblInfo.Name = "tblInfo";
            this.tblInfo.RowCount = 2;
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Size = new System.Drawing.Size(427, 477);
            this.tblInfo.TabIndex = 8;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtOprate);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(3, 153);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(421, 321);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "操作消息";
            // 
            // txtOprate
            // 
            this.txtOprate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOprate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOprate.Location = new System.Drawing.Point(3, 17);
            this.txtOprate.Multiline = true;
            this.txtOprate.Name = "txtOprate";
            this.txtOprate.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOprate.Size = new System.Drawing.Size(415, 301);
            this.txtOprate.TabIndex = 1;
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tpgHome);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.ItemSize = new System.Drawing.Size(60, 24);
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1008, 641);
            this.tbcMain.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnBuildStop);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnBuildPlay);
            this.groupBox3.Controls.Add(this.txtTime);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtChannel);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(564, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(427, 114);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "播放通道";
            // 
            // txtChannel
            // 
            this.txtChannel.Location = new System.Drawing.Point(75, 28);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(56, 21);
            this.txtChannel.TabIndex = 1;
            this.txtChannel.Text = "1";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(203, 28);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(60, 21);
            this.txtTime.TabIndex = 3;
            this.txtTime.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "播放时长";
            // 
            // btnBuildPlay
            // 
            this.btnBuildPlay.Location = new System.Drawing.Point(305, 21);
            this.btnBuildPlay.Name = "btnBuildPlay";
            this.btnBuildPlay.Size = new System.Drawing.Size(107, 34);
            this.btnBuildPlay.TabIndex = 4;
            this.btnBuildPlay.Text = "生成播放指令";
            this.btnBuildPlay.UseVisualStyleBackColor = true;
            this.btnBuildPlay.Click += new System.EventHandler(this.btnBuildPlay_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "秒";
            // 
            // btnBuildStop
            // 
            this.btnBuildStop.Location = new System.Drawing.Point(305, 64);
            this.btnBuildStop.Name = "btnBuildStop";
            this.btnBuildStop.Size = new System.Drawing.Size(107, 34);
            this.btnBuildStop.TabIndex = 6;
            this.btnBuildStop.Text = "生成停止指令";
            this.btnBuildStop.UseVisualStyleBackColor = true;
            this.btnBuildStop.Click += new System.EventHandler(this.btnBuildStop_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 144);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSend);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(302, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(119, 144);
            this.panel2.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtCmd);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 144);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "16进制指令";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(19, 61);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(88, 34);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "发送指令";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtCmd
            // 
            this.txtCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCmd.Location = new System.Drawing.Point(3, 17);
            this.txtCmd.Multiline = true;
            this.txtCmd.Name = "txtCmd";
            this.txtCmd.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCmd.Size = new System.Drawing.Size(296, 124);
            this.txtCmd.TabIndex = 0;
            // 
            // mainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 641);
            this.Controls.Add(this.tbcMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "蓝牙DEMO";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainFrm_FormClosing);
            this.Load += new System.EventHandler(this.mainFrm_Load);
            this.tpgHome.ResumeLayout(false);
            this.tblAutoCheck.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.gpbCheck.ResumeLayout(false);
            this.gpbCheck.PerformLayout();
            this.pnlBluetoothList.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.gpbCmd.ResumeLayout(false);
            this.tblInfo.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tbcMain.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.TabPage tpgHome;
        private System.Windows.Forms.TableLayoutPanel tblAutoCheck;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtShow;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.GroupBox gpbCheck;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSList;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel pnlBluetoothList;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView lvDevices;
        private System.Windows.Forms.GroupBox gpbCmd;
        private System.Windows.Forms.Button btnAutoCheck;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TableLayoutPanel tblInfo;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtOprate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBuildPlay;
        private System.Windows.Forms.Button btnBuildStop;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtCmd;
    }
}

