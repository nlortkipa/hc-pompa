using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Service.DTOs
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CreateAddressDTO
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateAddressDTO
    {
        public string Name { get; set; } = null!;
    }
}
