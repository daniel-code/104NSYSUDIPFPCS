using System;
using System.Drawing;
using DIPHua;

namespace Gaussianlowpass {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 5 ) {
                    BinaryImage B = (BinaryImage) Convert.ToInt32( args[ 3 ] );
                    double D0 = Convert.ToDouble( args[ 4 ] );
                    Bitmap Source = new Bitmap( args[ 0 ] );
                    Bitmap Result = ImgF.Gaussianpass( Source, D0, GaussianPass.High, B );
                    Result.Save( args[ 1 ], ImgF.ChooseImageFormat( args[ 2 ] ) );
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
