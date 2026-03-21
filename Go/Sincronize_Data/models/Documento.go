package models

import (
	"database/sql"
	"time"
)

type documento struct {
	CodiceControparte int64
	DataDocumento     time.Time
	NumeroDocumento   int64
	CodiceArticolo    string
	QtaMovimento      float64
	Prezzo            float64
	ScontoPerc        float64
	Importo           float64
	ImportoIvato      float64
	ImportoIva        float64
	AliquotaIva       sql.NullFloat64
}

type DBWritableDocumento interface {
	FatturaCliente | FatturaFornitore
	GetCodiceControparte() int64
	GetDataDocumento() time.Time
	GetNumeroDocumento() int64
	GetCodiceArticolo() string
	GetQtaMovimento() float64
	GetPrezzo() float64
	GetScontoPerc() float64
	GetImporto() float64
	GetImportoIvato() float64
	GetImportoIva() float64
	GetAliquotaIva() sql.NullFloat64
}
