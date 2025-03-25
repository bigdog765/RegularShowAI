using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.IO;
using UnityEngine.Events;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Reflection.Emit;
using NAudio.Wave;
using System;
using System.Linq;

public class APIListener : MonoBehaviour
{
    Thread thread;
    public int connectionPort;
    TcpListener server;
    TcpClient client;
    private bool running;
    public AudioSource CharacterVoices; // Reference to the AudioSource component
    public AudioClip audioClip;
    public float[] audioData;
    public bool audio_complete = false;
    public int talker = 0;
    public int lengthOfSpeech = 0;
    // Define the event
    [System.Serializable]
    public class audioEvent : UnityEvent<byte[]> { }
    //Event to be raised when the variable changes
    
    public int audioConsumed;

    // Sample rate of the audio data
    public int sampleRate = 44100;
    void Start()
    {
        //CharacterVoices = GetComponent<AudioSource>();
        if (CharacterVoices != null)
        {
            Debug.Log("Audio Source Found");
        }
        else
        {
            Debug.Log("No audio source found");
        }
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        Thread t = new Thread(new ThreadStart(GetData));
        t.Start();
    }
    private void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();
        Debug.Log("Server started on port "+connectionPort);
        // Create a client to get the data stream
        running = true;
        
        
            Debug.Log("Running server");
            AcceptConnection();
        
    }

    private void AcceptConnection()
    {
        // Create a client to get the data stream
        client = server.AcceptTcpClient();
        // Start listening for data
        
            ReceiveData();
            Debug.Log("Hello World");
        
        // Close the client connection
        //Debug.Log("Closing connection...");
        //client.Close();
    }
    private void ReceiveData()
    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();
        
        byte[] buffer = new byte[4*client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, buffer.Length);
        
        // Check if any data was received
        if (bytesRead > 0)
        {
            // Handle the received audio data
            
            HandleAudio(buffer, bytesRead);
            WaitForAudioProcessing(() =>
            {
                // Write back to the client
                byte[] success = new byte[1];
                Debug.Log("Sending back to client.");
                nwStream.Write(success, 0, 1);

                // Clear the buffer and initiate the next data reception
                Array.Clear(buffer, 0, buffer.Length);
                audio_complete = false;
                ReceiveData();
            });


        }
    }
    private void WaitForAudioProcessing(Action callback)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            StartCoroutine(WaitForAudioCoroutine(callback));
        });
        
    }
    private IEnumerator WaitForAudioCoroutine(Action callback)
    {
        while (!audio_complete)
        {
            yield return null; // Wait until audio processing is complete
        }

        // Call the provided callback function
        callback.Invoke();
    }

    // Use-case specific function, need to re-write this to interpret whatever data is being sent
    private void HandleAudio(byte[] aud, int length)
    {
        lengthOfSpeech = aud[1];
        Debug.Log("Length of speech (s):" + lengthOfSpeech);
        if (aud[0] == 1)
        {
            talker = 1;
        }
        if (aud[0] == 2)
        {
            talker = 2;
        }
        Debug.Log("Talker:" + talker);
        //Remove first & second byte
        byte[] bytesNew = new byte[length-2];
        for (int i = 0, j = 0; i < length; i++)
        {
            if (i > 1)
            {
                bytesNew[j] = aud[i];
                j++;
            }
        }
        decodeMP3(bytesNew);
    }
    // Event handler method
    void OnAudioChange(byte[] wavData)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Create AudioClip from WAV data on the main thread
            audioClip = WavUtility.ToAudioClip(wavData);
            // Play the audio
            CharacterVoices.clip = audioClip;
            CharacterVoices.Play();
            // Now you can use the 'audioClip' on the main thread
            // Start a coroutine to wait until the audio clip has finished playing
            StartCoroutine(WaitForAudioClip(CharacterVoices, () =>
            {
            }));
            
        }); 
    }
    IEnumerator WaitForAudioClip(AudioSource p, Action callback)
    {
        // Wait until the length of the audio clip has passed
        yield return new WaitForSeconds(lengthOfSpeech+2);

        // Code here will run after the audio clip has finished playing
        Debug.Log("Audio clip has finished playing!");
        audio_complete = true;
        // Call the callback function
        callback.Invoke();
    }
    void decodeMP3(byte[] audioBytes)
    {
        // Decode MP3 data
        using (var mp3Stream = new Mp3FileReader(new System.IO.MemoryStream(audioBytes)))
        {
            // Create a MemoryStream to hold WAV data
            using (var wavMemoryStream = new System.IO.MemoryStream())
            {
                // Create WAV audio data
                using (var wavStream = new WaveFileWriter(wavMemoryStream, mp3Stream.WaveFormat))
                {
                    // Copy data from MP3 to WAV stream
                    mp3Stream.CopyTo(wavStream);
                }

                // Get the WAV audio data as bytes
                byte[] wavData = wavMemoryStream.ToArray();
                // Raise the event on the main thread when the variable changes
                OnAudioChange(wavData);
            }
        }
    }
    void OnDestroy()
    {
        // Ensure proper cleanup when the script is destroyed
        running = false;
        if (client != null)
            client.Close();
        if (server != null)
            server.Stop();
    }
    void Update()
    { 
    }
}