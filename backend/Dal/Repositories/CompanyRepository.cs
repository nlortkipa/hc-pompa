using Dal.Interfaces;
using Data;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        private readonly ProjectContext _context;

        public CompanyRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }
    }
}
