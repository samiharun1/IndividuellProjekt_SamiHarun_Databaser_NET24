using System;
using System.Collections.Generic;

namespace IndividuellProjekt_SamiHarun_Databaser_NET24.Models;

public partial class Kurser
{
    public int Id { get; set; }

    public string? Kursnamn { get; set; }

    public virtual ICollection<Betyg> Betygs { get; set; } = new List<Betyg>();
}
