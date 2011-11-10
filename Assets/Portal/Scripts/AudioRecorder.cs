using UnityEngine;
using System;
using System.Collections;
using NAudio.Wave;
using System.IO;
using Wav2Flac;

public class AudioRecorder : MonoBehaviour {
	
	WaveIn waveInStream;
	WaveFileWriter waveWriter;
	
	MemoryStream flacBuffer;
	FlacWriter flacWriter;
	
	bool recording;
	int totalBytesWritten;
	
	int sampleRate = 44000;
	
	void Start()
	{
		EnumDevices();
		//StartCoroutine(GetSpeechToText(flacBuffer));
		//WavToFlac("Recordings/Recording22.wav");
		StartCoroutine(Test());
	}
	
	IEnumerator Test()
	{
		WWWForm form = new WWWForm();
		Hashtable headers = new Hashtable() {
			{"Content-Type", "audio/x-flac; rate=44000"}
		};
		WWW req = new WWW("https://www.google.com/speech-api/v1/recognize?client=chromium&lang=en-US", File.ReadAllBytes("c:\\Zigfu\\Portal\\Recordings\\Recording50.wav.super.flac"), headers);
		yield return req;
		print(req.text);
	}
	
	public void EnumDevices()
	{
		int waveInDevices = WaveIn.DeviceCount;
		for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
		{
		    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
		    print(String.Format("Device {0}: {1}, {2} channels", 
		        waveInDevice, deviceInfo.ProductName, deviceInfo.Channels));
		}	
	}
	
	public void StartRecording()
	{
		StartRecording(GetRecordingFilename());
	}
	
	public void StartRecording(string filename)
	{
		print("Starting recording " + filename);
		StopRecording();
		recording = true;
		totalBytesWritten = 0;
		
		waveInStream = new WaveIn();
		waveInStream.WaveFormat = new NAudio.Wave.WaveFormat(sampleRate, 2);
		waveWriter = new WaveFileWriter(filename, waveInStream.WaveFormat);
		flacBuffer = new MemoryStream();
		flacWriter = new FlacWriter(flacBuffer, waveInStream.WaveFormat.BitsPerSample, waveInStream.WaveFormat.Channels, waveInStream.WaveFormat.SampleRate);
		
		waveInStream.DataAvailable += new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);
		waveInStream.StartRecording();
		print("Recording format: " + waveInStream.WaveFormat);
	}
	
	public void StopRecording()
	{
		if (recording) {
			// stop recorder
			waveInStream.StopRecording();
			waveInStream.Dispose();
			waveInStream = null;
			// stop writer
			waveWriter.Close();
			// get speech to text
			
			flacWriter.Close();
			File.WriteAllBytes(waveWriter.Filename + ".flac", flacBuffer.GetBuffer());
			print(string.Format("Writing {0} bytes to {1}", flacBuffer.GetBuffer().Length, waveWriter.Filename));
			StartCoroutine(GetSpeechToText(flacBuffer));
			
			flacWriter = null;
			flacBuffer = null;
			waveWriter = null;
			recording = false;
			print(String.Format("Stopping recording. {0} bytes written", totalBytesWritten));
			
		}
	}
	
	IEnumerator GetSpeechToText(MemoryStream flacData)
	{
		WWWForm form = new WWWForm();
		Hashtable headers = new Hashtable() {
			{"Content-Type", "audio/x-flac; rate=" + sampleRate}
			//{"Content-Type", "audio/x-flac"}
		};
		WWW req = new WWW("https://www.google.com/speech-api/v1/recognize?client=chromium&lang=en-US", flacData.GetBuffer(), headers);
		yield return req;
		print(req.text);
		
	}

	void WavToFlac(string filename)
	{
		MemoryStream flacMem = new MemoryStream();
		using (WavReader wav = new WavReader(filename))
        {
            using (FlacWriter flac = new FlacWriter(File.OpenWrite(filename + ".flac"), wav.BitDepth, wav.Channels, wav.SampleRate))
            {
                // Buffer for 1 second's worth of audio data
                byte[] buffer = new byte[wav.Bitrate / 8];
                int bytesRead;

                do
                {
                    bytesRead = wav.InputStream.Read(buffer, 0, buffer.Length);
                    flac.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }
        }
	}

	
	void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
	{
		totalBytesWritten += e.BytesRecorded;
   		waveWriter.WriteData(e.Buffer, 0, e.BytesRecorded);
		flacWriter.Write(e.Buffer, 0, e.BytesRecorded);
	}
	
	string GetRecordingFilename()
    {
        System.IO.Directory.CreateDirectory("Recordings");
        int i=1;
        while (System.IO.File.Exists(System.IO.Path.Combine("Recordings", "Recording" + i + ".wav"))) {
            i++;
        }
        return System.IO.Path.Combine("Recordings", "Recording" + i + ".wav");
    }
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("r"))) {
			if (recording) {
				StopRecording();
			} else {
				StartRecording();
			}
			Event.current.Use();
		}
	}
}
