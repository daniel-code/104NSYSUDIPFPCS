using AMComplex = AForge.Math.Complex;
using AMFT = AForge.Math.FourierTransform;

namespace DIPHua2 {
    public class BGR {
        public BGR( ) {
        }
        public BGR( double R, double G, double B ) {
            this.R = R;
            this.G = G;
            this.B = B;
        }
        public double this[ int Index ] {
            get {
                if( Index == 0 ) {
                    return B;
                } else if( Index == 1 ) {
                    return G;
                } else if( Index == 2 ) {
                    return R;
                } else {
                    return 0;
                }
            }
            set {
                if( Index == 0 ) {
                    B = value;
                } else if( Index == 1 ) {
                    G = value;
                } else if( Index == 2 ) {
                    R = value;
                }
            }
        }
        static public BGR FromHSI( HSI Source ) {
            return ImgF.HSI2BGR( Source );
        }
        static public BGR FromHSI( double H, double S, double I ) {
            return FromHSI( new HSI( H, S, I ) );
        }
        public HSI ToHSI() {
            return ImgF.BGR2HSI( this );
        }
        public double Total {
            get {
                return R + G + B;
            }
        }
        public void ToGrayScale() {
            R = G = B = ImgF.DoubleToByte( Total / 3 );
        }
        public double R = 0;
        public double G = 0;
        public double B = 0;
        public override string ToString() {
            return "(" + R + "," + G + " ," + B + " )";
        }
    }
    public class BGRComplexImg {
        public BGRComplexImg( int Width, int Height ) {
            _R = new AMComplex[ Height, Width ];
            _G = new AMComplex[ Height, Width ];
            _B = new AMComplex[ Height, Width ];
            this._Width = Width;
            this._Height = Height;
        }
        public AMComplex[,] this[ int Index ] {
            get {
                if( Index == 0 ) {
                    return _B;
                } else if( Index == 1 ) {
                    return _G;
                } else if( Index == 2 ) {
                    return _R;
                } else {
                    return null;
                }
            }
        }
        public int Width {
            get {
                return _Width;
            }
        }
        public int Height {
            get {
                return _Height;
            }
        }
        public AMComplex[,] R {
            get {
                return _R;
            }
        }
        public AMComplex[,] G {
            get {
                return _G;
            }
        }
        public AMComplex[,] B {
            get {
                return _B;
            }
        }
        private int _Width = 0;
        private int _Height = 0;
        private AMComplex[,] _R = null;
        private AMComplex[,] _G = null;
        private AMComplex[,] _B = null;
        public void Ln() {
            Ln( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void Ln( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                ImgF.Ln( _R );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                ImgF.Ln( _G );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                ImgF.Ln( _B );
            }
        }
        public void Exp() {
            Exp( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void Exp( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                ImgF.Exp( _R );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                ImgF.Exp( _G );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                ImgF.Exp( _B );
            }
        }
        public void ToDouble() {
            ToDouble( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void ToDouble( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                ImgF.Divide( _R );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                ImgF.Divide( _G );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                ImgF.Divide( _B );
            }
        }
        public void Range() {
            Range( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void Range( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                ImgF.BGRRange( _R );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                ImgF.BGRRange( _G );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                ImgF.BGRRange( _B );
            }
        }
        public void BFFTShift() {
            BFFTShift( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void DFT2() {
            AMFT.DFT2( _R, AMFT.Direction.Forward );
            AMFT.DFT2( _G, AMFT.Direction.Forward );
            AMFT.DFT2( _B, AMFT.Direction.Forward );
        }
        public void IDFT2() {
            AMFT.DFT2( _R, AMFT.Direction.Backward );
            AMFT.DFT2( _G, AMFT.Direction.Backward );
            AMFT.DFT2( _B, AMFT.Direction.Backward );
        }
        public void FFT2() {
            FFT2( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void IFFT2() {
            IFFT2( BGRModel.R | BGRModel.G | BGRModel.B );
        }
        public void BFFTShift( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                ImgF.BFFTShift( _R );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                ImgF.BFFTShift( _G );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                ImgF.BFFTShift( _B );
            }
        }
        public void FFT2( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                AMFT.FFT2( _R, AMFT.Direction.Forward );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                AMFT.FFT2( _G, AMFT.Direction.Forward );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                AMFT.FFT2( _B, AMFT.Direction.Forward );
            }
        }
        public void IFFT2( BGRModel Type ) {
            if( ( Type & BGRModel.R ) == BGRModel.R ) {
                AMFT.FFT2( _R, AMFT.Direction.Backward );
            }
            if( ( Type & BGRModel.G ) == BGRModel.G ) {
                AMFT.FFT2( _G, AMFT.Direction.Backward );
            }
            if( ( Type & BGRModel.B ) == BGRModel.B ) {
                AMFT.FFT2( _B, AMFT.Direction.Backward );
            }
        }
    }
}
