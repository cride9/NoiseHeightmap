using SimplexNoise;
using System.Drawing.Drawing2D;

namespace HeightMap {
    public partial class HeightMap : Form {

        BufferedGraphics bufferedGraphics;

        public HeightMap( ) =>
            InitializeComponent( );
        
        float zoomLevel = 0.01f;
        bool renderOnce = true;

        private void OnPaint( object sender, PaintEventArgs e ) {

            // Render once, to not rape your CPU
            if ( renderOnce )
                BufferMap( e );
            
            // Magic happened
            bufferedGraphics.Render( e.Graphics );
        }

        private void BufferMap( PaintEventArgs e ) {

            // Make a brush from the forecolor
            SolidBrush forecolorBrush = new SolidBrush( ForeColor );

            // Levels to draw
            int[ ] levels = { 60, 120, 180, 240 };

            // Get primary screen size
            var bounds = Screen.PrimaryScreen!.Bounds;

            // 8) do NOT go out of the render size
            MaximumSize = new Size( bounds.Width, bounds.Height );

            // Allocate graphics buffer
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate( e.Graphics, new Rectangle( 0, 0, 1920, 1080 ) );
            Graphics bg = bufferedGraphics.Graphics;
            bg.SmoothingMode = SmoothingMode.HighQuality;

            // Decleare the new height map
            float[ , ] heightMap = new float[ bounds.Width, bounds.Height ];
            for ( int x = 0; x < bounds.Width; x++ )
                for ( int y = 0; y < bounds.Height; y++ )
                    heightMap[ x, y ] = Noise.CalcPixel2D( x, y, zoomLevel );

            // Do magic! ( draw each height layer by layer )

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
                            bg.FillRectangle( forecolorBrush, x, y, 1, 1 );
                        }
                    }
                }
            }

            // OK!
            renderOnce = false;
        }
    }
}
