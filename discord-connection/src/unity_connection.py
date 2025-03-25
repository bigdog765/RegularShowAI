import socket
import time
import struct
from elevenlabs import play
host, port = "127.0.0.1", 80

async def send_data(audioArray,audioLengths,audioSpeakers):
    # SOCK_STREAM means TCP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        # Connect to the server and send the data
        sock.connect((host, port))
        i = 0
        for audio in audioArray:
            if audioSpeakers[i] == 'Mordecai':
                print('Audio speaker is',audioSpeakers[i])
                header = b'\x01'
            if audioSpeakers[i] == 'Rigby':
                print('Audio speaker is', audioSpeakers[i])
                header = b'\x02'
            roundedLength = round(audioLengths[i])
            byteLength = roundedLength.to_bytes(1,byteorder='big')

            print('Byte length in bytes:',byteLength)

            sock.sendall(header+byteLength+audio)
            response = sock.recv(4096)  # Adjust the buffer size as needed
            print('Received from Unity:', response)  # Decode bytes to string
            #sleep right here
            #time.sleep(audioLengths[i] + 7)
            i+=1
        return 'success'
    finally:
        sock.close()
