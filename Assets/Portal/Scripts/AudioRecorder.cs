using UnityEngine;
using System;
using System.Collections;
using NAudio.Wave;
using System.IO;
using FlakeSharp;

public class AudioRecorder : MonoBehaviour {
	
	WaveIn waveInStream;
	WaveFileWriter waveWriter;
	
	MemoryStream wavBuffer;
	MemoryStream flacBuffer;
	FlakeWriter flacWriter;
	
	bool recording;
	int totalBytesWritten;
	
	public int sampleRate = 22050;
	
	void Start()
	{
		EnumDevices();
		//StartCoroutine(GetSpeechToText(flacBuffer));
		//WavToFlac("Recordings/Recording22.wav");
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
		StopRecording();
		recording = true;
		totalBytesWritten = 0;
		
		waveInStream = new WaveIn();
		waveInStream.WaveFormat = new NAudio.Wave.WaveFormat(sampleRate, 2);
		wavBuffer = new MemoryStream();
		waveWriter = new WaveFileWriter(wavBuffer, waveInStream.WaveFormat);
		
		
		waveInStream.DataAvailable += new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);
		waveInStream.StartRecording();
		print("Starting Recording. Format: " + waveInStream.WaveFormat);
	}
	
	public void StopRecording()
	{
		if (recording) {
			// stop recorder
			waveInStream.StopRecording();
			waveInStream.Dispose();
			waveInStream = null;
			
			// convert wav to flac
			wavBuffer.Seek(0, SeekOrigin.Begin);
			Wav2Flac.WavReader rdr = new Wav2Flac.WavReader(wavBuffer);
			flacBuffer = new MemoryStream();
			flacWriter = new FlakeWriter(rdr.Channels, rdr.SampleRate);
			flacWriter.ConvertFromWav(rdr.InputStream, flacBuffer);

			StartCoroutine(GetSpeechToText(flacBuffer));

			// stop writer
			waveWriter.Close();
			
			flacWriter = null;
			flacBuffer = null;
			waveWriter = null;
			recording = false;
		}
	}
	
	IEnumerator GetSpeechToText(MemoryStream flacData)
	{
		WWWForm form = new WWWForm();
		Hashtable headers = new Hashtable() {
			{"Content-Type", "audio/x-flac; rate=" + sampleRate}
		};
		WWW req = new WWW("https://www.google.com/speech-api/v1/recognize?client=chromium&lang=en-US", flacData.GetBuffer(), headers);
		yield return req;
		
		// output format:
		// {"status":0,"id":"eb5fc5394dcfabe632ffbdcae03da538-1","hypotheses":[{"utterance":"testing testing testing","confidence":0.9603586}]}
		
		Hashtable result = JSON.JsonDecode(req.text) as Hashtable;
		int resultCode = (int)(double)result["status"];
		if (0 == resultCode) {
			ArrayList hypotheses = result["hypotheses"] as ArrayList;
			string txt = (string)(hypotheses[0] as Hashtable)["utterance"];
			print("SpeechToText: " + txt);
		} else {
			print("SpeechToText error: " + resultCode);
		}
	}
	
	void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
	{
		totalBytesWritten += e.BytesRecorded;
   		waveWriter.WriteData(e.Buffer, 0, e.BytesRecorded);
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
