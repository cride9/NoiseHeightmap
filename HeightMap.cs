using SimplexNoise;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace HeightMap {
    public partial class HeightMap : Form {

        BufferedGraphics bufferedGraphics;

        public HeightMap( ) {
            InitializeComponent( );
            SetStyle(
                ControlStyles.Opaque |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true );
        }

        // Smaller value generates bigger "blobs"
        float zoomLevel = 0.005f;
        (int x, int y) offset = (0, 0);
        List<int> levelQueue = new( );

        private void OnPaint( object sender, PaintEventArgs e ) {

            // Render once, to not rape your CPU
            BufferMap( e );

            // Magic happened
            bufferedGraphics.Render( e.Graphics );
        }

        private void BufferMap( PaintEventArgs e ) {

            // Make a brush from the forecolor
            SolidBrush forecolorBrush = new SolidBrush( ForeColor );

            // Get primary screen size
            var bounds = Screen.PrimaryScreen!.Bounds;

            // 8) do NOT go out of the render size
            MaximumSize = new Size( bounds.Width, bounds.Height );

            // Allocate graphics buffer
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate( e.Graphics, new Rectangle( 0, 0, bounds.Width, bounds.Height ) );
            Graphics bg = bufferedGraphics.Graphics;
            bg.SmoothingMode = SmoothingMode.HighSpeed;
            bg.CompositingQuality = CompositingQuality.HighSpeed;
            bg.InterpolationMode = InterpolationMode.Low;

            // Decleare the new height map
            float[ , ] heightMap = new float[ bounds.Width, bounds.Height ];
            for ( int x = 0; x < bounds.Width; x++ )
                for ( int y = 0; y < bounds.Height; y++ )
                    heightMap[ x, y ] = Noise.CalcPixel2D( x + offset.x, y + offset.y, zoomLevel );

            // Do magic! ( draw each height layer by layer )

            // Levels to draw
            int[ ] levels = levelQueue.ToArray( );

            // Iterate through the levels
            for ( int i = 0; i < levels.Length; i++ ) {

                // Iterate through the screen size (X,Y)
                for ( int x = 1; x < bounds.Width; x++ ) {
                    for ( int y = 1; y < bounds.Height; y++ ) {

                        // Check if the level of that generation is good or not
                        if ( ( heightMap[ x, y ] >= levels[ i ] && heightMap[ x - 1, y ] < levels[ i ] ) ||
                            ( heightMap[ x, y ] < levels[ i ] && heightMap[ x - 1, y ] >= levels[ i ] ) ||
                            ( heightMap[ x, y ] >= levels[ i ] && heightMap[ x, y - 1 ] < levels[ i ] ) ||
                            ( heightMap[ x, y ] < levels[ i ] && heightMap[ x, y - 1 ] >= levels[ i ] ) ) {

                            // Draw a pixel :nerd:
                            bg.FillRectangle( new SolidBrush( Color.FromArgb( levels[ i ], 255, 255, 255 ) ), x, y, 1, 1 );
                        }
                    }
                }
            }

        }

        private DateTime backupTime;
        private Random rnd = new( );
        private int rndBackup = 0;
        private void RefreshTimer( object sender, EventArgs e ) {

            Invalidate( );
            
            if ( DateTime.Now.Subtract(backupTime).Seconds > rndBackup ) {

                backupTime = DateTime.Now;
                levelQueue.Add( 20 );
                rndBackup = rnd.Next( 2, 7 );
            }

            for ( int i = 0; i < levelQueue.Count; i++ )
                levelQueue[ i ]++;

            offset.x++;
            levelQueue.RemoveAll( i => i > 255 );
        }
    }
}
