package models

import (
	"auth/db"
	"auth/utils"
	"errors"
	"time"
)

type User struct {
	ID               int64
	Email            string `binding:"required"`
	Password         string `binding:"required"`
	RegistrationDate time.Time
	LastLogin        time.Time
	AuthorizedUser   bool
}

func (user User) SaveUser() error {
	qry := "INSERT INTO users (email, password, registrationDate) VALUES (?, ?, ?)"
	statment, err := db.DB.Prepare(qry)
	if err != nil {
		return err
	}
	defer statment.Close()
	hashedPassword, err := utils.HashPassword(user.Password)
	if err != nil {
		return err
	}
	result, err := statment.Exec(user.Email, hashedPassword, time.Now())
	if err != nil {
		return err
	}
	user.ID, err = result.LastInsertId()
	return err
}

func (user User) ValidateCredential() error {
	qry := "SELECT email, id, password, authorizedUser FROM users WHERE email=?"
	row := db.DB.QueryRow(qry, user.Email)
	var hashedPassword string
	err := row.Scan(&user.Email, &user.ID, &hashedPassword, &user.AuthorizedUser)

	if err != nil {
		return errors.New("Credenziali non valide 1")
	}

	credentialsAreValid := utils.CheckPasswordHash(user.Password, hashedPassword)
	if !credentialsAreValid {
		return errors.New("Credenziali non valide")
	}
	if !user.AuthorizedUser {
		return errors.New("Utente non autorizzato")
	}
	err = user.UpdateLastLogin()
	if err != nil {
		return err
	}
	return nil
}

func (user User) UpdateLastLogin() error {
	query := "UPDATE users SET lastlogin = ?	WHERE id = ?"
	stmt, err := db.DB.Prepare(query)
	if err != nil {
		return err
	}
	defer stmt.Close()
	_, err = stmt.Exec(time.Now(), user.ID)
	return err
}

func (user User) UpdateAuthUserStatus(status bool) error {
	query := "UPDATE users SET authorizedUser = ?	WHERE id = ?"
	_, err := db.DB.Exec(query, status, user.ID)
	return err
}
