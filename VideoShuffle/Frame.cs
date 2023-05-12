namespace Myitian.Shuffling
{
    public class Frame : IShuffleable
    {
        int FrameWidth;
        int UnitWidth;
        int UnitHeight;
        int ShuffleAreaStartIndex;
        int UnitLineOffset;
        int UnitCountX;
        public byte[] FrameData { get; private set; }
        public int Length { get; private set; }

        public Frame(byte[] frameData, int frameWidth, int shuffleAreaStartIndex, int unitLineOffset, int unitCountX, int unitCount, int unitWidth = 1, int unitHeight = 1)
        {
            SetData(frameData, frameWidth, shuffleAreaStartIndex, unitLineOffset, unitCountX, unitCount, unitWidth, unitHeight);
        }

        public void SetData(byte[] frameData, int frameWidth, int shuffleAreaStartIndex, int unitLineOffset, int unitCountX, int unitCount, int unitWidth = 1, int unitHeight = 1)
        {
            FrameData = frameData;
            FrameWidth = frameWidth;
            UnitWidth = unitWidth;
            UnitHeight = unitHeight;
            ShuffleAreaStartIndex = shuffleAreaStartIndex;
            UnitLineOffset = unitLineOffset;
            UnitCountX = unitCountX;
            Length = unitCount;
        }

        public void SetData(int frameWidth, int shuffleAreaStartIndex, int unitLineOffset, int unitCountX, int unitCount, int unitWidth = 1, int unitHeight = 1)
        {
            FrameWidth = frameWidth;
            UnitWidth = unitWidth;
            UnitHeight = unitHeight;
            ShuffleAreaStartIndex = shuffleAreaStartIndex;
            UnitLineOffset = unitLineOffset;
            UnitCountX = unitCountX;
            Length = unitCount;
        }

        public void Swap(int index0, int index1)
        {
            int s0 = ShuffleAreaStartIndex + index0 / UnitCountX * UnitLineOffset + index0 % UnitCountX * UnitWidth;
            int s1 = ShuffleAreaStartIndex + index1 / UnitCountX * UnitLineOffset + index1 % UnitCountX * UnitWidth;

            for (int y = 0; y < UnitHeight; y++)
            {
                for (int x = 0; x < UnitWidth; x++)
                {
                    int s0a = s0 + x;
                    int s1a = s1 + x;
                    byte t = FrameData[s0a];
                    FrameData[s0a] = FrameData[s1a];
                    FrameData[s1a] = t;
                }
                s0 += FrameWidth;
                s1 += FrameWidth;
            }
        }
    }
}
