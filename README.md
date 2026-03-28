# APL Project
## Overview

Il progetto in questione ha uno scopo puramente didattico. Le scelte implementative sono state effettuate cercando di rispettare i principali principi dell’ingegneria del software, come l’Open/Closed Principle, il Single Responsibility Principle e l’utilizzo di pattern GoF e principi GRASP.
Alcune decisioni progettuali sono state guidate dalla volontà di analizzare in modo pratico specifiche caratteristiche dei linguaggi utilizzati.
Durante la fase di progettazione ho cercato di assegnare a ciascun linguaggio un ruolo ben definito, con l’obiettivo di evidenziare esempi concreti di interfacciamento con database locali e remoti, modularità e comunicazione tra le diverse componenti del sistema.

## Use-case

Il sistema può essere utilizzato per:

- Monitorare il fatturato dei clienti nel tempo
- Analizzare i prodotti acquistati e venduti nel tempo
- Comparare i prezzi 
- Accedere ai dati da remoto in maniera sicura

## Ruoli
### Go → Data synchronization e authentication
### C# → API layer e business logic
### Python → Data visualization dashboard
### C++ → High-performance data aggregation




## Architettura

Il sistema è composto da quattro componenti fondamentali:

### Data Integration Layer (Go)
Utilizzato per sincronizzare i dati da un database locale (MSSQL) ad uno remoto (PostgreSQL)
### Authentication Service (Go)
Gestisce l'autenticazione, generando un JWT, gli utenti sono registrati in un database locale (SQLite)
### API Layer (C#)
Espone di endpoint, utilizza LINQ per costruire le risposte, valida i token JWT
### Presentation Layer (Python - Dash)
Permette di effettuare l'accesso, e fornisce dashboard interattive che permettono di visualizzare i dati tramite tabelle o grafici
#### Native Module (C++)
Realizza un'aggregazione di alcuni dati mediante l'uso di strutture piatte e la chiamata nativa tramite P/Invoke da C#

## Prerequisiti
.NET SDK
Go
Python 3.x
CMake
Access to:
SQL Server 17.0.x
Supabase 

## How to Run the App
1. Start Authentication Service  
  cd Go/auth  
  go run main.go

2. Start Data Synchronization  
    cd Go/Sincronize_Data  
    go run main.go

3. Start API Layer  
    cd CS\SupabaseApiCall\SupabaseApiCall  
    dotnet run

4. Start Dashboard  
     cd Python/ViewData  
     pip install -r requirements.txt  
     python app.py  

## How to Synchronize Data
1. Start Sincronization Service  
    cd Go/Sincronize_Data  
    go run main.go  
