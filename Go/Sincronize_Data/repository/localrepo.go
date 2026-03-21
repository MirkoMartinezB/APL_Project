package repository

import (
	"Sincronize_Data/db"
	"Sincronize_Data/models"
	"time"
)

func GetClienti(lastUpdate time.Time, doneChan chan []models.Cliente, errorChan chan error) {
	qry := "SELECT CodiceCliente, RagioneSociale FROM VCLIENTI WHERE DataLastUpdate >= (@p1)"
	rows, err := db.DB.Query(qry, lastUpdate)

	if err != nil {
		//return nil, err
		errorChan <- err
		return
	}
	defer rows.Close()

	var clienti []models.Cliente

	for rows.Next() {
		var c models.Cliente
		err := rows.Scan(&c.Codice, &c.RagioneSociale)
		if err != nil {
			//return nil, err
			errorChan <- err
			return
		}
		clienti = append(clienti, c)
	}

	if err = rows.Err(); err != nil {
		errorChan <- err
		return
	}
	doneChan <- clienti
	//return clienti, nil
}

func GetFornitori(lastUpdate time.Time, chFornitore chan []models.Fornitore, chError chan error) {
	qry := "SELECT CodiceFornitore, RagioneSociale FROM VFORNITORI WHERE DataLastUpdate >= (@p1)"
	rows, err := db.DB.Query(qry, lastUpdate)
	if err != nil {
		chError <- err
		return
	}
	defer rows.Close()

	var fornitore []models.Fornitore

	for rows.Next() {
		var f models.Fornitore
		err := rows.Scan(&f.Codice, &f.RagioneSociale)
		if err != nil {
			chError <- err
		}
		fornitore = append(fornitore, f)
	}
	if err = rows.Err(); err != nil {
		chError <- err
		return
	}
	chFornitore <- fornitore
	//return fornitore, nil
}

func GetArticoli(lastUpdate time.Time, chArticolo chan []models.Articolo, chError chan error) {
	qry := "SELECT CodiceArticolo, Descrizione FROM VARTICOLI WHERE DataLastUpdate >= (@p1)"
	rows, err := db.DB.Query(qry, lastUpdate)
	if err != nil {
		chError <- err
		return
	}
	defer rows.Close()

	var articolo []models.Articolo

	for rows.Next() {
		var a models.Articolo
		err := rows.Scan(&a.Codice, &a.Descrizione)
		if err != nil {
			chError <- err
			return
		}
		articolo = append(articolo, a)
	}

	if err = rows.Err(); err != nil {
		chError <- err
		return
	}
	chArticolo <- articolo
	//return articolo, nil
}

func GetFattureClienti(lastUpdate time.Time, chFattureClienti chan []models.FatturaCliente, chError chan error) {
	//Considero la possibilità che la fattura possa essere passata e poi cancellata in un secondo momento
	//Dato i tempi della fattura elettronica in Italia, dopo 12 giorni la situazione è stabile
	//Quindi la sottrzione di sotto non è casuale
	lastUpdate = lastUpdate.AddDate(0, 0, -12)
	qry := `SELECT CodiceCliente, DataDocumento, NumeroDocumento, CodiceArticolo, 
       QtaMovimento, Prezzo, ScontoPerc, Importo, ImportoIvato, ImportoIva, AliquotaIva 
FROM VFATTURECLIENTI WHERE DataDocumento >= (@p1)`
	rows, err := db.DB.Query(qry, lastUpdate)
	if err != nil {
		chError <- err
		return
	}
	defer rows.Close()

	var fatture []models.FatturaCliente

	for rows.Next() {

		var f models.FatturaCliente
		err := rows.Scan(&f.CodiceControparte,
			&f.DataDocumento,
			&f.NumeroDocumento,
			&f.CodiceArticolo,
			&f.QtaMovimento,
			&f.Prezzo,
			&f.ScontoPerc,
			&f.Importo,
			&f.ImportoIvato,
			&f.ImportoIva,
			&f.AliquotaIva,
		)
		if err != nil {
			chError <- err
			return
		}
		fatture = append(fatture, f)

	}

	if err = rows.Err(); err != nil {
		chError <- err
	}
	chFattureClienti <- fatture
	//return fatture, nil
}

func GetFattureFornitori(lastUpdate time.Time, chFattureFornitori chan []models.FatturaFornitore, chError chan error) {
	//Come in vendita
	lastUpdate = lastUpdate.AddDate(0, 0, -12)
	qry := `SELECT CodiceFornitore, DataDocumento, NumeroDocumento, CodiceArticolo, 
       QtaMovimento, Prezzo, ScontoPerc, Importo, ImportoIvato, ImportoIva, AliquotaIva 
FROM VFATTUREFORNITORI WHERE DataDocumento >= (@p1)`
	rows, err := db.DB.Query(qry, lastUpdate)
	if err != nil {
		chError <- err
		return
	}
	defer rows.Close()

	var fatture []models.FatturaFornitore

	for rows.Next() {

		var f models.FatturaFornitore
		err := rows.Scan(&f.CodiceControparte,
			&f.DataDocumento,
			&f.NumeroDocumento,
			&f.CodiceArticolo,
			&f.QtaMovimento,
			&f.Prezzo,
			&f.ScontoPerc,
			&f.Importo,
			&f.ImportoIvato,
			&f.ImportoIva,
			&f.AliquotaIva,
		)
		if err != nil {
			chError <- err
			return
		}
		fatture = append(fatture, f)
	}

	if err = rows.Err(); err != nil {
		chError <- err
		return
	}
	chFattureFornitori <- fatture
	//return fatture, nil
}
