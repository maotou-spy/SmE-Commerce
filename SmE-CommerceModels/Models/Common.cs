namespace SmE_CommerceModels.Models
{
    public class Common
    {
        public DateTime? CreatedAt { get; set; }

        public Guid? CreateById { get; set; }

        public User? CreateUser { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public Guid? ModifiedById { get; set; }

        public User? ModifiedByUser { get; set; }
    }
}
