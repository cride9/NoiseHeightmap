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

            bounds = Screen.PrimaryScreen!.Bounds;
            heightMap = new float[ bounds.Width, bounds.Height ];

            // 8) do NOT go out of the render size
            MaximumSize = new Size( bounds.Width, bounds.Height );
        }
        // Smaller value generates bigger "blobs"
        private const float zoomLevel = 0.005f;

        // Can 'move' around
        private (int x, int y) offset = (0, 0);

        // Handles level drawing
        private List<int> levelQueue = new( );

        // optimization
        private SolidBrush brush = new SolidBrush( Color.White );
        private Rectangle bounds;
        private float[ , ] heightMap;

        private void OnPaint( object sender, PaintEventArgs e ) {

            // Render!!
            BufferMap( e );
        }

        private void BufferMap( PaintEventArgs e ) {

            // Make a brush from the forecolor
            var bg = e.Graphics;

            // Decleare the new height map
            for ( int x = 0; x < bounds.Width; x++ )
                for ( int y = 0; y < bounds.Height; y++ )
                    heightMap[ x, y ] = Noise.CalcPixel2D( x + offset.x, y + offset.y, zoomLevel );

            // Do magic! ( draw each height layer by layer )

            // Levels to draw
            foreach ( var level in levelQueue ) {

                // Iterate through the screen size (X,Y)
                for ( int x = 1; x < bounds.Width; x++ ) {
                    for ( int y = 1; y < bounds.Height; y++ ) {

                        // Check if the level of that generation is good or not
                        if ( ( heightMap[ x, y ] >= level && heightMap[ x - 1, y ] < level ) ||
                            ( heightMap[ x, y ] < level && heightMap[ x - 1, y ] >= level ) ||
                            ( heightMap[ x, y ] >= level && heightMap[ x, y - 1 ] < level ) ||
                            ( heightMap[ x, y ] < level && heightMap[ x, y - 1 ] >= level ) ) {

                            // Draw a pixel :nerd:
                            brush.Color = Color.FromArgb( level, 255, 255, 255 );
                            bg.FillRectangle( brush, x, y, 1, 1 );
                        }
                    }
                }
            }
        }

        private DateTime backupTime;
        private Random rnd = new( );
        private int rndBackup = 0;
        private void RefreshTimer( object sender, EventArgs e ) {

            if ( DateTime.Now.Subtract(backupTime).Seconds > rndBackup ) {

                backupTime = DateTime.Now;
                levelQueue.Add( 20 );
                rndBackup = rnd.Next( 1, 4 );
            }

            for ( int i = 0; i < levelQueue.Count; i++ )
                levelQueue[ i ]++;

            offset.x++;
            levelQueue.RemoveAll( i => i > 255 );
            Invalidate( );
        }
    }
}
