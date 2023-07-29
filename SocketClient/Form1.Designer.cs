namespace SocketClient
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
            label1 = new Label();
            label2 = new Label();
            ServerIPAddress = new TextBox();
            ServerPortText = new TextBox();
            MessSendButton = new Button();
            SendMessBox = new RichTextBox();
            AddressSendButton = new Button();
            SocketCloseButton = new Button();
            RecvBox = new RichTextBox();
            label3 = new Label();
            label4 = new Label();
            ClientStateBox = new RichTextBox();
            label5 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(63, 29);
            label1.Name = "label1";
            label1.Size = new Size(55, 17);
            label1.TabIndex = 7;
            label1.Text = "服务器IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(326, 29);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 8;
            label2.Text = "端口";
            label2.Click += label2_Click;
            // 
            // ServerIPAddress
            // 
            ServerIPAddress.Location = new Point(124, 26);
            ServerIPAddress.Name = "ServerIPAddress";
            ServerIPAddress.Size = new Size(137, 23);
            ServerIPAddress.TabIndex = 9;
            // 
            // ServerPortText
            // 
            ServerPortText.Location = new Point(364, 26);
            ServerPortText.Name = "ServerPortText";
            ServerPortText.Size = new Size(137, 23);
            ServerPortText.TabIndex = 10;
            // 
            // MessSendButton
            // 
            MessSendButton.Location = new Point(615, 112);
            MessSendButton.Name = "MessSendButton";
            MessSendButton.Size = new Size(115, 23);
            MessSendButton.TabIndex = 11;
            MessSendButton.Text = "发送信息";
            MessSendButton.UseVisualStyleBackColor = true;
            MessSendButton.Click += SendMessClick;
            // 
            // SendMessBox
            // 
            SendMessBox.Location = new Point(63, 103);
            SendMessBox.Name = "SendMessBox";
            SendMessBox.Size = new Size(225, 104);
            SendMessBox.TabIndex = 12;
            SendMessBox.Text = "";
            // 
            // AddressSendButton
            // 
            AddressSendButton.Location = new Point(615, 70);
            AddressSendButton.Name = "AddressSendButton";
            AddressSendButton.Size = new Size(115, 23);
            AddressSendButton.TabIndex = 13;
            AddressSendButton.Text = "连接服务器";
            AddressSendButton.UseVisualStyleBackColor = true;
            AddressSendButton.Click += ConnectToServerClick;
            // 
            // SocketCloseButton
            // 
            SocketCloseButton.Location = new Point(615, 160);
            SocketCloseButton.Name = "SocketCloseButton";
            SocketCloseButton.Size = new Size(115, 23);
            SocketCloseButton.TabIndex = 14;
            SocketCloseButton.Text = "断开连接";
            SocketCloseButton.UseVisualStyleBackColor = true;
            SocketCloseButton.Click += SocketCloseClick;
            // 
            // RecvBox
            // 
            RecvBox.Location = new Point(63, 260);
            RecvBox.Name = "RecvBox";
            RecvBox.Size = new Size(225, 104);
            RecvBox.TabIndex = 15;
            RecvBox.Text = "";
            RecvBox.TextChanged += RecvBox_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(142, 83);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 16;
            label3.Text = "发送端";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(142, 240);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 17;
            label4.Text = "接收端";
            // 
            // ClientStateBox
            // 
            ClientStateBox.Location = new Point(446, 260);
            ClientStateBox.Name = "ClientStateBox";
            ClientStateBox.Size = new Size(225, 104);
            ClientStateBox.TabIndex = 18;
            ClientStateBox.Text = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(538, 240);
            label5.Name = "label5";
            label5.Size = new Size(44, 17);
            label5.TabIndex = 19;
            label5.Text = "状态框";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label5);
            Controls.Add(ClientStateBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(RecvBox);
            Controls.Add(SocketCloseButton);
            Controls.Add(AddressSendButton);
            Controls.Add(SendMessBox);
            Controls.Add(MessSendButton);
            Controls.Add(ServerPortText);
            Controls.Add(ServerIPAddress);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox ServerIPAddress;
        private TextBox ServerPortText;
        private Button MessSendButton;
        private RichTextBox SendMessBox;
        private Button AddressSendButton;
        private Button SocketCloseButton;
        private RichTextBox RecvBox;
        private Label label3;
        private Label label4;
        private RichTextBox ClientStateBox;
        private Label label5;
    }
}