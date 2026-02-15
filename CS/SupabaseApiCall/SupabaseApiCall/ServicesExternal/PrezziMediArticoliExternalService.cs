using SupabaseApiCall.Contracts.DatiIntermedi;
using System.Runtime.InteropServices;


namespace SupabaseApiCall.ServicesExternal
{


   
    public static class PrezziMediArticoliExternalService
    {
        //Metodo per chiamare la funzione C++ che aggrega le fatture prodotto anno-mese
        [DllImport("libAggregateInvoicesByProductYearMonth.dll")]
        /* Il metodo accetta un array di strutture FatturaParseCpp, la loro dimensione n,
         * e restituisce un array di strutture FatturaParseCpp con i prezzi medi calcolati e la loro dimensione responseSize.
         * L'attributo [In] indica che l'array fattura viene passato come input,
         * mentre l'attributo [Out] indica che l'array fatturaResponse viene popolato come output.
         * Dato che gli array sono statici inizialmente l'array fatturaResponse viene allocato della stessa dimensione di quello di input.
         * Successivamente bisogna gestire la dimensione effettiva dell'array di output tramite la variabile responseSize.
         * Quindi in C# conviene incapsulare il metodo in modo che restituisca una classica List<PrezzoMedioResponse>.
         */
        public static extern void AggregateInvoicesByProductYearMonth([In] FatturaParseCpp[] fattura, nuint n, [Out] FatturaParseCpp[] fatturaResponse, out nuint responseSize); //nuint si converte in size_t in C++




    }
}
