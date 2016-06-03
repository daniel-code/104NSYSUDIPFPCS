using System;
using System.Drawing;
using DIPHua2;

namespace Convolution {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 4 ) {
                    Bitmap SourceA = new Bitmap( args[ 0 ] );
                    Bitmap SourceB = new Bitmap( args[ 1 ] );
                    Bitmap Result = ImgF.Convolution( SourceA, SourceB );
                    Result.Save( args[ 2 ], ImgF.ChooseImgFormat( args[ 3 ] ) );
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
