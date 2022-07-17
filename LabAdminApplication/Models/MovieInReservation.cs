using System.Collections.Generic;

namespace LabAdminApplication.Models
{
    public class MovieInReservation
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public int Quantity { get; set; }
    }
}
