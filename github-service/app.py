from fastapi import FastAPI, Request
from urllib.parse import unquote
import json
import requests
import jwt
import os
import time

app = FastAPI()

@app.get("/issues")
def read_issues():
  response = requests.get("https://api.github.com/repos/TomaszSteblik/smart-triage/issues")
  data = response.json()

  return data

@app.post("/issues")
def update_issue(resquest: Request):
  pem = "smarttriage.2023-04-15.private-key.pem"
  with open(pem, 'rb') as pem_file:
    signing_key = jwt.jwk_from_pem(pem_file.read())

  app_id = os.environ.get('APP_ID')

  payload = {
    'iat': int(time.time()),
    'exp': int(time.time()) + 600,
    'iss': app_id
  }

  jwt_instance = jwt.JWT()
  encoded_jwt = jwt_instance.encode(payload, signing_key, alg='RS256')

  headers = {
    "Authorization": f"Bearer {encoded_jwt}"
  }

  # url = 'https://api.github.com/repos/{owner}/{repo}/issues/{issue_number}/comments'

  # payload = {'body': 'This is a comment.'}
  # response = requests.post(url.format(owner='TomaszSteblik', repo='smart-triage', issue_number='10'), headers=headers, json=payload)

  # print(response.json())

  return {
    'status': 200
  }

@app.post("/webhook")
async def process_webhook(request: Request):
  body = await request.body()
  body_json = json.loads(body)

  issue = body_json.get('issue', {})
  title = issue.get('title')
  
  description = issue.get('body')

  print(title, description)
  return {
    'status': 200
  }


