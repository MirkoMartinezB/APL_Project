import requests
from dash import html, dcc, callback, Input, Output, State, no_update
import dash
from services.auth import login_user


#Quando parte la prima videata è quella del login
dash.register_page(__name__, path="/")

#Il layout del login è abbastanza semplice, deve contenere un titolo, due input fields, ed un pulsante per il submit
#Gli id verranno utilizzati all'interno delle callbacks per recuperare i parametri in Input e Output
#Le classi verranno utilizzati per i file css all'interno della cartella assets
layout = html.Div(className='login-box', children=[
    dcc.Location(id="login-redirect"),
    html.H2("Login", id="title", className="login-title"),
    html.Hr(),
    dcc.Input(id='username', type='email', placeholder="Email", className="input-field"),
    dcc.Input(id='password', type='password', placeholder="Password", className="input-field"),
    dcc.Button('Login', id='button-login', className="button-login"),
    html.Div(id='output-container-button')
])


@callback(
    Output("jwt-store", "data"),
    Output("login-redirect", "pathname"),
    Output("output-container-button", "children"),
    Input("button-login", "n_clicks"),
    State("username", "value"),
    State("password", "value"),
    prevent_initial_call=True
)
def handle_login(n_clicks, username, password):
    #Definiamo i campi mandatory, utilizziamo no_update di dash per non cambiare gli elementi OUTPUT
    if not username or not password:
        return no_update, no_update, "Inserisci email e password"
    try:
        #funzione che effettua la richiesta
        data = login_user(username, password)
        token = data.get("token")
        #Verifichiamo che il token venga restituito correttamente
        if not token:
                    return no_update, no_update, "Login riuscito, ma il token non è presente nella risposta"
        #Richiesta è andata a buon fine inseriamo nel primo OUTPUT il token
        #Indichiamo la pagina delle dashboard
        return {"token": token}, "/dashboard", ""
    #Eccezione sollevata in caso di risposta con codice del tipo 4xx o 5xx
    except requests.exceptions.HTTPError as e:
        response = e.response

        if response is not None:
            return no_update, no_update, f"Errore login: {response.status_code} - {response.text}"
        #Nel caso mancasse la risposta
        return no_update, no_update, "Errore HTTP durante il login"
    #Eccezione generica sulla richiesta
    except requests.exceptions.RequestException as e:
        return no_update, no_update, f"Errore richiesta: {str(e)}"