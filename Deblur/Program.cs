using DIPHua3;
using System;
using System.Drawing.Imaging;

namespace Deblur {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 4 ) {
                    string RPath = args[ 0 ];
                    string WPath = args[ 1 ];
                    string WExt = args[ 2 ];
                    double Length = Convert.ToDouble( args[ 3 ] );
                    double Lambda = Convert.ToDouble( args[ 4 ] );
                    BGRImg Source = BGRImg.From( RPath );
                    Source.SimpleMotionDeblur( Length, Lambda ).ToImage().Save( WPath, Funct.ChooseImgFormat( WExt ) );
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
