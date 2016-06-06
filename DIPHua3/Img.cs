using MathNet.Numerics;
using MathNet.Numerics.Transformations;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DIPHua3 {
    public enum BGRModel {
        B = 1, G = 2, R = 4
    }
    public unsafe class ImgF {
        static public Complex[] SpatialFilter( Complex[] source, int width, int height, double[,] filter ) {
            int fheight = filter.GetLength( 0 ), fwidth = filter.GetLength( 1 );
            if( fheight % 2 == 0 || fheight != fwidth ) {
                throw new Exception( "Filter 必須為 [n*n]，n 為奇數。" );
            }
            int flen = fheight;
            int flend2 = flen / 2;
            int x, y, a, b, m, n;
            double t;
            Complex[] result = new Complex[ source.Length ];
            for( y = 0 ; y < height ; ++y ) {
                for( x = 0 ; x < width ; ++x ) {
                    t = 0;
                    for( a = y - flend2, m = 0 ; a <= y + flend2 ; ++a, ++m ) {
                        if( a >= 0 && a < height ) {
                            for( b = x - flend2, n = 0 ; b <= x + flend2 ; ++b, ++n ) {
                                if( b >= 0 && b < width ) {
                                    t += filter[ m, n ] * source[ a * width + b ].Modulus;
                                }
                            }
                        }
                    }
                    result[ y * width + x ] = Complex.FromRealImaginary( t, 0 );
                }
            }
            return result;
        }
        static public ComplexFourierTransformation CFTMatlab = new ComplexFourierTransformation( TransformationConvention.Matlab );
        static public ComplexFourierTransformation CFT = new ComplexFourierTransformation( TransformationConvention.Default );
        static public bool IsPowerOf2( int x ) {
            return ( x & ( x - 1 ) ) == 0;
        }
        static public int Pow2( int exponent ) {
            if( exponent >= 0 && exponent < 31 ) {
                return 1 << exponent;
            }
            return 0;
        }
        static public int Log2( int x ) {
            if( x <= 65536 ) {
                if( x <= 256 ) {
                    if( x <= 16 ) {
                        if( x <= 4 ) {
                            if( x <= 2 ) {
                                if( x <= 1 ) {
                                    return 0;
                                }
                                return 1;
                            }
                            return 2;
                        }
                        if( x <= 8 )
                            return 3;
                        return 4;
                    }
                    if( x <= 64 ) {
                        if( x <= 32 )
                            return 5;
                        return 6;
                    }
                    if( x <= 128 )
                        return 7;
                    return 8;
                }
                if( x <= 4096 ) {
                    if( x <= 1024 ) {
                        if( x <= 512 )
                            return 9;
                        return 10;
                    }
                    if( x <= 2048 )
                        return 11;
                    return 12;
                }
                if( x <= 16384 ) {
                    if( x <= 8192 )
                        return 13;
                    return 14;
                }
                if( x <= 32768 )
                    return 15;
                return 16;
            }
            if( x <= 16777216 ) {
                if( x <= 1048576 ) {
                    if( x <= 262144 ) {
                        if( x <= 131072 )
                            return 17;
                        return 18;
                    }
                    if( x <= 524288 )
                        return 19;
                    return 20;
                }
                if( x <= 4194304 ) {
                    if( x <= 2097152 )
                        return 21;
                    return 22;
                }
                if( x <= 8388608 )
                    return 23;
                return 24;
            }
            if( x <= 268435456 ) {
                if( x <= 67108864 ) {
                    if( x <= 33554432 )
                        return 25;
                    return 26;
                }
                if( x <= 134217728 )
                    return 27;
                return 28;
            }
            if( x <= 1073741824 ) {
                if( x <= 536870912 )
                    return 29;
                return 30;
            }
            return 31;
        }
        static public byte D2B( double a ) {
            if( a > 255 ) {
                return 255;
            } else if( a < 0 ) {
                return 0;
            } else {
                return (byte) a;
            }
        }
        static public ImageFormat ChooseImgFormat( string stringformat ) {
            switch( stringformat.ToUpper() ) {
                case "PNG":
                    return ImageFormat.Png;
                case "JPG":
                case "JPEG":
                    return ImageFormat.Jpeg;
                case "GIF":
                    return ImageFormat.Gif;
                case "BMP":
                    return ImageFormat.Bmp;
                case "EMF":
                    return ImageFormat.Emf;
                case "ICO":
                case "ICON":
                    return ImageFormat.Icon;
                case "WMF":
                    return ImageFormat.Wmf;
                default:
                    return ImageFormat.Tiff;
            }
        }
        static public void AddIn( Complex[] result, Complex[] source ) {
            int l = result.Length;
            if( l != source.Length ) {
                throw new Exception( "長度不同" );
            }
            int i;
            for( i = 0 ; i < l ; ++i ) {
                result[ i ] += source[ i ];
            }
        }
        static public double Max( Complex[] source ) {
            int l = source.Length;
            int i;
            double max = double.MinValue;
            for( i = 0 ; i < l ; ++i ) {
                max = max < source[ i ].Modulus ? source[ i ].Modulus : max;
            }
            return max;
        }
        static public double Min( Complex[] source ) {
            int l = source.Length;
            int i;
            double min = double.MaxValue;
            for( i = 0 ; i < l ; ++i ) {
                min = min > source[ i ].Modulus ? source[ i ].Modulus : min;
            }
            return min;
        }
        static public double[] MinMax( Complex[] source ) {
            int l = source.Length;
            int i;
            double max = double.MinValue;
            double min = double.MaxValue;
            double t;
            for( i = 0 ; i < l ; ++i ) {
                t = source[ i ].Modulus;
                min = min > t ? t : min;
                max = max < t ? t : max;
            }
            return new double[] { max, min };
        }
        static public void Between0And255( Complex[] source ) {
            double[] fmaxmin = MinMax( source );
            fmaxmin[ 0 ] -= fmaxmin[ 1 ];
            fmaxmin[ 0 ] = 255 / fmaxmin[ 0 ];
            int l = source.Length;
            int i;
            for( i = 0 ; i < l ; ++i ) {
                source[ i ] -= fmaxmin[ 1 ];
                source[ i ] *= fmaxmin[ 0 ];
            }
        }
        static public void Sharpen( Complex[] source, int width, int height ) {
            double[,] filter = new double[,] {
                { -1, -1, -1},
                { -1, 8, -1},
                { -1, -1, -1}
            };
            source = SpatialFilter( source, width, height, filter );
            /*
            Between0And255( ftmp );
            AddIn( source, ftmp );
            Between0And255( source );
            */
        }
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
            int ewidth = (int) Math.Pow( 2, ImgF.Log2( _Width ) ),
                eheight = (int) Math.Pow( 2, ImgF.Log2( _Height ) );
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
            ImgF.CFTMatlab.TransformForward( _Gray, new int[] { _Height, _Width } );
        }
        public void IFFT2() {
            ImgF.CFTMatlab.TransformBackward( _Gray, new int[] { _Height, _Width } );
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
            int i;
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
                resultpointer[ 0 ] = resultpointer[ 1 ] = resultpointer[ 2 ] = ImgF.D2B( ( tmpdata[ i ] - min ) * s );
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
            int ewidth = (int) Math.Pow( 2, ImgF.Log2( _Width ) ), 
                eheight = (int) Math.Pow( 2, ImgF.Log2( _Height ) );
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
                    resultpointer[ 0 ] = ImgF.D2B( ( tmpdata[ t, i ] - min ) * s );
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
        public void GLFilter() {
            GaussianFilter();
            LaplaceFilter();
        }
        public void GaussianFilter() {
            SpatialFilter( new double[,] {
                { -2, -4, -4, -4, -2},
                { -4, 0, 8, 0, -4},
                { -4, 8, 2, 8, -4},
                { -4, 0, 8, 0, -4},
                { -2, -4, -4, -4, -2},
            } );
        }
        public void LaplaceFilter() {
            SpatialFilter( new double[,] {
                { 0, 0, -1, 0, 0},
                { 0, -1, -2, -1, 0},
                { -1, -2, 16, -2, -1},
                { 0, -1, -2, -1, 0},
                { 0, 0, -1, 0, 0},
            } );
        }
        private void SpatialFilter( double[,] filter ) {
            _BGR[ 0 ] = ImgF.SpatialFilter( _BGR[ 0 ], _Width, _Height, filter );
            _BGR[ 1 ] = ImgF.SpatialFilter( _BGR[ 1 ], _Width, _Height, filter );
            _BGR[ 2 ] = ImgF.SpatialFilter( _BGR[ 2 ], _Width, _Height, filter );
        }
        public void Sharpen() {
            ImgF.Sharpen( _BGR[ 0 ], _Width, _Height );
            ImgF.Sharpen( _BGR[ 1 ], _Width, _Height );
            ImgF.Sharpen( _BGR[ 2 ], _Width, _Height );
        }
        public void FFT2() {
            ImgF.CFTMatlab.TransformForward( _BGR[ 0 ], new int[] { _Height, _Width } );
            ImgF.CFTMatlab.TransformForward( _BGR[ 1 ], new int[] { _Height, _Width } );
            ImgF.CFTMatlab.TransformForward( _BGR[ 2 ], new int[] { _Height, _Width } );
        }
        public void IFFT2() {
            ImgF.CFTMatlab.TransformBackward( _BGR[ 0 ], new int[] { _Height, _Width } );
            ImgF.CFTMatlab.TransformBackward( _BGR[ 1 ], new int[] { _Height, _Width } );
            ImgF.CFTMatlab.TransformBackward( _BGR[ 2 ], new int[] { _Height, _Width } );
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
            ImgF.CFTMatlab.TransformForward( _Kernel, new int[] { _Height, _Width } );
        }
        public Bitmap ToImage( bool Freq = false ) {
            int scale = _Width * _Height;
            int i;
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
                resultpointer[ 0 ] = resultpointer[ 1 ] = resultpointer[ 2 ] = ImgF.D2B( ( tmpdata[ i ] - min ) * s );
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
