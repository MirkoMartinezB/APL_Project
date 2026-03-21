package models

/*
Interfaccia che mi permette di scrivere tramite un'unico metodo sia i clienti che i fornitori
*/
type DBWritableAnagrafica interface {
	Cliente | Fornitore
	GetCodice() int64
	GetRagioneSociale() string
}

type anagrafica struct {
	Codice         int64
	RagioneSociale string
}
