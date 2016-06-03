using MathNet.Numerics;
using MathNet.Numerics.Transformations;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DIPHua3 {
    public enum BGRModel {
        B = 1, G = 2, R = 4
    }
    public unsafe class GrayImg {
        public GrayImg( int width, int height ) {
            _Width = width;
            _Height = height;
            _Gray = new Complex[ width * height ];
        }
        private int _Width, _Height;
        private Complex[] _Gray;
        public int Width {
            get { return _Width; }
        }
        public int Height {
            get { return _Height; }
        }
        public Complex[] Gray {
            get { return _Gray; }
        }
        public Complex this[ int x, int y ] {
            get {
                return _Gray[ x + y * _Width ];
            }
            set {
                _Gray[ x + y * _Width ] = value;
            }
        }
        public Complex this[ int i ] {
            get {
                return _Gray[ i ];
            }
            set {
                _Gray[ i ] = value;
            }
        }
        public GrayImg SimpleMotionDeblur( double length, double lambda ) {
            GrayImg oext = Extend();
            SimpleMotionKernel smk = new SimpleMotionKernel( oext.Width, oext.Height, length );
            GrayImg rext = new GrayImg( oext.Width, oext.Height );
            oext.FFT2();
            smk.FFT2();
            int i, scale = oext.Width * oext.Height;
            double d, s;
            for( i = 0 ; i < scale ; ++i ) {
                smk[ i ] += 0.0000001;
                d = smk[ i ].ModulusSquared;
                s = d / ( d + lambda );
                rext[ i ] = ( oext[ i ] / smk[ i ] ) * s;
            }
            rext.BFFTShift();
            rext.IFFT2();
            return rext.UnExtend( _Width, _Height );
        }
        public GrayImg InsideSimpleMotionDeblur( SimpleMotionKernel smk, double lambda ) {
            GrayImg rext = new GrayImg( _Width, _Height );
            int i, scale = _Width * _Height;
            double d, s;
            for( i = 0 ; i < scale ; ++i ) {
                smk[ i ] += 0.0000001;
                d = smk[ i ].ModulusSquared;
                s = d / ( d + lambda );
                rext[ i ] = ( this[ i ] / smk[ i ] ) * s;
            }
            return rext;
        }
        public GrayImg Extend() {
            int ewidth = (int) Math.Pow( 2, Funct.Log2( _Width ) ),
                eheight = (int) Math.Pow( 2, Funct.Log2( _Height ) );
            GrayImg result = new GrayImg( ewidth, eheight );
            int startx = ( ewidth - _Width ) / 2,
                starty = ( eheight - _Height ) / 2;
            int endx = startx + _Width,
                endy = starty + _Height;
            int ttx, tty;
            int y, x;
            for( y = 0 ; y < eheight ; ++y ) {
                for( x = 0 ; x < ewidth ; ++x ) {
                    ttx = x;
                    tty = y;
                    while( ttx - startx < 0 ) {
                        ttx += Math.Abs( ttx - startx ) * 2 - 1;
                    }
                    while( ttx - startx >= _Width ) {
                        ttx -= Math.Abs( ttx - startx - _Width ) * 2 + 1;
                    }
                    while( tty - starty < 0 ) {
                        tty += Math.Abs( tty - starty ) * 2 - 1;
                    }
                    while( tty - starty >= _Height ) {
                        tty -= Math.Abs( tty - starty - _Height ) * 2 + 1;
                    }
                    result[ x, y ] = Complex.FromRealImaginary( this[ ttx - startx, tty - starty ].Real, this[ ttx - startx, tty - starty ].Imag );
                }
            }
            return result;
        }
        public GrayImg UnExtend( int owidth, int oheight ) {
            GrayImg result = new GrayImg( owidth, oheight );
            int startx = ( _Width - owidth ) / 2, starty = ( _Height - oheight ) / 2;
            int endx = startx + owidth, endy = starty + oheight;
            int y, x;
            for( y = 0 ; y < _Height ; ++y ) {
                for( x = 0 ; x < _Width ; ++x ) {
                    if( x >= startx && y >= starty && x < endx && y < endy ) {
                        result[ x - startx, y - starty ] = this[ x, y ];
                    }
                }
            }
            return result;
        }
        public void FFT2() {
            Funct.CFTMatlab.TransformForward( _Gray, new int[] { _Height, _Width } );
        }
        public void IFFT2() {
            Funct.CFTMatlab.TransformBackward( _Gray, new int[] { _Height, _Width } );
        }
        public void BFFTShift() {
            int x, y, t;
            for( t = 0 ; t < 3 ; ++t ) {
                for( y = 0 ; y < _Height ; ++y ) {
                    for( x = 0 ; x < _Width ; ++x ) {
                        if( ( ( x + y ) & 0x1 ) != 0 ) {
                            _Gray[ x + y * _Width ].Real *= -1;
                            _Gray[ x + y * _Width ].Imag *= -1;
                        }
                    }
                }
            }
        }
        static public GrayImg From( Bitmap source ) {
            GrayImg result = new GrayImg( source.Width, source.Height );
            BitmapData sourcedata = source.LockBits( new Rectangle( 0, 0, source.Width, source.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
            byte* sourcepointer = (byte*) sourcedata.Scan0.ToPointer();
            int i;
            int scale = source.Width * source.Height;
            byte g;
            for( i = 0 ; i < scale ; ++i ) {
                g = (byte) ( ( sourcepointer[ 0 ] + sourcepointer[ 1 ] + sourcepointer[ 2 ] ) / 3.0D );
                result[ i ] = Complex.FromRealImaginary( g, 0 );
                sourcepointer += 4;
            }
            source.UnlockBits( sourcedata );
            return result;
        }
        static public GrayImg From( string path ) {
            return From( new Bitmap( path ) );
        }
        public GrayImg Copy() {
            GrayImg result = new GrayImg( _Width, _Height );
            int i, scale = _Width * _Height;
            for(i=0 ;i<scale ; ++i ) {
                result[ i ] = this[ i ];
            }
            return result;
        }
        public Bitmap ToImage( bool Freq = false ) {
            int scale = _Width * _Height;
            int i, t;
            double max = double.MinValue;
            double min = double.MaxValue;
            double[] tmpdata = new double[ scale ];
            for( i = 0 ; i < scale ; ++i ) {
                tmpdata[ i ] = this[ i ].Modulus;
                if( Freq ) {
                    tmpdata[ i ] = Math.Log10( tmpdata[ i ] );
                }
                max = max < tmpdata[ i ] ? tmpdata[ i ] : max;
                min = min > tmpdata[ i ] ? tmpdata[ i ] : min;
            }
            double s = 255 / ( max - min );
            Bitmap result = new Bitmap( _Width, _Height );
            BitmapData resultdata = result.LockBits( new Rectangle( 0, 0, result.Width, result.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
            byte* resultpointer = (byte*) resultdata.Scan0.ToPointer();
            for( i = 0 ; i < scale ; ++i ) {
                resultpointer[ 0 ] = resultpointer[ 1 ] = resultpointer[ 2 ] = Funct.D2B( ( tmpdata[ i ] - min ) * s );
                resultpointer[ 3 ] = 255;
                resultpointer+=4;
            }
            result.UnlockBits( resultdata );
            return result;
        }
    }
    public unsafe class BGRImg {
        public BGRImg( int width, int height ) {
            _Width = width;
            _Height = height;
            int scale = width * height;
            _BGR = new Complex[ 3 ][];
            for( int i = 0 ; i < 3 ; ++i ) {
                _BGR[ i ] = new Complex[ scale ];
            }
        }
        private int _Width, _Height;
        private Complex[][] _BGR;
        public int Width {
            get { return _Width; }
        }
        public int Height {
            get { return _Height; }
        }
        public Complex[] B {
            get { return _BGR[ 0 ]; }
        }
        public Complex[] G {
            get { return _BGR[ 1 ]; }
        }
        public Complex[] R {
            get { return _BGR[ 2 ]; }
        }
        public Complex this[ int model, int x, int y ] {
            get {
                return _BGR[ model ][ x + y * _Width ];
            }
            set {
                _BGR[ model ][ x + y * _Width ] = value;
            }
        }
        public Complex this[ int model, int i ] {
            get {
                return _BGR[ model ][ i ];
            }
            set {
                _BGR[ model ][ i ] = value;
            }
        }
        public BGRImg SimpleMotionBlur( double length ) {
            BGRImg oext = Extend();
            SimpleMotionKernel smk = new SimpleMotionKernel( oext.Width, oext.Height, length );
            BGRImg rext = new BGRImg( oext.Width, oext.Height );
            oext.FFT2();
            smk.FFT2();
            int x, y, t;
            for( y = 0 ; y < oext.Height ; ++y ) {
                for( x = 0 ; x < oext.Width ; ++x ) {
                    for( t = 0 ; t < 3 ; ++t ) {
                        rext[ t, x, y ] = oext[ t, x, y ] * smk[ x, y ];
                    }
                }
            }
            rext.BFFTShift();
            rext.IFFT2();
            return rext.UnExtend( _Width, _Height );
        }
        public BGRImg SimpleMotionDeblur( double length, double lambda ) {
            BGRImg oext = Extend();
            SimpleMotionKernel smk = new SimpleMotionKernel( oext.Width, oext.Height, length );
            BGRImg rext = new BGRImg( oext.Width, oext.Height );
            oext.FFT2();
            smk.FFT2();
            int i, t, scale = oext.Width * oext.Height;
            double d, s;
            for( t = 0 ; t < 3 ; ++t ) {
                for( i = 0 ; i < scale ; ++i ) {
                    smk[ i ] += 0.0000001;
                    d = smk[ i ].ModulusSquared;
                    s = d / ( d + lambda );
                    rext[ t, i ] = ( oext[ t, i ] / smk[ i ] ) * s;
                }
            }
            rext.BFFTShift();
            rext.IFFT2();
            return rext.UnExtend( _Width, _Height );
        }
        public BGRImg InsideSimpleMotionDeblur( SimpleMotionKernel smk, double lambda ) {
            BGRImg rext = new BGRImg( _Width, _Height );
            int i, t, scale = _Width * _Height;
            double d, s;
            for( t = 0 ; t < 3 ; ++t ) {
                for( i = 0 ; i < scale ; ++i ) {
                    smk[ i ] += 0.0000001;
                    d = smk[ i ].ModulusSquared;
                    s = d / ( d + lambda );
                    rext[ t, i ] = ( this[ t, i ] / smk[ i ] ) * s;
                }
            }
            return rext;
        }
        public BGRImg Extend() {
            int ewidth = (int) Math.Pow( 2, Funct.Log2( _Width ) ), 
                eheight = (int) Math.Pow( 2, Funct.Log2( _Height ) );
            BGRImg result = new BGRImg( ewidth, eheight );
            int startx = ( ewidth - _Width ) / 2, 
                starty = ( eheight - _Height ) / 2;
            int endx = startx + _Width, 
                endy = starty + _Height;
            int ttx, tty;
            int y, x, t;
            for( y = 0 ; y < eheight ; ++y ) {
                for( x = 0 ; x < ewidth ; ++x ) {
                    ttx = x;
                    tty = y;

                    while( ttx - startx < 0 ) {
                        ttx += Math.Abs( ttx - startx ) * 2 - 1;
                    }
                    while( ttx - startx >= _Width ) {
                        ttx -= Math.Abs( ttx - startx - _Width ) * 2 + 1;
                    }

                    while( tty - starty < 0 ) {
                        tty += Math.Abs( tty - starty ) * 2 - 1;
                    }
                    while( tty - starty >= _Height ) {
                        tty -= Math.Abs( tty - starty - _Height ) * 2 + 1;
                    }

                    for( t = 0 ; t < 3 ; ++t ) {
                        result[ t, x, y ] = Complex.FromRealImaginary( this[ t, ttx - startx, tty - starty ].Real, this[ t, ttx - startx, tty - starty ].Imag );
                    }
                }
            }
            return result;
        }
        public BGRImg UnExtend( int owidth, int oheight ) {
            BGRImg result = new BGRImg( owidth, oheight );
            int startx = ( _Width - owidth ) / 2, starty = ( _Height - oheight ) / 2;
            int endx = startx + owidth, endy = starty + oheight;
            int y, x, t;
            for( y = 0 ; y < _Height ; ++y ) {
                for( x = 0 ; x < _Width ; ++x ) {
                    if( x >= startx && y >= starty && x < endx && y < endy ) {
                        for( t = 0 ; t < 3 ; ++t ) {
                            result[ t, x - startx, y - starty ] = this[ t, x, y ];
                        }
                    }
                }
            }
            return result;
        }
        public Bitmap ToImage( bool Freq = false ) {
            int scale = _Width * _Height;
            int i, t;
            double max = double.MinValue;
            double min = double.MaxValue;
            double[,] tmpdata = new double[ 3, scale ];
            for( t = 0 ; t < 3 ; ++t ) {
                for( i = 0 ; i < scale ; ++i ) {
                    tmpdata[ t, i ] = this[ t, i ].Modulus;
                    if( Freq ) {
                        tmpdata[ t, i ] = Math.Log10( tmpdata[ t, i ] );
                    }
                    max = max < tmpdata[ t, i ] ? tmpdata[ t, i ] : max;
                    min = min > tmpdata[ t, i ] ? tmpdata[ t, i ] : min;
                }
            }
            double s = 255 / ( max - min );
            Bitmap result = new Bitmap( _Width, _Height );
            BitmapData resultdata = result.LockBits( new Rectangle( 0, 0, result.Width, result.Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            byte* resultpointer = (byte*) resultdata.Scan0.ToPointer();
            for( i = 0 ; i < scale ; ++i ) {
                for( t = 0 ; t < 3 ; ++t ) {
                    resultpointer[ 0 ] = Funct.D2B( ( tmpdata[ t, i ] - min ) * s );
                    ++resultpointer;
                }
                resultpointer[ 0 ] = 255;
                ++resultpointer;
            }
            result.UnlockBits( resultdata );
            return result;
        }
        static public BGRImg From( Bitmap source, bool grayscale = false ) {
            BGRImg result = new BGRImg( source.Width, source.Height );
            BitmapData sourcedata = source.LockBits( new Rectangle( 0, 0, source.Width, source.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
            byte* sourcepointer = (byte*) sourcedata.Scan0.ToPointer();
            int i, t;
            int scale = source.Width * source.Height;
            byte g;
            for( i = 0 ; i < scale ; ++i ) {
                if( !grayscale ) {
                    for( t = 0 ; t < 3 ; ++t ) {
                        result[ t, i ] = Complex.FromRealImaginary( sourcepointer[ 0 ], 0 );
                        ++sourcepointer;
                    }
                    ++sourcepointer;
                } else {
                    g = (byte) ( ( sourcepointer[ 0 ] + sourcepointer[ 1 ] + sourcepointer[ 2 ] ) / 3.0D );
                    result[ 0, i ] = result[ 1, i ] = result[ 2, i ] = Complex.FromRealImaginary( g, 0 );
                    sourcepointer += 4;
                }
            }
            source.UnlockBits( sourcedata );
            return result;
        }
        static public BGRImg From( string path, bool grayscale = false ) {
            return From( new Bitmap( path ), grayscale );
        }
        public void FFT2() {
            Funct.CFTMatlab.TransformForward( _BGR[ 0 ], new int[] { _Height, _Width } );
            Funct.CFTMatlab.TransformForward( _BGR[ 1 ], new int[] { _Height, _Width } );
            Funct.CFTMatlab.TransformForward( _BGR[ 2 ], new int[] { _Height, _Width } );
        }
        public void IFFT2() {
            Funct.CFTMatlab.TransformBackward( _BGR[ 0 ], new int[] { _Height, _Width } );
            Funct.CFTMatlab.TransformBackward( _BGR[ 1 ], new int[] { _Height, _Width } );
            Funct.CFTMatlab.TransformBackward( _BGR[ 2 ], new int[] { _Height, _Width } );
        }
        public void BFFTShift() {
            int x, y, t;
            for( t = 0 ; t < 3 ; ++t ) {
                for( y = 0 ; y < _Height ; ++y ) {
                    for( x = 0 ; x < _Width ; ++x ) {
                        if( ( ( x + y ) & 0x1 ) != 0 ) {
                            _BGR[ t ][ x + y * _Width ].Real *= -1;
                            _BGR[ t ][ x + y * _Width ].Imag *= -1;
                        }
                    }
                }
            }
        }
    }
    public unsafe class SimpleMotionKernel {
        public SimpleMotionKernel(int width,int height, double length ) {
            _Width = width;
            _Height = height;
            _Length = length;
            CreateKernel();
        }
        private int _Width, _Height;
        private double _Length;
        private Complex[] _Kernel;
        public int Width {
            get { return _Width; }
        }
        public int Height {
            get { return _Height; }
        }
        public double Length {
            get { return _Length; }
        }
        public Complex[] Kernel {
            get {
                return _Kernel;
            }
        }
        public Complex this[ int x, int y ] {
            get {
                return _Kernel[ x + y * Width ];
            }
            set {
                _Kernel[ x + y * Width ] = value;
            }
        }
        public Complex this[ int i ] {
            get {
                return _Kernel[ i ];
            }
            set {
                _Kernel[ i ] = value;
            }
        }
        private void CreateKernel() {
            int scale = _Width * _Height;
            _Kernel = new Complex[ scale ];
            int x, y;
            int cx = (int) ( ( _Width - _Length ) / 2 ), cy = _Height / 2;
            for( y = 0 ; y < _Height ; ++y ) {
                for( x = 0 ; x < _Width ; ++x ) {
                    if( y == cy && cx <= x && x < cx + _Length ) {
                        this[ x, y ] = Complex.FromRealImaginary( 1, 0 );
                    } else {
                        this[ x, y ] = Complex.FromRealImaginary( 0, 0 );
                    }
                }
            }
        }
        public void ChangeLength( double length ) {
            double olength = _Length;
            _Length = length;
            int x;
            int ox = (int) ( ( _Width - olength ) / 2 ),
                eox = (int) ( ox + olength ),
                cx = (int) ( ( _Width - length ) / 2 ),
                ecx = (int) ( cx + length ),
                y = _Height / 2;
            int bx = ox > cx ? cx : ox,
                ebx = eox < ecx ? ecx : eox;
            for( x = bx ; x < ebx ; ++x ) {
                if( cx <= x && x < ecx ) {
                    _Kernel[ x ].Real = 1;
                }
            }
        }
        public void Reset( double length ) {
            _Length = length;
            int x, y, t;
            int cx = (int) ( ( _Width - _Length ) / 2 ), cy = _Height / 2;
            for( y = 0 ; y < _Height ; ++y ) {
                for( x = 0 ; x < _Width ; ++x ) {
                    t = x + y * _Width;
                    if( y == cy && cx <= x && x < cx + _Length ) {
                        _Kernel[ t ].Real = 1;
                        _Kernel[ t ].Imag = 0;
                    } else {
                        _Kernel[ t ].Real = 0;
                        _Kernel[ t ].Imag = 0;
                    }
                }
            }
        }
        public void FFT2() {
            Funct.CFTMatlab.TransformForward( _Kernel, new int[] { _Height, _Width } );
        }
        public Bitmap ToImage( bool Freq = false ) {
            int scale = _Width * _Height;
            int i, t;
            double max = double.MinValue;
            double min = double.MaxValue;
            double[] tmpdata = new double[ scale ];
            for( i = 0 ; i < scale ; ++i ) {
                tmpdata[ i ] = this[ i ].Modulus;
                if( Freq ) {
                    tmpdata[ i ] = Math.Log10( tmpdata[ i ] );
                }
                max = max < tmpdata[ i ] ? tmpdata[ i ] : max;
                min = min > tmpdata[ i ] ? tmpdata[ i ] : min;
            }
            double s = 255 / ( max - min );
            Bitmap result = new Bitmap( _Width, _Height );
            BitmapData resultdata = result.LockBits( new Rectangle( 0, 0, result.Width, result.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
            byte* resultpointer = (byte*) resultdata.Scan0.ToPointer();
            for( i = 0 ; i < scale ; ++i ) {
                resultpointer[ 0 ] = resultpointer[ 1 ] = resultpointer[ 2 ] = Funct.D2B( ( tmpdata[ i ] - min ) * s );
                resultpointer[ 3 ] = 255;
                resultpointer += 4;
            }
            result.UnlockBits( resultdata );
            return result;
        }
        public Bitmap ToPreviewImage() {
            int width = (int)(_Length + 10);
            int height = width;
            Bitmap result = new Bitmap( width, height );
            int cx = (int) ( ( width - _Length ) / 2 ), cy = height / 2;
            using( Graphics g = Graphics.FromImage( result ) ) {
                g.Clear( Color.Black );
                g.DrawLine( new Pen( Color.White, 1 ), cx, cy, (float) ( cx + _Length ), cy );
            }
            return result;
        }
    }
}
