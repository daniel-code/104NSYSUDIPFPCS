using DIPHua3;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace SimpleDeblur {
    public partial class Index : Form {

        public Index() {
            InitializeComponent();
        }

        private void Index_Load( object sender, System.EventArgs e ) {
            CheckForIllegalCrossThreadCalls = false;
        }
        
        private void Index_FormClosing( object sender, FormClosingEventArgs e ) {
            StopProcess();
            if( OImg != null ) {
                OImg.Dispose();
                OImg = null;
            }
            if(RImg != null ) {
                RImg.Dispose();
                RImg = null;
            }
        }

        Bitmap OImg = null;
        Bitmap RImg = null;

        GrayImg OrQImg = null;
        GrayImg QImg = null;

        BGRImg OrSImg = null;
        BGRImg SImg = null;

        SimpleMotionKernel K = null;
        
        bool XProcessing = false;

        Thread QThread = null;

        Thread SThread = null;

        private void MS_File_Open_Click( object sender, System.EventArgs e ) {
            OFD.FileName = "";
            SS_LBStatus.Text = "圖片開啟中";
            if( OFD.ShowDialog() == DialogResult.OK ) {

                StopProcess();

                this.Cursor = Cursors.WaitCursor;

                string filepath = OFD.FileName;

                OImg = new Bitmap( filepath );
                RImg = new Bitmap( filepath );
                OrQImg = GrayImg.From( filepath );
                OrSImg = BGRImg.From( filepath );

                PB_Result.Image = OrQImg.ToImage();
                PB_Result.Height = FLP_Result.Height;
                PB_Result.Width = OrQImg.Width * FLP_Result.Height / OrQImg.Height;

                QImg = OrQImg.Extend();
                SImg = OrSImg.Extend();

                K = new SimpleMotionKernel( QImg.Width, QImg.Height, 53 );

                QImg.FFT2();
                SImg.FFT2();

                QThread = new Thread( QProcess );
                QThread.Start();

                this.Cursor = Cursors.Default;
            }
        }

        private void MS_File_Save_Click( object sender, System.EventArgs e ) {
            SFD.FileName = "";
            SS_LBStatus.Text = "圖片儲存中";
            if( RImg != null && !XProcessing && SFD.ShowDialog() == DialogResult.OK ) {
                string filepath = SFD.FileName;
                RImg.Save( filepath, ImageFormat.Tiff );
                SS_LBStatus.Text = "成功儲存圖片";
            }
        }

        private void TrB_Length_Scroll( object sender, System.EventArgs e ) {
            if( OrQImg != null ) {
                StopProcess();
                QThread = new Thread( QProcess );
                QThread.Start();
            }
            LB_Length.Text = TrB_Length.Value.ToString();
        }

        private void TrB_Lambda_Scroll( object sender, System.EventArgs e ) {
            if( OrQImg != null ) {
                StopProcess();
                QThread = new Thread( QProcess );
                QThread.Start();
            }
            LB_Lambda.Text = TrB_Lambda.Value.ToString();
        }

        private void Index_KeyDown( object sender, KeyEventArgs e ) {
            if( !XProcessing && e.KeyCode == Keys.F1 ) {
                PB_Result.Image = OImg;
            }
        }

        private void Index_KeyUp( object sender, KeyEventArgs e ) {
            if( !XProcessing && e.KeyCode == Keys.F1 ) {
                PB_Result.Image = RImg;
            }
        }

        private void QProcess() {
            SS_LBStatus.Text = "產生預覽圖片中";
            XProcessing = true;
            SS_PB.Value = 0;
            SS_PB.Visible = true;
            int length = TrB_Length.Value;
            int lambda = TrB_Lambda.Value;
            SS_PB.Value = 10;
            K.Reset( length );
            K.FFT2();
            PB_Kernel.Image = K.ToPreviewImage();
            SS_PB.Value = 20;
            SS_PB.Value = 30;
            GrayImg ReQImg = QImg.InsideSimpleMotionDeblur( K, lambda );
            SS_PB.Value = 40;
            ReQImg.BFFTShift();
            SS_PB.Value = 50;
            ReQImg.IFFT2();
            SS_PB.Value = 60;
            ReQImg = ReQImg.UnExtend( OrQImg.Width, OrQImg.Height );
            SS_PB.Value = 70;
            if( PB_Result.Image != null ) {
                PB_Result.Image.Dispose();
                PB_Result.Image = null;
            }
            PB_Result.Image = ReQImg.ToImage();
            SS_PB.Value = 100;

            SS_LBStatus.Text = "產生完整圖片中";
            if( SThread != null ) {
                SThread.Abort();
                SThread = null;
            }
            SThread = new Thread( SProcess );
            SThread.Start();
            GC.Collect();
        }

        private void SProcess() {
            SS_PB.Value = 0;
            int length = TrB_Length.Value;
            SS_PB.Value = 10;
            int lambda = TrB_Lambda.Value;
            SS_PB.Value = 15;
            BGRImg ReSImg = SImg.InsideSimpleMotionDeblur( K, lambda );
            SS_PB.Value = 70;
            ReSImg.BFFTShift();
            SS_PB.Value = 80;
            ReSImg.IFFT2();
            SS_PB.Value = 90;
            ReSImg = ReSImg.UnExtend( OrQImg.Width, OrQImg.Height );
            if( PB_Result.Image != null ) {
                PB_Result.Image.Dispose();
                PB_Result.Image = null;
            }
            if( RImg != null ) {
                RImg.Dispose();
                RImg = null;
            }
            RImg = ReSImg.ToImage();
            PB_Result.Image = RImg;
            SS_LBStatus.Text = "完整圖片";
            SS_PB.Value = 100;
            SS_PB.Visible = false;
            XProcessing = false;
            GC.Collect();
        }

        private void StopProcess() {
            if( SThread != null ) {
                SThread.Abort();
                SThread = null;
            }
            if( QThread != null ) {
                QThread.Abort();
                QThread = null;
            }
            GC.Collect();
        }

        private void Index_Resize( object sender, System.EventArgs e ) {
            if( OrQImg != null ) {
                PB_Result.Height = FLP_Result.Height;
                PB_Result.Width = OrQImg.Width * FLP_Result.Height / OrQImg.Height;
            }
        }
    }
}
