package models

type Cliente struct {
	anagrafica
}

func (c Cliente) GetCodice() int64 {
	return c.Codice
}
func (c Cliente) GetRagioneSociale() string {
	return c.RagioneSociale
}
