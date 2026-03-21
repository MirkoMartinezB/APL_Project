package db

import (
	"database/sql"
	"errors"
	"os"

	_ "github.com/denisenkom/go-mssqldb"
	"github.com/joho/godotenv"
)

var DB *sql.DB

func InitDB() error {
	var err error
	//Autenticazione di windows al SQL Server (Express)
	err = godotenv.Load()
	if err != nil {
		return errors.New("File .env con connessione non trovato")
	}
	connectToSQL := os.Getenv("SQL_SERVER_CONNECTION")
	DB, err = sql.Open("sqlserver", connectToSQL)
	if err != nil {
		return err
	}
	DB.SetMaxOpenConns(10)
	DB.SetMaxIdleConns(5)
	return nil

}
