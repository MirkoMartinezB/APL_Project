cmake_minimum_required(VERSION 4.0)
project(AggregateInvoicesByProductYearMonth)

set(CMAKE_CXX_STANDARD 20)

set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS OFF)

set(CMAKE_SHARED_LINKER_FLAGS
        "${CMAKE_SHARED_LINKER_FLAGS} -static-libstdc++ -static-libgcc"
)


add_library(AggregateInvoicesByProductYearMonth SHARED
        AggregateInvocesByProductYearMonth.cpp
        AggregateInvocesByProductYearMonth.cpp
)

target_link_options(AggregateInvoicesByProductYearMonth PRIVATE
        -static-libstdc++
        -static-libgcc
        -Wl,-Bstatic -lwinpthread -Wl,-Bdynamic
)



target_include_directories(AggregateInvoicesByProductYearMonth PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/include)

# Opzionale: definizione per gestire __declspec(dllexport/dllimport)
target_compile_definitions(AggregateInvoicesByProductYearMonth PRIVATE CalculateMeanFromInvoices_EXPORTS)

