import gpt_generation
import voicegeneration
async def handle_response(msg):

    msg = msg.lower()
    if msg == 'hello':
        return 'Hi, im family guy bot.'
    if msg.startswith('!topic'):
        msg_topic = msg[6:]
        if len(msg_topic) == 0:
            return 'No topic defined'
        else:
            gpt_msg = await gpt_generation.execute(msg)
            print('hello?')
            await voicegeneration.start(gpt_msg)

            return gpt_msg
    return
