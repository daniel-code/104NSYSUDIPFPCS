using AMComplex = AForge.Math.Complex;
using AMFT = AForge.Math.FourierTransform;

namespace DIPHua2 {
    public class HSI {
        public HSI() {
        }
        public HSI( double H, double S, double I ) {
            this.H = H;
            this.S = S;
            this.I = I;
        }
        public double this[ int Index ] {
            get {
                if( Index == 0 ) {
                    return H;
                } else if( Index == 1 ) {
                    return S;
                } else if( Index == 2 ) {
                    return I;
                } else {
                    return 0;
                }
            }
            set {
                if( Index == 0 ) {
                    H = value;
                } else if( Index == 1 ) {
                    S = value;
                } else if( Index == 2 ) {
                    I = value;
                }
            }
        }
        static public HSI FromBGR( BGR Source ) {
            return ImgF.BGR2HSI( Source );
        }
        static public HSI FromBGR( byte R, byte G, byte B ) {
            return FromBGR( new BGR( R, G, B ) );
        }
        public BGR ToBGR() {
            return ImgF.HSI2BGR( this );
        }
        public double I255 {
            get {
                return I * 255D;
            }
        }
        public double H = 0;
        public double S = 0;
        public double I = 0;
        public override string ToString() {
            return "(" + H + "," + S + " ," + I * 255 + " )";
        }
    }
    public class HSIComplexImg {
        public HSIComplexImg( int Width, int Height ) {
            _H = new AMComplex[ Height, Width ];
            _S = new AMComplex[ Height, Width ];
            _I = new AMComplex[ Height, Width ];
            this._Width = Width;
            this._Height = Height;
        }
        public AMComplex[,] this[ int Index ] {
            get {
                if( Index == 0 ) {
                    return _H;
                } else if( Index == 1 ) {
                    return _S;
                } else if( Index == 2 ) {
                    return _I;
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
        public AMComplex[,] H {
            get {
                return _H;
            }
        }
        public AMComplex[,] S {
            get {
                return _S;
            }
        }
        public AMComplex[,] I {
            get {
                return _I;
            }
        }
        private int _Width = 0;
        private int _Height = 0;
        private AMComplex[,] _H = null;
        private AMComplex[,] _S = null;
        private AMComplex[,] _I = null;
        public void BFFTShift() {
            BFFTShift( HSIModel.H | HSIModel.S | HSIModel.I );
        }
        public void FFT2() {
            FFT2( HSIModel.H | HSIModel.S | HSIModel.I );
        }
        public void IFFT2() {
            IFFT2( HSIModel.H | HSIModel.S | HSIModel.I );
        }
        public void BFFTShift( HSIModel Type ) {
            if( ( Type & HSIModel.H ) == HSIModel.H ) {
                ImgF.BFFTShift( _H );
            }
            if( ( Type & HSIModel.S ) == HSIModel.S ) {
                ImgF.BFFTShift( _S );
            }
            if( ( Type & HSIModel.I ) == HSIModel.I ) {
                ImgF.BFFTShift( _I );
            }
        }
        public void FFT2( HSIModel Type ) {
            if( ( Type & HSIModel.H ) == HSIModel.H ) {
                AMFT.FFT2( _H, AMFT.Direction.Forward );
            }
            if( ( Type & HSIModel.S ) == HSIModel.S ) {
                AMFT.FFT2( _S, AMFT.Direction.Forward );
            }
            if( ( Type & HSIModel.I ) == HSIModel.I ) {
                AMFT.FFT2( _I, AMFT.Direction.Forward );
            }
        }
        public void IFFT2( HSIModel Type ) {
            if( ( Type & HSIModel.H ) == HSIModel.H ) {
                AMFT.FFT2( _H, AMFT.Direction.Backward );
            }
            if( ( Type & HSIModel.S ) == HSIModel.S ) {
                AMFT.FFT2( _S, AMFT.Direction.Backward );
            }
            if( ( Type & HSIModel.I ) == HSIModel.I ) {
                AMFT.FFT2( _I, AMFT.Direction.Backward );
            }
        }
    }
}
