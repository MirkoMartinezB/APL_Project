using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SupabaseApiCall.Contracts.Anagrafiche;
using SupabaseApiCall.Contracts.DatiIntermedi;
using SupabaseApiCall.Contracts.CruscottiFinali;
using SupabaseApiCall.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using static Supabase.Postgrest.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var url = builder.Configuration["Supabase:Url"];
var key = builder.Configuration["Supabase:key"];

var options = new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
};
var supabase = new Supabase.Client(url, key, options);
await supabase.InitializeAsync();

Console.WriteLine("Connessione a Supabase riuscita!");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

/*
app.MapGet("/cliente/{CodiceCliente}", async (long CodiceCliente) =>
{

    var response = await supabase
        .From<Cliente>()
        .Where(n => n.CodiceCliente == CodiceCliente)
        .Get();

    var cliente = response.Models.FirstOrDefault();

    if (cliente is null)
    {
        return Results.NotFound();
    }

    var clienteResponse = new ClienteResponse
    {
        CodiceCliente = cliente.CodiceCliente,
        RagioneSociale = cliente.RagioneSociale
    };

    return Results.Ok(clienteResponse);
});
*/



app.MapGet("/clienti/", async () =>
{
    var response = await supabase
        .From<Cliente>()
        .Get();


    var listaClienteResponse = response.Models
        .Select(c => new ClienteResponse
        {
            Codice = c.CodiceCliente,
            RagioneSociale = c.RagioneSociale
        })
        .ToList();

    return Results.Ok(listaClienteResponse);
});



/*

app.MapGet("/articoli/", async () =>
{
    var response = await supabase
        .From<Articolo>()
        .Get();


    var listaArticoloResponse = response.Models
        .Select(a => new ArticoloResponse
        {
            CodiceArticolo = a.CodiceArticolo,
            DescrizioneArticolo = a.DescrizioneArticolo
        })
        .ToList();

    return Results.Ok(listaArticoloResponse);
});

app.MapGet("/articoli/{CodiceArticolo}", async (string CodiceArticolo) =>
{

    var response = await supabase
        .From<Articolo>()
        .Where(n => n.CodiceArticolo == CodiceArticolo)
        .Get();


    var articolo = response.Models.FirstOrDefault();

    if (articolo is null)
    {
        return Results.NotFound();
    }

    var articoloResponse = new ArticoloResponse
    {
        CodiceArticolo = articolo.CodiceArticolo,
        DescrizioneArticolo = articolo.DescrizioneArticolo
    };

    return Results.Ok(articoloResponse);
});


app.MapGet("/fatture/{CodiceCliente}", async (long CodiceCliente) =>
{
   

    var response = await supabase
        .From<FatturaCliente>()
        .Where(n => n.CodiceCliente == CodiceCliente)
        .Get();


    var listaFattureClienteResponse = response.Models
        .Select(f => new FatturaClienteResponse
        {
            CodiceCliente = f.CodiceCliente,
            DataFattura = f.DataFattura,
            NumeroFattura = f.NumeroFattura,
            CodiceArticolo = f.CodiceArticolo,
            QtaMovimento = f.QtaMovimento,
            Prezzo = f.Prezzo,
            ScontoPerc = f.ScontoPerc,
            Importo = f.Importo,
            ImportoIvato = f.ImportoIvato,
            ImportoIva = f.ImportoIva,
            AliquotaIva = f.AliquotaIva

        })
        .ToList();

    return Results.Ok(listaFattureClienteResponse);
});


app.MapGet("/fatture/", async () =>
{
    var response = await supabase
        .From<FatturaCliente>()
        .Get();


    var listaFattureClienteResponse = response.Models
        .Select(f => new FatturaClienteResponse
        {
            CodiceCliente = f.CodiceCliente,
            DataFattura = f.DataFattura,
            NumeroFattura = f.NumeroFattura,
            CodiceArticolo = f.CodiceArticolo,
            QtaMovimento = f.QtaMovimento,
            Prezzo = f.Prezzo,
            ScontoPerc = f.ScontoPerc,
            Importo = f.Importo,
            ImportoIvato = f.ImportoIvato,
            ImportoIva = f.ImportoIva,
            AliquotaIva = f.AliquotaIva

        })
        .ToList();

    return Results.Ok(listaFattureClienteResponse);
});


*/





app.MapGet("/vdaticli/{CodiceCliente}", async (long CodiceCliente) =>
{

    var cruscottoClienteBuilder = new CruscottoClienteResponse.CruscottoClienteResponseBuilder();


    var responseCliente = await supabase
        .From<Cliente>()
        .Where(c => c.CodiceCliente == CodiceCliente)
        .Get();

    var cliente = responseCliente.Models.FirstOrDefault();

    if (cliente is null)
    {
        return Results.NotFound();
    }

    var clienteResponse = new ClienteResponse
    {
        Codice = cliente.CodiceCliente,
        RagioneSociale = cliente.RagioneSociale
    };


    cruscottoClienteBuilder.setCliente(clienteResponse);


    //Creo una mappa degli Articoli per ricostruire l'oggetto presente nelle fatture 
    var dicArticoli = new Dictionary<string, ArticoloResponse>();

    var responseArticolo = await supabase
        .From<Articolo>()
        .Get();

    foreach (var articolo in responseArticolo.Models)
    {
        if(articolo.CodiceArticolo == null)
            continue;
        dicArticoli[articolo.CodiceArticolo] = new ArticoloResponse
        {
            CodiceArticolo = articolo.CodiceArticolo,
            DescrizioneArticolo = articolo.DescrizioneArticolo
        };
    }

    var dicArticoliMovimentati = new Dictionary<string, ArticoloResponse>();


    var responseFatture = await supabase
        .From<FatturaCliente>()
        .Where(n => n.CodiceCliente == CodiceCliente)
        .Get();

    var listaFattureClienteResponse = new List<FatturaClienteResponse>();


    var dicFatture = new Dictionary<(int anno, long numero), FatturaClienteResponse>();
    foreach (var fattura in responseFatture.Models)
    {
        
        var key = (fattura.DataFattura.Year, fattura.NumeroFattura);

        if (!dicFatture.TryGetValue(key, out var fatturaClienteResponse))
        {
            fatturaClienteResponse = new FatturaClienteResponse(
                clienteResponse,
                fattura.DataFattura,
                fattura.NumeroFattura
            );

            dicFatture.Add(key, fatturaClienteResponse);
            listaFattureClienteResponse.Add(fatturaClienteResponse);
        }
        fatturaClienteResponse.TotaleFattura += fattura.ImportoIvato;
        fatturaClienteResponse.TotaleImporto += fattura.Importo;
        fatturaClienteResponse.AggiungiCorpo(
            dicArticoli[fattura.CodiceArticolo],
            fattura.QtaMovimento,
            fattura.Prezzo,
            fattura.ScontoPerc,
            fattura.Importo,
            fattura.ImportoIvato,
            fattura.ImportoIva,
            fattura.AliquotaIva
        );
        if (!dicArticoliMovimentati.ContainsKey(fattura.CodiceArticolo)){
            dicArticoliMovimentati.Add(fattura.CodiceArticolo, dicArticoli[fattura.CodiceArticolo]);
        }
    }
    cruscottoClienteBuilder.setFattureCliente(listaFattureClienteResponse);

    cruscottoClienteBuilder.setArticoliMovimentati(dicArticoliMovimentati);

    //System.Console.WriteLine($"Articoli movimentati: {dicArticoliMovimentati.Count}");  

    var responseFattureFornitori = await supabase
       .From<FatturaFornitore>()
       .Filter("CodiceArticolo", Operator.In, dicArticoliMovimentati.Keys.ToList())
       .Get();






    var listaFattureFornitoreResponse = new List<FatturaFornitoreResponse>();
    //Aggiungo il codiceFornitore dato che le fatture sono quelle ricevute e quindi anno/numero non sono sufficienti a generare una chiave univoca
    var dicFattureFor = new Dictionary<(int anno, long numero, long codiceFornitore), FatturaFornitoreResponse>();
    foreach (var fattura in responseFattureFornitori.Models)
    {

        var key = (fattura.DataFattura.Year, fattura.NumeroFattura, fattura.CodiceFornitore);

        if (!dicFattureFor.TryGetValue(key, out var fatturaFornitoreResponse))
        {
            fatturaFornitoreResponse = new FatturaFornitoreResponse(
                null, //non mi interessa ricostruire il fornitore in questo contesto
                fattura.DataFattura,
                fattura.NumeroFattura
            );

            dicFattureFor.Add(key, fatturaFornitoreResponse);
            listaFattureFornitoreResponse.Add(fatturaFornitoreResponse);
        }
        fatturaFornitoreResponse.TotaleFattura += fattura.ImportoIvato;
        fatturaFornitoreResponse.TotaleImporto += fattura.Importo;
        fatturaFornitoreResponse.AggiungiCorpo(
            dicArticoli[fattura.CodiceArticolo],
            fattura.QtaMovimento,
            fattura.Prezzo,
            fattura.ScontoPerc,
            fattura.Importo,
            fattura.ImportoIvato,
            fattura.ImportoIva,
            fattura.AliquotaIva
        );
    }

    CruscottoClienteResponse Response = cruscottoClienteBuilder.Build();

    




    return Results.Ok(Response);
});



app.UseHttpsRedirection();
app.Run();
