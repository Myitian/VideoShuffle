# VideoShuffle
Shuffle video frame

打乱视频帧

可打乱视频内局部区域

打乱单元可为矩形，边长需要满足：打乱区域边长被单元边长整除

具体效果见 [Bilibili视频](https://www.bilibili.com/video/BV1xg4y157sm/)

# Example
详细例子见 VideoShuffle.Example

```csharp
using Myitian.Shuffling;
using System.Drawing;

XShuffle xs = new XShuffle();
VideoShuffle vs = new VideoShuffle(new Size(1920, 1080), new Rectangle(0, 0, 120, 1080), new Size(120, 120));
byte[] buffer;

// ... read ffmpeg stdout, store a rawvideo frame in buffer ...

Frame frame = vs.LoadFrame(buffer); // load frame info
xs.Shuffle(frame); // shuffle frame

// ... write buffer to ffmpeg stdin ...

```