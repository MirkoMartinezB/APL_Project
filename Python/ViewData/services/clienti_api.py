import requests
from config.settings import BASE_URL, REQUEST_TIMEOUT, VERIFY_SSL


def get_headers(token):
    return {
        "Authorization": f"Bearer {token}"
    }


def get_clienti(token):
    response = requests.get(
        f"{BASE_URL}/clienti",
        headers=get_headers(token),
        timeout=REQUEST_TIMEOUT,
        verify=VERIFY_SSL
    )
    response.raise_for_status()
    return response.json()


def get_fatturato_cliente(token, cliente_id, data_inizio, data_fine):
    response = requests.get(
        f"{BASE_URL}/clienti/{cliente_id}/fatturato?dal={data_inizio}&al={data_fine}",
        headers=get_headers(token),
        timeout=5,
        verify=VERIFY_SSL
    )
    response.raise_for_status()
    return response.json()

def get_fatture_cliente(token, cliente_id, data_inizio, data_fine):
    response = requests.get(
        f"{BASE_URL}/clienti/{cliente_id}/fatture?dal={data_inizio}&al={data_fine}",
        headers=get_headers(token),
        timeout=5,
        verify=VERIFY_SSL
    )
    response.raise_for_status()
    return response.json()
