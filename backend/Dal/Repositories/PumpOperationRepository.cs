using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class PumpOperationRepository(ProjectContext context) : BaseRepository<PumpOperation>(context), IPumpOperationRepository
    {
    }
}
