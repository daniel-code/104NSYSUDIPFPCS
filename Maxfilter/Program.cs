using System;
using System.Drawing;
using DIPHua;

namespace Maxfilter {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 4 ) {
                    int Range = Convert.ToInt32( args[ 3 ] );
                    if( Range % 2 == 0 ) {
                        Environment.Exit( 1 );
                    }
                    Bitmap Source = new Bitmap( args[ 0 ] );
                    Bitmap Result = ImgF.Maxfilter( Source, Range );
                    Result.Save( args[ 1 ], ImgF.ChooseImageFormat( args[ 2 ] ) );
                    Environment.Exit( 0 );
                } else {
                    Environment.Exit( 1 );
                }
            } catch {
                Environment.Exit( 2 );
            }
        }
    }
}
