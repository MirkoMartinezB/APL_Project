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
def load_fatture_cliente(n_clicks, jwt_data, cliente, data_inizio, data_fine):
    if not jwt_data or not jwt_data.get("token"):
        return [], "Token non disponibile. Effettua di nuovo il login."

    token = jwt_data["token"]

    headers = {
        "Authorization": f"Bearer {token}"
    }

    try:
        response = requests.get(
            f"https://localhost:7157/clienti/{cliente}/fatture?dal={data_inizio}&al={data_fine}",
            headers=headers,
            timeout=5,
            verify=False
        )

        if response.status_code != 200:
            return [], f"Errore recupero dati: {response.status_code} - {response.text}"

        data = response.json()


        fig = go.Figure(
            data=[
                go.Pie(
                    labels=[str(x["anno"])+"-"+(mesi[x["mese"]]) for x in data],
                    values=[x["fatturato"] for x in data]
                )
            ]
        )
        fig.update_layout(title=f"Fatturato cliente")

        return fig, ""

    except requests.exceptions.RequestException as e:
        return [], f"Errore richiesta: {str(e)}"

