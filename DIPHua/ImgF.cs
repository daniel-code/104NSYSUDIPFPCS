using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace DIPHua {
    public unsafe class ImgF {
        #region Pubilc
        #region 簡單的濾波
        /// <summary>
        /// 算術平均值濾波
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="Range">濾波範圍，通常為3，且必須為奇數</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Arithmeticmeanfilter( Bitmap Source, int Range ) {
            return Simplefilter( Source, Range, SimpleFilter.ArithmeticMean );
        }
        /// <summary>
        /// 中位數值濾波
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="Range">濾波範圍，通常為3，且必須為奇數</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Medianfilter( Bitmap Source, int Range ) {
            return Simplefilter( Source, Range, SimpleFilter.Median );
        }
        /// <summary>
        /// 最大值濾波
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="Range">濾波範圍，通常為3，且必須為奇數</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Maxfilter( Bitmap Source, int Range ) {
            return Simplefilter( Source, Range, SimpleFilter.Max );
        }
        /// <summary>
        /// 最小值濾波
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="Range">濾波範圍，通常為3，且必須為奇數</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Minfilter( Bitmap Source, int Range ) {
            return Simplefilter( Source, Range, SimpleFilter.Min );
        }
        /// <summary>
        /// 中數值濾波
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="Range">濾波範圍，通常為3，且必須為奇數</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Midpointfilter( Bitmap Source, int Range ) {
            return Simplefilter( Source, Range, SimpleFilter.MidPoint );
        }
        #endregion
        /// <summary>
        /// 灰階，即R、G、B值相同。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Grayscale( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            IntPtr SourceScan = SourceData.Scan0;
            IntPtr ResultScan = ResultData.Scan0;
            int X, Y, T, Total;
            byte Average;
            byte* SourcePointer = (byte*) SourceScan.ToPointer();
            byte* ResultPointer = (byte*) ResultScan.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Total = 0;
                    for( T = 0 ; T < 3 ; ++T ) {
                        Total += SourcePointer[ 0 ];
                        ++SourcePointer;
                    }
                    ++SourcePointer;
                    Average = (byte) ( Total / 3 );
                    for( T = 0 ; T < 3 ; ++T ) {
                        ResultPointer[ 0 ] = Average;
                        ++ResultPointer;
                    }
                    ResultPointer[ 0 ] = 255;
                    ++ResultPointer;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        /// <summary>
        /// 將一張顏色圖片，只顯示單色。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="HC">選擇顏色</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Threecolorimg( Bitmap Source, HColor HC ) {
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            int X, Y;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ResultPointer[ 0 ] = SourcePointer[ (int) HC ];
                    ResultPointer[ 1 ] = SourcePointer[ (int) HC ];
                    ResultPointer[ 2 ] = SourcePointer[ (int) HC ];
                    ResultPointer[ 3 ] = 255;
                    SourcePointer += 4;
                    ResultPointer += 4;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        /// <summary>
        /// 同態濾波(灰階)。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="GammaHigh"></param>
        /// <param name="GammaLow"></param>
        /// <param name="C">坡度銳利度</param>
        /// <param name="D0">截止頻率</param>
        /// <returns>結果圖片</returns>
        static public Bitmap HomomorphicfilterGrayscale( Bitmap Source, double GammaHigh = 2, double GammaLow = 0.25, double C = 1, double D0 = 80 ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImageExtendZero( Grayscale( Source ) );
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            AForge.Math.Complex[,] ComplexData = new AForge.Math.Complex[ Height, Width ];
            int X, Y;
            double D = GammaHigh - GammaLow, P, T;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexData[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 0 ], 0 );
                    ComplexData[ Y, X ].Re = Math.Log( ComplexData[ Y, X ].Re + 1 );
                    SourcePointer += 4;
                }
            }
            AForge.Math.FourierTransform.FFT2( ComplexData, AForge.Math.FourierTransform.Direction.Forward );
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    P = -C * ( ( Math.Pow( Y - ( Height / 2 ), 2 ) + Math.Pow( X - ( Width / 2 ), 2 ) ) / ( Math.Pow( D0, 2 ) ) );
                    T = ( 1 - Math.Exp( P ) );
                    ComplexData[ Y, X ] *= ( D * T ) + GammaLow;
                }
            }
            AForge.Math.FourierTransform.FFT2( ComplexData, AForge.Math.FourierTransform.Direction.Backward );
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            byte Tmp;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Tmp = DoubleToByte( Math.Exp( ComplexData[ Y, X ].Re ) );
                    ResultPointer[ 0 ] = Tmp;
                    ResultPointer[ 1 ] = Tmp;
                    ResultPointer[ 2 ] = Tmp;
                    ResultPointer[ 3 ] = 255;
                    ResultPointer += 4;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            Result = ImageUnExtend( Result, OWidth, OHeight, ImagePosition.LeftTop );
            return Result;
        }
        /// <summary>
        /// 上下左右反轉圖片。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Imagereverse( Bitmap Source ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImageExtendZero( Source );
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            AForge.Math.Complex[,] ComplexDataR = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataG = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataB = new AForge.Math.Complex[ Height, Width ];
            int X, Y;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexDataB[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 0 ], 0 );
                    ComplexDataG[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 1 ], 0 );
                    ComplexDataR[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 2 ], 0 );
                    SourcePointer += 4;
                }
            }
            BFFTShift( ComplexDataR );
            BFFTShift( ComplexDataG );
            BFFTShift( ComplexDataB );
            AForge.Math.FourierTransform.FFT2( ComplexDataR, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataG, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Forward );
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexDataR[ Y, X ].Im *= -1;
                    ComplexDataG[ Y, X ].Im *= -1;
                    ComplexDataB[ Y, X ].Im *= -1;
                }
            }
            AForge.Math.FourierTransform.FFT2( ComplexDataR, AForge.Math.FourierTransform.Direction.Backward );
            AForge.Math.FourierTransform.FFT2( ComplexDataG, AForge.Math.FourierTransform.Direction.Backward );
            AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Backward );
            BFFTShift( ComplexDataR );
            BFFTShift( ComplexDataG );
            BFFTShift( ComplexDataB );
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ResultPointer[ 0 ] = DoubleToByte( ComplexDataB[ Y, X ].Magnitude );
                    ResultPointer[ 1 ] = DoubleToByte( ComplexDataG[ Y, X ].Magnitude );
                    ResultPointer[ 2 ] = DoubleToByte( ComplexDataR[ Y, X ].Magnitude );
                    ResultPointer[ 3 ] = 255;
                    ResultPointer += 4;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            Result = ImageUnExtend( Result, OWidth, OHeight, ImagePosition.RightDown );
            return Result;
        }
        /// <summary>
        /// 頻域圖片顯示。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Frequencydisplay( Bitmap Source ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImageExtendZero( Source );
            int Width = Source.Width, Height = Source.Height;
            double Scale = Math.Sqrt( Width * Height );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            AForge.Math.Complex[,] ComplexDataR = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataG = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataB = new AForge.Math.Complex[ Height, Width ];
            int X, Y;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexDataB[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 0 ], 0 );
                    ComplexDataG[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 1 ], 0 );
                    ComplexDataR[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 2 ], 0 );
                    SourcePointer += 4;
                }
            }
            BFFTShift( ComplexDataR );
            BFFTShift( ComplexDataG );
            BFFTShift( ComplexDataB );
            AForge.Math.FourierTransform.FFT2( ComplexDataR, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataG, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Forward );
            /*
            double MaxR = 0;
            double MaxG = 0;
            double MaxB = 0;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    MaxR = MaxR < ComplexDataR[ Y, X ].Magnitude ? ComplexDataR[ Y, X ].Magnitude : MaxR;
                    MaxG = MaxG < ComplexDataG[ Y, X ].Magnitude ? ComplexDataG[ Y, X ].Magnitude : MaxG;
                    MaxB = MaxB < ComplexDataB[ Y, X ].Magnitude ? ComplexDataB[ Y, X ].Magnitude : MaxB;
                }
            }
            MaxB = 255D / ( Math.Log10( MaxB + 1 ) );
            MaxG = 255D / ( Math.Log10( MaxG + 1 ) );
            MaxR = 255D / ( Math.Log10( MaxR + 1 ) );
            */
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    /*
                    ResultPointer[ 0 ] = DoubleToByte( ( Math.Log10( ComplexDataB[ Y, X ].Magnitude + 1 ) ) * MaxB );
                    ResultPointer[ 1 ] = DoubleToByte( ( Math.Log10( ComplexDataG[ Y, X ].Magnitude + 1 ) ) * MaxG );
                    ResultPointer[ 2 ] = DoubleToByte( ( Math.Log10( ComplexDataR[ Y, X ].Magnitude + 1 ) ) * MaxR );
                    */
                    ResultPointer[ 0 ] = DoubleToByte( ComplexDataB[ Y, X ].Magnitude * Scale );
                    ResultPointer[ 1 ] = DoubleToByte( ComplexDataG[ Y, X ].Magnitude * Scale );
                    ResultPointer[ 2 ] = DoubleToByte( ComplexDataR[ Y, X ].Magnitude * Scale );
                    ResultPointer[ 3 ] = 255;
                    ResultPointer += 4;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        /// <summary>
        /// 頻域相位圖片顯示。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Phasedisplay( Bitmap Source ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImageExtendZero( Source );
            int Width = Source.Width, Height = Source.Height;
            double Scale = Math.Sqrt( Width * Height );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            AForge.Math.Complex[,] ComplexDataR = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataG = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataB = new AForge.Math.Complex[ Height, Width ];
            int X, Y;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexDataB[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 0 ], 0 );
                    ComplexDataG[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 1 ], 0 );
                    ComplexDataR[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 2 ], 0 );
                    SourcePointer += 4;
                }
            }
            BFFTShift( ComplexDataR );
            BFFTShift( ComplexDataG );
            BFFTShift( ComplexDataB );
            AForge.Math.FourierTransform.FFT2( ComplexDataR, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataG, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Forward );
            /*
            double MaxR = 0;
            double MaxG = 0;
            double MaxB = 0;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    MaxR = MaxR < ComplexDataR[ Y, X ].Phase ? ComplexDataR[ Y, X ].Phase : MaxR;
                    MaxG = MaxG < ComplexDataG[ Y, X ].Phase ? ComplexDataG[ Y, X ].Phase : MaxG;
                    MaxB = MaxB < ComplexDataB[ Y, X ].Phase ? ComplexDataB[ Y, X ].Phase : MaxB;
                }
            }
            MaxB = 255D / ( Math.Log10( MaxB + 1 ) );
            MaxG = 255D / ( Math.Log10( MaxG + 1 ) );
            MaxR = 255D / ( Math.Log10( MaxR + 1 ) );
            */
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    /*
                    ResultPointer[ 0 ] = DoubleToByte( ( Math.Log10( ComplexDataB[ Y, X ].Phase + 1 ) ) * MaxB );
                    ResultPointer[ 1 ] = DoubleToByte( ( Math.Log10( ComplexDataG[ Y, X ].Phase + 1 ) ) * MaxG );
                    ResultPointer[ 2 ] = DoubleToByte( ( Math.Log10( ComplexDataR[ Y, X ].Phase + 1 ) ) * MaxR );
                    */
                    ResultPointer[ 0 ] = DoubleToByte( ComplexDataB[ Y, X ].Phase * Scale );
                    ResultPointer[ 1 ] = DoubleToByte( ComplexDataG[ Y, X ].Phase * Scale );
                    ResultPointer[ 2 ] = DoubleToByte( ComplexDataR[ Y, X ].Phase * Scale );
                    ResultPointer[ 3 ] = 255;
                    ResultPointer += 4;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        /// <summary>
        /// 高斯高低通濾波。
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <param name="D0">截止頻率</param>
        /// <param name="GP">選擇高通或是低通</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Gaussianpass( Bitmap Source, double D0, GaussianPass GP, BinaryImage BI ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImageExtendZero( Source );
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            AForge.Math.Complex[,] ComplexDataR = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataG = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataB = new AForge.Math.Complex[ Height, Width ];
            int X, Y;
            double D, F;
            unsafe
            {
                byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
                for( Y = 0 ; Y < Height ; ++Y ) {
                    for( X = 0 ; X < Width ; ++X ) {
                        ComplexDataB[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 0 ], 0 );
                        ComplexDataG[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 1 ], 0 );
                        ComplexDataR[ Y, X ] = new AForge.Math.Complex( SourcePointer[ 2 ], 0 );
                        SourcePointer += 4;
                    }
                }
                BFFTShift( ComplexDataR );
                BFFTShift( ComplexDataG );
                BFFTShift( ComplexDataB );
                AForge.Math.FourierTransform.FFT2( ComplexDataR, AForge.Math.FourierTransform.Direction.Forward );
                AForge.Math.FourierTransform.FFT2( ComplexDataG, AForge.Math.FourierTransform.Direction.Forward );
                AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Forward );
                for( Y = 0 ; Y < Height ; ++Y ) {
                    for( X = 0 ; X < Width ; ++X ) {
                        D = Math.Pow( ( Y - ( Height / 2 ) ), 2 ) + Math.Pow( ( X - ( Width / 2 ) ), 2 );
                        F = Math.Exp( -1 * D / ( D0 * D0 ) );
                        if( GP == GaussianPass.High ) {
                            F = 1 - F;
                        }
                        ComplexDataR[ Y, X ] *= F;
                        ComplexDataG[ Y, X ] *= F;
                        ComplexDataB[ Y, X ] *= F;
                    }
                }
                AForge.Math.FourierTransform.FFT2( ComplexDataR, AForge.Math.FourierTransform.Direction.Backward );
                AForge.Math.FourierTransform.FFT2( ComplexDataG, AForge.Math.FourierTransform.Direction.Backward );
                AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Backward );
                BFFTShift( ComplexDataR );
                BFFTShift( ComplexDataG );
                BFFTShift( ComplexDataB );
                byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
                for( Y = 0 ; Y < Height ; ++Y ) {
                    for( X = 0 ; X < Width ; ++X ) {
                        if( BI != BinaryImage.False ) {
                            int B = (int) BI;
                            ComplexDataB[ Y, X ].Re = ComplexDataB[ Y, X ].Re <= B ? 0 : 255;
                            ComplexDataG[ Y, X ].Re = ComplexDataG[ Y, X ].Re <= B ? 0 : 255;
                            ComplexDataR[ Y, X ].Re = ComplexDataR[ Y, X ].Re <= B ? 0 : 255;
                        }
                        ResultPointer[ 0 ] = DoubleToByte( ComplexDataB[ Y, X ].Magnitude );
                        ResultPointer[ 1 ] = DoubleToByte( ComplexDataG[ Y, X ].Magnitude );
                        ResultPointer[ 2 ] = DoubleToByte( ComplexDataR[ Y, X ].Magnitude );
                        ResultPointer[ 3 ] = 255;
                        ResultPointer += 4;
                    }
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            Result = ImageUnExtend( Result, OWidth, OHeight, ImagePosition.LeftTop );
            return Result;
        }
        /// <summary>
        /// 直方圖
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Histogram( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            IntPtr SourceScan = SourceData.Scan0;
            int[,] Frequency = new int[ 4, 256 ];
            int Max = 0;
            int X, Y, T;
            byte* SourcePointer = (byte*) SourceScan.ToPointer();
            for( Y = 0 ; Y < 4 ; ++Y ) {
                for( X = 0 ; X < 256 ; ++X ) {
                    Frequency[ Y, X ] = 0;
                }
            }
            int Total = 0;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Total = 0;
                    for( T = 0 ; T < 3 ; ++T ) {
                        Total += SourcePointer[ 0 ];
                        ++Frequency[ T, SourcePointer[ 0 ] ];
                        Max = Max < Frequency[ T, SourcePointer[ 0 ] ] ? Frequency[ T, SourcePointer[ 0 ] ] : Max;
                        ++SourcePointer;
                    }
                    Total /= 3;
                    ++Frequency[ 3, Total ];
                    Max = Max < Frequency[ 3, Total ] ? Frequency[ 3, Total ] : Max;
                    ++SourcePointer;
                }
            }
            Source.UnlockBits( SourceData );
            PointF[][] PointFs = new PointF[ 4 ][];
            float XBigger = 5;
            float YSmaller = 1000.0F / Max;
            int ResWid = (int) ( 256 * XBigger ), ResHei = 1000;
            for( Y = 0 ; Y < 4 ; ++Y ) {
                PointFs[ Y ] = new PointF[ 256 ];
                for( X = 0 ; X < 256 ; ++X ) {
                    PointFs[ Y ][ X ] = new PointF( X * XBigger + XBigger / 2, ( Max - Frequency[ Y, X ] ) * YSmaller );
                }
            }
            Bitmap Result = new Bitmap( ResWid, ResHei );
            Graphics ResultGraphic = Graphics.FromImage( Result );
            ResultGraphic.Clear( Color.White );
            Pen Red = new Pen( Color.Red );
            Pen Green = new Pen( Color.Green );
            Pen Blue = new Pen( Color.Blue );
            Pen Black = new Pen( Color.Black );
            ResultGraphic.DrawLines( Black, PointFs[ 3 ] );
            ResultGraphic.DrawLines( Red, PointFs[ 2 ] );
            ResultGraphic.DrawLines( Green, PointFs[ 1 ] );
            ResultGraphic.DrawLines( Blue, PointFs[ 0 ] );
            ResultGraphic.Dispose();
            return Result;
        }
        /// <summary>
        /// 直方圖等化
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap Histogramequalization( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            int WMH = Width * Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            IntPtr SourceScan = SourceData.Scan0;
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            IntPtr ResultScan = ResultData.Scan0;
            int X, Y, T;
            int[,] Frequency = new int[ 3, 256 ];
            for( Y = 0 ; Y < 3 ; ++Y ) {
                for( X = 0 ; X < 256 ; ++X ) {
                    Frequency[ Y, X ] = 0;
                }
            }
            byte* SourcePointer = (byte*) SourceScan.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        Frequency[ T, SourcePointer[ 0 ] ]++;
                        ++SourcePointer;
                    }
                    ++SourcePointer;
                }
            }
            for( X = 1 ; X < 256 ; ++X ) {
                for( T = 0 ; T < 3 ; ++T ) {
                    Frequency[ T, X ] += Frequency[ T, X - 1 ];
                }
            }
            SourcePointer = (byte*) SourceScan.ToPointer();
            byte* ResultPointer = (byte*) ResultScan.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ResultPointer[ 0 ] = DoubleToByte( Frequency[ T, SourcePointer[ 0 ] ] * 255.0 / WMH );
                        ++ResultPointer;
                        ++SourcePointer;
                    }
                    ResultPointer[ 0 ] = 255;
                    ++ResultPointer;
                    ++SourcePointer;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        /// <summary>
        /// 捲積(灰階)
        /// </summary>
        /// <param name="SourceA">圖片A</param>
        /// <param name="SourceB">圖片B</param>
        /// <returns>結果圖片</returns>
        static public Bitmap ConvolutionGrayscale( Bitmap SourceA, Bitmap SourceB ) {
            int OWidth = SourceA.Width, OHeight = SourceA.Height;
            if( SourceB.Width != OWidth || SourceB.Height != OHeight ) {
                throw new Exception( "圖片大小不同" );
            }
            SourceA = ImageExtendZero( Grayscale( SourceA ) );
            SourceB = ImageExtendZero( Grayscale( SourceB ) );
            int Width = SourceA.Width, Height = SourceA.Height;
            BitmapData SourceAData = SourceA.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BitmapData SourceBData = SourceB.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            AForge.Math.Complex[,] ComplexDataA = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataB = new AForge.Math.Complex[ Height, Width ];
            AForge.Math.Complex[,] ComplexDataResult = new AForge.Math.Complex[ Height, Width ];
            double Scale = Math.Sqrt( Width * Height );
            int X, Y;
            byte* SourceAPointer = (byte*) SourceAData.Scan0.ToPointer();
            byte* SourceBPointer = (byte*) SourceBData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexDataA[ Y, X ] = new AForge.Math.Complex( SourceAPointer[ 0 ], 0 );
                    ComplexDataB[ Y, X ] = new AForge.Math.Complex( SourceBPointer[ 0 ], 0 );
                    SourceAPointer += 4;
                    SourceBPointer += 4;
                }
            }
            AForge.Math.FourierTransform.FFT2( ComplexDataA, AForge.Math.FourierTransform.Direction.Forward );
            AForge.Math.FourierTransform.FFT2( ComplexDataB, AForge.Math.FourierTransform.Direction.Forward );
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    ComplexDataResult[ Y, X ] = ComplexDataA[ Y, X ] * ComplexDataB[ Y, X ];
                }
            }
            AForge.Math.FourierTransform.FFT2( ComplexDataResult, AForge.Math.FourierTransform.Direction.Backward );
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            byte T;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    T = DoubleToByte( ComplexDataResult[ Y, X ].Magnitude );
                    ResultPointer[ 0 ] = T;
                    ResultPointer[ 1 ] = T;
                    ResultPointer[ 2 ] = T;
                    ResultPointer[ 3 ] = 255;
                    ResultPointer += 4;
                }
            }
            SourceA.UnlockBits( SourceAData );
            SourceB.UnlockBits( SourceBData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        /// <summary>
        /// 字串選擇副檔名。
        /// </summary>
        /// <param name="SaveFormat">字串副檔名(EX: png, gif, jpg)</param>
        /// <returns>儲存圖片時需要的附檔名類別</returns>
        static public ImageFormat ChooseImageFormat( string SaveFormat ) {
            switch( SaveFormat.ToUpper() ) {
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
        /// <summary>
        /// 縮小圖片
        /// </summary>
        /// <param name="Source">來源圖片</param>
        /// <returns>結果圖片</returns>
        static public Bitmap ImageSmaller(Bitmap Source) {
            int OWidth = Source.Width;
            int OHeight = Source.Height;
            double Smaller = 0;
            if( OWidth > OHeight ) {
                Smaller = (double) 512 / OWidth;
            } else {
                Smaller = (double) 512 / OHeight;
            }
            int AWidth = (int) ( OWidth * Smaller );
            int AHeight = (int) ( OHeight * Smaller );
            Bitmap Result = new Bitmap( AWidth, AHeight );
            using( var G = Graphics.FromImage( Result ) ) {
                G.DrawImage( Source , new Rectangle( 0, 0, AWidth, AHeight ), new Rectangle( 0, 0, OWidth, OHeight ), GraphicsUnit.Pixel );
            }
            return Result;
        }
        #endregion
        #region Private
        static private Bitmap Simplefilter( Bitmap Source, int Range, SimpleFilter SF ) {
            int Width = Source.Width, Height = Source.Height;
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            IntPtr SourceScan = SourceData.Scan0;
            IntPtr ResultScan = ResultData.Scan0;
            int X, Y, T, A, B;
            List<byte> RGBList = new List<byte>();
            byte[,,] SourceImage = new byte[ Height, Width, 3 ];
            byte[,,] ResultImage = new byte[ Height, Width, 3 ];
            byte* SourcePointer = (byte*) SourceScan.ToPointer();
            byte* ResultPointer = (byte*) ResultScan.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        SourceImage[ Y, X, T ] = SourcePointer[ 0 ];
                        ++SourcePointer;
                    }
                    ++SourcePointer;
                }
            }
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        RGBList.Clear();
                        for( A = Y - Range / 2 ; A <= Y + Range / 2 ; ++A ) {
                            if( A >= 0 && A < Height ) {
                                for( B = X - Range / 2 ; B <= X + Range / 2 ; ++B ) {
                                    if( B >= 0 && B < Width ) {
                                        RGBList.Add( SourceImage[ A, B, T ] );
                                    }
                                }
                            }
                        }
                        if( SF == SimpleFilter.Median ) {
                            ResultImage[ Y, X, T ] = Median( RGBList.ToArray() );
                        } else if( SF == SimpleFilter.ArithmeticMean ) {
                            ResultImage[ Y, X, T ] = Arithmeticmean( RGBList.ToArray() );
                        } else if( SF == SimpleFilter.Max ) {
                            ResultImage[ Y, X, T ] = Max( RGBList.ToArray() );
                        } else if( SF == SimpleFilter.Min ) {
                            ResultImage[ Y, X, T ] = Min( RGBList.ToArray() );
                        } else if( SF == SimpleFilter.MidPoint ) {
                            ResultImage[ Y, X, T ] = MidPoint( RGBList.ToArray() );
                        }
                    }
                }
            }
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ResultPointer[ 0 ] = ResultImage[ Y, X, T ];
                        ++ResultPointer;
                    }
                    ResultPointer[ 0 ] = 255;
                    ++ResultPointer;
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        static private byte Arithmeticmean( byte[] Search ) {
            int Total = 0;
            for( int i = 0 ; i < Search.Length ; ++i ) {
                Total += Search[ i ];
            }
            return (byte) ( (float) Total / Search.Length );
        }
        static private byte Median( byte[] Search ) {
            Array.Sort( Search );
            return Search[ Search.Length / 2 ];
        }
        static private byte Max( byte[] Search ) {
            Array.Sort( Search );
            return Search[ Search.Length - 1 ];
        }
        static private byte Min( byte[] Search ) {
            Array.Sort( Search );
            return Search[ 0 ];
        }
        static private byte MidPoint( byte[] Search ) {
            Array.Sort( Search );
            return (byte) ( (float) ( Search[ Search.Length - 1 ] + Search[ 0 ] ) / 2 );
        }
        static private Bitmap ImageExtendZero( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            int EWidth = (int) Math.Pow( 2, AForge.Math.Tools.Log2( Width ) ), EHeight = (int) Math.Pow( 2, AForge.Math.Tools.Log2( Height ) );
            Bitmap Result = new Bitmap( EWidth, EHeight, PixelFormat.Format32bppArgb );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, EWidth, EHeight ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            int X, Y, T;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 4 ; ++T ) {
                        ResultPointer[ 0 ] = SourcePointer[ 0 ];
                        ++ResultPointer;
                        ++SourcePointer;
                    }
                }
                for( ; X < EWidth ; ++X ) {
                    ResultPointer[ 0 ] = 0;
                    ResultPointer[ 1 ] = 0;
                    ResultPointer[ 2 ] = 0;
                    ResultPointer[ 3 ] = 255;
                    ResultPointer += 4;
                }
            }
            for( ; Y < EHeight ; ++Y ) {
                ResultPointer[ 0 ] = 0;
                ResultPointer[ 1 ] = 0;
                ResultPointer[ 2 ] = 0;
                ResultPointer[ 3 ] = 255;
                ResultPointer += 4;
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        static private Bitmap ImageUnExtend( Bitmap Source, int OWidth, int OHeight, ImagePosition IP ) {
            int Width = Source.Width, Height = Source.Height;
            Bitmap Result = new Bitmap( OWidth, OHeight );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, OWidth, OHeight ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            int X, Y, T;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            int DW = ( Width - OWidth ) * 4;
            int DH = Width * 4 * ( Height - OHeight );
            switch( IP ) {
                case ImagePosition.LeftTop:
                    break;
                case ImagePosition.RightTop:
                    SourcePointer += DW;
                    break;
                case ImagePosition.RightDown:
                    SourcePointer += DH + DW;
                    break;
                case ImagePosition.LeftDown:
                    SourcePointer += DH;
                    break;
            }
            for( Y = 0 ; Y < OHeight ; ++Y ) {
                for( X = 0 ; X < OWidth ; ++X ) {
                    for( T = 0 ; T < 4 ; ++T ) {
                        ResultPointer[ 0 ] = SourcePointer[ 0 ];
                        ++ResultPointer;
                        ++SourcePointer;
                    }
                }
                SourcePointer += DW;
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        static private byte DoubleToByte( double Source ) {
            if( Source > 255 ) {
                return 255;
            } else if( Source < 0 ) {
                return 0;
            }
            return (byte) Source;
        }
        static public void BFFTShift( AForge.Math.Complex[,] ComplexData ) {
            int Width = ComplexData.GetLength( 1 ), Height = ComplexData.GetLength( 0 );
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    if( ( ( X + Y ) & 0x1 ) != 0 ) {
                        ComplexData[ Y, X ].Re *= -1;
                        ComplexData[ Y, X ].Im *= -1;
                    }
                }
            }
        }
        #endregion
    }
}
