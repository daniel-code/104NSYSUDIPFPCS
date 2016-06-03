using System;
using System.Drawing;
using DIPHua;

namespace Gaussianlowpass {
    class Program {
        static void Main( string[] args ) {
            try {
                if( args.Length >= 4 ) {
                    double D0 = Convert.ToDouble( args[ 3 ] );
                    Bitmap Source = new Bitmap( args[ 0 ] );
                    Bitmap Result = ImgF.Gaussianpass( Source, D0, GaussianPass.Low, BinaryImage.False );
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
