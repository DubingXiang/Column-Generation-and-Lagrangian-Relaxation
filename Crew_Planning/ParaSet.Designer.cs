namespace Crew_Planning
{
    partial class ParaSet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxIter = new System.Windows.Forms.GroupBox();
            this.radioButtonUB = new System.Windows.Forms.RadioButton();
            this.radioButtonGap = new System.Windows.Forms.RadioButton();
            this.radioButtonk = new System.Windows.Forms.RadioButton();
            this.labelend = new System.Windows.Forms.Label();
            this.groupBoxparaset = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxmindrive = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxmindaycrew = new System.Windows.Forms.TextBox();
            this.labelmindaycrew = new System.Windows.Forms.Label();
            this.textBoxmaxdaycrew = new System.Windows.Forms.TextBox();
            this.textBoxtran = new System.Windows.Forms.TextBox();
            this.labeltran = new System.Windows.Forms.Label();
            this.textBoxmaxoutrelax = new System.Windows.Forms.TextBox();
            this.textBoxminoutrelax = new System.Windows.Forms.TextBox();
            this.labelminoutrelax = new System.Windows.Forms.Label();
            this.textBoxmaxrelax = new System.Windows.Forms.TextBox();
            this.textBoxminrelax = new System.Windows.Forms.TextBox();
            this.labelminrelax = new System.Windows.Forms.Label();
            this.textBoxconn = new System.Windows.Forms.TextBox();
            this.labelconn = new System.Windows.Forms.Label();
            this.textBoxmaxdrive = new System.Windows.Forms.TextBox();
            this.labeldrive = new System.Windows.Forms.Label();
            this.buttonok = new System.Windows.Forms.Button();
            this.buttonclear = new System.Windows.Forms.Button();
            this.textBoxend = new System.Windows.Forms.TextBox();
            this.textBoxdays = new System.Windows.Forms.TextBox();
            this.labeldays = new System.Windows.Forms.Label();
            this.groupdraw = new System.Windows.Forms.GroupBox();
            this.checkCur = new System.Windows.Forms.CheckBox();
            this.checkCurBest = new System.Windows.Forms.CheckBox();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.textBoxstep = new System.Windows.Forms.TextBox();
            this.labelstep = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog3 = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxIter.SuspendLayout();
            this.groupBoxparaset.SuspendLayout();
            this.groupdraw.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxIter
            // 
            this.groupBoxIter.Controls.Add(this.radioButtonUB);
            this.groupBoxIter.Controls.Add(this.radioButtonGap);
            this.groupBoxIter.Controls.Add(this.radioButtonk);
            this.groupBoxIter.Location = new System.Drawing.Point(13, 13);
            this.groupBoxIter.Name = "groupBoxIter";
            this.groupBoxIter.Size = new System.Drawing.Size(165, 101);
            this.groupBoxIter.TabIndex = 0;
            this.groupBoxIter.TabStop = false;
            this.groupBoxIter.Text = "迭代终止条件";
            // 
            // radioButtonUB
            // 
            this.radioButtonUB.AutoSize = true;
            this.radioButtonUB.Location = new System.Drawing.Point(20, 69);
            this.radioButtonUB.Name = "radioButtonUB";
            this.radioButtonUB.Size = new System.Drawing.Size(131, 16);
            this.radioButtonUB.TabIndex = 2;
            this.radioButtonUB.TabStop = true;
            this.radioButtonUB.Text = "最优可行解不变次数";
            this.radioButtonUB.UseVisualStyleBackColor = true;
            this.radioButtonUB.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButtonGap
            // 
            this.radioButtonGap.AutoSize = true;
            this.radioButtonGap.Location = new System.Drawing.Point(20, 47);
            this.radioButtonGap.Name = "radioButtonGap";
            this.radioButtonGap.Size = new System.Drawing.Size(83, 16);
            this.radioButtonGap.TabIndex = 1;
            this.radioButtonGap.TabStop = true;
            this.radioButtonGap.Text = "上下界之差";
            this.radioButtonGap.UseVisualStyleBackColor = true;
            this.radioButtonGap.CheckedChanged += new System.EventHandler(this.radioButtonGap_CheckedChanged);
            // 
            // radioButtonk
            // 
            this.radioButtonk.AutoSize = true;
            this.radioButtonk.Location = new System.Drawing.Point(20, 24);
            this.radioButtonk.Name = "radioButtonk";
            this.radioButtonk.Size = new System.Drawing.Size(71, 16);
            this.radioButtonk.TabIndex = 0;
            this.radioButtonk.TabStop = true;
            this.radioButtonk.Text = "迭代次数";
            this.radioButtonk.UseVisualStyleBackColor = true;
            this.radioButtonk.CheckedChanged += new System.EventHandler(this.radioButtonk_CheckedChanged);
            // 
            // labelend
            // 
            this.labelend.AutoSize = true;
            this.labelend.Location = new System.Drawing.Point(184, 13);
            this.labelend.Name = "labelend";
            this.labelend.Size = new System.Drawing.Size(41, 12);
            this.labelend.TabIndex = 2;
            this.labelend.Text = "label1";
            // 
            // groupBoxparaset
            // 
            this.groupBoxparaset.Controls.Add(this.label5);
            this.groupBoxparaset.Controls.Add(this.label4);
            this.groupBoxparaset.Controls.Add(this.label3);
            this.groupBoxparaset.Controls.Add(this.textBoxmindrive);
            this.groupBoxparaset.Controls.Add(this.label2);
            this.groupBoxparaset.Controls.Add(this.textBoxmindaycrew);
            this.groupBoxparaset.Controls.Add(this.labelmindaycrew);
            this.groupBoxparaset.Controls.Add(this.textBoxmaxdaycrew);
            this.groupBoxparaset.Controls.Add(this.textBoxtran);
            this.groupBoxparaset.Controls.Add(this.labeltran);
            this.groupBoxparaset.Controls.Add(this.textBoxmaxoutrelax);
            this.groupBoxparaset.Controls.Add(this.textBoxminoutrelax);
            this.groupBoxparaset.Controls.Add(this.labelminoutrelax);
            this.groupBoxparaset.Controls.Add(this.textBoxmaxrelax);
            this.groupBoxparaset.Controls.Add(this.textBoxminrelax);
            this.groupBoxparaset.Controls.Add(this.labelminrelax);
            this.groupBoxparaset.Controls.Add(this.textBoxconn);
            this.groupBoxparaset.Controls.Add(this.labelconn);
            this.groupBoxparaset.Controls.Add(this.textBoxmaxdrive);
            this.groupBoxparaset.Controls.Add(this.labeldrive);
            this.groupBoxparaset.Location = new System.Drawing.Point(12, 126);
            this.groupBoxparaset.Name = "groupBoxparaset";
            this.groupBoxparaset.Size = new System.Drawing.Size(435, 176);
            this.groupBoxparaset.TabIndex = 3;
            this.groupBoxparaset.TabStop = false;
            this.groupBoxparaset.Text = "参数设置";
            this.groupBoxparaset.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(102, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 22;
            this.label5.Text = "—";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(100, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 21;
            this.label4.Text = "—";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(314, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "—";
            // 
            // textBoxmindrive
            // 
            this.textBoxmindrive.Location = new System.Drawing.Point(233, 44);
            this.textBoxmindrive.Name = "textBoxmindrive";
            this.textBoxmindrive.Size = new System.Drawing.Size(75, 21);
            this.textBoxmindrive.TabIndex = 19;
            this.textBoxmindrive.Text = "210";
            this.textBoxmindrive.TextChanged += new System.EventHandler(this.textBoxmindrive_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(314, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "—";
            // 
            // textBoxmindaycrew
            // 
            this.textBoxmindaycrew.Location = new System.Drawing.Point(19, 44);
            this.textBoxmindaycrew.Name = "textBoxmindaycrew";
            this.textBoxmindaycrew.Size = new System.Drawing.Size(75, 21);
            this.textBoxmindaycrew.TabIndex = 17;
            this.textBoxmindaycrew.Text = "90";
            this.textBoxmindaycrew.TextChanged += new System.EventHandler(this.textBoxmindaycrew_TextChanged);
            // 
            // labelmindaycrew
            // 
            this.labelmindaycrew.AutoSize = true;
            this.labelmindaycrew.Location = new System.Drawing.Point(64, 26);
            this.labelmindaycrew.Name = "labelmindaycrew";
            this.labelmindaycrew.Size = new System.Drawing.Size(77, 12);
            this.labelmindaycrew.TabIndex = 16;
            this.labelmindaycrew.Text = "交路段时间窗";
            this.labelmindaycrew.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBoxmaxdaycrew
            // 
            this.textBoxmaxdaycrew.Location = new System.Drawing.Point(123, 44);
            this.textBoxmaxdaycrew.Name = "textBoxmaxdaycrew";
            this.textBoxmaxdaycrew.Size = new System.Drawing.Size(75, 21);
            this.textBoxmaxdaycrew.TabIndex = 15;
            this.textBoxmaxdaycrew.Text = "550";
            // 
            // textBoxtran
            // 
            this.textBoxtran.Location = new System.Drawing.Point(293, 148);
            this.textBoxtran.Name = "textBoxtran";
            this.textBoxtran.Size = new System.Drawing.Size(75, 21);
            this.textBoxtran.TabIndex = 13;
            this.textBoxtran.Text = "15";
            // 
            // labeltran
            // 
            this.labeltran.AutoSize = true;
            this.labeltran.Location = new System.Drawing.Point(290, 128);
            this.labeltran.Name = "labeltran";
            this.labeltran.Size = new System.Drawing.Size(77, 12);
            this.labeltran.TabIndex = 12;
            this.labeltran.Text = "最小换乘时间";
            // 
            // textBoxmaxoutrelax
            // 
            this.textBoxmaxoutrelax.Location = new System.Drawing.Point(339, 96);
            this.textBoxmaxoutrelax.Name = "textBoxmaxoutrelax";
            this.textBoxmaxoutrelax.Size = new System.Drawing.Size(76, 21);
            this.textBoxmaxoutrelax.TabIndex = 11;
            this.textBoxmaxoutrelax.Text = "840";
            // 
            // textBoxminoutrelax
            // 
            this.textBoxminoutrelax.Location = new System.Drawing.Point(233, 96);
            this.textBoxminoutrelax.Name = "textBoxminoutrelax";
            this.textBoxminoutrelax.Size = new System.Drawing.Size(75, 21);
            this.textBoxminoutrelax.TabIndex = 9;
            this.textBoxminoutrelax.Text = "600";
            // 
            // labelminoutrelax
            // 
            this.labelminoutrelax.AutoSize = true;
            this.labelminoutrelax.Location = new System.Drawing.Point(283, 77);
            this.labelminoutrelax.Name = "labelminoutrelax";
            this.labelminoutrelax.Size = new System.Drawing.Size(89, 12);
            this.labelminoutrelax.TabIndex = 8;
            this.labelminoutrelax.Text = "外段驻班时间窗";
            // 
            // textBoxmaxrelax
            // 
            this.textBoxmaxrelax.Location = new System.Drawing.Point(125, 96);
            this.textBoxmaxrelax.Name = "textBoxmaxrelax";
            this.textBoxmaxrelax.Size = new System.Drawing.Size(75, 21);
            this.textBoxmaxrelax.TabIndex = 7;
            this.textBoxmaxrelax.Text = "120";
            // 
            // textBoxminrelax
            // 
            this.textBoxminrelax.Location = new System.Drawing.Point(21, 96);
            this.textBoxminrelax.Name = "textBoxminrelax";
            this.textBoxminrelax.Size = new System.Drawing.Size(75, 21);
            this.textBoxminrelax.TabIndex = 5;
            this.textBoxminrelax.Text = "90";
            // 
            // labelminrelax
            // 
            this.labelminrelax.AutoSize = true;
            this.labelminrelax.Location = new System.Drawing.Point(71, 77);
            this.labelminrelax.Name = "labelminrelax";
            this.labelminrelax.Size = new System.Drawing.Size(65, 12);
            this.labelminrelax.TabIndex = 4;
            this.labelminrelax.Text = "间休时间窗";
            // 
            // textBoxconn
            // 
            this.textBoxconn.Location = new System.Drawing.Point(79, 148);
            this.textBoxconn.Name = "textBoxconn";
            this.textBoxconn.Size = new System.Drawing.Size(73, 21);
            this.textBoxconn.TabIndex = 3;
            this.textBoxconn.Text = "20";
            this.textBoxconn.TextChanged += new System.EventHandler(this.textBoxconn_TextChanged);
            // 
            // labelconn
            // 
            this.labelconn.AutoSize = true;
            this.labelconn.Location = new System.Drawing.Point(75, 128);
            this.labelconn.Name = "labelconn";
            this.labelconn.Size = new System.Drawing.Size(77, 12);
            this.labelconn.TabIndex = 2;
            this.labelconn.Text = "最大接续时间";
            // 
            // textBoxmaxdrive
            // 
            this.textBoxmaxdrive.Location = new System.Drawing.Point(340, 44);
            this.textBoxmaxdrive.Name = "textBoxmaxdrive";
            this.textBoxmaxdrive.Size = new System.Drawing.Size(75, 21);
            this.textBoxmaxdrive.TabIndex = 1;
            this.textBoxmaxdrive.Text = "270";
            // 
            // labeldrive
            // 
            this.labeldrive.AutoSize = true;
            this.labeldrive.Location = new System.Drawing.Point(275, 26);
            this.labeldrive.Name = "labeldrive";
            this.labeldrive.Size = new System.Drawing.Size(101, 12);
            this.labeldrive.TabIndex = 0;
            this.labeldrive.Text = "不间断驾驶时间窗";
            // 
            // buttonok
            // 
            this.buttonok.Location = new System.Drawing.Point(34, 318);
            this.buttonok.Name = "buttonok";
            this.buttonok.Size = new System.Drawing.Size(75, 23);
            this.buttonok.TabIndex = 4;
            this.buttonok.Text = "计算";
            this.buttonok.UseVisualStyleBackColor = true;
            this.buttonok.Click += new System.EventHandler(this.buttonok_Click);
            // 
            // buttonclear
            // 
            this.buttonclear.Location = new System.Drawing.Point(192, 318);
            this.buttonclear.Name = "buttonclear";
            this.buttonclear.Size = new System.Drawing.Size(75, 23);
            this.buttonclear.TabIndex = 5;
            this.buttonclear.Text = "清空";
            this.buttonclear.UseVisualStyleBackColor = true;
            // 
            // textBoxend
            // 
            this.textBoxend.Location = new System.Drawing.Point(184, 37);
            this.textBoxend.Name = "textBoxend";
            this.textBoxend.Size = new System.Drawing.Size(108, 21);
            this.textBoxend.TabIndex = 6;
            this.textBoxend.Text = "20";
            // 
            // textBoxdays
            // 
            this.textBoxdays.Location = new System.Drawing.Point(184, 93);
            this.textBoxdays.Name = "textBoxdays";
            this.textBoxdays.Size = new System.Drawing.Size(53, 21);
            this.textBoxdays.TabIndex = 8;
            this.textBoxdays.Text = "1";
            // 
            // labeldays
            // 
            this.labeldays.AutoSize = true;
            this.labeldays.Location = new System.Drawing.Point(184, 69);
            this.labeldays.Name = "labeldays";
            this.labeldays.Size = new System.Drawing.Size(53, 12);
            this.labeldays.TabIndex = 7;
            this.labeldays.Text = "研究天数";
            // 
            // groupdraw
            // 
            this.groupdraw.Controls.Add(this.checkCur);
            this.groupdraw.Controls.Add(this.checkCurBest);
            this.groupdraw.Location = new System.Drawing.Point(330, 13);
            this.groupdraw.Name = "groupdraw";
            this.groupdraw.Size = new System.Drawing.Size(118, 101);
            this.groupdraw.TabIndex = 9;
            this.groupdraw.TabStop = false;
            this.groupdraw.Text = "输出图像";
            // 
            // checkCur
            // 
            this.checkCur.AutoSize = true;
            this.checkCur.Location = new System.Drawing.Point(21, 68);
            this.checkCur.Name = "checkCur";
            this.checkCur.Size = new System.Drawing.Size(72, 16);
            this.checkCur.TabIndex = 1;
            this.checkCur.Text = "迭代图像";
            this.checkCur.UseVisualStyleBackColor = true;
            // 
            // checkCurBest
            // 
            this.checkCurBest.AutoSize = true;
            this.checkCurBest.Location = new System.Drawing.Point(21, 23);
            this.checkCurBest.Name = "checkCurBest";
            this.checkCurBest.Size = new System.Drawing.Size(72, 16);
            this.checkCurBest.TabIndex = 0;
            this.checkCurBest.Text = "收敛图像";
            this.checkCurBest.UseVisualStyleBackColor = true;
            // 
            // buttonQuit
            // 
            this.buttonQuit.Location = new System.Drawing.Point(355, 318);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(75, 23);
            this.buttonQuit.TabIndex = 10;
            this.buttonQuit.Text = "退出";
            this.buttonQuit.UseVisualStyleBackColor = true;
            // 
            // textBoxstep
            // 
            this.textBoxstep.Location = new System.Drawing.Point(265, 93);
            this.textBoxstep.Name = "textBoxstep";
            this.textBoxstep.Size = new System.Drawing.Size(53, 21);
            this.textBoxstep.TabIndex = 12;
            this.textBoxstep.Text = "80";
            // 
            // labelstep
            // 
            this.labelstep.AutoSize = true;
            this.labelstep.Location = new System.Drawing.Point(265, 69);
            this.labelstep.Name = "labelstep";
            this.labelstep.Size = new System.Drawing.Size(53, 12);
            this.labelstep.TabIndex = 11;
            this.labelstep.Text = "迭代步长";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 344);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 13;
            this.label1.Visible = false;
            // 
            // ParaSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 363);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxstep);
            this.Controls.Add(this.labelstep);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.groupdraw);
            this.Controls.Add(this.textBoxdays);
            this.Controls.Add(this.labeldays);
            this.Controls.Add(this.textBoxend);
            this.Controls.Add(this.buttonclear);
            this.Controls.Add(this.buttonok);
            this.Controls.Add(this.groupBoxparaset);
            this.Controls.Add(this.labelend);
            this.Controls.Add(this.groupBoxIter);
            this.Name = "ParaSet";
            this.Text = "参数设置";
            this.Load += new System.EventHandler(this.ParaSet_Load);
            this.groupBoxIter.ResumeLayout(false);
            this.groupBoxIter.PerformLayout();
            this.groupBoxparaset.ResumeLayout(false);
            this.groupBoxparaset.PerformLayout();
            this.groupdraw.ResumeLayout(false);
            this.groupdraw.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxIter;
        private System.Windows.Forms.RadioButton radioButtonGap;
        private System.Windows.Forms.RadioButton radioButtonk;
        private System.Windows.Forms.Label labelend;
        private System.Windows.Forms.GroupBox groupBoxparaset;
        private System.Windows.Forms.Label labelconn;
        private System.Windows.Forms.TextBox textBoxmaxdrive;
        private System.Windows.Forms.Label labeldrive;
        private System.Windows.Forms.TextBox textBoxtran;
        private System.Windows.Forms.Label labeltran;
        private System.Windows.Forms.TextBox textBoxmaxoutrelax;
        private System.Windows.Forms.TextBox textBoxminoutrelax;
        private System.Windows.Forms.Label labelminoutrelax;
        private System.Windows.Forms.TextBox textBoxmaxrelax;
        private System.Windows.Forms.TextBox textBoxminrelax;
        private System.Windows.Forms.Label labelminrelax;
        private System.Windows.Forms.TextBox textBoxconn;
        private System.Windows.Forms.Button buttonok;
        private System.Windows.Forms.Button buttonclear;
        private System.Windows.Forms.TextBox textBoxend;
        private System.Windows.Forms.TextBox textBoxmaxdaycrew;
        private System.Windows.Forms.TextBox textBoxdays;
        private System.Windows.Forms.Label labeldays;
        private System.Windows.Forms.RadioButton radioButtonUB;
        private System.Windows.Forms.TextBox textBoxmindaycrew;
        private System.Windows.Forms.Label labelmindaycrew;
        private System.Windows.Forms.GroupBox groupdraw;
        private System.Windows.Forms.CheckBox checkCur;
        private System.Windows.Forms.CheckBox checkCurBest;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.TextBox textBoxstep;
        private System.Windows.Forms.Label labelstep;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxmindrive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog3;

    }
}