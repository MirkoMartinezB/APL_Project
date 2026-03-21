package main

import (
	"auth/db"
	"auth/route"

	"github.com/gin-gonic/gin"
)

//La procedura permette la registrazione di un nuovo utente
//

func main() {
	db.InitDB()
	server := gin.Default() //configurazione HTTP Server default
	route.Routes(server)
	server.Run(":8080") //localhost:8080
}
