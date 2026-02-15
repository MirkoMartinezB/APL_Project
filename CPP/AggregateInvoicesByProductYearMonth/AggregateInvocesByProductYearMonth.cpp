#include <iostream>
#include <string>
#include <comdef.h>
#include <unordered_map>





struct FatturaParseCpp {
    int Anno;
    int Mese;
    char CodiceArticolo[25];
    double QtaMovimento;
    double Importo;

    FatturaParseCpp(int _Anno, int _Mese, const char* _CodiceArticolo, double _QtaMovimento, double _Importo) :
    Anno{_Anno}, Mese{_Mese},  QtaMovimento{_QtaMovimento}, Importo{_Importo}, CodiceArticolo{} {
        strncpy(CodiceArticolo, _CodiceArticolo, sizeof(CodiceArticolo) - 1);
        CodiceArticolo[sizeof(CodiceArticolo) - 1] = '\0';

    }
};

struct ImportoQta {
    double qta;
    double importo;
    ImportoQta() : qta{0.0}, importo{0.0} {}
    ImportoQta(double qta, double importo) : qta{qta}, importo{importo} {}
};
extern "C" __declspec(dllexport) void AggregateInvoicesByProductYearMonth(const FatturaParseCpp* fatture, size_t n,
    FatturaParseCpp* fattureResponse, size_t* responseSize);




void AggregateInvoicesByProductYearMonth(const FatturaParseCpp* fatture, size_t n,
    FatturaParseCpp* fattureResponse, size_t* responseSize) {
    //std::cout << n << std::endl;

    std::unordered_map<std::string, ImportoQta> mappaPerMedia;



    for (int i {0}; i < n; i++) {


        //std::cout << fatture[i].CodiceArticolo << std::endl;
        std::string key = std::string(fatture[i].CodiceArticolo) + "_" +
            std::to_string(fatture[i].Anno) + "_" + std::to_string(fatture[i].Mese);

        if(!mappaPerMedia.contains(key)) {

            mappaPerMedia[key] = ImportoQta(fatture[i].QtaMovimento, fatture[i].Importo);

        }else {
            ImportoQta dummy {mappaPerMedia[key]};
            mappaPerMedia[key] = ImportoQta(fatture[i].QtaMovimento + dummy.qta, fatture[i].Importo + dummy.importo);
        }

    }


    size_t i = 0;
    for (const auto& [key, value] : mappaPerMedia) {
        if (value.qta == 0)
            continue;

        char codice[25];
        int anno;
        int mese;
        //Formato codice_anno_mese, li estrae e mette nei campi corretti
        std::sscanf(key.c_str(), "%24[^_]_%d_%d", codice, &anno, &mese);
        fattureResponse[i++] = FatturaParseCpp(anno, mese, codice, value.qta, value.importo);
    }

    *responseSize = i;

}





