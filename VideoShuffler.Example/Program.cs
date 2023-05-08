using Myitian.XShuffle;
using Myitian.VideoShuffler;
using System.Diagnostics;
using System.Drawing;

namespace VideoShuffler.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MODE");
            string mode = Console.ReadLine()?.Trim().Trim('"').Trim();
            bool mode_r = mode == "r";
            Console.WriteLine("IN");
            string inputFile = Console.ReadLine()?.Trim().Trim('"').Trim();
            Console.WriteLine("OUT");
            string outputFile = Console.ReadLine()?.Trim().Trim('"').Trim();
            

            Process ffprobe = new Process()
            {
                StartInfo = new ProcessStartInfo("ffprobe", $"-v warning -show_streams -select_streams v:0 \"{inputFile}\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            ffprobe.Start();
            bool w = false, h = false, f = false;
            int width = 0;
            int height = 0;
            string frame_rate = null;
            string s;
            string[] sp;
            string ffp_out = ffprobe.StandardOutput.ReadLine();
            while (ffp_out != null)
            {
                switch (ffp_out[0])
                {
                    case 'w' when ffp_out.StartsWith("width="):
                        int.TryParse(ffp_out.Split('=')[1], out width);
                        w = true;
                        break;
                    case 'h' when ffp_out.StartsWith("height="):
                        int.TryParse(ffp_out.Split('=')[1], out height);
                        h = true;
                        break;
                    case 'r' when frame_rate is null && ffp_out.StartsWith("r_frame_rate="):
                        s = ffp_out.Split('=')[1];
                        sp = s.Split('/');
                        if (sp[0] != "0" && sp[1] != "0")
                        {
                            frame_rate = s;
                        }
                        break;
                    case 'a' when ffp_out.StartsWith("avg_frame_rate="):
                        s = ffp_out.Split('=')[1];
                        sp = s.Split('/');
                        if (sp[0] != "0" && sp[1] != "0")
                        {
                            frame_rate = s;
                            f = true;
                        }
                        break;
                }
                if (w && h && f)
                {
                    break;
                }
                ffp_out = ffprobe.StandardOutput.ReadLine();
            }
            int byte_per_pixel = 4;
            int stride = width * byte_per_pixel;

            XShuffle xs = new XShuffle();
            Size vid_size = new Size(width, height);
            VideoShuffle vs120s = new VideoShuffle(vid_size, new Rectangle(0, 0, 120, height), new Size(120, 120));
            VideoShuffle vs60s = new VideoShuffle(vid_size, new Rectangle(120, 0, 120, height), new Size(60, 60));
            VideoShuffle vs40s = new VideoShuffle(vid_size, new Rectangle(240, 0, 120, height), new Size(40, 40));
            VideoShuffle vs20s = new VideoShuffle(vid_size, new Rectangle(360, 0, 120, height), new Size(20, 20));
            VideoShuffle vs10s = new VideoShuffle(vid_size, new Rectangle(480, 0, 120, height), new Size(10, 10));
            VideoShuffle vs5s = new VideoShuffle(vid_size, new Rectangle(600, 0, 120, height), new Size(5, 5));
            VideoShuffle vs2s = new VideoShuffle(vid_size, new Rectangle(720, 0, 120, height), new Size(2, 2));
            VideoShuffle vs1s = new VideoShuffle(vid_size, new Rectangle(840, 0, 120, height), new Size(1, 1));
            VideoShuffle vs120d = new VideoShuffle(vid_size, new Rectangle(960, 0, 120, height), new Size(120, 120));
            VideoShuffle vs60d = new VideoShuffle(vid_size, new Rectangle(1080, 0, 120, height), new Size(60, 60));
            VideoShuffle vs40d = new VideoShuffle(vid_size, new Rectangle(1200, 0, 120, height), new Size(40, 40));
            VideoShuffle vs20d = new VideoShuffle(vid_size, new Rectangle(1320, 0, 120, height), new Size(20, 20));
            VideoShuffle vs10d = new VideoShuffle(vid_size, new Rectangle(1440, 0, 120, height), new Size(10, 10));
            VideoShuffle vs5d = new VideoShuffle(vid_size, new Rectangle(1560, 0, 120, height), new Size(5, 5));
            VideoShuffle vs2d = new VideoShuffle(vid_size, new Rectangle(1680, 0, 120, height), new Size(2, 2));
            VideoShuffle vs1d = new VideoShuffle(vid_size, new Rectangle(1800, 0, 120, height), new Size(1, 1));

            string dec_args = $"-i \"{inputFile}\" -v warning -pix_fmt bgra -f rawvideo -an -sn -";
            Console.WriteLine(dec_args);
            Process ffmpeg_dec = new Process()
            {
                StartInfo = new ProcessStartInfo("ffmpeg", dec_args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            string enc_args = $"-pix_fmt bgra -f rawvideo -s {width}x{height} -r {frame_rate} -i - -vn -i \"{inputFile}\" -v warning -c:a copy -y \"{outputFile}\"";
            Console.WriteLine(enc_args);
            Process ffmpeg_enc = new Process()
            {
                StartInfo = new ProcessStartInfo("ffmpeg", enc_args)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true
                }
            };

            ffmpeg_dec.Start();
            Stream dec_out = ffmpeg_dec.StandardOutput.BaseStream;
            ffmpeg_enc.Start();
            Stream enc_in = ffmpeg_enc.StandardInput.BaseStream;

            Action<IShuffleable, int, int?, uint?> func;
            if (mode_r)
                func = xs.ReversedShuffle;
            else
                func = xs.Shuffle;

            int index = 0;
            int prev = 0;

            int bufsize = stride * height;
            byte[] buffer = new byte[bufsize];
            int buf_index = 0;
            int read = dec_out.ReadByte();
            uint s_seed = xs.Seed;
            uint d_seed = xs.Seed;
            Frame frame;

            Stopwatch sw = Stopwatch.StartNew();
            Stopwatch sw_avg = Stopwatch.StartNew();
            double freq = Stopwatch.Frequency;

            while (read >= 0)
            {
                buffer[buf_index++] = (byte)read;
                if (buf_index == bufsize)
                {
                    if (sw.ElapsedTicks >= Stopwatch.Frequency)
                    {
                        sw.Stop();
                        int delta = index - prev;
                        double d = freq * delta / sw.ElapsedTicks;
                        double avg = freq * index / sw_avg.ElapsedTicks;
                        Console.WriteLine($"f: {index}, delta: {delta}, speed: [ {d} fps / {avg} fps(avg) ]");
                        prev = index;
                        sw.Restart();
                    }

                    frame = vs120s.LoadFrame(buffer);
                    func(frame, 0, null, s_seed);
                    vs60s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs40s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs20s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs10s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs5s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs2s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs1s.LoadFrame(frame);
                    func(frame, 0, null, s_seed);
                    vs120d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs60d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs40d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs20d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs10d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs5d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs2d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);
                    vs1d.LoadFrame(frame);
                    func(frame, 0, null, d_seed);


                    enc_in.Write(buffer, 0, bufsize);
                    buf_index = 0;
                    d_seed++;
                    index++;
                }
                read = dec_out.ReadByte();
            }
            enc_in.Close();

            ffmpeg_enc.WaitForExit();
            ffmpeg_dec.WaitForExit();
            ffprobe.WaitForExit();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}