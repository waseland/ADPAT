using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.lilypond
{
    enum TokenType
    {
        Note,
        Clef,
        relative,
        timeSignature,
        timeSignaturedata,
        ClefType,
        Startblok,
        EndBlok,
        Tempo,
        TempoValue,
        Maatstreep,
        Rust,
        Version,
        Header,
        Repeat,
        Number,
        Nothing
    }
}
