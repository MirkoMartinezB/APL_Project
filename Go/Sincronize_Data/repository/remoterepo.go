package repository

import (
	"Sincronize_Data/db"
	"Sincronize_Data/models"
	"database/sql"
	"fmt"
	"strings"

	"time"
)

// Funzione utilizzata per scrivere sia anagrafica cliente che anagrafica fornitore sul DB remoto
func WriteAnagrafica[T models.DBWritableAnagrafica](anagrafica []T, doneChan chan bool, errChan chan error) {
	//Query originale, troppo lenta, la cambio dinamicamente in modo da inviare più righe alla volta
	/*
			qry := `INSERT INTO "VCLIENTIMOVIMENTATI" ("CodiceCliente", "RagioneSociale")
		VALUES ($1, $2)
		ON CONFLICT ("CodiceCliente")
		DO UPDATE SET "RagioneSociale" = EXCLUDED."RagioneSociale"`
	*/
	const batchSize = 100
	//Avvio una singola transizione per l'insert in batch
	tx, err := db.DB_SUPABASE.Begin()
	if err != nil {
		errChan <- err
		return
	}
	//Effettuo il rollback in caso di errore
	defer tx.Rollback()
	var myType T
	var myQry string
	switch any(myType).(type) {
	case models.Cliente:
		myQry = `
		    INSERT INTO "VCLIENTIMOVIMENTATI" ("CodiceCliente", "RagioneSociale")
			VALUES %s
			ON CONFLICT ("CodiceCliente")
			DO UPDATE SET "RagioneSociale" = EXCLUDED."RagioneSociale"
			`
	case models.Fornitore:
		myQry = `
		    INSERT INTO "VFORNITORIMOVIMENTATI" ("CodiceFornitore", "RagioneSociale")
			VALUES %s
			ON CONFLICT ("CodiceFornitore")
			DO UPDATE SET "RagioneSociale" = EXCLUDED."RagioneSociale"
			`
	}

	for start := 0; start < len(anagrafica); start += batchSize {
		end := start + batchSize
		if end > len(anagrafica) {
			end = len(anagrafica)
		}
		//Effettuo lo slice degli elementi
		batch := anagrafica[start:end]
		//utilizzo questi array per completare la qry
		var valueStrings []string
		var valueArgs []interface{}

		for i, anagr := range batch {
			p1 := i*2 + 1
			p2 := i*2 + 2
			//Con questo creo la stringa da aggiungere alla qry
			valueStrings = append(valueStrings, fmt.Sprintf("($%d,$%d)", p1, p2))
			valueArgs = append(valueArgs, anagr.GetCodice(), anagr.GetRagioneSociale())
		}

		query := fmt.Sprintf(myQry, strings.Join(valueStrings, ","))

		_, err := tx.Exec(query, valueArgs...)
		if err != nil {
			errChan <- err
			return
		}
	}
	//Effettuo il commit, se c'è errore allora viene eseguito il rollback
	tx.Commit()
	doneChan <- true
}

func WriteArticoli(articoli []models.Articolo, doneChan chan bool, errChan chan error) {
	qry :=
		`INSERT INTO "VARTICOLIMOVIMENTATI" ("CodiceArticolo", "DescrizioneArticolo")
		VALUES %s
		ON CONFLICT ("CodiceArticolo")
		DO UPDATE SET "DescrizioneArticolo" = EXCLUDED."DescrizioneArticolo"
    `
	const batchSize = 100
	tx, err := db.DB_SUPABASE.Begin()
	if err != nil {
		errChan <- err
		return
	}
	defer tx.Rollback()
	for start := 0; start < len(articoli); start += batchSize {
		end := start + batchSize
		if end > len(articoli) {
			end = len(articoli)
		}
		batch := articoli[start:end]
		var valueStrings []string
		var valueArgs []interface{}
		for i, articolo := range batch {
			p1 := i*2 + 1
			p2 := i*2 + 2
			valueStrings = append(valueStrings, fmt.Sprintf("($%d,$%d)", p1, p2))
			valueArgs = append(valueArgs, articolo.Codice, articolo.Descrizione)
		}
		qry := fmt.Sprintf(qry, strings.Join(valueStrings, ","))
		_, err := tx.Exec(qry, valueArgs...)
		if err != nil {
			errChan <- err
			return
		}

	}
	tx.Commit()
	doneChan <- true
}

func DeleteFattureClienti(lastUpdate time.Time, doneChan chan bool, errChan chan error) {
	//Cancello tutte le fatture che possono aver subito una qualche modifica
	lastUpdate = lastUpdate.AddDate(0, 0, -12)
	qry := `Delete From "VFATTURECLIENTI" WHERE "DataFattura" >= ($1)`
	_, err := db.DB_SUPABASE.Exec(qry, lastUpdate)
	if err != nil {
		errChan <- err
	}
	doneChan <- true
}
func DeleteFattureFornitori(lastUpdate time.Time, doneChan chan bool, errChan chan error) {
	//Cancello tutte le fatture che possono aver subito una qualche modifica
	lastUpdate = lastUpdate.AddDate(0, 0, -12)
	qry := `Delete From "VFATTUREFORNITORI" WHERE "DataFattura" >= ($1)`
	_, err := db.DB_SUPABASE.Exec(qry, lastUpdate)
	if err != nil {
		errChan <- err
	}
	doneChan <- true
}

func WriteFatture[T models.DBWritableDocumento](fatture []T, doneChan chan bool, errChan chan error) {

	const batchSize = 100
	//Avvio una singola transizione per l'insert in batch
	tx, err := db.DB_SUPABASE.Begin()
	if err != nil {
		errChan <- err
		return
	}
	//Effettuo il rollback in caso di errore
	defer tx.Rollback()
	//Non faccio fatture[0] per il semplice fatto che l'array può essere vuoto
	var myType T
	var myQry string
	switch any(myType).(type) {
	case models.FatturaCliente:
		myQry = `
			INSERT INTO "VFATTURECLIENTI" ("CodiceCliente", "DataFattura", "NumeroFattura", "CodiceArticolo",
			"QtaMovimento", "Prezzo", "ScontoPerc", "Importo","ImportoIvato",
			"ImportoIva", "AliquotaIva")
			VALUES %s
			`
	case models.FatturaFornitore:
		myQry = `
		    INSERT INTO "VFATTUREFORNITORI" ("CodiceFornitore", "DataFattura", "NumeroFattura", "CodiceArticolo",
			"QtaMovimento", "Prezzo", "ScontoPerc", "Importo","ImportoIvato",
			"ImportoIva", "AliquotaIva")
			VALUES %s
			`
	}

	for start := 0; start < len(fatture); start += batchSize {
		end := start + batchSize
		if end > len(fatture) {
			end = len(fatture)
		}
		//Effettuo lo slice degli elementi
		batch := fatture[start:end]
		//utilizzo questi array per completare la qry
		var valueStrings []string
		var valueArgs []interface{}

		for i, fattura := range batch {
			p1 := i*11 + 1
			p2 := i*11 + 2
			p3 := i*11 + 3
			p4 := i*11 + 4
			p5 := i*11 + 5
			p6 := i*11 + 6
			p7 := i*11 + 7
			p8 := i*11 + 8
			p9 := i*11 + 9
			p10 := i*11 + 10
			p11 := i*11 + 11
			//Con questo creo la stringa da aggiungere alla qry
			valueStrings = append(valueStrings, fmt.Sprintf("($%d,$%d,$%d,$%d,$%d,$%d,$%d,$%d,$%d,$%d, $%d)",
				p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11))
			valueArgs = append(valueArgs, fattura.GetCodiceControparte(), fattura.GetDataDocumento(), fattura.GetNumeroDocumento(),
				fattura.GetCodiceArticolo(), fattura.GetQtaMovimento(), fattura.GetPrezzo(), fattura.GetScontoPerc(), fattura.GetImporto(),
				fattura.GetImportoIvato(), fattura.GetImportoIva(), fattura.GetAliquotaIva())
		}

		query := fmt.Sprintf(myQry, strings.Join(valueStrings, ","))

		_, err := tx.Exec(query, valueArgs...)
		if err != nil {
			errChan <- err
			return
		}
	}
	tx.Commit()
	doneChan <- true
}

func WriteLastUpdate() error {

	qry := `INSERT INTO "VLASTUPDATE" DEFAULT VALUES`
	_, err := db.DB_SUPABASE.Exec(qry)
	if err != nil {
		return err
	}
	return nil
}

func GetLastUpdate() (time.Time, error) {
	var createdAt time.Time

	qry := `SELECT created_at FROM "VLASTUPDATE" ORDER BY id DESC LIMIT 1`
	err := db.DB_SUPABASE.QueryRow(qry).Scan(&createdAt)
	if err != nil {
		if err == sql.ErrNoRows {
			//Se vuota ritorna 01/01/2000
			createdAt = time.Date(2000, 1, 1, 0, 0, 0, 0, time.UTC)
			return createdAt, nil
		}
		return createdAt, err
	}
	return createdAt, nil
}
