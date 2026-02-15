namespace SupabaseApiCall.Contracts.Anagrafiche
{
    /* La classe astratta Controparte rappresenta una controparte generica, che può essere un cliente o un fornitore.
     * Contiene le proprietà comuni a entrambe le entità, come il codice e la ragione sociale.
     * Viene utilizzata come base per le classi ClienteResponse e FornitoreResponse.
     * Tipicamente l'oggetto conterrebbe altre informazioni anagrafiche (Indirizzo, Città, Metodo di pagamento etc), 
     * ma per gli scopi attuali è sufficiente lasciare la RagioneSociale
     */
    public abstract class Controparte
    {
        public long Codice { get; set; }
        public string? RagioneSociale { get; set; }
    }
}
