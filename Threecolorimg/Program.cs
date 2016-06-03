using System;
using System.Drawing;
using DIPHua;

namespace Threecolorimg {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 4 ) {
                    HColor HC = (HColor) ( Convert.ToInt32( args[ 3 ] ) );
                    Bitmap Source = new Bitmap( args[ 0 ] );
                    Bitmap Result = ImgF.Threecolorimg( Source, HC );
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
