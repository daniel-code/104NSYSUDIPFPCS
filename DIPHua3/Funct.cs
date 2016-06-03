using MathNet.Numerics.Transformations;
using System.Drawing.Imaging;

namespace DIPHua3 {
    public class Funct {
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
    }
}
