from fastapi import FastAPI, Request
from urllib.parse import unquote
import json
import requests
import jwt
import os
import time
import openai

app = FastAPI()

def get_auth():
  pem = "smarttriage.2023-04-15.private-key.pem"
  with open(pem, 'rb') as pem_file:
    signing_key = jwt.jwk_from_pem(pem_file.read())

  app_id = os.environ.get('APP_ID')
  installation_id = os.environ.get('INSTAL_ID')

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

  print(f'https://api.github.com/app/installations/{installation_id}/access_tokens')

  auth_res = requests.post(f'https://api.github.com/app/installations/{installation_id}/access_tokens', headers=headers)
  auth_json = auth_res.json()
  print(auth_json)

  token = auth_json.get('token')

  return token

def change_assigne(assignee, issue_num, token):
    url = 'https://api.github.com/repos/{owner}/{repo}/issues/{issue_number}'
    req_headers = {
        "Authorization": f"Bearer {token}"
      }
    payload = {"assignees": [assignee]}

    response = requests.patch(url.format(owner='TomaszSteblik', repo='smart-triage', issue_number=issue_num), headers=req_headers, json=payload)

    print(response)


def post_comment(token, message, issue_num):
  url = 'https://api.github.com/repos/{owner}/{repo}/issues/{issue_number}/comments'

  req_headers = {
    "Authorization": f"Bearer {token}"
  }
  print(token)

  payload = {'body': message}
  response = requests.post(url.format(owner='TomaszSteblik', repo='smart-triage', issue_number=issue_num), headers=req_headers, json=payload)

@app.get("/issues")
def read_issues():
  response = requests.get("https://api.github.com/repos/TomaszSteblik/smart-triage/issues")
  data = response.json()

  return data

@app.post("/issues")
def update_issue(resquest: Request):
  url = 'https://api.github.com/repos/{owner}/{repo}/issues/{issue_number}/comments'

  token = get_auth()

  req_headers = {
    "Authorization": f"Bearer {token}"
  }
  print(token)

  payload = {'body': 'This is a comment.'}
  response = requests.post(url.format(owner='TomaszSteblik', repo='smart-triage', issue_number='19'), headers=req_headers, json=payload)

  print(response.json())

  return {
    'status': 200
  }

@app.post("/webhook")
async def process_webhook(request: Request):
  body = await request.body()
  body_json = json.loads(body)

  if body_json.get('action') != 'opened':
    return {
      'status': 200
    }

  issue = body_json.get('issue', {})
  title = issue.get('title')
  issue_num = issue.get('number')

  description = issue.get('body')
  l_desc = description.lower()

  token = get_auth()

  if "[BUG]" in title:
    if "steps to reproduce" not in l_desc:
      post_comment(token, 'Please provide steps to reproduce.',issue_num)
      return {
        'status': 200
      }
    if "description" not in l_desc:
      post_comment(token, 'Please provide description.',issue_num)
      return {
        'status': 200
      }
    if "windows version" not in l_desc:
      post_comment(token, 'Please provide Windows version.',issue_num)
      return {
        'status': 200
      }
  elif "[FEAT]" in title:
    if "description" not in l_desc:
      post_comment(token, 'Please provide description.',issue_num)
      return {
        'status': 200
      }
    if "requirements" not in l_desc:
      post_comment(token, 'Please provide requirements.',issue_num)
      return {
        'status': 200
      }
  else:
    post_comment(token, 'Please mark the issue as [BUG] or [FEAT]',issue_num)

    return {
      'status': 200
    }

  openai.api_key = os.environ.get('GPT_TOKEN')

  messages = [
    {"role": "system", "content": "You are a bot that evaluates whether the provided github issue description is sufficient for developer to work with the issue. You answer only yes or no"}
  ]
  messages.append({"role": "user", "content": "Is this github issue description sufficient for developer to work with the issue? answer yes or no: " + description})

  chat = openai.ChatCompletion.create(
    model="gpt-3.5-turbo",
    messages=messages
  )

  reply = chat.choices[0].message.content
  print(reply)
  
  if reply in ["No.", "no", "no.", "No"]:
    token = get_auth()
    post_comment(token, 'Please provide more information.',issue_num)

    return {
      'status': 200
    }

  return {
    'status': 200
  }


