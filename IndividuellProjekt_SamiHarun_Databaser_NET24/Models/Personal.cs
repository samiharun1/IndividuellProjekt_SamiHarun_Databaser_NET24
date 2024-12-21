using System;
using System.Collections.Generic;

namespace IndividuellProjekt_SamiHarun_Databaser_NET24.Models;

public partial class Personal
{
    public int Id { get; set; }

    public string? Namn { get; set; }

    public string? Befattning { get; set; }

    public DateOnly? Anstallningsdatum { get; set; }

    public string? Avdelning { get; set; }

    public decimal? Lon { get; set; }

    public virtual ICollection<Betyg> Betygs { get; set; } = new List<Betyg>();
}
