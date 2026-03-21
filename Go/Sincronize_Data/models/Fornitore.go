package models

type Fornitore struct {
	anagrafica
}

func (f Fornitore) GetCodice() int64 {
	return f.Codice
}
func (f Fornitore) GetRagioneSociale() string {
	return f.RagioneSociale
}
