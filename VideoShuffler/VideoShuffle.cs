using System;
using System.Drawing;

namespace Myitian.VideoShuffler
{
    public class VideoShuffle
    {
        private int frameWidth;
        private int frameHeight;
        private int shuffleAreaX;
        private int shuffleAreaY;
        private int shuffleAreaWidth;
        private int shuffleAreaHeight;
        private int unitWidth = 1;
        private int unitHeight = 1;
        private int bytesPerPixel = 4;

        private int frameWidthXBPP;
        private int shuffleAreaXXBPP;
        private int unitWidthXBPP;

        public bool Checked { get; private set; }

        public int FrameWidth
        {
            get => frameWidth;
            set
            {
                Checked = false;
                frameWidth = value;
            }
        }
        public int FrameHeight
        {
            get => frameHeight;
            set
            {
                Checked = false;
                frameHeight = value;
            }
        }
        public int ShuffleAreaX
        {
            get => shuffleAreaX;
            set
            {
                Checked = false;
                shuffleAreaX = value;
            }
        }
        public int ShuffleAreaY
        {
            get => shuffleAreaY;
            set
            {
                Checked = false;
                shuffleAreaY = value;
            }
        }
        public int ShuffleAreaWidth
        {
            get => shuffleAreaWidth;
            set
            {
                Checked = false;
                shuffleAreaWidth = value;
            }
        }
        public int ShuffleAreaHeight
        {
            get => shuffleAreaHeight;
            set
            {
                Checked = false;
                shuffleAreaHeight = value;
            }
        }
        public int UnitWidth
        {
            get => unitWidth;
            set
            {
                Checked = false;
                unitWidth = value;
            }
        }
        public int UnitHeight
        {
            get => unitHeight;
            set
            {
                Checked = false;
                unitHeight = value;
            }
        }
        public int BytesPerPixel
        {
            get => bytesPerPixel;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                bytesPerPixel = value;
            }
        }

        public int FrameSize { get; private set; }
        public int ShuffleAreaStartIndex { get; private set; }
        public int UnitLineOffset { get; private set; }
        public int UnitCountX { get; private set; }
        public int UnitCountY { get; private set; }
        public int UnitCount { get; private set; }

        public VideoShuffle(Size frameSize) : this(frameSize, new Rectangle(0, 0, frameSize.Width, frameSize.Height)) { }
        public VideoShuffle(Size frameSize, Rectangle shuffleArea) : this(frameSize, shuffleArea, new Size(1, 1)) { }
        public VideoShuffle(Size frameSize, Size unitSize) : this(frameSize, new Rectangle(0, 0, frameSize.Width, frameSize.Height), unitSize) { }
        public VideoShuffle(Size frameSize, Rectangle shuffleArea, Size unitSize)
        {
            FrameWidth = frameSize.Width;
            FrameHeight = frameSize.Height;
            ShuffleAreaX = shuffleArea.X;
            ShuffleAreaY = shuffleArea.Y;
            ShuffleAreaWidth = shuffleArea.Width;
            ShuffleAreaHeight = shuffleArea.Height;
            UnitWidth = unitSize.Width;
            UnitHeight = unitSize.Height;
            ApplyChanges();
        }

        public void ApplyChanges()
        {
            if (frameWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameWidth));
            if (frameHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameHeight));
            if (shuffleAreaX < 0 || shuffleAreaX >= frameWidth)
                throw new ArgumentOutOfRangeException(nameof(shuffleAreaX));
            if (shuffleAreaY < 0 || shuffleAreaY >= frameHeight)
                throw new ArgumentOutOfRangeException(nameof(shuffleAreaY));
            if (shuffleAreaWidth < 0 || shuffleAreaWidth > frameWidth - shuffleAreaX)
                throw new ArgumentOutOfRangeException(nameof(shuffleAreaWidth));
            if (shuffleAreaHeight < 0 || shuffleAreaHeight > frameHeight - shuffleAreaY)
                throw new ArgumentOutOfRangeException(nameof(shuffleAreaHeight));
            if (unitWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(unitWidth));
            if (unitHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(unitHeight));
            if (shuffleAreaWidth % unitWidth != 0)
                throw new ArgumentException("shuffleAreaWidth % unitWidth != 0");
            if (shuffleAreaHeight % unitHeight != 0)
                throw new ArgumentException("shuffleAreaHeight % unitHeight != 0");

            frameWidthXBPP = frameWidth * bytesPerPixel;
            shuffleAreaXXBPP = shuffleAreaX * bytesPerPixel;
            unitWidthXBPP = unitWidth * bytesPerPixel;

            FrameSize = frameWidthXBPP * frameHeight;
            UnitCountX = shuffleAreaWidth / unitWidth;
            UnitCountY = shuffleAreaHeight / unitHeight;
            UnitLineOffset = unitHeight * frameWidthXBPP;
            ShuffleAreaStartIndex = shuffleAreaXXBPP + shuffleAreaY * frameWidthXBPP;
            UnitCount = UnitCountX * UnitCountY;

            Checked = true;
        }

        public Frame LoadFrame(byte[] frameData)
        {
            if (frameData.Length < FrameSize)
                throw new ArgumentException("The length of frameData is less than FrameSize");
            return new Frame(frameData, frameWidthXBPP, ShuffleAreaStartIndex, UnitLineOffset, UnitCountX, UnitCount, unitWidthXBPP, unitHeight);
        }

        public void LoadFrame(Frame frame, byte[] frameData)
        {
            if (frameData.Length < FrameSize)
                throw new ArgumentException("The length of frameData is less than FrameSize");
            frame.SetData(frameData, frameWidthXBPP, ShuffleAreaStartIndex, UnitLineOffset, UnitCountX, UnitCount, unitWidthXBPP, unitHeight);
        }

        public void LoadFrame(Frame frame)
        {
            if (frame.FrameData.Length < FrameSize)
                throw new ArgumentException("The length of frameData is less than FrameSize");
            frame.SetData(frameWidthXBPP, ShuffleAreaStartIndex, UnitLineOffset, UnitCountX, UnitCount, unitWidthXBPP, unitHeight);
        }
    }
}
