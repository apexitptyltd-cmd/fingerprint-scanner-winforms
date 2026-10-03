namespace ApexITTA
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Main container
            this.SuspendLayout();

            // ===== TOP PANEL: Device Control Buttons =====
            this.pnlDeviceControl = new System.Windows.Forms.Panel();
            this.pnlDeviceControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDeviceControl.Height = 80;
            this.pnlDeviceControl.BackColor = System.Drawing.Color.LightGray;

            // Device combo
            this.lblDevice = new System.Windows.Forms.Label();
            this.lblDevice.Text = "Device:";
            this.lblDevice.Location = new System.Drawing.Point(10, 10);
            this.lblDevice.Size = new System.Drawing.Size(50, 20);

            this.cmbIdx = new System.Windows.Forms.ComboBox();
            this.cmbIdx.Location = new System.Drawing.Point(65, 10);
            this.cmbIdx.Size = new System.Drawing.Size(80, 22);

            // Init button
            this.bnInit = new System.Windows.Forms.Button();
            this.bnInit.Text = "Initialize";
            this.bnInit.Location = new System.Drawing.Point(160, 10);
            this.bnInit.Size = new System.Drawing.Size(90, 30);
            this.bnInit.Click += new System.EventHandler(this.bnInit_Click);

            // Free button
            this.bnFree = new System.Windows.Forms.Button();
            this.bnFree.Text = "Free";
            this.bnFree.Location = new System.Drawing.Point(260, 10);
            this.bnFree.Size = new System.Drawing.Size(90, 30);
            this.bnFree.Enabled = false;
            this.bnFree.Click += new System.EventHandler(this.bnFree_Click);

            // Open button
            this.bnOpen = new System.Windows.Forms.Button();
            this.bnOpen.Text = "Open";
            this.bnOpen.Location = new System.Drawing.Point(360, 10);
            this.bnOpen.Size = new System.Drawing.Size(90, 30);
            this.bnOpen.Enabled = false;
            this.bnOpen.Click += new System.EventHandler(this.bnOpen_Click);

            // Close button
            this.bnClose = new System.Windows.Forms.Button();
            this.bnClose.Text = "Close";
            this.bnClose.Location = new System.Drawing.Point(460, 10);
            this.bnClose.Size = new System.Drawing.Size(90, 30);
            this.bnClose.Enabled = false;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);

            // Member name label and textbox
            this.lblMemberName = new System.Windows.Forms.Label();
            this.lblMemberName.Text = "Member Name:";
            this.lblMemberName.Location = new System.Drawing.Point(10, 45);
            this.lblMemberName.Size = new System.Drawing.Size(100, 20);

            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtUserName.Location = new System.Drawing.Point(115, 45);
            this.txtUserName.Size = new System.Drawing.Size(200, 22);

            this.pnlDeviceControl.Controls.Add(this.lblDevice);
            this.pnlDeviceControl.Controls.Add(this.cmbIdx);
            this.pnlDeviceControl.Controls.Add(this.bnInit);
            this.pnlDeviceControl.Controls.Add(this.bnFree);
            this.pnlDeviceControl.Controls.Add(this.bnOpen);
            this.pnlDeviceControl.Controls.Add(this.bnClose);
            this.pnlDeviceControl.Controls.Add(this.lblMemberName);
            this.pnlDeviceControl.Controls.Add(this.txtUserName);

            // ===== MIDDLE PANEL: Live Capture + Status =====
            this.pnlCapture = new System.Windows.Forms.Panel();
            this.pnlCapture.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCapture.Height = 250;
            this.pnlCapture.BackColor = System.Drawing.Color.White;

            // Live fingerprint capture
            this.lblLiveCapture = new System.Windows.Forms.Label();
            this.lblLiveCapture.Text = "Live Capture";
            this.lblLiveCapture.Location = new System.Drawing.Point(10, 10);
            this.lblLiveCapture.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

            this.picFPImg = new System.Windows.Forms.PictureBox();
            this.picFPImg.Location = new System.Drawing.Point(10, 35);
            this.picFPImg.Size = new System.Drawing.Size(200, 200);
            this.picFPImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFPImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // Status/Instruction text
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatus.Text = "Status:";
            this.lblStatus.Location = new System.Drawing.Point(230, 10);
            this.lblStatus.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

            this.textRes = new System.Windows.Forms.TextBox();
            this.textRes.Location = new System.Drawing.Point(230, 35);
            this.textRes.Size = new System.Drawing.Size(550, 200);
            this.textRes.Multiline = true;
            this.textRes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textRes.ReadOnly = true;
            this.textRes.Font = new System.Drawing.Font("Courier New", 9);

            this.pnlCapture.Controls.Add(this.lblLiveCapture);
            this.pnlCapture.Controls.Add(this.picFPImg);
            this.pnlCapture.Controls.Add(this.lblStatus);
            this.pnlCapture.Controls.Add(this.textRes);

            // ===== ENROLLMENT PANEL: Hand Diagrams with Fingerprint Boxes =====
            this.pnlHandDiagram = new System.Windows.Forms.Panel();
            this.pnlHandDiagram.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHandDiagram.Height = 350;
            this.pnlHandDiagram.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlHandDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Left hand label
            this.lblLeftHand = new System.Windows.Forms.Label();
            this.lblLeftHand.Text = "LEFT HAND";
            this.lblLeftHand.Location = new System.Drawing.Point(50, 10);
            this.lblLeftHand.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.lblLeftHand.Size = new System.Drawing.Size(150, 25);

            // Right hand label
            this.lblRightHand = new System.Windows.Forms.Label();
            this.lblRightHand.Text = "RIGHT HAND";
            this.lblRightHand.Location = new System.Drawing.Point(650, 10);
            this.lblRightHand.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.lblRightHand.Size = new System.Drawing.Size(150, 25);

            this.pnlHandDiagram.Controls.Add(this.lblLeftHand);
            this.pnlHandDiagram.Controls.Add(this.lblRightHand);

            // LEFT HAND FINGERS
            // Thumb
            this.picLeftThumb = new System.Windows.Forms.PictureBox();
            this.picLeftThumb.Location = new System.Drawing.Point(20, 50);
            this.picLeftThumb.Size = new System.Drawing.Size(60, 80);
            this.picLeftThumb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLeftThumb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLeftThumb.BackColor = System.Drawing.Color.LightBlue;
            this.lblLeftThumb = new System.Windows.Forms.Label();
            this.lblLeftThumb.Text = "Thumb";
            this.lblLeftThumb.Location = new System.Drawing.Point(20, 135);
            this.lblLeftThumb.Size = new System.Drawing.Size(60, 20);
            this.lblLeftThumb.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Index
            this.picLeftIndex = new System.Windows.Forms.PictureBox();
            this.picLeftIndex.Location = new System.Drawing.Point(90, 40);
            this.picLeftIndex.Size = new System.Drawing.Size(50, 90);
            this.picLeftIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLeftIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLeftIndex.BackColor = System.Drawing.Color.LightBlue;
            this.lblLeftIndex = new System.Windows.Forms.Label();
            this.lblLeftIndex.Text = "Index";
            this.lblLeftIndex.Location = new System.Drawing.Point(90, 135);
            this.lblLeftIndex.Size = new System.Drawing.Size(50, 20);
            this.lblLeftIndex.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Middle
            this.picLeftMiddle = new System.Windows.Forms.PictureBox();
            this.picLeftMiddle.Location = new System.Drawing.Point(150, 35);
            this.picLeftMiddle.Size = new System.Drawing.Size(50, 95);
            this.picLeftMiddle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLeftMiddle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLeftMiddle.BackColor = System.Drawing.Color.LightBlue;
            this.lblLeftMiddle = new System.Windows.Forms.Label();
            this.lblLeftMiddle.Text = "Middle";
            this.lblLeftMiddle.Location = new System.Drawing.Point(150, 135);
            this.lblLeftMiddle.Size = new System.Drawing.Size(50, 20);
            this.lblLeftMiddle.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Ring
            this.picLeftRing = new System.Windows.Forms.PictureBox();
            this.picLeftRing.Location = new System.Drawing.Point(210, 45);
            this.picLeftRing.Size = new System.Drawing.Size(50, 85);
            this.picLeftRing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLeftRing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLeftRing.BackColor = System.Drawing.Color.LightBlue;
            this.lblLeftRing = new System.Windows.Forms.Label();
            this.lblLeftRing.Text = "Ring";
            this.lblLeftRing.Location = new System.Drawing.Point(210, 135);
            this.lblLeftRing.Size = new System.Drawing.Size(50, 20);
            this.lblLeftRing.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Pinky
            this.picLeftPinky = new System.Windows.Forms.PictureBox();
            this.picLeftPinky.Location = new System.Drawing.Point(270, 60);
            this.picLeftPinky.Size = new System.Drawing.Size(40, 70);
            this.picLeftPinky.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLeftPinky.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLeftPinky.BackColor = System.Drawing.Color.LightBlue;
            this.lblLeftPinky = new System.Windows.Forms.Label();
            this.lblLeftPinky.Text = "Pinky";
            this.lblLeftPinky.Location = new System.Drawing.Point(270, 135);
            this.lblLeftPinky.Size = new System.Drawing.Size(40, 20);
            this.lblLeftPinky.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // RIGHT HAND FINGERS
            // Thumb
            this.picRightThumb = new System.Windows.Forms.PictureBox();
            this.picRightThumb.Location = new System.Drawing.Point(920, 50);
            this.picRightThumb.Size = new System.Drawing.Size(60, 80);
            this.picRightThumb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRightThumb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRightThumb.BackColor = System.Drawing.Color.LightCoral;
            this.lblRightThumb = new System.Windows.Forms.Label();
            this.lblRightThumb.Text = "Thumb";
            this.lblRightThumb.Location = new System.Drawing.Point(920, 135);
            this.lblRightThumb.Size = new System.Drawing.Size(60, 20);
            this.lblRightThumb.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Index
            this.picRightIndex = new System.Windows.Forms.PictureBox();
            this.picRightIndex.Location = new System.Drawing.Point(860, 40);
            this.picRightIndex.Size = new System.Drawing.Size(50, 90);
            this.picRightIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRightIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRightIndex.BackColor = System.Drawing.Color.LightCoral;
            this.lblRightIndex = new System.Windows.Forms.Label();
            this.lblRightIndex.Text = "Index";
            this.lblRightIndex.Location = new System.Drawing.Point(860, 135);
            this.lblRightIndex.Size = new System.Drawing.Size(50, 20);
            this.lblRightIndex.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Middle
            this.picRightMiddle = new System.Windows.Forms.PictureBox();
            this.picRightMiddle.Location = new System.Drawing.Point(800, 35);
            this.picRightMiddle.Size = new System.Drawing.Size(50, 95);
            this.picRightMiddle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRightMiddle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRightMiddle.BackColor = System.Drawing.Color.LightCoral;
            this.lblRightMiddle = new System.Windows.Forms.Label();
            this.lblRightMiddle.Text = "Middle";
            this.lblRightMiddle.Location = new System.Drawing.Point(800, 135);
            this.lblRightMiddle.Size = new System.Drawing.Size(50, 20);
            this.lblRightMiddle.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Ring
            this.picRightRing = new System.Windows.Forms.PictureBox();
            this.picRightRing.Location = new System.Drawing.Point(740, 45);
            this.picRightRing.Size = new System.Drawing.Size(50, 85);
            this.picRightRing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRightRing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRightRing.BackColor = System.Drawing.Color.LightCoral;
            this.lblRightRing = new System.Windows.Forms.Label();
            this.lblRightRing.Text = "Ring";
            this.lblRightRing.Location = new System.Drawing.Point(740, 135);
            this.lblRightRing.Size = new System.Drawing.Size(50, 20);
            this.lblRightRing.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Pinky
            this.picRightPinky = new System.Windows.Forms.PictureBox();
            this.picRightPinky.Location = new System.Drawing.Point(690, 60);
            this.picRightPinky.Size = new System.Drawing.Size(40, 70);
            this.picRightPinky.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRightPinky.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRightPinky.BackColor = System.Drawing.Color.LightCoral;
            this.lblRightPinky = new System.Windows.Forms.Label();
            this.lblRightPinky.Text = "Pinky";
            this.lblRightPinky.Location = new System.Drawing.Point(690, 135);
            this.lblRightPinky.Size = new System.Drawing.Size(40, 20);
            this.lblRightPinky.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // Add all to hand diagram panel
            this.pnlHandDiagram.Controls.Add(this.picLeftThumb);
            this.pnlHandDiagram.Controls.Add(this.lblLeftThumb);
            this.pnlHandDiagram.Controls.Add(this.picLeftIndex);
            this.pnlHandDiagram.Controls.Add(this.lblLeftIndex);
            this.pnlHandDiagram.Controls.Add(this.picLeftMiddle);
            this.pnlHandDiagram.Controls.Add(this.lblLeftMiddle);
            this.pnlHandDiagram.Controls.Add(this.picLeftRing);
            this.pnlHandDiagram.Controls.Add(this.lblLeftRing);
            this.pnlHandDiagram.Controls.Add(this.picLeftPinky);
            this.pnlHandDiagram.Controls.Add(this.lblLeftPinky);
            this.pnlHandDiagram.Controls.Add(this.picRightThumb);
            this.pnlHandDiagram.Controls.Add(this.lblRightThumb);
            this.pnlHandDiagram.Controls.Add(this.picRightIndex);
            this.pnlHandDiagram.Controls.Add(this.lblRightIndex);
            this.pnlHandDiagram.Controls.Add(this.picRightMiddle);
            this.pnlHandDiagram.Controls.Add(this.lblRightMiddle);
            this.pnlHandDiagram.Controls.Add(this.picRightRing);
            this.pnlHandDiagram.Controls.Add(this.lblRightRing);
            this.pnlHandDiagram.Controls.Add(this.picRightPinky);
            this.pnlHandDiagram.Controls.Add(this.lblRightPinky);

            // ===== BOTTOM PANEL: Enrollment/Verification Buttons =====
            this.pnlEnrollment = new System.Windows.Forms.Panel();
            this.pnlEnrollment.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEnrollment.Height = 60;
            this.pnlEnrollment.BackColor = System.Drawing.Color.LightGray;

            // Enroll button
            this.bnEnroll = new System.Windows.Forms.Button();
            this.bnEnroll.Text = "START ENROLLMENT";
            this.bnEnroll.Location = new System.Drawing.Point(10, 10);
            this.bnEnroll.Size = new System.Drawing.Size(180, 40);
            this.bnEnroll.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            this.bnEnroll.BackColor = System.Drawing.Color.LimeGreen;
            this.bnEnroll.ForeColor = System.Drawing.Color.White;
            this.bnEnroll.Enabled = false;
            this.bnEnroll.Click += new System.EventHandler(this.bnEnroll_Click);

            // Verify button
            this.bnVerify = new System.Windows.Forms.Button();
            this.bnVerify.Text = "VERIFY";
            this.bnVerify.Location = new System.Drawing.Point(200, 10);
            this.bnVerify.Size = new System.Drawing.Size(120, 40);
            this.bnVerify.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            this.bnVerify.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.bnVerify.ForeColor = System.Drawing.Color.White;
            this.bnVerify.Enabled = false;
            this.bnVerify.Click += new System.EventHandler(this.bnVerify_Click);

            // Identify button
            this.bnIdentify = new System.Windows.Forms.Button();
            this.bnIdentify.Text = "IDENTIFY";
            this.bnIdentify.Location = new System.Drawing.Point(330, 10);
            this.bnIdentify.Size = new System.Drawing.Size(120, 40);
            this.bnIdentify.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            this.bnIdentify.BackColor = System.Drawing.Color.DarkOrange;
            this.bnIdentify.ForeColor = System.Drawing.Color.White;
            this.bnIdentify.Enabled = false;
            this.bnIdentify.Click += new System.EventHandler(this.bnIdentify_Click);

            this.pnlEnrollment.Controls.Add(this.bnEnroll);
            this.pnlEnrollment.Controls.Add(this.bnVerify);
            this.pnlEnrollment.Controls.Add(this.bnIdentify);

            // ===== ATTENDANCE GRID (BOTTOM) =====
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.BackColor = System.Drawing.Color.White;

            this.lblAttendance = new System.Windows.Forms.Label();
            this.lblAttendance.Text = "Today's Attendance Log";
            this.lblAttendance.Location = new System.Drawing.Point(10, 10);
            this.lblAttendance.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

            this.dgvAttendance = new System.Windows.Forms.DataGridView();
            this.dgvAttendance.Location = new System.Drawing.Point(10, 35);
            this.dgvAttendance.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.dgvAttendance.Size = new System.Drawing.Size(this.ClientSize.Width - 20, this.ClientSize.Height - 50);

            this.pnlGrid.Controls.Add(this.lblAttendance);
            this.pnlGrid.Controls.Add(this.dgvAttendance);

            // ===== Main Form Properties =====
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 900);
            this.Text = "10-Finger Biometric Enrollment System";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.White;

            // Add all panels to form in reverse order (bottom to top visual hierarchy)
            this.Controls.Add(this.pnlGrid);
            this.Controls.Add(this.pnlEnrollment);
            this.Controls.Add(this.pnlHandDiagram);
            this.Controls.Add(this.pnlCapture);
            this.Controls.Add(this.pnlDeviceControl);

            this.Load += new System.EventHandler(this.Form1_Load);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ===== DEVICE CONTROL PANEL =====
        private System.Windows.Forms.Panel pnlDeviceControl;
        private System.Windows.Forms.Label lblDevice;
        public System.Windows.Forms.ComboBox cmbIdx;
        private System.Windows.Forms.Button bnInit;
        private System.Windows.Forms.Button bnFree;
        private System.Windows.Forms.Button bnOpen;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Label lblMemberName;
        public System.Windows.Forms.TextBox txtUserName;

        // ===== CAPTURE PANEL =====
        private System.Windows.Forms.Panel pnlCapture;
        private System.Windows.Forms.Label lblLiveCapture;
        public System.Windows.Forms.PictureBox picFPImg;
        private System.Windows.Forms.Label lblStatus;
        public System.Windows.Forms.TextBox textRes;

        // ===== HAND DIAGRAM PANEL =====
        private System.Windows.Forms.Panel pnlHandDiagram;
        private System.Windows.Forms.Label lblLeftHand;
        private System.Windows.Forms.Label lblRightHand;

        // LEFT HAND CONTROLS
        public System.Windows.Forms.PictureBox picLeftThumb;
        private System.Windows.Forms.Label lblLeftThumb;
        public System.Windows.Forms.PictureBox picLeftIndex;
        private System.Windows.Forms.Label lblLeftIndex;
        public System.Windows.Forms.PictureBox picLeftMiddle;
        private System.Windows.Forms.Label lblLeftMiddle;
        public System.Windows.Forms.PictureBox picLeftRing;
        private System.Windows.Forms.Label lblLeftRing;
        public System.Windows.Forms.PictureBox picLeftPinky;
        private System.Windows.Forms.Label lblLeftPinky;

        // RIGHT HAND CONTROLS
        public System.Windows.Forms.PictureBox picRightThumb;
        private System.Windows.Forms.Label lblRightThumb;
        public System.Windows.Forms.PictureBox picRightIndex;
        private System.Windows.Forms.Label lblRightIndex;
        public System.Windows.Forms.PictureBox picRightMiddle;
        private System.Windows.Forms.Label lblRightMiddle;
        public System.Windows.Forms.PictureBox picRightRing;
        private System.Windows.Forms.Label lblRightRing;
        public System.Windows.Forms.PictureBox picRightPinky;
        private System.Windows.Forms.Label lblRightPinky;

        // ===== ENROLLMENT PANEL =====
        private System.Windows.Forms.Panel pnlEnrollment;
        private System.Windows.Forms.Button bnEnroll;
        private System.Windows.Forms.Button bnVerify;
        private System.Windows.Forms.Button bnIdentify;

        // ===== ATTENDANCE GRID PANEL =====
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.Label lblAttendance;
        public System.Windows.Forms.DataGridView dgvAttendance;
        
        // Placeholder for DB Image display (optional)
        public System.Windows.Forms.PictureBox picDBImg;
    }
}
