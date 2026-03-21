package models

import (
	"database/sql"
	"time"
)

type FatturaFornitore struct {
	documento
}

func (ff FatturaFornitore) GetCodiceControparte() int64 {
	return ff.CodiceControparte
}
func (ff FatturaFornitore) GetDataDocumento() time.Time {
	return ff.DataDocumento
}
func (ff FatturaFornitore) GetNumeroDocumento() int64 {
	return ff.NumeroDocumento
}

func (ff FatturaFornitore) GetCodiceArticolo() string {
	return ff.CodiceArticolo
}
func (ff FatturaFornitore) GetQtaMovimento() float64 {
	return ff.QtaMovimento
}
func (ff FatturaFornitore) GetPrezzo() float64 {
	return ff.Prezzo
}
func (ff FatturaFornitore) GetScontoPerc() float64 {
	return ff.ScontoPerc
}
func (ff FatturaFornitore) GetImporto() float64 {
	return ff.Importo
}
func (ff FatturaFornitore) GetImportoIvato() float64 {
	return ff.ImportoIvato
}
func (ff FatturaFornitore) GetImportoIva() float64 {
	return ff.ImportoIva
}
func (ff FatturaFornitore) GetAliquotaIva() sql.NullFloat64 {
	return ff.AliquotaIva
}
