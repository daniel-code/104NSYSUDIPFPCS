using System;
using DIPHua3;

namespace Blur {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 4 ) {
                    float Length = Convert.ToSingle( args[ 3 ] );
                    BGRImg Source = BGRImg.From( args[ 0 ] );
                    Source.SimpleMotionBlur( Length ).ToImage().Save( args[ 1 ], Funct.ChooseImgFormat( args[ 2 ] ) );
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
