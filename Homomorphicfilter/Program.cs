using System;
using System.Drawing;
using DIPHua2;

namespace Homomorphicfilter {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 7 ) {
                    Bitmap Source = new Bitmap( args[ 0 ] );
                    double GammaHigh = Convert.ToDouble( args[ 3 ] );
                    double GammaLow = Convert.ToDouble( args[ 4 ] );
                    double C = Convert.ToDouble( args[ 5 ] );
                    double D0 = Convert.ToDouble( args[ 6 ] );
                    Bitmap Result = ImgF.HomomorphicFilter( Source, GammaHigh, GammaLow, C, D0 );
                    Result.Save( args[ 1 ], ImgF.ChooseImgFormat( args[ 2 ] ) );
                    Environment.Exit( 0 );
                } else {
                    Environment.Exit( 1 );
                }
            } catch(Exception ex) {
                Console.WriteLine( ex.ToString() );
                Environment.Exit( 2 );
            }
        }
    }
}
