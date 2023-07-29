namespace SocketServer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ServerBeginButton = new Button();
            RecvBox = new RichTextBox();
            SendMessBox = new RichTextBox();
            ServerIPAddress = new TextBox();
            ServerPortText = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SendMessButton = new Button();
            label3 = new Label();
            label4 = new Label();
            ServerClosebutton = new Button();
            SuspendLayout();
            // 
            // ServerBeginButton
            // 
            ServerBeginButton.Location = new Point(570, 26);
            ServerBeginButton.Name = "ServerBeginButton";
            ServerBeginButton.Size = new Size(115, 23);
            ServerBeginButton.TabIndex = 0;
            ServerBeginButton.Text = "开启服务器";
            ServerBeginButton.UseVisualStyleBackColor = true;
            ServerBeginButton.Click += ServerOpenClick;
            // 
            // RecvBox
            // 
            RecvBox.Location = new Point(53, 91);
            RecvBox.Name = "RecvBox";
            RecvBox.Size = new Size(225, 104);
            RecvBox.TabIndex = 2;
            RecvBox.Text = "";
            // 
            // SendMessBox
            // 
            SendMessBox.Location = new Point(53, 264);
            SendMessBox.Name = "SendMessBox";
            SendMessBox.Size = new Size(225, 104);
            SendMessBox.TabIndex = 3;
            SendMessBox.Text = "";
            SendMessBox.TextChanged += richTextBox2_TextChanged;
            // 
            // ServerIPAddress
            // 
            ServerIPAddress.Location = new Point(102, 29);
            ServerIPAddress.Name = "ServerIPAddress";
            ServerIPAddress.Size = new Size(137, 23);
            ServerIPAddress.TabIndex = 4;
            // 
            // ServerPortText
            // 
            ServerPortText.Location = new Point(359, 26);
            ServerPortText.Name = "ServerPortText";
            ServerPortText.Size = new Size(100, 23);
            ServerPortText.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 32);
            label1.Name = "label1";
            label1.Size = new Size(55, 17);
            label1.TabIndex = 6;
            label1.Text = "服务器IP";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(298, 29);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 7;
            label2.Text = "端口";
            // 
            // SendMessButton
            // 
            SendMessButton.Location = new Point(570, 91);
            SendMessButton.Name = "SendMessButton";
            SendMessButton.Size = new Size(115, 23);
            SendMessButton.TabIndex = 8;
            SendMessButton.Text = "发送信息";
            SendMessButton.UseVisualStyleBackColor = true;
            SendMessButton.Click += SendMessButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(137, 71);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 9;
            label3.Text = "接受框";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(137, 244);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 10;
            label4.Text = "发送框";
            // 
            // ServerClosebutton
            // 
            ServerClosebutton.Location = new Point(570, 172);
            ServerClosebutton.Name = "ServerClosebutton";
            ServerClosebutton.Size = new Size(115, 23);
            ServerClosebutton.TabIndex = 11;
            ServerClosebutton.Text = "关闭服务器";
            ServerClosebutton.UseVisualStyleBackColor = true;
            ServerClosebutton.Click += CloseServerClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ServerClosebutton);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(SendMessButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ServerPortText);
            Controls.Add(ServerIPAddress);
            Controls.Add(SendMessBox);
            Controls.Add(RecvBox);
            Controls.Add(ServerBeginButton);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ServerBeginButton;
        private RichTextBox RecvBox;
        private RichTextBox SendMessBox;
        private TextBox ServerIPAddress;
        private TextBox ServerPortText;
        private Label label1;
        private Label label2;
        private Button SendMessButton;
        private Label label3;
        private Label label4;
        private Button ServerClosebutton;
    }
}