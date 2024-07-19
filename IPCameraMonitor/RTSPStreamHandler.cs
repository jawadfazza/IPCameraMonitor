using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FFmpeg.AutoGen;

public class RTSPStreamHandler
{
    private readonly string _url;
    private readonly PictureBox _pictureBox;
    private bool _isRunning;
    private Thread _thread;
    private Bitmap _bitmap;
    private object _lock = new object();

    public RTSPStreamHandler(string url, PictureBox pictureBox)
    {
        _url = url;
        _pictureBox = pictureBox;
        FFmpegHelper.Initialize();
    }

    public void Start()
    {
        _isRunning = true;
        _thread = new Thread(DecodeStream);
        _thread.IsBackground = true;
        _thread.Start();
    }

    public void Stop()
    {
        _isRunning = false;
        _thread?.Join();
    }

    private unsafe void DecodeStream()
    {
        AVFormatContext* pFormatContext = ffmpeg.avformat_alloc_context();

        // Open video file
        if (ffmpeg.avformat_open_input(&pFormatContext, _url, null, null) != 0)
        {
            Console.WriteLine("Could not open input stream.");
            return;
        }

        // Retrieve stream information
        if (ffmpeg.avformat_find_stream_info(pFormatContext, null) != 0)
        {
            Console.WriteLine("Could not find stream information.");
            ffmpeg.avformat_close_input(&pFormatContext);
            return;
        }

        // Find the first video stream
        AVCodec* pCodec = null;
        AVCodecParameters* pCodecParameters = null;
        int videoStreamIndex = -1;

        for (int i = 0; i < pFormatContext->nb_streams; i++)
        {
            AVStream* stream = pFormatContext->streams[i];
            AVCodecParameters* codecParameters = stream->codecpar;
            AVCodec* codec = ffmpeg.avcodec_find_decoder(codecParameters->codec_id);

            if (codec->type == AVMediaType.AVMEDIA_TYPE_VIDEO)
            {
                videoStreamIndex = i;
                pCodec = codec;
                pCodecParameters = codecParameters;
                break;
            }
        }

        if (videoStreamIndex == -1)
        {
            Console.WriteLine("Could not find video stream.");
            ffmpeg.avformat_close_input(&pFormatContext);
            return;
        }

        // Get a pointer to the codec context for the video stream
        AVCodecContext* pCodecContext = ffmpeg.avcodec_alloc_context3(pCodec);
        if (ffmpeg.avcodec_parameters_to_context(pCodecContext, pCodecParameters) < 0)
        {
            Console.WriteLine("Could not copy codec parameters to codec context.");
            ffmpeg.avformat_close_input(&pFormatContext);
            return;
        }

        if (ffmpeg.avcodec_open2(pCodecContext, pCodec, null) < 0)
        {
            Console.WriteLine("Could not open codec.");
            ffmpeg.avcodec_free_context(&pCodecContext);
            ffmpeg.avformat_close_input(&pFormatContext);
            return;
        }

        AVFrame* pFrame = ffmpeg.av_frame_alloc();
        AVPacket* pPacket = ffmpeg.av_packet_alloc();

        while (_isRunning)
        {
            if (ffmpeg.av_read_frame(pFormatContext, pPacket) < 0)
            {
                break;
            }

            if (pPacket->stream_index == videoStreamIndex)
            {
                if (ffmpeg.avcodec_send_packet(pCodecContext, pPacket) == 0)
                {
                    while (ffmpeg.avcodec_receive_frame(pCodecContext, pFrame) == 0)
                    {
                        // Use double buffering for smoother updates
                        Bitmap originalBitmap = new Bitmap(pFrame->width, pFrame->height, PixelFormat.Format24bppRgb);
                        var bitmapData = originalBitmap.LockBits(
                            new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format24bppRgb);

                        var src = pFrame->data[0];
                        var dest = (byte*)bitmapData.Scan0;

                        for (var y = 0; y < pFrame->height; y++)
                        {
                            Buffer.MemoryCopy(src, dest, bitmapData.Stride, pFrame->linesize[0]);
                            src += pFrame->linesize[0];
                            dest += bitmapData.Stride;
                        }

                        originalBitmap.UnlockBits(bitmapData);

                        // Resize the bitmap
                        Bitmap resizedBitmap = ResizeBitmap(originalBitmap, _pictureBox.Width, _pictureBox.Height);

                        lock (_lock)
                        {
                            _bitmap?.Dispose();
                            _bitmap = resizedBitmap;
                        }

                        originalBitmap.Dispose();

                        // Invoke to update the PictureBox
                        _pictureBox.Invoke(new Action(() =>
                        {
                            lock (_lock)
                            {
                                _pictureBox.Image?.Dispose();
                                _pictureBox.Image = (Bitmap)_bitmap.Clone();
                            }
                        }));
                    }
                }
            }

            ffmpeg.av_packet_unref(pPacket);
        }

        ffmpeg.av_frame_free(&pFrame);
        ffmpeg.av_packet_free(&pPacket);
        ffmpeg.avcodec_free_context(&pCodecContext);
        ffmpeg.avformat_close_input(&pFormatContext);
    }

    private Bitmap ResizeBitmap(Bitmap originalBitmap, int width, int height)
    {
        Bitmap resizedBitmap = new Bitmap(width, height);
        using (var g = Graphics.FromImage(resizedBitmap))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalBitmap, 0, 0, width, height);
        }
        return resizedBitmap;
    }
}
