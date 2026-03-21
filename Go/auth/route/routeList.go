package route

import (
	"github.com/gin-gonic/gin"
)

func Routes(server *gin.Engine) {
	server.POST("/signup", signup)
	server.POST("/login", login)
}
