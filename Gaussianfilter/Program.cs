using System;
using DIPHua3;

namespace Gaussianfilter {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 3 ) {
                    string RPath = args[ 0 ];
                    string WPath = args[ 1 ];
                    string WExt = args[ 2 ];
                    BGRImg Source = BGRImg.From( RPath );
                    Source.GaussianFilter();
                    Source.ToImage().Save( WPath, ImgF.ChooseImgFormat( WExt ) );
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
