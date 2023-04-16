from fastapi import FastAPI, Request
import openai
import json
import os
import time

app = FastAPI()

@app.post("/process")
async def get_tags(request: Request):
  body = await request.body()
  body_json = json.loads(body)

  openai.api_key = os.environ.get('GPT_TOKEN')

  messages = [
    {"role": "system", "content": "You are a bot that creates tags from description, you do not provide any explanations, just list of tags you found in an array format like that: [\"first tag\", \"second tag\"]"}
  ]
  messages.append({"role": "user", "content": body_json.get('input')})

  chat = openai.ChatCompletion.create(
    model="gpt-3.5-turbo",
    messages=messages
  )

  reply = chat.choices[0].message.content

  return {
    'status': 200,
    'reply': reply
  }

@app.post("/batch-process")
async def get_tags(request: Request):
  body = await request.body()
  body_json = json.loads(body)

  inputs = body_json.get('inputs')

  openai.api_key = os.environ.get('GPT_TOKEN')

  replies = []

  for input in inputs:
    messages = [
      {"role": "system", "content": "You are a bot that creates tags from description, you do not provide any explanations, just list of tags you found in an array format"}
    ]
    messages.append({"role": "user", "content": input.get('description')})

    chat = openai.ChatCompletion.create(
      model="gpt-3.5-turbo",
      messages=messages
    )

    print(chat.choices[0].message.content)

    replies.append({
      'reply': chat.choices[0].message.content
    })

    time.sleep(5)

  return {
    'status': 200,
    'reply': json.dumps(replies)
  }

