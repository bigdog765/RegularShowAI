import responses
import discord

async def send_msg(msg, user_msg, private):
    try:
        response = await responses.handle_response(user_msg)

        await msg.author.send(response) if private \
            else await msg.channel.send(response)
    except Exception as e:
        print(e)

def run_bot():
    TOKEN = 0
    intent = discord.Intents.default()
    intent.message_content = True
    client = discord.Client(intents=intent)

    @client.event
    async def on_ready():
        print(f'{client.user} is running!')

    @client.event
    async def on_message(msg):
        if msg.author == client.user:
            return
        username = str(msg.author)
        user_msg = str(msg.content)
        channel = str(msg.channel)

        print(f"{username} said: '{user_msg}' in ({channel})")
        await send_msg(msg,user_msg,private=False)

    client.run(TOKEN)