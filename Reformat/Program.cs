using DIPHua;
using System;
using System.Drawing;

namespace Reformat {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length == 4 ) {
                    bool Run = Convert.ToBoolean( args[ 3 ] );
                    Bitmap Source = null;
                    if( Run ) {
                        Source = ImgF.ImageSmaller( new Bitmap( args[ 0 ] ) );
                    } else {
                        Source = new Bitmap( args[ 0 ] );
                    }
                    Source.Save( args[ 1 ], ImgF.ChooseImageFormat( args[ 2 ] ) );
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
