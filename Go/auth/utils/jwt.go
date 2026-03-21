package utils

import (
	"errors"
	"os"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"github.com/joho/godotenv"
)

func GenerateToken(email string, userId int64) (string, error) {
	token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
		"email":  email,
		"userId": userId,
		"exp":    time.Now().Add(time.Hour * 2).Unix(),
	})
	err := godotenv.Load()
	if err != nil {
		return "", errors.New("File .env non trovato")
	}
	secretKey := os.Getenv("JWT_KEY")

	return token.SignedString([]byte(secretKey))
}

//La parte di validazione del token è fatta direttamente in C#
