import requests
from config.settings import BASE_URL, REQUEST_TIMEOUT, VERIFY_SSL, LOGIN_URL


def login_user(email, password):
    response = requests.post(
        f"{LOGIN_URL}/login",
        json={
            "email": email,
            "password": password
        },
        timeout=REQUEST_TIMEOUT,
        verify=VERIFY_SSL
    )
    response.raise_for_status()
    return response.json()