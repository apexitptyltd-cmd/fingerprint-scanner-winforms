using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using libzkfpcsharp;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Sample;

namespace ApexITTA
{
    public partial class Form1 : Form
    {
        // Finger Position Enumeration
        private enum HandSide
        {
            Left,
            Right
        }

        private enum FingerType
        {
            Thumb = 0,
            Index = 1,
            Middle = 2,
            Ring = 3,
            Pinky = 4
        }

        // Finger scan sequence tracking structure
        private class FingerprintStep
        {
            public HandSide Hand { get; set; }
            public FingerType Finger { get; set; }
            public int StepIndex { get; set; }
            public byte[] Template { get; set; }
            public bool IsComplete { get; set; }

            public FingerprintStep(int index, HandSide hand, FingerType finger)
            {
                StepIndex = index;
                Hand = hand;
                Finger = finger;
                Template = new byte[2048];
                IsComplete = false;
            }

            public override string ToString()
            {
                return $"{Hand} {Finger}";
            }

            public string GetDisplayName()
            {
                string handDisplay = Hand == HandSide.Left ? "LEFT" : "RIGHT";
                string fingerDisplay = Finger.ToString().ToUpper();
                return $"{handDisplay} {fingerDisplay}";
            }
        }

        // Core Context Tracking Engine Native Handles
        private IntPtr mDevHandle = IntPtr.Zero;
        private IntPtr mDBHandle = IntPtr.Zero;
        private IntPtr FormHandle = IntPtr.Zero;

        // Operation Control Variables
        private bool bIsTimeToDie = false;
        private bool IsRegister = false;
        private bool bIdentify = true;
        private byte[] FPBuffer;
        private int RegisterCount = 0;
        private const int REGISTER_FINGER_COUNT = 10; // 10 fingers total
        private const int SCANS_PER_FINGER = 3; // 3 scans per finger for quality verification
        private int ScansForCurrentFinger = 0; // Track scans for current finger

        // Finger scan sequence - Left hand first (Thumb to Pinky), then Right hand (Thumb to Pinky)
        private readonly List<FingerprintStep> _fingerprintSequence = new List<FingerprintStep>
        {
            // Left Hand
            new FingerprintStep(0, HandSide.Left, FingerType.Thumb),
            new FingerprintStep(1, HandSide.Left, FingerType.Index),
            new FingerprintStep(2, HandSide.Left, FingerType.Middle),
            new FingerprintStep(3, HandSide.Left, FingerType.Ring),
            new FingerprintStep(4, HandSide.Left, FingerType.Pinky),
            // Right Hand
            new FingerprintStep(5, HandSide.Right, FingerType.Thumb),
            new FingerprintStep(6, HandSide.Right, FingerType.Index),
            new FingerprintStep(7, HandSide.Right, FingerType.Middle),
            new FingerprintStep(8, HandSide.Right, FingerType.Ring),
            new FingerprintStep(9, HandSide.Right, FingerType.Pinky)
        };

        // Dynamic Native Memory Structural Holding Segments
        private byte[][] RegTmps = new byte[10][]; // 10 fingers
        private byte[] RegTmp = new byte[2048];
        private byte[] CapTmp = new byte[2048];
        private int cbCapTmp = 2048;
        private int cbRegTmp = 0;
        private int iFid = 1;
        private Thread captureThread = null;

        // Dynamic Device Parameter Dimensions Data Variables
        private int mfpWidth = 0;
        private int mfpHeight = 0;

        // Windows Hook Intercept Code Pointers
        private const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        // Target SQL Database Connection Configuration
        // NOTE: Modify server parameters to point to your target SQL instance
        private string connectionString = @"Data Source=192.168.0.220;Initial Catalog=ApexITTA;Persist Security Info=True;User ID=sa;Password=Wynand@5978";

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormHandle = this.Handle; // Bind window handle context mapping dependencies
            ConfigureAttendanceGrid(); // Prepare dashboard UI columns
            LoadTodayAttendance();     // Pull existing history logs on launch
        }

        /// <summary>
        /// Sets up the visual dashboard columns cleanly.
        /// </summary>
        private void ConfigureAttendanceGrid()
        {
            if (dgvAttendance == null) return;

            dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAttendance.AllowUserToAddRows = false;
            dgvAttendance.AllowUserToDeleteRows = false;
            dgvAttendance.ReadOnly = true;
            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void bnInit_Click(object sender, EventArgs e)
        {
            cmbIdx.Items.Clear();
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount();
                if (nCount > 0)
                {
                    for (int i = 0; i < nCount; i++)
                    {
                        cmbIdx.Items.Add(i.ToString());
                    }
                    cmbIdx.SelectedIndex = 0;
                    bnInit.Enabled = false;
                    bnFree.Enabled = true;
                    bnOpen.Enabled = true;
                }
                else
                {
                    zkfp2.Terminate();
                    MessageBox.Show("No device connected!");
                }
            }
            else
            {
                MessageBox.Show("Initialize fail, ret=" + ret + " !");
            }
        }

        private void bnFree_Click(object sender, EventArgs e)
        {
            zkfp2.Terminate();
            cbRegTmp = 0;
            bnInit.Enabled = true;
            bnFree.Enabled = false;
            bnOpen.Enabled = false;
            bnClose.Enabled = false;
            bnEnroll.Enabled = false;
            bnVerify.Enabled = false;
            bnIdentify.Enabled = false;
        }

        private void bnOpen_Click(object sender, EventArgs e)
        {
            int ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(cmbIdx.SelectedIndex)))
            {
                MessageBox.Show("OpenDevice fail");
                return;
            }
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return;
            }
            bnInit.Enabled = false;
            bnFree.Enabled = true;
            bnOpen.Enabled = false;
            bnClose.Enabled = true;
            bnEnroll.Enabled = true;
            bnVerify.Enabled = true;
            bnIdentify.Enabled = true;
            RegisterCount = 0;
            cbRegTmp = 0;
            iFid = 1;
            ScansForCurrentFinger = 0;
            
            // Initialize 10 finger templates
            for (int i = 0; i < 10; i++)
            {
                RegTmps[i] = new byte[2048];
            }
            
            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            bIsTimeToDie = false;
            textRes.Text = "Device opened successfully. Ready for enrollment.";
        }

        private void CloseDevice()
        {
            if (IntPtr.Zero != mDevHandle)
            {
                bIsTimeToDie = true;
                Thread.Sleep(1000);
                if (captureThread != null)
                    captureThread.Join();
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
            }
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            CloseDevice();
            RegisterCount = 0;
            ScansForCurrentFinger = 0;
            Thread.Sleep(1000);
            bnInit.Enabled = false;
            bnFree.Enabled = true;
            bnOpen.Enabled = true;
            bnClose.Enabled = false;
            bnEnroll.Enabled = false;
            bnVerify.Enabled = false;
            bnIdentify.Enabled = false;
        }

        private void bnEnroll_Click(object sender, EventArgs e)
        {
            if (!IsRegister)
            {
                // Validate user name before starting enrollment
                if (string.IsNullOrWhiteSpace(txtUserName.Text))
                {
                    MessageBox.Show("Please enter a member name before starting enrollment.", "Enrollment Error");
                    return;
                }

                IsRegister = true;
                RegisterCount = 0;
                ScansForCurrentFinger = 0;
                cbRegTmp = 0;

                // Clear enrollment data
                for (int i = 0; i < 10; i++)
                {
                    _fingerprintSequence[i].IsComplete = false;
                    _fingerprintSequence[i].Template = new byte[2048];
                }

                textRes.Text = "========== 10-FINGER ENROLLMENT STARTED ==========\r\n" +
                              "Sequence: LEFT HAND (Thumb→Index→Middle→Ring→Pinky)\r\n" +
                              "Then: RIGHT HAND (Thumb→Index→Middle→Ring→Pinky)\r\n" +
                              "Each finger: 3 scans for quality verification";
                
                // Start with first finger
                UpdateInstructionForCurrentFinger();
            }
        }

        private void bnIdentify_Click(object sender, EventArgs e)
        {
            if (!bIdentify)
            {
                bIdentify = true;
                textRes.Text = "Please press your finger!";
            }
        }

        private void bnVerify_Click(object sender, EventArgs e)
        {
            if (bIdentify)
            {
                bIdentify = false;
                textRes.Text = "Please press your finger!";
            }
        }

        /// <summary>
        /// Updates the UI instruction for the current finger in enrollment sequence
        /// </summary>
        private void UpdateInstructionForCurrentFinger()
        {
            if (RegisterCount < _fingerprintSequence.Count)
            {
                FingerprintStep currentStep = _fingerprintSequence[RegisterCount];
                int fingerProgressPercent = ((RegisterCount + 1) * 100) / REGISTER_FINGER_COUNT;
                int scanNum = ScansForCurrentFinger + 1;
                
                string fingerDisplay = currentStep.GetDisplayName();
                string instruction = $"[FINGER {RegisterCount + 1}/10 - SCAN {scanNum}/{SCANS_PER_FINGER}]\r\n" +
                                    $"Position: {fingerDisplay}\r\n" +
                                    $"Overall Progress: {fingerProgressPercent}%\r\n\r\n" +
                                    $"⚠ PLACE FINGER ON SCANNER NOW";

                textRes.Text = instruction;
            }
            else
            {
                textRes.Text = "All 10 fingers enrolled. Processing...";
            }
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                }
                Thread.Sleep(200);
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MESSAGE_CAPTURED_OK:
                    {
                        // Safe MemoryStream processing using explicit cloned bitmap creation to avoid crash
                        MemoryStream ms = new MemoryStream();
                        BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
                        if (ms.Length > 0)
                        {
                            using (ms)
                            {
                                using (Image img = Image.FromStream(ms))
                                {
                                    this.picFPImg.Image = new Bitmap(img);
                                }
                            }
                        }

                        if (IsRegister)
                        {
                            ProcessMultiFingerEnrollment();
                        }
                        else
                        {
                            if (bIdentify)
                            {
                                int matchedId = IdentifyFromSqlDatabase(CapTmp);

                                if (matchedId > 0)
                                {
                                    FetchAndDisplayUserProfile(matchedId);
                                }
                                else
                                {
                                    textRes.Text = "Access Authorization Refused: No matching biometric signature.";
                                    if (picDBImg != null) picDBImg.Image = null;
                                }
                            }
                        }
                        break;
                    }
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// Processes enrollment for all 10 fingers sequentially with per-finger quality validation
        /// </summary>
        private void ProcessMultiFingerEnrollment()
        {
            if (RegisterCount >= REGISTER_FINGER_COUNT)
                return;

            int ret = zkfp.ZKFP_ERR_OK;
            int fid = 0, score = 0;

            FingerprintStep currentStep = _fingerprintSequence[RegisterCount];

            // Check if fingerprint already exists in local DB
            ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
            if (zkfp.ZKFP_ERR_OK == ret)
            {
                textRes.Text = $"ERROR: {currentStep.GetDisplayName()} fingerprint already matched to another record.\r\n" +
                              "Try again with a different finger placement.";
                ScansForCurrentFinger = 0;
                return;
            }

            // For multi-scan verification: compare with first scan of current finger
            if (ScansForCurrentFinger > 0)
            {
                // Create temporary storage for first scan
                byte[] firstScan = new byte[2048];
                Array.Copy(currentStep.Template, firstScan, 2048);

                int matchScore = zkfp2.DBMatch(mDBHandle, CapTmp, firstScan);
                if (matchScore <= 0)
                {
                    textRes.Text = $"QUALITY CHECK FAILED: {currentStep.GetDisplayName()}\r\n" +
                                  $"This scan doesn't match the first scan.\r\n" +
                                  "Try again - ensure you're placing the same finger.";
                    ScansForCurrentFinger = 0;
                    return;
                }
            }

            // Store this scan attempt
            Array.Copy(CapTmp, currentStep.Template, cbCapTmp);
            ScansForCurrentFinger++;

            // Check if we have enough scans for this finger
            if (ScansForCurrentFinger >= SCANS_PER_FINGER)
            {
                // Finger enrollment complete - move to next finger
                currentStep.IsComplete = true;
                string fingerName = currentStep.GetDisplayName();
                
                textRes.Text = $"✓ {fingerName} ENROLLED SUCCESSFULLY\r\n" +
                              $"Moving to next finger...";

                // Move to next finger
                RegisterCount++;
                ScansForCurrentFinger = 0;

                // Check if all fingers are done
                if (RegisterCount >= REGISTER_FINGER_COUNT)
                {
                    // All fingers captured
                    CompleteTenFingerEnrollment();
                    IsRegister = false;
                    return;
                }

                // Brief delay before prompting for next finger
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
                {
                    if (IsRegister)
                    {
                        UpdateInstructionForCurrentFinger();
                    }
                });
            }
            else
            {
                // Need more scans for this finger
                int remaining = SCANS_PER_FINGER - ScansForCurrentFinger;
                string fingerName = currentStep.GetDisplayName();
                textRes.Text = $"✓ Scan {ScansForCurrentFinger}/{SCANS_PER_FINGER} captured for {fingerName}\r\n" +
                              $"Need {remaining} more scan(s) of this finger";
            }
        }

        /// <summary>
        /// Completes the 10-finger enrollment and saves to database
        /// </summary>
        private void CompleteTenFingerEnrollment()
        {
            RegisterCount = 0;
            cbRegTmp = 2048;

            string userNameInput = string.IsNullOrEmpty(txtUserName.Text) ? "New Enrollee Profile" : txtUserName.Text.Trim();
            
            // Prepare 10-finger templates with hand/finger identifiers
            Dictionary<string, byte[]> fingerTemplates = new Dictionary<string, byte[]>();
            
            for (int i = 0; i < 10; i++)
            {
                FingerprintStep step = _fingerprintSequence[i];
                string fingerKey = $"{step.Hand}_{step.Finger}";
                fingerTemplates[fingerKey] = step.Template;
            }

            // Capture current fingerprint image
            byte[] finalImageBlob = null;
            if (picFPImg.Image != null)
            {
                using (MemoryStream imgMs = new MemoryStream())
                {
                    picFPImg.Image.Save(imgMs, System.Drawing.Imaging.ImageFormat.Bmp);
                    finalImageBlob = imgMs.ToArray();
                }
            }

            // Save all 10 fingerprints to SQL database
            if (SaveMultiFingerToSql(userNameInput, fingerTemplates, finalImageBlob))
            {
                textRes.Text = $"✓✓✓ 10-FINGER ENROLLMENT COMPLETE ✓✓✓\r\n\r\n" +
                              $"Member: {userNameInput}\r\n" +
                              $"All 10 fingerprints saved to database.\r\n" +
                              $"Ready for identification.";
                
                // Reset for next enrollment
                IsRegister = false;
                ScansForCurrentFinger = 0;
            }
            else
            {
                textRes.Text = "ERROR: 10-finger templates captured, but SQL server upload failed.\r\n" +
                              "Please check your database connection.";
            }
        }

        /// <summary>
        /// Saves all 10 fingerprints to SQL database with finger position identifiers
        /// </summary>
        private bool SaveMultiFingerToSql(string name, Dictionary<string, byte[]> fingerTemplates, byte[] lastCapturedImage)
        {
            string query = @"INSERT INTO Members (MemberName, LeftThumbTemplate, LeftIndexTemplate, LeftMiddleTemplate, 
                             LeftRingTemplate, LeftPinkyTemplate, RightThumbTemplate, RightIndexTemplate, 
                             RightMiddleTemplate, RightRingTemplate, RightPinkyTemplate, LastFingerImage) 
                             VALUES (@Name, @LThumb, @LIndex, @LMiddle, @LRing, @LPinky, 
                             @RThumb, @RIndex, @RMiddle, @RRing, @RPinky, @Image)";
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                
                // Add all 10 finger templates
                cmd.Parameters.Add("@LThumb", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Left_Thumb") ? fingerTemplates["Left_Thumb"] : DBNull.Value;
                cmd.Parameters.Add("@LIndex", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Left_Index") ? fingerTemplates["Left_Index"] : DBNull.Value;
                cmd.Parameters.Add("@LMiddle", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Left_Middle") ? fingerTemplates["Left_Middle"] : DBNull.Value;
                cmd.Parameters.Add("@LRing", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Left_Ring") ? fingerTemplates["Left_Ring"] : DBNull.Value;
                cmd.Parameters.Add("@LPinky", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Left_Pinky") ? fingerTemplates["Left_Pinky"] : DBNull.Value;
                
                cmd.Parameters.Add("@RThumb", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Right_Thumb") ? fingerTemplates["Right_Thumb"] : DBNull.Value;
                cmd.Parameters.Add("@RIndex", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Right_Index") ? fingerTemplates["Right_Index"] : DBNull.Value;
                cmd.Parameters.Add("@RMiddle", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Right_Middle") ? fingerTemplates["Right_Middle"] : DBNull.Value;
                cmd.Parameters.Add("@RRing", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Right_Ring") ? fingerTemplates["Right_Ring"] : DBNull.Value;
                cmd.Parameters.Add("@RPinky", SqlDbType.VarBinary).Value = 
                    fingerTemplates.ContainsKey("Right_Pinky") ? fingerTemplates["Right_Pinky"] : DBNull.Value;
                
                cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = (object)lastCapturedImage ?? DBNull.Value;
                
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Save Error: " + ex.Message);
                    return false;
                }
            }
        }

        private int IdentifyFromSqlDatabase(byte[] targetTemplate)
        {
            zkfp2.DBClear(mDBHandle);

            string query = @"SELECT MemberID, LeftThumbTemplate, LeftIndexTemplate, LeftMiddleTemplate, 
                             LeftRingTemplate, LeftPinkyTemplate, RightThumbTemplate, RightIndexTemplate, 
                             RightMiddleTemplate, RightRingTemplate, RightPinkyTemplate FROM Members";
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int dbId = reader.GetInt32(0);
                            
                            // Add all available finger templates to the database
                            for (int i = 1; i <= 10; i++)
                            {
                                if (reader[i] != DBNull.Value)
                                {
                                    byte[] dbTemplate = (byte[])reader[i];
                                    zkfp2.DBAdd(mDBHandle, dbId, dbTemplate);
                                }
                            }
                        }
                    }

                    int matchedMemberId = 0;
                    int score = 0;
                    int matchResult = zkfp2.DBIdentify(mDBHandle, targetTemplate, ref matchedMemberId, ref score);

                    if (matchResult == zkfp.ZKFP_ERR_OK)
                    {
                        return matchedMemberId;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Reader Read Matching Error: " + ex.Message);
                }
            }
            return -1;
        }

        private void FetchAndDisplayUserProfile(int memberId)
        {
            string query = "SELECT MemberName, LastFingerImage FROM Members WHERE MemberID = @ID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID", memberId);
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string foundName = reader["MemberName"].ToString();
                            txtUserName.Text = foundName;

                            if (reader["LastFingerImage"] != DBNull.Value)
                            {
                                byte[] dbImageBytes = (byte[])reader["LastFingerImage"];
                                using (MemoryStream dbImageMs = new MemoryStream(dbImageBytes))
                                {
                                    using (Image img = Image.FromStream(dbImageMs))
                                    {
                                        if (picDBImg != null) picDBImg.Image = new Bitmap(img);
                                    }
                                }
                            }
                            else
                            {
                                if (picDBImg != null) picDBImg.Image = null;
                            }

                            reader.Close();

                            // Record attendance and dynamically update the visual dashboard
                            LogAttendance(memberId, foundName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Profile visual sync execution error: " + ex.Message);
                }
            }
        }

        private void LogAttendance(int memberId, string name)
        {
            string logQuery = "INSERT INTO AttendanceLogs (MemberID, MemberName, CheckInTime) VALUES (@ID, @Name, @Time)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(logQuery, conn))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = memberId;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@Time", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    textRes.Text = $"✓ Match Confirmed: {name} (ID: {memberId})\r\nLogged at {DateTime.Now:HH:mm:ss}";

                    LoadTodayAttendance();
                }
                catch (Exception ex)
                {
                    textRes.Text = $"Verified {name}, but attendance write transaction rolled back.";
                }
            }
        }

        private void LoadTodayAttendance()
        {
            if (dgvAttendance == null) return;

            string query = @"SELECT TOP 50 LogID AS [Log #], MemberID AS [User ID], 
                             MemberName AS [Full Name], CheckInTime AS [Time Stamp] 
                             FROM AttendanceLogs 
                             WHERE CAST(CheckInTime AS DATE) = CAST(GETDATE() AS DATE) 
                             ORDER BY CheckInTime DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                try
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dgvAttendance.InvokeRequired)
                    {
                        dgvAttendance.Invoke(new Action(() => dgvAttendance.DataSource = dt));
                    }
                    else
                    {
                        dgvAttendance.DataSource = dt;
                    }
                }
                catch (Exception)
                {
                    // Fail quietly to preserve application runtime
                }
            }
        }
    }
}
