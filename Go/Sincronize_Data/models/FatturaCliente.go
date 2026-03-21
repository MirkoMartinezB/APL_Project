package models

import (
	"database/sql"
	"time"
)

type FatturaCliente struct {
	documento
}

func (fc FatturaCliente) GetCodiceControparte() int64 {
	return fc.CodiceControparte
}
func (fc FatturaCliente) GetDataDocumento() time.Time {
	return fc.DataDocumento
}
func (fc FatturaCliente) GetNumeroDocumento() int64 {
	return fc.NumeroDocumento
}

func (fc FatturaCliente) GetCodiceArticolo() string {
	return fc.CodiceArticolo
}
func (fc FatturaCliente) GetQtaMovimento() float64 {
	return fc.QtaMovimento
}
func (fc FatturaCliente) GetPrezzo() float64 {
	return fc.Prezzo
}
func (fc FatturaCliente) GetScontoPerc() float64 {
	return fc.ScontoPerc
}
func (fc FatturaCliente) GetImporto() float64 {
	return fc.Importo
}
func (fc FatturaCliente) GetImportoIvato() float64 {
	return fc.ImportoIvato
}
func (fc FatturaCliente) GetImportoIva() float64 {
	return fc.ImportoIva
}
func (fc FatturaCliente) GetAliquotaIva() sql.NullFloat64 {
	return fc.AliquotaIva
}
