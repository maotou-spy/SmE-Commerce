namespace SmE_CommerceModels.Objects
{
    public class Common
    {
        public DateTime? CreatedDate { get; set; }

        public uint? CreatedById { get; set; }

        public User? CreateBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public uint? ModifiedById { get; set; }

        public User? ModifiedBy { get; set; }
    }
}
