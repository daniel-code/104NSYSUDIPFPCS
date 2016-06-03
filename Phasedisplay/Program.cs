using DIPHua;
using System;
using System.Drawing;

namespace Phasedisplay {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 3 ) {
                    Bitmap Source = new Bitmap( args[ 0 ] );
                    Bitmap Result = ImgF.Phasedisplay( Source );
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
