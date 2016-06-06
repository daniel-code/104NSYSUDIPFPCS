using System;
using DIPHua3;

namespace Magnitudedisplay {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 3 ) {
                    string RPath = args[ 0 ];
                    string WPath = args[ 1 ];
                    string WExt = args[ 2 ];
                    BGRImg Source = BGRImg.From( RPath );
                    BGRImg Result = Source.Extend();
                    Result.BFFTShift();
                    Result.FFT2();
                    Result.ToImage( true ).Save( WPath, ImgF.ChooseImgFormat( WExt ) );
                    Environment.Exit( 0 );
                } else {
                    Environment.Exit( 1 );
                }
            } catch( Exception ex ) {
                Console.WriteLine( ex.ToString() );
                Environment.Exit( 2 );
            }
        }
    }
}
