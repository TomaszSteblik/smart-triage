from fastapi import FastAPI, Request
from urllib.parse import unquote
import json

app = FastAPI()

@app.get("/")
def read_root():
  return {"Hello": "World"}

@app.post("/")
async def process_webhook(request: Request):
  body = await request.body()
  decoded = unquote(body[8:])
  body_json = json.loads(decoded)

  issue = body_json.get('issue', {})
  title = issue.get('title')
  
  description = issue.get('body')

  print(title, description)
  return {
    'status_code': 200
  }


