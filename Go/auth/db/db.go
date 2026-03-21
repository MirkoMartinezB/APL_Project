package db

import (
	"database/sql"
	_ "modernc.org/sqlite"
)

var DB *sql.DB

func InitDB() {
	var err error
	DB, err = sql.Open("sqlite", "users.db")
	if err != nil {
		panic(err)
	}
	DB.SetMaxIdleConns(5)
	DB.SetMaxOpenConns(10)

	createTable()
}

func createTable() {

	createUsersTable := `
    CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    registrationDate DATETIME,
	lastLogin DATETIME,
	authorizedUser BIT NOT NULL DEFAULT 1
)`
	_, err := DB.Exec(createUsersTable)
	if err != nil {
		panic(err)
	}
}
