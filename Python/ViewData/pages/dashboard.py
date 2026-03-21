import json
import requests
import dash

import plotly.graph_objs as go
from dash import html, dcc, callback, Input, Output, State, no_update
import dash_ag_grid as dag
import pandas as pd
from utils.constants import MESI
from services.clienti_api import get_clienti, get_fatturato_cliente, get_fatture_cliente

dash.register_page(__name__, path="/dashboard")

layout = html.Div(className="dashboard-container", children=[
    #Utilizzo dcc.Location per triggherare la callback che carica i clienti nel Dropdown
    dcc.Location(id="dashboard-url"),
    html.H2("Situazione Cliente"),
    html.Hr(),

    html.Label("Seleziona un Cliente:"),
    dcc.Dropdown(
        id="combo-clienti",
        options=[] #recupero le options dalla prima callback richiamata all'apertura della pagine
    ),

    html.Label("Data inizio:"),
    dcc.DatePickerSingle(
        id="data-inizio",
        display_format="YYYY-MM-DD",
        date="2024-01-01"
    ),
    html.Div(id="output-data"),
    html.Label("Data fine:"),
    dcc.DatePickerSingle(
        id="data-fine",
        display_format="YYYY-MM-DD",
        date="2025-12-31"
    ),
    html.Div(id="output-data"),
    html.Div(id="dashboard-message"),
    dcc.Button('Carica Fatturato', id='button-request-fatturato', className="button-request"),
    html.Div(id="dashboard-message-errore"),
    html.Div(id='output-container-button'),
    html.H4('Fatturato del cliente'),
    dcc.Graph(id="graph-pie-fatturato-mese"),
    html.H4("Elenco Fatture"),
    dag.AgGrid(
    id="grid-fatture",
    columnDefs= [
        {"headerName": "Numero", "field": "numeroFattura", "sortable": True, "filter": True},
        {"headerName": "Data", "field": "dataFattura", "sortable": True, "filter": True},
        {"headerName": "Cliente", "field": "ragioneSociale", "sortable": True, "filter": True, "flex": 1},
        {"headerName": "Totale Fattura", "field": "totaleFattura", "sortable": True, "filter": "agNumberColumnFilter"},
        {"headerName": "Totale Imponibile", "field": "totaleImporto", "sortable": True, "filter": "agNumberColumnFilter"},
    ],
    rowData=[],
    defaultColDef={
        "resizable": True,
        "sortable": True,
        "filter": True
    },
    dashGridOptions={
        "pagination": True,
        "paginationPageSize": 10,
        "animateRows": False
    },
    style={"height": "400px", "width": "100%"}
)
])


#Il Dropdown dei clienti deve caricarsi all'apertura della pagina, per questo come trigger
# di input mettidamo dashboard-url che corrisponde al dcc.Location (link corrente)
@callback(
    Output("combo-clienti", "options"),
    Output("dashboard-message", "children"),
    Input("dashboard-url", "pathname"),
    State("jwt-store", "data"),
    prevent_initial_call=False
)
def load_clienti(pathname, jwt_data):
    if pathname != "/dashboard":
        return no_update, no_update

    if not jwt_data or not jwt_data.get("token"):
        return [], "Token non disponibile. Effettua di nuovo il login."

    token = jwt_data["token"]


    try:
        #Servizio che recupera i clienti
        data = get_clienti(token)
        #Sfruttiamo la list comprehension per caricare le options del Dropdown
        options = [
            {"label": item["ragioneSociale"], "value": item["codice"]}
            for item in data
        ]
        return options, ""

    #Verifichiamo eventuali errori 4xx o 5xx
    except requests.exceptions.HTTPError as e:
        response = e.response
        if response is not None:
            return no_update, no_update, f"Errore durante il recupero dei clienti: {response.status_code} - {response.text}"
        #Nel caso mancasse la risposta
        return no_update, no_update, "Errore HTTP durante il recupero dei clienti"

    #Errore generico sulla richiesta
    except requests.exceptions.RequestException as e:
        return [], f"Errore richiesta: {str(e)}"


#La callback viene triggerata quando viene premuto il pulsante "carica fatturato"
@callback(
    Output("graph-pie-fatturato-mese", "figure"),
    Output("dashboard-message-errore", "children"),
    Input("button-request-fatturato", "n_clicks"),
    State("jwt-store", "data"),
    State("combo-clienti", "value"),
    State("data-inizio", "date"),
    State("data-fine", "date"),
    prevent_initial_call=True
)
def load_fatturato_cliente(n_clicks, jwt_data, cliente, data_inizio, data_fine):
    if not jwt_data or not jwt_data.get("token"):
        return [], "Token non disponibile. Effettua di nuovo il login."
    #Recuperiamo il token
    token = jwt_data["token"]
    try:
        #Servizio che recupera il fatturato
        data = get_fatturato_cliente(token, cliente, data_inizio, data_fine)
        #Costruiamo il grafico a torta
        fig = go.Figure(
           data=[
               go.Pie(
                    labels=[str(x["anno"])+"-"+(MESI[x["mese"]]) for x in data], #label del tipo anno-mese
                    values=[x["fatturato"] for x in data]
                )
            ]
        )
        fig.update_layout()
        return fig, ""
    # Verifichiamo eventuali errori 4xx o 5xx
    except requests.exceptions.HTTPError as e:
        response = e.response
        if response is not None:
            return no_update, no_update, f"Errore durante il recupero del fatturato del cliente: {response.status_code} - {response.text}"
        # Nel caso mancasse la risposta
        return no_update, no_update, "Errore HTTP durante il recupero del fatturato del cliente"
    except requests.exceptions.RequestException as e:
        return [], f"Errore richiesta: {str(e)}"



#La callback viene triggerata quando viene premuto il pulsante "carica fatturato"
@callback(
    Output("grid-fatture", "rowData"),
    Input("button-request-fatturato", "n_clicks"),
    State("jwt-store", "data"),
    State("combo-clienti", "value"),
    State("data-inizio", "date"),
    State("data-fine", "date"),
    prevent_initial_call=True
)
def load_fatture_cliente(n_clicks, jwt_data, cliente, data_inizio, data_fine):
    if not jwt_data or not jwt_data.get("token"):
        return [], "Token non disponibile. Effettua di nuovo il login."
    #Recuperiamo il token
    token = jwt_data["token"]
    try:
        #Servizio che recupera il fatturato
        data = get_fatture_cliente(token, cliente, data_inizio, data_fine)

        row_data = [
            {
                "numeroFattura": fattura.get("numeroFattura"),
                "dataFattura": fattura.get("dataFattura", "")[:10],
                "ragioneSociale": fattura.get("cliFor", {}).get("ragioneSociale", ""),
                "totaleFattura": fattura.get("totaleFattura"),
                "totaleImporto": fattura.get("totaleImporto")
            }
            for fattura in data
        ]
        return row_data

    # Verifichiamo eventuali errori 4xx o 5xx
    except requests.exceptions.HTTPError as e:
        response = e.response
        if response is not None:
            return no_update
        # Nel caso mancasse la risposta
        return no_update,
    except requests.exceptions.RequestException as e:
        return no_update
