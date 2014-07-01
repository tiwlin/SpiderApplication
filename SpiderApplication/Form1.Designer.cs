namespace SpiderApplication
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.wbTaobao = new System.Windows.Forms.WebBrowser();
            this.timerBrowser = new System.Windows.Forms.Timer(this.components);
            this.wbQunar = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(36, 29);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 0;
            this.btnInit.Text = "Init";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(135, 29);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(36, 71);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // wbTaobao
            // 
            this.wbTaobao.Location = new System.Drawing.Point(255, 29);
            this.wbTaobao.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbTaobao.Name = "wbTaobao";
            this.wbTaobao.Size = new System.Drawing.Size(634, 325);
            this.wbTaobao.TabIndex = 3;
            // 
            // timerBrowser
            // 
            this.timerBrowser.Tick += new System.EventHandler(this.timerBrowser_Tick);
            // 
            // wbQunar
            // 
            this.wbQunar.Location = new System.Drawing.Point(255, 360);
            this.wbQunar.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbQunar.Name = "wbQunar";
            this.wbQunar.Size = new System.Drawing.Size(634, 320);
            this.wbQunar.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 707);
            this.Controls.Add(this.wbQunar);
            this.Controls.Add(this.wbTaobao);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnInit);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.WebBrowser wbTaobao;
        private System.Windows.Forms.Timer timerBrowser;
        private System.Windows.Forms.WebBrowser wbQunar;
    }
}

