using System.Collections.Generic;

namespace LabAdminApplication.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public LapApplicationUser ReservedBy { get; set; }
        public List<MovieInReservation> Movies { get; set; }

    }
}
