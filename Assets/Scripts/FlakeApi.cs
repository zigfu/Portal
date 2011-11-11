using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace FlakeSharp
{
	public enum FlakeOrderMethod : int
	{
		Max = 0,
		Est = 1,
		TwoLevel = 2,
		FourLevel = 3,
		EightLevel = 4,
		Search = 5,
		Log = 6,
	}
	
	public enum FlakeStereoMethod : int
	{
		Independent = 0,
		Estimate = 1,
	}
	
	public enum FlakePrediction : int
	{
		None = 0,
		Fixed = 1,
		Levinson = 2,
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct FlakeEncodeParams 
	{
	    // compression quality
	    // set by user prior to calling flake_encode_init
	    // standard values are 0 to 8
	    // 0 is lower compression, faster encoding
	    // 8 is higher compression, slower encoding
	    // extended values 9 to 12 are slower and/or use
	    // higher prediction orders
	    public int compression;

	    // prediction order selection method
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // valid values are 0 to 5
	    // 0 = use maximum order only
	    // 1 = use estimation
	    // 2 = 2-level
	    // 3 = 4-level
	    // 4 = 8-level
	    // 5 = full search
	    // 6 = log search
	    public int order_method;

	    // stereo decorrelation method
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // valid values are 0 to 2
	    // 0 = independent L+R channels
	    // 1 = mid-side encoding
	    public int stereo_method;
	
	    // block size in samples
	    // set by the user prior to calling flake_encode_init
	    // if set to 0, a block size is chosen based on block_time_ms
	    // can also be changed by user before encoding a frame
	    public int block_size;
	
	    // block time in milliseconds
	    // set by the user prior to calling flake_encode_init
	    // used to calculate block_size based on sample rate
	    // can also be changed by user before encoding a frame
	    public int block_time_ms;

	    // padding size in bytes
	    // set by the user prior to calling flake_encode_init
	    // if set to less than 0, defaults to 4096
	    public int padding_size;
	
	    // maximum encoded frame size
	    // this is set by flake_encode_init based on input audio format
	    // it can be used by the user to allocate an output buffer
	    public int max_frame_size;
	
	    // minimum prediction order
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // valid values are 0 to 4 for fixed prediction and 1 to 32 for non-fixed
	    public int min_prediction_order;
	
	    // maximum prediction order
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // valid values are 0 to 4 for fixed prediction and 1 to 32 for non-fixed
	    public int max_prediction_order;
	
	    // type of linear prediction
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // 0 = fixed prediction
	    // 1 = Levinson-Durbin recursion
	    public int prediction_type;
	
	    // minimum partition order
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // valid values are 0 to 8
	    public int min_partition_order;
	
	    // maximum partition order
	    // set by user prior to calling flake_encode_init
	    // if set to less than 0, it is chosen based on compression.
	    // valid values are 0 to 8
	    public int max_partition_order;
	
	    // whether to use variable block sizes
	    // set by user prior to calling flake_encode_init
	    // 0 = fixed block size
	    // 1 = variable block size
	    public int variable_block_size;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct FlakeContext 
	{
	    // number of audio channels
	    // set by user prior to calling flake_encode_init
	    // valid values are 1 to 8
	    public int channels;
	
	    // audio sample rate in Hz
	    // set by user prior to calling flake_encode_init
	    public int sample_rate;
	
	    // sample size in bits
	    // set by user prior to calling flake_encode_init
	    // only 16-bit is currently supported
	    public int bits_per_sample;
	
	    // total stream samples
	    // set by user prior to calling flake_encode_init
	    // if 0, stream length is unknown
	    public uint samples;
	
	    public FlakeEncodeParams encodeparams;
	
	    // maximum frame size in bytes
	    // set by flake_encode_init
	    // this can be used to allocate memory for output
	    public int max_frame_size;
	
	    // MD5 digest
	    // set by flake_encode_close;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
	    public byte[] md5digest;
	
	    // header bytes
	    // allocated by flake_encode_init and freed by flake_encode_close
	    public IntPtr header;
	
	    // encoding context, which is hidden from the user
	    // allocated by flake_encode_init and freed by flake_encode_close
	    public IntPtr private_ctx;
	}
	
	public class FlakeApi
	{
		const string DLLName = "flakedll.dll";
		
		[DllImport(DLLName)]
		public static extern int flake_set_defaults(ref FlakeEncodeParams encodeparams);
		
		[DllImport(DLLName)]
		public static extern int flake_validate_params(ref FlakeContext context);
		
		[DllImport(DLLName)]
		public static extern int flake_encode_init(ref FlakeContext context);
		
		[DllImport(DLLName)]
		public static extern int flake_encode_frame(ref FlakeContext context, byte[] frame_buffer, byte[] samples);
		
		[DllImport(DLLName)]
		public static extern void flake_encode_close(ref FlakeContext context);
	}
	
	// ONLY 16 bits per sample is supported
	public class FlakeWriter : IDisposable
	{
		const int DefaultCompressionLevel = 5;

		FlakeContext context;
		byte[] sampleData;
		byte[] outputFrame;
		int headerSize;
		
		public FlakeWriter(int channels, int sampleRate)
			: this(channels, sampleRate, DefaultCompressionLevel) {}
		
		public FlakeWriter(int channels, int sampleRate, int compressionLevel)
		{
			context = new FlakeContext();
			context.encodeparams.compression = compressionLevel;
			context.channels = channels;
			context.sample_rate = sampleRate;
			context.bits_per_sample = 16;
			context.encodeparams.max_frame_size = 4000; // ???
			
			FlakeApi.flake_set_defaults(ref context.encodeparams);
			FlakeApi.flake_validate_params(ref context);
			headerSize = FlakeApi.flake_encode_init(ref context);
		}
	
		public void ConvertFromWav(Stream wavData, Stream flacData)
		{
			int sampleDataSize = context.encodeparams.block_size * context.channels * 2; // 2 = 16 bit
			if (null == sampleData || sampleData.Length < sampleDataSize) {
				sampleData = new byte[sampleDataSize];			
			}
			
			if (null == outputFrame || outputFrame.Length < context.encodeparams.max_frame_size) {
				outputFrame = new byte[context.encodeparams.max_frame_size];
			}
			
			byte[] headerData = new byte[headerSize];
			Marshal.Copy(context.header, headerData, 0, headerSize);
			flacData.Write(headerData,0,headerData.Length);
			
			int readBytes = 0;
			readBytes = wavData.Read(sampleData, 0, sampleData.Length);
			while (readBytes > 0) {
				// change block size to match this frame & restore after encode
				int orig_block_size = context.encodeparams.block_size;
				context.encodeparams.block_size = readBytes / 2 / context.channels;
				int encodedBytes = FlakeApi.flake_encode_frame(ref context, outputFrame, sampleData);
				context.encodeparams.block_size = orig_block_size;
				
				// write output frame
				flacData.Write(outputFrame, 0, encodedBytes);
				
				// read next wav chunk
				readBytes = wavData.Read(sampleData, 0, sampleData.Length);
			}
		}
		
		public void Close()
		{
			FlakeApi.flake_encode_close(ref context);
		}
		
		public void Dispose ()
		{
			Close();
		}
	}
}

