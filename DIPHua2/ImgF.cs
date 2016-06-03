using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AMComplex = AForge.Math.Complex;
using AMFT = AForge.Math.FourierTransform;
using EDSPComplex = Exocortex.DSP.Complex;
using EDSPComplexF = Exocortex.DSP.ComplexF;
using EDSPFT = Exocortex.DSP.Fourier;
using EDSPFD = Exocortex.DSP.FourierDirection;

namespace DIPHua2 {
    public unsafe class ImgF {
        #region Simple Spatial Filter
        static private Bitmap SimpleSpatialFilter( Bitmap Source, int Range, Func<byte[], byte> Type ) {
            if( Range % 2 == 0 ) {
                throw new Exception( "範圍必須為奇數" );
            }
            int Height = Source.Height, Width = Source.Width;
            BGR[,] BGRSource = Img2BGR( Source );
            BGR[,] Result = new BGR[ Height, Width ];
            int X, Y, T, A, B;
            List<byte> BGRList = new List<byte>();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Result[ Y, X ] = new BGR();
                    for( T = 0 ; T < 3 ; ++T ) {
                        BGRList.Clear();
                        for( A = Y - Range / 2 ; A <= Y + Range / 2 ; ++A ) {
                            if( A >= 0 && A < Height ) {
                                for( B = X - Range / 2 ; B <= X + Range / 2 ; ++B ) {
                                    if( B >= 0 && B < Width ) {
                                        BGRList.Add( DoubleToByte( BGRSource[ A, B ][ T ] ) );
                                    }
                                }
                            }
                        }
                        Result[ Y, X ][ T ] = Type( BGRList.ToArray() );
                    }
                }
            }
            return BGR2Img( Result );
        }
        static public Bitmap Arithmeticmeanfilter( Bitmap Source, int Range ) {
            return SimpleSpatialFilter( Source, Range, Arithmeticmean );
        }
        static public Bitmap Medianfilter( Bitmap Source, int Range ) {
            return SimpleSpatialFilter( Source, Range, Median );
        }
        static public Bitmap Maxfilter( Bitmap Source, int Range ) {
            return SimpleSpatialFilter( Source, Range, Max );
        }
        static public Bitmap Minfilter( Bitmap Source, int Range ) {
            return SimpleSpatialFilter( Source, Range, Min );
        }
        static public Bitmap Midpointfilter( Bitmap Source, int Range ) {
            return SimpleSpatialFilter( Source, Range, MidPoint );
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
        #endregion
        #region Main
        static public Bitmap GrayScale( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            BGR[,] BGRSource = Img2BGR( Source );
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    BGRSource[ Y, X ].ToGrayScale();
                }
            }
            return BGR2Img( BGRSource );
        }
        static public Bitmap StrongColor( Bitmap Source, BGRModel Type ) {
            int Width = Source.Width, Height = Source.Height;
            BGR[,] Result = Img2BGR( Source );
            int X, Y;
            byte Want = 0;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Want = DoubleToByte( Result[ Y, X ][ (int) Type / 2 ] );
                    Result[ Y, X ][ 0 ] = Result[ Y, X ][ 1 ] = Result[ Y, X ][ 2 ] = Want;
                }
            }
            return BGR2Img( Result );
        }
        static public Bitmap SingleColor( Bitmap Source, BGRModel Type ) {
            int Width = Source.Width, Height = Source.Height;
            BGR[,] Result = Img2BGR( Source );
            int X, Y, T;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        if( ( Type & BGRModel.R ) != BGRModel.R ) {
                            Result[ Y, X ].R = 0;
                        }
                        if( ( Type & BGRModel.G ) != BGRModel.G ) {
                            Result[ Y, X ].G = 0;
                        }
                        if( ( Type & BGRModel.B ) != BGRModel.B ) {
                            Result[ Y, X ].B = 0;
                        }
                    }
                }
            }
            return BGR2Img( Result );
        }
        static public Bitmap MagnitudeDisplay( Bitmap Source ) {
            Source = ImgExtend( Source );
            BGRComplexImg ComplexSource = Img2BGRComplexImg( Source );
            ComplexSource.BFFTShift();
            ComplexSource.FFT2();
            return Spectrum2Img( ComplexSource, Spectrum.Magnitude );
        }
        static public Bitmap PhaseDisplay( Bitmap Source ) {
            Source = ImgExtend( Source );
            BGRComplexImg ComplexSource = Img2BGRComplexImg( Source );
            ComplexSource.BFFTShift();
            ComplexSource.FFT2();
            return Spectrum2Img( ComplexSource, Spectrum.Phase );
        }
        static public Bitmap HistogramEqualization( Bitmap Source ) {
            HSI[,] HSISource = BGR2HSI( Img2BGR( Source ) );
            int Scale = Source.Width * Source.Height;
            int[] Frequency = new int[ 256 ];
            int X, Y, T;
            for( T = 0 ; T < 256 ; ++T ) {
                Frequency[ T ] = 0;
            }
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    ++Frequency[ DoubleToByte( HSISource[ Y, X ].I255 ) ];
                }
            }
            for( T = 1 ; T < 256 ; ++T ) {
                Frequency[ T ] += Frequency[ T - 1 ];
            }
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    HSISource[ Y, X ].I = Frequency[ DoubleToByte( HSISource[ Y, X ].I255 ) ] * 255.0 / Scale / 255D;
                }
            }
            return BGR2Img( HSI2BGR( HSISource ) );
        }
        static public Bitmap Convolution( Bitmap SourceA, Bitmap SourceB, bool Zero = false ) {
            int OWidth = SourceA.Width, OHeight = SourceA.Height;
            if( OWidth != SourceB.Width || OHeight != SourceB.Height ) {
                throw new Exception( "圖片大小不一" );
            }
            SourceA = ImgExtend( SourceA, Zero );
            SourceB = ImgExtend( SourceB, Zero );
            int Width = SourceA.Width, Height = SourceA.Height;
            int X, Y, T;
            BGRComplexImg ComplexSourceA = Img2BGRComplexImg( SourceA );
            BGRComplexImg ComplexSourceB = Img2BGRComplexImg( SourceB );
            BGRComplexImg ComplexResult = new BGRComplexImg( Width, Height );
            ComplexSourceA.FFT2();
            ComplexSourceB.FFT2();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ComplexResult[ T ][ Y, X ] = ComplexSourceA[ T ][ Y, X ] * ComplexSourceB[ T ][ Y, X ];
                    }
                }
            }
            ComplexResult.BFFTShift();
            ComplexResult.IFFT2();
            Bitmap Result = BGRComplexImg2Img( ComplexResult );
            return ImgUnExtend( Result, OWidth, OHeight );
        }
        static public Bitmap Deconvolution( Bitmap SourceA, Bitmap SourceB, double Lambda = 10, bool Zero = false ) {
            int OWidth = SourceA.Width, OHeight = SourceA.Height;
            if( OWidth != SourceB.Width || OHeight != SourceB.Height ) {
                throw new Exception( "圖片大小不一" );
            }
            SourceA = ImgExtend( SourceA, Zero );
            SourceB = ImgExtend( SourceB, Zero );
            int Width = SourceA.Width, Height = SourceA.Height;
            int X, Y, T;
            BGRComplexImg ComplexSourceA = Img2BGRComplexImg( SourceA );
            BGRComplexImg ComplexSourceB = Img2BGRComplexImg( SourceB );
            BGRComplexImg ComplexResult = new BGRComplexImg( Width, Height );
            ComplexSourceA.FFT2();
            ComplexSourceB.FFT2();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ComplexResult[ T ][ Y, X ] = AMComplex.MaxDivide( ComplexSourceA[ T ][ Y, X ], ComplexSourceB[ T ][ Y, X ] ) * ( ComplexSourceA[ T ][ Y, X ].Magnitude / ( ComplexSourceA[ T ][ Y, X ].Magnitude + Lambda ) );
                    }
                }
            }
            ComplexResult.BFFTShift();
            ComplexResult.IFFT2();
            Bitmap Result = BGRComplexImg2Img( ComplexResult );
            return ImgUnExtend( Result, OWidth, OHeight );
        }
        static public Bitmap SimpleMotionBlur( Bitmap Source, float Length ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImgExtend( Source, true );
            int Width = Source.Width, Height = Source.Height;
            var ComplexKernel = SimpleComplexKernel( Width, Height, Length, true );
            int X, Y, T;
            BGRComplexImg ComplexSource = Img2BGRComplexImg( Source, true );
            BGRComplexImg ComplexResult = new BGRComplexImg( Width, Height );
            ComplexSource.FFT2();
            AMFT.FFT2( ComplexKernel, AMFT.Direction.Forward );
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ComplexResult[ T ][ Y, X ] = ComplexSource[ T ][ Y, X ] * ComplexKernel[ Y, X ];
                    }
                }
            }
            ComplexResult.BFFTShift();
            ComplexResult.IFFT2();
            Bitmap Result = BGRComplexImg2Img( ComplexResult );
            return ImgUnExtend( Result, OWidth, OHeight );
        }
        static public Bitmap SimpleMotionDeblur( Bitmap Source, float Length, double Lambda = 10 ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImgExtend( GrayScale( Source ), true );
            int Width = Source.Width, Height = Source.Height;
            var ComplexKernel = AM2EDSP( SimpleComplexKernel( Width, Height, Length, true ) );
            var ComplexSource = Img2BGRComplexImg( Source, false );
            var ComplexResult = new BGRComplexImg( Width, Height );
            
            ComplexSource.FFT2();
            //AMFT.FFT2( ComplexKernel, AMFT.Direction.Forward );
            EDSPFT.FFT2( ComplexKernel, Width, Height, EDSPFD.Forward );

            Console.WriteLine();
            Console.WriteLine( "-----Source FFT--------------------------" );
            Console.WriteLine( Max( ComplexSource, BGRModel.R ) );
            Console.WriteLine( "-----------------------------------------" );
            Console.WriteLine();
            Console.WriteLine( "-----Kernel FFT--------------------------" );
            Console.WriteLine( Max( ComplexKernel ) );
            Console.WriteLine( "-----------------------------------------" );
            return Source;
            /*
            int X, Y, T;
            double D2;
            double Scale;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    D2 = ComplexKernel[ Y, X ].SquaredMagnitude;
                    Scale = D2 / ( D2 + Lambda );
                    for( T = 0 ; T < 3 ; ++T ) {
                        ComplexResult[ T ][ Y, X ] = ComplexSource[ T ][ Y, X ] * Scale / ComplexKernel[ Y, X ];
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine( "-----Result FFT--------------------------" );
            Console.WriteLine( Max( ComplexResult, BGRModel.R ) );
            Console.WriteLine( "-----------------------------------------" );

            ComplexResult.BFFTShift();
            ComplexResult.IFFT2();

            Console.WriteLine();
            Console.WriteLine( "-----Result------------------------------" );
            Console.WriteLine( Max( ComplexResult, BGRModel.R ) );
            Console.WriteLine( "-----------------------------------------" );

            Bitmap Result = BGRComplexImg2Img( ComplexResult );
            return ImgUnExtend( Result, OWidth, OHeight );
            */
        }
        static public Bitmap HomomorphicFilter( Bitmap Source, double GammaHigh = 2, double GammaLow = 0.25, double C = 1, double D0 = 80 ) {
            int OWidth = Source.Width, OHeight = Source.Height;
            Source = ImgExtend( Source );
            int Width = Source.Width, Height = Source.Height;
            int X, Y, T;
            double D = GammaHigh - GammaLow, P, W, G;
            BGRComplexImg ComplexSource = Img2BGRComplexImg( Source );
            //ComplexSource.ToDouble();
            ComplexSource.BFFTShift();
            ComplexSource.Ln();
            ComplexSource.FFT2();
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    P = -C * ( ( Math.Pow( Y - ( Height / 2 ), 2 ) + Math.Pow( X - ( Width / 2 ), 2 ) ) / ( Math.Pow( D0, 2 ) ) );
                    W = ( 1 - Math.Exp( P ) );
                    G = ( D * W ) + GammaLow;
                    for( T = 0 ; T < 3 ; ++T ) {
                        ComplexSource[ T ][ Y, X ] *= G;
                    }
                }
            }
            ComplexSource.IFFT2();
            ComplexSource.Exp();
            ComplexSource.BFFTShift();
            //ComplexSource.Range();
            Bitmap Result = BGRComplexImg2Img( ComplexSource );
            return ImgUnExtend( Result, OWidth, OHeight );
        }
        #endregion
        #region Not Main
        static public Bitmap Spectrum2Img( BGRComplexImg Source, Spectrum Type ) {
            BGR[,] Result = new BGR[ Source.Height, Source.Width ];
            double Scale = Math.Sqrt( Source.Height * Source.Width );
            int X, Y, T;
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    Result[ Y, X ] = new BGR();
                    for( T = 0 ; T < 3 ; ++T ) {
                        if( Type == Spectrum.Magnitude ) {
                            Result[ Y, X ][ T ] = Source[ T ][ Y, X ].Magnitude;
                        } else if( Type == Spectrum.Phase ) {
                            Result[ Y, X ][ T ] = Source[ T ][ Y, X ].Phase;
                        }
                        Result[ Y, X ][ T ] = DoubleToByte( Math.Log( Result[ Y, X ][ T ] + 1 ) * Scale );
                    }
                }
            }
            return BGR2Img( Result );
        }

        static public BGRComplexImg Img2BGRComplexImg( Bitmap Source, bool Compress = false ) {
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BGRComplexImg Result = new BGRComplexImg( Width, Height );
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            int X, Y, T;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        Result[ T ][ Y, X ] = new AMComplex( SourcePointer[ 0 ], 0 );
                        if( Compress ) {
                            Result[ T ][ Y, X ] /= 255;
                        }
                        ++SourcePointer;
                    }
                    ++SourcePointer;
                }
            }
            Source.UnlockBits( SourceData );
            return Result;
        }
        static public Bitmap BGRComplexImg2Img( BGRComplexImg Source ) {
            Bitmap Result = new Bitmap( Source.Width, Source.Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Source.Width, Source.Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            int X, Y, T;
            double Scale = 1;
            double Max = 0;
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        Max = Max < Source[ T ][ Y, X ].Magnitude ? Source[ T ][ Y, X ].Magnitude : Max;
                    }
                }
            }
            Scale = 255 / Max;
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ResultPointer[ 0 ] = DoubleToByte( Source[ T ][ Y, X ].Magnitude * Scale );
                        ++ResultPointer;
                    }
                    ResultPointer[ 0 ] = 255;
                    ++ResultPointer;
                }
            }
            Result.UnlockBits( ResultData );
            return Result;
        }
        static public HSIComplexImg Img2HSIComplexImg( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            HSIComplexImg Result = new HSIComplexImg( Width, Height );
            BGR BGRTmp = new BGR();
            HSI HSITmp;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            int X, Y, T;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        BGRTmp[ T ] = SourcePointer[ 0 ];
                        ++SourcePointer;
                    }
                    ++SourcePointer;
                    HSITmp = BGRTmp.ToHSI();
                    Result[ 0 ][ Y, X ] = new AMComplex( HSITmp.H, 0 );
                    Result[ 1 ][ Y, X ] = new AMComplex( HSITmp.S, 0 );
                    Result[ 2 ][ Y, X ] = new AMComplex( HSITmp.I, 0 );
                }
            }
            Source.UnlockBits( SourceData );
            return Result;
        }

        static public BGR[,] Img2BGR( Bitmap Source ) {
            int Width = Source.Width, Height = Source.Height;
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BGR[,] Result = new BGR[ Height, Width ];
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            int X, Y, T;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Result[ Y, X ] = new BGR();
                    for( T = 0 ; T < 3 ; ++T ) {
                        Result[ Y, X ][ T ] = SourcePointer[ 0 ];
                        ++SourcePointer;
                    }
                    ++SourcePointer;
                }
            }
            Source.UnlockBits( SourceData );
            return Result;
        }
        static public Bitmap BGR2Img( BGR[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            Bitmap Result = new Bitmap( Width, Height );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            int X, Y, T;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    for( T = 0 ; T < 3 ; ++T ) {
                        ResultPointer[ 0 ] = DoubleToByte( Source[ Y, X ][ T ] );
                        ++ResultPointer;
                    }
                    ResultPointer[ 0 ] = 255;
                    ++ResultPointer;
                }
            }
            Result.UnlockBits( ResultData );
            return Result;
        }

        static public HSIComplexImg BGR2HSI( BGRComplexImg Source ) {
            HSIComplexImg Result = new HSIComplexImg( Source.Width, Source.Height );
            BGR TmpBGR = new BGR();
            HSI TmpHSI = null;
            int X, Y;
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    TmpBGR.R = DoubleToByte( Source.R[ Y, X ].Re );
                    TmpBGR.B = DoubleToByte( Source.B[ Y, X ].Re );
                    TmpBGR.G = DoubleToByte( Source.G[ Y, X ].Re );
                    TmpHSI = TmpBGR.ToHSI();
                    Result.H[ Y, X ].Re = TmpHSI.H;
                    Result.S[ Y, X ].Re = TmpHSI.S;
                    Result.I[ Y, X ].Re = TmpHSI.I;
                }
            }
            return Result;
        }
        static public BGRComplexImg HSI2BGR( HSIComplexImg Source ) {
            BGRComplexImg Result = new BGRComplexImg( Source.Width, Source.Height );
            HSI TmpHSI = new HSI();
            BGR TmpBGR = null;
            int X, Y;
            for( Y = 0 ; Y < Source.Height ; ++Y ) {
                for( X = 0 ; X < Source.Width ; ++X ) {
                    TmpHSI.H = Source.H[ Y, X ].Re;
                    TmpHSI.S = Source.S[ Y, X ].Re;
                    TmpHSI.I = Source.I[ Y, X ].Re;
                    TmpBGR = TmpHSI.ToBGR();
                    Result.R[ Y, X ].Re = TmpBGR.R;
                    Result.G[ Y, X ].Re = TmpBGR.G;
                    Result.B[ Y, X ].Re = TmpBGR.B;
                }
            }
            return Result;
        }
        static public HSI[,] BGR2HSI( BGR[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            HSI[,] Result = new HSI[ Height, Width ];
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Result[ Y, X ] = Source[ Y, X ].ToHSI();
                }
            }
            return Result;
        }
        static public BGR[,] HSI2BGR( HSI[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            BGR[,] Result = new BGR[ Height, Width ];
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Result[ Y, X ] = Source[ Y, X ].ToBGR();
                }
            }
            return Result;
        }
        static public HSI BGR2HSI( BGR Source ) {
            double R = (double) Source.R / 255;
            double G = (double) Source.G / 255;
            double B = (double) Source.B / 255;
            double Max = Math.Max( R, Math.Max( G, B ) );
            double Min = Math.Min( R, Math.Min( G, B ) );
            double I = ( R + G + B ) / 3f;
            double H, S;
            if( Max == Min ) {
                H = 0f;
                S = 0f;
            } else {
                double C = Max - Min;
                if( Max == R ) {
                    H = ( G - B ) / C;
                } else if( Max == G ) {
                    H = ( B - R ) / C + 2f;
                } else {
                    H = ( R - G ) / C + 4f;
                }
                H *= 60f;
                if( H < 0f ) {
                    H += 360f;
                }
                S = 1 - Min / I;
            }
            return new HSI( H, S, I );
        }
        static public BGR HSI2BGR( HSI Source ) {
            double R, G, B;
            const double P = Math.PI / 180.0;
            double I = Source.I;
            double S = Source.S;
            double H = Source.H;
            double SI = Source.I * Source.S;
            if( H < 120 ) {
                double cos1 = Math.Cos( H * P );
                double cos2 = Math.Cos( ( 60 - H ) * P );
                R = I + SI * cos1 / cos2;
                B = I - SI;
                G = 3 * I - R - B;
            } else if( H < 240 ) {
                double cos1 = Math.Cos( ( H - 120 ) * P );
                double cos2 = Math.Cos( ( 180 - H ) * P );
                R = I - SI;
                G = I + SI * cos1 / cos2;
                B = 3 * I - R - G;
            } else if( H < 360 ) {
                double cos1 = Math.Cos( ( H - 240 ) * P );
                double cos2 = Math.Cos( ( 300 - H ) * P );
                G = I - SI;
                B = I + SI * cos1 / cos2;
                R = 3 * I - G - B;
            } else {
                throw new ArgumentException( "輸入HSI不正確", "HSI" );
            }
            R *= 255;
            G *= 255;
            B *= 255;
            return new BGR( DoubleToByte( R ), DoubleToByte( G ), DoubleToByte( B ) );
        }

        static public void Ln( AMComplex[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Source[ Y, X ].Re = Math.Log( Source[ Y, X ].Re + 1 );
                }
            }
        }
        static public void Exp( AMComplex[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Source[ Y, X ].Re = Math.Exp( Source[ Y, X ].Re ) - 1;
                }
            }
        }
        static public void Divide( AMComplex[,] Source, double D = 255 ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Source[ Y, X ].Re /= D;
                }
            }
        }
        static public void BGRRange( AMComplex[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            int X, Y;
            double Max = 0, Min = 255;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Max = Max < Source[ Y, X ].Re ? Source[ Y, X ].Re : Max;
                    Min = Min < Source[ Y, X ].Re ? Min : Source[ Y, X ].Re;
                }
            }
            Max -= Min;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Source[ Y, X ].Re -= Min;
                    Source[ Y, X ].Re *= 255;
                    Source[ Y, X ].Re /= Max;
                }
            }
        }
        static public double Max( AMComplex[,] Source ) {
            int X, Y, Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            double M = 0;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    M = M < Source[ Y, X ].Magnitude ? Source[ Y, X ].Magnitude : M;
                }
            }
            return M;
        }
        static public double Max( BGRComplexImg Source, BGRModel Model ) {
            int X, Y, Width = Source.Width, Height = Source.Height;
            double M = 0;
            int I = (int) Model / 2;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    M = M < Source[ I ][ Y, X ].Magnitude ? Source[ I ][ Y, X ].Magnitude : M;
                }
            }
            return M;
        }
        static public double Max( EDSPComplex[] Source ) {
            int T, L = Source.Length;
            double M = 0;
            for( T = 0 ; T < L ; ++T ) {
                if( M < Source[ T ].GetModulus() ) {
                    M = Source[ T ].GetModulus();
                }
            }
            return M;
        }

        static public ImageFormat ChooseImgFormat( string StringFormat ) {
            switch( StringFormat.ToUpper() ) {
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
        static public Bitmap ImgSmaller( Bitmap Source, int SWidth, int SHeight ) {
            int OWidth = Source.Width;
            int OHeight = Source.Height;
            double Smaller = 0;
            if( OWidth > OHeight ) {
                Smaller = (double) SWidth / OWidth;
            } else {
                Smaller = (double) SHeight / OHeight;
            }
            int AWidth = (int) ( OWidth * Smaller );
            int AHeight = (int) ( OHeight * Smaller );
            Bitmap Result = new Bitmap( AWidth, AHeight );
            using( var G = Graphics.FromImage( Result ) ) {
                G.DrawImage( Source, new Rectangle( 0, 0, AWidth, AHeight ), new Rectangle( 0, 0, OWidth, OHeight ), GraphicsUnit.Pixel );
            }
            return Result;
        }
        static public byte DoubleToByte( double Source ) {
            if( Source > 255 ) {
                return 255;
            } else if( Source < 0 ) {
                return 0;
            }
            return (byte) Source;
        }

        static public Bitmap SimpleKernel( int Width, int Height, float Length ) {
            Bitmap Result = new Bitmap( Width, Height );
            using( Graphics GraphicsResult = Graphics.FromImage( Result ) ) {
                GraphicsResult.Clear( Color.Black );
                float X = ( Width - Length ) / 2, Y = Height / 2;
                GraphicsResult.DrawLine( new Pen( Color.White, 1 ), X, Y, X + Length, Y );
            }
            return Result;
        }
        static public AMComplex[,] SimpleComplexKernel( int Width, int Height, float Length, bool Compress = false ) {
            AMComplex[,] Result = new AMComplex[ Height, Width ];
            int X, Y;
            int CX = (int) ( ( Width - Length ) / 2 ), CY = Height / 2;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    if( Y == CY && CX <= X && X < CX + Length ) {
                        if( Compress ) {
                            Result[ Y, X ] = new AMComplex( 1, 0 );
                        } else {
                            Result[ Y, X ] = new AMComplex( 255, 0 );
                        }
                    } else {
                        Result[ Y, X ] = new AMComplex( 0, 0 );
                    }
                }
            }
            return Result;
        }

        static public EDSPComplex[] AM2EDSP( AMComplex[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            EDSPComplex[] Result = new EDSPComplex[ Width * Height ];
            int Y, X, T = 0;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    Result[ T ] = new EDSPComplex( Source[ Y, X ].Re, Source[ Y, X ].Im );
                    ++T;
                }
            }
            return Result;
        }
        #endregion
        #region Private
        static private Bitmap ImgExtend( Bitmap Source, bool Zero = false ) {
            int Width = Source.Width, Height = Source.Height;
            int EWidth = (int) Math.Pow( 2, AForge.Math.Tools.Log2( Width ) ), EHeight = (int) Math.Pow( 2, AForge.Math.Tools.Log2( Height ) );
            BGR[,] BGRSource = Img2BGR( Source );
            Bitmap Result = new Bitmap( EWidth, EHeight, PixelFormat.Format32bppArgb );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, EWidth, EHeight ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            int X, Y, T;
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            int StartX = ( EWidth - Width ) / 2, StartY = ( EHeight - Height ) / 2;
            int EndX = StartX + Width, EndY = StartY + Height;
            int TmpX, TmpY;
            int TTmpX, TTmpY;
            for( Y = 0 ; Y < EHeight ; ++Y ) {
                for( X = 0 ; X < EWidth ; ++X ) {
                    TmpX = X - StartX;
                    TmpY = Y - StartY;
                    TTmpX = X - StartX;
                    TTmpY = Y - StartY;

                    while( TmpX < 0 ) {
                        TmpX += Width;
                    }
                    while( TmpY < 0 ) {
                        TmpY += Height;
                    }
                    while( TmpX >= Width ) {
                        TmpX -= Width;
                    }
                    while( TmpY >= Height ) {
                        TmpY -= Height;
                    }

                    for( T = 0 ; T < 3 ; ++T ) {
                        if( Zero && ( TTmpX < 0 || TTmpX >= Width || TTmpY < 0 || TTmpY >= Height ) ) {
                            ResultPointer[ 0 ] = 0;
                        } else {
                            ResultPointer[ 0 ] = DoubleToByte( BGRSource[ TmpY, TmpX ][ T ] );
                        }
                        ++ResultPointer;
                    }
                    ResultPointer[ 0 ] = 255;
                    ++ResultPointer;
                }
            }
            Result.UnlockBits( ResultData );
            return Result;
        }
        static private Bitmap ImgUnExtend( Bitmap Source, int OWidth, int OHeight ) {
            int Width = Source.Width, Height = Source.Height;
            Bitmap Result = new Bitmap( OWidth, OHeight );
            BitmapData SourceData = Source.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            BitmapData ResultData = Result.LockBits( new Rectangle( 0, 0, OWidth, OHeight ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            int X, Y, T;
            byte* SourcePointer = (byte*) SourceData.Scan0.ToPointer();
            byte* ResultPointer = (byte*) ResultData.Scan0.ToPointer();
            int StartX = ( Width - OWidth ) / 2, StartY = ( Height - OHeight ) / 2;
            int EndX = StartX + OWidth, EndY = StartY + OHeight;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    if( X >= StartX && Y >= StartY && X < EndX && Y < EndY ) {
                        for( T = 0 ; T < 4 ; ++T ) {
                            ResultPointer[ 0 ] = SourcePointer[ 0 ];
                            ++SourcePointer;
                            ++ResultPointer;
                        }
                    } else {
                        SourcePointer += 4;
                    }
                }
            }
            Source.UnlockBits( SourceData );
            Result.UnlockBits( ResultData );
            return Result;
        }
        static public void BFFTShift( AMComplex[,] Source ) {
            int Height = Source.GetLength( 0 ), Width = Source.GetLength( 1 );
            int X, Y;
            for( Y = 0 ; Y < Height ; ++Y ) {
                for( X = 0 ; X < Width ; ++X ) {
                    if( ( ( X + Y ) & 0x1 ) != 0 ) {
                        Source[ Y, X ].Re *= -1;
                        Source[ Y, X ].Im *= -1;
                    }
                }
            }
        }
        static public double Cmp( double SA, double SB, double SC ) {
            if( SA >= SB && SA >= SC ) {
                return SA;
            } else if( SB >= SC && SB >= SA ) {
                return SB;
            } else if( SC >= SA && SC >= SB ) {
                return SC;
            }
            return SA;
        }
        #endregion
    }
}
