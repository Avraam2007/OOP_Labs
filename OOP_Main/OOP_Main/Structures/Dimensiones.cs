using OOP_Main.Utilities;
using System;

namespace OOP_Main.Structures {
    public readonly struct Dimensions {
        public double Width { get; }
        public double Height { get; }
        public double Length { get; }

        public Dimensions(double width, double height, double length) {
            if (width <= 0) 
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(width), 
                    message: "Width should be positive"
                );
            if (height <= 0) 
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(height), 
                    message: "Height should be positive"
                );
            if (length <= 0) 
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(length), 
                    message: "Length should be positive"
                );


            Width = width.RoundNumber(3);
            Height = height.RoundNumber(3);
            Length = length.RoundNumber(3);
        }

        public Dimensions WithWidth(double width) => new Dimensions(width, Height, Length);
        public Dimensions WithHeight(double height) => new Dimensions(Width, height, Length);
        public Dimensions WithLength(double length) => new Dimensions(Width, Height, length);
    }
}
