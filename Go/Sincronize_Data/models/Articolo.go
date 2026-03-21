package models

type Articolo struct {
	Codice      string
	Descrizione string
}

func NewArticolo(code string, descrizione string) Articolo {
	return Articolo{
		Codice:      code,
		Descrizione: descrizione,
	}
}
