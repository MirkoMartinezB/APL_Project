using System.Runtime.InteropServices;

namespace SupabaseApiCall.Contracts.DatiIntermedi
{
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)] //Definisce il layout della struttura in memoria per l'interoperabilità con codice nativo C++
    /*  Struttura per il parsing delle fatture in C++
     * Utilizzata per passare i dati delle fatture alla funzione C++ che calcola i prezzi medi.
     * Contiene le informazioni essenziali per il calcolo: Anno, Mese, CodiceArticolo, QtaMovimento e Importo.
     * La struttura deve essere definita in modo compatibile con la struttura C++ corrispondente.
     */
    public struct FatturaParseCpp
    {

        public int Anno;
        public int Mese;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25)] //Massima dimensione CodiceArticolo pari a 25 caratteri
        
        public String CodiceArticolo;
        public double QtaMovimento; //Quantità acquistata/venduta nel periodo
        public double Importo; //Importo totale (senza IVA) per la quantità acquistata/venduta nel periodo

    }
}
