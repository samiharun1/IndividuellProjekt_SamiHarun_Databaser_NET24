using System;
using System.Collections.Generic;

namespace IndividuellProjekt_SamiHarun_Databaser_NET24.Models;

public partial class Studenter
{
    public int Id { get; set; }

    public string? Namn { get; set; }

    public string? Personnummer { get; set; }

    public string? Klass { get; set; }

    public virtual ICollection<Betyg> Betygs { get; set; } = new List<Betyg>();
}
