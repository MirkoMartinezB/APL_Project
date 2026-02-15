using SupabaseApiCall.Repositories;
using SupabaseApiCall.Services;
using SupabaseApiCall.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Tramite questa lambda vado a configurare il client Supabase che sarà un Singleton nell'app
// Di volta in volta questo viene iniettato nei repository/servizi che ne hanno bisogno
builder.Services.AddSingleton(supabaseService =>
{
    var url = builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Url errato o assente");
    var key = builder.Configuration["Supabase:Key"] ?? throw new InvalidOperationException("Key errata o assente");
    var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
    return new Supabase.Client(url, key, options);
});

// Init async all'avvio 
builder.Services.AddHostedService<SupabaseInitializer>();

// App layers
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IArticoloRepository, ArticoloRepository>();
builder.Services.AddScoped<IFatturaClienteRepository, FatturaClienteRepository>();
builder.Services.AddScoped<IFatturaFornitoreRepository, FatturaFornitoreRepository>();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IArticoloService, ArticoloService >();
builder.Services.AddScoped<IFattureService, FattureService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/error");
}


app.MapControllers();
app.Run();






/*

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


    var dicFatturatoAnnoMese = new Dictionary<string, double>();


    var responseFatture = await supabase
        .From<FatturaCliente>()
        .Where(n => n.CodiceCliente == CodiceCliente)
        .Get();

    var listaFattureClienteResponse = new List<FatturaClienteResponse>();

    var listaFattureCpp = new List<FatturaParseCpp>();


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

        if (!dicArticoliMovimentati.ContainsKey(fattura.CodiceArticolo))
        {
            dicArticoliMovimentati.Add(fattura.CodiceArticolo, dicArticoli[fattura.CodiceArticolo]);
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
        var keyAnnoMese = fattura.DataFattura.Year.ToString() + "_" + fattura.DataFattura.Month.ToString();

        if (!dicFatturatoAnnoMese.ContainsKey(keyAnnoMese))
        {
            dicFatturatoAnnoMese.Add(keyAnnoMese, 0);
        }
        dicFatturatoAnnoMese[keyAnnoMese] += fattura.ImportoIvato;

        

        //Popolo la lista per il calcolo C++
        listaFattureCpp.Add(new FatturaParseCpp
        {
            Anno = fattura.DataFattura.Year,
            Mese = fattura.DataFattura.Month,
            CodiceArticolo = fattura.CodiceArticolo!,
            QtaMovimento = fattura.QtaMovimento,
            Importo = fattura.Importo
        });     


    }
    //Parsing delle fatture Clienti a C++ per calcolare il prezzo medio di vendita per articolo x mese
    //trasformo in array per passarlo a C++
    var arrayFattureCpp = listaFattureCpp.ToArray();
    var arrayFattureResponseTmpCpp = new FatturaParseCpp[arrayFattureCpp.Length]; //array di risposta, dimensione massima come quella precendente ma sarà modificata

    nuint returnedSize = 0;

    


    test.CalculateMeanFromInvoices(arrayFattureCpp, (nuint)arrayFattureCpp.Length, arrayFattureResponseTmpCpp, out returnedSize);

    int sizeFinale = checked((int)returnedSize);

    //Slice reale dell'array di risposta
    var arrayFattureResponseCpp = arrayFattureResponseTmpCpp[0..sizeFinale];
    

    cruscottoClienteBuilder.setFattureCliente(listaFattureClienteResponse);

    cruscottoClienteBuilder.setArticoliMovimentati(dicArticoliMovimentati);

    cruscottoClienteBuilder.setFatturatoAnnoMese(dicFatturatoAnnoMese);

    
    


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

*/

//app.UseHttpsRedirection();
//app.Run();
