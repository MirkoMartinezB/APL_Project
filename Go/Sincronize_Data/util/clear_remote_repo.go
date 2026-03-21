package util

import (
	"Sincronize_Data/db"
	"fmt"
	"log"
)

func ClearRemoteRepo() {
	qry := `DELETE FROM "VFATTURECLIENTI"`
	_, err := db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Fatture Clienti Cancellate")
	qry = `DELETE FROM "VFATTUREFORNITORI"`
	_, err = db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Fatture Fornitori Cancellate")
	qry = `DELETE FROM "VARTICOLIMOVIMENTATI"`
	_, err = db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Lista Articoli Cancellati")
	qry = `DELETE FROM "VCLIENTIMOVIMENTATI"`
	_, err = db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Clienti Cancellati")
	qry = `DELETE FROM "VFORNITORIMOVIMENTATI"`
	_, err = db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Fornitori Cancellati")
	qry = `DELETE FROM "VFORNITORIMOVIMENTATI"`
	_, err = db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("Fornitori Cancellati")
	qry = `DELETE FROM "VLASTUPDATE"`
	_, err = db.DB_SUPABASE.Exec(qry)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println("LastUpdate Cancellato")

	fmt.Println("Repo remoto svuotato con successo")

}
