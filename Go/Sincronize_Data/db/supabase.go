package db

import (
	"database/sql"
	"errors"
	"os"

	_ "github.com/jackc/pgx/v5/stdlib"
	"github.com/joho/godotenv"
)

var DB_SUPABASE *sql.DB

func InitDB_Supabase() error {
	err := godotenv.Load()
	if err != nil {
		errors.New("File .env con connessione non trovato")
	}
	SUPABASE_URL := os.Getenv("SUPABASE_DIRECT_URL")

	DB_SUPABASE, err = sql.Open("pgx", SUPABASE_URL)
	if err != nil {
		//panic(err)
		//RETE SOLO IPV4 DEVO UTILIZZARE L'ALTRO TIPO
		SUPABASE_URL = os.Getenv("SUPABASE_DIRECT_URL_IPV4")
		DB_SUPABASE, err = sql.Open("pgx", SUPABASE_URL)
		if err != nil {
			return (err)
		}
	}
	DB_SUPABASE.SetMaxOpenConns(10)
	DB_SUPABASE.SetMaxIdleConns(5)
	return nil
}
