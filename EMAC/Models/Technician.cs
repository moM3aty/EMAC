using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMAC.Models
{
    public class Technician
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Specialty { get; set; }

        [Required]
        public string CoveredRegions { get; set; }

        public string WorkingHoursStart { get; set; } = "08:00";
        public string WorkingHoursEnd { get; set; } = "18:00";

        public bool IsAvailable(string region, string serviceType, DateTime date)
        {
            if (!CoveredRegions.Contains(region)) return false;
            if (!Specialty.Equals(serviceType, StringComparison.OrdinalIgnoreCase) && Specialty != "General") return false;

            return true;
        }
    }
}