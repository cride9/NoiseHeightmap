namespace HeightMap {
    partial class HeightMap {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( ) {
            components = new System.ComponentModel.Container( );
            refreshTimer = new System.Windows.Forms.Timer( components );
            SuspendLayout( );
            // 
            // refreshTimer
            // 
            refreshTimer.Enabled = true;
            refreshTimer.Interval = 16;
            refreshTimer.Tick +=  RefreshTimer ;
            // 
            // HeightMap
            // 
            AutoScaleDimensions = new SizeF( 7F, 15F );
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(     7,     0,     13 );
            ClientSize = new Size( 800, 450 );
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(     44,     44,     66 );
            Name = "HeightMap";
            Text = "Form1";
            Paint +=  OnPaint ;
            ResumeLayout( false );
        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
    }
}
