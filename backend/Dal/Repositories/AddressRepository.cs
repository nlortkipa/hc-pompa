using Dal.Interfaces;
using Data;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class AddressRepository(ProjectContext context) : BaseRepository<Address>(context), IAddressRepository
    {
    }
}
