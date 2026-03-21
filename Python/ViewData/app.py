from dash import Dash, html, dcc, page_container

app = Dash(__name__, use_pages=True, suppress_callback_exceptions=True)
#Effettuato il login riceveremo un JSON Web Token che rimarrà attivo fino a che sarà attiva la sessione
#Tipicamente la sessione rimane attiva fino alla chiusura del browser
app.layout = html.Div([
    dcc.Store(id="jwt-store", storage_type="session"),
    page_container
])

server = app.server

if __name__ == "__main__":
    app.run(debug=True)