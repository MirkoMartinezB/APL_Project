package main

import (
	"Sincronize_Data/db"
	"Sincronize_Data/models"
	"Sincronize_Data/repository"
	"fmt"
)

func main() {
	//Inizializzazione e Test connesione DB locale e remoto
	err := db.InitDB()
	if err != nil {
		panic(err)
	}
	err = db.InitDB_Supabase()
	if err != nil {
		panic(err)
	}
	//Per svuotare il repo remoto da utilizzare nei test
	//util.ClearRemoteRepo()

	//Definizione canali e variabili di ritorno
	var clienti []models.Cliente
	var fornitori []models.Fornitore
	var articoli []models.Articolo
	var fattureClienti []models.FatturaCliente
	var fattureFornitori []models.FatturaFornitore

	chClienti := make(chan []models.Cliente)
	errClienti := make(chan error)
	chFornitori := make(chan []models.Fornitore)
	errFornitore := make(chan error)
	chArticoli := make(chan []models.Articolo)
	errArticoli := make(chan error)
	chFattureClienti := make(chan []models.FatturaCliente)
	errFattureClienti := make(chan error)
	chFattureFornitori := make(chan []models.FatturaFornitore)
	errFattureFornitori := make(chan error)

	//Procedure di GET
	lastUpdate, err := repository.GetLastUpdate()
	if err != nil {
		panic(err)
	}
	fmt.Println("Last update: ", lastUpdate)
	go repository.GetClienti(lastUpdate, chClienti, errClienti)
	go repository.GetFornitori(lastUpdate, chFornitori, errFornitore)
	go repository.GetArticoli(lastUpdate, chArticoli, errArticoli)
	go repository.GetFattureClienti(lastUpdate, chFattureClienti, errFattureClienti)
	go repository.GetFattureFornitori(lastUpdate, chFattureFornitori, errFattureFornitori)
	//Prima di lanciare le scritture (POST) aspettiamo i risultati
	select {
	case err := <-errClienti:
		if err != nil {
			panic(err)
		}
	case clienti = <-chClienti:
	}
	select {
	case err := <-errFornitore:
		if err != nil {
			panic(err)
		}
	case fornitori = <-chFornitori:
	}
	select {
	case err := <-errArticoli:
		if err != nil {
			panic(err)
		}
	case articoli = <-chArticoli:
	}
	select {
	case err := <-errFattureClienti:
		if err != nil {
			panic(err)
		}
	case fattureClienti = <-chFattureClienti:
	}
	select {
	case err := <-errFattureFornitori:
		if err != nil {
			panic(err)
		}
	case fattureFornitori = <-chFattureFornitori:
	}
	numberOfChannels := 5
	doneChan := make([]chan bool, numberOfChannels)
	errChan := make([]chan error, numberOfChannels)
	for i := 0; i < numberOfChannels; i++ {
		doneChan[i] = make(chan bool)
		errChan[i] = make(chan error)
	}
	//Post Data
	go repository.WriteAnagrafica(clienti, doneChan[0], errChan[0])
	go repository.WriteAnagrafica(fornitori, doneChan[1], errChan[1])
	go repository.WriteArticoli(articoli, doneChan[2], errChan[2])
	go repository.DeleteFattureClienti(lastUpdate, doneChan[3], errChan[3])
	go repository.DeleteFattureFornitori(lastUpdate, doneChan[4], errChan[4])
	for i := 0; i < numberOfChannels; i++ {
		select {
		case err := <-errChan[i]:
			if err != nil {
				panic(err)
			}
		case <-doneChan[i]:
		}
	}
	//Adesso i vincoli di integrità sono rispettati quindi è possibile scrivere le fatture
	numberOfChannels = 2
	doneChan = make([]chan bool, numberOfChannels)
	errChan = make([]chan error, numberOfChannels)
	for i := 0; i < numberOfChannels; i++ {
		doneChan[i] = make(chan bool)
		errChan[i] = make(chan error)
	}
	go repository.WriteFatture(fattureClienti, doneChan[0], errChan[0])
	go repository.WriteFatture(fattureFornitori, doneChan[1], errChan[1])
	for i := 0; i < numberOfChannels; i++ {
		select {
		case err := <-errChan[i]:
			if err != nil {
				panic(err)
			}
		case <-doneChan[i]:
		}
	}
	//Chiudiamo aggiornando la data nel repository
	err = repository.WriteLastUpdate()
	if err != nil {
		panic(err)
	}
}
