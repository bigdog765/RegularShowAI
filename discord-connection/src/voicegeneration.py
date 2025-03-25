from elevenlabs import generate, set_api_key
import unity_connection
import requests
set_api_key("c06ebd62dc194af951a54c79ad5f2d6a")
audio_snips = []
audio_lengths = []
audio_speakers = []


async def speak(text,voice):
    CHUNK_SIZE = 1024
    url = f"https://api.elevenlabs.io/v1/text-to-speech/21m00Tcm4TlvDq8ikWAM"

    headers = {
        "Accept": "audio/mpeg",
        "Content-Type": "application/json",
        "xi-api-key": "c06ebd62dc194af951a54c79ad5f2d6a",
        "output_format":"pcm_16000"
    }

    data = {
        "text": "Hi! My name is Voice, nice to meet you!",
        "model_id": "eleven_monolingual_v1",
        "voice_settings": {
            "stability": 0.5,
            "similarity_boost": 0.5
        }
    }

    response = requests.post(url, json=data, headers=headers)
    #print("Response data:",response.content)
    audio = generate(text=text,voice=voice)

    length = calculate_byte_audio_duration(audio)
    #play(audio)
    #append to arrays
    audio_snips.append(audio)
    audio_lengths.append(length)
    audio_speakers.append(str(voice))
def calculate_byte_audio_duration(audio_bytes):
    num_of_bytes = len(audio_bytes)
    print('Num of bytes:',num_of_bytes)
    bytes_per_frame = 1
    num_frames = num_of_bytes / bytes_per_frame
    duration_seconds = 2*(num_frames / 44100)
    print(duration_seconds,'(sec)')
    return duration_seconds
async def send_to_unity(audioArray,audioLengths,audioSpeakers):
    unity_res = await unity_connection.send_data(audioArray, audioLengths, audioSpeakers)
    print(unity_res)

async def start(gpt_response):
    i=0
    split_responses = gpt_response.split('\n\n')
    print('Num of responses:',len(split_responses))
    for response in split_responses:
        i+=1
        if response.startswith('Mordecai'):
            mord_response = response[10:]
            print('mordecai says this:::',mord_response)
            await speak(mord_response,'Mordecai')
        elif response.startswith('Rigby'):
            rig_response = response[7:]
            await speak(rig_response, 'Rigby')
            print('Rigby says this:::',rig_response)

    await send_to_unity(audio_snips,audio_lengths,audio_speakers)

